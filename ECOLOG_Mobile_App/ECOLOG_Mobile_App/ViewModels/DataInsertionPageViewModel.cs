using Prism.Commands;
using Prism.Mvvm;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Realms;
using PCLStorage;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Reactive.Bindings;
using ECOLOG_Mobile_App.Models;

namespace ECOLOG_Mobile_App.ViewModels
{
    enum DataClass
    {
        AltitudeDarum,
        GraphDatum,
        SemanticLink,
        Efficiency,
        EfficiencyMax
    }
    public class DataInsertionPageViewModel : BindableBase
	{
        private static readonly string RootFolderPath = "/sdcard/Download";
        private readonly IPageDialogService _pageDialogService;

        public DelegateCommand InsertAltitudeDetumCommand { get; set; }
        public DelegateCommand InsertGraphDatumCommand { get; set; }
        public DelegateCommand InsertEfficienctyDatumCommand { get; set; }
        public DelegateCommand InsertEfficienctyMaxDatumCommand { get; set; }
        public DelegateCommand InsertSemanticLinkCommand { get; set; }
        public DelegateCommand PreviewAltitudeDatumCommand { get; set; }
        public DelegateCommand PreviewGraphDatumCommand { get; set; }
        public DelegateCommand PreviewEfficiencyCommand { get; set; }
        public DelegateCommand PreviewEfficiencyMaxDatumCommand { get; set; }
        public DelegateCommand PreviewSemanticLinkCommand { get; set; }
        public ReactiveProperty<string> FileName { get; set; }
        public ReactiveProperty<string> FilePath { get; set; }
        public ReactiveProperty<string> FileText { get; set; }
        public DataInsertionPageViewModel(IPageDialogService pageDialogService)
        {
            _pageDialogService = pageDialogService;
            FileName = new ReactiveProperty<string>();
            FilePath = new ReactiveProperty<string>();
            FileText = new ReactiveProperty<string>();

            FileText.Value = "Count: " + Realm.GetInstance().All<EfficiencyDatum>().Count();

            InsertAltitudeDetumCommand = new DelegateCommand(async () =>
            {
                await CheckPermissionAsync();
                await InsertDataAsync(DataClass.AltitudeDarum);
            });

            InsertGraphDatumCommand = new DelegateCommand(async () =>
            {
                await CheckPermissionAsync();
                await InsertDataAsync(DataClass.GraphDatum);
            });

            InsertSemanticLinkCommand = new DelegateCommand(async () =>
            {
                await CheckPermissionAsync();
                await InsertDataAsync(DataClass.SemanticLink);
            });

            InsertEfficienctyDatumCommand = new DelegateCommand(async () =>
            {
                await CheckPermissionAsync();
                await InsertDataAsync(DataClass.Efficiency);
            });

            InsertEfficienctyMaxDatumCommand = new DelegateCommand(async () =>
            {
                await CheckPermissionAsync();
                await InsertDataAsync(DataClass.EfficiencyMax);
            });

            PreviewAltitudeDatumCommand = new DelegateCommand(async () =>
            {
                await CheckPermissionAsync();
                await PreviewDataAsync(DataClass.AltitudeDarum);
            });

            PreviewGraphDatumCommand = new DelegateCommand(async () =>
            {
                await CheckPermissionAsync();
                await PreviewDataAsync(DataClass.GraphDatum);
            });

            PreviewEfficiencyCommand = new DelegateCommand(async () =>
            {
                await CheckPermissionAsync();
                await PreviewDataAsync(DataClass.Efficiency);
            });

            PreviewEfficiencyMaxDatumCommand = new DelegateCommand(async () =>
            {
                await CheckPermissionAsync();
                await PreviewDataAsync(DataClass.EfficiencyMax);
            });
        }

