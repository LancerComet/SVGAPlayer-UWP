using System;
using System.IO;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Svga {
  public sealed partial class MainPage : Page {
    public MainPage () {
      this.InitializeComponent();
    }

    private async void SelectFileAndInitPlayer () {
      var picker = new FileOpenPicker();
      picker.ViewMode = PickerViewMode.List;
      picker.SuggestedStartLocation = PickerLocationId.Desktop;
      picker.FileTypeFilter.Add(".svga");

      var file = await picker.PickSingleFileAsync();
      if (file != null) {
        using (var stream = await file.OpenAsync(FileAccessMode.Read, StorageOpenOptions.AllowOnlyReaders)) {
          var player = this.Player;
//          player.LoopCount = 1;
          player.LoadSvgaFileData(stream.AsStream());
          player.InitStage();
          player.Play();
        }
      }
    }

    private void OnSelectFileClick (object sender, RoutedEventArgs e) {
      this.SelectFileAndInitPlayer();
    }

    private void OnToggleClick (object sender, RoutedEventArgs e) {
      var player = this.Player;
      if (player.IsInPlay) {
        player.Pause();
      } else {
        player.Play();
      }
    }
  }
}
