using System.Web;
using System.Web.Optimization;

namespace Computyme
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/autocomplete").Include(
            "~/Scripts/ui/jquery.ui.autocomplete.js",
            "~/Scripts/ui/jquery.ui.menu.js"));


            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"
                     ));


            bundles.Add(new ScriptBundle("~/bundles/jqgrid").Include(
            "~/Scripts/js/i18n/grid.locale-en.js",
            "~/Scripts/jqGridExportToExcel.js",
            "~/Scripts/src/jqModal.js",
            "~/Scripts/jquery.jqGrid.min.js",
            "~/Scripts/jquery.jqGrid.src.js",
            "~/Scripts/src/jqDnR.js",
            "~/Scripts/src/JsonXml.js"
            ));

        


            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                    "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui/extended").Include(
               "~/scripts/ui/jquery.ui.core.js",
               "~/scripts/ui/jquery.ui.widget.js",
               "~/scripts/ui/jquery.ui.tabs.js",
               "~/scripts/ui/jquery.ui.accordion.js",
               "~/Scripts/ui/jquery.ui.dialog.js",
               "~/Scripts/ui/jquery.ui.button.js",
               "~/Scripts/ui/jquery.ui.progressbar.js",
                "~/Scripts/ui/jquery.ui.menu.js",
                "~/Scripts/ui/jquery.ui.effect.js",
                "~/Scripts/ui/jquery.ui.position.js"));

        }
    }
}
