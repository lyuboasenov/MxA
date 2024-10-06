using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Svg;

namespace MxA.ViewModels {
   public class AboutViewModel : BaseViewModel {
      public ImageSource Logo { get; set; }
      public AboutViewModel() {
         Title = "About";
         OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://github.com/lyuboasenov/MxA"));
         Logo = SvgImageSource.FromSvgResource("MxA.Resources.images.logo_exported.svg", 256, 256, Color.FromHex("6002ee"));
      }

      public ICommand OpenWebCommand { get; }
   }
}