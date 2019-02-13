using System.IO;
using System.IO.Compression;
using Com.Opensource.Svga;
using Google.Protobuf;
using Google.Protobuf.Collections;

namespace Svga.SvgaPlayer.Controls {
  public partial class SvgaPlayer {
    private byte[] InflatedBytes { get; set; }
    private MovieEntity MovieEntity { get; set; }

    /// <summary>
    /// 从 MovieEntity 中获取 SVGA 图片.
    /// MovieEntity 的 Images 属性下保存了 SVGA 的图片文件, 为一个可枚举的 byteString 集合,
    /// 键名为图片名称, 键值为 PNG 的二进制数据.
    /// </summary>
    private MapField<string, ByteString> Images => this.MovieEntity?.Images;

    /// <summary>
    /// SVGA 配置参数.
    /// </summary>
    private MovieParams MovieParams => this.MovieEntity?.Params;

    /// <summary>
    /// SVGA Sprite 对象.
    /// </summary>
    private RepeatedField<SpriteEntity> Sprites => this.MovieEntity?.Sprites;

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

      this.InflatedBytes = inflatedBytes;
    }

    /// <summary>
    /// 通过 Infalte 数据获取 SVGA 的 MovieEntity.
    /// </summary>
    /// <param name="inflatedBytes"></param>
    private void GetMovieEntity () {
      if (this.InflatedBytes != null) {
        this.MovieEntity = MovieEntity.Parser.ParseFrom(this.InflatedBytes);
      }
    }

    /// <summary>
    /// 载入 SVGA 文件数据.
    /// </summary>
    /// <param name="svgaFileBuffer"></param>
    public void LoadSvgaFileData (Stream svgaFileBuffer) {
      this.InflateSvgaFile(svgaFileBuffer);
      this.GetMovieEntity();
    }
  }
}
