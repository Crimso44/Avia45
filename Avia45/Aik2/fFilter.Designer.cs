namespace Aik2
{
    partial class fFilter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fFilter));
            this.bCountriesSelect = new System.Windows.Forms.Button();
            this.bCountriesUnselect = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.clCountries = new System.Windows.Forms.CheckedListBox();
            this.cVertYes = new System.Windows.Forms.CheckBox();
            this.cUavYes = new System.Windows.Forms.CheckBox();
            this.cGlidYes = new System.Windows.Forms.CheckBox();
            this.cLlYes = new System.Windows.Forms.CheckBox();
            this.cSinglYes = new System.Windows.Forms.CheckBox();
            this.cProjYes = new System.Windows.Forms.CheckBox();
            this.cVertNo = new System.Windows.Forms.CheckBox();
            this.cUavNo = new System.Windows.Forms.CheckBox();
            this.cGlidNo = new System.Windows.Forms.CheckBox();
            this.cLlNo = new System.Windows.Forms.CheckBox();
            this.cSinglNo = new System.Windows.Forms.CheckBox();
            this.cProjNo = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.eWings = new System.Windows.Forms.TextBox();
            this.eEngines = new System.Windows.Forms.TextBox();
            this.eText = new System.Windows.Forms.TextBox();
            this.eText2 = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.cWholeWords = new System.Windows.Forms.CheckBox();
            this.nYearFrom = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.nYearTo = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.clSources = new System.Windows.Forms.CheckedListBox();
            this.cInText = new System.Windows.Forms.CheckBox();
            this.cCaseSensitive = new System.Windows.Forms.CheckBox();
            this.bSrcR = new System.Windows.Forms.Button();
            this.bSrcW = new System.Windows.Forms.Button();
            this.bSrcRW = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nYearFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nYearTo)).BeginInit();
            this.SuspendLayout();
            // 
            // bCountriesSelect
            // 
            this.bCountriesSelect.Image = ((System.Drawing.Image)(resources.GetObject("bCountriesSelect.Image")));
            this.bCountriesSelect.Location = new System.Drawing.Point(13, 308);
            this.bCountriesSelect.Name = "bCountriesSelect";
            this.bCountriesSelect.Size = new System.Drawing.Size(26, 23);
            this.bCountriesSelect.TabIndex = 1;
            this.bCountriesSelect.UseVisualStyleBackColor = true;
            this.bCountriesSelect.Click += new System.EventHandler(this.bCountriesSelect_Click);
            // 
            // bCountriesUnselect
            // 
            this.bCountriesUnselect.Image = ((System.Drawing.Image)(resources.GetObject("bCountriesUnselect.Image")));
            this.bCountriesUnselect.Location = new System.Drawing.Point(115, 308);
            this.bCountriesUnselect.Name = "bCountriesUnselect";
            this.bCountriesUnselect.Size = new System.Drawing.Size(26, 23);
            this.bCountriesUnselect.TabIndex = 2;
            this.bCountriesUnselect.UseVisualStyleBackColor = true;
            this.bCountriesUnselect.Click += new System.EventHandler(this.bCountriesUnselect_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 335);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(521, 41);
            this.panel1.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(434, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // clCountries
            // 
            this.clCountries.CheckOnClick = true;
            this.clCountries.FormattingEnabled = true;
            this.clCountries.Location = new System.Drawing.Point(13, 13);
            this.clCountries.Name = "clCountries";
            this.clCountries.Size = new System.Drawing.Size(130, 289);
            this.clCountries.TabIndex = 4;
            // 
            // cVertYes
            // 
            this.cVertYes.AutoSize = true;
            this.cVertYes.Checked = true;
            this.cVertYes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cVertYes.Location = new System.Drawing.Point(194, 31);
            this.cVertYes.Name = "cVertYes";
            this.cVertYes.Size = new System.Drawing.Size(15, 14);
            this.cVertYes.TabIndex = 5;
            this.cVertYes.UseVisualStyleBackColor = true;
            // 
            // cUavYes
            // 
            this.cUavYes.AutoSize = true;
            this.cUavYes.Checked = true;
            this.cUavYes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cUavYes.Location = new System.Drawing.Point(224, 31);
            this.cUavYes.Name = "cUavYes";
            this.cUavYes.Size = new System.Drawing.Size(15, 14);
            this.cUavYes.TabIndex = 6;
            this.cUavYes.UseVisualStyleBackColor = true;
            // 
            // cGlidYes
            // 
            this.cGlidYes.AutoSize = true;
            this.cGlidYes.Checked = true;
            this.cGlidYes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cGlidYes.Location = new System.Drawing.Point(254, 31);
            this.cGlidYes.Name = "cGlidYes";
            this.cGlidYes.Size = new System.Drawing.Size(15, 14);
            this.cGlidYes.TabIndex = 7;
            this.cGlidYes.UseVisualStyleBackColor = true;
            // 
            // cLlYes
            // 
            this.cLlYes.AutoSize = true;
            this.cLlYes.Checked = true;
            this.cLlYes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cLlYes.Location = new System.Drawing.Point(284, 31);
            this.cLlYes.Name = "cLlYes";
            this.cLlYes.Size = new System.Drawing.Size(15, 14);
            this.cLlYes.TabIndex = 8;
            this.cLlYes.UseVisualStyleBackColor = true;
            // 
            // cSinglYes
            // 
            this.cSinglYes.AutoSize = true;
            this.cSinglYes.Checked = true;
            this.cSinglYes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cSinglYes.Location = new System.Drawing.Point(314, 31);
            this.cSinglYes.Name = "cSinglYes";
            this.cSinglYes.Size = new System.Drawing.Size(15, 14);
            this.cSinglYes.TabIndex = 9;
            this.cSinglYes.UseVisualStyleBackColor = true;
            // 
            // cProjYes
            // 
            this.cProjYes.AutoSize = true;
            this.cProjYes.Checked = true;
            this.cProjYes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cProjYes.Location = new System.Drawing.Point(344, 31);
            this.cProjYes.Name = "cProjYes";
            this.cProjYes.Size = new System.Drawing.Size(15, 14);
            this.cProjYes.TabIndex = 10;
            this.cProjYes.UseVisualStyleBackColor = true;
            // 
            // cVertNo
            // 
            this.cVertNo.AutoSize = true;
            this.cVertNo.Checked = true;
            this.cVertNo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cVertNo.Location = new System.Drawing.Point(194, 51);
            this.cVertNo.Name = "cVertNo";
            this.cVertNo.Size = new System.Drawing.Size(15, 14);
            this.cVertNo.TabIndex = 11;
            this.cVertNo.UseVisualStyleBackColor = true;
            // 
            // cUavNo
            // 
            this.cUavNo.AutoSize = true;
            this.cUavNo.Checked = true;
            this.cUavNo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cUavNo.Location = new System.Drawing.Point(224, 51);
            this.cUavNo.Name = "cUavNo";
            this.cUavNo.Size = new System.Drawing.Size(15, 14);
            this.cUavNo.TabIndex = 12;
            this.cUavNo.UseVisualStyleBackColor = true;
            // 
            // cGlidNo
            // 
            this.cGlidNo.AutoSize = true;
            this.cGlidNo.Checked = true;
            this.cGlidNo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cGlidNo.Location = new System.Drawing.Point(254, 51);
            this.cGlidNo.Name = "cGlidNo";
            this.cGlidNo.Size = new System.Drawing.Size(15, 14);
            this.cGlidNo.TabIndex = 13;
            this.cGlidNo.UseVisualStyleBackColor = true;
            // 
            // cLlNo
            // 
            this.cLlNo.AutoSize = true;
            this.cLlNo.Checked = true;
            this.cLlNo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cLlNo.Location = new System.Drawing.Point(284, 51);
            this.cLlNo.Name = "cLlNo";
            this.cLlNo.Size = new System.Drawing.Size(15, 14);
            this.cLlNo.TabIndex = 14;
            this.cLlNo.UseVisualStyleBackColor = true;
            // 
            // cSinglNo
            // 
            this.cSinglNo.AutoSize = true;
            this.cSinglNo.Checked = true;
            this.cSinglNo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cSinglNo.Location = new System.Drawing.Point(314, 51);
            this.cSinglNo.Name = "cSinglNo";
            this.cSinglNo.Size = new System.Drawing.Size(15, 14);
            this.cSinglNo.TabIndex = 15;
            this.cSinglNo.UseVisualStyleBackColor = true;
            // 
            // cProjNo
            // 
            this.cProjNo.AutoSize = true;
            this.cProjNo.Checked = true;
            this.cProjNo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cProjNo.Location = new System.Drawing.Point(344, 51);
            this.cProjNo.Name = "cProjNo";
            this.cProjNo.Size = new System.Drawing.Size(15, 14);
            this.cProjNo.TabIndex = 16;
            this.cProjNo.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(186, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Vert";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(216, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "UAV";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(246, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "Glider";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(276, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "LL";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(306, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 13);
            this.label5.TabIndex = 21;
            this.label5.Text = "Single";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(336, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 13);
            this.label6.TabIndex = 22;
            this.label6.Text = "Proj";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(161, 31);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(22, 13);
            this.label7.TabIndex = 23;
            this.label7.Text = "Да";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(161, 51);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(26, 13);
            this.label8.TabIndex = 24;
            this.label8.Text = "Нет";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(161, 84);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 13);
            this.label9.TabIndex = 25;
            this.label9.Text = "Wings";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(161, 112);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(45, 13);
            this.label10.TabIndex = 26;
            this.label10.Text = "Engines";
            // 
            // eWings
            // 
            this.eWings.Location = new System.Drawing.Point(219, 81);
            this.eWings.Name = "eWings";
            this.eWings.Size = new System.Drawing.Size(140, 20);
            this.eWings.TabIndex = 27;
            // 
            // eEngines
            // 
            this.eEngines.Location = new System.Drawing.Point(219, 109);
            this.eEngines.Name = "eEngines";
            this.eEngines.Size = new System.Drawing.Size(140, 20);
            this.eEngines.TabIndex = 28;
            // 
            // eText
            // 
            this.eText.Location = new System.Drawing.Point(195, 163);
            this.eText.Name = "eText";
            this.eText.Size = new System.Drawing.Size(164, 20);
            this.eText.TabIndex = 29;
            // 
            // eText2
            // 
            this.eText2.Location = new System.Drawing.Point(194, 199);
            this.eText2.Name = "eText2";
            this.eText2.Size = new System.Drawing.Size(165, 20);
            this.eText2.TabIndex = 30;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(161, 166);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(28, 13);
            this.label11.TabIndex = 31;
            this.label11.Text = "Text";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(191, 183);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(16, 13);
            this.label12.TabIndex = 32;
            this.label12.Text = "or";
            // 
            // cWholeWords
            // 
            this.cWholeWords.AutoSize = true;
            this.cWholeWords.Location = new System.Drawing.Point(382, 162);
            this.cWholeWords.Name = "cWholeWords";
            this.cWholeWords.Size = new System.Drawing.Size(85, 17);
            this.cWholeWords.TabIndex = 33;
            this.cWholeWords.Text = "whole words";
            this.cWholeWords.UseVisualStyleBackColor = true;
            // 
            // nYearFrom
            // 
            this.nYearFrom.Location = new System.Drawing.Point(219, 260);
            this.nYearFrom.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.nYearFrom.Name = "nYearFrom";
            this.nYearFrom.Size = new System.Drawing.Size(80, 20);
            this.nYearFrom.TabIndex = 34;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(161, 262);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(52, 13);
            this.label13.TabIndex = 35;
            this.label13.Text = "Year from";
            // 
            // nYearTo
            // 
            this.nYearTo.Location = new System.Drawing.Point(219, 286);
            this.nYearTo.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.nYearTo.Name = "nYearTo";
            this.nYearTo.Size = new System.Drawing.Size(80, 20);
            this.nYearTo.TabIndex = 36;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(193, 288);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(16, 13);
            this.label14.TabIndex = 37;
            this.label14.Text = "to";
            // 
            // clSources
            // 
            this.clSources.CheckOnClick = true;
            this.clSources.FormattingEnabled = true;
            this.clSources.Items.AddRange(new object[] {
            "1 - Airwar",
            "2 - Russian",
            "5 - Aerofiles",
            "6 - AviaDejavu",
            "7 - FlyingMachines",
            "8 - My Travels"});
            this.clSources.Location = new System.Drawing.Point(382, 13);
            this.clSources.Name = "clSources";
            this.clSources.Size = new System.Drawing.Size(130, 94);
            this.clSources.TabIndex = 38;
            // 
            // cInText
            // 
            this.cInText.AutoSize = true;
            this.cInText.Location = new System.Drawing.Point(382, 201);
            this.cInText.Name = "cInText";
            this.cInText.Size = new System.Drawing.Size(54, 17);
            this.cInText.TabIndex = 34;
            this.cInText.Text = "in text";
            this.cInText.UseVisualStyleBackColor = true;
            // 
            // cCaseSensitive
            // 
            this.cCaseSensitive.AutoSize = true;
            this.cCaseSensitive.Location = new System.Drawing.Point(382, 182);
            this.cCaseSensitive.Name = "cCaseSensitive";
            this.cCaseSensitive.Size = new System.Drawing.Size(93, 17);
            this.cCaseSensitive.TabIndex = 34;
            this.cCaseSensitive.Text = "case sensitive";
            this.cCaseSensitive.UseVisualStyleBackColor = true;
            // 
            // bSrcR
            // 
            this.bSrcR.Location = new System.Drawing.Point(382, 107);
            this.bSrcR.Name = "bSrcR";
            this.bSrcR.Size = new System.Drawing.Size(23, 23);
            this.bSrcR.TabIndex = 39;
            this.bSrcR.Text = "R";
            this.bSrcR.UseVisualStyleBackColor = true;
            this.bSrcR.Click += new System.EventHandler(this.bSrcR_Click);
            // 
            // bSrcW
            // 
            this.bSrcW.Location = new System.Drawing.Point(411, 107);
            this.bSrcW.Name = "bSrcW";
            this.bSrcW.Size = new System.Drawing.Size(23, 23);
            this.bSrcW.TabIndex = 40;
            this.bSrcW.Text = "W";
            this.bSrcW.UseVisualStyleBackColor = true;
            this.bSrcW.Click += new System.EventHandler(this.bSrcW_Click);
            // 
            // bSrcRW
            // 
            this.bSrcRW.Location = new System.Drawing.Point(465, 107);
            this.bSrcRW.Name = "bSrcRW";
            this.bSrcRW.Size = new System.Drawing.Size(47, 23);
            this.bSrcRW.TabIndex = 41;
            this.bSrcRW.Text = "RW";
            this.bSrcRW.UseVisualStyleBackColor = true;
            this.bSrcRW.Click += new System.EventHandler(this.bSrcRW_Click);
            // 
            // fFilter
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(521, 376);
            this.Controls.Add(this.bSrcRW);
            this.Controls.Add(this.bSrcW);
            this.Controls.Add(this.bSrcR);
            this.Controls.Add(this.cCaseSensitive);
            this.Controls.Add(this.cInText);
            this.Controls.Add(this.clSources);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.nYearTo);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.nYearFrom);
            this.Controls.Add(this.cWholeWords);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.eText2);
            this.Controls.Add(this.eText);
            this.Controls.Add(this.eEngines);
            this.Controls.Add(this.eWings);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cProjNo);
            this.Controls.Add(this.cSinglNo);
            this.Controls.Add(this.cLlNo);
            this.Controls.Add(this.cGlidNo);
            this.Controls.Add(this.cUavNo);
            this.Controls.Add(this.cVertNo);
            this.Controls.Add(this.cProjYes);
            this.Controls.Add(this.cSinglYes);
            this.Controls.Add(this.cLlYes);
            this.Controls.Add(this.cGlidYes);
            this.Controls.Add(this.cUavYes);
            this.Controls.Add(this.cVertYes);
            this.Controls.Add(this.clCountries);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.bCountriesUnselect);
            this.Controls.Add(this.bCountriesSelect);
            this.Name = "fFilter";
            this.Text = "fFilter";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nYearFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nYearTo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button bCountriesSelect;
        private System.Windows.Forms.Button bCountriesUnselect;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckedListBox clCountries;
        private System.Windows.Forms.CheckBox cVertYes;
        private System.Windows.Forms.CheckBox cUavYes;
        private System.Windows.Forms.CheckBox cGlidYes;
        private System.Windows.Forms.CheckBox cLlYes;
        private System.Windows.Forms.CheckBox cSinglYes;
        private System.Windows.Forms.CheckBox cProjYes;
        private System.Windows.Forms.CheckBox cVertNo;
        private System.Windows.Forms.CheckBox cUavNo;
        private System.Windows.Forms.CheckBox cGlidNo;
        private System.Windows.Forms.CheckBox cLlNo;
        private System.Windows.Forms.CheckBox cSinglNo;
        private System.Windows.Forms.CheckBox cProjNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox eWings;
        private System.Windows.Forms.TextBox eEngines;
        private System.Windows.Forms.TextBox eText;
        private System.Windows.Forms.TextBox eText2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox cWholeWords;
        private System.Windows.Forms.NumericUpDown nYearFrom;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown nYearTo;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckedListBox clSources;
        private System.Windows.Forms.CheckBox cInText;
        private System.Windows.Forms.CheckBox cCaseSensitive;
        private System.Windows.Forms.Button bSrcR;
        private System.Windows.Forms.Button bSrcW;
        private System.Windows.Forms.Button bSrcRW;
    }
}