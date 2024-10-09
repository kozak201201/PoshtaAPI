namespace Poshta.Application.Auth;

public interface IJwtProvider
{
    string Generate(Guid userId, IList<string> roles, IDictionary<string, string> additionalClaims);
}