using Microsoft.Extensions.Caching.Memory;

namespace AppLibrary.DataAccess
{
    public class MongoSuggestionData : ISuggestionData
    {
        private readonly IDbConnection _db;
        private readonly IUserData _userData;
        private readonly IMemoryCache _cache;
        private readonly IMongoCollection<SuggestionModel> _suggestions;
        private const string _cacheName = "SuggestionData";


        public MongoSuggestionData(IDbConnection db,
                                   IUserData userData,
                                   IMemoryCache cache)
        {
            _db = db;
            _userData = userData;
            _cache = cache;
            _suggestions = _db.SuggestionCollection;
        }

        public async Task<List<SuggestionModel>> GetAllSuggestions()
        {
            var output = _cache.Get<List<SuggestionModel>>(_cacheName);

            if (output is null)
            {
                var result = await _suggestions.FindAsync(s => s.Archived == false);
                output = result.ToList();
                _cache.Set(_cacheName, output, TimeSpan.FromHours(1));
            }

            return output;
        }

        public async Task<List<SuggestionModel>> GetAllAprovedSuggestions()
        {
            var output = await GetAllSuggestions();
            return output.Where(s => s.ApprovedForRelease).ToList();
        }

        public async Task<SuggestionModel> GetSuggestion(string id)
        {
            var result = await _suggestions.FindAsync(s => s.Id == id);
            return result.FirstOrDefault();
        }

        public async Task<List<SuggestionModel>> GetAllSuggestionAwaitingForApproval()
        {
            var output = await GetAllSuggestions();
            return output.Where(s => s.ApprovedForRelease == false && s.Rejected == false).ToList();
        }

        public async Task UpdateSuggestion(SuggestionModel suggestion)
        {
            await _suggestions.ReplaceOneAsync(s => s.Id == suggestion.Id, suggestion);
            _cache.Remove(_cacheName);
        }
        public async Task UpvoteSuggestion(string suggestionId, string userId)
        {
            var client = _db.Client;
            using var session = await client.StartSessionAsync();
            session.StartTransaction();

            try
            {
                var db = client.GetDatabase(_db.DbName);
                var suggestionTransaction = db.GetCollection<SuggestionModel>(_db.SuggestionCollectionName);
                var suggestion = (await suggestionTransaction.FindAsync(s => s.Id == suggestionId)).First();
                bool isUpvote = suggestion.UserVotes.Add(userId);

                if (!isUpvote)
                {
                    suggestion.UserVotes.Remove(userId);
                }

                await suggestionTransaction.ReplaceOneAsync(s => s.Id == suggestionId, suggestion);

                var userInTransaction = db.GetCollection<UserModel>(_db.UserCollectionName);
                var user = await _userData.GetUserByIdAsync(userId);

                if (isUpvote)
                {
                    user.VotedOnSuggestions.Add(new BasicSuggestionModel(suggestion));
                }
                else
                {
                    var suggestionToRemove = user.VotedOnSuggestions.Where(s => s.Id == suggestionId).First();
                    user.VotedOnSuggestions.Remove(suggestionToRemove);
                }

                await userInTransaction.ReplaceOneAsync(u => u.Id == userId, user);
                await session.CommitTransactionAsync();

                _cache.Remove(_cacheName);
            }
            catch (Exception)
            {
                await session.AbortTransactionAsync();
                throw;
            }
        }

        public async Task CreateSuggestion(SuggestionModel suggestion)
        {
            var client = _db.Client;
            using var session = await client.StartSessionAsync();
            session.StartTransaction();

            try
            {
                var db = client.GetDatabase(_db.DbName);
                var suggestionInTransaction = db.GetCollection<SuggestionModel>(_db.SuggestionCollectionName);
                await suggestionInTransaction.InsertOneAsync(suggestion);

                var userInTransaction = db.GetCollection<UserModel>(_db.UserCollectionName);
                var user = await _userData.GetUserByIdAsync(suggestion.Author.Id);
                user.AuthoredSuggestions.Add(new BasicSuggestionModel(suggestion));
                await userInTransaction.ReplaceOneAsync(u => u.Id == user.Id, user);

                await session.CommitTransactionAsync();
            }
            catch (Exception)
            {

                await session.AbortTransactionAsync();
                throw;
            }
        }
    }
}
