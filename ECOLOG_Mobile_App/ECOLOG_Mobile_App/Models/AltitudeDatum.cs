using System;
using System.Collections.Generic;
using System.Text;
using Realms;

namespace ECOLOG_Mobile_App.Models
{
    public class AltitudeDatum: RealmObject
    {
        public double LowerLatitude { get; set; }
        public double LowerLongitude { get; set; }
        public double UpperLatitude { get; set; }
        public double UpperLongitude { get; set; }
        public double Altitude { get; set; }
    }
}
