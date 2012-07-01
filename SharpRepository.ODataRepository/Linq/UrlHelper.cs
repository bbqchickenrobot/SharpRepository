using System.IO;
using System.Net;

namespace SharpRepository.ODataRepository.Linq
{
    internal static class UrlHelper
    {
        public static string AddParameter(this string url, string param)
        {
            if (url.Contains("?"))
                return url + "&" + param;
            
            return url + "?" + param;
        }

        public static string Get(string requestUrl)
        {
            //Console.WriteLine("requesting " + reuqestUrl);
            var request = WebRequest.Create(requestUrl);
            request.ContentType = "application/json";
            
            string responseFromServer;
            using (var response = request.GetResponse())
            {
                using (var dataStream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(dataStream))
                    {
                        responseFromServer = reader.ReadToEnd();
                    }
                }
            }

            //Console.WriteLine("responding " + responseFromServer);

            return responseFromServer;
        }
    }
}