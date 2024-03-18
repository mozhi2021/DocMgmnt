using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using DocMgmnt.Interface;
using DocMgmnt.Models;
using System.Diagnostics.Metrics;
using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using Amazon.Runtime;
using Microsoft.AspNetCore.Routing;

namespace DocMgmnt.Repositories
{
    public class DocumentHandler : IDocumentHandler
    {
        private readonly string _awsBucketName;
        private readonly string _awssourceBucketName;
        private readonly string? _awsdestinationBucketName;
        private readonly IConfiguration _configuration;
        private readonly IAmazonS3 _amazonS3Client;

        public DocumentHandler(IOptions<AWSConfig> awsConfig, IAmazonS3 amazonS3Client)
        {
            _awsBucketName = awsConfig.Value.BucketName;
            _awssourceBucketName = awsConfig?.Value.sourceBucketName;
           _awsdestinationBucketName = awsConfig?.Value.destinationBucketName;
            _amazonS3Client = amazonS3Client;
        }

        //Upload file to S3 bucket
        public async Task<string> UploadDocumentAsync(DocItem item)
        {
            string awsRequestID = string.Empty;
            var stream = item.File.OpenReadStream();
            var fileNameInS3Bucket = item.File.FileName;

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
            //var response = await _amazonS3Client.CopyObjectAsync(request);
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

        public string GeneratePreSignedUrl(string objectkey)
        {
            //The code first creates a presigned url and the uses it to upload
            //an object to an Amazon S3 bucket using that URL

            string url = string.Empty;
            AmazonS3Client client = new AmazonS3Client();

            try
            {
                AWSConfigsS3.UseSignatureVersion4 = true;
                var request = new GetPreSignedUrlRequest(); //create a copy object request
                request.BucketName = _awsBucketName;
                request.Key = objectkey;
                request.Verb = HttpVerb.PUT;
              //request.Expires = DateTime.UtcNow.AddDays(3);
                url = _amazonS3Client.GetPreSignedURL(request); //get path for request
            }
            catch (Exception ex)
            {
                throw;
            }
            return url;
        }

    }
}
