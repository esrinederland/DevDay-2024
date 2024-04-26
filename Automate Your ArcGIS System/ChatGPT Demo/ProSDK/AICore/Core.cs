using ArcGIS.Core.CIM;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Mapping;
using Azure.AI.OpenAI;
using System.Text;
using System.Text.Json;

namespace AiCore
{
    public static class Core
    {
        public static string GetAiResponse(string question)
        {       
            // bron: https://platform.openai.com/
            // maak een key aan en plak deze hieronder
            string openAiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;
            OpenAIClient client = new(openAiKey);
            ChatCompletionsOptions options = new(
                "gpt-3.5-turbo",
                //"gpt-4",
                new List<ChatRequestMessage>()
                {
                    new ChatRequestSystemMessage(question)
                }
            ){
                Temperature = 0
            };


            var answers = client.GetChatCompletions(options);            

            StringBuilder sb = new();
            foreach (var choice in answers.Value.Choices)
            {
                sb.AppendLine(choice.Message.Content);                
            }
            return sb.ToString();
        }

        public static void WriteResultToFile(JsonDocument json, string path)
        {
            var options = new JsonWriterOptions
            {
                Indented = true
            };

            using var stream = new FileStream(path, FileMode.CreateNew);
            using var writer = new Utf8JsonWriter(stream, options);
            json.WriteTo(writer);
            writer.Flush();
        }

        public static string OpenJsonStreamResonse(string path)
        {
            using var stream = new FileStream(path, FileMode.Open);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public static CIMTextGraphic GetTextGraphic(string name, MapPoint point, CIMColor color)
        {
            // Create a simple text symbol
            CIMTextSymbol textSymbol = SymbolFactory.Instance.ConstructTextSymbol(color, 10.0, "Arial");
            CIMTextGraphic textGraphic = new()
            {
                Text = "   " + name,
                Symbol = textSymbol.MakeSymbolReference(),
                Placement = Anchor.CenterPoint,
                Shape = point
            };
            return textGraphic;
        }

        public static CIMPointGraphic GetPointGraphic(string name, MapPoint projectedXY, CIMColor color)
        {
            // Maak een puntgraphic
            CIMPointSymbol pointSymbol = SymbolFactory.Instance.ConstructPointSymbol(color, 10.0, SimpleMarkerStyle.Circle);
            CIMPointGraphic pointGraphic = new()
            {
                Symbol = pointSymbol.MakeSymbolReference(),
                Location = projectedXY,
                Name = name,
                Placement = Anchor.CenterPoint
            };
            return pointGraphic;
        }
    }
}
