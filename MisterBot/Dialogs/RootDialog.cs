using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace MisterBot.Dialogs
{
    [Serializable]
    public class RootDialog
    {

        public static readonly IDialog<string> Dialog = Chain.PostToChain()
            .Select(msg => msg.Text)
            .Switch(
                new RegexCase<IDialog<string>>(new Regex("^hi", RegexOptions.IgnoreCase), (context, text) =>
                {
                    return Chain.ContinueWith(new GreetingDialog(), AfterGreetingContinuation);
                }),
                new RegexCase<IDialog<string>>(new Regex("^bye", RegexOptions.IgnoreCase), (context, text) =>
                {
                    return Chain.ContinueWith(new GoodbyeDialog(), AfterGoodbyeContinuation);
                }),
                new DefaultCase<string, IDialog<string>>((context, text) =>
                {
                    return Chain.ContinueWith(new ConfusedDialog(), ConfusedContinuation);
                }))
            .Unwrap()
            .PostToUser();


        private async static Task<IDialog<string>> AfterGreetingContinuation(IBotContext context, IAwaitable<object> item)
        {
            var token = await item;
            var name = "User";
            context.UserData.TryGetValue<string>("Name", out name);
            return Chain.Return(string.Empty);
        }

        private static async Task<IDialog<string>> AfterGoodbyeContinuation(IBotContext context, IAwaitable<object> item)
        {
            var token = await item;
            return Chain.Return("Please leave now.");
        }

        private static async Task<IDialog<string>> ConfusedContinuation(IBotContext context, IAwaitable<object> item)
        {
            var token = await item;
            return Chain.Return("Tell me more.");
        }
    }
}