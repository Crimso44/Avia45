using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aik2
{
    public partial class fFilter : Form
    {
        public fFilter()
        {
            InitializeComponent();
        }

        public CheckedListBox ClCountries { get { return clCountries; }  }
        public CheckedListBox ClSources { get { return clSources; } }
        public CheckBox CVertYes { get { return cVertYes; }  }
        public CheckBox CUavYes { get { return cUavYes; } }
        public CheckBox CGlidYes { get { return cGlidYes; } }
        public CheckBox CLlYes { get { return cLlYes; } }
        public CheckBox CSinglYes { get { return cSinglYes; } }
        public CheckBox CProjYes { get { return cProjYes; } }
        public CheckBox CVertNo { get { return cVertNo; } }
        public CheckBox CUavNo { get { return cUavNo; } }
        public CheckBox CGlidNo { get { return cGlidNo; } }
        public CheckBox CLlNo { get { return cLlNo; } }
        public CheckBox CSinglNo { get { return cSinglNo; } }
        public CheckBox CProjNo { get { return cProjNo; } }

        public TextBox EWings { get { return eWings; } }
        public TextBox EEngines { get { return eEngines; } }
        public TextBox EText { get { return eText; } }
        public TextBox EText2 { get { return eText2; } }

        public CheckBox CWholeWords { get { return cWholeWords; } }
        public CheckBox CInText { get { return cInText; } }

        public NumericUpDown NYearFrom { get { return nYearFrom; } }
        public NumericUpDown NYearTo { get { return nYearTo; } }

        private void bCountriesSelect_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clCountries.Items.Count; i++) // loop to set all items checked or unchecked
            {
                clCountries.SetItemChecked(i, true);
            }
        }

        private void bCountriesUnselect_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clCountries.Items.Count; i++) // loop to set all items checked or unchecked
            {
                clCountries.SetItemChecked(i, false);
            }
        }
    }
}
