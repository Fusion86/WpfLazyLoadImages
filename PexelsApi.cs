using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WpfLazyLoadImages
{
    public class PexelsApi
    {
        IRestClient client;

        public PexelsApi(string key)
        {
            client = new RestClient("https://api.pexels.com");
            client.AddDefaultHeader("Authorization", key);
        }

        public async Task<List<PexelsPhoto>> GetCurated(int count = 15, int page = 1)
        {
            var req = new RestRequest("/v1/curated");
            req.AddParameter("per_page", count);
            req.AddParameter("page", page);
            var resp = await client.ExecuteTaskAsync<PexelsCurated>(req);
            return resp.Data.Photos;
        }

        public async Task<byte[]> DownloadImage(string url)
        {
            var req = new RestRequest(url);
            var res = await client.ExecuteTaskAsync(req);
            return res.RawBytes;
        }
    }

    public partial class PexelsCurated
    {
        public long Page { get; set; }
        public long PerPage { get; set; }
        public List<PexelsPhoto> Photos { get; set; }
        public string NextPage { get; set; }
    }

    public partial class PexelsPhoto
    {
        public long Id { get; set; }
        public long Width { get; set; }
        public long Height { get; set; }
        public string Url { get; set; }
        public string Photographer { get; set; }
        public string PhotographerUrl { get; set; }
        public PexelsSrc Src { get; set; }
    }

    public partial class PexelsSrc
    {
        public string Original { get; set; }
        public string Large2X { get; set; }
        public string Large { get; set; }
        public string Medium { get; set; }
        public string Small { get; set; }
        public string Portrait { get; set; }
        public string Landscape { get; set; }
        public string Tiny { get; set; }
    }

}
