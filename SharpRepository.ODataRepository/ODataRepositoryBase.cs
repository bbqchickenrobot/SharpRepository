using System;
using System.Linq;
using SharpRepository.ODataRepository.Linq;
using SharpRepository.Repository;
using SharpRepository.Repository.Caching;
using SharpRepository.Repository.FetchStrategies;
using SharpRepository.Repository.Traits;

namespace SharpRepository.ODataRepository
{
    public class ODataRepositoryBase<T, TKey> : LinqRepositoryBase<T, TKey>, ICanGet<T,TKey>, ICanFind<T> where T : class, new()
    {
        private ODataProvider _provider;
        private string _typeName;
        private IQueryable<T> _baseQuery;

        internal ODataRepositoryBase(string url, string collectionName = null, ICachingStrategy<T, TKey> cachingStrategy = null)
            : base(cachingStrategy)
        {
            Initialize(url, collectionName);
        }

        private void Initialize(string url, string collectionName)
        {
            _typeName = typeof (T).Name;

            if (String.IsNullOrEmpty(collectionName))
            {
                // generate based on the type name
                collectionName = _typeName + "s"; // TODO: do we need to take into account Person to People, or things like that
            }

            _provider = new ODataProvider(url);
            _baseQuery = _provider.CreateQuery<T>(collectionName);
        }

        protected override IQueryable<T> BaseQuery(IFetchStrategy<T> fetchStrategy)
        {
            return _baseQuery;
        }

        protected override void AddItem(T entity)
        {
            throw new NotImplementedException();
        }

        protected override void DeleteItem(T entity)
        {
            throw new NotImplementedException();
        }

        protected override void UpdateItem(T entity)
        {
            throw new NotImplementedException();
        }

        protected override void SaveChanges()
        {
            throw new NotImplementedException();
        }

        // we override the implementation fro LinqBaseRepository becausee this is built in and doesn't need to find the key column and do dynamic expressions, etc.
        protected override T GetQuery(TKey key)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _baseQuery = null;
            _provider = null;
        }
    }
}
