namespace Ophelia.Services.Aws;

public interface IParameterStoreService
{
    Task<string> GetParameterValueAsync(string parameterName);
}