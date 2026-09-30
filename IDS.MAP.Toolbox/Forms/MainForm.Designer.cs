namespace IDS.MAP.Toolbox.Forms
{
  partial class MainForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.grp_1 = new System.Windows.Forms.GroupBox();
      this.panel2 = new System.Windows.Forms.Panel();
      this.txt_path = new System.Windows.Forms.TextBox();
      this.btn_path = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.panel1 = new System.Windows.Forms.Panel();
      this.btn_update = new System.Windows.Forms.Button();
      this.grp_2 = new System.Windows.Forms.GroupBox();
      this.panel3 = new System.Windows.Forms.Panel();
      this.btn_2_searchPreview = new System.Windows.Forms.Button();
      this.btn_2_excelColumns = new System.Windows.Forms.Button();
      this.btn_2_createXml = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this.grp_3 = new System.Windows.Forms.GroupBox();
      this.panel4 = new System.Windows.Forms.Panel();
      this.btn_3_wrongId = new System.Windows.Forms.Button();
      this.btn_3_nostructure = new System.Windows.Forms.Button();
      this.label3 = new System.Windows.Forms.Label();
      this.grp_4 = new System.Windows.Forms.GroupBox();
      this.panel5 = new System.Windows.Forms.Panel();
      this.bnt_4_link = new System.Windows.Forms.Button();
      this.btn_4_xref = new System.Windows.Forms.Button();
      this.btn_4_xmlSyntax = new System.Windows.Forms.Button();
      this.btn_4_missingXml = new System.Windows.Forms.Button();
      this.label4 = new System.Windows.Forms.Label();
      this.grp_5 = new System.Windows.Forms.GroupBox();
      this.panel6 = new System.Windows.Forms.Panel();
      this.btn_build = new System.Windows.Forms.Button();
      this.label5 = new System.Windows.Forms.Label();
      this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.grp_1.SuspendLayout();
      this.panel2.SuspendLayout();
      this.panel1.SuspendLayout();
      this.grp_2.SuspendLayout();
      this.panel3.SuspendLayout();
      this.grp_3.SuspendLayout();
      this.panel4.SuspendLayout();
      this.grp_4.SuspendLayout();
      this.panel5.SuspendLayout();
      this.grp_5.SuspendLayout();
      this.panel6.SuspendLayout();
      this.SuspendLayout();
      // 
      // pictureBox1
      // 
      this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Left;
      this.pictureBox1.Image = global::IDS.MAP.Toolbox.Properties.Resources.ids2019_farbig_auf_hell_300dpi;
      this.pictureBox1.Location = new System.Drawing.Point(0, 0);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(233, 78);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
      this.pictureBox1.DoubleClick += new System.EventHandler(this.pictureBox1_DoubleClick);
      // 
      // grp_1
      // 
      this.grp_1.Controls.Add(this.panel2);
      this.grp_1.Controls.Add(this.label1);
      this.grp_1.Dock = System.Windows.Forms.DockStyle.Top;
      this.grp_1.Location = new System.Drawing.Point(0, 78);
      this.grp_1.Name = "grp_1";
      this.grp_1.Size = new System.Drawing.Size(800, 63);
      this.grp_1.TabIndex = 1;
      this.grp_1.TabStop = false;
      this.grp_1.Text = "1. MAP-Datenpfad";
      // 
      // panel2
      // 
      this.panel2.Controls.Add(this.txt_path);
      this.panel2.Controls.Add(this.btn_path);
      this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel2.Location = new System.Drawing.Point(3, 34);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(794, 21);
      this.panel2.TabIndex = 1;
      // 
      // txt_path
      // 
      this.txt_path.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txt_path.Location = new System.Drawing.Point(0, 0);
      this.txt_path.Name = "txt_path";
      this.txt_path.ReadOnly = true;
      this.txt_path.Size = new System.Drawing.Size(719, 22);
      this.txt_path.TabIndex = 1;
      // 
      // btn_path
      // 
      this.btn_path.Dock = System.Windows.Forms.DockStyle.Right;
      this.btn_path.Location = new System.Drawing.Point(719, 0);
      this.btn_path.Name = "btn_path";
      this.btn_path.Size = new System.Drawing.Size(75, 21);
      this.btn_path.TabIndex = 0;
      this.btn_path.Text = "Ändern";
      this.btn_path.UseVisualStyleBackColor = true;
      this.btn_path.Click += new System.EventHandler(this.btn_path_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Dock = System.Windows.Forms.DockStyle.Top;
      this.label1.Location = new System.Drawing.Point(3, 18);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(385, 16);
      this.label1.TabIndex = 0;
      this.label1.Text = "Bitte hier den Datenpfad auswählen, der alle MAP-Daten enthält:";
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.btn_update);
      this.panel1.Controls.Add(this.pictureBox1);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel1.Location = new System.Drawing.Point(0, 0);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(800, 78);
      this.panel1.TabIndex = 2;
      // 
      // btn_update
      // 
      this.btn_update.Dock = System.Windows.Forms.DockStyle.Right;
      this.btn_update.Location = new System.Drawing.Point(722, 0);
      this.btn_update.Name = "btn_update";
      this.btn_update.Size = new System.Drawing.Size(78, 78);
      this.btn_update.TabIndex = 1;
      this.btn_update.Text = "Aktualisieren";
      this.btn_update.UseVisualStyleBackColor = true;
      this.btn_update.Click += new System.EventHandler(this.btn_update_Click);
      // 
      // grp_2
      // 
      this.grp_2.Controls.Add(this.panel3);
      this.grp_2.Controls.Add(this.label2);
      this.grp_2.Dock = System.Windows.Forms.DockStyle.Top;
      this.grp_2.Location = new System.Drawing.Point(0, 141);
      this.grp_2.Name = "grp_2";
      this.grp_2.Size = new System.Drawing.Size(800, 82);
      this.grp_2.TabIndex = 3;
      this.grp_2.TabStop = false;
      this.grp_2.Text = "2. Excel-Datei";
      // 
      // panel3
      // 
      this.panel3.Controls.Add(this.btn_2_searchPreview);
      this.panel3.Controls.Add(this.btn_2_excelColumns);
      this.panel3.Controls.Add(this.btn_2_createXml);
      this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel3.Location = new System.Drawing.Point(3, 34);
      this.panel3.Name = "panel3";
      this.panel3.Size = new System.Drawing.Size(794, 42);
      this.panel3.TabIndex = 1;
      // 
      // btn_2_searchPreview
      // 
      this.btn_2_searchPreview.Dock = System.Windows.Forms.DockStyle.Right;
      this.btn_2_searchPreview.Location = new System.Drawing.Point(534, 0);
      this.btn_2_searchPreview.Name = "btn_2_searchPreview";
      this.btn_2_searchPreview.Size = new System.Drawing.Size(130, 42);
      this.btn_2_searchPreview.TabIndex = 3;
      this.btn_2_searchPreview.Text = "Suche-Vorschau";
      this.btn_2_searchPreview.UseVisualStyleBackColor = true;
      this.btn_2_searchPreview.Visible = false;
      this.btn_2_searchPreview.Click += new System.EventHandler(this.btn_2_searchPreview_Click);
      // 
      // btn_2_excelColumns
      // 
      this.btn_2_excelColumns.Dock = System.Windows.Forms.DockStyle.Left;
      this.btn_2_excelColumns.Location = new System.Drawing.Point(0, 0);
      this.btn_2_excelColumns.Name = "btn_2_excelColumns";
      this.btn_2_excelColumns.Size = new System.Drawing.Size(216, 42);
      this.btn_2_excelColumns.TabIndex = 2;
      this.btn_2_excelColumns.Text = "F21: Fehlende Spalten";
      this.btn_2_excelColumns.UseVisualStyleBackColor = true;
      this.btn_2_excelColumns.Click += new System.EventHandler(this.btn_2_excelColumns_Click);
      // 
      // btn_2_createXml
      // 
      this.btn_2_createXml.Dock = System.Windows.Forms.DockStyle.Right;
      this.btn_2_createXml.Location = new System.Drawing.Point(664, 0);
      this.btn_2_createXml.Name = "btn_2_createXml";
      this.btn_2_createXml.Size = new System.Drawing.Size(130, 42);
      this.btn_2_createXml.TabIndex = 0;
      this.btn_2_createXml.Text = "XML-Erstellen";
      this.btn_2_createXml.UseVisualStyleBackColor = true;
      this.btn_2_createXml.Click += new System.EventHandler(this.btn_2_createXml_Click);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Dock = System.Windows.Forms.DockStyle.Top;
      this.label2.Location = new System.Drawing.Point(3, 18);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(803, 16);
      this.label2.TabIndex = 0;
      this.label2.Text = "Wenn die Excel-Datei nicht der Spezifikation entspricht, erscheinen Fehlermeldung" +
    "en - andernfalls können XML-Dateien erstellt werden.";
      // 
      // grp_3
      // 
      this.grp_3.Controls.Add(this.panel4);
      this.grp_3.Controls.Add(this.label3);
      this.grp_3.Dock = System.Windows.Forms.DockStyle.Top;
      this.grp_3.Location = new System.Drawing.Point(0, 223);
      this.grp_3.Name = "grp_3";
      this.grp_3.Size = new System.Drawing.Size(800, 82);
      this.grp_3.TabIndex = 4;
      this.grp_3.TabStop = false;
      this.grp_3.Text = "3. Struktur-Informationen";
      // 
      // panel4
      // 
      this.panel4.Controls.Add(this.btn_3_wrongId);
      this.panel4.Controls.Add(this.btn_3_nostructure);
      this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel4.Location = new System.Drawing.Point(3, 34);
      this.panel4.Name = "panel4";
      this.panel4.Size = new System.Drawing.Size(794, 42);
      this.panel4.TabIndex = 1;
      // 
      // btn_3_wrongId
      // 
      this.btn_3_wrongId.Dock = System.Windows.Forms.DockStyle.Left;
      this.btn_3_wrongId.Location = new System.Drawing.Point(216, 0);
      this.btn_3_wrongId.Name = "btn_3_wrongId";
      this.btn_3_wrongId.Size = new System.Drawing.Size(150, 42);
      this.btn_3_wrongId.TabIndex = 4;
      this.btn_3_wrongId.Text = "F32: Struktur-Fehler";
      this.btn_3_wrongId.UseVisualStyleBackColor = true;
      this.btn_3_wrongId.Click += new System.EventHandler(this.btn_3_wrongId_Click);
      // 
      // btn_3_nostructure
      // 
      this.btn_3_nostructure.Dock = System.Windows.Forms.DockStyle.Left;
      this.btn_3_nostructure.Location = new System.Drawing.Point(0, 0);
      this.btn_3_nostructure.Name = "btn_3_nostructure";
      this.btn_3_nostructure.Size = new System.Drawing.Size(216, 42);
      this.btn_3_nostructure.TabIndex = 0;
      this.btn_3_nostructure.Text = "F31: Keine _struktur.xml";
      this.btn_3_nostructure.UseVisualStyleBackColor = true;
      this.btn_3_nostructure.Click += new System.EventHandler(this.btn_3_nostructure_Click);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Dock = System.Windows.Forms.DockStyle.Top;
      this.label3.Location = new System.Drawing.Point(3, 18);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(467, 16);
      this.label3.TabIndex = 0;
      this.label3.Text = "Wenn Fehler mit Bezug zu _struktur.xml vorhanden sind, erscheinen diese hier:";
      // 
      // grp_4
      // 
      this.grp_4.Controls.Add(this.panel5);
      this.grp_4.Controls.Add(this.label4);
      this.grp_4.Dock = System.Windows.Forms.DockStyle.Top;
      this.grp_4.Location = new System.Drawing.Point(0, 305);
      this.grp_4.Name = "grp_4";
      this.grp_4.Size = new System.Drawing.Size(800, 69);
      this.grp_4.TabIndex = 5;
      this.grp_4.TabStop = false;
      this.grp_4.Text = "4. XML-Dokumente";
      // 
      // panel5
      // 
      this.panel5.Controls.Add(this.bnt_4_link);
      this.panel5.Controls.Add(this.btn_4_xref);
      this.panel5.Controls.Add(this.btn_4_xmlSyntax);
      this.panel5.Controls.Add(this.btn_4_missingXml);
      this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel5.Location = new System.Drawing.Point(3, 34);
      this.panel5.Name = "panel5";
      this.panel5.Size = new System.Drawing.Size(794, 35);
      this.panel5.TabIndex = 1;
      // 
      // bnt_4_link
      // 
      this.bnt_4_link.Dock = System.Windows.Forms.DockStyle.Left;
      this.bnt_4_link.Location = new System.Drawing.Point(516, 0);
      this.bnt_4_link.Name = "bnt_4_link";
      this.bnt_4_link.Size = new System.Drawing.Size(150, 35);
      this.bnt_4_link.TabIndex = 3;
      this.bnt_4_link.Text = "F44: LINK-Falsch";
      this.bnt_4_link.UseVisualStyleBackColor = true;
      this.bnt_4_link.Click += new System.EventHandler(this.bnt_4_link_Click);
      // 
      // btn_4_xref
      // 
      this.btn_4_xref.Dock = System.Windows.Forms.DockStyle.Left;
      this.btn_4_xref.Location = new System.Drawing.Point(366, 0);
      this.btn_4_xref.Name = "btn_4_xref";
      this.btn_4_xref.Size = new System.Drawing.Size(150, 35);
      this.btn_4_xref.TabIndex = 2;
      this.btn_4_xref.Text = "F43: XREF-Falsch";
      this.btn_4_xref.UseVisualStyleBackColor = true;
      this.btn_4_xref.Click += new System.EventHandler(this.btn_4_xref_Click);
      // 
      // btn_4_xmlSyntax
      // 
      this.btn_4_xmlSyntax.Dock = System.Windows.Forms.DockStyle.Left;
      this.btn_4_xmlSyntax.Location = new System.Drawing.Point(216, 0);
      this.btn_4_xmlSyntax.Name = "btn_4_xmlSyntax";
      this.btn_4_xmlSyntax.Size = new System.Drawing.Size(150, 35);
      this.btn_4_xmlSyntax.TabIndex = 0;
      this.btn_4_xmlSyntax.Text = "F42: XML-Syntax";
      this.btn_4_xmlSyntax.UseVisualStyleBackColor = true;
      this.btn_4_xmlSyntax.Click += new System.EventHandler(this.btn_4_xmlSyntax_Click);
      // 
      // btn_4_missingXml
      // 
      this.btn_4_missingXml.Dock = System.Windows.Forms.DockStyle.Left;
      this.btn_4_missingXml.Location = new System.Drawing.Point(0, 0);
      this.btn_4_missingXml.MinimumSize = new System.Drawing.Size(160, 0);
      this.btn_4_missingXml.Name = "btn_4_missingXml";
      this.btn_4_missingXml.Size = new System.Drawing.Size(216, 35);
      this.btn_4_missingXml.TabIndex = 5;
      this.btn_4_missingXml.Text = "F41: Fehlender XML-Artikel";
      this.btn_4_missingXml.UseVisualStyleBackColor = true;
      this.btn_4_missingXml.Click += new System.EventHandler(this.btn_4_missingXml_Click);
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Dock = System.Windows.Forms.DockStyle.Top;
      this.label4.Location = new System.Drawing.Point(3, 18);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(448, 16);
      this.label4.TabIndex = 0;
      this.label4.Text = "Wenn es in einem XML-Dokument fehler gibt, erscheinen Fehlermeldungen";
      // 
      // grp_5
      // 
      this.grp_5.Controls.Add(this.panel6);
      this.grp_5.Controls.Add(this.label5);
      this.grp_5.Dock = System.Windows.Forms.DockStyle.Top;
      this.grp_5.Location = new System.Drawing.Point(0, 374);
      this.grp_5.Name = "grp_5";
      this.grp_5.Size = new System.Drawing.Size(800, 72);
      this.grp_5.TabIndex = 6;
      this.grp_5.TabStop = false;
      this.grp_5.Text = "5. Veröffentlichung";
      // 
      // panel6
      // 
      this.panel6.Controls.Add(this.btn_build);
      this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel6.Location = new System.Drawing.Point(3, 34);
      this.panel6.Name = "panel6";
      this.panel6.Size = new System.Drawing.Size(794, 34);
      this.panel6.TabIndex = 1;
      // 
      // btn_build
      // 
      this.btn_build.Dock = System.Windows.Forms.DockStyle.Left;
      this.btn_build.Location = new System.Drawing.Point(0, 0);
      this.btn_build.Name = "btn_build";
      this.btn_build.Size = new System.Drawing.Size(150, 34);
      this.btn_build.TabIndex = 0;
      this.btn_build.Text = "Veröffentlichen";
      this.btn_build.UseVisualStyleBackColor = true;
      this.btn_build.Click += new System.EventHandler(this.btn_build_Click);
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Dock = System.Windows.Forms.DockStyle.Top;
      this.label5.Location = new System.Drawing.Point(3, 18);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(446, 16);
      this.label5.TabIndex = 0;
      this.label5.Text = "Sind alle Fehler beseitigt, kann eine neue Veröffentlichung erzeugt werden.";
      // 
      // backgroundWorker
      // 
      this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
      this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
      // 
      // MainForm
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.BackColor = System.Drawing.Color.White;
      this.ClientSize = new System.Drawing.Size(800, 447);
      this.Controls.Add(this.grp_5);
      this.Controls.Add(this.grp_4);
      this.Controls.Add(this.grp_3);
      this.Controls.Add(this.grp_2);
      this.Controls.Add(this.grp_1);
      this.Controls.Add(this.panel1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "MainForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "IDS - MAP-Workflow";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
      this.Load += new System.EventHandler(this.MainForm_Load);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.grp_1.ResumeLayout(false);
      this.grp_1.PerformLayout();
      this.panel2.ResumeLayout(false);
      this.panel2.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.grp_2.ResumeLayout(false);
      this.grp_2.PerformLayout();
      this.panel3.ResumeLayout(false);
      this.grp_3.ResumeLayout(false);
      this.grp_3.PerformLayout();
      this.panel4.ResumeLayout(false);
      this.grp_4.ResumeLayout(false);
      this.grp_4.PerformLayout();
      this.panel5.ResumeLayout(false);
      this.grp_5.ResumeLayout(false);
      this.grp_5.PerformLayout();
      this.panel6.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.GroupBox grp_1;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.TextBox txt_path;
    private System.Windows.Forms.Button btn_path;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.GroupBox grp_2;
    private System.Windows.Forms.Panel panel3;
    private System.Windows.Forms.Button btn_2_createXml;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.GroupBox grp_3;
    private System.Windows.Forms.Panel panel4;
    private System.Windows.Forms.Button btn_3_nostructure;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button btn_update;
    private System.Windows.Forms.Button btn_2_excelColumns;
    private System.Windows.Forms.GroupBox grp_4;
    private System.Windows.Forms.Panel panel5;
    private System.Windows.Forms.Button bnt_4_link;
    private System.Windows.Forms.Button btn_4_xref;
    private System.Windows.Forms.Button btn_4_xmlSyntax;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.GroupBox grp_5;
    private System.Windows.Forms.Panel panel6;
    private System.Windows.Forms.Button btn_build;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Button btn_4_missingXml;
    private System.Windows.Forms.Button btn_3_wrongId;
    private System.ComponentModel.BackgroundWorker backgroundWorker;
    private System.Windows.Forms.Button btn_2_searchPreview;
  }
}