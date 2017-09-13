using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using System.Threading;
using AccediBot.Helpers;

namespace AccediBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var messageActivity = await result as Activity;
            
            string messageText = messageActivity.Text;
            if (messageText.ToLower().Contains(Constants.LinkCommand))
            {
                await context.Forward(new LinksDialog(), this.ResumeAfterLinksDialog, messageActivity, CancellationToken.None);
            }
            else if (messageText.ToLower().Contains(Constants.LunchCommand))
            {
                await context.Forward(new LunchDialog(), this.ResumeAfterLunchDialog, messageActivity, CancellationToken.None);
            }
            else
            {

                // return our reply to the user
                Activity replyActivity = ((Activity)context.Activity).CreateReply();
                replyActivity.Text = $"You can type #link and the name of the service for which you want info";
                //reply.AddHeroCard("Choose option", new List<string>() { "Option 1", "Option 2", "Option 3" });
                await context.PostAsync(replyActivity);
                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterLunchDialog(IDialogContext context, IAwaitable<object> result)
        {
            //At this point, lunch dialog has finished and returned some value to use within the root dialog.
            var resultFromLinks = await result;

            await context.PostAsync(resultFromLinks.ToString());

            // Again, wait for the next message from the user.
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task ResumeAfterLinksDialog(IDialogContext context, IAwaitable<object> result)
        {
            //At this point, links dialog has finished and returned some value to use within the root dialog.
            var resultFromLinks = await result;

            await context.PostAsync($"The link you requested is: {resultFromLinks}");

            // Again, wait for the next message from the user.
            context.Wait(this.MessageReceivedAsync);
        }
    }
}