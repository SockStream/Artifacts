using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Artifacts.Items;
using Artifacts.Maps;
using Artifacts.Monsters;
using Artifacts.MyAccount;
using Artifacts.MyCharacters;
using Artifacts.Utilities;
using Artifacts.Utilities.ConsoleManager;
using static Artifacts.Resources.ResourcesResponse;

namespace Artifacts.Bot.Personnage
{
    internal class Gatherer : Personnage
    {
        internal Enums.GathererTypeEnum gathererTypeEnum;
        List<int> ListePaliers = new List<int>() { 10, 30 };
        public Gatherer(Enums.GathererTypeEnum type, MCU mCU)
        {
            gathererTypeEnum = type;
            mcu = mCU;
            WorkOrderList = new List<WorkOrder>();
        }

        internal override void QueFaire()
        {
            while (!termine)
            {
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
                    RecupererCommande();
                    EquiperWeapon(WorkOrderList.First().Code);
                    CommandeEnAttente = false;
                }

                if (NiveauMetierGagne && WorkOrderList.Count == 0)
                {
                    int niveauMetier = GetNiveauMetier();
                    int? palier = ListePaliers.Where(x => x <= niveauMetier).OrderByDescending(x => x).FirstOrDefault();
                    if (palier != 0)
                    {
                        List<Objet> ListeBest_tool = mcu.getObjetList().Where(x => x.type == Constantes.weapon && x.subtype == Constantes.tool && x.level == palier && x.effects.Where(y => y.name == metier).Any()).ToList();
                        Objet best_tool = null;
                        if (ListeBest_tool.Count > 0)
                        {
                            best_tool = ListeBest_tool.OrderByDescending(x => x.level).FirstOrDefault();
                        }
                        if (best_tool != null && FeuillePerso.weapon_slot != best_tool.code)
                        {
                            WorkOrder wo = new WorkOrder();
                            wo.Quantité = 1;
                            wo.Code = best_tool.code;
                            wo.Demandeur = this;
                            mcu.AjouterWorkOrder(wo);
                            WorkOrderList.Add(wo);
                        }
                    }
                    NiveauMetierGagne = false;
                }

                
                
                List<Tuple<string, int>> listeTuplesCodeQuantite = mcu.GetListeDesResourcesPourTousLesCrafts(); //regarder pour le wood staff
                if (listeTuplesCodeQuantite != null && listeTuplesCodeQuantite.Count > 0)
                {
                    bool action_faite = false;
                    foreach (Tuple<string, int> t in listeTuplesCodeQuantite)
                    {
                        Resource resourceARecuperer = null;
                        Objet objet = mcu.getObjetList().Where(x => x.code == t.Item1).FirstOrDefault();
                        Objet tst = mcu.getObjetList().Where(x => x.code == "copper_ore").FirstOrDefault();
                        
                        if (objet.craft == null)
                        {
                            Resource TupleAsResource = mcu.GetResourceList().Where(x => x.code == t.Item1).FirstOrDefault();
                            if (TupleAsResource != null)
                            {
                                if (TupleAsResource.skill == Constantes.fishing && FeuillePerso.fishing_level >= TupleAsResource.level)
                                {
                                    resourceARecuperer = TupleAsResource;
                                }
                                if (TupleAsResource.skill == Constantes.mining && FeuillePerso.mining_level >= TupleAsResource.level)
                                {
                                    resourceARecuperer = TupleAsResource;
                                }
                                if (TupleAsResource.skill == Constantes.woodcutting && FeuillePerso.woodcutting_level >= TupleAsResource.level)
                                {
                                    resourceARecuperer = TupleAsResource;
                                }
                            }

                            if (resourceARecuperer != null)
                            {
                                Aller_Banque();
                                Vider_Inventaire();
                                //aller sur la bonne tuile
                                AllerSurTuile(resourceARecuperer);
                                int niveau_actuel = GetNiveauMetier();
                                Character c = MyCharactersEndPoint.Gathering(FeuillePerso.name);
                                if (c != null)
                                {
                                    FeuillePerso = c;
                                }
                                if (GetNiveauMetier() != niveau_actuel)
                                {
                                    NiveauMetierGagne = true;
                                }
                                action_faite = true;
                            }
                        }
                        else
                        {
                            if (objet.craft.skill == metier && objet.craft.level <= GetNiveauMetier()) //je peux le crafter
                            {
                                Aller_Banque();
                                Vider_Inventaire();
                                int nombre_de_fournées = (int)t.Item2 / 10;
                                int restant = t.Item2 % 10;
                                for(int i = 0; i < nombre_de_fournées; i++)
                                {
                                    CrafterObjet(t.Item1, 10);
                                    Aller_Banque();
                                    Vider_Inventaire();
                                }
                                if (restant != 0)
                                {
                                    CrafterObjet(t.Item1, restant);
                                    Aller_Banque();
                                    Vider_Inventaire();
                                }

                                action_faite = true;
                            }
                        }
                    }
                    if (!action_faite)
                    {
                        FarmerMeilleureRessource(metier);
                    }
                }
                else
                {
                    FarmerMeilleureRessource(metier);
                }
            }
            //code de fin
            Aller_Banque();
            Vider_Gold();
            MyCharactersEndPoint.Move(FeuillePerso.name, 0, 0, false);
            ConsoleManager.Write(FeuillePerso.name + "-> Fin", ConsoleColor.Gray);
        }

