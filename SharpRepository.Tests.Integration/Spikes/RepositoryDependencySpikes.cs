﻿
using System.Reflection;
using NUnit.Framework;
using SharpRepository.EfCoreRepository;
using SharpRepository.Ioc.StructureMap;
using SharpRepository.Repository;
using SharpRepository.Repository.Ioc;
using SharpRepository.Tests.Integration.TestObjects;
using Shouldly;
using StructureMap;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace SharpRepository.Tests.Integration.Spikes
{
    [TestFixture]
    public class RepositoryDependencySpikes
    {
        protected Container container;

        [SetUp]
        public void Setup()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<TestObjectContextCore>()
                 .UseSqlite(connection)
                 .Options;

            // structure map
            container = new Container(x =>
            {
                x.AddRegistry(new StructureMapRegistry(options));
                x.ForRepositoriesUseSharpRepository();
            });

            RepositoryDependencyResolver.SetDependencyResolver(new StructureMapRepositoryDependencyResolver(container));
        }

        [Test]
        public void EfConfigRepositoryFactory_Using_Ioc_Should_Not_Require_ConnectionString()
        {
            var config = new EfCoreRepositoryConfiguration("TestConfig", null, typeof (TestObjectContextCore));
            var factory = new EfCoreConfigRepositoryFactory(config);

            factory.GetInstance<Contact, string>();
        }

        [Test]
        public void EfConfigRepositoryFactory_Using_Ioc_Should_Return_TestObjectEntites_Without_DbContextType_Defined()
        {
            var config = new EfCoreRepositoryConfiguration("TestConfig");
            var factory = new EfCoreConfigRepositoryFactory(config);

            var repos = factory.GetInstance<Contact, string>();

            var propInfo = repos.GetType().GetProperty("Context", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var dbContext = (TestObjectContextCore)propInfo.GetValue(repos, null);
            dbContext.ShouldBeOfType<TestObjectContextCore>();
        }

        [Test]
        public void EfConfigRepositoryFactory_Using_Ioc_Should_Share_DbContext()
        {
            var config = new EfCoreRepositoryConfiguration("TestConfig", "tmp", typeof (TestObjectContextCore));
            var factory = new EfCoreConfigRepositoryFactory(config);

            var repos1 = factory.GetInstance<Contact, string>();
            var repos2 = factory.GetInstance<Contact, string>();

            // use reflecton to get the protected Context property
            var propInfo1 = repos1.GetType().GetProperty("Context", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var dbContext1 = (TestObjectContextCore)propInfo1.GetValue(repos1, null);
            var propInfo2 = repos2.GetType().GetProperty("Context", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var dbContext2 = (TestObjectContextCore)propInfo2.GetValue(repos2, null);

            dbContext1.ShouldBe(dbContext2);
        }

        [Test]
        public void Ioc_For_IRepository_T_Should_Be_EfRepository_T()
        {
            var repos = container.GetInstance<IRepository<ContactType>>();
            repos.ShouldBeOfType<EfCoreRepository<ContactType>>();
        }

        [Test]
        public void Ioc_For_IRepository_T_TKey_Should_Be_EfRepository_T_TKey()
        {
            var repos = container.GetInstance<IRepository<ContactType, int>>();
            repos.ShouldBeOfType<EfCoreRepository<ContactType, int>>();
        }

        [Test]
        public void Ioc_For_ICompoundKeyRepository_T_TKey_TKey2_Should_Be_EfRepository_T_TKey_TKey2()
        {
            var repos = container.GetInstance<ICompoundKeyRepository<ContactType, int, string>>();
            repos.ShouldBeOfType<EfCoreRepository<ContactType, int, string>>();
        }

        [Test]
        public void Ioc_For_ICompoundRepository_T_TKey_TKey2_TKey3_Should_Be_EfRepository_T_TKey_TKey2_TKey3()
        {
            var repos = container.GetInstance<ICompoundKeyRepository<ContactType, int, string, string>>();
            repos.ShouldBeOfType<EfCoreRepository<ContactType, int,string,string>>();
        }

        [Test]
        public void Ioc_For_ICompoundRepository_T_Should_Be_EfCompoundRepository_T()
        {
            var repos = container.GetInstance<ICompoundKeyRepository<ContactType>>();
            repos.ShouldBeOfType<EfCoreCompoundKeyRepository<ContactType>>();
        }
    }

    public class StructureMapRegistry : Registry
    {
        public StructureMapRegistry(DbContextOptions<TestObjectContextCore> options)
        {
            Scan(scanner =>
            {
                scanner.TheCallingAssembly();
                scanner.WithDefaultConventions();
            });

            For<DbContext>()
                .Use<TestObjectContextCore>()
                .Ctor<DbContextOptions<TestObjectContextCore>>("options").Is(options);

            For<TestObjectContextCore>()
                .Use<TestObjectContextCore>()
                .Ctor<DbContextOptions<TestObjectContextCore>>("options").Is(options);
        }
    }
}
