using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
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

        public ReactiveCommand<Unit, Unit> LoadPhotosCommand { get; }

        private SourceList<ImageViewModel> images = new SourceList<ImageViewModel>();
        public ObservableCollectionExtended<ImageViewModel> Images { get; } = new ObservableCollectionExtended<ImageViewModel>();

        [Reactive]
        public long LoadedImages { get; private set; } = 0;

        // TODO: This doesn't actually update in the UI for some reason
        [ObservableAsProperty]
        public string LoadedImagesText { get; }

        private int page = 1;

        private readonly Stopwatch loadImageStopwatch = new Stopwatch();

        public AppViewModel()
        {
            Pexels = new PexelsApi("563492ad6f91700001000001b68e8f6901fa462896a5bc6024e49b0d");

            images.Connect().Bind(Images).Subscribe();

            images.Connect()
                .ObserveOn(RxApp.TaskpoolScheduler)
                .SubscribeMany(LoadImage)
                .Subscribe();

            LoadPhotosCommand = ReactiveCommand.CreateFromTask(LoadData);

            var obs = this.WhenAnyValue(x => x.Images.Count, x => x.LoadedImages,
                (total, loaded) => $"Loaded images/total: {loaded}/{total}");

            obs.ToPropertyEx(this, x => x.LoadedImagesText);

            obs.Subscribe(x => this.Log().Info(x));
        }

        private async Task LoadData()
        {
            int multiplier = 10;
            var data = await Pexels.GetCurated(80, page++);
            var vms = data.Select(x => new ImageViewModel(x));

            Stopwatch sw = new Stopwatch();
            sw.Start();

            images.Edit(list =>
            {
                // Simulate loading more data by duplicating our dataset
                for (int i = 0; i < multiplier; i++)
                    list.AddRange(vms);
            });

            sw.Stop();
            this.Log().Info($"Added {vms.Count() * multiplier} items in {sw.ElapsedMilliseconds}ms");
        }

        private IDisposable LoadImage(ImageViewModel image)
        {
            this.Log().Info("Downloading thumbnail for " + image.Name);
            loadImageStopwatch.Restart();

            byte[] bytes = Pexels.DownloadImage(image.ThumbnailUrl);

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                IBitmap bmp = BitmapLoader.Current.Load(ms, null, null).Result;
                image.Thumbnail = bmp.ToNative();
                LoadedImages++;
            }
            loadImageStopwatch.Stop();
            this.Log().Info("Downloaded in " + loadImageStopwatch.ElapsedMilliseconds + "ms");

            return Disposable.Empty;
        }
    }
}
