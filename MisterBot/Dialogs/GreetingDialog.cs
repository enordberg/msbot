using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace MisterBot.Dialogs
{
    [Serializable]
    public class GreetingDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var welcomeDelivered = false;
            context.UserData.TryGetValue<bool>("Welcome", out welcomeDelivered);

            if (!welcomeDelivered)
            {
                await context.PostAsync("Welcome to botville!");
                context.UserData.SetValue<bool>("Welcome", true);
                await Respond(context);
            }            
            
            context.Wait(MessageReceivedAsync);
        }

        private static async Task Respond(IDialogContext context)
        {
            var username = string.Empty;
            context.UserData.TryGetValue<string>("Name", out username);

            if (string.IsNullOrEmpty(username))
            {
                await context.PostAsync("What is your name cowboy?");
                context.UserData.SetValue<bool>("GetName", true);
            }
            else
            {
                await context.PostAsync($"Hello {username}.  I am a bot.  What can I do for you?");
            }

        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = await result;
            var username = string.Empty;
            var getName = false;

            context.UserData.TryGetValue<bool>("GetName", out getName);

            if (getName)
            {
                username = activity.Text;
                context.UserData.SetValue<string>("Name", username);
                context.UserData.SetValue<bool>("GetName", false);
                await Respond(context);
            }
            
            context.Done(activity);
        }



    }
}