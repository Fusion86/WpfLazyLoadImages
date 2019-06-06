using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Windows.Media.Imaging;

namespace WpfLazyLoadImages
{
    public class ImageViewModel : ReactiveObject
    {
        public string Name { get; }
        public string ThumbnailUrl { get; set; }
        public string ContentUrl { get; set; }

        [Reactive]
        public BitmapSource Content { get; set; }

        [Reactive]
        public BitmapSource Thumbnail { get; set; }

        public ImageViewModel(PexelsPhoto x)
        {
            Name = "Photo " + x.Id;
            ThumbnailUrl = x.Src.Tiny;
            ContentUrl = x.Src.Large2X;
        }
    }
}
