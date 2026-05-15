using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using QManager.ViewModels;

namespace QManager.View
{
    public partial class ChangePhotoView : UserControl
    {
        public event EventHandler<NavigationRequestEventArgs>? NavigationRequested;

        public ChangePhotoView()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = new ChangePhotoViewModel();
        }

        private async void SelectPhoto_Click(object? sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel != null && DataContext is ChangePhotoViewModel vm)
            {
                var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "Select Profile Photo",
                    FileTypeFilter = new[] { FilePickerFileTypes.ImageAll },
                    AllowMultiple = false
                });

                if (files.Count > 0)
                {
                    vm.SelectedPhotoPath = files[0].Path.LocalPath;
                }
            }
        }

        private void Back_Click(object? sender, RoutedEventArgs e)
        {
            NavigationRequested?.Invoke(this, new NavigationRequestEventArgs("Settings"));
        }
    }
}