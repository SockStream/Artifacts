using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Artifacts.Utilities.Network
{
    internal sealed class QuerySender
    {
        private static string server = "https://api.artifactsmmo.com";
        private static string token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VybmFtZSI6IlNvY2tTdHJlYW0iLCJwYXNzd29yZF9jaGFuZ2VkIjoiIn0.oXkgugB20AMqWw8c7iJovnMVwAPXKvrKTnAdspma04E";
        private static QuerySender instance = new QuerySender();

        public static QuerySender Instance
        {
            get { return instance; }
        }

        public HttpWebResponse PostQuery(string EndPoint, string JsonPayload)
        {
            string Query = server + EndPoint;

            WebHeaderCollection Headers = new WebHeaderCollection();
            Headers.Add("Content-Type", "application/json");
            Headers.Add(@"Accept", "application/json");
            Headers.Add("Authorization", "Bearer " + token);

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Query);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers = Headers;

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {

                streamWriter.Write(JsonPayload);
            }
            HttpWebResponse httpResponse = null;
            try
            {
                httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            }
            catch (WebException ex)
            {
                throw ex;
            }
            /*using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }*/
            return httpResponse;
        }

        public HttpWebResponse GetQuery(string EndPoint)
        {
            string Query = server + EndPoint;

            WebHeaderCollection Headers = new WebHeaderCollection();
            Headers.Add(@"Accept", "application/json");
            Headers.Add("Authorization", "Bearer " + token);

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Query);
            httpWebRequest.Headers = Headers;
            HttpWebResponse httpResponse = null;
            try
            {
                httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            }
            catch (WebException ex)
            {
                throw ex;
            }
            /*using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }*/
            return httpResponse;
        }
    }
}