        internal void CrafterObjet(string code, int quantite)
        {
            Objet objet = mcu.getObjetList().Where(x => x.code == code).FirstOrDefault();
            if (objet.craft == null)
            {
                Resource resource = mcu.GetResourceList().Where(x => x.drops != null && x.drops.Where(y => y.code == code).Any()).FirstOrDefault();
                Inventory inv = FeuillePerso.inventory.Where(x => x.code == code).FirstOrDefault();
                int quantiteDansSac = 0;
                if (inv != null)
                {
                    quantiteDansSac = inv.quantity;
                }
                Item objetEnBanque = mcu.ConsulterBanque().Where(x => x.code == code).FirstOrDefault();
                int quantiteEnBanque = 0;
                if (objetEnBanque != null)
                {
                    quantiteEnBanque = Int32.Parse(objetEnBanque.quantity);
                }
                if (quantiteDansSac >= quantite)
                {
                    return;
                }
                if (quantiteDansSac + quantiteEnBanque >= quantite)
                {
                    Aller_Banque();
                    mcu.RecupererDeBanque(FeuillePerso.name, code, quantite - quantiteDansSac);
                    return;
                }
                AllerSurTuile(resource);
                int niveauMetier_actuel = GetNiveauMetier();
                int nvelleQuantite = quantiteDansSac;
                while ((nvelleQuantite + quantiteEnBanque < quantite) &&!termine)
                {
                    Character c = MyCharactersEndPoint.Gathering(FeuillePerso.name);
                    if (c != null)
                    {
                        FeuillePerso = c;
                    }

                    nvelleQuantite = FeuillePerso.inventory.Where(x => x.code == code).First().quantity;
                    
                    objetEnBanque = mcu.ConsulterBanque().Where(x => x.code == code).FirstOrDefault();
                    quantiteEnBanque = 0;
                    if (objetEnBanque != null)
                    {
                        quantiteEnBanque = Int32.Parse(objetEnBanque.quantity);
                    }
                }
                if (quantiteEnBanque > 0)
                {
                    Aller_Banque();
                    mcu.RecupererDeBanque(FeuillePerso.name, code, quantiteEnBanque);
                }
                if (GetNiveauMetier() != niveauMetier_actuel)
                {
                    NiveauMetierGagne = true;
                }
                return;
            }

            if (objet == null)
            {
                throw new Exception("normalement pas possible");
            }
            else
            {
                foreach (Item item in objet.craft.items)
                {
                    if (!ExistsInInventory(item.code, Int32.Parse(item.quantity) * quantite))
                    {
                        int quantite_requise = Int32.Parse(item.quantity) * quantite - NombreDansInventaire(item.code);
                        CrafterObjet(item.code, quantite_requise);
                        if (termine)
                        {
                            return;
                        }
                    }
                }
            }
            //trouver et aller au bon workshop
            Artifacts.Items.Craft craft = objet.craft;
            if (craft.skill == Constantes.woodcutting)
            {
                Aller_woodcutting();
            }
            if (craft.skill == Constantes.mining)
            {
                Aller_mining();
            }
            if (craft.skill == Constantes.jewelrycrafting)
            {
                Aller_fishing();
            }
            MyCharactersEndPoint.Crafting(FeuillePerso.name, code, quantite);
        }

        internal void Aller_woodcutting()
        {
            Tile tuile = mcu.Map.Where(x => x.content != null && x.content.code == Constantes.woodcutting).First();
            Character c = MyCharactersEndPoint.Move(FeuillePerso.name, tuile.x, tuile.y);
            if (c != null)
            {
                FeuillePerso = c;
            }
        }
        internal void Aller_mining()
        {
            Tile tuile = mcu.Map.Where(x => x.content != null && x.content.code == Constantes.mining).First();
            Character c = MyCharactersEndPoint.Move(FeuillePerso.name, tuile.x, tuile.y);
            if (c != null)
            {
                FeuillePerso = c;
            }
        }
        internal void Aller_fishing()
        {
            Tile tuile = mcu.Map.Where(x => x.content != null && x.content.code == Constantes.fishing).First();
            Character c = MyCharactersEndPoint.Move(FeuillePerso.name, tuile.x, tuile.y);
            if (c != null)
            {
                FeuillePerso = c;
            }
        }

        internal void Vider_Inventaire()
        {
            List<Tuple<string,int>> aDeposer = new List<Tuple<string,int>>();
            foreach(Inventory inventory in FeuillePerso.inventory)
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
