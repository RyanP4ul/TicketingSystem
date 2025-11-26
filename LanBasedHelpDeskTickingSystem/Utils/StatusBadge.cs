namespace LanBasedHelpDeskTickingSystem.Utils;

public static class StatusBadge
{
    
    public static string GetStatusBadge(string status)
    {
        
        return status.ToLower() switch
        {
            "open" => "<span class='bg-blue-100 text-blue-800 text-xs font-medium me-2 px-2.5 py-0.5 rounded-sm dark:bg-blue-900 dark:text-blue-300'>Open</span>",
            "in_progress" => "<span class='bg-yellow-100 text-yellow-800 text-xs font-medium me-2 px-2.5 py-0.5 rounded-sm dark:bg-yellow-900 dark:text-yellow-300'>In Progress</span>",
            "pending" => "<span class='bg-gray-100 text-gray-800 text-xs font-medium me-2 px-2.5 py-0.5 rounded-sm dark:bg-gray-700 dark:text-gray-300'>Pending</span>",
            "resolved" => "<span class='bg-green-100 text-green-800 text-xs font-medium me-2 px-2.5 py-0.5 rounded-sm dark:bg-green-900 dark:text-green-300'>Resolved</span>",
            "closed" => "<span class='bg-red-100 text-red-800 text-xs font-medium me-2 px-2.5 py-0.5 rounded-sm dark:bg-red-900 dark:text-red-300'>Closed</span>",
            _ => throw new ArgumentOutOfRangeException($"{status} Invalid status value")
        };
    }
    
}