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
                        /*Objet obj = mcu.getObjetList().Where(x => x.code == wo.Code).FirstOrDefault();
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
                        }*/
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
                                int quantite = 1;
                                if (piece.type == Constantes.ring)
                                {
                                    quantite = 2;
                                }
                                if (piece.type == Constantes.artifact)
                                {
                                    quantite = 3;
                                }
                                WorkOrder wo = new WorkOrder();
                                wo.Quantité = quantite;
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
                List<Tuple<Enums.EmplacementPieceStuff, string>> stuffAEquiper = new List<Tuple<Enums.EmplacementPieceStuff, string>>();
                
                //on va convertir le code de la ressource en Objet
                List<Tuple<string, int>> listeTuplesCodeQuantite = mcu.GetListeDesResourcesPourTousLesCrafts();
                if (listeTuplesCodeQuantite != null)
                {
                    foreach (Tuple<string, int> t in listeTuplesCodeQuantite)
                    {
                        Monster m = mcu.MonsterList.Where(x => x.drops.Where(y => y.code == t.Item1).Any()).FirstOrDefault();
                        if (m != null)
                        {
                            List<Tuple<Enums.EmplacementPieceStuff, string>> resultat = mcu.CSPCombat(this, m);
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
            Aller_Banque();
            Vider_Gold();
            MyCharactersEndPoint.Move(FeuillePerso.name, 0, 0, false);
            ConsoleManager.Write(FeuillePerso.name + "-> Fin", ConsoleColor.Gray);
        }

        private void FarmerMeilleurMonstre()
        {
            foreach (Monster m in mcu.MonsterList.OrderByDescending(x => x.level))
            {
                //Console.WriteLine("__" + m.name + "__");
                List<Tuple<Enums.EmplacementPieceStuff, string>> resultat = mcu.CSPCombat(this, m);
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

        internal void EquiperStuff(List<Tuple<Enums.EmplacementPieceStuff, string>> stuffAEquiper) // prérequis, le stuff est dans mon inventaire
        {
            List<string> listeAnneaux = new List<string>();
            List<string> listeArtefacts = new List<string>();

            foreach(Tuple<Enums.EmplacementPieceStuff, string> piece in stuffAEquiper)
            {
                Objet objet = mcu.ObjetList.Where(x => x.code ==piece.Item2).FirstOrDefault();
                if (objet!= null)
                {
                    switch(piece.Item1)
                    {
                        case EmplacementPieceStuff.weapon:
                            if (FeuillePerso.weapon_slot != piece.Item2 && FeuillePerso.weapon_slot != "")
                            {
                                RetirerWeapon(); // retire l'amulette si j'en ai une
                                
                            }
                            if (FeuillePerso.weapon_slot != piece.Item2)
                            {
                                EquiperWeapon(piece.Item2);
                            }
                            break;
                        case EmplacementPieceStuff.helmet:
                            if (FeuillePerso.helmet_slot != piece.Item2 && FeuillePerso.helmet_slot != "")
                            {
                                RetirerHelmet(); // retire l'amulette si j'en ai une
                            }
                            if (FeuillePerso.helmet_slot != piece.Item2)
                            {
                                EquiperHelmet(piece.Item2);
                            }
                            break;
                        case EmplacementPieceStuff.shield:
                            if (FeuillePerso.shield_slot != piece.Item2 && FeuillePerso.shield_slot != "")
                            {
                                RetirerShield(); // retire l'amulette si j'en ai une
                            }
                            if (FeuillePerso.shield_slot != piece.Item2)
                            {
                                EquiperShield(piece.Item2);
                            }
                            break;
                        case EmplacementPieceStuff.body_armor:
                            if (FeuillePerso.body_armor_slot != piece.Item2 && FeuillePerso.body_armor_slot != "")
                            {
                                RetirerBodyArmor(); // retire l'amulette si j'en ai une
                            }
                            if (FeuillePerso.body_armor_slot != piece.Item2)
                            {
                                EquiperBodyArmor(piece.Item2);
                            }
                            break;
                        case EmplacementPieceStuff.amulet:
                            if (FeuillePerso.amulet_slot != piece.Item2 && FeuillePerso.amulet_slot != "")
                            {
                                RetirerAmulette(); // retire l'amulette si j'en ai une
                            }
                            if (FeuillePerso.amulet_slot != piece.Item2)
                            {
                                EquiperAmulette(piece.Item2);
                            }
                            break;
                        case EmplacementPieceStuff.leg_armor:
                            if (FeuillePerso.leg_armor_slot != piece.Item2 && FeuillePerso.leg_armor_slot != "")
                            {
                                RetirerLegArmor(); // retire l'amulette si j'en ai une
                            }
                            if (FeuillePerso.leg_armor_slot != piece.Item2)
                            {
                                EquiperLegArmor(piece.Item2);
                            }
                            break;
                        case EmplacementPieceStuff.boots:
                            if (FeuillePerso.boots_slot != piece.Item2 && FeuillePerso.boots_slot != "")
                            {
                                RetirerBoots(); // retire l'amulette si j'en ai une
                            }
                            if (FeuillePerso.boots_slot != piece.Item2)
                            {
                                EquiperBoots(piece.Item2);
                            }
                            break;
                        case EmplacementPieceStuff.ring1:
                            if (FeuillePerso.ring1_slot != piece.Item2 && FeuillePerso.ring1_slot != "")
                            {
                                RetirerAnneau1(); // retire l'amulette si j'en ai une
                            }
                            if (FeuillePerso.ring1_slot != piece.Item2)
                            {
                                EquiperAnneau1(piece.Item2);
                            }
                            break;
                        case EmplacementPieceStuff.ring2:
                            if (FeuillePerso.ring2_slot != piece.Item2 && FeuillePerso.ring2_slot != "")
                            {
                                RetirerAnneau2(); // retire l'amulette si j'en ai une
                            }
                            if (FeuillePerso.ring2_slot != piece.Item2)
                            {
                                EquiperAnneau2(piece.Item2);
                            }
                            break;
                        case EmplacementPieceStuff.artifact1:
                            if (FeuillePerso.artifact1_slot != piece.Item2 && FeuillePerso.artifact1_slot != "")
                            {
                                RetirerArtifact1(); // retire l'amulette si j'en ai une
                            }
                            if (FeuillePerso.artifact1_slot != piece.Item2)
                            {
                                EquiperArtifact1(piece.Item2);
                            }
                            break;
                        case EmplacementPieceStuff.artifact2:
                            if (FeuillePerso.artifact2_slot != piece.Item2 && FeuillePerso.artifact2_slot != "")
                            {
                                RetirerArtifact2(); // retire l'amulette si j'en ai une
                            }
                            if (FeuillePerso.artifact2_slot != piece.Item2)
                            {
                                EquiperArtifact2(piece.Item2);
                            }
                            break;
                        case EmplacementPieceStuff.artifact3:
                            if (FeuillePerso.artifact3_slot != piece.Item2 && FeuillePerso.artifact3_slot != "")
                            {
                                RetirerArtifact3(); // retire l'amulette si j'en ai une
                            }
                            if (FeuillePerso.artifact3_slot != piece.Item2)
                            {
                                EquiperArtifact3(piece.Item2);
                            }
                            break;
                    }
                }
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
