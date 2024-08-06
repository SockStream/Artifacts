using Artifacts.Utilities.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Artifacts.Maps
{
    internal class MapsEndPoint
    {
        private static QuerySender querySender = QuerySender.Instance;

        public static AllMapResponse GetAllMaps(string content_code, string content_type, int page = 1, int size = 50)
        {
            string endpoint = "/maps/";
            endpoint += "?page=" + page;
            endpoint += "&size=" + size;
            if (content_code != string.Empty)
            {
                endpoint += "&content_code=" + content_code;
            }
            if (content_type != string.Empty)
            {
                endpoint += "&content_type=" + content_type;
            }

            HttpWebResponse response = querySender.GetQuery(endpoint);
            
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                AllMapResponse myReturnedDatas = JsonConvert.DeserializeObject<AllMapResponse>(result);
                return myReturnedDatas;
            }
        }

        public static Tile GetMap(int x, int y)
        {
            string endpoint = "/maps/_X_/_Y_";
            endpoint = endpoint.Replace("_X_",x.ToString()).Replace("_Y_",y.ToString());

            HttpWebResponse response = querySender.GetQuery(endpoint);
            
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                MapResponse myReturnedDatas = JsonConvert.DeserializeObject<MapResponse>(result);
                return myReturnedDatas.data;
            }
        }


    }
}
