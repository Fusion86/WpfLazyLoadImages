using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace WpfLazyLoadImages
{
    public partial class MainWindow : ReactiveWindow<AppViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new AppViewModel();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.ImagesBinding, v => v.ListView.ItemsSource).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.LoadedImagesText, v => v.LoadedImagesText.Text).DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.ScrollToBottom, v => v.ScrollToBottomCheck.IsChecked).DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.LoadPhotosCommand, v => v.LoadMoreButton).DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.CancelCommand, v => v.CancelButton).DisposeWith(disposables);

                // This method kinda works, but doesn't trigger the ListView_ScrollChanged infinite scrolling AND it's surely not the best way (with ReactiveUI at our disposal)
                //ViewModel.ImagesBinding.CollectionChanged += (sender, e) =>
                //{
                //    if (ViewModel.ScrollToBottom)
                //    {
                //        ListView.ScrollIntoView(e.NewItems[e.NewItems.Count - 1]);
                //    }
                //};
            });
        }

        private void ListView_ScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
        {
            // Disabled because buggy as duck
            //double distanceToEnd = e.ExtentHeight - e.VerticalOffset - e.ViewportHeight;

            //if (distanceToEnd - 10 <= e.ViewportHeight && !ViewModel.IsLoadingPhotos)
            //    ViewModel.LoadPhotosCommand.Execute().Subscribe();
        }
    }
}
