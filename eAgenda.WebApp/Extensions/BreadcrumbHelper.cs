using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;

namespace eAgenda.WebApp.Extensions;

public static class BreadcrumbHelper
{
    public static IHtmlContent BuildBreadcrumbNavigation(this IHtmlHelper helper)
    {
        var viewContext = helper.ViewContext;
        var routeValues = viewContext.RouteData.Values;

        string controller = routeValues["controller"]?.ToString() ?? "";
        string action = routeValues["action"]?.ToString() ?? "";

        var urlHelperFactory = (IUrlHelperFactory)viewContext.HttpContext.RequestServices.GetService(typeof(IUrlHelperFactory))!;
        var urlHelper = urlHelperFactory.GetUrlHelper(viewContext);

        string breadcrumb = $"""
            <a href="{urlHelper.Action("Index", "Home")}">Home</a> &gt;
            <a href="{urlHelper.Action("Index", controller)}">{controller}</a> &gt;
            {action}
        """;

        return new HtmlString(breadcrumb);
    }
}
