using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace AccediBot.Dialogs
{
    [Serializable]
    public class InfoDialog : IDialog<object>
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
                var infoMessageActivity = await result as Activity;
                string response = "To see everything I can do, please visit https://github.com/pep4eto1211/AccediBot/wiki";

                context.Done<string>(response);
            }
            catch (Exception)
            {
                context.Done<string>("Something went wrong. Please try again!");
            }
        }
        #endregion
    }
}