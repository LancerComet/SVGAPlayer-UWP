using System;
using System.Linq;
using System.Net.Mime;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.DirectX;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Google.Protobuf;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace Svga.Controls {
  public partial class SvgaPlayer {
    /// <summary>
    /// 是否已初始化画布.
    /// </summary>
    private bool IsStageInited { get; set; }

    /// <summary>
    /// 是否启用绘制.
    /// </summary>
    private bool inDraw { get; set; }

    /// <summary>
    /// CanvasControl 对象.
    /// </summary>
    private CanvasAnimatedControl Stage => this.Canvas;

    /// <summary>
    /// Stage OnCreateResource 事件.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private async void StageOnCreateResources (CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args) {
      var sprite = this.Sprites.FirstOrDefault();
      if (sprite == null) {
        return;
      }

      var image = this.Images.FirstOrDefault(item => item.Key == sprite.ImageKey);
      var bytes = image.Value.ToByteArray();

      using (var stream = new InMemoryRandomAccessStream()) {
        await stream.WriteAsync(bytes.AsBuffer());
        stream.Seek(0);
        var bitmap = await CanvasBitmap.LoadAsync(sender, stream);
        this.canvasBitmap = bitmap;
      }
    }

    /// <summary>
    /// Stage OnUpdate 事件.
    /// 用于更新舞台数据, 一般在运行一次 Update 后运行一次 Draw 事件.
    /// 但当程序运行缓慢时, 可能会运行多次 Update 后再执行一次 Draw 事件.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void StageOnUpdate (ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args) {
    }

    /// <summary>
    /// Stage OnDraw 事件.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void StageOnDraw (ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args) {
      using (var session = args.DrawingSession) {
        session.DrawImage(this.canvasBitmap, new Vector2(50, 50));
        session.DrawText("LancerComet", new Vector2(0, 0), Colors.DeepSkyBlue);
      }
    }

    /// <summary>
    /// 初始化 Player 舞台,
    /// </summary>
    public void InitStage () {
      if (this.IsStageInited) {
        return;
      }

      var param = this.MovieParams;
      var stage = this.Stage;

      stage.Width = param.ViewBoxWidth;
      stage.Height = param.ViewBoxHeight;
      stage.TargetElapsedTime = TimeSpan.FromMilliseconds(1000d / this.MovieParams.Fps);

      stage.CreateResources += StageOnCreateResources;
      stage.Update += this.StageOnUpdate;
      stage.Draw += this.StageOnDraw;

      this.IsStageInited = true;
    }

    private CanvasBitmap canvasBitmap;

    /// <summary>
    /// 开始画布播放.
    /// </summary>
    public void Play () {
      this.inDraw = true;
      this.Stage.Paused = false;
    }

    /// <summary>
    /// 暂停画布播放.
    /// </summary>
    public void Pause () {
      this.inDraw = false;
      this.Stage.Paused = true;
    }
  }
}
