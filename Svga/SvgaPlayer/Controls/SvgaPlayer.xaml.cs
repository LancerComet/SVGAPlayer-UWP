using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Svga.Annotations;

namespace Svga.SvgaPlayer.Controls {
  public partial class SvgaPlayer : UserControl, INotifyPropertyChanged {
    /// <summary>
    /// 单词循环播放完毕事件.
    /// 只有非循环模式才进行触发.
    /// </summary>
    public delegate void OnLoopFinishHandler ();
    public event OnLoopFinishHandler OnLoopFinish;

    /// <summary>
    /// 载入 SVGA 文件数据.
    /// </summary>
    /// <param name="svgaFileBuffer">SVGA 文件二进制 Stream.</param>
    public void LoadSvgaFileData (Stream svgaFileBuffer) {
      this.InflateSvgaFile(svgaFileBuffer);
      this.InitMovieEntity();
    }

    /// <summary>
    /// 初始化 Player 舞台.
    /// 任何配置项请在调用此方法前执行.
    /// </summary>
    public void InitStage () {
      if (this._isStageInited) {
        return;
      }

      var stage = this.Stage;
      stage.Width = this.StageWidth;
      stage.Height = this.StageHeight;

      var fps = this.Fps > 0 ? this.Fps : this._movieParams.Fps;
      stage.TargetElapsedTime = TimeSpan.FromMilliseconds(1000d / fps);

      this.InitStageResource();
      this._isStageInited = true;
    }

    /// <summary>
    /// 开始画布播放.
    /// </summary>
    public void Play () {
      this.IsInPlay = true;
      this.Stage.Paused = false;
    }

    /// <summary>
    /// 暂停画布播放.
    /// </summary>
    public void Pause() {
      this.IsInPlay = false;
    }

    public void Seek (int frame) {
      if (frame < 0) {
        frame = 0;
      }

      if (frame > this.TotalFrame - 1) {
        frame = this.TotalFrame - 1;
      }

      this._drawNextFrame = frame;
    }

    /// <summary>
    /// 卸载舞台所有数据.
    /// </summary>
    public void UnloadStage () {
      this.Pause();
      this._playedCount = 0;
      this.CurrentFrame = 0;
      this._isStageInited = false;
      this._isResourceReady = false;
      this._stageResource = null;
      this._inflatedBytes = null;
      this._movieEntity = null;
    }

    public SvgaPlayer () {
      this.InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void Notify ([CallerMemberName] string propertyName = null) {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
