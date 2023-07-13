using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using static MxA.ViewModels.SettingsViewModel;

namespace MxA.Helpers {
   public static class Settings {

      public static Dictionary<string, string> ToneResources { get; set; } = new Dictionary<string, string>() {
         { "Sound 1", "MxA.Resources.sounds.end_rep.mp3" },
         { "Sound 2", "MxA.Resources.sounds.countdown.wav" },
         { "Sound 3", "MxA.Resources.sounds.Tones.ogg" },
      };
      // 0 = default, 1 = light, 2 = dark
      const int theme = 0;

      public static int Theme {
         get => Preferences.Get(nameof(Theme), theme);
         set => Preferences.Set(nameof(Theme), value);
      }

      public static Color PreparationColor { 
         get => Color.FromHex(Preferences.Get(nameof(PreparationColor), Color.Orange.ToHex())); 
         set => Preferences.Set(nameof(PreparationColor), value.ToHex()); 
      }
      public static Color WorkColor {
         get => Color.FromHex(Preferences.Get(nameof(WorkColor), Color.Green.ToHex()));
         set => Preferences.Set(nameof(WorkColor), value.ToHex());
      }
      public static Color RepetitionRestColor {
         get => Color.FromHex(Preferences.Get(nameof(RepetitionRestColor), Color.Blue.ToHex()));
         set => Preferences.Set(nameof(RepetitionRestColor), value.ToHex());
      }
      public static Color SetRestColor {
         get => Color.FromHex(Preferences.Get(nameof(SetRestColor), Color.Blue.ToHex()));
         set => Preferences.Set(nameof(SetRestColor), value.ToHex());
      }
      public static bool T0SoundEnabled {
         get => Preferences.Get(nameof(T0SoundEnabled), true);
         internal set => Preferences.Set(nameof(T0SoundEnabled), value);
      }
      public static bool T_1SoundEnabled {
         get => Preferences.Get(nameof(T_1SoundEnabled), true);
         internal set => Preferences.Set(nameof(T_1SoundEnabled), value);
      }
      public static bool T_2SoundEnabled {
         get => Preferences.Get(nameof(T_2SoundEnabled), true);
         internal set => Preferences.Set(nameof(T_2SoundEnabled), value);
      }
      public static bool T_3SoundEnabled {
         get => Preferences.Get(nameof(T_3SoundEnabled), true);
         internal set => Preferences.Set(nameof(T_3SoundEnabled), value);
      }
      public static bool T_4SoundEnabled {
         get => Preferences.Get(nameof(T_4SoundEnabled), false);
         internal set => Preferences.Set(nameof(T_4SoundEnabled), value);
      }
      public static bool T_5SoundEnabled {
         get => Preferences.Get(nameof(T_5SoundEnabled), false);
         internal set => Preferences.Set(nameof(T_5SoundEnabled), value);
      }
      public static bool T_10SoundEnabled {
         get => Preferences.Get(nameof(T_10SoundEnabled), false);
         internal set => Preferences.Set(nameof(T_10SoundEnabled), value);
      }
      public static bool T_30SoundEnabled {
         get => Preferences.Get(nameof(T_30SoundEnabled), false);
         internal set => Preferences.Set(nameof(T_30SoundEnabled), value);
      }
      public static bool T_60SoundEnabled {
         get => Preferences.Get(nameof(T_60SoundEnabled), false);
         internal set => Preferences.Set(nameof(T_60SoundEnabled), value);
      }
      public static string T0Sound {
         get => Preferences.Get(nameof(T0Sound), "Sound 1");
         internal set => Preferences.Set(nameof(T0Sound), value);
      }
      public static string T_1Sound {
         get => Preferences.Get(nameof(T_1Sound), "Sound 2");
         internal set => Preferences.Set(nameof(T_1Sound), value);
      }
      public static string T_2Sound {
         get => Preferences.Get(nameof(T_2Sound), "Sound 2");
         internal set => Preferences.Set(nameof(T_2Sound), value);
      }
      public static string T_3Sound {
         get => Preferences.Get(nameof(T_3Sound), "Sound 2");
         internal set => Preferences.Set(nameof(T_3Sound), value);
      }
      public static string T_4Sound {
         get => Preferences.Get(nameof(T_4Sound), "");
         internal set => Preferences.Set(nameof(T_4Sound), value);
      }
      public static string T_5Sound {
         get => Preferences.Get(nameof(T_5Sound), "");
         internal set => Preferences.Set(nameof(T_5Sound), value);
      }
      public static string T_10Sound {
         get => Preferences.Get(nameof(T_10Sound), "");
         internal set => Preferences.Set(nameof(T_10Sound), value);
      }
      public static string T_30Sound {
         get => Preferences.Get(nameof(T_30Sound), "");
         internal set => Preferences.Set(nameof(T_30Sound), value);
      }
      public static string T_60Sound {
         get => Preferences.Get(nameof(T_60Sound), "");
         internal set => Preferences.Set(nameof(T_60Sound), value);
      }
   }
}
