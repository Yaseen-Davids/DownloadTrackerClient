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
    [Activity(Label = "Edit movie", Theme = "@style/AppTheme")]
    public class EditMovie : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.movies_edit);

            string movie = Intent.GetStringExtra("Movie");

            Button CancelEditMovie = FindViewById<Button>(Resource.Id.cancel_edit_movie_button);

            CancelEditMovie.Click += delegate (object sender, EventArgs e)
            {
                Finish();
            };

            SetMovie(movie);
        }

        public void SetMovie(string movie)
        {
            dynamic jsonData = JsonConvert.DeserializeObject(movie);

            TextInputEditText title = FindViewById<TextInputEditText>(Resource.Id.edit_series_title);
            Button Submit = FindViewById<Button>(Resource.Id.submit_edit_movie_btn);
            Button DeleteMovieBtn = FindViewById<Button>(Resource.Id.delete_movie_btn);

            title.Text = jsonData.title;

            string id = jsonData.id.ToString();

            Submit.Click += delegate (object sender, EventArgs e)
            {
                UpdateSeries(id);
            };

            DeleteMovieBtn.Click += delegate (object sender, EventArgs e)
            {
                ShowConfirmDelete("Confirm delete", "Are you sure you want to delete " + jsonData.title + " ?", id);
            };
        }

        public async void UpdateSeries(string id)
        {
            TextInputEditText title = FindViewById<TextInputEditText>(Resource.Id.edit_series_title);
            Button Submit = FindViewById<Button>(Resource.Id.submit_edit_movie_btn);

            JObject UpdatedMovie = new JObject
            {
                ["title"] = title.Text,
            };
            try
            {
                var response = await MoviesAPI.UpdateMovie(UpdatedMovie, id);
                var formatted = Constants.ShowError(response["Contents"], title.Text);
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

        public async void DeleteMovie(string id)
        {
            try
            {
                var response = await MoviesAPI.DeleteMovie(id);
                var formatted = Constants.ShowError(response["Contents"], "Movie");
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
                    DeleteMovie(id);
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