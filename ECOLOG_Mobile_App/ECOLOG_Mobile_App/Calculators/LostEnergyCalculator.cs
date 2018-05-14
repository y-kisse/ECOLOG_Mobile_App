using System;
using System.Collections.Generic;
using System.Text;
using ECOLOG_Mobile_App.Models;

namespace ECOLOG_Mobile_App.Calculators
{
    public static class LostEnergyCalculator
    {
        // From SensorLogInserterRe
        public static double CalcEnergy(double drivingPower, Car car, double vehicleSpeed, double rho, double windspeed, double myu, double theta, int efficiency)
        {
            double regeneEnergy = RegeneEnergyCalculator.CalcEnergy(drivingPower, vehicleSpeed, car, efficiency);
            return Math.Abs(ConvertLossCalculator.CalcEnergy(drivingPower, car, vehicleSpeed, efficiency))
                + Math.Abs(RegeneLossCalculator.CalcEnergy(drivingPower, regeneEnergy, car, vehicleSpeed, efficiency))
                + AirResistanceCalculator.CalcPower(rho, car.CdValue, car.FrontalProjectedArea, windspeed, vehicleSpeed)
                + RollingResistanceCalculator.CalcPower(myu, car.Weight, theta, vehicleSpeed);
        }

        public static double CalcEnergy(double convertLoss, double regeneLoss, double airResistance,
            double rollingResistance)
        {
            return Math.Abs(convertLoss) + Math.Abs(regeneLoss) + airResistance + rollingResistance;
        }
    }
}
