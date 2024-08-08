using Artifacts.Items;
using Artifacts.Maps;
using Artifacts.Monsters;
using Artifacts.MyCharacters;
using Artifacts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Artifacts.Resources.ResourcesResponse;

namespace Artifacts.Bot.Personnage
{
    public abstract class Personnage
    {
        internal List<WorkOrder> WorkOrderList { get; set; }
        public Character FeuillePerso { get; set; }
        public bool doitViderEnBanque = false;
        public bool termine = false;
        public bool NiveauMetierGagne = true, CommandeEnAttente = false;
        internal MCU mcu;
        public string metier;
        internal abstract void QueFaire();
        internal virtual void Vider_Inventaire(List<Tuple<string, int>> aDeposer)
        {
            foreach (Tuple<string,int> t in aDeposer)
            {
                Character c = mcu.DeposerBanque(FeuillePerso.name, t.Item1, t.Item2);
                if (c != null)
                {
                    FeuillePerso = c;
                }
            }
        }

        internal void RamenerDrops(List<Tuple<string, int>> listeDesResources)
        {
            if (!doitViderEnBanque)
            {
                foreach (Tuple<string, int> t in listeDesResources)
                {
                    if (FeuillePerso.inventory.Where(x => x.code.Equals(t.Item1)).Any())
                    {
                        doitViderEnBanque = true;
                        break;
                    }
                }
            }
        }

        internal int GetNiveauMetier()
        {
            int niveauMetier = 0;
            if (metier == Constantes.mining)
            {
                niveauMetier = FeuillePerso.mining_level;
            }
            if (metier == Constantes.woodcutting)
            {
                niveauMetier = FeuillePerso.woodcutting_level;
            }
            if (metier == Constantes.fishing)
            {
                niveauMetier = FeuillePerso.fishing_level;
            }
            if (metier == Constantes.combat)
            {
                niveauMetier = FeuillePerso.level;
            }
            return niveauMetier;
        }
        internal void FarmerMeilleureRessource(string metier)
        {
            int niveauMetier_actuel = GetNiveauMetier();
            //on cherche la meilleure ressource associée à notre métier, on se déplace sur la case si on est pas déjà dessus, et on fait 1 Gather
            Resource meilleureResource = mcu.GetResourceList().Where(x => x.skill == metier && x.level <= niveauMetier_actuel).OrderByDescending(x => x.level).FirstOrDefault();
            if (meilleureResource != null)
            {
                AllerSurTuile(meilleureResource);
                Character c = MyCharactersEndPoint.Gathering(FeuillePerso.name);
                if (c != null)
                {
                    FeuillePerso = c;
                }
                if (GetNiveauMetier() != niveauMetier_actuel)
                {
                    NiveauMetierGagne = true;
                }
            }

        }

        internal void AllerSurTuile(Resource resource)
        {
            Maps.Tile tuile = mcu.Map.Where(x => x.content != null && x.content.code == resource.code).FirstOrDefault();
            if (tuile != null)
            {
                if (FeuillePerso.x == tuile.x && FeuillePerso.y == tuile.y)
                {
                    return;
                }
                Character c = MyCharactersEndPoint.Move(FeuillePerso.name, tuile.x, tuile.y);
                if (c != null)
                {
                    FeuillePerso = c;
                }
            }
        }

        internal bool IsInventairePlein()
        {
            bool plein = false;
            int cpt = 0;
            foreach (Inventory inventory in FeuillePerso.inventory)
            {
                cpt += inventory.quantity;
            }
            if (cpt >= FeuillePerso.inventory_max_items)
            {
                plein = true;
            }
            if (plein || FeuillePerso.inventory.Where(x => String.IsNullOrEmpty(x.code)).Count() == 0)
            {
                plein = true;
            }
            return plein;
        }

        internal void Aller_Banque()
        {
            Maps.Tile tuile = mcu.Map.Where(x => x.content != null && x.content.type == "bank").First();
            if (tuile.x == FeuillePerso.x && tuile.y == FeuillePerso.y)
            {
                return;
            }
            Character c = MyCharactersEndPoint.Move(FeuillePerso.name, tuile.x, tuile.y);
            if (c != null)
            {
                FeuillePerso = c;
            }
        }

