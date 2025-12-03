namespace LanBasedHelpDeskTickingSystem.Entities.Responses;

public class KnowledgeMatchResponse
{
    public int id { get; set; }
    public string title { get; set; }
    public string snippet { get; set; } // A short 1-sentence summary
    public int match_score { get; set; } // The 0-100 score
}