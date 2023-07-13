using MxA.Helpers;
using MxA.Themes;
using Plugin.SimpleAudioPlayer;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MxA.ViewModels {
   public class SettingsViewModel : BaseViewModel {

      private bool _initialized = false;

      public class PickerColor {
         public string Name { get; set; }
         public Color Color { get; set; }
      }

      public class PickerSound {
         public string Name { get; set; }
         public ISimpleAudioPlayer Sound { get; set; }
      }

      public ObservableCollection<PickerColor> Colors { get; } = new ObservableCollection<PickerColor>() {
         new PickerColor { Name = "AliceBlue", Color = Color.AliceBlue },
         new PickerColor { Name = "AntiqueWhite", Color = Color.AntiqueWhite },
         new PickerColor { Name = "Aqua", Color = Color.Aqua },
         new PickerColor { Name = "Aquamarine", Color = Color.Aquamarine },
         new PickerColor { Name = "Azure", Color = Color.Azure },
         new PickerColor { Name = "Beige", Color = Color.Beige },
         new PickerColor { Name = "Bisque", Color = Color.Bisque },
         new PickerColor { Name = "Black", Color = Color.Black },
         new PickerColor { Name = "BlanchedAlmond", Color = Color.BlanchedAlmond },
         new PickerColor { Name = "Blue", Color = Color.Blue },
         new PickerColor { Name = "BlueViolet", Color = Color.BlueViolet },
         new PickerColor { Name = "Brown", Color = Color.Brown },
         new PickerColor { Name = "BurlyWood", Color = Color.BurlyWood },
         new PickerColor { Name = "CadetBlue", Color = Color.CadetBlue },
         new PickerColor { Name = "Chartreuse", Color = Color.Chartreuse },
         new PickerColor { Name = "Chocolate", Color = Color.Chocolate },
         new PickerColor { Name = "Coral", Color = Color.Coral },
         new PickerColor { Name = "CornflowerBlue", Color = Color.CornflowerBlue },
         new PickerColor { Name = "Cornsilk", Color = Color.Cornsilk },
         new PickerColor { Name = "Crimson", Color = Color.Crimson },
         new PickerColor { Name = "Cyan", Color = Color.Cyan },
         new PickerColor { Name = "DarkBlue", Color = Color.DarkBlue },
         new PickerColor { Name = "DarkCyan", Color = Color.DarkCyan },
         new PickerColor { Name = "DarkGoldenrod", Color = Color.DarkGoldenrod },
         new PickerColor { Name = "DarkGray", Color = Color.DarkGray },
         new PickerColor { Name = "DarkGreen", Color = Color.DarkGreen },
         new PickerColor { Name = "DarkKhaki", Color = Color.DarkKhaki },
         new PickerColor { Name = "DarkMagenta", Color = Color.DarkMagenta },
         new PickerColor { Name = "DarkOliveGreen", Color = Color.DarkOliveGreen },
         new PickerColor { Name = "DarkOrange", Color = Color.DarkOrange },
         new PickerColor { Name = "DarkOrchid", Color = Color.DarkOrchid },
         new PickerColor { Name = "DarkRed", Color = Color.DarkRed },
         new PickerColor { Name = "DarkSalmon", Color = Color.DarkSalmon },
         new PickerColor { Name = "DarkSeaGreen", Color = Color.DarkSeaGreen },
         new PickerColor { Name = "DarkSlateBlue", Color = Color.DarkSlateBlue },
         new PickerColor { Name = "DarkSlateGray", Color = Color.DarkSlateGray },
         new PickerColor { Name = "DarkTurquoise", Color = Color.DarkTurquoise },
         new PickerColor { Name = "DarkViolet", Color = Color.DarkViolet },
         new PickerColor { Name = "DeepPink", Color = Color.DeepPink },
         new PickerColor { Name = "DeepSkyBlue", Color = Color.DeepSkyBlue },
         new PickerColor { Name = "DimGray", Color = Color.DimGray },
         new PickerColor { Name = "DodgerBlue", Color = Color.DodgerBlue },
         new PickerColor { Name = "Firebrick", Color = Color.Firebrick },
         new PickerColor { Name = "FloralWhite", Color = Color.FloralWhite },
         new PickerColor { Name = "ForestGreen", Color = Color.ForestGreen },
         new PickerColor { Name = "Fuchsia", Color = Color.Fuchsia },
         new PickerColor { Name = "Gainsboro", Color = Color.Gainsboro },
         new PickerColor { Name = "GhostWhite", Color = Color.GhostWhite },
         new PickerColor { Name = "Gold", Color = Color.Gold },
         new PickerColor { Name = "Goldenrod", Color = Color.Goldenrod },
         new PickerColor { Name = "Gray", Color = Color.Gray },
         new PickerColor { Name = "Green", Color = Color.Green },
         new PickerColor { Name = "GreenYellow", Color = Color.GreenYellow },
         new PickerColor { Name = "Honeydew", Color = Color.Honeydew },
         new PickerColor { Name = "HotPink", Color = Color.HotPink },
         new PickerColor { Name = "IndianRed", Color = Color.IndianRed },
         new PickerColor { Name = "Indigo", Color = Color.Indigo },
         new PickerColor { Name = "Ivory", Color = Color.Ivory },
         new PickerColor { Name = "Khaki", Color = Color.Khaki },
         new PickerColor { Name = "Lavender", Color = Color.Lavender },
         new PickerColor { Name = "LavenderBlush", Color = Color.LavenderBlush },
         new PickerColor { Name = "LawnGreen", Color = Color.LawnGreen },
         new PickerColor { Name = "LemonChiffon", Color = Color.LemonChiffon },
         new PickerColor { Name = "LightBlue", Color = Color.LightBlue },
         new PickerColor { Name = "LightCoral", Color = Color.LightCoral },
         new PickerColor { Name = "LightCyan", Color = Color.LightCyan },
         new PickerColor { Name = "LightGoldenrodYellow", Color = Color.LightGoldenrodYellow },
         new PickerColor { Name = "LightGray", Color = Color.LightGray },
         new PickerColor { Name = "LightGreen", Color = Color.LightGreen },
         new PickerColor { Name = "LightPink", Color = Color.LightPink },
         new PickerColor { Name = "LightSalmon", Color = Color.LightSalmon },
         new PickerColor { Name = "LightSeaGreen", Color = Color.LightSeaGreen },
         new PickerColor { Name = "LightSkyBlue", Color = Color.LightSkyBlue },
         new PickerColor { Name = "LightSlateGray", Color = Color.LightSlateGray },
         new PickerColor { Name = "LightSteelBlue", Color = Color.LightSteelBlue },
         new PickerColor { Name = "LightYellow", Color = Color.LightYellow },
         new PickerColor { Name = "Lime", Color = Color.Lime },
         new PickerColor { Name = "LimeGreen", Color = Color.LimeGreen },
         new PickerColor { Name = "Linen", Color = Color.Linen },
         new PickerColor { Name = "Magenta", Color = Color.Magenta },
         new PickerColor { Name = "Maroon", Color = Color.Maroon },
         new PickerColor { Name = "MediumAquamarine", Color = Color.MediumAquamarine },
         new PickerColor { Name = "MediumBlue", Color = Color.MediumBlue },
         new PickerColor { Name = "MediumOrchid", Color = Color.MediumOrchid },
         new PickerColor { Name = "MediumPurple", Color = Color.MediumPurple },
         new PickerColor { Name = "MediumSeaGreen", Color = Color.MediumSeaGreen },
         new PickerColor { Name = "MediumSlateBlue", Color = Color.MediumSlateBlue },
         new PickerColor { Name = "MediumSpringGreen", Color = Color.MediumSpringGreen },
         new PickerColor { Name = "MediumTurquoise", Color = Color.MediumTurquoise },
         new PickerColor { Name = "MediumVioletRed", Color = Color.MediumVioletRed },
         new PickerColor { Name = "MidnightBlue", Color = Color.MidnightBlue },
         new PickerColor { Name = "MintCream", Color = Color.MintCream },
         new PickerColor { Name = "MistyRose", Color = Color.MistyRose },
         new PickerColor { Name = "Moccasin", Color = Color.Moccasin },
         new PickerColor { Name = "NavajoWhite", Color = Color.NavajoWhite },
         new PickerColor { Name = "Navy", Color = Color.Navy },
         new PickerColor { Name = "OldLace", Color = Color.OldLace },
         new PickerColor { Name = "Olive", Color = Color.Olive },
         new PickerColor { Name = "OliveDrab", Color = Color.OliveDrab },
         new PickerColor { Name = "Orange", Color = Color.Orange },
         new PickerColor { Name = "OrangeRed", Color = Color.OrangeRed },
         new PickerColor { Name = "Orchid", Color = Color.Orchid },
         new PickerColor { Name = "PaleGoldenrod", Color = Color.PaleGoldenrod },
         new PickerColor { Name = "PaleGreen", Color = Color.PaleGreen },
         new PickerColor { Name = "PaleTurquoise", Color = Color.PaleTurquoise },
         new PickerColor { Name = "PaleVioletRed", Color = Color.PaleVioletRed },
         new PickerColor { Name = "PapayaWhip", Color = Color.PapayaWhip },
         new PickerColor { Name = "PeachPuff", Color = Color.PeachPuff },
         new PickerColor { Name = "Peru", Color = Color.Peru },
         new PickerColor { Name = "Pink", Color = Color.Pink },
         new PickerColor { Name = "Plum", Color = Color.Plum },
         new PickerColor { Name = "PowderBlue", Color = Color.PowderBlue },
         new PickerColor { Name = "Purple", Color = Color.Purple },
         new PickerColor { Name = "Red", Color = Color.Red },
         new PickerColor { Name = "RosyBrown", Color = Color.RosyBrown },
         new PickerColor { Name = "RoyalBlue", Color = Color.RoyalBlue },
         new PickerColor { Name = "SaddleBrown", Color = Color.SaddleBrown },
         new PickerColor { Name = "Salmon", Color = Color.Salmon },
         new PickerColor { Name = "SandyBrown", Color = Color.SandyBrown },
         new PickerColor { Name = "SeaGreen", Color = Color.SeaGreen },
         new PickerColor { Name = "SeaShell", Color = Color.SeaShell },
         new PickerColor { Name = "Sienna", Color = Color.Sienna },
         new PickerColor { Name = "Silver", Color = Color.Silver },
         new PickerColor { Name = "SkyBlue", Color = Color.SkyBlue },
         new PickerColor { Name = "SlateBlue", Color = Color.SlateBlue },
         new PickerColor { Name = "SlateGray", Color = Color.SlateGray },
         new PickerColor { Name = "Snow", Color = Color.Snow },
         new PickerColor { Name = "SpringGreen", Color = Color.SpringGreen },
         new PickerColor { Name = "SteelBlue", Color = Color.SteelBlue },
         new PickerColor { Name = "Tan", Color = Color.Tan },
         new PickerColor { Name = "Teal", Color = Color.Teal },
         new PickerColor { Name = "Thistle", Color = Color.Thistle },
         new PickerColor { Name = "Tomato", Color = Color.Tomato },
         new PickerColor { Name = "Transparent", Color = Color.Transparent },
         new PickerColor { Name = "Turquoise", Color = Color.Turquoise },
         new PickerColor { Name = "Violet", Color = Color.Violet },
         new PickerColor { Name = "Wheat", Color = Color.Wheat },
         new PickerColor { Name = "White", Color = Color.White },
         new PickerColor { Name = "WhiteSmoke", Color = Color.WhiteSmoke },
         new PickerColor { Name = "Yellow", Color = Color.Yellow },
         new PickerColor { Name = "YellowGreen", Color = Color.YellowGreen },
      };
      
      public ObservableCollection<PickerSound> Sounds { get; private set; } = new ObservableCollection<PickerSound>();

      public PickerColor PreparationColor { get; set; }
      public PickerColor WorkColor { get; set; }
      public PickerColor RepetitionRestColor { get; set; }
      public PickerColor SetRestColor { get; set; }

      public bool T0SoundEnabled { get; set; }
      public PickerSound T0Sound { get; set; }
      public bool T_1SoundEnabled { get; set; }
      public PickerSound T_1Sound { get; set; }
      public bool T_2SoundEnabled { get; set; }
      public PickerSound T_2Sound { get; set; }
      public bool T_3SoundEnabled { get; set; }
      public PickerSound T_3Sound { get; set; }
      public bool T_4SoundEnabled { get; set; }
      public PickerSound T_4Sound { get; set; }
      public bool T_5SoundEnabled { get; set; }
      public PickerSound T_5Sound { get; set; }
      public bool T_10SoundEnabled { get; set; }
      public PickerSound T_10Sound { get; set; }
      public bool T_30SoundEnabled { get; set; }
      public PickerSound T_30Sound { get; set; }
      public bool T_60SoundEnabled { get; set; }
      public PickerSound T_60Sound { get; set; }

      public ICommand PlaySoundCommand { get; set; }

      public SettingsViewModel() {
         PlaySoundCommand = new Command(async (param) => await OnPlaySoundCommand(param));
         Title = "Settings";
         Theme = Settings.Theme == 2 ? "Dark" : "Light";
         PreparationColor = Colors.FirstOrDefault(f => f.Color == Settings.PreparationColor);
         WorkColor = Colors.FirstOrDefault(f => f.Color == Settings.WorkColor);
         RepetitionRestColor = Colors.FirstOrDefault(f => f.Color == Settings.RepetitionRestColor);
         SetRestColor = Colors.FirstOrDefault(f => f.Color == Settings.SetRestColor);

         var assembly = typeof(App).GetTypeInfo().Assembly;

         foreach (var tone in Settings.ToneResources) {
            var pickerSound = new PickerSound() {
               Name = tone.Key,
               Sound = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer()
            };

            Stream stream = assembly.GetManifestResourceStream(tone.Value);
            pickerSound.Sound.Load(stream);
            Sounds.Add(pickerSound);
         }

         T0SoundEnabled = Settings.T0SoundEnabled;
         T_1SoundEnabled = Settings.T_1SoundEnabled;
         T_2SoundEnabled = Settings.T_2SoundEnabled;
         T_3SoundEnabled = Settings.T_3SoundEnabled;
         T_4SoundEnabled = Settings.T_4SoundEnabled;
         T_5SoundEnabled = Settings.T_5SoundEnabled;
         T_10SoundEnabled = Settings.T_10SoundEnabled;
         T_30SoundEnabled = Settings.T_30SoundEnabled;
         T_60SoundEnabled = Settings.T_60SoundEnabled;

         T0Sound = Sounds.FirstOrDefault(f => f.Name == Settings.T0Sound);
         T_1Sound = Sounds.FirstOrDefault(f => f.Name == Settings.T_1Sound);
         T_2Sound = Sounds.FirstOrDefault(f => f.Name == Settings.T_2Sound);
         T_3Sound = Sounds.FirstOrDefault(f => f.Name == Settings.T_3Sound);
         T_4Sound = Sounds.FirstOrDefault(f => f.Name == Settings.T_4Sound);
         T_5Sound = Sounds.FirstOrDefault(f => f.Name == Settings.T_5Sound);
         T_10Sound = Sounds.FirstOrDefault(f => f.Name == Settings.T_10Sound);
         T_30Sound = Sounds.FirstOrDefault(f => f.Name == Settings.T_30Sound);
         T_60Sound = Sounds.FirstOrDefault(f => f.Name == Settings.T_60Sound);

         _initialized = true;
      }

      private Task OnPlaySoundCommand(object param) {
         if (param is PickerSound sound) {
            sound.Sound.Play();
         }

         return Task.CompletedTask;
      }

      public string Theme { get; set; }

      public void OnThemeChanged() {
         if (Theme == "Dark") {
            Settings.Theme = 2;
         } else if (Theme == "Light") {
            Settings.Theme = 1;
         } else {
            Settings.Theme = 0;
         }
         TheTheme.SetTheme();
      }

      public void OnPreparationColorChanged() {
         if (!_initialized)
            return;
         Settings.PreparationColor = PreparationColor.Color;
      }

      public void OnWorkColorChanged() {
         if (!_initialized)
            return;
         Settings.WorkColor = WorkColor.Color;
      }

      public void OnRepetitionRestColorChanged() {
         if (!_initialized)
            return;
         Settings.RepetitionRestColor = RepetitionRestColor.Color;
      }

      public void OnSetRestColorChanged() {
         if (!_initialized)
            return;
         Settings.SetRestColor = SetRestColor.Color;
      }

      public void OnT0SoundEnabledChanged() {
         if (!_initialized)
            return;
         Settings.T0SoundEnabled = T0SoundEnabled;
      }

      public void OnT0SoundChanged() {
         if (!_initialized)
            return;
         Settings.T0Sound = T0Sound.Name;
      }

      public void OnT_1SoundEnabledChanged() {
         if (!_initialized)
            return;
         Settings.T_1SoundEnabled = T_1SoundEnabled;
      }

      public void OnT_1SoundChanged() {
         if (!_initialized)
            return;
         Settings.T_1Sound = T_1Sound.Name;
      }

      public void OnT_2SoundEnabledChanged() {
         if (!_initialized)
            return;
         Settings.T_2SoundEnabled = T_2SoundEnabled;
      }

      public void OnT_2SoundChanged() {
         if (!_initialized)
            return;
         Settings.T_2Sound = T_2Sound.Name;
      }

      public void OnT_3SoundEnabledChanged() {
         if (!_initialized)
            return;
         Settings.T_3SoundEnabled = T_3SoundEnabled;
      }

      public void OnT_3SoundChanged() {
         if (!_initialized)
            return;
         Settings.T_3Sound = T_3Sound.Name;
      }

      public void OnT_4SoundEnabledChanged() {
         if (!_initialized)
            return;
         Settings.T_4SoundEnabled = T_4SoundEnabled;
      }

      public void OnT_4SoundChanged() {
         if (!_initialized)
            return;
         Settings.T_4Sound = T_4Sound.Name;
      }

      public void OnT_5SoundEnabledChanged() {
         if (!_initialized)
            return;
         Settings.T_5SoundEnabled = T_5SoundEnabled;
      }

      public void OnT_5SoundChanged() {
         if (!_initialized)
            return;
         Settings.T_5Sound = T_5Sound.Name;
      }

      public void OnT_10SoundEnabledChanged() {
         if (!_initialized)
            return;
         Settings.T_10SoundEnabled = T_10SoundEnabled;
      }

      public void OnT_10SoundChanged() {
         if (!_initialized)
            return;
         Settings.T_10Sound = T_10Sound.Name;
      }

      public void OnT_30SoundEnabledChanged() {
         if (!_initialized)
            return;
         Settings.T_30SoundEnabled = T_30SoundEnabled;
      }

      public void OnT_30SoundChanged() {
         if (!_initialized)
            return;
         Settings.T_30Sound = T_30Sound.Name;
      }

      public void OnT_60SoundEnabledChanged() {
         if (!_initialized)
            return;
         Settings.T_60SoundEnabled = T_60SoundEnabled;
      }

      public void OnT_60SoundChanged() {
         if (!_initialized)
            return;
         Settings.T_60Sound = T_60Sound.Name;
      }
   }
}