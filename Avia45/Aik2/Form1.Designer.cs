namespace Aik2
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageArts = new System.Windows.Forms.TabPage();
            this.pArtTable = new System.Windows.Forms.Panel();
            this.gridArt = new SourceGrid.Grid();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.chArtSortYear = new System.Windows.Forms.RadioButton();
            this.chArtSortSerie = new System.Windows.Forms.RadioButton();
            this.chArtSortAuthor = new System.Windows.Forms.RadioButton();
            this.tabPageCrafts = new System.Windows.Forms.TabPage();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.pCraftTable = new System.Windows.Forms.Panel();
            this.gridCraft = new SourceGrid.Grid();
            this.pCraftText = new System.Windows.Forms.Panel();
            this.panel9 = new System.Windows.Forms.Panel();
            this.edCraftText = new System.Windows.Forms.RichTextBox();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.pCraftPic = new System.Windows.Forms.Panel();
            this.imgCraftPic = new System.Windows.Forms.PictureBox();
            this.sbCraftPics = new System.Windows.Forms.HScrollBar();
            this.panel4 = new System.Windows.Forms.Panel();
            this.tabPagePics = new System.Windows.Forms.TabPage();
            this.pPicTable = new System.Windows.Forms.Panel();
            this.gridPic = new SourceGrid.Grid();
            this.panel10 = new System.Windows.Forms.Panel();
            this.bLockPic = new System.Windows.Forms.Button();
            this.splitter3 = new System.Windows.Forms.Splitter();
            this.pPicImg = new System.Windows.Forms.Panel();
            this.splitter5 = new System.Windows.Forms.Splitter();
            this.picPicImage = new System.Windows.Forms.PictureBox();
            this.pLinkImage = new System.Windows.Forms.Panel();
            this.picLinkImage = new System.Windows.Forms.PictureBox();
            this.splitter4 = new System.Windows.Forms.Splitter();
            this.pPicText = new System.Windows.Forms.Panel();
            this.splitter6 = new System.Windows.Forms.Splitter();
            this.edPicText = new System.Windows.Forms.RichTextBox();
            this.pLinkTable = new System.Windows.Forms.Panel();
            this.gridLink = new SourceGrid.Grid();
            this.panel5 = new System.Windows.Forms.Panel();
            this.lCraft = new System.Windows.Forms.Label();
            this.lArt = new System.Windows.Forms.Label();
            this.chPicSelCraft = new System.Windows.Forms.CheckBox();
            this.chPicSelArt = new System.Windows.Forms.CheckBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.miscToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.calcWordsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lInfo = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.tabControl1.SuspendLayout();
            this.tabPageArts.SuspendLayout();
            this.pArtTable.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPageCrafts.SuspendLayout();
            this.pCraftTable.SuspendLayout();
            this.pCraftText.SuspendLayout();
            this.panel9.SuspendLayout();
            this.pCraftPic.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgCraftPic)).BeginInit();
            this.tabPagePics.SuspendLayout();
            this.pPicTable.SuspendLayout();
            this.panel10.SuspendLayout();
            this.pPicImg.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPicImage)).BeginInit();
            this.pLinkImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLinkImage)).BeginInit();
            this.pPicText.SuspendLayout();
            this.pLinkTable.SuspendLayout();
            this.panel5.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel7.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageArts);
            this.tabControl1.Controls.Add(this.tabPageCrafts);
            this.tabControl1.Controls.Add(this.tabPagePics);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(800, 400);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPageArts
            // 
            this.tabPageArts.Controls.Add(this.pArtTable);
            this.tabPageArts.Controls.Add(this.panel1);
            this.tabPageArts.Location = new System.Drawing.Point(4, 22);
            this.tabPageArts.Name = "tabPageArts";
            this.tabPageArts.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageArts.Size = new System.Drawing.Size(792, 374);
            this.tabPageArts.TabIndex = 0;
            this.tabPageArts.Text = "Arts";
            this.tabPageArts.UseVisualStyleBackColor = true;
            // 
            // pArtTable
            // 
            this.pArtTable.Controls.Add(this.gridArt);
            this.pArtTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pArtTable.Location = new System.Drawing.Point(3, 31);
            this.pArtTable.Name = "pArtTable";
            this.pArtTable.Size = new System.Drawing.Size(786, 340);
            this.pArtTable.TabIndex = 3;
            // 
            // gridArt
            // 
            this.gridArt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridArt.EnableSort = false;
            this.gridArt.Location = new System.Drawing.Point(0, 0);
            this.gridArt.Name = "gridArt";
            this.gridArt.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.gridArt.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.gridArt.Size = new System.Drawing.Size(786, 340);
            this.gridArt.TabIndex = 4;
            this.gridArt.TabStop = true;
            this.gridArt.ToolTipText = "";
            this.gridArt.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.gridArt_PreviewKeyDown);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.chArtSortYear);
            this.panel1.Controls.Add(this.chArtSortSerie);
            this.panel1.Controls.Add(this.chArtSortAuthor);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(786, 28);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Sort:";
            // 
            // chArtSortYear
            // 
            this.chArtSortYear.Appearance = System.Windows.Forms.Appearance.Button;
            this.chArtSortYear.AutoSize = true;
            this.chArtSortYear.Checked = true;
            this.chArtSortYear.Location = new System.Drawing.Point(145, 1);
            this.chArtSortYear.Name = "chArtSortYear";
            this.chArtSortYear.Size = new System.Drawing.Size(39, 23);
            this.chArtSortYear.TabIndex = 2;
            this.chArtSortYear.TabStop = true;
            this.chArtSortYear.Text = "Year";
            this.chArtSortYear.UseVisualStyleBackColor = true;
            this.chArtSortYear.Click += new System.EventHandler(this.chArtSortYear_Click);
            // 
            // chArtSortSerie
            // 
            this.chArtSortSerie.Appearance = System.Windows.Forms.Appearance.Button;
            this.chArtSortSerie.AutoSize = true;
            this.chArtSortSerie.Location = new System.Drawing.Point(98, 1);
            this.chArtSortSerie.Name = "chArtSortSerie";
            this.chArtSortSerie.Size = new System.Drawing.Size(41, 23);
            this.chArtSortSerie.TabIndex = 1;
            this.chArtSortSerie.Text = "Serie";
            this.chArtSortSerie.UseVisualStyleBackColor = true;
            this.chArtSortSerie.Click += new System.EventHandler(this.chArtSortYear_Click);
            // 
            // chArtSortAuthor
            // 
            this.chArtSortAuthor.Appearance = System.Windows.Forms.Appearance.Button;
            this.chArtSortAuthor.AutoSize = true;
            this.chArtSortAuthor.Location = new System.Drawing.Point(44, 1);
            this.chArtSortAuthor.Name = "chArtSortAuthor";
            this.chArtSortAuthor.Size = new System.Drawing.Size(48, 23);
            this.chArtSortAuthor.TabIndex = 0;
            this.chArtSortAuthor.Text = "Author";
            this.chArtSortAuthor.UseVisualStyleBackColor = true;
            this.chArtSortAuthor.Click += new System.EventHandler(this.chArtSortYear_Click);
            // 
            // tabPageCrafts
            // 
            this.tabPageCrafts.Controls.Add(this.splitter1);
            this.tabPageCrafts.Controls.Add(this.pCraftTable);
            this.tabPageCrafts.Controls.Add(this.pCraftText);
            this.tabPageCrafts.Controls.Add(this.panel4);
            this.tabPageCrafts.Location = new System.Drawing.Point(4, 22);
            this.tabPageCrafts.Name = "tabPageCrafts";
            this.tabPageCrafts.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCrafts.Size = new System.Drawing.Size(792, 374);
            this.tabPageCrafts.TabIndex = 1;
            this.tabPageCrafts.Text = "Crafts";
            this.tabPageCrafts.UseVisualStyleBackColor = true;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(586, 31);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 340);
            this.splitter1.TabIndex = 0;
            this.splitter1.TabStop = false;
            // 
            // pCraftTable
            // 
            this.pCraftTable.Controls.Add(this.gridCraft);
            this.pCraftTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pCraftTable.Location = new System.Drawing.Point(3, 31);
            this.pCraftTable.Name = "pCraftTable";
            this.pCraftTable.Size = new System.Drawing.Size(586, 340);
            this.pCraftTable.TabIndex = 2;
            // 
            // gridCraft
            // 
            this.gridCraft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridCraft.EnableSort = false;
            this.gridCraft.Location = new System.Drawing.Point(0, 0);
            this.gridCraft.Name = "gridCraft";
            this.gridCraft.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.gridCraft.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.gridCraft.Size = new System.Drawing.Size(586, 340);
            this.gridCraft.TabIndex = 0;
            this.gridCraft.TabStop = true;
            this.gridCraft.ToolTipText = "";
            this.gridCraft.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.gridCraft_PreviewKeyDown);
            // 
            // pCraftText
            // 
            this.pCraftText.Controls.Add(this.panel9);
            this.pCraftText.Controls.Add(this.splitter2);
            this.pCraftText.Controls.Add(this.pCraftPic);
            this.pCraftText.Dock = System.Windows.Forms.DockStyle.Right;
            this.pCraftText.Location = new System.Drawing.Point(589, 31);
            this.pCraftText.Name = "pCraftText";
            this.pCraftText.Size = new System.Drawing.Size(200, 340);
            this.pCraftText.TabIndex = 1;
            this.pCraftText.Resize += new System.EventHandler(this.pCraftText_Resize);
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.edCraftText);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel9.Location = new System.Drawing.Point(0, 0);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(200, 237);
            this.panel9.TabIndex = 3;
            // 
            // edCraftText
            // 
            this.edCraftText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.edCraftText.Location = new System.Drawing.Point(0, 0);
            this.edCraftText.Margin = new System.Windows.Forms.Padding(1);
            this.edCraftText.Name = "edCraftText";
            this.edCraftText.Size = new System.Drawing.Size(200, 237);
            this.edCraftText.TabIndex = 0;
            this.edCraftText.Text = "";
            this.edCraftText.TextChanged += new System.EventHandler(this.edCraftText_TextChanged);
            this.edCraftText.DoubleClick += new System.EventHandler(this.edPicText_DoubleClick);
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter2.Location = new System.Drawing.Point(0, 237);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(200, 3);
            this.splitter2.TabIndex = 2;
            this.splitter2.TabStop = false;
            // 
            // pCraftPic
            // 
            this.pCraftPic.Controls.Add(this.imgCraftPic);
            this.pCraftPic.Controls.Add(this.sbCraftPics);
            this.pCraftPic.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pCraftPic.Location = new System.Drawing.Point(0, 240);
            this.pCraftPic.Name = "pCraftPic";
            this.pCraftPic.Size = new System.Drawing.Size(200, 100);
            this.pCraftPic.TabIndex = 1;
            this.pCraftPic.Resize += new System.EventHandler(this.pCraftPic_Resize);
            // 
            // imgCraftPic
            // 
            this.imgCraftPic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imgCraftPic.Location = new System.Drawing.Point(0, 0);
            this.imgCraftPic.Name = "imgCraftPic";
            this.imgCraftPic.Size = new System.Drawing.Size(200, 83);
            this.imgCraftPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imgCraftPic.TabIndex = 1;
            this.imgCraftPic.TabStop = false;
            // 
            // sbCraftPics
            // 
            this.sbCraftPics.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.sbCraftPics.LargeChange = 1;
            this.sbCraftPics.Location = new System.Drawing.Point(0, 83);
            this.sbCraftPics.Name = "sbCraftPics";
            this.sbCraftPics.Size = new System.Drawing.Size(200, 17);
            this.sbCraftPics.TabIndex = 2;
            this.sbCraftPics.ValueChanged += new System.EventHandler(this.sbCraftPics_ValueChanged);
            // 
            // panel4
            // 
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(3, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(786, 28);
            this.panel4.TabIndex = 0;
            // 
            // tabPagePics
            // 
            this.tabPagePics.Controls.Add(this.pPicTable);
            this.tabPagePics.Controls.Add(this.splitter3);
            this.tabPagePics.Controls.Add(this.pPicImg);
            this.tabPagePics.Controls.Add(this.splitter4);
            this.tabPagePics.Controls.Add(this.pPicText);
            this.tabPagePics.Controls.Add(this.panel5);
            this.tabPagePics.Location = new System.Drawing.Point(4, 22);
            this.tabPagePics.Name = "tabPagePics";
            this.tabPagePics.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePics.Size = new System.Drawing.Size(792, 374);
            this.tabPagePics.TabIndex = 2;
            this.tabPagePics.Text = "Pics";
            this.tabPagePics.UseVisualStyleBackColor = true;
            // 
            // pPicTable
            // 
            this.pPicTable.Controls.Add(this.gridPic);
            this.pPicTable.Controls.Add(this.panel10);
            this.pPicTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pPicTable.Location = new System.Drawing.Point(3, 57);
            this.pPicTable.Name = "pPicTable";
            this.pPicTable.Size = new System.Drawing.Size(786, 208);
            this.pPicTable.TabIndex = 3;
            // 
            // gridPic
            // 
            this.gridPic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPic.EnableSort = false;
            this.gridPic.Location = new System.Drawing.Point(0, 0);
            this.gridPic.Name = "gridPic";
            this.gridPic.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.gridPic.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.gridPic.Size = new System.Drawing.Size(755, 208);
            this.gridPic.TabIndex = 0;
            this.gridPic.TabStop = true;
            this.gridPic.ToolTipText = "";
            this.gridPic.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.gridPic_PreviewKeyDown);
            // 
            // panel10
            // 
            this.panel10.Controls.Add(this.bLockPic);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel10.Location = new System.Drawing.Point(755, 0);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(31, 208);
            this.panel10.TabIndex = 4;
            // 
            // bLockPic
            // 
            this.bLockPic.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bLockPic.Image = ((System.Drawing.Image)(resources.GetObject("bLockPic.Image")));
            this.bLockPic.Location = new System.Drawing.Point(4, 164);
            this.bLockPic.Name = "bLockPic";
            this.bLockPic.Size = new System.Drawing.Size(24, 23);
            this.bLockPic.TabIndex = 0;
            this.bLockPic.UseVisualStyleBackColor = true;
            this.bLockPic.Click += new System.EventHandler(this.bLockPic_Click);
            // 
            // splitter3
            // 
            this.splitter3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter3.Location = new System.Drawing.Point(3, 265);
            this.splitter3.Name = "splitter3";
            this.splitter3.Size = new System.Drawing.Size(786, 3);
            this.splitter3.TabIndex = 4;
            this.splitter3.TabStop = false;
            // 
            // pPicImg
            // 
            this.pPicImg.Controls.Add(this.splitter5);
            this.pPicImg.Controls.Add(this.picPicImage);
            this.pPicImg.Controls.Add(this.pLinkImage);
            this.pPicImg.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pPicImg.Location = new System.Drawing.Point(3, 268);
            this.pPicImg.Name = "pPicImg";
            this.pPicImg.Size = new System.Drawing.Size(786, 50);
            this.pPicImg.TabIndex = 2;
            this.pPicImg.Resize += new System.EventHandler(this.pPicImg_Resize);
            // 
            // splitter5
            // 
            this.splitter5.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter5.Location = new System.Drawing.Point(583, 0);
            this.splitter5.Name = "splitter5";
            this.splitter5.Size = new System.Drawing.Size(3, 50);
            this.splitter5.TabIndex = 2;
            this.splitter5.TabStop = false;
            // 
            // picPicImage
            // 
            this.picPicImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picPicImage.Location = new System.Drawing.Point(0, 0);
            this.picPicImage.Name = "picPicImage";
            this.picPicImage.Size = new System.Drawing.Size(586, 50);
            this.picPicImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picPicImage.TabIndex = 0;
            this.picPicImage.TabStop = false;
            // 
            // pLinkImage
            // 
            this.pLinkImage.Controls.Add(this.picLinkImage);
            this.pLinkImage.Dock = System.Windows.Forms.DockStyle.Right;
            this.pLinkImage.Location = new System.Drawing.Point(586, 0);
            this.pLinkImage.Name = "pLinkImage";
            this.pLinkImage.Size = new System.Drawing.Size(200, 50);
            this.pLinkImage.TabIndex = 1;
            this.pLinkImage.Resize += new System.EventHandler(this.pLinkImage_Resize);
            // 
            // picLinkImage
            // 
            this.picLinkImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picLinkImage.Location = new System.Drawing.Point(0, 0);
            this.picLinkImage.Name = "picLinkImage";
            this.picLinkImage.Size = new System.Drawing.Size(200, 50);
            this.picLinkImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLinkImage.TabIndex = 0;
            this.picLinkImage.TabStop = false;
            // 
            // splitter4
            // 
            this.splitter4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter4.Location = new System.Drawing.Point(3, 318);
            this.splitter4.Name = "splitter4";
            this.splitter4.Size = new System.Drawing.Size(786, 3);
            this.splitter4.TabIndex = 5;
            this.splitter4.TabStop = false;
            // 
            // pPicText
            // 
            this.pPicText.Controls.Add(this.splitter6);
            this.pPicText.Controls.Add(this.edPicText);
            this.pPicText.Controls.Add(this.pLinkTable);
            this.pPicText.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pPicText.Location = new System.Drawing.Point(3, 321);
            this.pPicText.Name = "pPicText";
            this.pPicText.Size = new System.Drawing.Size(786, 50);
            this.pPicText.TabIndex = 1;
            this.pPicText.Resize += new System.EventHandler(this.pPicText_Resize);
            // 
            // splitter6
            // 
            this.splitter6.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter6.Location = new System.Drawing.Point(583, 0);
            this.splitter6.Name = "splitter6";
            this.splitter6.Size = new System.Drawing.Size(3, 50);
            this.splitter6.TabIndex = 2;
            this.splitter6.TabStop = false;
            // 
            // edPicText
            // 
            this.edPicText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.edPicText.Location = new System.Drawing.Point(0, 0);
            this.edPicText.Margin = new System.Windows.Forms.Padding(1);
            this.edPicText.Name = "edPicText";
            this.edPicText.Size = new System.Drawing.Size(586, 50);
            this.edPicText.TabIndex = 0;
            this.edPicText.Text = "";
            this.edPicText.DoubleClick += new System.EventHandler(this.edPicText_DoubleClick);
            // 
            // pLinkTable
            // 
            this.pLinkTable.Controls.Add(this.gridLink);
            this.pLinkTable.Dock = System.Windows.Forms.DockStyle.Right;
            this.pLinkTable.Location = new System.Drawing.Point(586, 0);
            this.pLinkTable.Name = "pLinkTable";
            this.pLinkTable.Size = new System.Drawing.Size(200, 50);
            this.pLinkTable.TabIndex = 1;
            this.pLinkTable.Resize += new System.EventHandler(this.pLinkTable_Resize);
            // 
            // gridLink
            // 
            this.gridLink.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridLink.EnableSort = true;
            this.gridLink.Location = new System.Drawing.Point(0, 0);
            this.gridLink.Name = "gridLink";
            this.gridLink.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.gridLink.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.gridLink.Size = new System.Drawing.Size(200, 50);
            this.gridLink.TabIndex = 0;
            this.gridLink.TabStop = true;
            this.gridLink.ToolTipText = "";
            this.gridLink.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.gridLink_PreviewKeyDown);
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.lCraft);
            this.panel5.Controls.Add(this.lArt);
            this.panel5.Controls.Add(this.chPicSelCraft);
            this.panel5.Controls.Add(this.chPicSelArt);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(3, 3);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(786, 54);
            this.panel5.TabIndex = 0;
            // 
            // lCraft
            // 
            this.lCraft.AutoSize = true;
            this.lCraft.Location = new System.Drawing.Point(124, 17);
            this.lCraft.Name = "lCraft";
            this.lCraft.Size = new System.Drawing.Size(10, 13);
            this.lCraft.TabIndex = 3;
            this.lCraft.Text = ".";
            // 
            // lArt
            // 
            this.lArt.AutoSize = true;
            this.lArt.Location = new System.Drawing.Point(124, 4);
            this.lArt.Name = "lArt";
            this.lArt.Size = new System.Drawing.Size(10, 13);
            this.lArt.TabIndex = 2;
            this.lArt.Text = ".";
            // 
            // chPicSelCraft
            // 
            this.chPicSelCraft.Appearance = System.Windows.Forms.Appearance.Button;
            this.chPicSelCraft.AutoSize = true;
            this.chPicSelCraft.Checked = true;
            this.chPicSelCraft.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chPicSelCraft.Location = new System.Drawing.Point(42, 4);
            this.chPicSelCraft.Name = "chPicSelCraft";
            this.chPicSelCraft.Size = new System.Drawing.Size(39, 23);
            this.chPicSelCraft.TabIndex = 1;
            this.chPicSelCraft.Text = "Craft";
            this.chPicSelCraft.UseVisualStyleBackColor = true;
            this.chPicSelCraft.Click += new System.EventHandler(this.chPicSelArt_Click);
            // 
            // chPicSelArt
            // 
            this.chPicSelArt.Appearance = System.Windows.Forms.Appearance.Button;
            this.chPicSelArt.AutoSize = true;
            this.chPicSelArt.Location = new System.Drawing.Point(6, 4);
            this.chPicSelArt.Name = "chPicSelArt";
            this.chPicSelArt.Size = new System.Drawing.Size(30, 23);
            this.chPicSelArt.TabIndex = 0;
            this.chPicSelArt.Text = "Art";
            this.chPicSelArt.UseVisualStyleBackColor = true;
            this.chPicSelArt.Click += new System.EventHandler(this.chPicSelArt_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miscToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.Stretch = false;
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // miscToolStripMenuItem
            // 
            this.miscToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.calcWordsToolStripMenuItem});
            this.miscToolStripMenuItem.Name = "miscToolStripMenuItem";
            this.miscToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.miscToolStripMenuItem.Text = "Misc";
            // 
            // calcWordsToolStripMenuItem
            // 
            this.calcWordsToolStripMenuItem.Name = "calcWordsToolStripMenuItem";
            this.calcWordsToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.calcWordsToolStripMenuItem.Text = "Calc words";
            this.calcWordsToolStripMenuItem.Click += new System.EventHandler(this.calcWordsToolStripMenuItem_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lInfo);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 424);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(800, 26);
            this.panel2.TabIndex = 3;
            // 
            // lInfo
            // 
            this.lInfo.AutoSize = true;
            this.lInfo.Location = new System.Drawing.Point(4, 8);
            this.lInfo.Name = "lInfo";
            this.lInfo.Size = new System.Drawing.Size(0, 13);
            this.lInfo.TabIndex = 0;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.tabControl1);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(0, 24);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(800, 400);
            this.panel7.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel7);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.panel2);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.tabControl1.ResumeLayout(false);
            this.tabPageArts.ResumeLayout(false);
            this.pArtTable.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabPageCrafts.ResumeLayout(false);
            this.pCraftTable.ResumeLayout(false);
            this.pCraftText.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            this.pCraftPic.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imgCraftPic)).EndInit();
            this.tabPagePics.ResumeLayout(false);
            this.pPicTable.ResumeLayout(false);
            this.panel10.ResumeLayout(false);
            this.pPicImg.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picPicImage)).EndInit();
            this.pLinkImage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picLinkImage)).EndInit();
            this.pPicText.ResumeLayout(false);
            this.pLinkTable.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageArts;
        private System.Windows.Forms.TabPage tabPageCrafts;
        private System.Windows.Forms.TabPage tabPagePics;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pArtTable;
        private SourceGrid.Grid gridArt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton chArtSortYear;
        private System.Windows.Forms.RadioButton chArtSortSerie;
        private System.Windows.Forms.RadioButton chArtSortAuthor;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lInfo;
        private System.Windows.Forms.Panel pCraftTable;
        private System.Windows.Forms.Panel pCraftText;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Splitter splitter1;
        private SourceGrid.Grid gridCraft;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.RichTextBox edCraftText;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.Panel pCraftPic;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem miscToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem calcWordsToolStripMenuItem;
        private System.Windows.Forms.Panel pPicTable;
        private System.Windows.Forms.Panel pPicImg;
        private System.Windows.Forms.Panel pPicText;
        private System.Windows.Forms.Panel panel5;
        private SourceGrid.Grid gridPic;
        private System.Windows.Forms.Splitter splitter3;
        private System.Windows.Forms.Splitter splitter4;
        private System.Windows.Forms.RichTextBox edPicText;
        private System.Windows.Forms.CheckBox chPicSelCraft;
        private System.Windows.Forms.CheckBox chPicSelArt;
        private System.Windows.Forms.PictureBox picPicImage;
        private System.Windows.Forms.Label lCraft;
        private System.Windows.Forms.Label lArt;
        private System.Windows.Forms.Splitter splitter5;
        private System.Windows.Forms.Panel pLinkImage;
        private System.Windows.Forms.Splitter splitter6;
        private System.Windows.Forms.Panel pLinkTable;
        private SourceGrid.Grid gridLink;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Button bLockPic;
        private System.Windows.Forms.PictureBox picLinkImage;
        private System.Windows.Forms.PictureBox imgCraftPic;
        private System.Windows.Forms.HScrollBar sbCraftPics;
    }
}

