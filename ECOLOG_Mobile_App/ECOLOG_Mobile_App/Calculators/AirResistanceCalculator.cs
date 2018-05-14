using System;
using System.Collections.Generic;
using System.Text;

namespace ECOLOG_Mobile_App.Calculators
{
    public static class AirResistanceCalculator
    {
        //空気抵抗力
        //meter per sec
        public static double CalcForce(double rho, double Cd, double frontProjectedArea, double windSpeed)
        {
            return rho * Cd * frontProjectedArea * Math.Pow(windSpeed, 2) / 2;
        }

        //空気抵抗による損失エネルギー，kWh/s
        public static double CalcPower(double rho, double Cd, double frontProjectedArea, double windSpeed, double vehicleSpeed)
        {
            return CalcForce(rho, Cd, frontProjectedArea, windSpeed) * vehicleSpeed / 3600 / 1000;
        }
    }
}
