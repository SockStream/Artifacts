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
        public List<WorkOrder> WorkOrderList { get; set; }
        public Character FeuillePerso { get; set; }
        public bool doitViderEnBanque = false;
        public bool termine = false;
        public bool NiveauMetierGagne = true, CommandeEnAttente = false, NouveauWorkOrder = false;
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
            if (metier == Constantes.fighting)
            {
                niveauMetier = FeuillePerso.level;
            }
            return niveauMetier;
        }
        internal void FarmerMeilleureRessource(string metier)
        {
            if (GetNiveauMetier() == 10)
            {
                Console.WriteLine("youpi !");
            }
            int niveauMetier_actuel = GetNiveauMetier();
            //on cherche la meilleure ressource associée à notre métier, on se déplace sur la case si on est pas déjà dessus, et on fait 1 Gather
            Resource meilleureResource = mcu.ResourceList.Where(x => x.skill == metier && x.level <= niveauMetier_actuel).OrderByDescending(x => x.level).First();
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
            Character c = MyCharactersEndPoint.Move(FeuillePerso.name, tuile.x, tuile.y);
            if (c != null)
            {
                FeuillePerso = c;
            }
        }

        internal void Retirer_Banque(string code, int quantity)
        {

        }

        internal void AllerSurTuileMonstre(Monster monstre)
        {
            Maps.Tile tuile = mcu.Map.Where(x => x.content != null && x.content.type == "monster" && x.content.code == monstre.code).First();
            Character c = MyCharactersEndPoint.Move(FeuillePerso.name, tuile.x, tuile.y);
            if (c != null)
            {
                FeuillePerso = c;
            }
        }

        internal void RecupererCommande()
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
                    mcu.SupprimerCommandeLivree(wo);
                }
            }
            foreach (WorkOrder wo in WoRecupere)
            {
                EquiperCommande(wo);
                WorkOrderList.Remove(wo);
            }
        }

        private void EquiperCommande(WorkOrder wo)
        {
            //todo
            throw new NotImplementedException();
        }

        internal bool ExistsInInventory(string code)
        {
            return FeuillePerso.inventory.Where(x => x.code == code).Any();
        }

        internal void RetirerAmulette()
        {
            Character c = MyCharactersEndPoint.UnequipItem(FeuillePerso.name, "amulet_slot");
            if (c != null)
            {
                FeuillePerso = c;
            }
        }
        internal void EquiperAmulette(string code)
        {
            Character c = MyCharactersEndPoint.EquipItem(FeuillePerso.name, code, "amulet_slot");
            if (c != null)
            {
                FeuillePerso = c;
            }
        }

        internal void RetirerBodyArmor()
        {
            Character c = MyCharactersEndPoint.UnequipItem(FeuillePerso.name, "body_armor_slot");
            if (c != null)
            {
                FeuillePerso = c;
            }
        }
        internal void EquiperBodyArmor(string code)
        {
            Character c = MyCharactersEndPoint.EquipItem(FeuillePerso.name, code, "body_armor_slot");
            if (c != null)
            {
                FeuillePerso = c;
            }
        }

        internal void RetirerWeapon()
        {
            Character c = MyCharactersEndPoint.UnequipItem(FeuillePerso.name, "weapon_slot");
            if (c != null)
            {
                FeuillePerso = c;
            }
        }
        internal void EquiperWeapon(string code)
        {
            Character c = MyCharactersEndPoint.EquipItem(FeuillePerso.name, code, "weapon_slot");
            if (c != null)
            {
                FeuillePerso = c;
            }
        }

        internal void RetirerLegArmor()
        {
            Character c = MyCharactersEndPoint.UnequipItem(FeuillePerso.name, "leg_armor_slot");
            if (c != null)
            {
                FeuillePerso = c;
            }
        }
        internal void EquiperLegArmor(string code)
        {
            Character c = MyCharactersEndPoint.EquipItem(FeuillePerso.name, code, "leg_armor_slot");
            if (c != null)
            {
                FeuillePerso = c;
            }
        }

        internal void RetirerHelmet()
        {
            Character c = MyCharactersEndPoint.UnequipItem(FeuillePerso.name, "helmet_slot");
            if (c != null)
            {
                FeuillePerso = c;
            }
        }
        internal void EquiperHelmet(string code)
        {
            Character c = MyCharactersEndPoint.EquipItem(FeuillePerso.name, code, "helmet_slot");
            if (c != null)
            {
                FeuillePerso = c;
            }
        }

        internal void RetirerBoots()
        {
            Character c = MyCharactersEndPoint.UnequipItem(FeuillePerso.name, "boots_slot");
            if (c != null)
            {
                FeuillePerso = c;
            }
        }
        internal void EquiperBoots(string code)
        {
            Character c = MyCharactersEndPoint.EquipItem(FeuillePerso.name, code, "boots_slot");
            if (c != null)
            {
                FeuillePerso = c;
            }
        }

        internal void RetirerShield()
        {
            Character c = MyCharactersEndPoint.UnequipItem(FeuillePerso.name, "shield_slot");
            if (c != null)
            {
                FeuillePerso = c;
            }
        }
        internal void EquiperShield(string code)
        {
            Character c = MyCharactersEndPoint.EquipItem(FeuillePerso.name, code, "shield_slot");
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
                MyCharactersEndPoint.UnequipItem(FeuillePerso.name, "ring1_slot");
                if (c != null)
                {
                    FeuillePerso = c;
                }
            }

            if (!String.IsNullOrEmpty(FeuillePerso.ring1_slot))
            {
                c = MyCharactersEndPoint.UnequipItem(FeuillePerso.name, "ring2_slot");
                if (c != null)
                {
                    FeuillePerso = c;
                }
            }
        }
        internal void EquiperAnneau(string code)
        {
            if (String.IsNullOrEmpty(FeuillePerso.ring1_slot))
            {
                Character c = MyCharactersEndPoint.EquipItem(FeuillePerso.name,code, "ring1_slot");
                if (c != null)
                {
                    FeuillePerso = c;
                }
            }
            else
            {
                Character c = MyCharactersEndPoint.EquipItem(FeuillePerso.name,code, "ring2_slot");
                if (c != null)
                {
                    FeuillePerso = c;
                }
            }
        }

        internal void RetirerArtefacts()
        {
            Character c = MyCharactersEndPoint.UnequipItem(FeuillePerso.name, "artifact1_slot");
            if (c != null)
            {
                FeuillePerso = c;
            }
            c = MyCharactersEndPoint.UnequipItem(FeuillePerso.name, "artifact2_slot");
            if (c != null)
            {
                FeuillePerso = c;
            }
            c = MyCharactersEndPoint.UnequipItem(FeuillePerso.name, "artifact3_slot");
            if (c != null)
            {
                FeuillePerso = c;
            }
        }
        internal void EquiperArtefact(string code)
        {
            if (String.IsNullOrEmpty(FeuillePerso.artifact1_slot))
            {
                Character c = MyCharactersEndPoint.EquipItem(FeuillePerso.name, code, "artifact1_slot");
                if (c != null)
                {
                    FeuillePerso = c;
                }
            }
            else if (String.IsNullOrEmpty(FeuillePerso.artifact2_slot))
            {
                Character c = MyCharactersEndPoint.EquipItem(FeuillePerso.name, code, "artifact2_slot");
                if (c != null)
                {
                    FeuillePerso = c;
                }
            }
            else
            {
                Character c = MyCharactersEndPoint.EquipItem(FeuillePerso.name, code, "artifact3_slot");
                if (c != null)
                {
                    FeuillePerso = c;
                }
            }
        }
    }
}
