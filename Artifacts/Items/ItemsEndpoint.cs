using Artifacts.Maps;
using Artifacts.Utilities.Network;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Artifacts.Items
{
    public class ItemsEndpoint
    {
        private static QuerySender querySender = QuerySender.Instance;

        public static AllItemsResponse GetAllItems(string craft_material, string craft_skill, int? max_level, int? min_level, string name, string type,  int page = 1, int size = 50)
        {
            string endpoint = "/items/";
            endpoint += "?page=" + page;
            endpoint += "&size=" + size;

            if (!String.IsNullOrEmpty(craft_material))
            {
                endpoint += "&craft_material=" + craft_material;
            }
            if (!String.IsNullOrEmpty(craft_skill))
            {
                endpoint += "&craft_skill=" + craft_skill;
            }
            if (!String.IsNullOrEmpty(name))
            {
                endpoint += "&name=" + name;
            }
            if (!String.IsNullOrEmpty(type))
            {
                endpoint += "&type=" + type;
            }
            if (max_level.HasValue)
            {
                endpoint += "&max_level=" + max_level;
            }
            if (min_level.HasValue)
            {
                endpoint += "&min_level=" + min_level;
            }

            HttpWebResponse response = querySender.GetQuery(endpoint);

            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                AllItemsResponse myReturnedDatas = JsonConvert.DeserializeObject<AllItemsResponse>(result);
                return myReturnedDatas;
            }
        }
    }
}
