using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace MisterBot.Dialogs
{
    [Serializable]
    public class ConfusedDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var name = "User";
            context.UserData.TryGetValue<string>("Name", out name);

            await context.PostAsync($"Sorry {name}, I am confused by that...");
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = await result;
            context.Done(activity);
        }



    }
}