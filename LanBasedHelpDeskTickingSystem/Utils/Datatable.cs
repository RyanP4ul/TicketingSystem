using System.Data;
using System.Text;

namespace LanBasedHelpDeskTickingSystem.Utils;

public class Datatable
{
    
    public static string DataTableToCsv(DataTable table)
    {
        var csv = new StringBuilder();

        for (int i = 0; i < table.Columns.Count; i++)
        {
            csv.Append(table.Columns[i].ColumnName);
            if (i < table.Columns.Count - 1)
                csv.Append(",");
        }
        csv.AppendLine();

        foreach (DataRow row in table.Rows)
        {
            for (int i = 0; i < table.Columns.Count; i++)
            {
                csv.Append(row[i]?.ToString()?.Replace(",", " "));
                if (i < table.Columns.Count - 1)
                    csv.Append(",");
            }
            csv.AppendLine();
        }

        return csv.ToString();
    }
    
}