        internal void Aller_GrandExchange()
        {
            Maps.Tile tuile = mcu.Map.Where(x => x.content != null && x.content.type == "grand_exchange").First();
            if (tuile.x == FeuillePerso.x && tuile.y == FeuillePerso.y)
            {
                return;
            }
            Character c = MyCharactersEndPoint.Move(FeuillePerso.name, tuile.x, tuile.y);
            if (c != null)
            {
                FeuillePerso = c;
            }
        }

        internal void AllerSurTuileMonstre(Monster monstre)
        {
            Maps.Tile tuile = mcu.Map.Where(x => x.content != null && x.content.type == "monster" && x.content.code == monstre.code).First();
            if (FeuillePerso.x == tuile.x && FeuillePerso.y == tuile.y)
            {
                return;
            }
            Character c = MyCharactersEndPoint.Move(FeuillePerso.name, tuile.x, tuile.y);
            if (c != null)
            {
                FeuillePerso = c;
            }
        }

        internal List<WorkOrder> RecupererCommande()
        {
            List<WorkOrder> WoRecupere = new List<WorkOrder>();
            foreach (WorkOrder wo in WorkOrderList)
            {
                if (mcu.ConsulterBanque().Where(x => x.code == wo.Code && Int32.Parse(x.quantity) >= wo.Quantité).Any())
                {
                    Character c =  mcu.RecupererDeBanque(FeuillePerso.name, wo.Code, wo.Quantité);
                    if (c != null)
                    {
                        FeuillePerso = c;
                    }
                    WoRecupere.Add(wo);
                }
            }
            foreach (WorkOrder wo in WoRecupere)
            {
                WorkOrderList.Remove(wo);
            }
            return WoRecupere;
        }

        internal bool ExistsInInventory(string code, int quantite = 1)
        {
            Inventory inventory = FeuillePerso.inventory.Where(x => x.code == code).FirstOrDefault();
            if (inventory == null)
            {
                return false;
            }
            if (inventory.quantity >= quantite)
            {
                return true;
            }
            return false;
        }

        internal int NombreDansInventaire(string code)
        {
            int nb = 0;

            Inventory inventory = FeuillePerso.inventory.Where(x => x.code == code).FirstOrDefault();
            if (inventory == null)
            {
                nb = 0;
            }
            else
            {
                nb = inventory.quantity;
            }
            return nb;
        }

        internal void RetirerAmulette()
        {
            Character c = MyCharactersEndPoint.UnequipItem(FeuillePerso.name, "amulet");
            if (c != null)
            {
                FeuillePerso = c;
            }
        }
        internal void EquiperAmulette(string code)
        {
            Character c = MyCharactersEndPoint.EquipItem(FeuillePerso.name, code, "amulet");
            if (c != null)
            {
                FeuillePerso = c;
            }
        }

        internal void RetirerBodyArmor()
        {
            Character c = MyCharactersEndPoint.UnequipItem(FeuillePerso.name, "body_armor");
            if (c != null)
            {
                FeuillePerso = c;
            }
        }
        internal void EquiperBodyArmor(string code)
        {
            Character c = MyCharactersEndPoint.EquipItem(FeuillePerso.name, code, "body_armor");
            if (c != null)
            {
                FeuillePerso = c;
            }
        }

        internal void RetirerWeapon()
        {
            Character c = MyCharactersEndPoint.UnequipItem(FeuillePerso.name, "weapon");
            if (c != null)
            {
                FeuillePerso = c;
            }
        }
        internal void EquiperWeapon(string code)
        {
            Character c = MyCharactersEndPoint.EquipItem(FeuillePerso.name, code, "weapon");
            if (c != null)
            {
                FeuillePerso = c;
            }
        }

        internal void RetirerLegArmor()
        {
            Character c = MyCharactersEndPoint.UnequipItem(FeuillePerso.name, "leg_armor");
            if (c != null)
            {
                FeuillePerso = c;
            }
        }
        internal void EquiperLegArmor(string code)
        {
            Character c = MyCharactersEndPoint.EquipItem(FeuillePerso.name, code, "leg_armor");
            if (c != null)
            {
                FeuillePerso = c;
            }
        }

