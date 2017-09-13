using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccediBot.Repositories
{
    public static class LunchRepository
    {
        #region Private members
        private static List<LunchPlaceData> _lunchPlaces;
        #endregion

        #region Constructor
        static LunchRepository()
        {
            _lunchPlaces = new List<LunchPlaceData>();
        }
        #endregion

        #region Public properties
        public static List<LunchPlaceData> LunchPlaces
        {
            get
            {
                return _lunchPlaces;
            }
        }
        #endregion
    }
}