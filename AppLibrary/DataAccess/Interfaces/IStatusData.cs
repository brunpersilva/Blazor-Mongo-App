namespace AppLibrary.DataAccess.Interfaces
{
    public interface IStatusData
    {
        Task CreateCategory(StatusModel status);
        Task<List<StatusModel>> GetAllStatuses();
    }
}