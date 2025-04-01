using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.Text.Json;

namespace _01
{
    sealed class FunctionInvocationFilter() : IFunctionInvocationFilter
    {
        /// <summary>
        /// log the function that the model calls and what parameters it sends. 
        /// It is interesting to see what the model uses as a search query when calling the SearchPlugin.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
        {
            if (context.Function.PluginName == "SearchPlugin")
            {
                Console.WriteLine($"*******{context.Function.Name}:{JsonSerializer.Serialize(context.Arguments)}\n");
            }
            await next(context);
        }
    }
}
