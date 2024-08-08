using Artifacts.Items;
using Artifacts.Monsters;
using Artifacts.MyCharacters;
using Artifacts.Utilities;
using Artifacts.Utilities.ConsoleManager;
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
        List<int> ListePaliers;
        List<string> listeTypesPiecesEquipements = new List<string> { Constantes.bodyArmor, Constantes.weapon, Constantes.leg_armor, Constantes.helmet, Constantes.boots, Constantes.shield, Constantes.amulet, Constantes.ring};
        public Guerrier(MCU mCU)
        {
            mcu = mCU;
            WorkOrderList = new List<WorkOrder>();
            metier = Constantes.combat;
            ListePaliers = new List<int> { 1, 5, 10, 15, 20, 25, 30 };
            NiveauMetierGagne = true;
        }
        internal override void QueFaire()
        {
            while (!termine)
            {
                /*foreach (WorkOrder workOrder in WorkOrderList)
                {
                    if (!mcu.GetListeWorkorderAttenteRessource().Contains(workOrder) && mcu.GetWorkorderAttenteCraft() != workOrder)
                    {
                        mcu.AjouterWorkOrder(workOrder);
                    }
                }*/
                if (IsInventairePlein() || doitViderEnBanque)
                {
                    Aller_Banque();
                    Vider_Inventaire();
                    doitViderEnBanque = false;
                }
                if (CommandeEnAttente)
                {
                    Aller_Banque();
                    Vider_Inventaire();
                    List<WorkOrder> recupere = RecupererCommande();
                    foreach (WorkOrder wo in recupere)
                    {
                        Objet obj = mcu.getObjetList().Where(x => x.code == wo.Code).FirstOrDefault();
                        if (obj == null)
                        {
                            throw new Exception("should not happen");
                        }
                        if (obj.type == Constantes.weapon)
                        {
                            RetirerWeapon();
                            EquiperWeapon(obj.code);
                        }
                        if (obj.type == Constantes.helmet)
                        {
                            RetirerHelmet();
                            EquiperHelmet(obj.code);
                        }
                        if (obj.type == Constantes.bodyArmor)
                        {
                            RetirerBodyArmor();
                            EquiperBodyArmor(obj.code);
                        }
                        if (obj.type == Constantes.leg_armor)
                        {
                            RetirerLegArmor();
                            EquiperLegArmor(obj.code);
                        }
                        if (obj.type == Constantes.boots)
                        {
                            RetirerBoots();
                            EquiperBoots(obj.code);
                        }
                        if (obj.type == Constantes.shield)
                        {
                            RetirerShield();
                            EquiperShield(obj.code);
                        }
                        if (obj.type == Constantes.amulet)
                        {
                            RetirerAmulette();
                            EquiperAmulette(obj.code);
                        }
                        if (obj.type == Constantes.ring)
                        {
                            RetirerAnneaux();
                            EquiperAnneau(obj.code);
                        }
                    }
                    CommandeEnAttente = false;
                }

                if (NiveauMetierGagne && WorkOrderList.Count == 0)
                {
                    int niveauMetier = GetNiveauMetier();

                    int? palier = ListePaliers.Where(x => x <= niveauMetier).OrderByDescending(x=> x).FirstOrDefault();
                    if (palier != 0)
                    {
                        List<Objet> List_stuff_palier = mcu.getObjetList().Where(x => listeTypesPiecesEquipements.Contains(x.type) && x.subtype != Constantes.tool && x.level == palier.Value).ToList();
                        

                        foreach (Objet piece in List_stuff_palier)
                        {
                            bool pieceCommandeeOuExiste = false;
                            if (ExistsInInventory(piece.code))
                            {
                                pieceCommandeeOuExiste = true;
                            }
                            foreach (WorkOrder wo in WorkOrderList)
                            {
                                if (wo.Code == piece.code)
                                {
                                    pieceCommandeeOuExiste = true;
                                }
                            }
                            if (!pieceCommandeeOuExiste)
                            {
                                WorkOrder wo = new WorkOrder();
                                wo.Quantité = 1;
                                wo.Code = piece.code;
                                wo.Demandeur = this;
                                mcu.AjouterWorkOrder(wo);
                                WorkOrderList.Add(wo);
                            }
                        }
                    }
                    NiveauMetierGagne = false;
                }

                Monster monstreATuer = null;
                List<string> stuffAEquiper = new List<string>();
                
                //on va convertir le code de la ressource en Objet
                List<Tuple<string, int>> listeTuplesCodeQuantite = mcu.GetListeDesResourcesPourTousLesCrafts();
                if (listeTuplesCodeQuantite != null)
                {
                    foreach (Tuple<string, int> t in listeTuplesCodeQuantite)
                    {
                        Monster m = mcu.MonsterList.Where(x => x.drops.Where(y => y.code == t.Item1).Any()).FirstOrDefault();
                        if (m != null)
                        {
                            List<string> resultat = mcu.CSPCombat(this, m);
                            if (resultat.Count > 0)
                            {
                                monstreATuer = m;
                                stuffAEquiper = resultat;
                                break;
                            }
                        }
                    }
                }

                if (monstreATuer != null)
                {
                    //equiperStuff
                    EquiperStuff(stuffAEquiper);
                    //aller sur la bonne tuile
                    AllerSurTuileMonstre(monstreATuer);
                    int niveau_actuel = GetNiveauMetier();
                    Character c = MyCharactersEndPoint.Fight(FeuillePerso.name);
                    if (c != null)
                    {
                        FeuillePerso = c;
                    }
                    if (FeuillePerso.level != niveau_actuel)
                    {
                        NiveauMetierGagne = true;
                    }
                }
                else
                {
                    int niveau_actuel = GetNiveauMetier();
                    FarmerMeilleurMonstre();
                    if (FeuillePerso.level != niveau_actuel)
                    {
                        NiveauMetierGagne = true;
                    }
                }
            }
            //code de fin
            MyCharactersEndPoint.Move(FeuillePerso.name, 0, 0, false);
            ConsoleManager.Write(FeuillePerso.name + "-> Fin", ConsoleColor.Gray);
        }

        private void FarmerMeilleurMonstre()
        {
            foreach (Monster m in mcu.MonsterList.OrderByDescending(x => x.level))
            {
                //Console.WriteLine("__" + m.name + "__");
                List<string> resultat = mcu.CSPCombat(this, m);
                if (resultat.Count > 0)
                {
                    //equiperStuff
                    EquiperStuff(resultat);
                    AllerSurTuileMonstre(m);
                    MyCharactersEndPoint.Fight(FeuillePerso.name);
                    return;
                }
            }
        }

        internal void EquiperStuff(List<string> stuffAEquiper) // prérequis, le stuff est dans mon inventaire
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
                                mcu.RecupererDeBanque(FeuillePerso.name,piece, 1);
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
                                mcu.RecupererDeBanque(FeuillePerso.name,piece, 1);
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
                                mcu.RecupererDeBanque(FeuillePerso.name,piece, 1);
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
                                mcu.RecupererDeBanque(FeuillePerso.name,piece, 1);
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
                                mcu.RecupererDeBanque(FeuillePerso.name,piece, 1);
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
                                mcu.RecupererDeBanque(FeuillePerso.name,piece, 1);
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
                    mcu.RecupererDeBanque(FeuillePerso.name,anneau, 1);
                }
                EquiperAnneau(anneau);
            }
            foreach (string artefact in listeArtefacts)
            {
                if (!ExistsInInventory(artefact))
                {
                    //je vais la récupérer à la banque
                    Aller_Banque();
                    mcu.RecupererDeBanque(FeuillePerso.name,artefact, 1);
                }
                EquiperArtefact(artefact);
            }
        }

        internal void Vider_Inventaire()
        {
            int palier = ListePaliers.Where(x => x <= GetNiveauMetier()).FirstOrDefault();
            int palierPrecedent = ListePaliers.Where(x => x < palier).FirstOrDefault();
            List <Tuple<string, int>> aDeposer = new List<Tuple<string, int>>();
            foreach (Inventory inventory in FeuillePerso.inventory)
            {
                if (!String.IsNullOrEmpty(inventory.code))
                {
                    Objet obj = mcu.getObjetList().Where(x => x.code == inventory.code).FirstOrDefault();
                    if (obj == null)
                    {
                        throw new Exception("normalement ça arrive pas");
                    }
                    if (obj.level < palierPrecedent || obj.type == Constantes.resource)
                    {
                        aDeposer.Add(new Tuple<string, int>(inventory.code, inventory.quantity));
                    }
                }
            }
            base.Vider_Inventaire(aDeposer);
        }
    }
}
