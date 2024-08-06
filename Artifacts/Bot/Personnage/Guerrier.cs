using Artifacts.Items;
using Artifacts.Monsters;
using Artifacts.MyCharacters;
using Artifacts.Utilities;
using Decider.Csp.BaseTypes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Artifacts.Resources.ResourcesResponse;
using static Artifacts.Utilities.Enums;

namespace Artifacts.Bot.Personnage
{
    internal class Guerrier : Personnage
    {
        public Guerrier(MCU mCU)
        {
            mcu = mCU;
            WorkOrderList = new List<WorkOrder>();
            metier = Constantes.combat;
        }
        internal override void QueFaire()
        {
            while (!termine)
            {
                
                if (IsInventairePlein() || doitViderEnBanque)
                {
                    Aller_Banque();
                    Vider_Inventaire();
                }
                if (CommandeEnAttente)
                {
                    Aller_Banque();
                    Vider_Inventaire();
                    RecupererCommande();
                    CommandeEnAttente = false;
                }

                if (NiveauMetierGagne && WorkOrderList.Count == 0)
                {
                    int niveauMetier = GetNiveauMetier();
                    //la meilleure arme
                    List<Objet> ListeBest_weapon = mcu.getObjetList().Where(x => x.type == Constantes.weapon && x.subtype != Constantes.tool && x.level <= niveauMetier).ToList();
                    Objet best_weapon = null;
                    if (ListeBest_weapon.Count > 0)
                    {
                        best_weapon = ListeBest_weapon.OrderByDescending(x => x.level).FirstOrDefault();
                    }
                    if (best_weapon != null && FeuillePerso.weapon_slot != best_weapon.code)
                    {
                        WorkOrder wo = new WorkOrder();
                        wo.Quantité = 1;
                        wo.Code = best_weapon.code;
                        wo.Demandeur = this;
                        mcu.AjouterWorkOrder(wo);
                        WorkOrderList.Add(wo);
                    }
                    NiveauMetierGagne = false;
                }

                Resource resourceARecuperer = null;
                WorkOrder wo_pris = null;
                Monster monstreATuer = null;
                List<string> stuffAEquiper = new List<string>();
                //traiter les workOrder
                foreach (WorkOrder wo in mcu.GetListeWorkorderAttenteRessource())
                {
                    if (resourceARecuperer != null && wo.ListeDesResources != null)
                    {
                        break;
                    }
                    //on va convertir le code de la ressource en Objet
                    List<Tuple<string, int>> listeTuplesCodeQuantite = wo.ListeDesResources;
                    if (listeTuplesCodeQuantite != null)
                    {
                        foreach (Tuple<string, int> t in listeTuplesCodeQuantite)
                        {
                            Monster m = mcu.MonsterList.Where(x => x.drops.Where(y => y.code == t.Item1).Any()).FirstOrDefault();
                            if (m != null)
                            {
                                List<string> resultat = mcu.CSPCombat(this, m);
                                if (resultat.Count >= 0)
                                {
                                    wo_pris = wo;
                                    monstreATuer = m;
                                    stuffAEquiper = resultat;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (wo_pris != null)
                {
                    //equiperStuff
                    EquiperStuff(stuffAEquiper);
                    //aller sur la bonne tuile
                    AllerSurTuileMonstre(monstreATuer);
                    while (wo_pris.ListeDesResources.Where(x => x.Item1 == monstreATuer.code).Count() > 0)
                    {
                        int niveau_actuel = FeuillePerso.level;
                        Character c = MyCharactersEndPoint.Fight(FeuillePerso.name);
                        if (c != null)
                        {
                            FeuillePerso = c;
                        }
                        if (FeuillePerso.level != niveau_actuel)
                        {
                            NiveauMetierGagne = true;
                        }
                        mcu.ActualiserResources();
                    }
                }
                else
                {
                    while (!NiveauMetierGagne && !IsInventairePlein() && !NouveauWorkOrder && !termine)
                    {
                        int niveauMetier = GetNiveauMetier();

                        //FarmerMeilleureRessource(metier);
                    }
                }
                
            }
        }

        internal void EquiperStuff(List<string> stuffAEquiper)
        {
            List<string> listeAnneaux = new List<string>();
            List<string> listeArtefacts = new List<string>();

            foreach(string piece in stuffAEquiper)
            {
                Objet objet = mcu.ObjetList.Where(x => x.code ==piece).FirstOrDefault();
                if (objet!= null)
                {
                    if (objet.type == Constantes.amulet)
                    {
                        //si je ne la porte pas 
                        if (FeuillePerso.amulet_slot != piece)
                        {
                            RetirerAmulette(); // retire l'amulette si j'en ai une
                            //si elle est dans mon inventaire
                            if (!ExistsInInventory(piece))
                            {
                                //je vais la récupérer à la banque
                                Aller_Banque();
                                Retirer_Banque(piece,1);
                            }
                            //je mets la pièce depuis mon inventaire
                            EquiperAmulette(piece);
                        }
                    }
                    if (objet.type == Constantes.bodyArmor)
                    {
                        //si je ne la porte pas 
                        if (FeuillePerso.body_armor_slot != piece)
                        {
                            RetirerBodyArmor(); // retire l'amulette si j'en ai une
                            //si elle est dans mon inventaire
                            if (!ExistsInInventory(piece))
                            {
                                //je vais la récupérer à la banque
                                Aller_Banque();
                                Retirer_Banque(piece, 1);
                            }
                            //je mets la pièce depuis mon inventaire
                            EquiperBodyArmor(piece);
                        }
                    }
                    if (objet.type == Constantes.weapon)
                    {
                        //si je ne la porte pas 
                        if (FeuillePerso.weapon_slot != piece)
                        {
                            RetirerWeapon(); // retire l'amulette si j'en ai une
                            //si elle est dans mon inventaire
                            if (!ExistsInInventory(piece))
                            {
                                //je vais la récupérer à la banque
                                Aller_Banque();
                                Retirer_Banque(piece, 1);
                            }
                            //je mets la pièce depuis mon inventaire
                            EquiperWeapon(piece);
                        }
                    }
                    if (objet.type == Constantes.leg_armor)
                    {
                        //si je ne la porte pas 
                        if (FeuillePerso.leg_armor_slot != piece)
                        {
                            RetirerLegArmor(); // retire l'amulette si j'en ai une
                            //si elle est dans mon inventaire
                            if (!ExistsInInventory(piece))
                            {
                                //je vais la récupérer à la banque
                                Aller_Banque();
                                Retirer_Banque(piece, 1);
                            }
                            //je mets la pièce depuis mon inventaire
                            EquiperLegArmor(piece);
                        }
                    }
                    if (objet.type == Constantes.helmet)
                    {
                        //si je ne la porte pas 
                        if (FeuillePerso.helmet_slot != piece)
                        {
                            RetirerHelmet(); // retire l'amulette si j'en ai une
                            //si elle est dans mon inventaire
                            if (!ExistsInInventory(piece))
                            {
                                //je vais la récupérer à la banque
                                Aller_Banque();
                                Retirer_Banque(piece, 1);
                            }
                            //je mets la pièce depuis mon inventaire
                            EquiperHelmet(piece);
                        }
                    }
                    if (objet.type == Constantes.boots)
                    {
                        //si je ne la porte pas 
                        if (FeuillePerso.helmet_slot != piece)
                        {
                            RetirerBoots(); // retire l'amulette si j'en ai une
                            //si elle est dans mon inventaire
                            if (!ExistsInInventory(piece))
                            {
                                //je vais la récupérer à la banque
                                Aller_Banque();
                                Retirer_Banque(piece, 1);
                            }
                            //je mets la pièce depuis mon inventaire
                            EquiperBoots(piece);
                        }
                    }
                    if (objet.type == Constantes.shield)
                    {
                        //si je ne la porte pas 
                        if (FeuillePerso.helmet_slot != piece)
                        {
                            RetirerShield(); // retire l'amulette si j'en ai une
                            //si elle est dans mon inventaire
                            if (!ExistsInInventory(piece))
                            {
                                //je vais la récupérer à la banque
                                Aller_Banque();
                                Retirer_Banque(piece, 1);
                            }
                            //je mets la pièce depuis mon inventaire
                            EquiperShield(piece);
                        }
                    }
                    if (objet.type == Constantes.artifact)
                    {
                        listeArtefacts.Add(piece);
                    }
                    if (objet.type == Constantes.ring)
                    {
                        listeAnneaux.Add(piece);
                    }
                }
            }

            RetirerAnneaux();
            RetirerArtefacts();
            foreach (string anneau in listeAnneaux)
            {
                if (!ExistsInInventory(anneau))
                {
                    //je vais la récupérer à la banque
                    Aller_Banque();
                    Retirer_Banque(anneau, 1);
                }
                EquiperAnneau(anneau);
            }
            foreach (string artefact in listeArtefacts)
            {
                if (!ExistsInInventory(artefact))
                {
                    //je vais la récupérer à la banque
                    Aller_Banque();
                    Retirer_Banque(artefact, 1);
                }
                EquiperArtefact(artefact);
            }
        }

        internal void Vider_Inventaire()
        {
            List<Tuple<string, int>> aDeposer = new List<Tuple<string, int>>();
            foreach (Inventory inventory in FeuillePerso.inventory)
            {
                if (!String.IsNullOrEmpty(inventory.code))
                {
                    aDeposer.Add(new Tuple<string, int>(inventory.code, inventory.quantity));
                }
            }
            base.Vider_Inventaire(aDeposer);
        }
    }
}
