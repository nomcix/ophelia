    using System.Net;
    using Amazon.S3;
    using Microsoft.AspNetCore.Mvc;
    using Ophelia.Services;
    using Ophelia.Services.Aws;

    namespace Ophelia.Controllers;

    public class S3Controller : ControllerBase
    {
        private readonly IS3Service _s3Service;
        public S3Controller(IS3Service s3Service, ILogger<S3Controller> logger)
        {
            _s3Service = s3Service;
        }
        
        [HttpGet("list/{bucketName}")]
        public async Task<IActionResult> ListObjects(string bucketName)
        {
            var objects = await _s3Service.ListObjectsAsync(bucketName).ConfigureAwait(false);
            return Ok(objects);
        }

        [HttpGet("get/{bucketName}/{*key}")]
        public async Task<IActionResult> GetObject(string bucketName, string key, string contentType = "audio/mpeg")
        {
            if (string.IsNullOrWhiteSpace(bucketName) || string.IsNullOrWhiteSpace(key))
            {
                return BadRequest("Bucket name and key are required.");
            }

            try
            {
                var decodedKey = WebUtility.UrlDecode(key);
                var stream = await _s3Service.GetObjectAsync(bucketName, decodedKey).ConfigureAwait(false);

                if (!IsValidContentType(contentType))
                {
                    return BadRequest("Invalid content type.");
                }

                return File(stream, contentType, enableRangeProcessing: true);
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound(ex.Message);
            }
            catch (AmazonS3Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("exists/{bucketName}/{*key}")]
        public async Task<IActionResult> DoesS3ObjectExist(string bucketName, string key)
        {
            var decodedKey = WebUtility.UrlDecode(key);
            var exists = await _s3Service.DoesS3ObjectExist(bucketName: bucketName, key: decodedKey).ConfigureAwait(false);
            return Ok(exists);
        }
        
        private static bool IsValidContentType(string contentType)
        {
            var validContentTypes = new HashSet<string>
            {
                "audio/mpeg",
                "audio/*",
            };
            return validContentTypes.Contains(contentType);
        }
    }