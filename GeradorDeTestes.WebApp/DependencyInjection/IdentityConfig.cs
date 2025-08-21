using GeradorDeTestes.Dominio.ModuloAutenticacao;
using GeradorDeTestes.Infraestrutura.ORM.Compartilhado;
using GeradorDeTestes.WebApp.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace GeradorDeTestes.WebApp.DependencyInjection;

public static class IdentityConfig
{
    public static void AddIndentityProviderConfig(this IServiceCollection services)
    {

        services.AddScoped<ITenantProvider, IdentityTenantProvider>();

        services.AddIdentity<Usuario, Cargo>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 6;
        })
        .AddEntityFrameworkStores<GeradorDeTestesDbContext>()
        .AddDefaultTokenProviders();
    }

    public static void AddCookieAuthenticationConfig(this IServiceCollection services)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "AspNetCore.Cookies";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                options.SlidingExpiration = true;
            });

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/autenticacao/login";
            options.AccessDeniedPath = "/";
        });
    }
}
