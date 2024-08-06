using Artifacts.Characters;
using Artifacts.Utilities.ConsoleManager;
using Artifacts.Utilities.Network;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Artifacts.Status
{
    internal static class StatusEndPoint
    {
        private static QuerySender querySender = QuerySender.Instance;
        public static StatusResponse GetStatus()
        {
            string endpoint = String.Empty;
            try
            {
                HttpWebResponse response = querySender.GetQuery(endpoint);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    ConsoleManager.Write(response.StatusCode.ToString(),ConsoleManager.errorConsoleColor);
                }
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    StatusResponse myReturnedDatas = JsonConvert.DeserializeObject<StatusResponse>(result);
                    return myReturnedDatas;
                }
            }
            catch (WebException ex)
            {
                ConsoleManager.Write(ex.Message, ConsoleManager.errorConsoleColor);
                ConsoleManager.Write(ex.StackTrace, ConsoleManager.errorConsoleColor);
                return null;
            }
        }
    }
}
