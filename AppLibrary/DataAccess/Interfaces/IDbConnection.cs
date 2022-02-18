namespace AppLibrary.DataAccess.Interfaces
{
    public interface IDbConnection
    {
        IMongoCollection<CategoryModel> CategoryCollection { get; set; }
        string CategoryCollectionName { get; }
        MongoClient Client { get; }
        string DbName { get; }
        IMongoCollection<StatusModel> StatusCollection { get; set; }
        string StatusCollectionName { get; set; }
        IMongoCollection<SuggestionModel> SuggestionCollection { get; set; }
        string SuggestionCollectionName { get; set; }
        IMongoCollection<UserModel> UserCollection { get; set; }
        string UserCollectionName { get; set; }
    }
}