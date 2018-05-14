using System;
using System.Collections.Generic;
using System.Text;
using ECOLOG_Mobile_App.Calculators.Components;

namespace ECOLOG_Mobile_App.Calculators
{
    // From SensorLogInserterRe
    static class RollingResistanceCalculator
    {
        public static double CalcForce(double myu, double vehicleMass, double theta)
        {
            return myu * vehicleMass * Math.Cos(theta) * Constants.GravityResistanceCoefficient;
        }

        //転がり抵抗による損失エネルギー, kWh/s
        public static double CalcPower(double myu, double vehicleMass, double theta, double vehicleSpeed)
        {
            return CalcForce(myu, vehicleMass, theta) * vehicleSpeed / 1000 / 3600;
        }
    }
}
