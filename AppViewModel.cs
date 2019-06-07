using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace WpfLazyLoadImages
{
    public class AppViewModel : ReactiveObject, IEnableLogger
    {
        public PexelsApi Pexels { get; }

        public ReactiveCommand<Unit, ImageViewModel> LoadPhotosCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        private SourceList<ImageViewModel> images = new SourceList<ImageViewModel>();
        public ObservableCollectionExtended<ImageViewModel> ImagesBinding { get; } = new ObservableCollectionExtended<ImageViewModel>();

        [Reactive]
        public bool ScrollToBottom { get; set; }

        // TODO: This doesn't actually update in the UI for some reason
        [ObservableAsProperty]
        public string LoadedImagesText { get; }

        [ObservableAsProperty]
        public bool IsLoadingPhotos { get; }

        private int page = 1;

        // TODO: Auto fetch new posts when scrolling to the bottom of the ListView

        public AppViewModel()
        {
            Pexels = new PexelsApi("snip");

            images.Connect().Bind(ImagesBinding).Subscribe();

            LoadPhotosCommand = ReactiveCommand.CreateFromObservable(LoadPhotos);
            LoadPhotosCommand.Subscribe(x => images.Add(x));
            LoadPhotosCommand.IsExecuting.ToPropertyEx(this, x => x.IsLoadingPhotos);

            CancelCommand = ReactiveCommand.Create(() => { }, LoadPhotosCommand.IsExecuting);
        }

        private IObservable<ImageViewModel> LoadPhotos()
        {
            int multiplier = 1;

            return Observable.Create<ImageViewModel>(async observer =>
            {
                var data = await Pexels.GetCurated(80, page++);

                // Multiply our dataset
                List<PexelsPhoto> dataSet = new List<PexelsPhoto>(data.Count * multiplier);
                for (int i = 0; i < multiplier; i++)
                    dataSet.AddRange(data);

                var vms = dataSet.Select(x => new ImageViewModel(x));

                // TODO: Do this parallel (with like 4 threads at the same time)
                foreach (var vm in vms)
                {
                    if (!IsLoadingPhotos)
                    {
                        this.Log().Info("Cancelled");
                        break;
                    }

                    this.Log().Info("Downloading thumbnail for " + vm.Name);
                    byte[] bytes = Pexels.DownloadImage(vm.ThumbnailUrl);

                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        IBitmap bmp = await BitmapLoader.Current.Load(ms, null, null);
                        vm.Thumbnail = bmp.ToNative();
                        observer.OnNext(vm);
                    }
                }

                observer.OnCompleted();
            }).TakeUntil(CancelCommand);
        }
    }
}
