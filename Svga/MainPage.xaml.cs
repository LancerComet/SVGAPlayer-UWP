using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Svga.Annotations;

namespace Svga {
  public sealed partial class MainPage : Page, INotifyPropertyChanged {
    public MainPage () {
      this.InitializeComponent();
    }

    private bool _isShowDoneText;
    private bool IsShowDoneText {
      get => this._isShowDoneText;
      set {
        this._isShowDoneText = value;
        this.Notify(nameof(this.IsShowDoneText));
      }
    }

    private async void SelectFileAndInitPlayer () {
      var picker = new FileOpenPicker {
        ViewMode = PickerViewMode.List,
        SuggestedStartLocation = PickerLocationId.Desktop
      };
      picker.FileTypeFilter.Add(".svga");

      var file = await picker.PickSingleFileAsync();
      if (file != null) {
        this.IsShowDoneText = false;
        using (var stream = await file.OpenAsync(FileAccessMode.Read, StorageOpenOptions.AllowOnlyReaders)) {
          var player = this.Player;
          player.UnloadStage();
//          player.LoopCount = 1;
          player.LoadSvgaFileData(stream.AsStream());
          player.InitStage();
          player.Play();
          player.OnLoopFinish += () => { this.IsShowDoneText = true; };
        }
      }
    }

    private void OnSelectFileClick (object sender, RoutedEventArgs e) {
      this.SelectFileAndInitPlayer();
    }

    private void OnPlayClick (object sender, RoutedEventArgs e) {
      this.Player.Play();
    }

    private void OnPauseClick (object sender, RoutedEventArgs e) {
      this.Player.Pause();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    private void Notify ([CallerMemberName] string propertyName = null) {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
