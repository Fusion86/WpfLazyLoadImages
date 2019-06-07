using ReactiveUI;
using System.Reactive.Disposables;

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
                this.OneWayBind(ViewModel, vm => vm.Images, v => v.ListView.ItemsSource).DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.LoadPhotosCommand, v => v.LoadMoreButton).DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.CancelCommand, v => v.CancelButton).DisposeWith(disposables);
            });
        }
    }
}
