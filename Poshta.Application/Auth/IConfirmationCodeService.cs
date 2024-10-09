namespace Poshta.Application.Auth
{
    public interface IConfirmationCodeService
    {
        Task<string> GenerateCodeAsync(string key);
        Task<bool> ValidateCodeAsync(string key, string code);
        void RemoveCode(string key);
    }
}
