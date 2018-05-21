using Prism;
using Prism.Ioc;
using ECOLOG_Mobile_App.ViewModels;
using ECOLOG_Mobile_App.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Prism.Unity;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace ECOLOG_Mobile_App
{
    public partial class App : PrismApplication
    {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        public static string AppStatus;
        public static IList<IDisposable> EventList = new List<IDisposable>();
        public static Position CurrentPosition;

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync("NavigationPage/MainPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage>();
            containerRegistry.RegisterForNavigation<DataInsertionPage>();
            containerRegistry.RegisterForNavigation<EnergyStackPage>();
            containerRegistry.RegisterForNavigation<ECGsPage>();
            containerRegistry.RegisterForNavigation<ResultPage>();
            containerRegistry.RegisterForNavigation<MapPage>();
            containerRegistry.RegisterForNavigation<ECGsDemoPage>();
            containerRegistry.RegisterForNavigation<TestPlotPage>();
            containerRegistry.RegisterForNavigation<RealmObjectPage>();
        }
    }
}
