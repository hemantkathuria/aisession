using Microsoft.SemanticKernel;


namespace _01
{
    sealed class PromptFilterExample : IPromptRenderFilter
    {
        public async Task OnPromptRenderAsync(PromptRenderContext context, Func<PromptRenderContext, Task> next)
        {
            // Example: get function information 
            var functionName = context.Function.Name;

            await next(context);

            // Example: override rendered prompt before sending it to AI 
            //context.RenderedPrompt = "Respond with following text: Prompt from filter.";

            Console.WriteLine($"Rendered Prompt : {context.RenderedPrompt}");
            Console.WriteLine("****-------------********");
        }
    }
}
