using Microsoft.Identity.Client;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;

namespace ProbabilityTrades.Domain.Services.ApiServices;

public class OpenAIApiService : IOpenAIApiInterface
{
    private readonly IConfiguration _configuration;
    private readonly IOpenAIAPI _openAiAPI;

    public OpenAIApiService(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _openAiAPI = new OpenAIAPI(_configuration.GetValue<string>("OpenAI:SecretKey"));
    }

    public async Task Test()
    {
        try
        {

            // read file from desktop
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string text = File.ReadAllText(Path.Combine(path, "test-input1.txt"));

            string answer = string.Empty;

            List<ChatMessage> messages = new List<ChatMessage>();
            messages.Add(new ChatMessage { Role = ChatMessageRole.User, TextContent = GetContentString("BTC") });
            messages.Add(new ChatMessage { Role = ChatMessageRole.Assistant, TextContent = text });

            ChatRequest chatRequest = new ChatRequest
            {
                Messages = messages,
                Model = Model.ChatGPTTurbo,
                Temperature = 0
            };

            var result = _openAiAPI.Chat.CreateChatCompletionAsync(chatRequest);

            if (result != null)
            {
                Console.WriteLine(text);
                foreach (var item in result.Result.Choices)
                {
                    Console.WriteLine(item.Message.Content);
                }
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    // This is the key to getting the response we are looking for.
    private string GetContentString(string coin)
    {
        return $"Taking into consideration {coin} and given the input text, please give me your sentiment as a one-word response as to whether this text " +
               $"is either positive for {coin}, negative against {coin},  neutral with {coin}, or does-not-appy meaning this text is not about {coin} whether or " +
               $"not it's positive, negative, or neutral.";
    }
}