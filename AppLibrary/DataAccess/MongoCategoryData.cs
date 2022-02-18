using AppLibrary.DataAccess.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace AppLibrary.DataAccess
{
    public class MongoCategoryData : ICategoryData
    {
        private readonly IMongoCollection<CategoryModel> _categories;
        private readonly IMemoryCache _cache;
        private const string _cacheName = "CategoryData";

        public MongoCategoryData(IDbConnection db, IMemoryCache cache)
        {
            _categories = db.CategoryCollection;
            _cache = cache;
        }

        public async Task<List<CategoryModel>> GetAllCategories()
        {
            var output = _cache.Get<List<CategoryModel>>(_cacheName);

            if (output is null)
            {
                var result = await _categories.FindAsync(_ => true);
                output = result.ToList();
                _cache.Set(_cacheName, output, TimeSpan.FromHours(1));
            }

            return output;
        }

        public async Task CreateCategory(CategoryModel category)
        {
            await _categories.InsertOneAsync(category);
        }
    }
}
