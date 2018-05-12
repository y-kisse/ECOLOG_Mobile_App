using System;
using System.Collections.Generic;
using System.Text;

namespace ECOLOG_Mobile_App.Models
{
    // From SensorLogInserterRe
    class Quaternion
    {
        public double T { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Quaternion(double t, double x, double y, double z)
        {
            this.T = t;
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
}
