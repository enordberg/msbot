using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using MisterBot.Models;
using MisterBot.Service;

namespace MisterBot.Dialogs
{
    [Serializable]
    public class BotsDialog : IDialog<bool>
    {
        private int _botDialogQualityCounter = 0;
        private GBotRepository _repo;

        public async Task StartAsync(IDialogContext context)
        {
            context.ConversationData.RemoveValue("Bot");
            _repo = new GBotRepository();
            context.Wait(MessageReceivedAsync);
        }

        private async Task ListTheBots(IDialogContext context)
        {
            var bots = _repo.RetrieveGBots();
            var sb = new StringBuilder();
            foreach (var bot in bots)
            {
                sb.Append(bot.Name + ", ");
            }
            var botList = sb.ToString();
            botList = botList.Substring(0, botList.Length - 2);
            await context.PostAsync("We have lots of cool bots at GMI. Here are a few: " + botList + ".");
            await context.PostAsync("Which one can I tell you more about?");
        }


        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            var text = message.Text.ToLower();
            GBot bot = null;
            if (text!="y")
            {
                bot = _repo.RetrieveBotByName(text);
            }
            var botNameTyped = false;
            var wasAskedAboutBotDetail = false;
            if (bot != null)
            {
                botNameTyped = true;
                context.ConversationData.SetValue<GBot>("Bot", bot);
            }
            else
            {
                context.ConversationData.TryGetValue("Bot", out bot);
                context.ConversationData.TryGetValue("AskedForDetail", out wasAskedAboutBotDetail);
            }

            if (botNameTyped)
            {
                await context.PostAsync($"{bot.Name} {bot.Description}");
                if (bot.Url!=null)
                {
                    await context.PostAsync($"I can tell you who created it, or the URL, if you like.");
                }
                else
                {
                    await context.PostAsync($"I can tell you who created it, if you like.");
                }
                context.ConversationData.SetValue<bool>("AskedForDetail", true);
                _botDialogQualityCounter = _botDialogQualityCounter+2;
                context.Wait(MessageReceivedAsync);
            }
            else if (text.Contains("url"))
            {
                if (bot == null)
                {
                    await NullBot(context);
                }
                else
                {
                    if (bot.Url != null)
                    {
                        await context.PostAsync($"{bot.Name} can be found here: {bot.Url}");
                    }
                    else
                    {
                        await context.PostAsync($"I don't have a URL for {bot.Name} sorry.");
                    }                    
                }
                _botDialogQualityCounter = _botDialogQualityCounter + 3;
                context.Wait(MessageReceivedAsync);
            }
            else if (text.Contains("who"))
            {
                if (bot == null)
                {
                    await NullBot(context);
                }
                else
                {
                    await context.PostAsync($"{bot.Name} was created by {bot.WhoCreatedIt}.");
                }
                _botDialogQualityCounter = _botDialogQualityCounter + 3;
                context.Wait(MessageReceivedAsync);
            }
            else if ((text.Equals("y", StringComparison.OrdinalIgnoreCase) || text.Equals("yes", StringComparison.OrdinalIgnoreCase)) && wasAskedAboutBotDetail)
            {
                if (bot == null)
                {
                    await NullBot(context);
                }
                else
                {
                    if (bot.Url != null)
                    {
                        await context.PostAsync($"{bot.Name} was created by {bot.WhoCreatedIt}.");
                        await context.PostAsync($"And, it can be found here: {bot.Url}");
                    }
                    else
                    {
                        await context.PostAsync($"{bot.Name} was created by {bot.WhoCreatedIt}.");
                    }
                }
                context.ConversationData.SetValue<bool>("AskedForDetail", false);
                _botDialogQualityCounter++;
                context.Wait(MessageReceivedAsync);
            }
            else if (text.Contains("bots") || text.Contains("list"))
            {
                await ListTheBots(context);
                _botDialogQualityCounter++;
                context.Wait(MessageReceivedAsync);
            }
            else if (text.Equals("stop") || text.Equals("quit") || text.Equals("exit"))
            {
                context.Done(_botDialogQualityCounter>2);
            }
            else if (text.Equals("help"))
            {
                await context.PostAsync("Type a bot name to here more.  Or, type list for the list.  Or, exit to stop talking bots.");
                await context.PostAsync("I could talk bots all day!");
                _botDialogQualityCounter--;
                context.Wait(MessageReceivedAsync);
            }
            else
            {
                await context.PostAsync("I'm sorry.  I don't know what you mean.");
                _botDialogQualityCounter = _botDialogQualityCounter - 2;
                context.Wait(MessageReceivedAsync);
            }
            
        }

        private async Task NullBot(IDialogContext context)
        {
            await context.PostAsync(
                "Which bot do you want to know about?  Type bots if you want me to list them again.");
        }
    }
}