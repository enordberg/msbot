using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace MisterBot.Dialogs
{
    [Serializable]
    public class ConfusedDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {            
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = await result;
            var name = "User";
            context.UserData.TryGetValue<string>("Name", out name);

            await context.PostAsync($"Sorry {name}, I am confused by that...");
            context.Done(activity);
        }



    }
}