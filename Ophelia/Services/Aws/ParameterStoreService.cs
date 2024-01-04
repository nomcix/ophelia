using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

namespace Ophelia.Services.Aws
{
    public class ParameterStoreService : IParameterStoreService
    {
        private readonly IAmazonSimpleSystemsManagement _ssmClient;
        private readonly ILogger<ParameterStoreService> _logger;

        public ParameterStoreService(IAmazonSimpleSystemsManagement ssmClient, ILogger<ParameterStoreService> logger)
        {
            _ssmClient = ssmClient;
            _logger = logger;
        }

        public async Task<string> GetParameterValueAsync(string parameterName)
        {
            try
            {
                var request = new GetParameterRequest
                {
                    Name = parameterName,
                    WithDecryption = true
                };

                var response = await _ssmClient.GetParameterAsync(request);
                return response.Parameter.Value;
            }
            catch (AmazonSimpleSystemsManagementException ex)
            {
                _logger.LogError(ex, "Error fetching parameter from AWS Parameter Store: {ParameterName}", parameterName);
                throw new ApplicationException($"Error fetching parameter: {parameterName}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetParameterValueAsync for parameter: {ParameterName}", parameterName);
                throw;
            }
        }
    }
}