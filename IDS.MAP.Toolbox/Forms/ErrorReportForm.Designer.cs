namespace IDS.MAP.Toolbox.Forms
{
  partial class ErrorReportForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorReportForm));
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.tabStructured = new System.Windows.Forms.TabPage();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.dataGridView1 = new System.Windows.Forms.DataGridView();
      this.colFile = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colLine = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.txtDetails = new System.Windows.Forms.TextBox();
      this.panel1 = new System.Windows.Forms.Panel();
      this.txtFilter = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.lblSummary = new System.Windows.Forms.Label();
      this.tabText = new System.Windows.Forms.TabPage();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.tabControl1.SuspendLayout();
      this.tabStructured.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
      this.panel1.SuspendLayout();
      this.tabText.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabControl1
      // 
      this.tabControl1.Controls.Add(this.tabStructured);
      this.tabControl1.Controls.Add(this.tabText);
      this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl1.Location = new System.Drawing.Point(0, 0);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(1100, 650);
      this.tabControl1.TabIndex = 0;
      // 
      // tabStructured
      // 
      this.tabStructured.Controls.Add(this.splitContainer1);
      this.tabStructured.Controls.Add(this.panel1);
      this.tabStructured.Location = new System.Drawing.Point(4, 25);
      this.tabStructured.Name = "tabStructured";
      this.tabStructured.Padding = new System.Windows.Forms.Padding(3);
      this.tabStructured.Size = new System.Drawing.Size(1092, 621);
      this.tabStructured.TabIndex = 0;
      this.tabStructured.Text = "Strukturiert";
      this.tabStructured.UseVisualStyleBackColor = true;
      // 
      // splitContainer1
      // 
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.Location = new System.Drawing.Point(3, 51);
      this.splitContainer1.Name = "splitContainer1";
      this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.dataGridView1);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.txtDetails);
      this.splitContainer1.Size = new System.Drawing.Size(1086, 567);
      this.splitContainer1.SplitterDistance = 405;
      this.splitContainer1.TabIndex = 1;
      // 
      // dataGridView1
      // 
      this.dataGridView1.AllowUserToAddRows = false;
      this.dataGridView1.AllowUserToDeleteRows = false;
      this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
      this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colFile,
            this.colLine,
            this.colMessage});
      this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dataGridView1.Location = new System.Drawing.Point(0, 0);
      this.dataGridView1.MultiSelect = false;
      this.dataGridView1.Name = "dataGridView1";
      this.dataGridView1.ReadOnly = true;
      this.dataGridView1.RowHeadersVisible = false;
      this.dataGridView1.RowHeadersWidth = 51;
      this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.dataGridView1.Size = new System.Drawing.Size(1086, 405);
      this.dataGridView1.TabIndex = 0;
      // 
      // colFile
      // 
      this.colFile.DataPropertyName = "FileName";
      this.colFile.FillWeight = 120F;
      this.colFile.HeaderText = "Datei";
      this.colFile.MinimumWidth = 6;
      this.colFile.Name = "colFile";
      this.colFile.ReadOnly = true;
      // 
      // colLine
      // 
      this.colLine.DataPropertyName = "Line";
      this.colLine.FillWeight = 45F;
      this.colLine.HeaderText = "Zeile";
      this.colLine.MinimumWidth = 6;
      this.colLine.Name = "colLine";
      this.colLine.ReadOnly = true;
      // 
      // colMessage
      // 
      this.colMessage.DataPropertyName = "UserMessage";
      this.colMessage.FillWeight = 300F;
      this.colMessage.HeaderText = "Meldung";
      this.colMessage.MinimumWidth = 6;
      this.colMessage.Name = "colMessage";
      this.colMessage.ReadOnly = true;
      // 
      // txtDetails
      // 
      this.txtDetails.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtDetails.Location = new System.Drawing.Point(0, 0);
      this.txtDetails.Multiline = true;
      this.txtDetails.Name = "txtDetails";
      this.txtDetails.ReadOnly = true;
      this.txtDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.txtDetails.Size = new System.Drawing.Size(1086, 158);
      this.txtDetails.TabIndex = 0;
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.txtFilter);
      this.panel1.Controls.Add(this.label2);
      this.panel1.Controls.Add(this.lblSummary);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel1.Location = new System.Drawing.Point(3, 3);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(1086, 48);
      this.panel1.TabIndex = 0;
      // 
      // txtFilter
      // 
      this.txtFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtFilter.Location = new System.Drawing.Point(376, 14);
      this.txtFilter.Name = "txtFilter";
      this.txtFilter.Size = new System.Drawing.Size(707, 22);
      this.txtFilter.TabIndex = 2;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(301, 16);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(48, 16);
      this.label2.TabIndex = 1;
      this.label2.Text = "Suche:";
      // 
      // lblSummary
      // 
      this.lblSummary.AutoSize = true;
      this.lblSummary.Location = new System.Drawing.Point(8, 16);
      this.lblSummary.Name = "lblSummary";
      this.lblSummary.Size = new System.Drawing.Size(121, 16);
      this.lblSummary.TabIndex = 0;
      this.lblSummary.Text = "Zusammenfassung";
      // 
      // tabText
      // 
      this.tabText.Controls.Add(this.textBox1);
      this.tabText.Location = new System.Drawing.Point(4, 25);
      this.tabText.Name = "tabText";
      this.tabText.Padding = new System.Windows.Forms.Padding(3);
      this.tabText.Size = new System.Drawing.Size(1092, 621);
      this.tabText.TabIndex = 1;
      this.tabText.Text = "Rohtext";
      this.tabText.UseVisualStyleBackColor = true;
      // 
      // textBox1
      // 
      this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.textBox1.Location = new System.Drawing.Point(3, 3);
      this.textBox1.Multiline = true;
      this.textBox1.Name = "textBox1";
      this.textBox1.ReadOnly = true;
      this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.textBox1.Size = new System.Drawing.Size(1086, 615);
      this.textBox1.TabIndex = 0;
      // 
      // ErrorReportForm
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.ClientSize = new System.Drawing.Size(1100, 650);
      this.Controls.Add(this.tabControl1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ErrorReportForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Fehlerbericht";
      this.tabControl1.ResumeLayout(false);
      this.tabStructured.ResumeLayout(false);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.Panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.tabText.ResumeLayout(false);
      this.tabText.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabStructured;
    private System.Windows.Forms.TabPage tabText;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label lblSummary;
    private System.Windows.Forms.TextBox txtFilter;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.DataGridView dataGridView1;
    private System.Windows.Forms.TextBox txtDetails;
    private System.Windows.Forms.DataGridViewTextBoxColumn colFile;
    private System.Windows.Forms.DataGridViewTextBoxColumn colLine;
    private System.Windows.Forms.DataGridViewTextBoxColumn colMessage;
  }
}
