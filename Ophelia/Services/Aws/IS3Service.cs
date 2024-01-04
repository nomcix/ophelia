namespace Ophelia.Services.Aws;

public interface IS3Service
{
    Task PutObjectAsync(string bucketName, string key, Stream inputStream, string contentType);
    Task<Stream> GetObjectAsync(string bucketName, string key);
    Task<List<string>> ListObjectsAsync(string bucketName);
    Task<bool> DoesS3ObjectExist(string bucketName, string key);
}
