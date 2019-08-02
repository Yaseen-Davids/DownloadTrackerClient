using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Budget.Resources
{
    class Series
    {
        public static string URL = "https://a57a3c0f.ngrok.io/series/all";

        public static async void GetSeries()
        {
            Console.WriteLine("Getting series data...");

            using (var client = new HttpClient())
            {
                try
                {
                    string data = await client.GetStringAsync(URL);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}