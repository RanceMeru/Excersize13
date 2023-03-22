using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

class OpenAIGenerationResponse
{
    public List<OpenAIGenerationChoice> choices { get; set; }
}

class OpenAIGenerationChoice
{
    public string text { get; set; }
}
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Hi! I'm a chatbot. What's your name?");
        var name = Console.ReadLine();

        while (true)
        {
            Console.Write($"{name}: ");
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input)) continue;
            if (input.ToLower() == "exit") break;

            var response = await GenerateResponse(input);
            Console.WriteLine($"Bot: {response}");
            Console.WriteLine();
        }
    }

    static async Task<string> GenerateResponse(string prompt)
    {
        var httpClient = new HttpClient();

        var openaiApiKey = "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";
        var openaiApiUrl = "https://api.openai.com/v1/engines/davinci-codex/completions";

        var requestBody = new
        {
            prompt,
            max_tokens = 50,
            temperature = 0.5,
            top_p = 1,
            n = 1,
            stop = "\n"
        };

        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {openaiApiKey}");

        var jsonRequestBody = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");
        var httpResponse = await httpClient.PostAsync(openaiApiUrl, content);

        var jsonResponse = await httpResponse.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<OpenAIGenerationResponse>(jsonResponse);

        return responseObject.choices[0].text.Trim();
    }
}
