using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace accountmanager
{
    public class Reservation
    {
        public int eventID;
        public string eventName;
        public string date;
        public string location;
        public string type;
        public string description;

        public int reservationID;
        public string email;
        public string rsvp;
        public int userID;
    }
}