using System.Numerics;
using Windows.Foundation;
using Microsoft.Graphics.Canvas;
using Svga.SvgaPlayer.Models;

namespace Svga.SvgaPlayer.Controls {
  public partial class SvgaPlayer {
    /// <summary>
    /// 绘制单个 Sprite.
    /// </summary>
    /// <param name="session"></param>
    /// <param name="sprite"></param>
    private void DrawSingleSprite (CanvasDrawingSession session, SvgaSprite sprite) {
      var currentFrame = sprite.Frames[this.CurrentFrame];
      if (currentFrame != null) {
        Matrix4x4 perspective = new Matrix4x4();  // Sprite 透视参数.
        var transform = currentFrame.Transform;
        if (transform != null) {
          perspective = new Matrix4x4(new Matrix3x2(
            transform.A, transform.B, transform.C, transform.D, transform.Tx, transform.Ty
          ));
        }

        var width = 0f;
        var height = 0f;
        var layout = currentFrame.Layout;
        if (layout != null) {
          width = layout.Width;
          height = layout.Height;
        }

        var alpha = currentFrame.Alpha;
        
        session.DrawImage(
          sprite.CanvasBitmap, 0, 0, new Rect(0, 0, width, height), alpha,
          CanvasImageInterpolation.Linear, perspective
        );
        sprite.CurrentFrame++;
      }
    }
  }
}
