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
                string response = string.Empty;
                response += "To get the link of an internal system you can type **#link {the name of the system}** \n\n";
                response += "To get a list of all internal systems you can type **#link all** \n\n";
                response += "To add a new place for lunch you can type **#lunch at {the name of the place}** \n\n";
                response += "To see the count of all people for a certain lunch place you can type **#lunch count {the name of the place}** \n\n";
                response += "To see a list for the people who signed up for a certain lunch place you can type **#lunch who {the name of the place}** \n\n";
                response += "To see a list of all places for lunch today you can type **#lunch places** \n\n";
                response += "To add people to a certain lunch place you can type **#lunch +{the number of people, e.g. +3} {the name of the place}** \n\n";
                response += "To remove people to a certain lunch place you can type **#lunch -{the number of people, e.g. -3} {the name of the place}** \n\n";

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