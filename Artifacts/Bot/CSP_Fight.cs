using Artifacts.Bot.Personnage;
using Artifacts.Items;
using Artifacts.Monsters;
using Artifacts.MyCharacters;
using Artifacts.Utilities;
using Decider.Csp.BaseTypes;
using Decider.Csp.Integer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Artifacts.Bot
{
    internal class CSP_Fight
    {
        private Personnage.Personnage p;
        private Monster m;
        private static List<Objet> ObjetList;



        public CSP_Fight(Personnage.Personnage perso, Monster monster, List<Objet> objetlist)
        {
            p = perso;
            m = monster;
            ObjetList = objetlist;
        }

        public IList<IDictionary<string,IVariable<int>>> CalculerSolutions(List<Objet> liste_arme, List<Objet> liste_helmet, List<Objet> liste_shield, List<Objet> liste_BodyArmor, List<Objet> liste_Amulet, List<Objet> liste_LegArmor, List<Objet> liste_Boots, List<Objet> liste_Rings, List<Objet> liste_Artifacts)
        {
            var weapon = new VariableInteger("weapon", -1, -1);
            var helmet = new VariableInteger("helmet", -1, -1);
            var shield = new VariableInteger("shield", -1, -1);
            var body_armor = new VariableInteger("body_armor", -1, -1);
            var amulet = new VariableInteger("amulet", -1, -1);
            var leg_armor = new VariableInteger("leg_armor",-1,-1);
            var boots = new VariableInteger("boots",-1,-1);
            var ring1 = new VariableInteger("ring1", -1, -1);
            var ring2 = new VariableInteger("ring2", -1, -1);
            var artifact1 = new VariableInteger("artifact1", -1, -1);
            var artifact2 = new VariableInteger("artifact2", -1, -1); //merde, il faut améliorer pour pouvoir les utiliser
            var artifact3 = new VariableInteger("artifact3", -1, -1);

            if (liste_arme.Count > 0)
            {
                weapon = new VariableInteger(Constantes.weapon, 0, liste_arme.Count - 1);
            }

            if (liste_helmet.Count > 0)
            {
                helmet = new VariableInteger(Constantes.helmet, 0, liste_helmet.Count - 1);
            }

            if (liste_shield.Count > 0)
            {
                shield = new VariableInteger(Constantes.shield, 0, liste_shield.Count - 1);
            }

            if (liste_BodyArmor.Count > 0)
            {
                body_armor = new VariableInteger(Constantes.bodyArmor, 0, liste_BodyArmor.Count - 1);
            }

            if (liste_Amulet.Count > 0)
            {
                amulet = new VariableInteger(Constantes.amulet, 0, liste_Amulet.Count - 1);
            }

            if (liste_LegArmor.Count > 0)
            {
                leg_armor = new VariableInteger(Constantes.leg_armor, 0, liste_LegArmor.Count - 1);
            }

            if (liste_Boots.Count > 0)
            {
                boots = new VariableInteger(Constantes.boots, 0, liste_Boots.Count -1);
            }

            if (liste_Rings.Count > 0)
            {
                ring1 = new VariableInteger(Constantes.ring1, 0, liste_Rings.Count - 1);
                ring2 = new VariableInteger(Constantes.ring2, 0, liste_Rings.Count - 1);
            }
            if (liste_Artifacts.Count > 0) //todo : mal géré
            {
                artifact1 = new VariableInteger(Constantes.artifact1, 0, liste_Artifacts.Count - 1);
                artifact2 = new VariableInteger(Constantes.artifact2, 0, liste_Artifacts.Count - 1);
                artifact3 = new VariableInteger(Constantes.artifact3, 0, liste_Artifacts.Count - 1);
            }

            VariableInteger res = new VariableInteger("resultat", 1, 1);
            var constraints = new List<IConstraint>
            {
            };
            var variables = new[] { weapon, helmet, shield, body_armor, amulet, leg_armor, boots, ring1, ring2, artifact1, artifact2, artifact3 };
            //var variables = new[] { weapon, helmet, shield, body_armor, amulet, leg_armor, boots, ring1,  artifact1};
            var state = new StateInteger(variables, constraints);
            StateOperationResult searchResult = state.SearchAllSolutions();
            //Console.WriteLine(state.Runtime);
            //Console.WriteLine(state.Solutions.Count);
            return state.Solutions;
        }

        private bool Tuable(Objet weapon,
            Objet helmet,
            Objet shield,
            Objet body_armor,
            Objet amulet,
            Objet leg_armor,
            Objet boots,
            Objet ring1,
            Objet ring2,
            Objet artifacts1,
            Objet artifacts2,
            Objet artifacts3)
        {
            int hp_joueur = p.FeuillePerso.hp;
            int hp_monstre = m.hp;
            bool victoire = false;
            bool finCombat = false;
            int i = 1;

            int attaque_feu = 0, attaque_eau = 0, attaque_terre = 0, attaque_air = 0;
            int degat_feu = 0, degat_eau = 0, degat_terre = 0, degat_air = 0;
            int res_feu = 0, res_eau = 0, res_terre = 0, res_air = 0;

            List<Objet> ListeEquipement = new List<Objet>();
            ListeEquipement.Add(weapon);
            ListeEquipement.Add(helmet);
            ListeEquipement.Add(shield);
            ListeEquipement.Add(body_armor);
            ListeEquipement.Add(amulet);
            ListeEquipement.Add(leg_armor);
            ListeEquipement.Add(boots);
            ListeEquipement.Add(ring1);
            ListeEquipement.Add(ring2);
            ListeEquipement.Add(artifacts1);
            ListeEquipement.Add(artifacts2);
            ListeEquipement.Add(artifacts3);


            Objet conso1 = null, conso2 = null;

            conso1 = ObjetList.Where(x => x.type == "consumable" && x.code == p.FeuillePerso.consumable1_slot).FirstOrDefault();
            conso2 = ObjetList.Where(x => x.type == "consumable" && x.code == p.FeuillePerso.consumable2_slot).FirstOrDefault();

            int boost_fire_dmg = 0, boost_water_dmg = 0, boost_earth_dmg = 0, boost_air_dmg = 0;
            if (conso1 != null)
            {
                if (conso1.effects.Where(x => x.name.Equals("boost_dmg_air")).Any())
                {
                    boost_air_dmg += conso1.effects.Where(x => x.name.Equals("boost_dmg_air")).First().value;
                }
                if (conso1.effects.Where(x => x.name.Equals("boost_dmg_fire")).Any())
                {
                    boost_fire_dmg += conso1.effects.Where(x => x.name.Equals("boost_dmg_fire")).First().value;
                }

                if (conso1.effects.Where(x => x.name.Equals("boost_dmg_earth")).Any())
                {
                    boost_earth_dmg += conso1.effects.Where(x => x.name.Equals("boost_dmg_earth")).First().value;
                }

                if (conso1.effects.Where(x => x.name.Equals("boost_dmg_water")).Any())
                {
                    boost_water_dmg += conso1.effects.Where(x => x.name.Equals("boost_dmg_water")).First().value;
                }
            }

            foreach(Objet obj in ListeEquipement)
            {
                if (obj != null && obj.effects != null)
                {
                    #region attaque_types
                    Artifacts.Items.Effect effet_atk_feu = obj.effects.Where(x => x.name == Constantes.attack_fire).FirstOrDefault();
                    if (effet_atk_feu != null)
                    {
                        attaque_feu += effet_atk_feu.value;
                    }
                    Artifacts.Items.Effect effet_atk_eau = obj.effects.Where(x => x.name == Constantes.attack_water).FirstOrDefault();
                    if (effet_atk_eau != null)
                    {
                        attaque_eau += effet_atk_eau.value;
                    }
                    Artifacts.Items.Effect effet_atk_terre = obj.effects.Where(x => x.name == Constantes.attack_earth).FirstOrDefault();
                    if (effet_atk_terre != null)
                    {
                        attaque_terre += effet_atk_terre.value;
                    }
                    Artifacts.Items.Effect effet_atk_air = obj.effects.Where(x => x.name == Constantes.attack_air).FirstOrDefault();
                    if (effet_atk_air != null)
                    {
                        attaque_air += effet_atk_air.value;
                    }
                    #endregion
                    #region degats_types
                    Artifacts.Items.Effect effet_dmg_feu = obj.effects.Where(x => x.name == Constantes.dmg_fire).FirstOrDefault();
                    if (effet_dmg_feu != null)
                    {
                        degat_feu += effet_dmg_feu.value;
                    }
                    Artifacts.Items.Effect effet_dmg_eau = obj.effects.Where(x => x.name == Constantes.dmg_water).FirstOrDefault();
                    if (effet_dmg_eau != null)
                    {
                        degat_eau += effet_dmg_eau.value;
                    }
                    Artifacts.Items.Effect effet_dmg_terre = obj.effects.Where(x => x.name == Constantes.dmg_earth).FirstOrDefault();
                    if (effet_dmg_terre != null)
                    {
                        degat_terre += effet_dmg_terre.value;
                    }
                    Artifacts.Items.Effect effet_dmg_air = obj.effects.Where(x => x.name == Constantes.dmg_air).FirstOrDefault();
                    if (effet_dmg_air != null)
                    {
                        degat_air += effet_dmg_air.value;
                    }
                    #endregion
                    #region res_types
                    Artifacts.Items.Effect effet_res_feu = obj.effects.Where(x => x.name == Constantes.res_fire).FirstOrDefault();
                    if (effet_res_feu != null)
                    {
                        res_feu += effet_res_feu.value;
                    }
                    Artifacts.Items.Effect effet_res_eau = obj.effects.Where(x => x.name == Constantes.res_water).FirstOrDefault();
                    if (effet_res_eau != null)
                    {
                        res_eau += effet_res_eau.value;
                    }
                    Artifacts.Items.Effect effet_res_terre = obj.effects.Where(x => x.name == Constantes.res_earth).FirstOrDefault();
                    if (effet_res_terre != null)
                    {
                        res_terre += effet_res_terre.value;
                    }
                    Artifacts.Items.Effect effet_res_air = obj.effects.Where(x => x.name == Constantes.res_air).FirstOrDefault();
                    if (effet_res_air != null)
                    {
                        res_air += effet_res_air.value;
                    }
                    #endregion
                }
            }

            while ( i <= 50 && !finCombat)
            {
                int pv_perdu_feu = (int)(attaque_feu * (1 + (double)((degat_feu + boost_fire_dmg) / 100)) * (1 - ((double)m.res_fire / 100)));
                int pv_perdu_eau = (int)(attaque_eau * (1 + (double)((degat_eau + boost_water_dmg) / 100)) * (1 - ((double)m.res_water / 100)));
                int pv_perdu_terre = (int)(attaque_terre * (1 + (double)((degat_terre + boost_earth_dmg) / 100)) * (1 - ((double)m.res_earth / 100)));
                int pv_perdu_air = (int)(attaque_air * (1 + (double)((degat_air + boost_air_dmg) / 100)) * (1 - ((double)m.res_air / 100)));

                hp_monstre = hp_monstre - pv_perdu_feu - pv_perdu_eau - pv_perdu_terre - pv_perdu_air;

                if (hp_monstre <= 0)
                {
                    victoire = true;
                    finCombat = true;
                    continue;
                }


                pv_perdu_feu = (int)Math.Ceiling(m.attack_fire * (1) * (1 - ((double)res_feu / 100)));
                pv_perdu_eau = (int)Math.Ceiling(m.attack_water * (1) * (1 - ((double)res_eau / 100)));
                pv_perdu_terre = (int)Math.Ceiling(m.attack_earth * (1) * (1 - ((double)res_terre / 100)));
                pv_perdu_air = (int)Math.Ceiling(m.attack_air * (1) * (1 - ((double)res_air / 100)));

                hp_joueur = hp_joueur - pv_perdu_feu - pv_perdu_eau - pv_perdu_terre - pv_perdu_air;



                if (hp_joueur <= 0)
                {
                    victoire = false;
                    finCombat = true;
                    continue;
                }
                i++;
            }

            return victoire;
        }

        public List<Tuple<Enums.EmplacementPieceStuff, string>> GenererListePossibilites()
        {
            List<Objet> liste_arme = new List<Objet>();
            List<Objet> liste_helmet = new List<Objet>();
            List<Objet> liste_shield = new List<Objet>();
            List<Objet> liste_BodyArmor = new List<Objet>();
            List<Objet> liste_Amulet = new List<Objet>();
            List<Objet> liste_LegArmor = new List<Objet>();
            List<Objet> liste_Boots = new List<Objet>();
            List<Objet> liste_Rings = new List<Objet>();
            List<Objet> liste_Artifacts = new List<Objet>();
            List<Item> itemsEnBanque = new List<Item>();

            foreach (Inventory inv in p.FeuillePerso.inventory)
            {
                if (!String.IsNullOrEmpty(inv.code))
                {
                    Item item = new Item();
                    item.code = inv.code;
                    itemsEnBanque.Add(item);
                }
            }
            //stuff_équipé
            Item item_porte = new Item();
            item_porte.code = p.FeuillePerso.weapon_slot;
            itemsEnBanque.Add(item_porte);
            item_porte = new Item();
            item_porte.code = p.FeuillePerso.shield_slot;
            itemsEnBanque.Add(item_porte);
            item_porte = new Item();
            item_porte.code = p.FeuillePerso.helmet_slot;
            itemsEnBanque.Add(item_porte);
            item_porte = new Item();
            item_porte.code = p.FeuillePerso.body_armor_slot;
            itemsEnBanque.Add(item_porte);
            item_porte = new Item();
            item_porte.code = p.FeuillePerso.leg_armor_slot;
            itemsEnBanque.Add(item_porte);
            item_porte = new Item();
            item_porte.code = p.FeuillePerso.boots_slot;
            itemsEnBanque.Add(item_porte);
            item_porte = new Item();
            item_porte.code = p.FeuillePerso.ring1_slot;
            itemsEnBanque.Add(item_porte);
            item_porte = new Item();
            item_porte.code = p.FeuillePerso.ring2_slot;
            itemsEnBanque.Add(item_porte);
            item_porte = new Item();
            item_porte.code = p.FeuillePerso.amulet_slot;
            itemsEnBanque.Add(item_porte);
            item_porte = new Item();
            item_porte.code = p.FeuillePerso.artifact1_slot;
            itemsEnBanque.Add(item_porte);
            item_porte = new Item();
            item_porte.code = p.FeuillePerso.artifact1_slot;
            itemsEnBanque.Add(item_porte);
            item_porte = new Item();
            item_porte.code = p.FeuillePerso.artifact3_slot;
            itemsEnBanque.Add(item_porte);

            foreach (Item item in itemsEnBanque)
            {
                Objet obj = ObjetList.Where (x => x.code == item.code).FirstOrDefault();
                if (obj != null)
                {
                    if (obj.type == Constantes.weapon)
                    {
                        liste_arme.Add(obj);
                    }
                    if (obj.type == Constantes.helmet)
                    {
                        liste_helmet.Add(obj);
                    }
                    if (obj.type == Constantes.shield)
                    {
                        liste_shield.Add(obj);
                    }
                    if (obj.type == Constantes.bodyArmor)
                    {
                        liste_BodyArmor.Add(obj);
                    }
                    if (obj.type == Constantes.amulet)
                    {
                        liste_Amulet.Add(obj);
                    }
                    if (obj.type == Constantes.leg_armor)
                    {
                        liste_LegArmor.Add(obj);
                    }
                    if (obj.type == Constantes.boots)
                    {
                        liste_Boots.Add(obj);
                    }
                    if (obj.type == Constantes.ring)
                    {
                        liste_Rings.Add(obj);
                    }
                    if (obj.type == Constantes.artifact)
                    {
                    liste_Artifacts.Add(obj);
                    }

                }
            }

            List<IDictionary<string, IVariable<int>>> resultat = CalculerSolutions(
                liste_arme,
                liste_helmet,
                liste_shield,
                liste_BodyArmor,
                liste_Amulet,
                liste_LegArmor,
                liste_Boots,
                liste_Rings,
                liste_Artifacts
                ).ToList();

            Objet weapon = null;
            Objet helmet = null;
            Objet shield = null;
            Objet body_armor = null;
            Objet amulet = null;
            Objet leg_armor = null;
            Objet boots = null;
            Objet ring1 = null;
            Objet ring2 = null;
            Objet artifact1 = null;
            Objet artifact2 = null;
            Objet artifact3 = null;


            List<Tuple<Enums.EmplacementPieceStuff, string>> listeStuff = new List<Tuple<Enums.EmplacementPieceStuff, string>>();

            foreach (IDictionary<string, IVariable<int>> sol in resultat)
            {
                foreach (IVariable<int> variable in sol.Values)
                {
                    //Console.WriteLine(variable.Name + variable.InstantiatedValue);
                    if (variable.Name == Constantes.weapon && variable.InstantiatedValue >= 0)
                    {
                        weapon = liste_arme[variable.InstantiatedValue];
                    }
                    if (variable.Name == Constantes.helmet && variable.InstantiatedValue >= 0)
                    {
                        helmet = liste_helmet[variable.InstantiatedValue];
                    }
                    if (variable.Name == Constantes.shield && variable.InstantiatedValue >= 0)
                    {
                        shield = liste_shield[variable.InstantiatedValue];
                    }
                    if (variable.Name == Constantes.bodyArmor && variable.InstantiatedValue >= 0)
                    {
                        body_armor = liste_BodyArmor[variable.InstantiatedValue];
                    }
                    if (variable.Name == Constantes.amulet && variable.InstantiatedValue >= 0)
                    {
                        amulet = liste_Amulet[variable.InstantiatedValue];
                    }
                    if (variable.Name == Constantes.leg_armor && variable.InstantiatedValue >= 0)
                    {
                        leg_armor = liste_LegArmor[variable.InstantiatedValue];
                    }
                    if (variable.Name == Constantes.boots && variable.InstantiatedValue >= 0)
                    {
                        boots = liste_Boots[variable.InstantiatedValue];
                    }
                    if (variable.Name == Constantes.ring1 && variable.InstantiatedValue >= 0)
                    {
                        ring1 = liste_Rings[variable.InstantiatedValue];
                    }
                    if (variable.Name == Constantes.ring2 && variable.InstantiatedValue >= 0)
                    {
                        ring2 = liste_Rings[variable.InstantiatedValue];
                    }
                    if (variable.Name == Constantes.artifact1 && variable.InstantiatedValue >= 0)
                    {
                        artifact1 = liste_Artifacts[variable.InstantiatedValue];
                    }
                    if (variable.Name == Constantes.artifact2 && variable.InstantiatedValue >= 0)
                    {
                        artifact2 = liste_Artifacts[variable.InstantiatedValue];
                    }
                    if (variable.Name == Constantes.artifact3 && variable.InstantiatedValue >= 0)
                    {
                        artifact3 = liste_Artifacts[variable.InstantiatedValue];
                    }
                }

                if (Tuable(weapon,helmet,shield,body_armor,amulet,leg_armor,boots,ring1,ring2,artifact1,artifact2 ,artifact3))
                {
                    if (weapon != null)
                    {
                        listeStuff.Add(new Tuple<Enums.EmplacementPieceStuff,string>(Enums.EmplacementPieceStuff.weapon, weapon.code));
                    }
                    if (helmet != null)
                    {
                        listeStuff.Add(new Tuple<Enums.EmplacementPieceStuff, string>(Enums.EmplacementPieceStuff.helmet,helmet.code));
                    }
                    if (shield != null)
                    {
                        listeStuff.Add(new Tuple<Enums.EmplacementPieceStuff, string>(Enums.EmplacementPieceStuff.shield,shield.code));
                    }
                    if (body_armor != null)
                    {
                        listeStuff.Add(new Tuple<Enums.EmplacementPieceStuff, string>(Enums.EmplacementPieceStuff.body_armor,body_armor.code));
                    }
                    if (leg_armor != null)
                    {
                        listeStuff.Add(new Tuple<Enums.EmplacementPieceStuff, string>(Enums.EmplacementPieceStuff.leg_armor,leg_armor.code));
                    }
                    if (boots != null)
                    {
                        listeStuff.Add(new Tuple<Enums.EmplacementPieceStuff, string>(Enums.EmplacementPieceStuff.boots,boots.code));
                    }
                    if (ring1 != null)
                    {
                        listeStuff.Add(new Tuple<Enums.EmplacementPieceStuff, string>(Enums.EmplacementPieceStuff.ring1,ring1.code));
                    }
                    if (ring2 != null)
                    {
                        listeStuff.Add(new Tuple<Enums.EmplacementPieceStuff, string>(Enums.EmplacementPieceStuff.ring2,ring2.code));
                    }
                    if (artifact1 != null)
                    {
                        listeStuff.Add(new Tuple<Enums.EmplacementPieceStuff, string>(Enums.EmplacementPieceStuff.artifact1,artifact1.code));
                    }
                    if (artifact2 != null)
                    {
                        listeStuff.Add(new Tuple<Enums.EmplacementPieceStuff, string>(Enums.EmplacementPieceStuff.artifact2,artifact2.code));
                    }
                    if (artifact3 != null)
                    {
                        listeStuff.Add(new Tuple<Enums.EmplacementPieceStuff, string>(Enums.EmplacementPieceStuff.artifact3,artifact3.code));
                    }
                    break;
                }
                //Console.WriteLine("");
            }
            
            return listeStuff;
        }



    }
}
