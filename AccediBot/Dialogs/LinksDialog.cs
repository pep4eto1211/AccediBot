using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using AccediBot.Helpers;

namespace AccediBot.Dialogs
{
    [Serializable]
    public class LinksDialog : IDialog<object>
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
            var linkMessageActivity = await result as Activity;
            string linkName = linkMessageActivity.Text.Substring(linkMessageActivity.Text.IndexOf(Constants.LinkCommand) + Constants.LinkCommand.Length);
            string link = LinksRepository.Links[linkName.Trim().ToLower()];
            context.Done<string>(link);
        } 
        #endregion
    }
}