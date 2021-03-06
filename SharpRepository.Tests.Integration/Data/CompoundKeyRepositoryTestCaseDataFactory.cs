using NUnit.Framework;
using SharpRepository.CacheRepository;
using SharpRepository.EfRepository;
using SharpRepository.InMemoryRepository;
using SharpRepository.Tests.Integration.TestObjects;
using System.Collections.Generic;
using System.Linq;


namespace SharpRepository.Tests.Integration.Data
{
    public class CompoundKeyRepositoryTestCaseDataFactory
    {
        public static IEnumerable<TestCaseData> Build(RepositoryType[] includeType)
        {
            if (includeType.Contains(RepositoryType.InMemory))
            {
                yield return new TestCaseData(new InMemoryRepository<User, string, int>()).SetName("InMemoryRepository Test");
            }

            if (includeType.Contains(RepositoryType.Ef))
            {
                var dbPath = EfDataDirectoryFactory.Build();
                yield return new TestCaseData(new EfRepository<User, string, int>(new TestObjectEntities("Data Source=" + dbPath))).SetName("EfRepository Test");
            }

            if (includeType.Contains(RepositoryType.Cache))
            {
                yield return new TestCaseData(new CacheRepository<User, string, int>(CachePrefixFactory.Build())).SetName("CacheRepository Test");
            }
        }
    }
}