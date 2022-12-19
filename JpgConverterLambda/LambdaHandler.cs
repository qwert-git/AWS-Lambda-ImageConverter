using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaTest;

public class LambdaHandler
{
    private const string PngBucketName = "highload-png-images";
    private const string GifBucketName = "highload-gif-images";
    private const string BmpBucketName = "highload-bmp-images";

    IAmazonS3 S3Client { get; set; }

    /// <summary>
    /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
    /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
    /// region the Lambda function is executed in.
    /// </summary>
    public LambdaHandler()
    {
        S3Client = new AmazonS3Client();
    }

    /// <summary>
    /// Constructs an instance with a preconfigured S3 client. This can be used for testing outside of the Lambda environment.
    /// </summary>
    /// <param name="s3Client"></param>
    public LambdaHandler(IAmazonS3 s3Client)
    {
        this.S3Client = s3Client;
    }

    /// <summary>
    /// This method is called for every Lambda invocation. This method takes in an S3 event object and can be used 
    /// to respond to S3 notifications.
    /// </summary>
    /// <param name="evnt"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task handleRequest(S3Event evnt, ILambdaContext context)
    {
        var eventRecords = evnt.Records ?? new List<S3Event.S3EventNotificationRecord>();
        foreach (var record in eventRecords)
        {
            var s3Event = record.S3;
            if (s3Event == null)
            {
                continue;
            }

            try
            {
                using var response = await S3Client.GetObjectAsync(s3Event.Bucket.Name, s3Event.Object.Key);
                context.Logger.LogInformation($"GetObject status code: {response.HttpStatusCode}");
                context.Logger.LogInformation($"GetObject ContentLength: {response.ContentLength}");

                var image = new ImageConverter(response.ResponseStream);

                await PutToBucketAsync(image.ToPng(), PngBucketName, GetConvertedImageKey(s3Event.Object.Key, ".png"));
                await PutToBucketAsync(image.ToGif(), GifBucketName, GetConvertedImageKey(s3Event.Object.Key, ".gif"));
                await PutToBucketAsync(image.ToBmp(), BmpBucketName, GetConvertedImageKey(s3Event.Object.Key, ".bmp"));
            }
            catch (Exception e)
            {
                context.Logger.LogError(e.Message);
                context.Logger.LogError(e.StackTrace);
                throw;
            }
        }
    }

    private async Task<PutObjectResponse> PutToBucketAsync(Stream contentBody, string pngBucketName, string keyName)
    {
        if (!(await AmazonS3Util.DoesS3BucketExistV2Async(S3Client, pngBucketName)))
        {
            var putBucketRequest = new PutBucketRequest
            {
                BucketName = pngBucketName,
                UseClientRegion = true
            };

            PutBucketResponse putBucketResponse = await S3Client.PutBucketAsync(putBucketRequest);
        }

        var putObjectRequest = new PutObjectRequest
        {
            BucketName = pngBucketName,
            Key = keyName,
            InputStream = contentBody
        };

        return await S3Client.PutObjectAsync(putObjectRequest);
    }

    private static string GetConvertedImageKey(string originalKey, string extension)
    {
        var filename = Path.GetFileName(originalKey);
        return filename.Replace(Path.GetExtension(filename), extension);
    }
}