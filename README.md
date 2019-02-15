# SVGAPlayer For UWP

This is the UWP version for SVGA.

![img](https://raw.githubusercontent.com/LancerComet/SVGAPlayer-UWP/master/Static/Screenshot.gif)

## Quickstart

Please check both `MainPage.xaml` and `MainPage.xaml.cs` to see the demo.

## API

```c#
class SvgaPlayer {
  /// <summary>
  /// 播放循环次数, 默认为 0.
  /// 当为 0 时代表无限循环播放.
  /// </summary>
  public int LoopCount { get; set; }

  /// <summary>
  /// 目标播放帧率.
  /// 若不设置或设置为 0 时使用默认帧率, 设置后将使用自定义帧率.
  /// </summary>
  public int Fps { get; set; }

  /// <summary>
  /// 当前是否处于播放状态.
  /// </summary>
  public bool IsInPlay { get; }

  /// <summary>
  /// 载入 SVGA 文件数据.
  /// </summary>
  /// <param name="svgaFileBuffer">SVGA 文件二进制 Stream.</param>
  public void LoadSvgaFileData (Stream svgaFileBuffer) {}

  /// <summary>
  /// 初始化 Player 舞台.
  /// 任何配置项请在调用此方法前执行.
  /// </summary>
  public void InitStage () {} 

  /// <summary>
  /// 开始播放.
  /// </summary>
  public void Play () {}

  /// <summary>
  /// 暂停.
  /// </summary>
  public void Pause () {}
  
  /// <summary>
  /// 卸载舞台所有数据.
  /// </summary>
  public void UnloadStage () {}
}
```

## SVGA File Struct

More Information about SVGA File: [Link](https://github.com/yyued/SVGA-Format)

![img](https://raw.githubusercontent.com/LancerComet/SVGAPlayer-UWP/master/Svga/SVGA%20File%20Sturct.png)
