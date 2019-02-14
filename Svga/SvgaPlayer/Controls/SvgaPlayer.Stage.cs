using System;
using System.Linq;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Svga.SvgaPlayer.Models;

namespace Svga.SvgaPlayer.Controls {
  public partial class SvgaPlayer {
    /// <summary>
    /// 播放循环次数, 默认为 0.
    /// 当为 0 时代表无限循环播放.
    /// </summary>
    public int LoopCount { get; set; }

    /// <summary>
    /// 是否处于播放状态.
    /// </summary>
    public bool IsInPlay => this.Stage.Paused == false;

    /// <summary>
    /// 目标播放帧率.
    /// 若不设置或设置为 0 时使用默认帧率, 设置后将使用自定义帧率.
    /// </summary>
    public int Fps {
      get => this._fps;
      set {
        if (value < 0) {
          value = 0;
        }
        this._fps = value;
      }
    }
    private int _fps;

    /// <summary>
    /// 舞台是否已经初始化.
    /// </summary>
    private bool IsStageInited { get; set; }

    /// <summary>
    /// 舞台资源是否准备完毕.
    /// </summary>
    private bool IsResourceReady { get; set; }

    /// <summary>
    /// 动画总帧数.
    /// </summary>
    private int TotalFrame => this.MovieParams.Frames;

    /// <summary>
    /// 当前播放帧.
    /// </summary>
    private int CurrentFrame { get; set; }

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
      if (!this.IsInPlay || !this.IsResourceReady) {
        return;
      }

      var stageResource = this.StageResource;
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
        this.PlayedCount++;
      }
      this.CurrentFrame = nextFrame;

      // 判断是否继续播放.
      // 此条件需要写在结尾, 否则当前帧会被清空而显示空白.
      if (this.LoopCount > 0 && this.PlayedCount >= this.LoopCount) {
        this.Pause();
      }
    }

    /// <summary>
    /// 初始化 Player 舞台.
    /// 任何配置项请在调用此方法前执行.
    /// </summary>
    public void InitStage () {
      if (this.IsStageInited) {
        return;
      }

      var param = this.MovieParams;
      var stage = this.Stage;

      stage.Width = param.ViewBoxWidth;
      stage.Height = param.ViewBoxHeight;

      var fps = this.MovieParams.Fps;
      if (this.Fps > 0) {
        fps = this.Fps;
      }
      stage.TargetElapsedTime = TimeSpan.FromMilliseconds(1000d / fps);

      stage.CreateResources += this.StageOnCreateResources;
      stage.Update += this.StageOnUpdate;
      stage.Draw += this.StageOnDraw;

      this.IsStageInited = true;
    }

    /// <summary>
    /// 开始画布播放.
    /// </summary>
    public void Play () {
      this.Stage.Paused = false;
    }

    /// <summary>
    /// 暂停画布播放.
    /// </summary>
    public void Pause () {
      this.Stage.Paused = true;
    }
  }
}
