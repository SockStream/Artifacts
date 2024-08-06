using Artifacts.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifacts.Resources
{
    public class ResourcesResponse
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Resource
        {
            public string name { get; set; }
            public string code { get; set; }
            public string skill { get; set; }
            public int level { get; set; }
            public List<Drop> drops { get; set; }
        }

        public class AllResourcesResponse
        {
            public List<Resource> data { get; set; }
            public int total { get; set; }
            public int page { get; set; }
            public int size { get; set; }
            public int pages { get; set; }
        }


    }
}
