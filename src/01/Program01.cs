//// See https://aka.ms/new-console-template for more information
//using _01;
//using Azure.Identity;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.SemanticKernel;
//using Microsoft.SemanticKernel.Data;
//using Microsoft.SemanticKernel.Plugins.Web.Bing;
//using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

//#pragma warning disable SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
//#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

//Console.WriteLine("Hello, World!");

//// 01- This sample SHOWS How to use Bing Text Search with RAG. First part of sample makes a call with no bing search output and second one includes the bing search output as a part of prompt ************
//// Direct use of bing search and with plugin in RAG

//#region ADD BING KEY BELOW FOR THIS TO RUN
//string apiKey = "";
//#endregion

//// Create an ITextSearch instance using Bing search
//var textSearch = new BingTextSearch(apiKey: apiKey);

//var query = "delhi assembly election 2025";

//// Search and return results
//// Performs a search operation to get results for the provided query.
//// The search results will contain a snippet of text from the webpage that describes its contents.
//// This provides only a limited context i.e., a subset of the web page contents and no link to the source of the information.
//KernelSearchResults<string> searchResults = await textSearch.SearchAsync(query, new() { Top = 4 });
//await foreach (string result in searchResults.Results)
//{
//    Console.WriteLine(result);
//    Console.WriteLine("-------***********---------");
//}

////Similary Google Text Search is also available by Sementic Kernel
////var textSearch = new GoogleTextSearch(searchEngineId: "<Your Google Search Engine Id>",apiKey: "<Your Google API Key>");

//Console.WriteLine("Hit enter to continue. Above are just the bing search result. It shows how easily use can use BingTextSearch class from Sementic kernel");
//Console.ReadLine();



//IKernelBuilder kernelBuilder = Kernel.CreateBuilder();

//kernelBuilder.AddAzureOpenAIChatCompletion(
//        "gpt-4o", "https://hkragsessionai498708833706.openai.azure.com/", new DefaultAzureCredential());

//kernelBuilder.Services.AddSingleton<IPromptRenderFilter, PromptFilterExample>();

//Kernel kernel = kernelBuilder.Build();

//// Build a text search plugin with Bing search and add to the kernel
//var searchPlugin = textSearch.CreateWithGetTextSearchResults("SearchPlugin");
//kernel.Plugins.Add(searchPlugin);

//// WITH OUT RAG
//// Invoke prompt and use text search plugin to provide grounding information
//query = "Delhi Assembly election 2025?";

//string promptTemplate = """
//{{query}}
//""";

//KernelArguments arguments = new() { { "query", query } };

//HandlebarsPromptTemplateFactory promptTemplateFactory = new();

//Console.WriteLine(await kernel.InvokePromptAsync(
//    promptTemplate,
//    arguments,
//    templateFormat: HandlebarsPromptTemplateFactory.HandlebarsTemplateFormat,
//    promptTemplateFactory: promptTemplateFactory
//));

//Console.WriteLine("-------------WITH RAG--------------------");

//string promptWithBingResultTemplate = """
//{{#with (SearchPlugin-GetTextSearchResults query)}}  
//    {{#each this}}  
//    Name: {{Name}}
//    Value: {{Value}}
//    Link: {{Link}}
//    -----------------
//    {{/each}}  
//{{/with}}  

//{{query}}

//Include citations to the relevant information where it is referenced in the response.
//""";

//arguments = new() { { "query", query } };

//promptTemplateFactory = new();

//Console.WriteLine(await kernel.InvokePromptAsync(
//    promptWithBingResultTemplate,
//    arguments,
//    templateFormat: HandlebarsPromptTemplateFactory.HandlebarsTemplateFormat,
//    promptTemplateFactory: promptTemplateFactory
//));

//Console.ReadLine();
