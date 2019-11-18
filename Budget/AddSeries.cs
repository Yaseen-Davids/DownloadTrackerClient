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
            
            JObject UpdatedSeries = new JObject
            {
                ["title"] = title.Text,
                ["season"] = season.Text,
                ["episode"] = episode.Text,
            };
            try
            {
                var response = await SeriesAPI.CreateSeries(UpdatedSeries.ToString());
                var formatted = Constants.ShowError(response["Contents"], title.Text);
                if (response["StatusCode"] == Constants.SERVER_ERROR || response["Status"] == "error")
                {
                    Constants.ShowAlert("Error", formatted["Formatted"], this);
                    return;
                    
                }
                else if (response["Status"] == "complete" && response["StatusCode"] == "OK")
                {
                    Finish();
                    Constants.ShowToast(this, "Successfully created!");
                }
            }
            catch (Exception e)
            {
                Constants.ShowAlert("Error", e.Message, this);
                return;
            }
        }
    }
}