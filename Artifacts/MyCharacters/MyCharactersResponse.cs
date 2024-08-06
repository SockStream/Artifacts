using Artifacts.Utilities;
using Artifacts.Utilities.Network;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Artifacts.Monsters;
using Artifacts.Items;

namespace Artifacts.MyCharacters
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Character
    {
        public string name { get; set; }
        public string skin { get; set; }
        public int level { get; set; }
        public int xp { get; set; }
        public int max_xp { get; set; }
        public int total_xp { get; set; }
        public int gold { get; set; }
        public int speed { get; set; }
        public int mining_level { get; set; }
        public int mining_xp { get; set; }
        public int mining_max_xp { get; set; }
        public int woodcutting_level { get; set; }
        public int woodcutting_xp { get; set; }
        public int woodcutting_max_xp { get; set; }
        public int fishing_level { get; set; }
        public int fishing_xp { get; set; }
        public int fishing_max_xp { get; set; }
        public int weaponcrafting_level { get; set; }
        public int weaponcrafting_xp { get; set; }
        public int weaponcrafting_max_xp { get; set; }
        public int gearcrafting_level { get; set; }
        public int gearcrafting_xp { get; set; }
        public int gearcrafting_max_xp { get; set; }
        public int jewelrycrafting_level { get; set; }
        public int jewelrycrafting_xp { get; set; }
        public int jewelrycrafting_max_xp { get; set; }
        public int cooking_level { get; set; }
        public int cooking_xp { get; set; }
        public int cooking_max_xp { get; set; }
        public int hp { get; set; }
        public int haste { get; set; }
        public int critical_strike { get; set; }
        public int stamina { get; set; }
        public int attack_fire { get; set; }
        public int attack_earth { get; set; }
        public int attack_water { get; set; }
        public int attack_air { get; set; }
        public int dmg_fire { get; set; }
        public int dmg_earth { get; set; }
        public int dmg_water { get; set; }
        public int dmg_air { get; set; }
        public int res_fire { get; set; }
        public int res_earth { get; set; }
        public int res_water { get; set; }
        public int res_air { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int cooldown { get; set; }
        public DateTime cooldown_expiration { get; set; }
        public string weapon_slot { get; set; }
        public string shield_slot { get; set; }
        public string helmet_slot { get; set; }
        public string body_armor_slot { get; set; }
        public string leg_armor_slot { get; set; }
        public string boots_slot { get; set; }
        public string ring1_slot { get; set; }
        public string ring2_slot { get; set; }
        public string amulet_slot { get; set; }
        public string artifact1_slot { get; set; }
        public string artifact2_slot { get; set; }
        public string artifact3_slot { get; set; }
        public string consumable1_slot { get; set; }
        public int consumable1_slot_quantity { get; set; }
        public string consumable2_slot { get; set; }
        public int consumable2_slot_quantity { get; set; }
        public string task { get; set; }
        public string task_type { get; set; }
        public int task_progress { get; set; }
        public int task_total { get; set; }
        public int inventory_max_items { get; set; }
        public List<Inventory> inventory { get; set; }
        public string inventory_slot1 { get; set; }
        public int inventory_slot1_quantity { get; set; }
        public string inventory_slot2 { get; set; }
        public int inventory_slot2_quantity { get; set; }
        public string inventory_slot3 { get; set; }
        public int inventory_slot3_quantity { get; set; }
        public string inventory_slot4 { get; set; }
        public int inventory_slot4_quantity { get; set; }
        public string inventory_slot5 { get; set; }
        public int inventory_slot5_quantity { get; set; }
        public string inventory_slot6 { get; set; }
        public int inventory_slot6_quantity { get; set; }
        public string inventory_slot7 { get; set; }
        public int inventory_slot7_quantity { get; set; }
        public string inventory_slot8 { get; set; }
        public int inventory_slot8_quantity { get; set; }
        public string inventory_slot9 { get; set; }
        public int inventory_slot9_quantity { get; set; }
        public string inventory_slot10 { get; set; }
        public int inventory_slot10_quantity { get; set; }
        public string inventory_slot11 { get; set; }
        public int inventory_slot11_quantity { get; set; }
        public string inventory_slot12 { get; set; }
        public int inventory_slot12_quantity { get; set; }
        public string inventory_slot13 { get; set; }
        public int inventory_slot13_quantity { get; set; }
        public string inventory_slot14 { get; set; }
        public int inventory_slot14_quantity { get; set; }
        public string inventory_slot15 { get; set; }
        public int inventory_slot15_quantity { get; set; }
        public string inventory_slot16 { get; set; }
        public int inventory_slot16_quantity { get; set; }
        public string inventory_slot17 { get; set; }
        public int inventory_slot17_quantity { get; set; }
        public string inventory_slot18 { get; set; }
        public int inventory_slot18_quantity { get; set; }
        public string inventory_slot19 { get; set; }
        public int inventory_slot19_quantity { get; set; }
        public string inventory_slot20 { get; set; }
        public int inventory_slot20_quantity { get; set; }
    }

    public class Content
    {
        public string type { get; set; }
        public string code { get; set; }
    }

    public class Cooldown
    {
        public int total_seconds { get; set; }
        public int remaining_seconds { get; set; }
        public int totalSeconds { get; set; }
        public int remainingSeconds { get; set; }
        public DateTime expiration { get; set; }
        public string reason { get; set; }
    }

    public class Fight
    {
        public int xp { get; set; }
        public int gold { get; set; }
        public List<Drop> drops { get; set; }
        public int turns { get; set; }
        public MonsterBlockedHits monster_blocked_hits { get; set; }
        public PlayerBlockedHits player_blocked_hits { get; set; }
        public List<string> logs { get; set; }
        public string result { get; set; }
    }

    public class MonsterBlockedHits
    {
        public int fire { get; set; }
        public int earth { get; set; }
        public int water { get; set; }
        public int air { get; set; }
        public int total { get; set; }
    }

    public class PlayerBlockedHits
    {
        public int fire { get; set; }
        public int earth { get; set; }
        public int water { get; set; }
        public int air { get; set; }
        public int total { get; set; }
    }

    public class Effect
    {
        public string name { get; set; }
        public int value { get; set; }
    }

    public class Craft
    {
        public string skill { get; set; }
        public int level { get; set; }
        public List<Item> items { get; set; }
        public int quantity { get; set; }
    }

    public class Data
    {
        public Cooldown cooldown { get; set; }
        public string slot { get; set; }
        public Destination destination { get; set; }
        public Character character { get; set; }
        public Fight fight { get; set; }

        public Item item { get; set; }
    }

    public class Destination
    {
        public string name { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public Content content { get; set; }
    }

    public class Inventory
    {
        public int slot { get; set; }
        public string code { get; set; }
        public int quantity { get; set; }
    }

    public class MyCharactersResponse
    {
        public Data data { get; set; }
    }

    public class AllMyCharactersResponse
    {
        public List<Character> data { get; set; }
    }


}
