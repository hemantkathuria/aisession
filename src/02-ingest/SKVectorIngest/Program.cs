using Azure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using SKVectorIngest;
using SKVectorIngest.Models;
using StackExchange.Redis;

#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0020 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

//Summary//
// This sample demonstrates how to use the Semantic Kernel to ingest data into a Redis vector store.
// Sementic kernel has provided abstraction with the help of IVectorStore. Hence changing the vector store will only need changes at the time of start up and not across the code.
// REDIS Enterprise cluster is must. Also ensure RedisJSON, RediSearch modules are ENABLED. User is listed in Managed Identity section.
// Ensure user has Cognitive Services OpenAI User and  Cognitive Services User on the Azure Open AI
// See https://aka.ms/new-console-template for more information
// See https://learn.microsoft.com/en-us/semantic-kernel/concepts/vector-store-connectors/how-to/vector-store-data-ingestion?pivots=programming-language-csharp
Console.WriteLine("Sample Ingestion!");


// Replace with your values.
var deploymentName = "text-embedding-ada-002";
var endpoint = "https://hkragsessionai498708833706.openai.azure.com/";
//var apiKey = "your-api-key";// Register Azure Open AI text embedding generation service and Redis vector store.
var collectionName = "sk-documentation";

// Initialize a connection to the Redis database.
//var configurationOptions = ConfigurationOptions.Parse("hkragsessionrdis.northcentralus.redis.azure.net:10000");

var configurationOptions = ConfigurationOptions.Parse("ragsessionredis.northcentralus.redis.azure.net:10000");

//https://github.com/Azure/Microsoft.Azure.StackExchangeRedis
await configurationOptions.ConfigureForAzureWithTokenCredentialAsync(new DefaultAzureCredential());

ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(
    configurationOptions
);

IDatabase database = connectionMultiplexer.GetDatabase();

var builder = Kernel.CreateBuilder()
    .AddAzureOpenAITextEmbeddingGeneration(deploymentName, endpoint, new DefaultAzureCredential())
    .AddRedisVectorStore();
//.AddRedisVectorStore("hkragsessionredis.northcentralus.redis.azure.net:10000");

builder.Services.AddSingleton<IDatabase>(sp => connectionMultiplexer.GetDatabase());


// Register the data uploader.
builder.Services.AddSingleton<DataUploader>();

// Build the kernel and get the data uploader.
var kernel = builder.Build();
var dataUploader = kernel.Services.GetRequiredService<DataUploader>();


#region Load Data
//// This section loads the data. Comment the section for next runs. 
//var textParagraphs = DocumentReader.ReadParagraphs(
//    new FileStream(
//        "vector-store-data-ingestion-input.docx",
//        FileMode.Open),
//    "file:///c:/vector-store-data-ingestion-input.docx");

//await dataUploader.GenerateEmbeddingsAndUpload(
//    collectionName,
//    textParagraphs);

#endregion Load Data


ITextEmbeddingGenerationService textEmbeddingGenerationService = kernel.Services.GetRequiredService<ITextEmbeddingGenerationService>();

IVectorStore vectorStore = kernel.Services.GetRequiredService<IVectorStore>();

// Search the collection using a vector search.
//var searchString = "For any questions whom to reach?";

//perform the SECOND ATTEMPT with below question. The document is having charles team information in hindi. But we are asking the question in english. still it will return the right response.
var searchString = "what is the responsibility of charles team?";

var searchVector = await textEmbeddingGenerationService.GenerateEmbeddingAsync(searchString);

var collection = vectorStore.GetCollection<string, TextParagraph>(collectionName);

var searchResult = await collection.VectorizedSearchAsync(searchVector, new() { Top = 1 });

// Inspect the returned hotel.
await foreach (var record in searchResult.Results)
{
    Console.WriteLine("Test: " + record.Record.Text);
    Console.WriteLine("score: " + record.Score);
}

Console.ReadLine();



//var filter = new VectorSearchFilter().EqualTo(nameof(Glossary.Category), "External Definitions");
//searchResult = await collection.VectorizedSearchAsync(searchVector, new() { Top = 3, Filter = filter });
