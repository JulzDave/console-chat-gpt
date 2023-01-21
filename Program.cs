using OpenAI_API;
using System.Text;

DotNetEnv.Env.Load();
var openApiKey = Environment.GetEnvironmentVariable("OPEN_API_KEY");
var api = new OpenAIAPI(openApiKey);
bool firstRun = true;
StringBuilder memory = new StringBuilder();

while (true)
{
    firstRun = CheckIfFirstRun(firstRun);

    string? question = Console.ReadLine();

    if (string.IsNullOrEmpty(question))
    {
        continue;
    }
    else if (question.ToLower() == "exit")
    {
        break;
    }

    await CallGpt(api, memory, question);
}

static bool CheckIfFirstRun(bool firstRun)
{
    if (firstRun)
    {
        Console.WriteLine("\nWhat is your question please? (Type 'exit' to quit) \n");
        firstRun = false;
    }
    else
    {
        Console.Write("\n(Type 'exit' to quit) >>> ");
    }

    return firstRun;
}

static async Task CallGpt(OpenAIAPI api, StringBuilder memory, string? question)
{
    memory.Append($" {question}");
    
    CompletionRequest request = new CompletionRequest(
        question,
        Model.DavinciText,
        200,
        0.5,
        presencePenalty: 0.1,
        frequencyPenalty: 0.1
        );

    request.Prompt += memory.ToString();

    await foreach (var token in api.Completions.StreamCompletionEnumerableAsync(request))
    {
        memory.Append($" {token}");
        Console.Write(token);
    }

    Console.WriteLine();
}