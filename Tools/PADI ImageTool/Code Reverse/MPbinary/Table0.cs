using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MPbinary
{
	public class Table0 : Form
	{
		private byte[] membuf = null;

		private int base_addr = 0;

		public bool force_sync = false;

		public bool need_erase = false;

		private ListViewItem.ListViewSubItem currSubItem = null;

		private IContainer components = null;

		private ListView listView1;

		private Label label1;

		private TextBox textBox1;

		private Label label2;

		private TextBox textBox2;

		private Button button2;

		private Button button1;

		public Table0()
		{
			this.InitializeComponent();
		}

		public void SetMemoryBuffer(byte[] buffer)
		{
			this.membuf = buffer;
		}

		public void SetBaseAddr(int addr)
		{
			this.base_addr = addr;
		}

		public void DisableEdit()
		{
			this.button2.Enabled = false;
			this.textBox1.Enabled = false;
			this.textBox2.Enabled = false;
			this.label1.Enabled = false;
			this.label2.Enabled = false;
			this.button1.Enabled = false;
		}

		public void HideSync()
		{
			this.button1.Visible = false;
		}

		private void Table0_Load(object sender, EventArgs e)
		{
			this.listView1.View = View.Details;
			this.listView1.Columns.Add("Addr");
			for (int i = 0; i < 16; i++)
			{
				this.listView1.Columns.Add(i.ToString("X"), 30);
			}
			for (int i = 0; i < this.membuf.Length / 16; i++)
			{
				ListViewItem listViewItem = new ListViewItem((this.base_addr + i * 16).ToString("X4"));
				for (int j = 0; j < 16; j++)
				{
					if (this.membuf[i * 16 + j] == 255)
					{
						listViewItem.SubItems.Add(this.membuf[i * 16 + j].ToString("x2"));
					}
					else
					{
						listViewItem.SubItems.Add(this.membuf[i * 16 + j].ToString("X2"));
					}
				}
				this.listView1.Items.Add(listViewItem);
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			int num = -1;
			try
			{
				num = Convert.ToInt32(this.textBox2.Text, 16);
			}
			catch
			{
			}
			int num2 = -1;
			try
			{
				num2 = Convert.ToInt32(this.textBox1.Text, 16);
			}
			catch
			{
			}
			if (num2 != -1)
			{
				if (num2 >= this.base_addr)
				{
					if (num2 - this.base_addr <= this.membuf.Length)
					{
						if (num >= 0 && num <= 255)
						{
							int num3 = (int)this.membuf[num2 - this.base_addr];
							if ((num3 ^ num) + num != num3)
							{
								this.need_erase = true;
							}
							this.membuf[num2 - this.base_addr] = (byte)num;
							if (num == 255)
							{
								this.currSubItem.Text = num.ToString("x2");
							}
							else
							{
								this.currSubItem.Text = num.ToString("X2");
							}
						}
					}
				}
			}
		}

		private void listView1_MouseDown(object sender, MouseEventArgs e)
		{
			Point point = this.listView1.PointToClient(Control.MousePosition);
			ListViewHitTestInfo listViewHitTestInfo = this.listView1.HitTest(point);
			int num = listViewHitTestInfo.Item.SubItems.IndexOf(listViewHitTestInfo.SubItem);
			int num2 = Convert.ToInt32(listViewHitTestInfo.Item.Text, 16) + num - 1;
			this.textBox1.Text = num2.ToString("X4");
			int num3 = Convert.ToInt32(listViewHitTestInfo.SubItem.Text, 16);
			this.textBox2.Text = num3.ToString("X2");
			this.currSubItem = listViewHitTestInfo.SubItem;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.force_sync = true;
			base.Close();
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
			this.listView1 = new ListView();
			this.label1 = new Label();
			this.textBox1 = new TextBox();
			this.label2 = new Label();
			this.textBox2 = new TextBox();
			this.button2 = new Button();
			this.button1 = new Button();
			base.SuspendLayout();
			this.listView1.GridLines = true;
			this.listView1.Location = new Point(12, 12);
			this.listView1.Name = "listView1";
			this.listView1.Size = new Size(578, 344);
			this.listView1.TabIndex = 0;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.MouseDown += new MouseEventHandler(this.listView1_MouseDown);
			this.label1.AutoSize = true;
			this.label1.Location = new Point(12, 365);
			this.label1.Name = "label1";
			this.label1.Size = new Size(42, 12);
			this.label1.TabIndex = 1;
			this.label1.Text = "Address";
			this.textBox1.Location = new Point(60, 360);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new Size(69, 22);
			this.textBox1.TabIndex = 2;
			this.label2.AutoSize = true;
			this.label2.Location = new Point(148, 365);
			this.label2.Name = "label2";
			this.label2.Size = new Size(32, 12);
			this.label2.TabIndex = 3;
			this.label2.Text = "Value";
			this.textBox2.Location = new Point(186, 360);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new Size(50, 22);
			this.textBox2.TabIndex = 4;
			this.button2.Location = new Point(251, 360);
			this.button2.Name = "button2";
			this.button2.Size = new Size(75, 23);
			this.button2.TabIndex = 6;
			this.button2.Text = "Update";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new EventHandler(this.button2_Click);
			this.button1.Location = new Point(515, 360);
			this.button1.Name = "button1";
			this.button1.Size = new Size(75, 23);
			this.button1.TabIndex = 7;
			this.button1.Text = "Sync";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new EventHandler(this.button1_Click);
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(602, 397);
			base.Controls.Add(this.button1);
			base.Controls.Add(this.button2);
			base.Controls.Add(this.textBox2);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.textBox1);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.listView1);
			base.Name = "Table0";
			this.Text = "Memory";
			base.Load += new EventHandler(this.Table0_Load);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
