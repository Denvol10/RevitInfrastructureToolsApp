using System;
using System.Collections.Generic;
using System.IO;
using Autodesk.Revit.UI;
using System.Windows.Media.Imaging;

namespace RevitInfrastructureTools
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class App : IExternalApplication
    {
        public static string assemblyPath;
        public static string assemblyFolder;
        public static string ribbonPath;

        public Result OnStartup(UIControlledApplication application)
        {
            assemblyPath = typeof(App).Assembly.Location;
            assemblyFolder = Path.GetDirectoryName(assemblyPath);
            ribbonPath = assemblyFolder;
            string tabName = "Revit Infrastructure Tools";


            application.CreateRibbonTab(tabName);

            try
            {
                CreateLineUtilsRibbon(application, tabName);
                CreateBridgeDeckRibbon(application, tabName);
                CreateArrangementRoadRibbon(application, tabName);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ribbon Sample", ex.ToString());

                return Result.Failed;
            }

    }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        private void CreateLineUtilsRibbon(UIControlledApplication uiapp, string tabName)
        {
            string str = "Линии";
            RibbonPanel ribbonPanel = uiapp.CreateRibbonPanel(tabName, str);
            ribbonPanel.AddItem(CreateButtonData("DWGtoRVTLineConverter", "RevitCommand"));
            ribbonPanel.AddItem(CreateButtonData("ProjectPlaneCurves", "RevitCommand"));
        }

        private void CreateBridgeDeckRibbon(UIControlledApplication uiapp, string tabName)
        {
            string str = "Мостовые плиты";
            RibbonPanel ribbonPanel = uiapp.CreateRibbonPanel(tabName, str);

            ribbonPanel.AddItem(CreateButtonData("BridgeDeck", "RevitCommand"));
            ribbonPanel.AddItem(CreateButtonData("ApproachSlab", "RevitCommand"));

            PushButtonData buttonData1 = CreateButtonData("AdaptationForSlopeOnePoint", "RevitCommand");
            PushButtonData buttonData2 = CreateButtonData("AdaptationForSlopeTwoPoints", "RevitCommand");
            ribbonPanel.AddStackedItems(buttonData1, buttonData2);
        }

        private void CreateArrangementRoadRibbon(UIControlledApplication uiapp, string tabName)
        {
            string str = "Обустройство";
            RibbonPanel ribbonPanel = uiapp.CreateRibbonPanel(tabName, str);

            ribbonPanel.AddItem(CreateButtonData("SafetyBarriers", "RevitCommand"));
            ribbonPanel.AddItem(CreateButtonData("NoiseBarriers", "RevitCommand"));
        }

        public PushButtonData CreateButtonData(string assemblyName, string className)
        {
            string dllPath = Path.Combine(ribbonPath, assemblyName + ".dll");
            string fullClassname = assemblyName + "." + "Infrastructure" + "." + className;
            string dataPath = Path.Combine(ribbonPath, assemblyName + "_data");
            string largeIcon = Path.Combine(dataPath, className + "_large.png");
            string smallIcon = Path.Combine(dataPath, className + "_small.png");
            string textPath = Path.Combine(dataPath, className + ".txt");
            string[] text = File.ReadAllLines(textPath);
            string title = text[0];
            if (title.Contains("/"))
                title = title.Replace("/", "\n");
            string tooltip = text[1];
            string url = text[2];

            PushButtonData data = new PushButtonData(fullClassname, title, dllPath, fullClassname);

            data.LargeImage = new BitmapImage(new Uri(largeIcon, UriKind.Absolute));
            data.Image = new BitmapImage(new Uri(smallIcon, UriKind.Absolute));

            data.ToolTip = text[1];

            ContextualHelp chelp = new ContextualHelp(ContextualHelpType.Url, url);
            data.SetContextualHelp(chelp);

            return data;
        }
    }
}
