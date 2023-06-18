using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.View;
using MxA.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(MxA.Droid.Environment))]

namespace MxA.Droid {
   public class Environment : IEnvironment {
      public async void SetStatusBarColor(System.Drawing.Color color, bool darkStatusBarTint) {
         if (Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.Lollipop)
            return;

         var activity = Platform.CurrentActivity;
         var window = activity.Window;

         //this may not be necessary(but may be fore older than M)
         window.AddFlags(Android.Views.WindowManagerFlags.DrawsSystemBarBackgrounds);
         window.ClearFlags(Android.Views.WindowManagerFlags.TranslucentStatus);


         if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M) {
            await Task.Delay(50);
            WindowCompat.GetInsetsController(window, window.DecorView).AppearanceLightStatusBars = darkStatusBarTint;
         }

         window.SetStatusBarColor(color.ToPlatformColor());
      }
   }
}