using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Windows.UI.Core;
using Com.Opensource.Svga;
using Google.Protobuf;
using Google.Protobuf.Collections;

namespace Svga.SvgaPlayer.Controls {
  public partial class SvgaPlayer {
    /// <summary>
    /// SVGA 文件原始二进制.
    /// </summary>
    private byte[] _inflatedBytes;

    /// <summary>
    /// MovieEntity 对象.
    /// SVGA 的所有数据将从本对象中读取.
    /// </summary>
    private MovieEntity _movieEntity;

    /// <summary>
    /// 从 MovieEntity 中获取 SVGA 图片.
    /// MovieEntity 的 Images 属性下保存了 SVGA 的图片文件, 为一个可枚举的 byteString 集合,
    /// 键名为图片名称, 键值为 PNG 的二进制数据.
    /// </summary>
    private MapField<string, ByteString> _images;

    /// <summary>
    /// SVGA 配置参数.
    /// </summary>
    private MovieParams _movieParams;

    /// <summary>
    /// SVGA Sprite Entity 列表.
    /// </summary>
    private List<SpriteEntity> _sprites;

    /// <summary>
    /// 绘制的下一帧
    /// </summary>
    private int _drawNextFrame = -1;

    /// <summary>
    /// Sprite 数量.
    /// </summary>
    private int _spriteCount;
    public int SpriteCount {
      get => this._spriteCount;
      set {
        this._spriteCount = value;
        this.Notify(nameof(this.SpriteCount));
      }
    }

    /// <summary>
    /// 播放循环次数, 默认为 0.
    /// 当为 0 时代表无限循环播放.
    /// </summary>
    public int LoopCount { get; set; }

    /// <summary>
    /// 当前播放帧.
    /// </summary>
    private int _currentFrame;
    public int CurrentFrame {
      get => this._currentFrame;
      private set {
        this._currentFrame = value;
        this.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => {
          this.Notify(nameof(this.CurrentFrame));
        });
      }
    }

    /// <summary>
    /// 是否处于播放状态.
    /// </summary>
    private bool _isInPlay;
    public bool IsInPlay {
      get => this._isInPlay;
      set {
        this._isInPlay = value;
        this.Notify(nameof(this.IsInPlay));
      }
    }

    /// <summary>
    /// 动画总帧数.
    /// </summary>
    private int _totalFrame;
    public int TotalFrame {
      get => this._totalFrame;
      set {
        this._totalFrame = value;
        this.Notify(nameof(this.TotalFrame));
      }
    }

    /// <summary>
    /// 目标播放帧率.
    /// 若不设置或设置为 0 时使用默认帧率, 设置后将使用自定义帧率.
    /// </summary>
    private int _fps;
    public int Fps {
      get => this._fps;
      set {
        if (value < 0) { value = 0; }
        this._fps = value;
        this.Notify(nameof(this.Fps));
      }
    }

    /// <summary>
    /// 画布宽度.
    /// </summary>
    private float _stageWidth;
    public float StageWidth {
      get => this._stageWidth;
      set {
        this._stageWidth = value;
        this.Notify(nameof(this.StageWidth));
      }
    }

    /// <summary>
    /// 画布高度.
    /// </summary>
    private float _stageHeight;
    public float StageHeight {
      get => this._stageHeight;
      set {
        this._stageHeight = value;
        this.Notify(nameof(this.StageHeight));
      }
    }

    /// <summary>
    /// Inflate SVGA 文件, 获取其原始数据.
    /// SVGA 文件已经经过 Deflate, 所以第一步需要先进行 Inflate.
    /// </summary>
    private void InflateSvgaFile (Stream svgaFileBuffer) {
      byte[] inflatedBytes;

      // 微软自带的 DeflateStream 不认识文件头两个字节，SVGA 的这两字节为 78 9C，是 Deflate 的默认压缩表示字段.
      // 关于此问题请看 https://stackoverflow.com/questions/17212964/net-zlib-inflate-with-net-4-5.
      // Zlib 文件头请看 https://stackoverflow.com/questions/9050260/what-does-a-zlib-header-look-like.
      svgaFileBuffer.Seek(2, SeekOrigin.Begin);

      using (var deflatedStream = new DeflateStream(svgaFileBuffer, CompressionMode.Decompress)) {
        using (var stream = new MemoryStream()) {
          deflatedStream.CopyTo(stream);
          inflatedBytes = stream.ToArray();
        }
      }

      this._inflatedBytes = inflatedBytes;
    }

    /// <summary>
    /// 通过 Inflate 数据获取 SVGA 的 MovieEntity.
    /// </summary>
    /// <param name="inflatedBytes"></param>
    private void InitMovieEntity () {
      if (this._inflatedBytes == null) {
        return;
      }

      var moveEntity = MovieEntity.Parser.ParseFrom(this._inflatedBytes);
      this._movieEntity = moveEntity;
      this._movieParams = moveEntity.Params;
      this._images = moveEntity.Images;
      this._sprites = moveEntity.Sprites.ToList();
      this.TotalFrame = moveEntity.Params.Frames;
      this.SpriteCount = this._sprites.Count;
      this.StageWidth = this._movieParams.ViewBoxWidth;
      this.StageHeight = this._movieParams.ViewBoxHeight;
    }
  }
}
