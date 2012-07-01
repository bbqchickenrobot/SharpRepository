using System;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SharpRepository.ODataRepository.Linq
{
    internal class ODataProvider : QueryProvider
    {
        private readonly string _url;

        public ODataProvider(string url)
        {
            _url = url;
        }

        public IQueryable<T> CreateQuery<T>(string collection)
        {
            return new Query<T>(this, collection);
        }

        public override TResult Execute<TResult>(Expression expression)
        {
            var collectionName = expression.GetCollectionName();

            var requestUrl = _url + '/' + collectionName;

            requestUrl += Translate(expression);

            var response = UrlHelper.Get(requestUrl);

            return DeserializeObject<TResult>(response);
        }

        private T DeserializeObject<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (JsonSerializationException ex)
            {

            }

            var jobject = JsonConvert.DeserializeObject(json) as JObject;
            var objectString = String.Empty;

            try
            {
                var token = jobject["d"];

                try
                {
                    var results = token["results"]; // netflix OData service (http://odata.netflix.com/v2/Catalog/) adds a results array inside the d array while other services don't

                    if (results != null)
                    {
                        objectString = results.ToString();
                    }
                }
                catch
                {
                    
                }

                if (String.IsNullOrEmpty(objectString))
                {
                    objectString = token.ToString();
                }
                
            }
            catch
            {
                
            }


            var items = JsonConvert.DeserializeObject<T>(objectString);

            return items;
        }

        private string Translate(Expression expression)
        {
            var visitor = new ODataExpressionVisitor();

            return visitor.Parse(expression);
        }
    }
}