using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Svga.SvgaPlayer.Controls {
  public partial class SvgaPlayer : UserControl {
    /// <summary>
    /// 单词循环播放完毕事件.
    /// 只有非循环模式才进行触发.
    /// </summary>
    public delegate void OnLoopFinishHandler ();
    public event OnLoopFinishHandler OnLoopFinish;

    /// <summary>
    /// Onload handler.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnLoaded (object sender, RoutedEventArgs e) {
      // ...
    }

    /// <summary>
    /// Onunload handler.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnUnloaded (object sender, RoutedEventArgs e) {
      // ...
    }

    public SvgaPlayer () {
      this.InitializeComponent();
    }
  }
}