        private async Task CheckPermissionAsync()
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
            if (status != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))
                {
                    await _pageDialogService.DisplayAlertAsync("Need storage accesss", "Gunna need that storage access", "OK");

                }
                await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
            }
        }

        private async Task InsertDataAsync(DataClass dataClass)
        {
            var rootFolder = await FileSystem.Current.GetFolderFromPathAsync(RootFolderPath);
            var file = await rootFolder.GetFileAsync(FileName.Value);

            FilePath.Value = file.Path;
            FileText.Value = "Start ...";

            using (var reader = new StreamReader(await file.OpenAsync(PCLStorage.FileAccess.Read)))
            {
                using (var realm = Realm.GetInstance())
                {
                    realm.Write(() =>
                    {
                        while (!reader.EndOfStream)
                        {
                            var values = reader.ReadLine().Split(',');

                            switch (dataClass)
                            {
                                case DataClass.AltitudeDarum:

                                    realm.Add(new AltitudeDatum
                                    {
                                        LowerLatitude = double.Parse(values[0]),
                                        LowerLongitude = double.Parse(values[1]),
                                        UpperLatitude = double.Parse(values[2]),
                                        UpperLongitude = double.Parse(values[3]),
                                        Altitude = double.Parse(values[4])
                                    });

                                    break;
                                case DataClass.GraphDatum:

                                    realm.Add(new GraphDatum
                                    {
                                        SemanticLinkId = int.Parse(values[0]),
                                        TripId = int.Parse(values[1]),
                                        Date = DateTimeOffset.Parse(values[2]),
                                        TransitTime = int.Parse(values[3]),
                                        ConsumedElectricEnergy = float.Parse(values[4]),
                                        LostEnergy = float.Parse(values[5]),
                                        RegeneLoss = float.Parse(values[6]),
                                        ConvertLoss = float.Parse(values[7]),
                                        RollingResistance = float.Parse(values[8]),
                                        AirResistance = float.Parse(values[9])
                                    });
                                    break;
                                case DataClass.Efficiency:

                                    realm.Add(new EfficiencyDatum
                                    {
                                        Torque = int.Parse(values[0]),
                                        Rev = int.Parse(values[1]),
                                        Efficiency = int.Parse(values[2])
                                    });

                                    break;
                                case DataClass.EfficiencyMax:

                                    realm.Add(new EfficiencyMaxDatum
                                    {
                                        Torque = int.Parse(values[0]),
                                        Rev = int.Parse(values[1]),
                                        Efficiency = int.Parse(values[2])
                                    });

                                    break;
                            }
                        }
                    });
                }
            }
            FileText.Value = "Finish!!";
        }

        private async Task PreviewDataAsync(DataClass dataClass)
        {
            var rootFolder = await FileSystem.Current.GetFolderFromPathAsync(RootFolderPath);
            IFile file;
            #region 保存先ファイル名を指定する
            switch (dataClass)
            {
                case DataClass.AltitudeDarum:
                    file = await rootFolder.CreateFileAsync("Preview_Altitude.csv", CreationCollisionOption.ReplaceExisting);
                    break;
                case DataClass.GraphDatum:
                    file = await rootFolder.CreateFileAsync("Preview_GraphDatum.csv", CreationCollisionOption.ReplaceExisting);
                    break;
                case DataClass.Efficiency:
                    file = await rootFolder.CreateFileAsync("Preview_Efficiency.csv", CreationCollisionOption.ReplaceExisting);
                    break;
                case DataClass.EfficiencyMax:
                    file = await rootFolder.CreateFileAsync("Preview_EfficiencyMax.csv", CreationCollisionOption.ReplaceExisting);
                    break;
                default:
                    file = await rootFolder.CreateFileAsync("Preview_something.csv", CreationCollisionOption.ReplaceExisting);
                    break;
            }
            #endregion

            using (var writer = new StreamWriter(await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite)))
            {
                using (var realm = Realm.GetInstance())
                {
                    writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    switch (dataClass)
                    {
                        case DataClass.AltitudeDarum:
                            var altitudeData = realm.All<AltitudeDatum>();
                            writer.WriteLine("LowerLatitude, LowerLongitude, UpperLatitude, UpperLongitude, Altitude");
                            foreach(var item in altitudeData)
                            {
                                writer.WriteLine(item.LowerLatitude + ","
                                                        + item.LowerLongitude + ","
                                                        + item.UpperLatitude + ","
                                                        + item.UpperLongitude + ","
                                                        + item.Altitude);
                            }
                            break;
                        case DataClass.GraphDatum:
                            var graphData = realm.All<GraphDatum>();
                            writer.WriteLine("SemanticLinkId, TripId, Date, TransitTime, ConsumedElectricEnergy, LostEnergy, RegeneLoss, ConvertLoss, RollingResistance, AirResistance");
                            foreach (var item in graphData)
                            {
                                writer.WriteLine(item.SemanticLinkId + ","
                                                        + item.TripId + ","
                                                        + item.Date + ","
                                                        + item.TransitTime + ","
                                                        + item.ConsumedElectricEnergy + ","
                                                        + item.LostEnergy + ","
                                                        + item.RegeneLoss + ","
                                                        + item.ConvertLoss + ","
                                                        + item.RollingResistance + ","
                                                        + item.AirResistance + ",");
                            }
                            break;
                        case DataClass.Efficiency:
                            var efficiencyData = realm.All<EfficiencyDatum>();
                            writer.WriteLine("Torque, Rev, Efficiency");
                            foreach (var item in efficiencyData)
                            {
                                writer.WriteLine(item.Torque + ","
                                                        + item.Rev + ","
                                                        + item.Efficiency);
                            }
                            break;
                        case DataClass.EfficiencyMax:
                            var efficiencyMaxData = realm.All<EfficiencyMaxDatum>();
                            writer.WriteLine("Torque, Rev, Efficiency");
                            foreach (var item in efficiencyMaxData)
                            {
                                writer.WriteLine(item.Torque + ","
                                                        + item.Rev + ","
                                                        + item.Efficiency);
                            }
                            break;
                        default:
                            
                            break;
                    }
                }
            }
        }
    }
}