        internal void RetirerHelmet()
        {
            Character c = MyCharactersEndPoint.UnequipItem(FeuillePerso.name, "helmet");
            if (c != null)
            {
                FeuillePerso = c;
            }
        }
        internal void EquiperHelmet(string code)
        {
            Character c = MyCharactersEndPoint.EquipItem(FeuillePerso.name, code, "helmet");
            if (c != null)
            {
                FeuillePerso = c;
            }
        }

        internal void RetirerBoots()
        {
            Character c = MyCharactersEndPoint.UnequipItem(FeuillePerso.name, "boots");
            if (c != null)
            {
                FeuillePerso = c;
            }
        }
        internal void EquiperBoots(string code)
        {
            Character c = MyCharactersEndPoint.EquipItem(FeuillePerso.name, code, "boots");
            if (c != null)
            {
                FeuillePerso = c;
            }
        }

        internal void RetirerShield()
        {
            Character c = MyCharactersEndPoint.UnequipItem(FeuillePerso.name, "shield");
            if (c != null)
            {
                FeuillePerso = c;
            }
        }
        internal void EquiperShield(string code)
        {
            Character c = MyCharactersEndPoint.EquipItem(FeuillePerso.name, code, "shield");
            if (c != null)
            {
                FeuillePerso = c;
            }
        }

        internal void RetirerAnneaux()
        {
            Character c = null;
            if (!String.IsNullOrEmpty(FeuillePerso.ring1_slot))
            {
                MyCharactersEndPoint.UnequipItem(FeuillePerso.name, "ring1");
                if (c != null)
                {
                    FeuillePerso = c;
                }
            }

            if (!String.IsNullOrEmpty(FeuillePerso.ring2_slot))
            {
                c = MyCharactersEndPoint.UnequipItem(FeuillePerso.name, "ring2");
                if (c != null)
                {
                    FeuillePerso = c;
                }
            }
        }
        internal void EquiperAnneau(string code)
        {
            //de la merde
            if (String.IsNullOrEmpty(FeuillePerso.ring1_slot))
            {
                Character c = MyCharactersEndPoint.EquipItem(FeuillePerso.name,code, "ring1");
                if (c != null)
                {
                    FeuillePerso = c;
                }
            }
            else
            {
                Character c = MyCharactersEndPoint.EquipItem(FeuillePerso.name,code, "ring2");
                if (c != null)
                {
                    FeuillePerso = c;
                }
            }
        }

        internal void RetirerArtefacts()
        {
            if (!String.IsNullOrEmpty(FeuillePerso.artifact1_slot))
            {
                Character c = MyCharactersEndPoint.UnequipItem(FeuillePerso.name, "artifact1");
                if (c != null)
                {
                    FeuillePerso = c;
                }
            }
            if (!String.IsNullOrEmpty(FeuillePerso.artifact2_slot))
            {
                Character c = MyCharactersEndPoint.UnequipItem(FeuillePerso.name, "artifact2");
                if (c != null)
                {
                    FeuillePerso = c;
                }
            }
            if (!String.IsNullOrEmpty(FeuillePerso.artifact3_slot))
            {
                Character c = MyCharactersEndPoint.UnequipItem(FeuillePerso.name, "artifact3");
                if (c != null)
                {
                    FeuillePerso = c;
                }
            }
        }
        internal void EquiperArtefact(string code)
        {
            if (String.IsNullOrEmpty(FeuillePerso.artifact1_slot))
            {
                Character c = MyCharactersEndPoint.EquipItem(FeuillePerso.name, code, "artifact1");
                if (c != null)
                {
                    FeuillePerso = c;
                }
            }
            else if (String.IsNullOrEmpty(FeuillePerso.artifact2_slot))
            {
                Character c = MyCharactersEndPoint.EquipItem(FeuillePerso.name, code, "artifact2");
                if (c != null)
                {
                    FeuillePerso = c;
                }
            }
            else
            {
                Character c = MyCharactersEndPoint.EquipItem(FeuillePerso.name, code, "artifact3");
                if (c != null)
                {
                    FeuillePerso = c;
                }
            }
        }

        internal int NombreDansBanque(string code)
        {
            Item item = mcu.ConsulterBanque().Where(x => x.code == code).FirstOrDefault();
            if (item == null)
            {
                return 0;
            }
            return Int32.Parse(item.quantity);
        }
    }
}
