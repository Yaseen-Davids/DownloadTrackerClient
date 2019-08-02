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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Budget
{
    [Activity(Label = "Edit series", Theme = "@style/AppTheme")]
    public class EditSeries : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            SetContentView(Resource.Layout.series_edit);

            string series = Intent.GetStringExtra("Series");

            Button CancelEditSeries = FindViewById<Button>(Resource.Id.cancel_edit_button);

            CancelEditSeries.Click += delegate (object sender, EventArgs e)
            {
                Finish();
            };

            SetEdit(series);
        }

        public void SetEdit(string series)
        {
            dynamic jsonData = JsonConvert.DeserializeObject(series);

            TextInputEditText title = FindViewById<TextInputEditText>(Resource.Id.edit_series_title);
            TextInputEditText season = FindViewById<TextInputEditText>(Resource.Id.edit_series_season);
            TextInputEditText episode = FindViewById<TextInputEditText>(Resource.Id.edit_series_episode);
            Button Submit = FindViewById<Button>(Resource.Id.submit_button);
            Button delete_series_btn = FindViewById<Button>(Resource.Id.delete_series_btn);

            title.Text = jsonData.title;
            season.Text = jsonData.season;
            episode.Text = jsonData.episode;

            string id = jsonData.id.ToString();

            Submit.Click += delegate (object sender, EventArgs e)
            {
                UpdateSeries(id);
            };

            delete_series_btn.Click += delegate (object sender, EventArgs e)
            {
                ShowConfirmDelete("Confirm delete", "Are you sure you want to delete " + jsonData.title + " ?", id);
            };

        }

        public async void UpdateSeries(string id)
        {
            string URL = "https://download-tracker.herokuapp.com/series/" + id;

            TextInputEditText title = FindViewById<TextInputEditText>(Resource.Id.edit_series_title);
            TextInputEditText season = FindViewById<TextInputEditText>(Resource.Id.edit_series_season);
            TextInputEditText episode = FindViewById<TextInputEditText>(Resource.Id.edit_series_episode);
            // TextView message = FindViewById<TextView>(Resource.Id.update_msg);
            Button Submit = FindViewById<Button>(Resource.Id.submit_button);

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
                    var response = await client.PutAsync(URL, httpContent);
                    
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

        public async void DeleteSeries(string URL)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.DeleteAsync(URL);

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

        public void ShowConfirmDelete(string alerTitle, string alertMessage, string id)
        {
            Android.App.AlertDialog.Builder showDialog = new Android.App.AlertDialog.Builder(this);
            Android.App.AlertDialog Alert = showDialog.Create();
            Alert.SetTitle(alerTitle);
            Alert.SetMessage(alertMessage);
            Alert.SetButton("YES", delegate
            {
                try
                {
                    string URL = "https://download-tracker.herokuapp.com/series/" + id;
                    DeleteSeries(URL);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            });
            Alert.SetButton2("NO", delegate
            {
                Alert.Dismiss();
                Alert.Dispose();
                return;
            });
            Alert.Show();
        }

    }
}