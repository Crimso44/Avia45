namespace Aik2
{
    partial class fReport
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbReport = new System.Windows.Forms.ListBox();
            this.bSaveReport = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.bSaveReport);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 412);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 38);
            this.panel1.TabIndex = 0;
            // 
            // lbReport
            // 
            this.lbReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbReport.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbReport.FormattingEnabled = true;
            this.lbReport.ItemHeight = 14;
            this.lbReport.Location = new System.Drawing.Point(0, 0);
            this.lbReport.Name = "lbReport";
            this.lbReport.Size = new System.Drawing.Size(800, 412);
            this.lbReport.TabIndex = 1;
            this.lbReport.DoubleClick += new System.EventHandler(this.lbReport_DoubleClick);
            // 
            // bSaveReport
            // 
            this.bSaveReport.Location = new System.Drawing.Point(713, 9);
            this.bSaveReport.Name = "bSaveReport";
            this.bSaveReport.Size = new System.Drawing.Size(75, 23);
            this.bSaveReport.TabIndex = 0;
            this.bSaveReport.Text = "Сохранить";
            this.bSaveReport.UseVisualStyleBackColor = true;
            this.bSaveReport.Click += new System.EventHandler(this.bSaveReport_Click);
            // 
            // fReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lbReport);
            this.Controls.Add(this.panel1);
            this.Name = "fReport";
            this.Text = "fReport";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox lbReport;
        private System.Windows.Forms.Button bSaveReport;
    }
}