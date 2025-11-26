namespace LanBasedHelpDeskTickingSystem.Services.Implementations;

public class GeminiEmbeddingService
{
    // private readonly GenerativeModel _model;
    //
    // public GeminiEmbeddingService(IConfiguration config)
    // {
    //     var apiKey = config["Gemini:ApiKey"];
    //     var googleAI = new GoogleAI(apiKey);
    //
    //     // Use an embedding model name — check which embeddings are supported
    //     _model = googleAI.GenerativeModel(Model.TextEmbedding004);
    // }
    //
    // public async Task<float[]> GetEmbeddingAsync(string text)
    // {
    //     // Mscc.GenerativeAI provides GenerateEmbedding / Embedding capabilities
    //     var embeddingResponse = await _model.CreateEmbeddingAsync(new EmbeddingRequest
    //     {
    //         Model = Model.TextEmbedding004,
    //         Input = new[] { text }
    //     });
    //
    //     // The API returns a collection, we take the first
    //     return embeddingResponse.Data[0].Embedding.ToArray();
    // }
    //
    // public double CosineSimilarity(float[] a, float[] b)
    // {
    //     double dot = 0, magA = 0, magB = 0;
    //     for (int i = 0; i < a.Length; i++)
    //     {
    //         dot += a[i] * b[i];
    //         magA += a[i] * a[i];
    //         magB += b[i] * b[i];
    //     }
    //     return dot / (Math.Sqrt(magA) * Math.Sqrt(magB));
    // }
}