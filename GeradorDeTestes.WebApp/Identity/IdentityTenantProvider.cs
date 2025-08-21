using GeradorDeTestes.Dominio.ModuloAutenticacao;
using System.Security.Claims;

namespace GeradorDeTestes.WebApp.Identity;

public class IdentityTenantProvider : ITenantProvider
{
    public IHttpContextAccessor ContextAccessor { get; }

    public IdentityTenantProvider(IHttpContextAccessor contextAccessor)
    {
        ContextAccessor = contextAccessor;
    }

    public Guid? UsuarioId
    {
        get
        {
            Claim? claimId = ContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);

            if (claimId is null)
                return null;

            return Guid.Parse(claimId!.Value);
        }
    }
}
