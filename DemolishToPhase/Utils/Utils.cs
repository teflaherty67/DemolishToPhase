using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemolishToPhase
{
    internal static class Utils
    {
        internal static RibbonPanel CreateRibbonPanel(UIControlledApplication app, string tabName, string panelName)
        {
            RibbonPanel currentPanel = GetRibbonPanelByName(app, tabName, panelName);

            if (currentPanel == null)
                currentPanel = app.CreateRibbonPanel(tabName, panelName);

            return currentPanel;
        }

        internal static RibbonPanel GetRibbonPanelByName(UIControlledApplication app, string tabName, string panelName)
        {
            foreach (RibbonPanel tmpPanel in app.GetRibbonPanels(tabName))
            {
                if (tmpPanel.Name == panelName)
                    return tmpPanel;
            }

            return null;
        }

        internal static int GetPhaseIndex(PhaseArray phaseArray, Phase targetPhase)
        {
            int phaseIndex = -1;
            if (phaseArray != null && targetPhase != null)
            {
                PhaseArrayIterator iterator = phaseArray.ForwardIterator();
                int index = 0;
                while (iterator.MoveNext())
                {
                    Phase phase = iterator.Current as Phase;
                    if (phase.Id == targetPhase.Id)
                    {
                        phaseIndex = index;
                        break;
                    }
                    index++;
                }
            }
            return phaseIndex;
        }
    }
}
