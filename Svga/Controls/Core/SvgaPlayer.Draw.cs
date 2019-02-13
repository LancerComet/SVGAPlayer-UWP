using Com.Opensource.Svga;

namespace Svga.Controls {
  public partial class SvgaPlayer {
    /// <summary>
    /// 绘制 Sprites.
    /// </summary>
    private void DrawSprites () {
      var sprites = this.Sprites;
      foreach (var spriteEntity in sprites) {
        this.DrawSingleSprite(spriteEntity);
      }
    }

    /// <summary>
    /// 绘制单个 Sprite.
    /// </summary>
    /// <param name="sprite"></param>
    private void DrawSingleSprite (SpriteEntity sprite) {
      if (sprite == null) {
        return;
      }

      var frames = sprite.Frames;
      foreach (var frameEntity in frames) {
        this.DrawSingleFrame(frameEntity);
      }
    }

    /// <summary>
    /// 绘制图像单帧.
    /// </summary>
    /// <param name="frame"></param>
    private void DrawSingleFrame (FrameEntity frame) {
    }
  }
}
