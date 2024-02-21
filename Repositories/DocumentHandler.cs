using Amazon.S3.Model;
using Amazon.S3;
using Microsoft.Extensions.Options;
using DocMgmnt.Interface;
using DocMgmnt.Models;
using System.Diagnostics.Metrics;
using Amazon.Runtime.Internal.Endpoints.StandardLibrary;

namespace DocMgmnt.Repositories
{
    public class DocumentHandler : IDocumentHandler
    {
        private readonly string _awsBucketName;
        private readonly string _awssourceBucketName;
        private readonly string? _awsdestinationBucketName;
        private readonly IConfiguration _configuration;
        private readonly AmazonS3Client _amazonS3Client;

        public DocumentHandler(IOptions<AWSConfig> awsConfig)
        {
            _awsBucketName = awsConfig.Value.BucketName;
            _awssourceBucketName = awsConfig?.Value.sourceBucketName;
            _awsdestinationBucketName = awsConfig?.Value.destinationBucketName;
        }

        //Upload file to S3 bucket
        public async Task<string> UploadDocumentAsync(DocItem item)
        {
            string awsRequestID = string.Empty;
            var stream = item.File.OpenReadStream();
            var fileNameInS3Bucket = item.File.FileName;
            //string sourceObjectKey = item;

            AmazonS3Client client = new AmazonS3Client();
            var request = new PutObjectRequest()
            {
                BucketName = _awsBucketName,
                Key = fileNameInS3Bucket,
                InputStream = stream
            };
            try
            {
                var response = await client.PutObjectAsync(request);
                awsRequestID = response.ResponseMetadata.RequestId.ToString();
            }
            catch (AmazonS3Exception amazonS3exception)
            {
                var error = amazonS3exception.Message;
                throw new Exception(error);
            }
            return awsRequestID;
        }

        //Move one file S3 to another S3
        public async Task<string> MoveDocumentAsync(string file)
        {
            string awsRequestID = string.Empty;

            string sourceObjectKey = file;
            string destinationObjectKey = file;

            AmazonS3Client client = new AmazonS3Client();

            var request = new CopyObjectRequest
            {
                SourceBucket = _awssourceBucketName,
                SourceKey = sourceObjectKey,
                DestinationBucket = _awsdestinationBucketName,
                DestinationKey = destinationObjectKey,
            };

            var response = await client.CopyObjectAsync(request);
            awsRequestID = response.ResponseMetadata.RequestId.ToString();
            return awsRequestID;
        }

        //Move all files S3 to another S3
        public async Task<string> MoveAllDocumentAsync()
        {

            int counter = 0;

            AmazonS3Client client = new AmazonS3Client();

            ListObjectsRequest request = new ListObjectsRequest
            {
                BucketName = _awssourceBucketName,
            };

            ListObjectsResponse response = await client.ListObjectsAsync(request);

            foreach (S3Object obj in response.S3Objects)
            {
                string result = await MoveDocumentAsync(obj.Key);
                counter++;
            }

            return counter.ToString() + " files are moved";
        }

        //GetPredefined URL
        public async Task<string> GeneratePreSignedUploadUrl(string objectkey)
        {
            //The code first creates a presigned url and the uses it to upload
            //an object to an Amazon S3 bucket using that URL
            string url = string.Empty;
            AmazonS3Client client = new AmazonS3Client();

            try
            {
                var request = new GetPreSignedUrlRequest();
                request.BucketName = _awsBucketName;
                request.Key = objectkey;
                request.Verb = HttpVerb.PUT;

                // url = _s3Client.GetPreSignedURL(request);
                // url = _client.GetPreSignedURL(request);
                url = _amazonS3Client.GetPreSignedURL(request);
            }
            catch (Exception)
            {
                throw;
            }
            return url;
        }

    }

}
