namespace StockApp.Domain.Interfaces
{
    public interface ISentimentAnalysisService
    {
        string AnalyzeSentiment(string text);
    }
}
