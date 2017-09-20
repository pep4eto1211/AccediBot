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
    [Serializable]
    public class LunchDialog : IDialog<object>
    {
        #region Private members
        private string _finalResponse = string.Empty;
        #endregion

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
            var lunchMessageActivity = await result as Activity;

            try
            {
                string lunchUserCommand = lunchMessageActivity.Text.ToLower().Substring(lunchMessageActivity.Text.IndexOf(Constants.LunchCommand) + Constants.LunchCommand.Length);
                string[] lunchUserCommandParts = lunchUserCommand.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(e => e.Trim()).ToArray();
                bool operationResult = true;
                if (lunchUserCommandParts.Length >= 1)
                {
                    switch (lunchUserCommandParts[0])
                    {
                        case "at":
                            if (lunchUserCommandParts.Length >= 2)
                            {
                                operationResult = AddPlace(lunchUserCommandParts, lunchMessageActivity.From.Name);
                            }
                            else
                            {
                                this._finalResponse = "Please type a place name";
                                operationResult = false;
                            }
                            break;
                        case "count":
                            if (lunchUserCommandParts.Length >= 2)
                            {
                                operationResult = GetPlaceCount(lunchUserCommandParts[1]);
                            }
                            else
                            {
                                this._finalResponse = "Please type a place name";
                                operationResult = false;
                            }
                            break;
                        case "who":
                            if (lunchUserCommandParts.Length >= 2)
                            {
                                operationResult = GetPeopleListForPlace(lunchUserCommandParts[1]);
                            }
                            else
                            {
                                this._finalResponse = "Please type a place name";
                                operationResult = false;
                            }
                            break;
                        case "places":
                            operationResult = GetAllPlaces();
                            break;
                        default:
                            if (lunchUserCommandParts.Length >= 2)
                            {
                                if (lunchUserCommandParts[0].StartsWith("-") || lunchUserCommandParts[0].StartsWith("+"))
                                {
                                    operationResult = ModifyPeopleForPlace(lunchUserCommandParts, lunchMessageActivity.From.Name);
                                }
                                else
                                {
                                    this._finalResponse = "Please type + or - and the number of people, like this: +1 Some Place";
                                    operationResult = false;
                                }
                            }
                            else
                            {
                                this._finalResponse = "Please type a place name";
                                operationResult = false;
                            }
                            break;
                    } 
                }
                else
                {
                    this._finalResponse = "Please enter a valid command";
                    operationResult = false;
                }

                context.Done<string>(_finalResponse);
            }
            catch (Exception)
            {
                context.Done<string>("Something went wrong. Please try again!");
            }
        }
        #endregion

        #region Helper methods
        private bool ModifyPeopleForPlace(string[] lunchUserCommandParts, string username)
        {
            var lunchPlace = LunchRepository.LunchPlaces.Where(e => e.PlaceName == lunchUserCommandParts[1]).SingleOrDefault();
            int number;
            if (int.TryParse(lunchUserCommandParts[0], out number))
            {
                if ((lunchPlace != null) && ((DateTime.Now - lunchPlace.AddedDate).Hours < 12))
                {
                    lunchPlace.AddPerson(username, number);
                    if (number > 0)
                    {
                        _finalResponse = $"{ number.ToString()} people added to {lunchPlace.PlaceName}";
                    }
                    else
                    {
                        _finalResponse = $"{ Math.Abs(number).ToString()} people removed from {lunchPlace.PlaceName}";
                    }
                    return true;
                }
                else
                {
                    this._finalResponse = $"{lunchUserCommandParts[1]} hasn't been added to the places for lunch yet. You can type #lunch at {lunchUserCommandParts[1]} to add it";
                    return false;
                }
            }
            else
            {
                this._finalResponse = "Please type a number when adding or removing people";
                return false;
            }
        }

        private bool GetAllPlaces()
        {
            var placesList = LunchRepository.LunchPlaces.Where(e => ((DateTime.Now - e.AddedDate).Hours < 12)).Select(e => e.PlaceName).ToList();
            _finalResponse = "Places for lunch today are: ";
            foreach (var singlePlace in placesList)
            {
                _finalResponse += singlePlace + ", ";
            }
            return true;
        }

        private bool GetPeopleListForPlace(string placeName)
        {
            var lunchPlace = LunchRepository.LunchPlaces.Where(e => e.PlaceName == placeName).SingleOrDefault();
            if ((lunchPlace != null) && ((DateTime.Now - lunchPlace.AddedDate).Hours < 12))
            {
                _finalResponse = $"People who signed up for {lunchPlace.PlaceName} are: ";
                foreach (var singlePerson in lunchPlace.SignedUpPeople)
                {
                    _finalResponse += singlePerson + ", ";
                }
                _finalResponse = _finalResponse.Remove(_finalResponse.Length - 2);
                return true;
            }
            else
            {
                this._finalResponse = $"{placeName} hasn't been added to the places for lunch yet. You can type #lunch at {placeName} to add it";
                return false;
            }
        }

        private bool GetPlaceCount(string placeName)
        {
            var lunchPlace = LunchRepository.LunchPlaces.Where(e => e.PlaceName == placeName).SingleOrDefault();
            if ((lunchPlace != null) && ((DateTime.Now - lunchPlace.AddedDate).Hours < 12))
            {
                _finalResponse = $"People count for {lunchPlace.PlaceName} is {lunchPlace.SignedUpCount.ToString()}";
            }
            else
            {
                this._finalResponse = $"{placeName} hasn't been added to the places for lunch yet. You can type #lunch at {placeName} to add it";
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
                this._finalResponse = "The place name can't be \"count\", \"at\", \"list\", \"+\" or \"-\"";
                return false;
            }

            string placeName = lunchUserCommandParts[1];
            var lunchPlace = LunchRepository.LunchPlaces.Where(e => e.PlaceName == placeName).SingleOrDefault();
            if (lunchPlace != null)
            {
                if ((DateTime.Now - lunchPlace.AddedDate).Hours < 12)
                {
                    this._finalResponse = $"{placeName} has already been added as a lunch place today";
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

            _finalResponse = placeName + " added.";
            return true;
        } 
        #endregion
    }
}