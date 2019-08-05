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
using System;
using System.Net.Http;

namespace Budget
{
    [Activity(Label = "Download Tracker", Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);

            try
            {
                OnNavigationItemSelected(navigation.Menu.GetItem(0));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            ImageButton AddSeriesBtn = FindViewById<ImageButton>(Resource.Id.add_series_btn);
            ImageButton AddMoviesBtn = FindViewById<ImageButton>(Resource.Id.add_movies_button);

            AddSeriesBtn.Click += delegate (object sender, EventArgs e)
            {
                try
                {
                    Intent AddSeriesActivity = new Intent(this, typeof(AddSeries));
                    StartActivity(AddSeriesActivity);
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                }
            };
            AddMoviesBtn.Click += delegate (object sender, EventArgs e)
            {
                try
                {
                    Intent AddMovieActivity = new Intent(this, typeof(AddMovie));
                    StartActivity(AddMovieActivity);
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                }
            };
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_series:
                    SetSeriesView();
                    return true;
                case Resource.Id.navigation_movies:
                    SetMoviesView();
                    return true;
                case Resource.Id.navigation_music:
                    Console.WriteLine("Music selected");
                    return true;
            }
            return false;
        }

        protected override void OnRestart()
        {
            base.OnRestart();

            LinearLayout loading = FindViewById<LinearLayout>(Resource.Id.loading_data);
            LinearLayout SeriesMain = FindViewById<LinearLayout>(Resource.Id.main_layout);
            ImageButton AddSeriesBtn = FindViewById<ImageButton>(Resource.Id.add_series_btn);

            loading.Visibility = ViewStates.Visible;
            SeriesMain.RemoveAllViews();
            AddSeriesBtn.Visibility = ViewStates.Gone;

            SetSeriesView();
        }

        public async void SetSeriesView()
        {
            LinearLayout MainLayout = FindViewById<LinearLayout>(Resource.Id.main_layout);
            TextView textMessage = FindViewById<TextView>(Resource.Id.message);
            LinearLayout loading = FindViewById<LinearLayout>(Resource.Id.loading_data);
            ImageButton AddSeriesBtn = FindViewById<ImageButton>(Resource.Id.add_series_btn);
            ImageButton AddMoviesBtn = FindViewById<ImageButton>(Resource.Id.add_movies_button);

            AddMoviesBtn.Visibility = ViewStates.Gone;

            if (MainLayout.ChildCount != 0)
            {
                loading.Visibility = ViewStates.Visible;
                MainLayout.RemoveAllViews();
            }

            string URL = "https://download-tracker.herokuapp.com/series/all";

            using (var client = new HttpClient())
            {
                try
                {
                    string data = await client.GetStringAsync(URL);
                    dynamic jsonData = JsonConvert.DeserializeObject(data);
                    loading.Visibility = ViewStates.Gone;

                    foreach (var series in jsonData)
                    {
                        View SeriesView = LayoutInflater.From(this).Inflate(Resource.Layout.Series_layout, null);
                        MainLayout.AddView(SeriesView);

                        TextView Title = SeriesView.FindViewById<TextView>(Resource.Id.series_name);
                        TextView Season = SeriesView.FindViewById<TextView>(Resource.Id.series_season);
                        TextView Episode = SeriesView.FindViewById<TextView>(Resource.Id.series_episode);
                        TextView EditBtn = SeriesView.FindViewById<TextView>(Resource.Id.edit_series_btn);

                        Title.Text = series.title;
                        Season.Text = "Season " + series.season;
                        Episode.Text = "Episode " + series.episode;

                        JObject SeriesObj = new JObject
                        {
                            ["id"] = series.id,
                            ["title"] = series.title,
                            ["season"] = series.season,
                            ["episode"] = series.episode
                        };

                        EditBtn.Click += delegate (object sender, EventArgs e)
                        {
                            try
                            {
                                Intent EditSeriesActivity = new Intent(this, typeof(EditSeries));
                                EditSeriesActivity.PutExtra("Series", SeriesObj.ToString());
                                StartActivity(EditSeriesActivity);
                            }
                            catch (Exception err)
                            {
                                Console.WriteLine(err.Message);
                            }
                        };

                    }
                    AddSeriesBtn.Visibility = ViewStates.Visible;

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public async void SetMoviesView()
        {
            LinearLayout MainLayout = FindViewById<LinearLayout>(Resource.Id.main_layout);
            LinearLayout loading = FindViewById<LinearLayout>(Resource.Id.loading_data);
            ImageButton AddMoviesBtn = FindViewById<ImageButton>(Resource.Id.add_movies_button);
            ImageButton AddSeriesBtn = FindViewById<ImageButton>(Resource.Id.add_series_btn);

            AddSeriesBtn.Visibility = ViewStates.Gone;

            if (MainLayout.ChildCount != 0)
            {
                loading.Visibility = ViewStates.Visible;
                MainLayout.RemoveAllViews();
            }

            string URL = "https://download-tracker.herokuapp.com/movies/all";

            using (var client = new HttpClient())
            {
                try
                {
                    string data = await client.GetStringAsync(URL);
                    dynamic jsonData = JsonConvert.DeserializeObject(data);
                    loading.Visibility = ViewStates.Gone;

                    foreach (var movie in jsonData)
                    {
                        View MoviesView = LayoutInflater.From(this).Inflate(Resource.Layout.Movies_layout, null);
                        MainLayout.AddView(MoviesView);

                        TextView Title = MoviesView.FindViewById<TextView>(Resource.Id.movies_name);
                        TextView EditBtn = MoviesView.FindViewById<TextView>(Resource.Id.edit_movies_btn);

                        Title.Text = movie.title;

                        JObject MovieObj = new JObject
                        {
                            ["id"] = movie.id,
                            ["title"] = movie.title,
                        };

                        EditBtn.Click += delegate (object sender, EventArgs e)
                        {
                            try
                            {
                                Intent EditMovieActivity = new Intent(this, typeof(EditMovie));
                                EditMovieActivity.PutExtra("Movie", MovieObj.ToString());
                                StartActivity(EditMovieActivity);
                            }
                            catch (Exception err)
                            {
                                Console.WriteLine(err.Message);
                            }
                        };

                    }
                    AddMoviesBtn.Visibility = ViewStates.Visible;

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public void HideButtons()
        {
            ImageButton AddMoviesBtn = FindViewById<ImageButton>(Resource.Id.add_movies_button);
            ImageButton AddSeriesBtn = FindViewById<ImageButton>(Resource.Id.add_series_btn);

            AddMoviesBtn.Visibility = ViewStates.Gone;
            AddSeriesBtn.Visibility = ViewStates.Gone;
        }

        public void ShowToast(string toastMsg)
        {
            Toast.MakeText(this, toastMsg, ToastLength.Short).Show();
        }
    }
}

