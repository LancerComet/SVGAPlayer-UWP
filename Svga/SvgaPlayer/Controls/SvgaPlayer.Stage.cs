using System;
using System.Linq;
using System.Numerics;
using Windows.UI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Svga.SvgaPlayer.Models;

namespace Svga.SvgaPlayer.Controls {
  public partial class SvgaPlayer {
    private bool IsStageInited { get; set; }
    private bool IsResourceReady { get; set; }
    private bool inDraw { get; set; }

    /// <summary>
    /// CanvasControl 对象.
    /// </summary>
    private CanvasAnimatedControl Stage => this.Canvas;

    /// <summary>
    /// 舞台资源对象.
    /// </summary>
    private StageResource StageResource { get; set; }

    /// <summary>
    /// Stage OnCreateResource 事件.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private async void StageOnCreateResources (CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args) {
      if (this.IsResourceReady) {
        return;
      }

      if (this.StageResource == null) {
        this.StageResource = new StageResource(this.Stage);
      }

      var sprites = this.Sprites;
      foreach (var sprite in sprites) {
        var imageKey = sprite.ImageKey;
        var image = this.Images.FirstOrDefault(item => item.Key == imageKey);
        await this.StageResource.AddSprite(sprite, image.Value);
      }

      this.IsResourceReady = true;
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
      if (!this.inDraw || !this.IsResourceReady) {
        return;
      }

      var stageResource = this.StageResource;
      var sprites = stageResource.Sprites;
      using (var session = args.DrawingSession) {
        // 遍历 Sprites 进行绘制.
        foreach (var sprite in sprites) {
          var currentFrame = sprite.Frames[sprite.CurrentFrame];
          if (currentFrame != null) {
            var x = 0f;
            var y = 0f;
            var transform = currentFrame.Transform;
            if (transform != null) {
              x = transform.Tx;
              y = transform.Ty;
            }
            session.DrawImage(sprite.CanvasBitmap, new Vector2(x, y));
            sprite.CurrentFrame++;
          }
        }

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

      stage.CreateResources += this.StageOnCreateResources;
      stage.Update += this.StageOnUpdate;
      stage.Draw += this.StageOnDraw;

      this.IsStageInited = true;
    }

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
