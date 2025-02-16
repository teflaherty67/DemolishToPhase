using DemolishToPhase.Forms;

namespace DemolishToPhase
{
    internal static class Utils
    {
        public static bool ShowForm;
        internal static void Run(UIApplication uiapp, Document curDoc)
        {
            frmDemolishToPhase curWin = new frmDemolishToPhase(uiapp.ActiveUIDocument.Document);
            curWin.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
            curWin.ShowDialog();

            if (Utils.ShowForm)
            {
                IList<Reference> pickList = uiapp.ActiveUIDocument.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element);

                using (Transaction t = new Transaction(curDoc))
                {
                    t.Start("Demolish to Phase");

                    if (pickList.Count != 0)
                    {
                        foreach (Reference pick in pickList)
                        {
                            // get current element from the pick list
                            Element curElem = curDoc.GetElement(pick);

                            ElementId demoPhaseId = null;

                            Parameter paramPhaseDemo = curElem.get_Parameter(BuiltInParameter.PHASE_DEMOLISHED);

                            // get the PhaseDemolishedID
                            if (curWin.selectedPhase == "None")
                            {
                                demoPhaseId = ElementId.InvalidElementId;
                            }
                            else if (curWin.selectedPhase != "None")
                            {
                                // get the value of the PHASE_CREATED parameter for curElem
                                Parameter paramPhaseCreated = curElem.get_Parameter(BuiltInParameter.PHASE_CREATED);

                                Phase createdPhase = curElem.Document.GetElement(paramPhaseCreated.AsElementId()) as Phase;

                                // get the index of the PHASE_CREATED value from the phase array
                                int indexCreated = GetPhaseIndex(curWin.arrayPhases, createdPhase);

                                // get the value of the selectedDemoPhase variable
                                Phase demoPhase = Utils.getPhaseByName(curDoc, curWin.selectedPhase);

                                // get the index of the selectedDemoPhase variable from the phase array
                                int indexDemo = GetPhaseIndex(curWin.arrayPhases, demoPhase);

                                // if the index of selectedDemoPhase is less than the index of PHASE_CREATED warn the user
                                if (indexDemo < indexCreated)
                                {
                                    TaskDialog tdIndex = new TaskDialog("Invalid");
                                    tdIndex.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
                                    tdIndex.Title = "Phase to Demolish";
                                    tdIndex.TitleAutoPrefix = false;
                                    tdIndex.MainContent = "Invalid order of phases: an object cannot be demolished before it was created.";
                                    tdIndex.CommonButtons = TaskDialogCommonButtons.Close;

                                    TaskDialogResult tdIndexRes = tdIndex.Show();
                                }

                                // if the index is greater than, or equal to, the index of PHASE_CREATED
                                else if (indexDemo >= indexCreated)
                                {
                                    // set the demoPhaseID = selectedDemoPhase.Id
                                    demoPhaseId = demoPhase.Id;
                                }
                            }

                            // set the Phase_Demolished parameter
                            if (demoPhaseId != null)
                            {
                                paramPhaseDemo.Set(demoPhaseId);
                            }
                        }
                    }

                    else
                    {
                        TaskDialog tdNone = new TaskDialog("Invalid");
                        tdNone.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
                        tdNone.Title = "Phase to Demolish";
                        tdNone.TitleAutoPrefix = false;
                        tdNone.MainContent = "No elements were selected. Select some elements and try again.";
                        tdNone.CommonButtons = TaskDialogCommonButtons.Close;

                        TaskDialogResult tdNoneRes = tdNone.Show();
                    }

                    t.Commit();
                }
            }
        }

        #region Phases

        public static List<Element> getAllPhases(Document curDoc)
        {
            FilteredElementCollector curColl = new FilteredElementCollector(curDoc);
            curColl.OfCategory(BuiltInCategory.OST_Phases);

            return curColl.OrderBy(x => x.Name).ToList();
        }

        public static List<Element> getAllPhaseFilters(Document curDoc)
        {
            FilteredElementCollector curColl = new FilteredElementCollector(curDoc);
            curColl.OfClass(typeof(PhaseFilter));

            return curColl.OrderBy(x => x.Name).ToList();
        }

        public static Phase getPhaseByName(Document curDoc, string phaseName)
        {
            // get all phases
            List<Element> phaseList = getAllPhases(curDoc);

            foreach (Phase curPhase in phaseList)
            {
                if (curPhase.Name == phaseName)
                    return curPhase as Phase;
            }

            return null;

        }

        internal static int GetPhaseIndex(PhaseArray phaseArray, Phase targetPhase)
        {
            int phaseIndex = -1;

            if (phaseArray != null && targetPhase != null)
            {
                PhaseArrayIterator m_iterator = phaseArray.ForwardIterator();
                int m_index = 0;

                while (m_iterator.MoveNext())
                {
                    Phase curPhase = m_iterator.Current as Phase;

                    if (curPhase.Id == targetPhase.Id)
                    {
                        phaseIndex = m_index;
                        break;
                    }

                    m_index++;
                }
            }

            return phaseIndex;
        }


        #endregion

        #region Ribbon

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

        #endregion
    }
}