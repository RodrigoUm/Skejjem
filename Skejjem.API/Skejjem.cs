using System.Text.Json;
using System.Text;
using System.Runtime.CompilerServices;

namespace Skejjem.API;

public class Skejjem
{
    // this needs to come from the config file
    // or environment variable
    // or some other secure place
    // this is a fake key
    public Skejjem(IConfiguration config) {
        this.ApiKey = config["OpenAI:ApiKey"];
    }
    private readonly string ApiKey;
    private static readonly string Endpoint = "https://api.openai.com/v1/chat/completions";

    public async Task<string> GetRecommendedSchedule(string taskList)
    {
        var prompt = $@"You are a helpful assistant that organizes tasks into a daily schedule. Prioritize by importance and urgency. Don't output any stars or emojis.  The output schedule should be fun and simple visualization of what they should do. Output a json object of an array of tasks with a time next to each task. Use the 24-hour format. The tasks are as follows:
        Tasks:  {taskList} 
        Provide a thoughtful schedule with some fun explanations. 
        Here's a sample of the exact output format:
          {{
          ""schedule"": {{  ""tasks"": [    {{      ""task"": ""drink 44 gallons throughout the day"",      ""time"": ""08:00""    }},    {{      ""task"": ""run 44 miles"",      ""time"": ""09:00""    }},    {{      ""task"": ""eat a whole cow"",      ""time"": ""15:00""    }},    {{      ""task"": ""eat a whole pig"",      ""time"": ""18:00""    }}  ]}}
        }}
    ";

        var requestData = new
        {
            model = "gpt-4o-mini",
            messages = new object[]
            {
            new { role = "user", content = prompt }
            },
            temperature = 0.7,
            max_tokens = 300
        };

        var jsonContent = JsonSerializer.Serialize(requestData);
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(Endpoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error querying OpenAI: {response.StatusCode}, {errorContent}");
            }

            var resultJson = await response.Content.ReadAsStringAsync();
            using (var jsonDoc = JsonDocument.Parse(resultJson))
            {
                var root = jsonDoc.RootElement;
                string output = root!.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                return output;
            }
        }
    }
}
