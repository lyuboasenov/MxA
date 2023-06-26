using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
[assembly: ExportFont("Resources.fonts.MaterialIconsRound-Regular.otf", Alias = "MD")]
[assembly: ExportFont("Resources.fonts.MaterialSymbolsRounded[FILL,GRAD,opsz,wght].ttf", Alias = "MD_new")]
[assembly: ExportFont("Resources.fonts.materialdesignicons.ttf", Alias = "MD_old")]
[assembly: Preserve(AllMembers = true)]