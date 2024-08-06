using Artifacts.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifacts.Monsters
{
    public class Monster
    {
        public string name { get; set; }
        public string code { get; set; }
        public int level { get; set; }
        public int hp { get; set; }
        public int attack_fire { get; set; }
        public int attack_earth { get; set; }
        public int attack_water { get; set; }
        public int attack_air { get; set; }
        public int res_fire { get; set; }
        public int res_earth { get; set; }
        public int res_water { get; set; }
        public int res_air { get; set; }
        public int min_gold { get; set; }
        public int max_gold { get; set; }
        public List<Drop> drops { get; set; }
    }

    public class Drop
    {
        public string code { get; set; }
        public int rate { get; set; }
        public int min_quantity { get; set; }
        public int max_quantity { get; set; }
    }

    public class AllMonstersResponse
    {
        public List<Monster> data { get; set; }
        public int total { get; set; }
        public int page { get; set; }
        public int size { get; set; }
        public int pages { get; set; }
    }
    public class MonsterResponse
    {
        public Monster data { get; set; }
    }
}
