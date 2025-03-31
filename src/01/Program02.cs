using _01;
using Azure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Plugins.Web.Bing;
using System.Text.Json;


#pragma warning disable SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

// Bing Search with function calling.

// Create a kernel with OpenAI chat completion
IKernelBuilder kernelBuilder = Kernel.CreateBuilder();

kernelBuilder.AddAzureOpenAIChatCompletion(
        "gpt-4o", "https://hkragsessionai498708833706.openai.azure.com/", new DefaultAzureCredential());

//kernelBuilder.Services.AddSingleton<ITestOutputHelper>(output);
kernelBuilder.Services.AddSingleton<IFunctionInvocationFilter, FunctionInvocationFilter>();
Kernel kernel = kernelBuilder.Build();

#region
string apiKey = "";
#endregion

// Create a search service with Bing search
var textSearch = new BingTextSearch(apiKey: apiKey);

// Build a text search plugin with Bing search and add to the kernel
var searchPlugin = textSearch.CreateWithSearch("SearchPlugin");
kernel.Plugins.Add(searchPlugin);

// Invoke prompt and use text search plugin to provide grounding information
OpenAIPromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };
KernelArguments arguments = new(settings);

Console.WriteLine(await kernel.InvokePromptAsync("Share latest trends on delhi assembly elections?", arguments));

//Console.WriteLine(await kernel.InvokePromptAsync("write a function in c# to loop through array of numbers?", arguments));

Console.ReadLine();