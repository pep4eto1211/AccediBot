﻿using System;
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
        #region IDialog members
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }
        #endregion

        #region Message handling
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            try
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
                else if (messageText.ToLower().Contains(Constants.InfoCommand))
                {
                    await context.Forward(new InfoDialog(), this.ResumeAfterInfoDialog, messageActivity, CancellationToken.None);
                }
                else
                {
                    Activity replyActivity = ((Activity)context.Activity).CreateReply();
                    replyActivity.Text = $"You can type #info to see what I can do";
                    await context.PostAsync(replyActivity);
                    context.Wait(MessageReceivedAsync);
                }
            }
            catch (Exception e)
            {
                await context.PostAsync("Something went wrong. Please try again!");
                context.Wait(MessageReceivedAsync);
            }
        }
        #endregion

        #region Dialog resume callbacks
        private async Task ResumeAfterInfoDialog(IDialogContext context, IAwaitable<object> result)
        {
            //At this point, info dialog has finished and returned some value to use within the root dialog.
            var resultFromInfo = await result;

            await context.PostAsync(resultFromInfo.ToString());

            // Again, wait for the next message from the user.
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task ResumeAfterLunchDialog(IDialogContext context, IAwaitable<object> result)
        {
            //At this point, lunch dialog has finished and returned some value to use within the root dialog.
            var resultFromLunch = await result as Activity;

            await context.PostAsync(resultFromLunch);

            // Again, wait for the next message from the user.
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task ResumeAfterLinksDialog(IDialogContext context, IAwaitable<object> result)
        {
            //At this point, links dialog has finished and returned some value to use within the root dialog.
            var resultFromLinks = await result;

            await context.PostAsync(resultFromLinks.ToString());

            // Again, wait for the next message from the user.
            context.Wait(this.MessageReceivedAsync);
        } 
        #endregion
    }
}