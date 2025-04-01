using Azure.Identity;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel;
using _01;
using Microsoft.Extensions.DependencyInjection;
using _01.Models;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0020 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

// RAG with Vector Store Text Search. In this case, document chunks are stored in inmemory vector data base. hence ensure load part is execute all the times.

// See https://aka.ms/new-console-template for more information
// See https://learn.microsoft.com/en-us/semantic-kernel/concepts/vector-store-connectors/how-to/vector-store-data-ingestion?pivots=programming-language-csharp
Console.WriteLine("Sample Ingestion!");


// Replace with your values.
var deploymentName = "text-embedding-ada-002";
var endpoint = "https://hkragsessionai498708833706.openai.azure.com/";
//var apiKey = "your-api-key";// Register Azure Open AI text embedding generation service and Redis vector store.
var collectionName = "sk-election";


var builder = Kernel.CreateBuilder()
    .AddAzureOpenAITextEmbeddingGeneration(deploymentName, endpoint, new DefaultAzureCredential())
    .AddInMemoryVectorStore()
    .AddAzureOpenAIChatCompletion("gpt-4o", "https://hkragsessionai498708833706.openai.azure.com/", new DefaultAzureCredential());

// Register the data uploader.
builder.Services.AddSingleton<DataUploader>();

builder.Services.AddSingleton<IPromptRenderFilter, PromptFilterExample>();

// Build the kernel and get the data uploader.
var kernel = builder.Build();
var dataUploader = kernel.Services.GetRequiredService<DataUploader>();


//// Load the data.
var textParagraphs = DocumentReader.ReadParagraphs(
    new FileStream(
        "Delhi-Election-2025.docx",
        FileMode.Open),
    "file:///c:/Delhi-Election-2025.docx");

await dataUploader.GenerateEmbeddingsAndUpload(
    collectionName,
    textParagraphs);


ITextEmbeddingGenerationService textEmbeddingGenerationService = kernel.Services.GetRequiredService<ITextEmbeddingGenerationService>();

IVectorStore vectorStore = kernel.Services.GetRequiredService<IVectorStore>();

#region DIRECT VECTOR STORE SEARCH
// Search the collection using a vector search.
//var searchString = "For any questions whom to reach?";
//var searchString = "How much insurance is announced?";

//var searchVector = await textEmbeddingGenerationService.GenerateEmbeddingAsync(searchString);

//var collection = vectorStore.GetCollection<string, TextParagraph>(collectionName);

//var searchResult = await collection.VectorizedSearchAsync(searchVector, new() { Top = 1 });

//// Inspect the returned hotel.
//await foreach (var record in searchResult.Results)
//{
//    Console.WriteLine("Test: " + record.Record.Text);
//    Console.WriteLine("score: " + record.Score);
//}
#endregion

var collection = vectorStore.GetCollection<string, TextParagraph>(collectionName);

var textSearch = new VectorStoreTextSearch<TextParagraph>(collection, textEmbeddingGenerationService);

//Build a text search plugin with vector store search and add to the kernel
var searchPlugin = textSearch.CreateWithGetTextSearchResults("SearchPlugin");

kernel.Plugins.Add(searchPlugin);

//Invoke prompt and use text search plugin to provide grounding information
var query = "How much insurance is announced in delhi elections 2025?";

string promptTemplate = """
            {{#with (SearchPlugin-GetTextSearchResults query)}}  
              {{#each this}}  
                Name: {{Name}}
                Value: {{Value}}
                Link: {{Link}}
                -----------------
              {{/each}}  
            {{/with}}  

            {{query}}

            Include citations to the relevant information where it is referenced in the response.
            """;
KernelArguments arguments = new() { { "query", query } };
HandlebarsPromptTemplateFactory promptTemplateFactory = new();
Console.WriteLine(await kernel.InvokePromptAsync(
    promptTemplate,
    arguments,
    templateFormat: HandlebarsPromptTemplateFactory.HandlebarsTemplateFormat,
    promptTemplateFactory: promptTemplateFactory
));


Console.ReadLine();

