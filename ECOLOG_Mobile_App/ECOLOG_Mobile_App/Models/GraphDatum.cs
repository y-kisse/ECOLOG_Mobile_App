using System;
using System.Collections.Generic;
using System.Text;
using Realms;

namespace ECOLOG_Mobile_App.Models
{
    // From TOD2017MobileApp
    public class GraphDatum: RealmObject
    {
        public int SemanticLinkId { get; set; }
        public int TripId { get; set; }
        public DateTimeOffset Date { get; set; }
        public float ConsumedElectricEnergy { get; set; }
        public float LostEnergy { get; set; }
        public float ConvertLoss { get; set; }
        public float AirResistance { get; set; }
        public float RollingResistance { get; set; }
        public float RegeneLoss { get; set; }
        public int TransitTime { get; set; }

        // RealmDBに保存
        public void saveToDB()
        {

            var realm = Realm.GetInstance();
            realm.Write(() =>
            {
                realm.Add(this);
            });
        }
    }
}
