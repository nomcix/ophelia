using Amazon.S3;
using Amazon.S3.Model;

namespace Ophelia.Services.Aws;

public class S3Service : IS3Service
{
    private readonly AmazonS3Client _s3Client;
    private readonly ILogger<S3Service> _logger;

    public S3Service(ILogger<S3Service> logger)
    {
        _s3Client = new AmazonS3Client();
        _logger = logger;
    }
    
    public async Task PutObjectAsync(string bucketName, string key, Stream inputStream, string contentType)
    {
        try
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = key,
                InputStream = inputStream,
                ContentType = contentType
            };

            await _s3Client.PutObjectAsync(putRequest);
            
            _logger.LogInformation($"File uploaded successfully to bucket '{bucketName}' with key '{key}'.");
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, $"Error encountered on server when uploading object to S3. Message:'{ex.Message}' when writing an object");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unknown encountered on server. Message:'{ex.Message}' when writing an object");
            throw;
        }
    }

    public async Task<Stream> GetObjectAsync(string bucketName, string key)
    {
        try
        {
            var response = await _s3Client.GetObjectAsync(bucketName, key);
            return response.ResponseStream;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError($"Error encountered on server when accessing S3 bucket. Message:'{ex.Message}' when writing an object");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unknown encountered on server. Message:'{ex.Message}' when writing an object");
            throw;
        }
    }

    public async Task<List<string>> ListObjectsAsync(string bucketName)
    {
        try
        {
            var response = await _s3Client.ListObjectsV2Async(new ListObjectsV2Request
            {
                BucketName = bucketName
            });

            return response.S3Objects.Select(o => o.Key).ToList();
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, $"AmazonS3Exception in GetObjectAsync: {ex.Message}, Stack Trace: {ex.StackTrace}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception in GetObjectAsync: {ex.Message}, Stack Trace: {ex.StackTrace}");
            throw;
        }
    }
    
    public async Task<bool> DoesS3ObjectExist(string bucketName, string key)
    {
        try
        {
            await _s3Client.GetObjectMetadataAsync(new GetObjectMetadataRequest
            {
                BucketName = bucketName,
                Key = key
            });
            return true;
        }
        catch (AmazonS3Exception ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                return false;

            throw;
        }
    }
}