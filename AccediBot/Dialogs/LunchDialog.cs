using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using AccediBot.Helpers;
using AccediBot.Repositories;

namespace AccediBot.Dialogs
{
    public class LunchDialog : IDialog<object>
    {
        public string finalResponse = string.Empty;

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var lunchMessageActivity = await result as Activity;

            string lunchUserCommand = lunchMessageActivity.Text.Substring(lunchMessageActivity.Text.IndexOf(Constants.LunchCommand) + Constants.LunchCommand.Length);
            string[] lunchUserCommandParts = lunchUserCommand.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            bool operationResult = true;
            switch (lunchUserCommandParts[0])
            {
                case "at":
                    operationResult = AddPlace(lunchUserCommandParts, lunchMessageActivity.From.Name);
                    break;
                case "count":
                    operationResult = GetPlaceCount(lunchUserCommandParts[1]);
                    break;
                case "list":
                    operationResult = GetPlaceList(lunchUserCommandParts[1]);
                    break;
                default:
                    operationResult = false;
                    break;
            }

            context.Done<string>("");
        }

        private bool GetPlaceList(string v)
        {
            throw new NotImplementedException();
        }

        private bool GetPlaceCount(string placeName)
        {
            var lunchPlace = LunchRepository.LunchPlaces.Where(e => e.PlaceName == placeName).SingleOrDefault();
            if ((lunchPlace != null) && ((DateTime.Now - lunchPlace.AddedDate).Hours < 12))
            {
                finalResponse = $"People count for {lunchPlace.PlaceName} is {lunchPlace.SignedUpCount.ToString()}";
            }
            else
            {
                return false;
            }

            return true;
        }

        private bool AddPlace(string[] lunchUserCommandParts, string userName)
        {
            if (lunchUserCommandParts[1] == "count" || 
                lunchUserCommandParts[1] == "at" || 
                lunchUserCommandParts[1] == "list" || 
                lunchUserCommandParts[1][0] == '+' ||
                lunchUserCommandParts[1][0] == '-')
            {
                return false;
            }
            string placeName = lunchUserCommandParts[1];
            var lunchPlace = LunchRepository.LunchPlaces.Where(e => e.PlaceName == placeName).SingleOrDefault();
            if (lunchPlace != null)
            {
                if ((DateTime.Now - lunchPlace.AddedDate).Hours > 12)
                {
                    return false;
                }
                else
                {
                    LunchRepository.LunchPlaces.Remove(lunchPlace);
                    LunchRepository.LunchPlaces.Add(new LunchPlaceData(placeName, userName));
                }
            }
            else
            {
                LunchRepository.LunchPlaces.Add(new LunchPlaceData(placeName, userName));
            }

            return true;
        }
    }
}