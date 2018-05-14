using System;
using System.Collections.Generic;
using System.Text;
using ECOLOG_Mobile_App.Models;

namespace ECOLOG_Mobile_App.Calculators
{
    // クラス名間違えていたので修正
    // 参照しているクラスの移植時に注意が必要
    public static class ConvertLossCalculator
    {
        // From SensorLogInserterRe
        public static double CalcEnergy(double drivingPower, Car car, double vehicleSpeed, int efficiency)
        {
            double convertLoss;

            if (drivingPower >= 0)
            {
                convertLoss = ConsumedEnergyCalculator.CalcEnergy(drivingPower, car, vehicleSpeed, efficiency)
                                                       * ((1.0f - (efficiency + 0.0f) / 100.0f * car.InverterEfficiency));
            }
            else
            {
                convertLoss = ConsumedEnergyCalculator.CalcEnergy(drivingPower, car, vehicleSpeed, efficiency)
                    * ((1.0f / (efficiency + 0.0f) * 100.0f / car.InverterEfficiency - 1.0f));
            }
            return convertLoss;
        }

        // From TOD2017MobileApp
        public static double CalcEnergyPreVer(double drivingPower, double speed, int efficiency)
        {
            double regeneLimit = 0.15 * 9.8 * 1600;
            double convertLoss;

            if (drivingPower > 0)
            {
                convertLoss = (drivingPower / (efficiency * 0.95) * 100) - drivingPower;
            }
            else if (speed * 3.6 < 7)
            {
                convertLoss = 0;
            }
            else if (drivingPower > -regeneLimit * speed * 0.278 * 0.000001)
            {
                convertLoss = drivingPower - (drivingPower * (efficiency * 0.95) / 100);
            }
            else
            {
                convertLoss = -regeneLimit * speed * 0.278 * 0.000001
                                                      - (-regeneLimit * speed * 0.278 * 0.000001 * (efficiency * 0.95) / 100);
            }
            return convertLoss;
        }
    }
}
