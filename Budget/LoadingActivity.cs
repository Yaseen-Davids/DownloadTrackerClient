using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Budget
{
    [Activity(Label = "@string/app_name", MainLauncher = true, NoHistory = true)]
    public class LoadingActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.loading_screen);
            Console.WriteLine("Starting up...");
        }

        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(() => { SimulateStartUp(); });
            startupWork.Start();
        }

        async void SimulateStartUp()
        {
            await Task.Delay(1000);
            Console.WriteLine("Startup complete");
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }
    }
}