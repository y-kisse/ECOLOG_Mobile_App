using System;
using System.Collections.Generic;
using System.Text;
using Realms;
using ECOLOG_Mobile_App.Models;
using ECOLOG_Mobile_App.Utils;
using System.Linq;

namespace ECOLOG_Mobile_App.Calculators
{
    class EfficiencyCalculator
    {
        // From TOD2017MobileApp
        // Realmを使っている関係か、SensorLogInserterと大きく構造が違う
        // 確認が必要
        public static EfficiencyDatum CalcEfficiency(Car car, double speed, double torque)
        {
            double rpm = MathUtil.ConvertSpeedToRev(car, speed);

            var efficiency = Realm.GetInstance()
                .All<EfficiencyDatum>()
                .Where(v => v.Torque == (int)Math.Round(torque))
                .FirstOrDefault(v => v.Rev == (int)(Math.Round(rpm / 10)) * 10);

            if (efficiency == null)
            {
                //Debug.WriteLine("****** Efficiency is NULL *********");
                efficiency = new EfficiencyDatum { Efficiency = 70 };
            }

            return efficiency;
        }
    }
}
