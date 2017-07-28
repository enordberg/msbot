using System;

namespace MisterBot.Models
{
    [Serializable]
    public class GBot
    {

        public string Name { get; set; }

        public string Description { get; set; }

        public string WhoCreatedIt { get; set; }

        public string Url { get; set; }
    }
}