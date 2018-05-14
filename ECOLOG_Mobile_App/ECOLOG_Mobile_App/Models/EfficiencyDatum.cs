using System;
using System.Collections.Generic;
using System.Text;
using Realms;

namespace ECOLOG_Mobile_App.Models
{
    public class EfficiencyDatum: RealmObject
    {
        public int Torque { get; set; }
        public int Rev { get; set; }
        public int Efficiency { get; set; }
    }
}
