using LanBasedHelpDeskTickingSystem.Libs;

namespace LanBasedHelpDeskTickingSystem.Entities.Responses;

public class PaginateResponse<T>
{
    public PaginatedList<T> Data { get; set; }
    public int Total { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    
    public static PaginateResponse<T> Create(PaginatedList<T> data)
    {
        return new PaginateResponse<T>
        {
            Data = data,
            Total = data.Count,
            TotalItems = data.TotalItems,
            TotalPages = data.TotalPages
        };
    }
}
