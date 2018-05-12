using System;
using System.Collections.Generic;
using System.Text;

namespace ECOLOG_Mobile_App.Models
{
    // From SensorLogInserterRe
    class ThreeDimensionalVector
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public ThreeDimensionalVector(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
}
