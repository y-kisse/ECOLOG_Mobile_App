using System;
using System.Collections.Generic;
using System.Text;
using ECOLOG_Mobile_App.Models;

namespace ECOLOG_Mobile_App.Calculators
{
    public static class ConsumedEnergyCalculator
    {
        public static double CalcEnergy(double drivingPower, Car car, double vehicleSpeed, int efficiency)
        {
            double consumedEnergy;

            // 力行時
            if (drivingPower >= 0)
            {
                consumedEnergy = drivingPower / efficiency * 100 / car.InverterEfficiency;
            }
            // 回生時
            else
            {
                consumedEnergy = RegeneEnergyCalculator.CalcEnergy(drivingPower, vehicleSpeed, car, efficiency);
            }
            return consumedEnergy;
        }
    }
}
