namespace AppLibrary.DataAccess.Interfaces
{
    public interface ISuggestionData
    {
        Task CreateSuggestion(SuggestionModel suggestion);
        Task<List<SuggestionModel>> GetAllAprovedSuggestions();
        Task<List<SuggestionModel>> GetAllSuggestionAwaitingForApproval();
        Task<List<SuggestionModel>> GetAllSuggestions();
        Task<SuggestionModel> GetSuggestion(string id);
        Task UpdateSuggestion(SuggestionModel suggestion);
        Task UpvoteSuggestion(string suggestionId, string userId);
    }
}