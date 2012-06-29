using System.Linq;
using NUnit.Framework;
using SharpRepository.ODataRepository;
using SharpRepository.Repository.Queries;
using Should;

namespace SharpRepository.Tests.Integration.Spikes
{
    [TestFixture]
    public class ODataTests
    {
        public class Title
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int ReleaseYear { get; set; }
        }

        public class Customer
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }

        [Test]
        public void FindAll_OData_Test_Service()
        {
            

            var repository = new ODataRepository<Customer, int>("http://services.odata.org/website/odata.svc", "ODataConsumers");
            var list = repository.FindAll(x => x.Name.StartsWith("T")).ToList();

            list.Count.ShouldEqual(4);
        }

        [Test]
        public void FindAll_Netflix_Titles_From_2007()
        {
            var repository = new ODataRepository<Title, string>("http://odata.netflix.com/v2/Catalog/");
            var list = repository.FindAll(x => x.ReleaseYear == 2007).ToList();

            list.Count.ShouldEqual(500);
        }

        [Test]
        public void FindAll_Netflix_Titles_From_2007_Pagination()
        {
            // TODO: expand ODataProvider logic to handle OrderBy, OrderByDescending, First and FirstOrDefault

            var repository = new ODataRepository<Title, string>("http://odata.netflix.com/v2/Catalog/");
            var list = repository.FindAll(x => x.ReleaseYear == 2007, new PagingOptions<Title, string>(1, 20, x => x.Name, true )).ToList();

            list.Count.ShouldEqual(20);
        }
    }
}
