using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;

namespace MisterBot.Models
{
    public enum RatingOptions { Good = 1, Okay, Bad };

    [Serializable]
    public class Feedback
    {
        [Prompt("What is your {&}?")]
        public string Email { get; set; }

        [Prompt("How do you rate this bot? {||}")]
        public RatingOptions Rating { get; set; }

        [Prompt("Any {&}?")]
        public string Suggestions { get; set; }

        public static IForm<Feedback> BuildForm()
        {
            // Builds an IForm<T> based on BasicForm
            return new FormBuilder<Feedback>().Build();
        }

        public static IFormDialog<Feedback> BuildFormDialog(FormOptions options = FormOptions.PromptInStart)
        {
            // Generate a new FormDialog<T> based on IForm<BasicForm>
            return FormDialog.FromForm(BuildForm, options);
        }
    }
}