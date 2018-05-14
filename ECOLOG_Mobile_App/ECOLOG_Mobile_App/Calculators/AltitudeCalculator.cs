using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Realms;
using ECOLOG_Mobile_App.Models;

namespace ECOLOG_Mobile_App.Calculators
{
    class AltitudeCalculator
    {
        // TODO: DBへのアクセスなので、Calculatorに書くのは気持ち悪い
        public static AltitudeDatum CalcAltitude(double latitude, double longitude)
        {
            return Realm.GetInstance()
                .All<AltitudeDatum>()
                .FirstOrDefault(row => row.LowerLatitude <= latitude
                                       && row.UpperLatitude > latitude
                                       && row.LowerLongitude <= longitude
                                       && row.UpperLongitude > longitude);
        }
    }
}
