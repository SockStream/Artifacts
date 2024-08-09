using Artifacts.GrandExchange;
using Artifacts.Items;
using Artifacts.Maps;
using Artifacts.MyCharacters;
using Artifacts.Utilities;
using Artifacts.Utilities.ConsoleManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifacts.Bot.Personnage
{
    internal class Crafteur : Personnage
    {
        public bool CrafterWorkOrder = false;
        
        public Crafteur(MCU mCU)
        {
            mcu = mCU;
            metier = Constantes.mining;
            WorkOrderList = new List<WorkOrder>();
        }

        internal override void QueFaire()
        {
            while (!termine)
            {
                bool action_faite = false;
                if (doitViderEnBanque)
                {
                    Aller_Banque();
                    Vider_Inventaire();
                    doitViderEnBanque = false;
                }

                WorkOrder wo = mcu.GetWorkorderAttenteCraft();
                if (wo != null && CrafterWorkOrder && PeutCrafter(wo.Code))
                {
                    action_faite = true;
                    CrafterWorkOrder = false;
                    Aller_Banque();
                    RecupererCompos(wo.Code, wo.Quantité);
                    CrafterItem(wo.Code, wo.Quantité);
                    Aller_Banque();
                    Vider_Inventaire();
                    if (wo.Demandeur.GetType() != typeof(Crafteur))
                    {
                        wo.Demandeur.CommandeEnAttente = true;

                    }
                    else
                    {
                        wo.Demandeur.WorkOrderList.Remove(wo);
                        Aller_Banque();
                        Vider_Inventaire();
                        //Aller_GrandExchange();
                        //GrandExchange.GrandExchange exchange = GrandExchangeEndPoint.GetItem(wo.Code);
                        //todo faire une boucle pour vendre si plus de 50
                        //Character c = MyCharactersEndPoint.GeSellItem(FeuillePerso.name, wo.Code, Math.Min(wo.Quantité, 50), exchange.sell_price);

                    }
                    mcu.SupprimerCommandeLivree(wo);
                }
                else
                {
                    
                    if (WorkOrderList.Count == 0)
                    {
                        string minCompetence = Constantes.gearcrafting;
                        int minlvl = FeuillePerso.gearcrafting_level;

                        if (FeuillePerso.weaponcrafting_level <= minlvl)
                        {
                            minlvl = FeuillePerso.weaponcrafting_level;
                            minCompetence = Constantes.weaponcrafting;
                        }
                        if (FeuillePerso.jewelrycrafting_level <= minlvl)
                        {
                            minlvl = FeuillePerso.jewelrycrafting_level;
                            minCompetence = Constantes.jewelrycrafting;
                        }
                        if (FeuillePerso.cooking_level <= minlvl && FeuillePerso.gearcrafting_level >= 25 && FeuillePerso.weaponcrafting_level >= 25 && FeuillePerso.jewelrycrafting_level >= 25)
                        {
                            minlvl = FeuillePerso.cooking_level;
                            minCompetence = Constantes.cooking;
                        }

                        if (minCompetence == Constantes.gearcrafting)
                        {
                            Farmer_gearcrafting();
                        }
                        if (minCompetence == Constantes.weaponcrafting)
                        {
                            Farmer_weaponcrafting();
                        }
                        if (minCompetence == Constantes.jewelrycrafting)
                        {
                            Farmer_jewelrycrafting();
                        }
                        if (minCompetence == Constantes.cooking )
                        {
                            Farmer_cooking();
                        }
                    }
                }
                if (!action_faite)
                {
                    FarmerMeilleureRessource(metier);
                    if (nb_Items_Inventaire() >=20)
                    {
                        doitViderEnBanque = true;
                    }
                }
            }
            //code de fin
            Aller_Banque();
            Vider_Gold();
            MyCharactersEndPoint.Move(FeuillePerso.name, 0, 0, false);
            ConsoleManager.Write(FeuillePerso.name + "-> Fin", ConsoleColor.Gray);
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

        internal void Farmer_gearcrafting()
        {
            Objet obj = mcu.ObjetList.Where(x => x.craft != null && x.craft.skill == Constantes.gearcrafting && x.craft.level <= FeuillePerso.gearcrafting_level).OrderByDescending(x => x.level).FirstOrDefault();
            if (obj != null)
            {
                WorkOrder wo = new WorkOrder();
                wo.Quantité = 5;
                wo.Demandeur = this;
                wo.Code = obj.code;
                mcu.AjouterWorkOrder(wo);
                WorkOrderList.Add(wo);
            }
        }

        internal void Farmer_weaponcrafting()
        {
            Objet obj = mcu.ObjetList.Where(x => x.craft != null && x.craft.skill == Constantes.weaponcrafting && x.craft.level <= FeuillePerso.weaponcrafting_level).OrderByDescending(x => x.level).FirstOrDefault();
            if (obj != null)
            {
                WorkOrder wo = new WorkOrder();
                wo.Quantité = 5;
                wo.Demandeur = this;
                wo.Code = obj.code;
                mcu.AjouterWorkOrder(wo);
                WorkOrderList.Add(wo);
            }
        }

        internal void Farmer_jewelrycrafting()
        {
            Objet obj = mcu.ObjetList.Where(x => x.craft != null && x.craft.skill == Constantes.jewelrycrafting && x.craft.level <= FeuillePerso.jewelrycrafting_level).OrderByDescending(x => x.level).FirstOrDefault();
            if (obj != null)
            {
                WorkOrder wo = new WorkOrder();
                wo.Quantité = 5;
                wo.Demandeur = this;
                wo.Code = obj.code;
                mcu.AjouterWorkOrder(wo);
                WorkOrderList.Add(wo);
            }
        }

        internal void Farmer_cooking()
        {
            List<Objet> list = mcu.ObjetList.Where(x => x.craft!=null && x.subtype == Constantes.food && x.craft.level <= FeuillePerso.cooking_level).OrderByDescending(x => x.level).ToList();
            Objet commande = null;
            foreach (Objet recette in list)
            {
                bool cook = true;
                foreach (Item item in recette.craft.items)
                {
                    if (!mcu.GetResourceList().Where(x => x.drops != null && x.drops.Where(x => x.code == item.code).Any()).Any()) //ça veut dire que la recette contient un drop de monstre
                    {
                        cook = false;
                    }
                }
                if (cook)
                {
                    commande = recette;
                    break;
                }
            }

            if (commande != null)
            {
                WorkOrder wo = new WorkOrder();
                wo.Quantité = 10;
                wo.Demandeur = this;
                wo.Code = commande.code;
                mcu.AjouterWorkOrder(wo);
                WorkOrderList.Add(wo);
            }
        }

        public bool PeutCrafter(string code)
        {
            Objet objet = mcu.getObjetList().Where(x => x.code == code).FirstOrDefault();
            if ( (objet == null))
            {
                throw new Exception("normalement ça n'arrive pas");
            }
            if (objet.type == Constantes.resource)
            {
                return true;
            }
            else
            {
                string skill = objet.craft.skill;
                if (skill == Constantes.weaponcrafting)
                {
                    if (objet.craft.level > FeuillePerso.weaponcrafting_level)
                    {
                        return false;
                    }
                }
                if (skill == Constantes.gearcrafting)
                {
                    if (objet.craft.level > FeuillePerso.gearcrafting_level)
                    {
                        return false;
                    }
                }
                if (skill == Constantes.jewelrycrafting)
                {
                    if (objet.craft.level > FeuillePerso.jewelrycrafting_level)
                    {
                        return false;
                    }
                }
                if (skill == Constantes.cooking)
                {
                    if (objet.craft.level > FeuillePerso.cooking_level)
                    {
                        return false;
                    }
                }
                if (skill == Constantes.woodcutting)
                {
                    if (objet.craft.level > FeuillePerso.woodcutting_level)
                    {
                        return false;
                    }
                }
                if (skill == Constantes.mining)
                {
                    if (objet.craft.level > FeuillePerso.mining_level)
                    {
                        return false;
                    }
                }

                //ça veut dire que j'ai le niveau
                foreach(Item item in objet.craft.items)
                {
                    if (!PeutCrafter(item.code))
                    {
                        return false ;
                    }
                }
                return true;

            }
        }

        internal void RecupererCompos(string code, int quantite)
        {
            Objet objet = mcu.getObjetList().Where(x => x.code == code).FirstOrDefault();
            if (objet == null)
            {
                throw new Exception("normalement pas possible");
            }
            else
            {
                if (objet.type == "resource")
                {
                    MyCharactersEndPoint.WithdrawBank(FeuillePerso.name, code, quantite);
                    return;
                }
                else
                {
                    Artifacts.Items.Craft craft = objet.craft;
                    foreach(Item item in craft.items)
                    {
                        int nombreEnBanque = NombreDansBanque(item.code);
                        if (nombreEnBanque < quantite * craft.quantity)
                        {
                            if (nombreEnBanque != 0)
                            {
                                mcu.RecupererDeBanque(FeuillePerso.name, item.code, nombreEnBanque);
                            }
                            RecupererCompos(item.code, quantite * Int32.Parse(item.quantity) - nombreEnBanque);
                        }
                        else
                        {
                            mcu.RecupererDeBanque(FeuillePerso.name, item.code, quantite * Int32.Parse(item.quantity));
                        }
                    }
                }
            }
        }

        internal void CrafterItem(string code, int quantite)
        {
            Objet objet = mcu.getObjetList().Where(x => x.code == code).FirstOrDefault();
            if(objet.type == Constantes.resource)
            {
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
                        CrafterItem(item.code, quantite_requise);
                    }
                }
            }
            //trouver et aller au bon workshop
            Artifacts.Items.Craft craft = objet.craft;
            if (craft.skill == Constantes.weaponcrafting)
            {
                Aller_weaponcrafting();
            }
            if (craft.skill == Constantes.gearcrafting)
            {
                Aller_gearcrafting();
            }
            if (craft.skill == Constantes.jewelrycrafting)
            {
                Aller_jewelrycrafting();
            }
            if (craft.skill == Constantes.cooking)
            {
                Aller_cooking();
            }
            MyCharactersEndPoint.Crafting(FeuillePerso.name, code, quantite);
        }

        internal void Aller_weaponcrafting()
        {
            Tile tuile = mcu.Map.Where(x => x.content != null && x.content.code == Constantes.weaponcrafting).First();
            Character c = MyCharactersEndPoint.Move(FeuillePerso.name, tuile.x, tuile.y);
            if (c != null)
            {
                FeuillePerso = c;
            }
        }
        internal void Aller_gearcrafting()
        {
            Tile tuile = mcu.Map.Where(x => x.content != null && x.content.code == Constantes.gearcrafting).First();
            Character c = MyCharactersEndPoint.Move(FeuillePerso.name, tuile.x, tuile.y);
            if (c != null)
            {
                FeuillePerso = c;
            }
        }
        internal void Aller_jewelrycrafting()
        {
            Tile tuile = mcu.Map.Where(x => x.content != null && x.content.code == Constantes.jewelrycrafting).First();
            Character c = MyCharactersEndPoint.Move(FeuillePerso.name, tuile.x, tuile.y);
            if (c != null)
            {
                FeuillePerso = c;
            }
        }
        internal void Aller_cooking()
        {
            Tile tuile = mcu.Map.Where(x => x.content != null && x.content.code == Constantes.cooking).First();
            Character c = MyCharactersEndPoint.Move(FeuillePerso.name, tuile.x, tuile.y);
            if (c != null)
            {
                FeuillePerso = c;
            }
        }
    }
}
