using System.Collections.Generic;
using System.Threading.Tasks;
using Com.Opensource.Svga;
using Google.Protobuf;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace Svga.SvgaPlayer.Models {
  /// <summary>
  /// StageResource.
  /// 此类型代表舞台中的 SVGA 资源集合.
  /// </summary>
  public class StageResource {
    /// <summary>
    /// 包含了舞台中所有的 Sprite.
    /// </summary>
    public List<SvgaSprite> Sprites = new List<SvgaSprite>();

    /// <summary>
    /// 舞台引用.
    /// </summary>
    private CanvasAnimatedControl Stage { get; set; }

    /// <summary>
    /// 向 StageResource 添加一个 Sprite.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="imageByteString"></param>
    public async Task AddSprite (SpriteEntity entity, ByteString imageByteString) {
      var sprite = new SvgaSprite(this.Stage, entity, imageByteString);
      await sprite.Init();
      this.Sprites.Add(sprite);
    }

    public StageResource (CanvasAnimatedControl stage) {
      this.Stage = stage;
    }
  }
}
