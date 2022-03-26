using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppUI.Pages
{
    public partial class Index
    {
        private List<SuggestionModel> suggestions;
        private List<CategoryModel> categories;
        private List<StatusModel> status;
        private string selectedCategory = "All";
        private string selectedStatus = "All";
        private string searchText = "";
        bool isSortedByNew = true;
        protected async override Task OnInitializedAsync()
        {
            categories = await categoryData.GetAllCategories();
            status = await statusData.GetAllStatuses();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadFilterState();
                await FilterSuggestions();
                StateHasChanged();
            }
        }

        private async Task LoadFilterState()
        {
            var stringResults = await sessionStorage.GetAsync<string>(nameof(selectedCategory));
            selectedCategory = stringResults.Success ? stringResults.Value : "All";
            stringResults = await sessionStorage.GetAsync<string>(nameof(selectedStatus));
            selectedStatus = stringResults.Success ? stringResults.Value : "All";
            stringResults = await sessionStorage.GetAsync<string>(nameof(searchText));
            searchText = stringResults.Success ? stringResults.Value : "All";
            var boolResults = await sessionStorage.GetAsync<bool>(nameof(isSortedByNew));
            isSortedByNew = stringResults.Success ? boolResults.Value : true;
        }

        private async Task FilterSuggestions()
        {
            var output = await suggestionData.GetAllAprovedSuggestions();
            if (selectedCategory != "All")
                output = output.Where(s => s.Category?.CategoryName == selectedCategory).ToList();
            if (selectedStatus != "All")
                output = output.Where(s => s.SuggestionStatus?.StatusName == selectedStatus).ToList();
            if (!string.IsNullOrEmpty(searchText))
            {
                output = output.Where(s => s.Suggestion.Contains(searchText, StringComparison.InvariantCulture) || s.Description.Contains(searchText, StringComparison.InvariantCulture)).ToList();
            }

            if (isSortedByNew)
            {
                output = output.OrderByDescending(s => s.DateCreated).ToList();
            }
            else
            {
                output = output.OrderByDescending(s => s.UserVotes.Count).ThenByDescending(s => s.DateCreated).ToList();
            }

            suggestions = output;
            await SaveFilterState();
        }

        private async Task SaveFilterState()
        {
            await sessionStorage.SetAsync(nameof(selectedCategory), selectedCategory);
            await sessionStorage.SetAsync(nameof(status), status);
            await sessionStorage.SetAsync(nameof(searchText), searchText);
            await sessionStorage.SetAsync(nameof(isSortedByNew), isSortedByNew);
        }
    }
}