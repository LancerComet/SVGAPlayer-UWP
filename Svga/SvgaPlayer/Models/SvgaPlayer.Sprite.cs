using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Com.Opensource.Svga;
using Google.Protobuf;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace Svga.SvgaPlayer.Models {
  /// <summary>
  /// 代表 SVGA 的一个 Sprite.
  /// </summary>
  public class SvgaSprite {
    /// <summary>
    /// 原始 Entity 引用对象.
    /// </summary>
    private SpriteEntity SpriteEntity { get; }

    /// <summary>
    /// 原始素材的 ByteString.
    /// </summary>
    private ByteString ImageByteString { get; }

    /// <summary>
    /// 舞台引用.
    /// </summary>
    private CanvasAnimatedControl Stage { get; }

    /// <summary>
    /// Sprite 的 CanvasBitmap.
    /// 用于在 CanvasControl 中进行绘制.
    /// </summary>
    public CanvasBitmap CanvasBitmap { get; private set; }

    /// <summary>
    /// 包含了 Sprite 中的所有 Frame.
    /// </summary>
    public readonly List<FrameEntity> Frames = new List<FrameEntity>();

    /// <summary>
    /// 总 Frame 数量.
    /// </summary>
    public int TotalFrames => this.Frames.Count;

    /// <summary>
    /// 初始化 CanvasBitmap.
    /// </summary>
    private async Task InitCanvasBitmap () {
      using (var stream = new InMemoryRandomAccessStream()) {
        var buffer = this.ImageByteString.ToByteArray().AsBuffer();
        await stream.WriteAsync(buffer);
        stream.Seek(0);
        var bitmap = await CanvasBitmap.LoadAsync(this.Stage, stream);
        this.CanvasBitmap = bitmap;
      }
    }

    /// <summary>
    /// 初始化 Frames.
    /// </summary>
    private void InitFrames () {
      var frames = this.SpriteEntity.Frames;
      foreach (var frameEntity in frames) {
        this.Frames.Add(frameEntity);
      }
    }

    /// <summary>
    /// Sprite 初始化方法.
    /// 请在外部 await 方式调取.
    /// </summary>
    /// <returns></returns>
    public async Task Init () {
      this.InitFrames();
      await this.InitCanvasBitmap();
    }

    public SvgaSprite (CanvasAnimatedControl stage, SpriteEntity entity, ByteString imageByteString) {
      this.Stage = stage;
      this.SpriteEntity = entity;
      this.ImageByteString = imageByteString;
    }
  }
}
