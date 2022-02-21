using AppLibrary.DataAccess;
using AppLibrary.DataAccess.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ServicesInjection
{
    public static class RegisterServices
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddSingleton<IDbConnection, DbConnection>();
            services.AddSingleton<ICategoryData, MongoCategoryData>();
            services.AddSingleton<IStatusData, MongoStatusData>();
            services.AddSingleton<ISuggestionData, MongoSuggestionData>();
            services.AddSingleton<IUserData, MongoUserData>();
        }
    }
}