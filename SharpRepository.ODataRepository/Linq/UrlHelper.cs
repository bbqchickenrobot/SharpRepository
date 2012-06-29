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

        public static string Get(string reuqestUrl)
        {
            //Console.WriteLine("requesting " + reuqestUrl);
            var request = WebRequest.Create(reuqestUrl);
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