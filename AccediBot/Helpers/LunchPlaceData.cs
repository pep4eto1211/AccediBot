using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccediBot
{
    public class LunchPlaceData
    {
        #region Private members
        private string _placeName;
        private Dictionary<string, int> _signedUpPeople;
        private DateTime _addedDate;
        #endregion

        #region Constructor
        public LunchPlaceData(string placeName, string userName)
        {
            this._placeName = placeName;
            this._signedUpPeople = new Dictionary<string, int>();
            this._addedDate = DateTime.Now;
            this._signedUpPeople.Add(userName, 1);
        }
        #endregion

        #region Public methods
        public void AddPerson(string userName, int count)
        {
            if (this._signedUpPeople.ContainsKey(userName))
            {
                this._signedUpPeople[userName] += count;
            }
            else
            {
                this._signedUpPeople.Add(userName, count);
            }
        }
        #endregion

        #region Public properties
        public string PlaceName
        {
            get
            {
                return this._placeName;
            }
        }

        public int SignedUpCount
        {
            get
            {
                return this._signedUpPeople.Values.ToList().Sum();
            }
        }

        public List<string> SignedUpPeople
        {
            get
            {
                return this._signedUpPeople.Keys.ToList();
            }
        }

        public DateTime AddedDate
        {
            get
            {
                return this._addedDate;
            }
        }
        #endregion
    }
}