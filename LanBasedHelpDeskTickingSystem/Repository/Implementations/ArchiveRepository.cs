using LanBasedHelpDeskTickingSystem.Data;
using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Entities.Responses;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;

namespace LanBasedHelpDeskTickingSystem.Repository.Implementations;

public class ArchiveRepository(AppDbContext db) : IArchiveRepository
{
    
    public async Task<ApiResultResponse> ArchiveActionAsync(int id, string type, string action)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
            object? entity = type.ToLower() switch
            {
                "ticket" => await db.SetEntity<Ticket>().FindAsync(id),
                "kb" => await db.SetEntity<KnowledgeBase>().FindAsync(id),
                _ => null
            };

            if (entity == null) return ApiResultResponse.Error($"{type} not found.");

            switch (action.ToLower())
            {
                case "restore":
                    switch (entity)
                    {
                        case Ticket ticket:
                            ticket.IsDeleted = false;
                            ticket.DeletedBy = null;
                            break;
                        case KnowledgeBase kb:
                            kb.IsDeleted = false;
                            break;
                    }
                    break;
                case "delete":
                    db.Remove(entity);
                    break;
            }
            
            await db.SaveChangesAsync();
            await transaction.CommitAsync();

            return ApiResultResponse.Ok($"Archive {action} successfully.");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return ApiResultResponse.Error("An error occurred while processing the archive action.");
        }
    }
    
}