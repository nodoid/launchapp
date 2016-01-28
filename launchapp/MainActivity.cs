using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Android.Content;
using Android.Speech;
using Android.Content.PM;
using System.Linq;

namespace launchapp
{
    [Activity(Label = "launchapp", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        List<string> packages = new List<string>();
        EditText editText;
        readonly int VOICE = 0;
        Context context;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            editText = FindViewById<EditText>(Resource.Id.editText);
            var btnSpeak = FindViewById<Button>(Resource.Id.btnSpeak);

            editText.Enabled = false;
            context = editText.Context;

            btnSpeak.Click += delegate
            {
                var voiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                voiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
                voiceIntent.PutExtra(RecognizerIntent.ExtraPrompt, "Speak now");
                voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
                voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
                voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
                voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
                voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
                StartActivityForResult(voiceIntent, VOICE);
            };

            GetPackages();
        }

        protected override void OnActivityResult(int requestCode, Result resultVal, Intent data)
        {
            if (requestCode == VOICE)
            {
                if (resultVal == Result.Ok)
                {
                    var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                    var textInput = editText.Text + matches[0].ToString();
                    if (textInput.Length > 50)
                        textInput = textInput.Substring(0, 50);
                    editText.Text = textInput;
                    Launcher();
                }
            }

            base.OnActivityResult(requestCode, resultVal, data);
        }

        void GetPackages()
        {
            var pm = PackageManager;
            packages.AddRange(pm.GetInstalledPackages(PackageInfoFlags.Activities).Select(t => t.PackageName).ToList());
        }

        void Launcher()
        {
            if (!string.IsNullOrEmpty(editText.Text))
            {
                var words = editText.Text.Split(' ');
                if (words[0] != "launch")
                {
                    Toast.MakeText(context, "You need to say launch before the app name", ToastLength.Long).Show();
                    return;
                }
                foreach (var packs in packages)
                {
                    var parts = packs.Split('.').ToList();
                    foreach (var p in parts)
                        if (p.Contains(words[1]))
                        {
                            var LaunchIntent = PackageManager.GetLaunchIntentForPackage(packs);
                            StartActivity(LaunchIntent);   
                        }
                }
            }
            else
                Toast.MakeText(context, "You need to give me an command", ToastLength.Long).Show();
        }
    }
}


