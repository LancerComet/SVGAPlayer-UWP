using System;
using System.IO;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Svga {
  public sealed partial class MainPage : Page {
    public MainPage () {
      this.InitializeComponent();
    }

    private async void InitPlayer () {
      var uri = new Uri("ms-appx:///Assets/Svga/p1.svga");
      var svgaFile = await StorageFile.GetFileFromApplicationUriAsync(uri);

      using (Stream stream = await svgaFile.OpenStreamForReadAsync()) {
        var player = this.Player;
        player.LoadSvgaFileData(stream);
        player.InitStage();
        player.Play();
      }
    }

    private void OnStartClick (object sender, RoutedEventArgs e) {
      this.InitPlayer();
    }

    private void OnPauseClick (object sender, RoutedEventArgs e) {
      var player = this.Player;
      if (player.IsInPlay) {
        player.Pause();
      } else {
        player.Play();
      }
    }
  }
}
