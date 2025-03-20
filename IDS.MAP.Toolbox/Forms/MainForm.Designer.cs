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
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.panel2 = new System.Windows.Forms.Panel();
      this.txt_path = new System.Windows.Forms.TextBox();
      this.btn_path = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.panel1 = new System.Windows.Forms.Panel();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.groupBox1.SuspendLayout();
      this.panel2.SuspendLayout();
      this.panel1.SuspendLayout();
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
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.panel2);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
      this.groupBox1.Location = new System.Drawing.Point(0, 78);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(800, 54);
      this.groupBox1.TabIndex = 1;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "1. MAP-Datenpfad";
      // 
      // panel2
      // 
      this.panel2.Controls.Add(this.txt_path);
      this.panel2.Controls.Add(this.btn_path);
      this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel2.Location = new System.Drawing.Point(3, 29);
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
      this.txt_path.Size = new System.Drawing.Size(719, 20);
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
      this.label1.Location = new System.Drawing.Point(3, 16);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(312, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Bitte hier den Datenpfad auswählen, der alle MAP-Daten enthält:";
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.pictureBox1);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel1.Location = new System.Drawing.Point(0, 0);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(800, 78);
      this.panel1.TabIndex = 2;
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.White;
      this.ClientSize = new System.Drawing.Size(800, 450);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.panel1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "MainForm";
      this.Text = "IDS - MAP-Workflow";
      this.Load += new System.EventHandler(this.MainForm_Load);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.panel2.ResumeLayout(false);
      this.panel2.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.TextBox txt_path;
    private System.Windows.Forms.Button btn_path;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Panel panel1;
  }
}