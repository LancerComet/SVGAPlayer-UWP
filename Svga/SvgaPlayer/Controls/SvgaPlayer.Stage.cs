using System;
using System.Linq;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Core;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Svga.SvgaPlayer.Models;

namespace Svga.SvgaPlayer.Controls {
  public partial class SvgaPlayer {
    /// <summary>
    /// 当前已播放次数.
    /// </summary>
    private int _playedCount;

    /// <summary>
    /// 舞台是否已经初始化.
    /// </summary>
    private bool _isStageInited;

    /// <summary>
    /// 舞台资源是否准备完毕.
    /// </summary>
    private bool _isResourceReady;

    /// <summary>
    /// 舞台资源对象.
    /// </summary>
    private StageResource _stageResource;

    /// <summary>
    /// 初始化舞台资源.
    /// </summary>
    private async void InitStageResource () {
      if (this._isResourceReady) {
        return;
      }

      if (this._stageResource == null) {
        this._stageResource = new StageResource(this.Stage);
      }

      var sprites = this._sprites;
      foreach (var sprite in sprites) {
        var imageKey = sprite.ImageKey;
        var image = this._images.FirstOrDefault(item => item.Key == imageKey);

        // 有可能导出的 SVGA Image 实际不存在 PNG Binary.
        if (image.Value != null) {
          await this._stageResource.AddSprite(sprite, image.Value);
        }
      }

      this._isResourceReady = true;
    }

    /// <summary>
    /// 绘制单个 Sprite.
    /// </summary>
    /// <param name="session"></param>
    /// <param name="sprite"></param>
    private void DrawSingleSprite(CanvasDrawingSession session, SvgaSprite sprite) {
      var currentFrame = sprite.Frames[this.CurrentFrame];
      if (currentFrame == null) {
        return;
      }

      var width = 0f;
      var height = 0f;
      var alpha = currentFrame.Alpha;
      var layout = currentFrame.Layout;
      if (layout != null) {
        width = layout.Width;
        height = layout.Height;
      }

      var transform = currentFrame.Transform;
      if (transform != null) {
        // Sprite 透视参数.
        var perspective = new Matrix4x4(new Matrix3x2(
          transform.A, transform.B, transform.C, transform.D, transform.Tx, transform.Ty
        ));

        session.DrawImage(
          sprite.CanvasBitmap, 0, 0, new Rect(0, 0, width, height), alpha,
          CanvasImageInterpolation.Linear, perspective
        );
      } else {
        session.DrawImage(
          sprite.CanvasBitmap, 0, 0, new Rect(0, 0, width, height), alpha,
          CanvasImageInterpolation.Linear
        );
      }
    }

    /// <summary>
    /// Stage OnCreateResource 事件.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void StageOnCreateResources (CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args) {
      // ...
    }

    /// <summary>
    /// Stage OnUpdate 事件.
    /// 用于更新舞台数据, 一般在运行一次 Update 后运行一次 Draw 事件.
    /// 但当程序运行缓慢时, 可能会运行多次 Update 后再执行一次 Draw 事件.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void StageOnUpdate (ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args) {
      // ...
    }

    /// <summary>
    /// Stage OnDraw 事件.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void StageOnDraw (ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args) {
      if (!this.IsInPlay || !this._isResourceReady || !this._isStageInited) {
        return;
      }

      var stageResource = this._stageResource;
      var sprites = stageResource.Sprites;
      using (var session = args.DrawingSession) {
        // 遍历 Sprites 进行绘制.
        foreach (var sprite in sprites) {
          this.DrawSingleSprite(session, sprite);
        }
      }

      var nextFrame = this.CurrentFrame + 1;
      var isLoopFinished = nextFrame > this.TotalFrame - 1;
      if (isLoopFinished) {
        nextFrame = 0;
        this._playedCount++;
      }
      this.CurrentFrame = nextFrame;

      // 判断是否继续播放.
      // 此条件需要写在结尾, 否则当前帧会被清空而显示空白.
      if (this.LoopCount > 0 && this._playedCount >= this.LoopCount) {
        this.Pause();
        this.NotifyLoopFinish();
      }
    }

    /// <summary>
    /// 通知 UI 线程播放完成.
    /// </summary>
    private async void NotifyLoopFinish () {
      await this.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => {
        this.OnLoopFinish?.Invoke();
      });
    }
  }
}
