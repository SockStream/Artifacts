using Artifacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Artifacts.Utilities.Payloads;
using Artifacts.Utilities.Network;
using System.Xml.Linq;
using Artifacts.MyCharacters;
using Artifacts.Utilities.ConsoleManager;

namespace Artifacts.Characters
{
    internal static class CharactersEndPoint
    {
        private static QuerySender querySender = QuerySender.Instance;
        public static Character Create(string name, string skin)
        {
            string endpoint = "/characters/create";
            NameSkinPayload payload = new NameSkinPayload();
            payload.name = name;
            payload.skin = skin;
            string strPayload = JsonConvert.SerializeObject(payload);
            try
            {
                HttpWebResponse response = querySender.PostQuery(endpoint, strPayload);
                
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    DataCharacter myReturnedDatas = JsonConvert.DeserializeObject<DataCharacter>(result);
                    return myReturnedDatas.data;
                }
            }
            catch (WebException ex)
            {
                ConsoleManager.Write(ex.Message,ConsoleManager.errorConsoleColor);
                return null;
            }
        }

        public static Character DeleteCharacter(string name)
        {
            string endpoint = "/characters/delete";
            NamePayload payload = new NamePayload();
            payload.name = name;
            string strPayload = JsonConvert.SerializeObject(payload);
            try
            {
                HttpWebResponse response = querySender.PostQuery(endpoint, strPayload);

                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    DataCharacter myReturnedDatas = JsonConvert.DeserializeObject<DataCharacter>(result);
                    return myReturnedDatas.data;
                }
            }
            catch (WebException ex)
            {
                ConsoleManager.Write(ex.Message, ConsoleManager.errorConsoleColor);
                return null;
            }
        }

        public static GetAllCharactersResponse GetAllCharacters(int page, int size, string sort)
        {
            string endpoint = "/characters/?page=_PAGE_&size=_SIZE_";
            if (! String.IsNullOrEmpty(sort))
            {
                endpoint += "&sort=_SORT_";
            }

            endpoint = endpoint.Replace("_PAGE_", page.ToString()).Replace("_SIZE_", size.ToString()).Replace("_SORT_", sort);

            try
            {
                HttpWebResponse response = querySender.GetQuery(endpoint);
                
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    result = result.Replace("data", "character");
                    GetAllCharactersResponse myReturnedDatas = JsonConvert.DeserializeObject<GetAllCharactersResponse>(result);
                    return myReturnedDatas;
                }
            }
            catch (WebException ex)
            {
                ConsoleManager.Write(ex.Message,ConsoleManager.errorConsoleColor);
                return null;
            }
        }

        public static Character GetCharacter(string name)
        {
            string endpoint = "/characters/" + name;
            try
            {
                HttpWebResponse response = querySender.GetQuery(endpoint);
                
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    DataCharacter myReturnedDatas = JsonConvert.DeserializeObject<DataCharacter>(result);
                    return myReturnedDatas.data;
                }
            }
            catch (WebException ex)
            {
                ConsoleManager.Write(ex.Message,ConsoleManager.errorConsoleColor);
                return null;
            }
        }
    }
}
