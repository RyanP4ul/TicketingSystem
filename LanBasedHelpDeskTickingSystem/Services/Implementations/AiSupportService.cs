using GenerativeAI;
using GenerativeAI.Types;
using LanBasedHelpDeskTickingSystem.Data;
using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Entities.Responses;
using LanBasedHelpDeskTickingSystem.Services.Interfaces;
using System.Text.Json; // Import this for JSON parsing
using System.Text.RegularExpressions;

namespace LanBasedHelpDeskTickingSystem.Services.Implementations;

public class AiSupportService(AppDbContext db, GoogleAi client) : IAiSupportService
{
    
    private const string ModelName = "gemini-2.5-flash";
    private const string BaseUrl = "http://localhost:5194";
    private readonly string[] _stopWords = { "how", "do", "i", "to", "the", "a", "an", "my", "is", "it", "not" };
    
    private const string SystemInstruction = $"""
                                                       You are the **LAN BASED HELP DESK AI** for **STI COLLEGE BALAGTAS** students and teachers.
                                                       You will assist users by answering their questions based on the content of the provided article from the knowledge base.
                                                       You were developed by **Ryan Paul Espinola** to provide immediate assistance to students and teachers regarding campus technical concerns.
                                                       Your primary goal is to provide technical support regarding **technical issues, account problems, and network connectivity** ONLY.
                                                       You **MUST** base your answer exclusively on the article provided in the KNOWLEDGE BASE below.
                                                       You **MUST NOT** assist with or provide information about any exploits, vulnerabilities, or activities that could harm the website, database, or any system.
                                                       If the answer is not in the article, you must state, 'I recommend submitting a ticket through the help desk system, as that specific information is not in my knowledge base. You can submit a new ticket [here]({BaseUrl}/User/Tickets/New).'
                                                       If the user's prompt is about **cashier, scholarship, tuition fee, or Microsoft license**, you must state, 'This topic is outside my area of expertise. Please contact the appropriate department for assistance.'
                                                       If asked about your identity, clearly state your name and who developed you.
                                                       If the user asks what campus you are, respond with: 'I am the AI assistant for **STI College Balagtas**.'
                                                       If the user asks for a link, URL, or domain, provide the appropriate reference link(s) from the list below.
                                                       
                                                       If the user's prompt is about the **LAN BASED HELP DESK TICKING SYSTEM**, you must explain what it is, its purpose, and its key features. For example:
                                                       - "The LAN BASED HELP DESK TICKING SYSTEM is a platform designed to assist students and teachers with technical issues, account problems, and network connectivity."
                                                       - "Its purpose is to streamline the process of reporting and resolving technical concerns within the campus."
                                                       - "Key features include ticket submission, knowledge base access."
                                                       
                                                       You are allowed to use emojis or create custom Unicode characters to make your responses more engaging. For example:
                                                       - Use ✅ for confirmation.
                                                       - Use ❓ for questions.
                                                       - Use ⚠️ for warnings.
                                                       - Create custom emojis or Unicode characters when appropriate, such as 🌟, 🎓, or 🖥️.
                                                    
                                                       IMPORTANT: You must format links using Markdown like this: [Link Text](URL).
                                                   
                                                       Here are your reference links:
                                                       - Main Page: [Home]({BaseUrl})
                                                       - Login Page: [Login]({BaseUrl}/auth/login)
                                                       - Knowledge Base Page: [KnowledgeBase]({BaseUrl}/knowledge-base)
                                                       - Register Page: [Register]({BaseUrl}/auth/register)
                                               """;

