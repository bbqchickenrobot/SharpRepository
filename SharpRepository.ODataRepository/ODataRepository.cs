using System;
using SharpRepository.Repository.Caching;

namespace SharpRepository.ODataRepository
{
    /// <summary>
    /// OData repository layer
    /// </summary>
    /// <typeparam name="T">The Entity type</typeparam>
    /// <typeparam name="TKey">The type of the primary key.</typeparam>
    public class ODataRepository<T, TKey> : ODataRepositoryBase<T, TKey> where T : class, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ODataRepository&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="url">The OData base URL</param>
        /// <param name="collectionName">Override the collection name if it doesn't match the plural of the entity type</param>
        /// <param name="cachingStrategy">The caching strategy to use.  Defaults to <see cref="NoCachingStrategy&lt;T, TKey&gt;" /></param>
        public ODataRepository(string url, string collectionName = null, ICachingStrategy<T, TKey> cachingStrategy = null)
            : base(url, collectionName, cachingStrategy)
        {
            if (String.IsNullOrEmpty(url)) throw new ArgumentNullException("url");
        }
    }

    /// <summary>
    /// OData repository layer
    /// </summary>
    /// <typeparam name="T">The Entity type</typeparam>
    public class ODataRepository<T> : ODataRepositoryBase<T, int> where T : class, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ODataRepository&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="url">The OData base URL</param>
        /// <param name="collectionName">Override the collection name if it doesn't match the plural of the entity type</param>
        /// <param name="cachingStrategy">The caching strategy to use.  Defaults to <see cref="NoCachingStrategy&lt;T, TKey&gt;" /></param>
        public ODataRepository(string url, string collectionName = null, ICachingStrategy<T, int> cachingStrategy = null)
            : base(url, collectionName, cachingStrategy)
        {
            if (String.IsNullOrEmpty(url)) throw new ArgumentNullException("url");
        }
    }
}
