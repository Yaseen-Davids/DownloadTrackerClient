using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json.Linq;

namespace Budget
{
    [Activity(Label = "Add new series", Theme = "@style/AppTheme")]
    public class AddSeries : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AddNewSeries);

            Button cancel_new_series = FindViewById<Button>(Resource.Id.cancel_new_button);
            Button submit_new_series = FindViewById<Button>(Resource.Id.submit_new_button);

            cancel_new_series.Click += delegate (object sender, EventArgs e)
            {
                try
                {
                    Finish();
                }
                catch (Exception cancel_err)
                {
                    Console.WriteLine(cancel_err.Message);
                }
            };

            submit_new_series.Click += delegate (object sender, EventArgs e)
            {
                try
                {
                    AddNewSeries();
                }
                catch (Exception submit_err)
                {
                    Console.WriteLine(submit_err.Message);
                }
            };
        }

        public async void AddNewSeries()
        {
            TextInputEditText title = FindViewById<TextInputEditText>(Resource.Id.new_series_title);
            TextInputEditText season = FindViewById<TextInputEditText>(Resource.Id.new_series_season);
            TextInputEditText episode = FindViewById<TextInputEditText>(Resource.Id.new_series_episode);

            string URL = "https://download-tracker.herokuapp.com/series/";

            JObject UpdatedSeries = new JObject
            {
                ["title"] = title.Text,
                ["season"] = season.Text,
                ["episode"] = episode.Text,
            };

            using (var client = new HttpClient())
            {
                try
                {
                    var httpContent = new StringContent(UpdatedSeries.ToString(), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(URL, httpContent);

                    if (response.StatusCode.ToString() == "OK")
                    {
                        Finish();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}