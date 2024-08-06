using Artifacts.Utilities.Network;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Artifacts.MyAccount
{
    public static class MyAccountEndpoint
    {
        private static QuerySender querySender = QuerySender.Instance;
        public static BankItemResponse GetBankItems(string item_code, int page=1, int size=100)
        {
            string endpoint = "/my/bank/items";
            endpoint += "?page=" + page;
            endpoint += "&size=" + size;
            if (!String.IsNullOrEmpty(item_code))
            {
                endpoint += "&item_code=" + item_code;
            }
            try
            {
                HttpWebResponse response = querySender.GetQuery(endpoint);

                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    BankItemResponse myReturnedDatas = JsonConvert.DeserializeObject<BankItemResponse>(result);
                    return myReturnedDatas;
                }
            }
            catch (WebException ex )
            {
                if (ex.Response != null)
                {
                    var response = (HttpWebResponse)ex.Response;
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        BankItemResponse bankItemResponse = new BankItemResponse();
                        bankItemResponse.data = new List<Items.Item>();
                        return bankItemResponse;
                    }
                }
                throw ex;
            }
            return null;
        }
    }
}
