using System;
using System.Collections.Generic;
using System.Text;
using ECOLOG_Mobile_App.Models;

namespace ECOLOG_Mobile_App.Calculators
{
    public static class RegeneLossCalculator
    {
        // From SensorLogInserter
        public static double CalcEnergy(double drivingPower, double regeneEnergy, Car car, double vehicleSpeed, int efficiency)
        {

            double regeneLoss;
            if (drivingPower >= 0)//力行時
            {
                regeneLoss = 0;
            }

            else
            {//回生時
                regeneLoss = -Math.Abs(drivingPower - regeneEnergy / efficiency * 100 / car.InverterEfficiency);
            }
            return regeneLoss;
        }

        // From TOD2017MobileApp
        public static double CalcEnergyPreVer(double drivingPower, double speed, int efficiency)
        {
            double regeneLimit = 0.15 * 9.8 * 1600;
            double regeneLoss;
            if (drivingPower > 0)
            {
                regeneLoss = 0;
            }
            else if (speed * 3.6 < 7)
            {
                regeneLoss = drivingPower;
            }
            else if (drivingPower > -regeneLimit * speed * 0.278 * 0.000001)
            {
                regeneLoss = 0;
            }
            else
            {
                regeneLoss = drivingPower + regeneLimit * speed * 0.278 * 0.000001;
            }

            return regeneLoss;
        }
    }
}
