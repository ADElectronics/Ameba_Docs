using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MPbinary
{
	public class JlinkPath : Form
	{
		public string selectJlinkPath = null;

		private IContainer components = null;

		private Label label1;

		private Button button1;

		private TextBox textBox1;

		private Label label2;

		public JlinkPath()
		{
			this.InitializeComponent();
		}

		private void jlinkBrowise(object sender, MouseEventArgs e)
		{
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
			folderBrowserDialog.ShowDialog();
			if (folderBrowserDialog.SelectedPath != null)
			{
				this.textBox1.Text = folderBrowserDialog.SelectedPath + "\\";
				this.selectJlinkPath = this.textBox1.Text;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.label1 = new Label();
			this.button1 = new Button();
			this.textBox1 = new TextBox();
			this.label2 = new Label();
			base.SuspendLayout();
			this.label1.AutoSize = true;
			this.label1.Location = new Point(23, 9);
			this.label1.Name = "label1";
			this.label1.Size = new Size(119, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "J-Link path not found!!!";
			this.button1.Location = new Point(223, 48);
			this.button1.Name = "button1";
			this.button1.Size = new Size(51, 23);
			this.button1.TabIndex = 1;
			this.button1.Text = "Browse";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.MouseClick += new MouseEventHandler(this.jlinkBrowise);
			this.textBox1.Location = new Point(23, 48);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new Size(194, 22);
			this.textBox1.TabIndex = 2;
			this.label2.AutoSize = true;
			this.label2.Location = new Point(23, 33);
			this.label2.Name = "label2";
			this.label2.Size = new Size(220, 12);
			this.label2.TabIndex = 3;
			this.label2.Text = "please enter J-Link path that contain JLink.exe";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(286, 83);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.textBox1);
			base.Controls.Add(this.button1);
			base.Controls.Add(this.label1);
			base.Name = "JlinkPath";
			this.Text = "JlinkPath";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
