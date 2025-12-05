using LanBasedHelpDeskTickingSystem.Data;
using LanBasedHelpDeskTickingSystem.Entities.Enums;
using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Entities.Responses;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;
using LanBasedHelpDeskTickingSystem.Services.Implementations;
using LanBasedHelpDeskTickingSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LanBasedHelpDeskTickingSystem.Repository.Implementations;

public class FileRepository(AppDbContext db, IFileService fileService) : IFileRepository
{

    public async Task<ApiResultResponse> SaveTicketAttachmentsAsync(int ticketId, List<IFormFile> files)
    {
        var errors = new List<string>();
        var attachments = new List<Attachment>();

        foreach (var file in files)
        {
            var kvp = await fileService.SaveFileAsync(file);

            if (kvp == null)
            {
                errors.Add(file.FileName);
                continue;
            }
            
            var distinctName = kvp.Value.Value;
            var extension = kvp.Value.Key;
            
            attachments.Add(new Attachment
            {
                TicketId = ticketId,
                FileName = distinctName,
                FileExtension = extension
            });
        }

        if (attachments.Count > 0)
        {
            await db.SetEntity<Attachment>().AddRangeAsync(attachments);
            await db.SaveChangesAsync();
        }
        
        return errors.Count > 0 ? ApiResultResponse.Ok("Successfully uploaded. Some files failed to upload: " + string.Join(", ", errors)) : ApiResultResponse.Ok("Files uploaded successfully.");
    }
    
    public async Task<ApiResultResponse> RemoveAttachmentAsync(int attachmentId)
    {
        var attachment = await db.SetEntity<Attachment>().FindAsync(attachmentId);
        
        if (attachment == null) return ApiResultResponse.Error("Attachment not found.");

        // var deleteResult = await fileService.DeleteFileAsync(attachment.FileName);
        // if (!deleteResult)
        // {
        //     return ApiResultResponse.Error("Failed to delete the file from storage.");
        // }

        await using var transaction = await db.Database.BeginTransactionAsync();
        
        try
        {
            db.SetEntity<Attachment>().Remove(attachment);
        
            await db.SaveChangesAsync();
            await transaction.CommitAsync();

            return ApiResultResponse.Ok("Attachment deleted successfully.");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return ApiResultResponse.Error("Failed to delete attachment.");
        }
    }
    
    // public async Task DownloadCsvAsync(int ticketId, HttpResponse response)
    // {
    //     var attachments = await db.SetEntity<Attachment>()
    //         .Where(a => a.TicketId == ticketId)
    //         .ToListAsync();
    //
    //     var csvContent = "AttachmentId,FileName,FileExtension,UploadedAt\n" +
    //                      string.Join("\n", attachments.Select(a => $"{a.Id},{a.FileName},{a.FileExtension},{a.UploadedAt:O}"));
    //
    //     response.ContentType = "text/csv";
    //     response.Headers.Add("Content-Disposition", $"attachment; filename=\"ticket_{ticketId}_attachments.csv\"");
    //     await response.WriteAsync(csvContent);
    // }
    
}