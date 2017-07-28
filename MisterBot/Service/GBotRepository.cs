using System;
using System.Collections.Generic;
using System.Linq;
using MisterBot.Models;

namespace MisterBot.Service
{
    [Serializable]
    public class GBotRepository
    {
        public IList<GBot> RetrieveGBots()
        {
            return new List<GBot>
            {
                new GBot
                {
                    Name = "CI Bot",
                    Description = "answers questions about CI.  Ask it things like... What is hard savings?",
                    WhoCreatedIt = "Interns in our Mumbai office - Contact = Rathish Pillai",
                    Url = "https://cibot.generalmills.com/"
                },
                new GBot
                {
                    Name = "Cloverleaf Bot",
                    Description = "is a bot to help answer common support questions, like... How do I decommission a site?",
                    WhoCreatedIt = "The Cloverleaf team - Contact = Mohammad Imran Ansari",
                    Url = "https://webchat.botframework.com/embed/CloverleafSupport?s=eXstShJbjzM.cwA.l84.KNvSHFbBmMf_qpIwmmKSPdf2fFidtJ2behTHmGVZA0M"
                },
                new GBot
                {
                    Name = "Middleware Slack Bot",
                    Description = "is a bot to assist with automation and support routing.  See Caleb's Code Camp presentation for more info.",
                    WhoCreatedIt = "The MiddleWare Team - Contact = Caleb Gosiak",
                    Url = null
                },
                new GBot
                {
                    Name = "Alexa - Ask Betty",
                    Description = "is an Alexa Service that exposes the Ask Betty FAQ Archive and assists with common cooking questions.",
                    WhoCreatedIt = "The PMC Team - Contact = Roman Loy",
                    Url = "Release pending - Under review by Amazon"
                }
            };
        }

        public GBot RetrieveBotByName(string botName)
        {
            var bots = RetrieveGBots();
            return bots.FirstOrDefault(bot => bot.Name.ToLower().Contains(botName));
        }
    }
}