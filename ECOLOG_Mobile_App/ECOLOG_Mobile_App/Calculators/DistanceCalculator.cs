using System;
using System.Collections.Generic;
using System.Text;
using ECOLOG_Mobile_App.Calculators.Components;

namespace ECOLOG_Mobile_App.Calculators
{
    // From SensorLogInserterRe
    class DistanceCalculator
    {
        public static double CalcDistance(double latitudeFirst, double longitudeFirst, double latitudeSecond, double longitudeSecond)
        {
            //ヒュベニの公式で距離を計算
            return HubenyDistanceCalculator.CalcHubenyFormula(latitudeFirst, longitudeFirst, latitudeSecond, longitudeSecond);
        }
    }
}
