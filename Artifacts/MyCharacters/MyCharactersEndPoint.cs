using Artifacts.Utilities;
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
using System.Xml.Linq;

namespace Artifacts.MyCharacters
{
    internal static class MyCharactersEndPoint
    {
        private static QuerySender querySender = QuerySender.Instance;

        public static Character Move(string name, int x, int y, bool WaitForCoolDown = true)
        {
            string endpoint = "/my/" + name + "/action/move";
            Position position = new Position();
            position.x = x.ToString();
            position.y = y.ToString();
            string StrPosition = JsonConvert.SerializeObject(position);
            try
            {
                HttpWebResponse response = querySender.PostQuery(endpoint, StrPosition);
                
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    MyCharactersResponse myReturnedDatas = JsonConvert.DeserializeObject<MyCharactersResponse>(result);
                    if (WaitForCoolDown)
                    {
                        Thread.Sleep((myReturnedDatas.data.cooldown.remaining_seconds+1) * 1000);
                    }
                    return myReturnedDatas.data.character;
                }
            }
            catch (WebException ex)
            {
                ConsoleManager.Write(ex.Message,ConsoleManager.errorConsoleColor);
                return null;
            }
        }

        public static Character EquipItem(string name, string code, string slot)
        {
            string endpoint = "/my/" + name + "/action/equip";
            try
            {
                CodeSlotPayload payload = new CodeSlotPayload();
                payload.code = code;
                payload.slot = slot;
                string strPayload = JsonConvert.SerializeObject(payload);
                HttpWebResponse response = querySender.PostQuery(endpoint, strPayload);
                
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    MyCharactersResponse myReturnedDatas = JsonConvert.DeserializeObject<MyCharactersResponse>(result);
                    Thread.Sleep((myReturnedDatas.data.cooldown.remaining_seconds+1) * 1000);
                    return myReturnedDatas.data.character;
                }
            }
            catch (WebException ex)
            {
                ConsoleManager.Write(ex.Message,ConsoleManager.errorConsoleColor);
                return null;
            }
        }

        public static Character UnequipItem(string name, string slot)
        {
            string endpoint = "/my/" + name + "/action/unequip";
            try
            {
                SlotPayload payload = new SlotPayload();
                payload.slot = slot;
                string strPayload = JsonConvert.SerializeObject(payload);
                HttpWebResponse response = querySender.PostQuery(endpoint, strPayload);
                
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    MyCharactersResponse myReturnedDatas = JsonConvert.DeserializeObject<MyCharactersResponse>(result);
                    Thread.Sleep((myReturnedDatas.data.cooldown.remaining_seconds+1) * 1000);
                    return myReturnedDatas.data.character;
                }
            }
            catch (WebException ex)
            {
                ConsoleManager.Write(ex.Message,ConsoleManager.errorConsoleColor);
                return null;
            }
        }

        public static Character Fight(string name)
        {
            string endpoint = "/my/" + name + "/action/fight";
            try
            {
                HttpWebResponse response = querySender.PostQuery(endpoint, "");
                
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    MyCharactersResponse myReturnedDatas = JsonConvert.DeserializeObject<MyCharactersResponse>(result);
                    Thread.Sleep((myReturnedDatas.data.cooldown.remaining_seconds+1) * 1000);
                    return myReturnedDatas.data.character;
                }
            }
            catch (WebException ex)
            {
                ConsoleManager.Write(ex.Message,ConsoleManager.errorConsoleColor);
                return null;
            }
        }

        public static Character Gathering(string name)
        {
            string endpoint = "/my/" + name + "/action/gathering";
            try
            {
                HttpWebResponse response = querySender.PostQuery(endpoint, "");
                
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    MyCharactersResponse myReturnedDatas = JsonConvert.DeserializeObject<MyCharactersResponse>(result);
                    Thread.Sleep((myReturnedDatas.data.cooldown.remaining_seconds+1) * 1000);
                    return myReturnedDatas.data.character;
                }
            }
            catch (WebException ex)
            {
                ConsoleManager.Write(ex.Message,ConsoleManager.errorConsoleColor);
                return null;
            }
        }

        public static Character Crafting(string name, string code, int quantity)
        {
            string endpoint = "/my/" + name + "/action/crafting";
            try
            {
                CodeQuantityPayload payload = new CodeQuantityPayload();
                payload.code = code;
                payload.quantity = quantity;
                string strPayload = JsonConvert.SerializeObject(payload);
                HttpWebResponse response = querySender.PostQuery(endpoint, strPayload);
                
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    MyCharactersResponse myReturnedDatas = JsonConvert.DeserializeObject<MyCharactersResponse>(result);
                    Thread.Sleep((myReturnedDatas.data.cooldown.remaining_seconds+1) * 1000);
                    return myReturnedDatas.data.character;
                }
            }
            catch (WebException ex)
            {
                ConsoleManager.Write(ex.Message,ConsoleManager.errorConsoleColor);
                return null;
            }
        }

        public static Character DepositBank(string name, string code, int quantity)
        {
            string endpoint = "/my/" + name + "/action/bank/deposit";
            try
            {
                CodeQuantityPayload payload = new CodeQuantityPayload();
                payload.code = code;
                payload.quantity = quantity;
                string strPayload = JsonConvert.SerializeObject(payload);
                HttpWebResponse response = querySender.PostQuery(endpoint, strPayload);
                
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    MyCharactersResponse myReturnedDatas = JsonConvert.DeserializeObject<MyCharactersResponse>(result);
                    Thread.Sleep((myReturnedDatas.data.cooldown.remaining_seconds+1) * 1000);
                    return myReturnedDatas.data.character;
                }
            }
            catch (WebException ex)
            {
                ConsoleManager.Write(ex.Message,ConsoleManager.errorConsoleColor);
                return null;
            }
        }

        public static Character DepositBankGold(string name, int quantity)
        {
            string endpoint = "/my/" + name + "/action/bank/deposit/gold";
            try
            {
                QuantityPayload payload = new QuantityPayload(); ;
                payload.quantity = quantity;
                string strPayload = JsonConvert.SerializeObject(payload);
                HttpWebResponse response = querySender.PostQuery(endpoint, strPayload);
                
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    MyCharactersResponse myReturnedDatas = JsonConvert.DeserializeObject<MyCharactersResponse>(result);
                    Thread.Sleep((myReturnedDatas.data.cooldown.remaining_seconds+1) * 1000);
                    return myReturnedDatas.data.character;
                }
            }
            catch (WebException ex)
            {
                ConsoleManager.Write(ex.Message,ConsoleManager.errorConsoleColor);
                return null;
            }
        }

        public static Character Recycling(string name, string code, int quantity)
        {
            string endpoint = "/my/" + name + "/action/recycling";
            try
            {
                CodeQuantityPayload payload = new CodeQuantityPayload();
                payload.code = code;
                payload.quantity = quantity;
                string strPayload = JsonConvert.SerializeObject(payload);
                HttpWebResponse response = querySender.PostQuery(endpoint, strPayload);
                
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    MyCharactersResponse myReturnedDatas = JsonConvert.DeserializeObject<MyCharactersResponse>(result);
                    Thread.Sleep((myReturnedDatas.data.cooldown.remaining_seconds+1) * 1000);
                    return myReturnedDatas.data.character;
                }
            }
            catch (WebException ex)
            {
                ConsoleManager.Write(ex.Message,ConsoleManager.errorConsoleColor);
                return null;
            }
        }

        public static Character WithdrawBank(string name, string code, int quantity)
        {
            string endpoint = "/my/" + name + "/action/bank/withdraw";
            try
            {
                CodeQuantityPayload payload = new CodeQuantityPayload();
                payload.code = code;
                payload.quantity = quantity;
                string strPayload = JsonConvert.SerializeObject(payload);
                HttpWebResponse response = querySender.PostQuery(endpoint, strPayload);
                
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    MyCharactersResponse myReturnedDatas = JsonConvert.DeserializeObject<MyCharactersResponse>(result);
                    Thread.Sleep((myReturnedDatas.data.cooldown.remaining_seconds+1) * 1000);
                    return myReturnedDatas.data.character;
                }
            }
            catch (WebException ex)
            {
                ConsoleManager.Write(ex.Message,ConsoleManager.errorConsoleColor);
                return null;
            }
        }

        public static Character WithdrawBankGold(string name, int quantity)
        {
            string endpoint = "/my/" + name + "/action/bank/withdraw/gold";
            try
            {
                QuantityPayload payload = new QuantityPayload(); ;
                payload.quantity = quantity;
                string strPayload = JsonConvert.SerializeObject(payload);
                HttpWebResponse response = querySender.PostQuery(endpoint, strPayload);
                
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    MyCharactersResponse myReturnedDatas = JsonConvert.DeserializeObject<MyCharactersResponse>(result);
                    Thread.Sleep((myReturnedDatas.data.cooldown.remaining_seconds+1) * 1000);
                    return myReturnedDatas.data.character;
                }
            }
            catch (WebException ex)
            {
                ConsoleManager.Write(ex.Message,ConsoleManager.errorConsoleColor);
                return null;
            }
        }

        public static Character GeBuyItem(string name, string code, int quantity, int price)
        {
            string endpoint = "/my/" + name + "/action/ge/buy";
            try
            {
                CodeQuantityPricePayload payload = new CodeQuantityPricePayload();
                payload.code = code;
                payload.quantity = quantity;
                payload.price = price;
                string strPayload = JsonConvert.SerializeObject(payload);
                HttpWebResponse response = querySender.PostQuery(endpoint, strPayload);
                
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    MyCharactersResponse myReturnedDatas = JsonConvert.DeserializeObject<MyCharactersResponse>(result);
                    Thread.Sleep((myReturnedDatas.data.cooldown.remaining_seconds+1) * 1000);
                    return myReturnedDatas.data.character;
                }
            }
            catch (WebException ex)
            {
                ConsoleManager.Write(ex.Message,ConsoleManager.errorConsoleColor);
                return null;
            }
        }

        public static Character GeSellItem(string name, string code, int quantity, int price)
        {
            string endpoint = "/my/" + name + "/action/ge/sell";
            try
            {
                CodeQuantityPricePayload payload = new CodeQuantityPricePayload();
                payload.code = code;
                payload.quantity = quantity;
                payload.price = price;
                string strPayload = JsonConvert.SerializeObject(payload);
                HttpWebResponse response = querySender.PostQuery(endpoint, strPayload);
                
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    MyCharactersResponse myReturnedDatas = JsonConvert.DeserializeObject<MyCharactersResponse>(result);
                    Thread.Sleep((myReturnedDatas.data.cooldown.remaining_seconds+1) * 1000);
                    return myReturnedDatas.data.character;
                }
            }
            catch (WebException ex)
            {
                ConsoleManager.Write(ex.Message,ConsoleManager.errorConsoleColor);
                return null;
            }
        }

        public static Character AcceptNewTask(string name)
        {
            string endpoint = "/my/" + name + "/action/task/new";
            try
            {
                NamePayload payload = new NamePayload();
                payload.name = name;
                string strPayload = JsonConvert.SerializeObject(payload);
                HttpWebResponse response = querySender.PostQuery(endpoint, strPayload);
                
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    MyCharactersResponse myReturnedDatas = JsonConvert.DeserializeObject<MyCharactersResponse>(result);
                    Thread.Sleep((myReturnedDatas.data.cooldown.remaining_seconds+1) * 1000);
                    return myReturnedDatas.data.character;
                }
            }
            catch (WebException ex)
            {
                ConsoleManager.Write(ex.Message,ConsoleManager.errorConsoleColor);
                return null;
            }
        }

        public static Character CompleteTask(string name)
        {
            string endpoint = "/my/" + name + "/action/task/complete";
            try
            {
                NamePayload payload = new NamePayload();
                payload.name = name;
                string strPayload = JsonConvert.SerializeObject(payload);
                HttpWebResponse response = querySender.PostQuery(endpoint, strPayload);
                
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    MyCharactersResponse myReturnedDatas = JsonConvert.DeserializeObject<MyCharactersResponse>(result);
                    Thread.Sleep((myReturnedDatas.data.cooldown.remaining_seconds+1) * 1000);
                    return myReturnedDatas.data.character;
                }
            }
            catch (WebException ex)
            {
                ConsoleManager.Write(ex.Message,ConsoleManager.errorConsoleColor);
                return null;
            }
        }

        public static Character TaskExchange(string name)
        {
            string endpoint = "/my/" + name + "/action/task/exchange";
            try
            {
                NamePayload payload = new NamePayload();
                payload.name = name;
                string strPayload = JsonConvert.SerializeObject(payload);
                HttpWebResponse response = querySender.PostQuery(endpoint, strPayload);
                
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    MyCharactersResponse myReturnedDatas = JsonConvert.DeserializeObject<MyCharactersResponse>(result);
                    Thread.Sleep((myReturnedDatas.data.cooldown.remaining_seconds+1) * 1000);
                    return myReturnedDatas.data.character;
                }
            }
            catch (WebException ex)
            {
                ConsoleManager.Write(ex.Message,ConsoleManager.errorConsoleColor);
                return null;
            }
        }

        public static Character DeleteItem(string name, string code, int quantity)
        {
            string endpoint = "/my/" + name + "/action/delete";
            try
            {
                CodeQuantityPayload payload = new CodeQuantityPayload();
                payload.code = code;
                payload.quantity = quantity;
                string strPayload = JsonConvert.SerializeObject(payload);
                HttpWebResponse response = querySender.PostQuery(endpoint, strPayload);
                
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    MyCharactersResponse myReturnedDatas = JsonConvert.DeserializeObject<MyCharactersResponse>(result);
                    Thread.Sleep((myReturnedDatas.data.cooldown.remaining_seconds+1) * 1000);
                    return myReturnedDatas.data.character;
                }
            }
            catch (WebException ex)
            {
                ConsoleManager.Write(ex.Message,ConsoleManager.errorConsoleColor);
                return null;
            }
        }

        public static Character[] GetMyCharacters ()
        {
            string endpoint = "/my/characters";
            HttpWebResponse response = querySender.GetQuery(endpoint);
            
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                AllMyCharactersResponse myReturnedDatas = JsonConvert.DeserializeObject<AllMyCharactersResponse>(result);
                return myReturnedDatas.data.ToArray();
            }
        }

    }
}
