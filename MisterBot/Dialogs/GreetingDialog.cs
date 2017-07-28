using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace MisterBot.Dialogs
{
    [Serializable]
    public class GreetingDialog : IDialog<object>
    {
        private bool _isCollectingName = true;

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Welcome.  I'm Mister Bot!");
            await Respond(context, true);
            context.Wait(MessageReceivedAsync);
        }

        private static async Task Respond(IDialogContext context, bool isFirstTimeThrough)
        {
            var username = string.Empty;
            context.UserData.TryGetValue<string>("Name", out username);

            if (string.IsNullOrEmpty(username))
            {
                await context.PostAsync("What is your name?");
                context.UserData.SetValue<bool>("GetName", true);
            }
            else
            {                
                await context.PostAsync($"Hello {username}!");
                if (isFirstTimeThrough)
                {
                    await context.PostAsync($"Is it nice to be back?");
                }                
            }
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = await result;
            var getName = false;
            context.UserData.TryGetValue<bool>("GetName", out getName);

            if (getName)
            {
                var username = activity.Text;
                context.UserData.SetValue<string>("Name", username);
                context.UserData.SetValue<bool>("GetName", false);
                await Respond(context, false);
            }
            else
            {
                if (activity.Text.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                    activity.Text.Equals("y", StringComparison.OrdinalIgnoreCase))
                {
                    await context.PostAsync($"Great.  It's good to have you back.");
                }
                else
                {
                    await context.PostAsync($"Huh...");
                }
            }

            context.Done(activity);
        }
    }
}