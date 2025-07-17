using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;

namespace GeradorDeTestes.WebApp.Helpers;

public static class BreadcrumbHelper
{
    public static IHtmlContent BuildBreadcrumbNavigation(this IHtmlHelper helper)
    {
        Dictionary<string, string> controllerDisplayNames = new()
        {
            { "Disciplina", "Disciplinas" },
            { "Materia", "Materias" },
            { "Questao", "Questoes" },
            { "Teste", "Testes" },
        };

        ViewContext viewContext = helper.ViewContext;
        RouteValueDictionary routeValues = viewContext.RouteData.Values;

        string controller = routeValues["controller"]?.ToString() ?? "";
        string action = routeValues["action"]?.ToString() ?? "";

        IUrlHelperFactory urlHelperFactory = (IUrlHelperFactory)viewContext.HttpContext.RequestServices.GetService(typeof(IUrlHelperFactory))!;
        IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(viewContext);

        string controllerDisplay = controllerDisplayNames.ContainsKey(controller)
             ? controllerDisplayNames[controller] : controller;

        string breadcrumb = $"""
            <a href="{urlHelper.Action("Index", "Home")}">Home</a>
        """;

        if (!string.Equals(controller, "Home", StringComparison.OrdinalIgnoreCase))
        {
            breadcrumb += $"""
                <span class="separator"><i class="bi bi-chevron-right fw-bold"></i></span>
                <a href="{urlHelper.Action("Index", controller)}">{controllerDisplay}</a>
            """;
        }

        if (!string.Equals(action, "Index", StringComparison.OrdinalIgnoreCase))
        {
            breadcrumb += $"""
                <span class="separator"><i class="bi bi-chevron-right fw-bold"></i></span>
                {action}
            """;
        }

        return new HtmlString(breadcrumb);
    }
}
