using DemolishToPhase.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DemolishToPhase.Forms
{
    /// <summary>
    /// Interaction logic for frmDemolishToPhase.xaml
    /// </summary>
    public partial class frmDemolishToPhase : Window
    {
        public PhaseArray arrayPhases;

        public string selectedPhase;

        public frmDemolishToPhase(Document curDoc)
        {
            // get all the phases in the project
            arrayPhases = curDoc.Phases;

            // create an empty list to hold the phase names
            List<string> phases = new List<string>();

            phases.Add("None");

            // loop through the phases
            foreach (Phase curPhase in arrayPhases)
            {
                // add the phase name to the list
                phases.Add(curPhase.Name);
            }

            InitializeComponent();

            foreach (string phase in phases)
            {
                RadioButton rb = new RadioButton() { Content = phase, Height = 25, Width = 120 };
                sp.Children.Add(rb);
                rb.Checked += new RoutedEventHandler(rb_Checked);
                rb.Unchecked += new RoutedEventHandler(rb_Unchecked);
            }
        }

        void rb_Unchecked(object sender, RoutedEventArgs e)
        {
            selectedPhase = "";
        }

        void rb_Checked(object sender, RoutedEventArgs e)
        {
            selectedPhase = (sender as RadioButton).Content.ToString();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            Utils.ShowForm = true;
            this.Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Utils.ShowForm = false;
            this.Close();
        }
    }
}