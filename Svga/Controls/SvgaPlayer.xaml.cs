using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Svga.Controls {
  public partial class SvgaPlayer : UserControl {
    /// <summary>
    /// Onload handler.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnLoaded (object sender, RoutedEventArgs e) {
    }

    /// <summary>
    /// Onunload handler.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnUnloaded (object sender, RoutedEventArgs e) {
      this.Stage.CreateResources -= this.StageOnCreateResources;
      this.Stage.Update -= this.StageOnUpdate;
      this.Stage.Draw -= this.StageOnDraw;

      this.Loaded -= this.OnLoaded;
      this.Unloaded -= this.OnUnloaded;
    }

    public SvgaPlayer () {
      this.InitializeComponent();
      this.Loaded += this.OnLoaded;
      this.Unloaded += this.OnUnloaded;
    }
  }
}
