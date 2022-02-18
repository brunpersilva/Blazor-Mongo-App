using Microsoft.Extensions.Caching.Memory;

namespace AppLibrary.DataAccess
{
    public class MongoStatusData : IStatusData
    {
        private readonly IMongoCollection<StatusModel> _statuses;
        private readonly IMemoryCache _cache;
        private const string _cacheName = "StatusData";

        public MongoStatusData(IDbConnection db, IMemoryCache cache)
        {
            _cache = cache;
            _statuses = db.StatusCollection;
        }

        public async Task<List<StatusModel>> GetAllStatuses()
        {
            var output = _cache.Get<List<StatusModel>>(_cacheName);

            if (output is null)
            {
                var result = await _statuses.FindAsync(_ => true);
                output = result.ToList();
                _cache.Set(_cacheName, output, TimeSpan.FromHours(1));
            }

            return output;
        }

        public async Task CreateCategory(StatusModel status)
        {
            await _statuses.InsertOneAsync(status);
        }
    }
}
