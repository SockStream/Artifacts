using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifacts.Items
{
    public class Craft
    {
        public string skill { get; set; }
        public int level { get; set; }
        public List<Item> items { get; set; }
        public int quantity { get; set; }
    }

    public class Objet
    {
        public string name { get; set; }
        public string code { get; set; }
        public int level { get; set; }
        public string type { get; set; }
        public string subtype { get; set; }
        public string description { get; set; }
        public List<Effect> effects { get; set; }
        public Craft craft { get; set; }
    }

    public class Effect
    {
        public string name { get; set; }
        public int value { get; set; }
    }

    public class Item
    {
        public string name { get; set; }
        public string quantity { get; set; }
        public string code { get; set; }
        public int level { get; set; }
        public string type { get; set; }
        public string subtype { get; set; }
        public string description { get; set; }
        public List<Effect> effects { get; set; }
        public Craft craft { get; set; }
    }

    public class AllItemsResponse
    {
        public List<Objet> data { get; set; }
        public int total { get; set; }
        public int page { get; set; }
        public int size { get; set; }
        public int pages { get; set; }
    }


}
