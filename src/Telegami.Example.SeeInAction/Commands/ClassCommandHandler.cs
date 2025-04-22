using Telegami.Commands;

namespace Telegami.Example.SeeInAction.Commands
{
    internal class ClassCommandHandler : ITelegamiCommandHandler
    {
        // ReSharper disable once UnusedParameter.Local
        public ClassCommandHandler(MyCustomService customService)
        {
        }
        
        public async Task HandleAsync(MessageContext ctx)
        {
            await ctx.SendAsync("Response from class command");
        }
    }
}
