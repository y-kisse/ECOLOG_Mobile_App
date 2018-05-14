using System;
using System.Collections.Generic;
using System.Text;
using ECOLOG_Mobile_App.Calculators.Components;

namespace ECOLOG_Mobile_App.Calculators
{
    public static class ClimbingResistanceCalculator
    {
        // From SensorLogInserterRe
        //登坂抵抗力
        public static double CalcForce(double vehicleMass, double theta)
        {
            return vehicleMass * Math.Sin(theta) * Constants.GravityResistanceCoefficient;
        }

        //登坂抵抗による損失エネルギー, kWh/s
        public static double CalcPower(double vehicleMass, double theta, double vehicleSpeed)
        {
            return CalcForce(vehicleMass, theta) * vehicleSpeed / 3600 / 1000;
        }

        // From TOD2017MobileApp 
        // この辺が微妙なので、ここを呼び出している箇所はCalcPower()を呼べるか検討する必要がある。  
        // というか、このメソッド計算雑説がある。  
        // TOD2017MobileAppでは、ECOLOGCalculatorから呼ばれている。  
        public static double CalcPowerPreVer(double carWeight, double altitudeDiff)
        {
            return carWeight * 9.8 * altitudeDiff * 0.278 * 0.000001;
        }
    }
}
