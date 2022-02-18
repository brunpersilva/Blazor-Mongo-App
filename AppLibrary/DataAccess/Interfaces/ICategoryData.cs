namespace AppLibrary.DataAccess.Interfaces
{
    public interface ICategoryData
    {
        Task CreateCategory(CategoryModel category);
        Task<List<CategoryModel>> GetAllCategories();
    }
}