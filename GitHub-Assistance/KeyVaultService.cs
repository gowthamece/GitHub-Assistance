using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace GitHub_Assistance
{
    public class KeyVaultService
    {
        private readonly SecretClient _secretClient;

        public KeyVaultService(string keyVaultUrl)
        {
            _secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
        }

        public async Task<string> GetSecretAsync(string secretName)
        {
            try
            {
                KeyVaultSecret secret = await _secretClient.GetSecretAsync(secretName);
                return secret.Value;
            }
            catch (Exception ex)
            {
                // Log and handle exceptions
                throw new InvalidOperationException($"Failed to retrieve secret: {secretName}", ex);
            }
        }
    }
}
