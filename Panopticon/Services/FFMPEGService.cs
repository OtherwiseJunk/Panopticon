using FFMpegCore.Extensions.System.Drawing.Common;
using System.Drawing;
using System.Net;
using System.Text.RegularExpressions;

namespace Panopticon.Services
{
    public class FFMPEGService
    {

        public HttpClient _httpClient { get; set; }

        public FFMPEGService(HttpClient client)
        {
            _httpClient = client;
        }

        private string ImageURLRegex = @"(http(s?):)([/|.|\w|\s|-])*\.(?:jpg|png)";
        private string VideoUrlRegex = @"(http(s?):)([/|.|\w|\s|-])*\.mp4";
        private string AnimateImageURLRegex = @"(http(s?):)([/|.|\w|\s|-])*\.(?:gif|gifv)";
        private string WebpUrlRegex = @"(http(s?):)([/|.|\w|\s|-])*\.webp";
        private string MediaUrlRegex = @"(http(s?):)([/|.|\w|\s|-])*\.(?:gif|gifv|webp|mp4|jpg|png)";

        public MediaType? GetMediaTypeFromUrl(string url)
        {
            MediaType? urlMediaType = null;
            if (Regex.Match(url, VideoUrlRegex).Success)
            {
                urlMediaType = MediaType.Video;
            }
            if (Regex.Match(url, AnimateImageURLRegex).Success)
            {
                urlMediaType = MediaType.AnimatedImage;
            }
            if (Regex.Match(url, WebpUrlRegex).Success)
            {
                urlMediaType = MediaType.Webp;
            }
            if (Regex.Match(url, ImageURLRegex).Success)
            {
                urlMediaType = MediaType.Image;
            }
            return urlMediaType;
        }

        public async Task<bool> AddWilhelmToAttachment(string attachmentUrl, MediaType mediaType, string filename = "output")
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, attachmentUrl))
            {
                using (HttpResponseMessage response = await _httpClient.SendAsync(request))
                {
                    switch (mediaType)
                    {
                        case MediaType.Video:
                            return false;
                        case MediaType.AnimatedImage:
                            return false;
                        case MediaType.Image:
                            return AddWilhemToImage(response.Content.ReadAsStream(), filename);
                        case MediaType.Webp:
                            return false;
                        default:
                            return false;
                    }
                }
            }
        }

        private void VerifyWilhelmFileExists()
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
            if (!File.Exists("./wilhelm.ogg"))
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile("https://upload.wikimedia.org/wikipedia/commons/d/d9/Wilhelm_Scream.ogg", "wilhelm.ogg");
                    
                }
            }
        }

        private bool AddWilhemToImage(Stream fileStream, string fileName)
        {
            Image image = MakeImageDimensionsEven(Image.FromStream(fileStream));
            VerifyWilhelmFileExists();            

            return image.AddAudio("./wilhelm.ogg", $"{fileName}.mp4");
        }

        private System.Drawing.Image MakeImageDimensionsEven(System.Drawing.Image image)
        {
            int height = image.Height % 2 == 0 ? image.Height : image.Height + 1;
            int width = image.Width % 2 == 0 ? image.Width : image.Width + 1;
            if (image.Height % 2 != 0 || image.Width % 2 != 0)
            {
                image = new Bitmap(image, new Size(width, height));
            }
            return image;
        }
    }

    public enum MediaType
    {
        Image,
        AnimatedImage,
        Webp,
        Video
    };
}
