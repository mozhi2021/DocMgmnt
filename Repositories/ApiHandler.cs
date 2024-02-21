using DocMgmnt.Interface;
using DocMgmnt.Models;

namespace DocMgmnt.Repositories
{
    public class ApiHandler : IApiHandler
    {
        private readonly IConfiguration _configuration;
        private string? userApiKey;

        public ApiHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool IsValidApiKey(string userapiKey)
        {
            if (string.IsNullOrWhiteSpace(userApiKey))
                return false;

            string? apiKey = _configuration.GetValue<string>(Models.Constants.ApiKeyName);

            if (apiKey == null || apiKey != userApiKey)
                return false;

            return true;
        }
    }
}
