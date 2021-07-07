using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aik2
{
    public partial class fReport : Form
    {
        private AiKEntities _ctx;
        private string _imagesPath;

        public fReport(AiKEntities ctx, string imagesPath)
        {
            InitializeComponent();
            Mode = 0;

            _ctx = ctx;
            _imagesPath = imagesPath;
        }

        public ListBox ReportList { get { return lbReport; } }
        public Button SaveButton { get { return bSaveReport; } }
        public int Mode { get; set; }

        private void bSaveReport_Click(object sender, EventArgs e)
        {
            switch (Mode)
            {
                case 0:
                    _ctx.Database.ExecuteSqlCommand("Delete from Report");
                    _ctx.Database.ExecuteSqlCommand(
                        "Insert Into Report ([Mag],[IYear],[IMonth],[Source],[pics],[crafts],[uniq]) " +
                        "Select [Mag],[IYear],[IMonth],[Source],[pics],[crafts],[uniq] from vwReport");
                    _ctx.Database.Connection.Close();
                    _ctx.Database.Connection.Open();
                    MessageBox.Show("Ok");
                    break;
                case 1:
                    var zipPath = _imagesPath + "NonUsed.zip";
                    if (File.Exists(zipPath))
                    {
                        File.Delete(zipPath);
                    }
                    using(var zip = ZipFile.Open(zipPath, ZipArchiveMode.Create)) {
                        foreach(string item in lbReport.Items)
                        {
                            if (item[0] != '-')
                            {
                                var path = _imagesPath + item;
                                if (File.Exists(path))
                                    zip.CreateEntryFromFile(path, item);
                            }
                        }
                    }
                    foreach (string item in lbReport.Items)
                    {
                        if (item[0] != '-')
                        {
                            var path = _imagesPath + item;
                            if (File.Exists(path))
                                File.Delete(path);
                        }
                    }
                    MessageBox.Show("Ok");
                    break;
            }
        }

        private void lbReport_DoubleClick(object sender, EventArgs e)
        {
            if (Mode == 1 && lbReport.SelectedIndex >= 0)
            {
                var path = _imagesPath + lbReport.Items[lbReport.SelectedIndex];
                System.Diagnostics.Process.Start(path);
            }
        }

        private void bCopyText_Click(object sender, EventArgs e)
        {
            var txt = "";
            for (var i = 0; i < lbReport.Items.Count; i++)
            {
                txt += lbReport.Items[i] + "\n";
            }
            Clipboard.SetText(txt);
            MessageBox.Show("Скопировано");
        }
    }
}
