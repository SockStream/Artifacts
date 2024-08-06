using Artifacts.Maps;
using Artifacts.Utilities.Network;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Artifacts.Resources.ResourcesResponse;

namespace Artifacts.Resources
{
    public class ResourcesEndPoint
    {
        private static QuerySender querySender = QuerySender.Instance;

        public static AllResourcesResponse GetAllResources(string drop, int? max_level, int? min_level, string skill, int page = 1, int size = 50)
        {
            string endpoint = "/resources/";
            endpoint += "?page=" + page;
            endpoint += "&size=" + size;
            if (!String.IsNullOrEmpty(drop))
            {
                endpoint += "&drop=" + drop;
            }
            if (!String.IsNullOrEmpty(skill))
            {
                endpoint += "&skill=" + skill;
            }
            if (max_level.HasValue)
            {
                endpoint += "&max_level=" + max_level.Value;
            }
            if (min_level.HasValue)
            {
                endpoint += "&min_level=" + min_level.Value;
            }

            HttpWebResponse response = querySender.GetQuery(endpoint);

            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                AllResourcesResponse myReturnedDatas = JsonConvert.DeserializeObject<AllResourcesResponse>(result);
                return myReturnedDatas;
            }
        }
    }
}
