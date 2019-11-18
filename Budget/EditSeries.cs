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
            TextInputEditText title = FindViewById<TextInputEditText>(Resource.Id.edit_series_title);
            TextInputEditText season = FindViewById<TextInputEditText>(Resource.Id.edit_series_season);
            TextInputEditText episode = FindViewById<TextInputEditText>(Resource.Id.edit_series_episode);
            Button Submit = FindViewById<Button>(Resource.Id.submit_button);

            JObject UpdatedSeries = new JObject
            {
                ["title"] = title.Text,
                ["season"] = season.Text,
                ["episode"] = episode.Text,
            };
            try
            {
                var response = await SeriesAPI.UpdateSeries(UpdatedSeries, id);
                var formatted = Constants.ShowError(response["Contents"], "Series");
                if (response["StatusCode"] == Constants.SERVER_ERROR || response["Status"] == "error")
                {
                    Constants.ShowAlert("Error", formatted["Formatted"], this);
                    return;
                }
                else if (response["Status"] == "complete" && response["StatusCode"] == "OK")
                {
                    Finish();
                    Constants.ShowToast(this, "Successfully updated!");
                }
            }
            catch (Exception e)
            {
                Constants.ShowAlert("Error", e.Message, this);
                return;
            }
        }

        public async void DeleteSeries(string id)
        {
            try
            {
                var response = await SeriesAPI.DeleteSeries(id);
                var formatted = Constants.ShowError(response["Contents"], "Series");
                if (response["StatusCode"] == Constants.SERVER_ERROR || response["Status"] == "error")
                {
                    Constants.ShowAlert("Error", formatted["Formatted"], this);
                    return;
                }
                else if (response["Status"] == "complete" && response["StatusCode"] == "OK")
                {
                    Finish();
                    Constants.ShowToast(this, "Successfully deleted!");
                }
            }
            catch (Exception e)
            {
                Constants.ShowAlert("Error", e.Message, this);
                return;
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
                    DeleteSeries(id);
                }
                catch (Exception e)
                {
                    Constants.ShowAlert("Error", e.Message, this);
                    return;
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