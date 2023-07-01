using Amazon.S3;
using Amazon.S3.Transfer;

namespace Panopticon.Services
{
    public class DOSpacesService
    {
		public static string PublicKey { get; set; }
		public static string SecretKey { get; set; }
		public static string Url { get; set; }
		public static string Bucket { get; set; }
		public DOSpacesService(string s3PublicKey, string s3SecretKey, string s3Url, string s3Bucket)
		{
			PublicKey = s3PublicKey;
			SecretKey = s3SecretKey;
			Url = s3Url;
			Bucket = s3Bucket;
		}

        public string UploadMedia(string folderName, Stream ImageStream, string fileExtension="png")
		{
			string filename = $"{Guid.NewGuid()}.{fileExtension}";
			AmazonS3Config s3ClientConfig = new AmazonS3Config()
			{
				ServiceURL = $"https://{Url}",
			};
			using (AmazonS3Client client = new AmazonS3Client(PublicKey, SecretKey, s3ClientConfig))
			{
				using (TransferUtility fileTransferUtility = new TransferUtility(client))
				{
					try
					{
						TransferUtilityUploadRequest request = new TransferUtilityUploadRequest
						{
							BucketName = $"{Bucket}/{folderName}",
							InputStream = ImageStream,
							Key = filename,
							CannedACL = S3CannedACL.PublicRead
						};
						fileTransferUtility.Upload(request);
						return $"https://{Bucket}.nyc3.cdn.digitaloceanspaces.com/{folderName}/{filename}";
					}
					catch (AmazonS3Exception e)
					{
						Console.WriteLine("Error encountered ***. Message:'{0}' when writing an object", e.Message);
					}
					catch (Exception e)
					{
						Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
					}
				}
			}
			return null;
		}

		public async void DeleteMedia(string folderName, string key)
		{
			AmazonS3Config s3ClientConfig = new AmazonS3Config
			{
				ServiceURL = Url,
			};
			using (AmazonS3Client client = new AmazonS3Client(PublicKey, SecretKey, s3ClientConfig))
			{
				try
				{
					await client.DeleteObjectAsync($"{Bucket}/{folderName}", key);
				}
				catch (AmazonS3Exception e)
				{
					Console.WriteLine("Error encountered ***. Message:'{0}' when deleting an object", e.Message);
				}
				catch (Exception e)
				{
					Console.WriteLine("Unknown encountered on server. Message:'{0}' when deleting an object", e.Message);
				}
			}
		}
	}
}
