using System.Web.Optimization;

namespace SimpleViewEngine.Example
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/bundles/css")
                .Include("~/Content/site.css")
                .Include("~/Content/home.css"));
        }
    }
}
