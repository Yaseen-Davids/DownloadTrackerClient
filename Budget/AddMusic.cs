using System;
using System.Collections.Generic;
using System.Linq;
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
    [Activity(Label = "Add new song")]
    public class AddMusic : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AddNewMusic);

            Button CancelNewMusicButton = FindViewById<Button>(Resource.Id.cancel_new_music_button);
            Button SubmitNewMusicButton = FindViewById<Button>(Resource.Id.submit_new_music_btn);

            CancelNewMusicButton.Click += delegate (object sender, EventArgs e)
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

            SubmitNewMusicButton.Click += delegate (object sender, EventArgs e)
            {
                try
                {
                    AddNewMusic();
                }
                catch (Exception submit_err)
                {
                    Console.WriteLine(submit_err.Message);
                }
            };
        }

        public async void AddNewMusic()
        {
            TextInputEditText title = FindViewById<TextInputEditText>(Resource.Id.new_music_title);
            TextInputEditText artist = FindViewById<TextInputEditText>(Resource.Id.new_music_artist);

            JObject NewMusic = new JObject
            {
                ["title"] = title.Text,
                ["artist"] = artist.Text,
            };

            try
            {
                var response = await MusicAPI.CreateMusic(NewMusic.ToString());
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