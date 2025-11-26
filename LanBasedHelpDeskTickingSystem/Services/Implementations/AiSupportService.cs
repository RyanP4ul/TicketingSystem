using GenerativeAI;
using GenerativeAI.Types;
using LanBasedHelpDeskTickingSystem.Data;
using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Services.Interfaces;

namespace LanBasedHelpDeskTickingSystem.Services.Implementations;

public class AiSupportService(AppDbContext db, GoogleAi client) : IAiSupportService
{
    
    private const string ModelName = "gemini-2.5-flash";
    private const string BaseUrl = "http://localhost:5194";
    
    private const string SystemInstruction = $$"""
                                                       You are the **LAN BASED HELP DESK AI** for **STI COLLEGE BALAGTAS** students and teachers.
                                                       You will assist users by answering their questions based on the content of the provided article from the knowledge base.
                                                       You were developed by **Ryan Paul Espinola** to provide immediate assistance to students and teachers regarding campus technical concerns.
                                                       Your primary goal is to provide technical support regarding **technical issues, account problems, and network connectivity** ONLY.
                                                       You **MUST** base your answer exclusively on the article provided in the KNOWLEDGE BASE below.
                                                       You **MUST NOT** assist with or provide information about any exploits, vulnerabilities, or activities that could harm the website, database, or any system.
                                                       If the answer is not in the article, you must state, 'I recommend submitting a ticket through the help desk system, as that specific information is not in my knowledge base. You can submit a new ticket [here]({{BaseUrl}}/User/Tickets/New).'
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
                                                       - Main Page: [Home]({{BaseUrl}})
                                                       - Login Page: [Login]({{BaseUrl}}/auth/login)
                                                       - Knowledge Base Page: [KnowledgeBase]({{BaseUrl}}/knowledge-base)
                                                       - Register Page: [Register]({{BaseUrl}}/auth/register)
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
                var stopWords = new[] { "how", "do", "i", "to", "the", "a", "an", "my", "is" };
        
                var keywords = cleanInput.Split(' ')
                    .Where(word => !stopWords.Contains(word) && word.Length > 2)
                    .ToList();
        
                var article = db.SetEntity<KnowledgeBase>()
                    .AsEnumerable()
                    .Select(a => new 
                    { 
                        a.Id,
                        a.Title, 
                        a.Content,
                        Score = keywords.Count(k => a.Content.ToLower().Contains(k) || a.Title.ToLower().Contains(k)) 
                    })
                    .OrderByDescending(x => x.Score)
                    .FirstOrDefault(x => x.Score > 0);
        
                // ALSO THE RESOLVED TICKETS
        
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
    
}