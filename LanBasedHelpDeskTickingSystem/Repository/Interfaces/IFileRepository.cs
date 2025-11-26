using LanBasedHelpDeskTickingSystem.Entities.Responses;

namespace LanBasedHelpDeskTickingSystem.Repository.Interfaces;

public interface IFileRepository
{
    public Task<ApiResultResponse> SaveTicketAttachmentsAsync(int ticketId, List<IFormFile> files);
    public Task<ApiResultResponse> RemoveAttachmentAsync(int attachmentId);
}