    public async Task<string?> GetAnswerFromArticleAsync(string prompt)
    {
        var maxRetries = 3;
        var delay = 2000;

        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                var model = client.CreateGenerativeModel(ModelName, systemInstruction: SystemInstruction);
                var cleanInput = prompt.ToLower();

                var keywords = cleanInput.Split(' ')
                    .Where(word => !_stopWords.Contains(word) && word.Length > 2)
                    .ToList();
        
                var article = db.SetEntity<KnowledgeBase>()
                    .AsEnumerable()
                    .Select(a => new 
                    { 
                        a.Id,
                        a.Title, 
                        a.Content,
                        a.IsDeleted,
                        Score = keywords.Count(k => a.Content.Contains(k, StringComparison.CurrentCultureIgnoreCase) || a.Title.Contains(k, StringComparison.CurrentCultureIgnoreCase)) 
                    })
                    .OrderByDescending(x => x.Score)
                    .FirstOrDefault(x => x.Score > 0 && !x.IsDeleted);
        
                return article == null ? 
                    (await model.GenerateContentAsync(prompt)).Text :
                    (await model.GenerateContentAsync($"Here is the knowledge base:\n{article}\n\nArticle Link (use Markdown format): [Link Text]({BaseUrl}/User/KnowledgeBase/{article.Id})\n\nAnswer the user question: {prompt}")).Text;
            }
            catch (Exception)
            {
                if (i == maxRetries - 1)
                {
                    throw;
                }
                
                await Task.Delay(delay);
            }
        }

        return null;
    }

    public async Task<List<KnowledgeMatchResponse>> GetTicketSuggestionsAsync(string ticketTitle, string ticketDescription)
    {
        try
        {
            string fullSearchTerm = $"{ticketTitle} {ticketDescription}".ToLower();

            var keywords = fullSearchTerm.Split(new[] { ' ', '.', ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(word => !_stopWords.Contains(word) && word.Length > 2)
                .Distinct()
                .ToList();

            if (!keywords.Any()) return new List<KnowledgeMatchResponse>();
            
            var candidates = db.SetEntity<KnowledgeBase>()
                .AsEnumerable()
                .Select(a => new
                {
                    a.Id,
                    a.Title,
                    a.Content,
                    a.IsDeleted,
                    KeywordHits = keywords.Count(k => a.Content.Contains(k, StringComparison.CurrentCultureIgnoreCase) || a.Title.Contains(k, StringComparison.CurrentCultureIgnoreCase))
                })
                .Where(x => x.KeywordHits > 0 && !x.IsDeleted)
                .OrderByDescending(x => x.KeywordHits)
                .Take(5)
                .ToList();
            
            if (!candidates.Any()) return new List<KnowledgeMatchResponse>();
            
            var prompt = $$"""
                               You are a Help Desk Triage AI.
                               
                               USER TICKET:
                               Title: {{ticketTitle}}
                               Description: {{ticketDescription}}
                           
                               CANDIDATE ARTICLES:
                               {{string.Join("\n\n", candidates.Select((c, i) => $"Article #{i + 1} (ID: {c.Id}):\nTitle: {c.Title}\nContent: {c.Content.Substring(0, Math.Min(c.Content.Length, 300))}..."))}}
                           
                               TASK:
                               Analyze the User Ticket against the Candidate Articles.
                               Return a JSON ARRAY of the top 3 matches.
                               For each match, calculate a 'match_score' (0-100) based on how well the article solves the specific problem.
                               Generate a 'snippet' (max 15 words) explaining why it matches.
                           
                               REQUIREMENTS:
                               - Output raw JSON only. No markdown formatting (no ```json).
                               - Structure: [{"id": 123, "title": "...", "snippet": "...", "match_score": 95}]
                               - If an article is irrelevant (score < 40), do not include it.
                           """;
            
            var model = client.CreateGenerativeModel(ModelName);
            var response = await model.GenerateContentAsync(prompt);
            var jsonResponse = response.Text;
            
            jsonResponse = jsonResponse.Replace("```json", "").Replace("```", "").Trim();

            var matches = JsonSerializer.Deserialize<List<KnowledgeMatchResponse>>(jsonResponse);
            
            return matches?.OrderByDescending(m => m.match_score).ToList() ?? new List<KnowledgeMatchResponse>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AI Suggestion Error: {ex.Message}");
            return new List<KnowledgeMatchResponse>();
        }
    }

    public async Task<string> AnalyzeTicketPriorityAsync(string title, string description)
    {
        try
        {
            var prompt = $$"""
                           You are an IT Support Triage System.
                           Analyze the following ticket and assign a priority level.

                           Ticket Title: {{title}}
                           Ticket Description: {{description}}

                           Rules for Priority:
                           - High:
                             * An entire Computer Lab has lost connectivity.
                             * A Teacher/Staff member cannot teach or work due to technical failure.
                             * Student account issue preventing them from taking an ongoing EXAM.
                             * Internet loss in key areas (Faculty Room, Library).
                           
                           - Medium:
                             * Single PC in a laboratory is malfunctioning (mouse/keyboard/monitor).
                             * Printer or peripheral issues in offices.
                             * Standard Student account/password reset (non-exam).
                             * Software installation requests.
                           
                           - Low:
                             * General inquiries ("How do I connect?").
                             * Cosmetic issues on the website.
                             * Personal device (BYOD) connectivity questions.
                             * Feedback or feature requests.

                           RESPONSE FORMAT:
                           Reply strictly with ONE word from this list: [Low, Medium, High].
                           Do not add punctuation, reasoning, or markdown.
                           """;
            
            var model = client.CreateGenerativeModel(ModelName);
            var response = await model.GenerateContentAsync(prompt);
        
            var priority = response.Text?.Trim() ?? "Medium";

            string[] validPriorities = { "Low", "Medium", "High" };
        
            return !validPriorities.Contains(priority, StringComparer.OrdinalIgnoreCase) ? "Medium" : priority;
        }
        catch (Exception)
        {
            return "Medium";
        }
    }
    
    public async Task<bool> IsTicketRelevantAsync(string title, string description)
    {
        try
        {
            var prompt = $$"""
                           You are a Content Moderator for the STI College Balagtas Help Desk System.
                           Your job is to VALIDATE if a user's ticket is relevant to school operations, technical support, or campus facilities.

                           Ticket Title: {{title}}
                           Ticket Description: {{description}}

                           SCOPE OF ALLOWED TOPICS (Return YES):
                           - Technical issues (Computer Lab PCs, Projectors, Printers).
                           - Network issues (WiFi, LAN, Internet connectivity).
                           - Account issues (Student Portal, ELMS, Office 365, Microsoft Teams).
                           - School Facilities (Aircon, Lights, Chairs in classrooms/labs).
                           - General inquiries about school IT policies or software.

                           SCOPE OF REJECTED TOPICS (Return NO):
                           - Personal life advice, dating, or non-school related chatter.
                           - Video games (Mobile Legends, Valorant) unless related to installing school-sanctioned esports.
                           - Homework answers (e.g., "Write an essay for me").
                           - Spam, gibberish, or profanity.
                           - Topics clearly unrelated to STI College.

                           RESPONSE FORMAT:
                           Reply strictly with ONE word: "YES" if it is relevant, or "NO" if it should be rejected.
                           """;

            var model = client.CreateGenerativeModel(ModelName);
            var response = await model.GenerateContentAsync(prompt);
        
            var result = response.Text?.Trim().ToUpper();

            return result == "YES";
        }
        catch (Exception)
        {
            return true; 
        }
    }

}