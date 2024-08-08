using Artifacts.Characters;
using Artifacts.MyCharacters;
using Artifacts.Utilities.ConsoleManager;
using Artifacts.Utilities.Network;
using Artifacts.Utilities.Payloads;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Artifacts.GrandExchange
{
    internal class GrandExchangeEndPoint
    {
        private static QuerySender querySender = QuerySender.Instance;
        public static GrandExchange GetItem(string code)
        {
            string endpoint = "/ge/";
            endpoint += code;
            try
            {
                HttpWebResponse response = querySender.GetQuery(endpoint);

                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    GrandExchangeResponse myReturnedDatas = JsonConvert.DeserializeObject<GrandExchangeResponse>(result);
                    return myReturnedDatas.data;
                }
            }
            catch (WebException ex)
            {
                ConsoleManager.Write(ex.Message, ConsoleManager.errorConsoleColor);
                return null;
            }
        }
    }
}
