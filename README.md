# WpfLazyLoadImages

Small experiment to figure out how to load large image datasets (1000+ images) in WPF without making the UI hang for ages.

One issue I have with this solution is the fact that we have to manually call LoadImage on each ImageViewModel (which btw should probably be defined in ImageViewModel and not AppViewModel)./
This is needed because I don't know of any other way to 'lazy load' a ViewModel property in a controllable way (so that we can decide in which order we want to load them, top to bottom in this case).


## What this does

1. Asks some API (Pexels in this demo case) for a dataset (which contains some metadata and a ThumbnailUrl)
2. Transform said dataset in collection of ViewModels.
3. Update UI with all these ViewModels (this currently does lag the UI a bit, but to be fair we are adding 800 items at once)
    * After this point the user can scroll through the dataset and look at all the metadata etc.
4. Load all the thumbnails from top to bottom (one at a time)
    * While loading these thumbnails the user can request more images, which will load a new dataset and append it the the exiting dataset. These thumbnails will be loaded after all the thumbnails from the first dataset have been loaded.


## Possible improvements

- UI virtualization (WrapPanel does NOT support this out of the box, hence why it is disabled)
    - This doesn't actually make the scrolling any smoother (because it already is butter smooth), but when resizing the window huge 'lagspikes' occur.
- Memory usage is not fantastic (ALL thumbnails will be loaded into the RAM)
    - For my use case this is fine, because there won't be more than 1000 images loaded at the same time (which uses like 400MB, which is like half of the League of Legends Client, [this is not a joke](https://i.imgur.com/zzi0gBF.png))
    - A possible solution for this problem could be caching the images to the disk and unloading them whenever they scroll out of view(ing-range) and fetch them from the cache whenever they scroll into view.
- Maybe load like 4 thumbnails in parallel, instead of synchronously. We obviously don't want to load all 800 at the same time, but maybe loading like 4 at the same time could be faster. This could be faster because not all the time is spent on actually downloading the image (the network bandwidth is not fully saturated) but also on setting up the HTTP request etc (this is just a guess, I didn't actually measure any of this).
- Maybe port to AvaloniaUI and test on macOS and linux.
- Fix RestSharp's ArgumentException when calling DownloadData.
- Option to cancel the loading of thumbnails when the dataset is Clear()'ed.


## Troubleshooting

Q: It crashes when I click on the "Load more images" button  
A: Make sure to set the API key in AppViewModel.cs:37


## Extra reading material

https://www.codeproject.com/Articles/34405/WPF-Data-Virtualization
