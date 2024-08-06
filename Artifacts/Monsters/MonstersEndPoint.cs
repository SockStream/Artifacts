using Artifacts.Utilities.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Artifacts.Monsters
{
    public class MonstersEndPoint
    {
        private static QuerySender querySender = QuerySender.Instance;

        public static AllMonstersResponse GetAllMonsters(string? drop, int? max_level, int? min_level, int page = 1, int size = 50)
        {
            string endpoint = "/monsters/";
            endpoint += "?page=" + page;
            endpoint += "&size=" + size;

            if (! String.IsNullOrEmpty(drop))
            {
                endpoint += "&drop=" + drop;
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
                AllMonstersResponse myReturnedDatas = JsonConvert.DeserializeObject<AllMonstersResponse>(result);
                return myReturnedDatas;
            }
        }

        public static Monster GetMonster(string code)
        {
            string endpoint = "/monsters/_CODE_";
            endpoint = endpoint.Replace("_CODE_", code);

            HttpWebResponse response = querySender.GetQuery(endpoint);
            
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                MonsterResponse myReturnedDatas = JsonConvert.DeserializeObject<MonsterResponse>(result);
                return myReturnedDatas.data;
            }
        }


    }
}
