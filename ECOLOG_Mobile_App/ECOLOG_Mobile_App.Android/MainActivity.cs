using Android.App;
using Android.Content.PM;
using Android.OS;
using Prism;
using Prism.Ioc;
using System;
using System.IO;
using System.Threading.Tasks;
using Android.Runtime;
using Java.IO;

namespace ECOLOG_Mobile_App.Droid
{
    [Activity(Label = "ECOLOG_Mobile_App", 
                Icon = "@mipmap/ic_launcher", 
                Theme = "@style/MainTheme", 
                MainLauncher = true, 
                ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.SensorLandscape)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private static readonly string RootFolderPath = "/sdcard/Download";
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            OxyPlot.Xamarin.Forms.Platform.Android.PlotViewRenderer.Init();
            LoadApplication(new App(new AndroidInitializer()));

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var exception = e.ExceptionObject as Exception;

                var filePath = RootFolderPath + $"/Exception_{DateTime.Now.ToString("yyyyMMdd-HHmmss")}.txt";

                var fos = new FileStream(filePath, FileMode.CreateNew);
                var osw = new OutputStreamWriter(fos, "UTF-8");
                var bw = new BufferedWriter(osw);
                bw.Write(exception.Message + "\n" + exception.StackTrace);
                bw.Flush();
                bw.Close();
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                var filePath = RootFolderPath + $"/Exception_{DateTime.Now.ToString("yyyyMMdd-HHmmss")}.txt";

                var fos = new FileStream(filePath, FileMode.CreateNew);
                var osw = new OutputStreamWriter(fos, "UTF-8");
                var bw = new BufferedWriter(osw);
                bw.Write(e.Exception.Message + "\n" + e.Exception.StackTrace);
                bw.Flush();
                bw.Close();
            };

            AndroidEnvironment.UnhandledExceptionRaiser += (s, e) =>
            {
                var filePath = RootFolderPath + $"/Exception_{DateTime.Now.ToString("yyyyMMdd-HHmmss")}.txt";

                var fos = new FileStream(filePath, FileMode.CreateNew);
                var osw = new OutputStreamWriter(fos, "UTF-8");
                var bw = new BufferedWriter(osw);
                bw.Write(e.Exception.Message + "\n" + e.Exception.StackTrace);
                bw.Flush();
                bw.Close();
            };
        }
    }

    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry container)
        {
            // Register any platform specific implementations
        }
    }
}

