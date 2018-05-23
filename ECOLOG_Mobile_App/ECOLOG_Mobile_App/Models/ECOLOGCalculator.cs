using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Plugin.Geolocator.Abstractions;
using System.Text;
using ECOLOG_Mobile_App.Calculators;
using ECOLOG_Mobile_App.Calculators.Components;
using Xamarin.Forms;
using System.Linq;

namespace ECOLOG_Mobile_App.Models
{
    // TOD2017MobileAppから移植
    // ECOLOG計算で、問題があったとのことなので、この辺は念入りに調査をする必要がある。
    public class ECOLOGCalculator
    {
        public ObservableCollection<Position> PositionCollection { get; set; }
        public IList<double> LostEnergyList { get; set; }
        public IList<double> AirResistanceList { get; set; }
        public IList<double> RollingResistanceList { get; set; }
        public IList<double> ConvertLossList { get; set; }
        public IList<double> RegeneLossList { get; set; }

        private AltitudeDatum _altitudeBefore;
        private double _speedBefore;

        public void Init()
        {
            PositionCollection = new ObservableCollection<Position>();
            PositionCollection.CollectionChanged += (sender, args) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    CalcEcolog(PositionCollection.Count);
                });
            };
            LostEnergyList = new List<double>();
            AirResistanceList = new List<double>();
            RollingResistanceList = new List<double>();
            ConvertLossList = new List<double>();
            RegeneLossList = new List<double>();
        }

        public GraphDatum GetGraphDatum()
        {
            return new GraphDatum
            {
                TripId = -1,
                Date = new DateTimeOffset(DateTime.Now),
                LostEnergy = (float)LostEnergyList.Sum(),
                ConvertLoss = (float)ConvertLossList.Sum(),
                AirResistance = (float)AirResistanceList.Sum(),
                RollingResistance = (float)RollingResistanceList.Sum(),
                RegeneLoss = (float)RegeneLossList.Sum(),
                TransitTime  = (int)(PositionCollection.Last().Timestamp -
                                            PositionCollection.First().Timestamp).TotalSeconds,
        };
        }

        public static double CalcLostEnergy(IList<Position> positions)
        {
            Debug.WriteLine("List Count: " + positions.Count);

            AltitudeDatum altitudeBefore;
            double speedBefore = 0;

            double airSum = 0;
            double rollingSum = 0;
            double convertLossSum = 0;
            double regeneLossSum = 0;
            double lostEnergy = 0;

            altitudeBefore = AltitudeCalculator.CalcAltitude(positions[1].Latitude, positions[1].Longitude);

            for (int i = 3; i < positions.Count; i++)
            {
                var positionBefore = positions[i - 3];
                var positionCurrent = positions[i - 2];
                var positionAfter = positions[i - 1];

                var distanceDiff = DistanceCalculator.CalcDistance(positionBefore.Latitude,
                    positionBefore.Longitude,
                    positionCurrent.Latitude,
                    positionCurrent.Longitude);
                //Debug.WriteLine("DistanceDiff: " + distanceDiff);

                // meter per sec
                var speed = SpeedCalculator.CalcSpeed(positionBefore.Latitude,
                    positionBefore.Longitude,
                    positionBefore.Timestamp.DateTime,
                    positionAfter.Latitude,
                    positionAfter.Longitude,
                    positionAfter.Timestamp.DateTime,
                    positionCurrent.Latitude,
                    positionCurrent.Longitude) / 3.6;
                //Debug.WriteLine("Speed: " + speed * 3.6);

                if (i == 3)
                    speedBefore = speed;

                var altitude = AltitudeCalculator.CalcAltitude(positionCurrent.Latitude, positionCurrent.Longitude);
                double altitudeDiff = 0;
                if (altitude != null && altitudeBefore != null)
                {
                    altitudeDiff = altitude.Altitude - altitudeBefore.Altitude;
                }
                altitudeBefore = altitude;
                //Debug.WriteLine("AltitudeDiff: " + altitudeDiff);

                double airResistancePower = 0;
                if (speed > 1.0 / 3.6 && distanceDiff > 0)
                    airResistancePower = AirResistanceCalculator.CalcPower(
                        Constants.Rho, Car.GetLeaf().CdValue, Car.GetLeaf().FrontalProjectedArea, speed, speed);
                //Debug.WriteLine("AirResistace: " + airResistancePower);
                airSum += airResistancePower;

                double rollingResistancePower = 0;
                if (speed > 1.0 / 3.6 && distanceDiff > 0)
                    rollingResistancePower = RollingResistanceCalculator.CalcPower(
                        Constants.Myu, Car.GetLeaf().Weight, Math.Atan(altitudeDiff / distanceDiff), speed);
                //Debug.WriteLine("rollingResistancePower: " + rollingResistancePower);
                rollingSum += rollingResistancePower;

                double climbingResistancePower = 0;
                if (speed > 1.0 / 3.6 && distanceDiff > 0)
                    climbingResistancePower = ClimbingResistanceCalculator.CalcPowerPreVer(
                        Car.GetLeaf().Weight, altitudeDiff);
                //Debug.WriteLine("climbingResistancePower: " + climbingResistancePower);

                double accResistancePower = 0;
                if (speed > 1.0 / 3.6 && distanceDiff > 0)
                    accResistancePower = AccResistanceCalculator.CalcPower(
                        speedBefore,
                        positionBefore.Timestamp.DateTime,
                        speed,
                        positionCurrent.Timestamp.DateTime,
                        Car.GetLeaf().Weight);
                //Debug.WriteLine("accResistancePower: " + accResistancePower);

                double drivingResistancePower =
                    airResistancePower + rollingResistancePower + climbingResistancePower + accResistancePower;

                double torque = 0;
                if (drivingResistancePower > 0 && speed > 0)
                    torque = drivingResistancePower * 1000 * 3600 / speed * Car.GetLeaf().TireRadius /
                             Car.GetLeaf().ReductionRatio;
                //Debug.WriteLine("torque: " + torque);

                var efficiency = EfficiencyCalculator.CalcEfficiency(Car.GetLeaf(), speed, torque).Efficiency;
                //Debug.WriteLine("efficiency: " + efficiency);

                double convertLoss = ConvertLossCalculator.CalcEnergyPreVer(
                    drivingResistancePower, speed, efficiency);
                //Debug.WriteLine("convertLoss: " + convertLoss);
                convertLossSum += Math.Abs(convertLoss);

                double regeneEnergy = RegeneEnergyCalculator.CalcEnergy(drivingResistancePower,
                    speed, Car.GetLeaf(), efficiency);
                //Debug.WriteLine("regeneEnergy: " + regeneEnergy);

                double regeneLoss = RegeneLossCalculator.CalcEnergyPreVer(drivingResistancePower, speed, efficiency);
                //Debug.WriteLine($"{positionCurrent.Timestamp.DateTime}: {regeneLoss}, {efficiency}");
                regeneLossSum += Math.Abs(regeneLoss);

                lostEnergy += LostEnergyCalculator.CalcEnergy(convertLoss, regeneLoss, airResistancePower,
                    rollingResistancePower);
                //Debug.WriteLine("LostEnergy: " + lostEnergy);

                speedBefore = speed;

                //var consumedEnergy = ConsumedEnergyCaluculator.CalcEnergy(drivingResistancePower, Car.GetLeaf(), speed, efficiency);
                //Debug.WriteLine($"Efficiency: {efficiency}, CalcEfficiency: {(consumedEnergy / convertLoss) * 100}");
                //LostEnergyList.Add(lostEnergy);
            }

            Debug.WriteLine("LostEnergy: " + lostEnergy);
            Debug.WriteLine("Air: " + airSum);
            Debug.WriteLine("Rolling: " + rollingSum);
            Debug.WriteLine("Convert: " + convertLossSum);
            Debug.WriteLine("Regene: " + regeneLossSum);

            return lostEnergy;
        }

        private void CalcEcolog(int count)
        {
            if (count == 2)
            {
                _altitudeBefore = AltitudeCalculator.CalcAltitude(PositionCollection[count - 1].Latitude, PositionCollection[count - 1].Longitude);
                //Debug.WriteLine("AltitudeBefore: " + _altitudeBefore.Altitude);
            }
            else if (PositionCollection.Count > 2)
            {
                var positionBefore = PositionCollection[count - 3];
                var positionCurrent = PositionCollection[count - 2];
                var positionAfter = PositionCollection[count - 1];

                var distanceDiff = DistanceCalculator.CalcDistance(positionBefore.Latitude,
                    positionBefore.Longitude,
                    positionCurrent.Latitude,
                    positionCurrent.Longitude);
                //Debug.WriteLine("DistanceDiff: " + distanceDiff);

                // meter per sec
                var speed = SpeedCalculator.CalcSpeed(positionBefore.Latitude,
                    positionBefore.Longitude,
                    positionBefore.Timestamp.DateTime,
                    positionAfter.Latitude,
                    positionAfter.Longitude,
                    positionAfter.Timestamp.DateTime,
                    positionCurrent.Latitude,
                    positionCurrent.Longitude) / 3.6;
                //Debug.WriteLine("Speed: " + speed * 3.6);

                if (count == 3)
                    _speedBefore = speed;

                var altitude = AltitudeCalculator.CalcAltitude(positionCurrent.Latitude, positionCurrent.Longitude);
                //var altitude = new AltitudeDatum { Altitude = 40 };
                double altitudeDiff = 0;
                if (altitude != null && _altitudeBefore != null)
                {
                    altitudeDiff = altitude.Altitude - _altitudeBefore.Altitude;
                }
                _altitudeBefore = altitude;
                //Debug.WriteLine("AltitudeDiff: " + altitudeDiff);

                double airResistancePower = 0;
                if (speed > 1.0 / 3.6 && distanceDiff > 0)
                    airResistancePower = AirResistanceCalculator.CalcPower(
                        Constants.Rho, Car.GetLeaf().CdValue, Car.GetLeaf().FrontalProjectedArea, speed, speed);
                //Debug.WriteLine("AirResistace: " + airResistancePower);

                double rollingResistancePower = 0;
                if (speed > 1.0 / 3.6 && distanceDiff > 0)
                    rollingResistancePower = RollingResistanceCalculator.CalcPower(
                        Constants.Myu, Car.GetLeaf().Weight, Math.Atan(altitudeDiff / distanceDiff), speed);
                //Debug.WriteLine("rollingResistancePower: " + rollingResistancePower);

                double climbingResistancePower = 0;
                if (speed > 1.0 / 3.6 && distanceDiff > 0)
                    climbingResistancePower = ClimbingResistanceCalculator.CalcPowerPreVer(
                        Car.GetLeaf().Weight, altitudeDiff);
                //Debug.WriteLine("climbingResistancePower: " + climbingResistancePower);

                double accResistancePower = 0;
                if (speed > 1.0 / 3.6 && distanceDiff > 0)
                    accResistancePower = AccResistanceCalculator.CalcPower(
                        _speedBefore,
                        positionBefore.Timestamp.DateTime,
                        speed,
                        positionCurrent.Timestamp.DateTime,
                        Car.GetLeaf().Weight);
                //Debug.WriteLine("accResistancePower: " + accResistancePower);

                double drivingResistancePower =
                    airResistancePower + rollingResistancePower + climbingResistancePower + accResistancePower;

                double torque = 0;
                if (drivingResistancePower > 0 && speed > 0)
                    torque = drivingResistancePower * 1000 * 3600 / speed * Car.GetLeaf().TireRadius /
                             Car.GetLeaf().ReductionRatio;
                //Debug.WriteLine("torque: " + torque);

                var efficiency = EfficiencyCalculator.CalcEfficiency(Car.GetLeaf(), speed, torque).Efficiency;
                //var efficiency = 90;
                //Debug.WriteLine("efficiency: " + efficiency);

                double convertLoss = ConvertLossCalculator.CalcEnergyPreVer(
                    drivingResistancePower, speed, efficiency);
                //Debug.WriteLine("convertLoss: " + convertLoss);

                double regeneEnergy = RegeneEnergyCalculator.CalcEnergy(drivingResistancePower,
                    speed, Car.GetLeaf(), efficiency);
                //Debug.WriteLine("regeneEnergy: " + regeneEnergy);

                double regeneLoss = RegeneLossCalculator.CalcEnergyPreVer(drivingResistancePower, speed, efficiency);
                //Debug.WriteLine($"{positionCurrent.Timestamp.DateTime}: {regeneLoss}, {efficiency}");

                double lostEnergy = LostEnergyCalculator.CalcEnergy(convertLoss, regeneLoss, airResistancePower,
                    rollingResistancePower);
                //Debug.WriteLine("LostEnergy: " + lostEnergy);

                _speedBefore = speed;

                //var consumedEnergy = ConsumedEnergyCaluculator.CalcEnergy(drivingResistancePower, Car.GetLeaf(), speed, efficiency);

                //Debug.WriteLine($"Efficiency: {efficiency}, CalcEfficiency: {(consumedEnergy / convertLoss) * 100}");

                LostEnergyList.Add(lostEnergy);
                AirResistanceList.Add(airResistancePower);
                RollingResistanceList.Add(rollingResistancePower);
                ConvertLossList.Add(Math.Abs(convertLoss));
                RegeneLossList.Add(Math.Abs(regeneLoss));
            }
        }
    }
}
