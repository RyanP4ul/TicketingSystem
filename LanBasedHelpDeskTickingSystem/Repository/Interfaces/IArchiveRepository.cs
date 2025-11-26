using LanBasedHelpDeskTickingSystem.Entities.Responses;

namespace LanBasedHelpDeskTickingSystem.Repository.Interfaces;

public interface IArchiveRepository
{
    public Task<ApiResultResponse> ArchiveActionAsync(int id, string type, string action);
}