using StockApp.Domain.Interfaces;

namespace StockApp.Application.Services
{
    public class SentimentAnalysisService : ISentimentAnalysisService
    {
        public string AnalizeSentiment (string text)
        {
            var lower = text.ToLower();

            if (lower.Contains("ótimo") || lower.Contains("bom") || lower.Contains("excelente"))
                return "Positivo";

            if (lower.Contains("ruim") || lower.Contains("péssimo") || lower.Contains("horrível"))
                return "Negativo";

            return "Neutro";
        }

        string ISentimentAnalysisService.AnalyzeSentiment(string text)
        {
            throw new NotImplementedException();
        }
    }
}
