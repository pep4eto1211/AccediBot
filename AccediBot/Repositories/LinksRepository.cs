using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccediBot
{
    public static class LinksRepository
    {
        #region Private members
        private static Dictionary<string, string> _links;
        #endregion

        #region Constructor
        static LinksRepository()
        {
            _links = new Dictionary<string, string>();
            InsertLinks();
        }
        #endregion

        #region Private methods
        private static void InsertLinks()
        {
            _links.Add("timetracking", "https://www.keyedinprojects.co.uk/secure/sec_login.aspx?SiteID=ODM4214670");
        } 
        #endregion

        #region Public members
        public static Dictionary<string, string> Links
        {
            get
            {
                return _links;
            }
            set
            {
                _links = value;
            }
        }
        #endregion
    }
}