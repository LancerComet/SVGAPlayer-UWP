using System;
using System.IO;
using System.Numerics;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Svga.Services;

namespace Svga {
  public sealed partial class MainPage : Page {
    public MainPage () {
      this.InitializeComponent();
      this.Decode();
    }

    private void OnDraw (CanvasControl sender, CanvasDrawEventArgs args) {
      using (var session = args.DrawingSession) {
        session.DrawText("LancerComet", new Vector2(0, 0), Colors.DeepSkyBlue);
      }
    }

    private async void Decode () {
      var uri = new Uri("ms-appx:///Assets/Svga/p1.svga");
      var svgaFile = await StorageFile.GetFileFromApplicationUriAsync(uri);

      using (Stream stream = await svgaFile.OpenStreamForReadAsync()) {
        var inflatedBytes = SvgaParser.InflateSvgaFile(stream);
        var movieEntity = SvgaParser.GetMovieEntity(inflatedBytes);
      }
    }
  }
}
