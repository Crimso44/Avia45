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
    public partial class fReport : Form
    {
        public fReport()
        {
            InitializeComponent();
        }

        public ListBox ReportList { get { return lbReport; } }
    }
}
