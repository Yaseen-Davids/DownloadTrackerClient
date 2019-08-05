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
    [Activity(Label = "Add new movie", Theme = "@style/AppTheme")]
    public class AddMovie : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AddNewMovie);

            Button CancelNewMovieButton = FindViewById<Button>(Resource.Id.cancel_new_movie_button);
            Button SubmitNewMovieButton = FindViewById<Button>(Resource.Id.submit_new_movie_button);

            CancelNewMovieButton.Click += delegate (object sender, EventArgs e)
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

            SubmitNewMovieButton.Click += delegate (object sender, EventArgs e)
            {
                try
                {
                    AddNewMovie();
                }
                catch (Exception submit_err)
                {
                    Console.WriteLine(submit_err.Message);
                }
            };
        }

        public async void AddNewMovie()
        {
            TextInputEditText title = FindViewById<TextInputEditText>(Resource.Id.new_movie_title);

            string URL = "https://download-tracker.herokuapp.com/movies/";

            JObject NewMovie = new JObject
            {
                ["title"] = title.Text,
            };

            using (var client = new HttpClient())
            {
                try
                {
                    var httpContent = new StringContent(NewMovie.ToString(), Encoding.UTF8, "application/json");
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