using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace MisterBot.Dialogs
{
    [Serializable]
    public class GreetingDialog : IDialog<string>
    {

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Welcome.  I'm Mister Bot!");
            context.UserData.RemoveValue("Name");
            context.Wait(RequestName);
        }

        public virtual async Task RequestName(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = await result;

            await context.PostAsync("What is your name?");
            context.Wait(MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = await result;

            var username = activity.Text;
            context.UserData.SetValue<string>("Name", username);
            await context.PostAsync($"Hello {username}!");
            context.Done(username);
        }
    }
}