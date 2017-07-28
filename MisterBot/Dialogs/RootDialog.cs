using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using MisterBot.Models;

namespace MisterBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            var text = message.Text.ToLower();
            if (text.Equals("hi") || text.Equals("hello"))
            {
                await context.Forward(new GreetingDialog(), this.AfterGreeting, message, CancellationToken.None);
            }
            else if (text.Equals("feedback") || text.Equals("rate"))
            {
                await context.PostAsync("Starting feedback form.  Type help - if needed.");
                context.Call(Feedback.BuildFormDialog(FormOptions.PromptInStart), FeedbackComplete);
            }
            else if (text.Equals("credits"))
            {
                await context.PostAsync("created for Code Camp by Eric Nordberg");
                context.Wait(MessageReceivedAsync);
            }
            else if (text.Contains("bots"))
            {
                await context.PostAsync("I'll tell you about other GMI Bots.");
                await context.Forward(new BotsDialog(), this.AfterBots, message, CancellationToken.None);
            }
            else if (text.Equals("bye"))
            {
                string username = null;
                context.UserData.TryGetValue("Name", out username);
                await context.PostAsync($"see you later {username ?? "alligator"}");
                context.Wait(MessageReceivedAsync);
            }
            else if (text.Equals("help"))
            {
                await context.PostAsync("Try the following commands... hi, feedback, list bots, credits, bye.");
                context.Wait(MessageReceivedAsync);
            }
            else
            {
                await context.PostAsync("I'm sorry.  I don't know what you mean.");
                context.Wait(MessageReceivedAsync);
            }
        }

        
        private async Task AfterGreeting(IDialogContext context, IAwaitable<string> result)
        {
            var username = await result;

            await context.PostAsync($"It is nice to meet you.");
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task AfterBots(IDialogContext context, IAwaitable<bool> result)
        {
            var botDialoagWasGood = await result;

            if (botDialoagWasGood)
            {
                await context.PostAsync($"Thanks for letting me tell you about bots.");
            }
            else
            {
                await context.PostAsync($"Sorry I wasn't able to answer your bot questions better.");
            }
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task FeedbackComplete(IDialogContext context, IAwaitable<Feedback> result)
        {
            try
            {
                var form = await result;
                if (form != null)
                {
                    await context.PostAsync("Thanks for the feedback!");
                    await context.PostAsync($"I think you are {form.Rating} too!");
                }
                else
                {
                    await context.PostAsync("We did not get that feedback.");
                }
            }
            catch (OperationCanceledException)
            {
                await context.PostAsync("Feedback cancelled.");
            }

            context.Wait(MessageReceivedAsync);
        }
    }
}