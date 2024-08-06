using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifacts.Maps
{
    public class Content
    {
        public string type { get; set; }
        public string code { get; set; }
    }

    public class Tile
    {
        public string name { get; set; }
        public string skin { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public Content content { get; set; }
    }

    public class AllMapResponse
    {
        public List<Tile> data { get; set; }
        public int total { get; set; }
        public int page { get; set; }
        public int size { get; set; }
        public int pages { get; set; }
    }
    public class MapResponse
    {
        public Tile data { get; set; }
    }
}
