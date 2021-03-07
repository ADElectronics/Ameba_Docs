using Microsoft.Win32;
using MPbinary.Properties;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;

namespace MPbinary
{
	public partial class MainWindow : Window, IComponentConnector
	{
		private bool factory_mode = false;

		private bool enalbe_reset_msg = false;

		private long flash_size = 1048576L;

		private string jlinkpathString = null;

		private string tmppathString = null;

		private byte[] system_data = null;

		private byte[] calibrate_data = null;

		private HwndSource _source;

		private int last_hotkey = 0;

		private int pass_cnt = 0;

		private bool handleUserMouse;

		private bool handleFW2Mouse;

		private bool handleFW1Mouse;

		private bool handleOTAMouse;

		private bool handleUser1Mouse;

		private long[] flash_table = new long[]
		{
			1048576L,
			2097152L,
			4194304L,
			8388608L,
			16777216L,
			33554432L,
			67108864L
		};

		private int hidden_cnt = 0;

		private int hidden_cnt2 = 0;

		private int bootImgLen = 0;

		private int fw1ImgLen = 0;

		private int fw2ImgLen = 0;

		private int userImgLen = 0;

		private uint bootImgOffset = 0u;

		private uint fw1ImgOffset = 0u;

		private uint fw2ImgOffset = 0u;

		private uint userImgOffset = 0u;

		private bool fw1IsRamAll = false;

		private int defaultImgLen = 0;

		private int otaImgLen = 0;

		private int user1ImgLen = 0;

		private long flash1_size;

		private uint defaultImgOffset = 0u;

		private uint otaImgOffset = 0u;

		private uint user1ImgOffset = 0u;

		private byte[] system_data_gen = null;

		//internal MainWindow Window1;

		//internal Grid pageDownload;

		//internal Grid gridFactoryUpper;
        /*
		internal Button initFlashButton;

		internal Button clearButton;

		internal CheckBox advModeCheck;

		internal GroupBox tabBootImg;

		internal Grid bootGrid;

		internal TextBox bootFileName;

		internal CheckBox verifyBoot;

		internal TextBox offsetBoot;

		internal GroupBox tabFW2Img;

		internal Grid fw2Grid;

		internal TextBox offsetFW2;

		internal TextBox fw2FileName;

		internal CheckBox verifyFW2;

		internal CheckBox defaultFW2Check;

		internal GroupBox tabFW1Img;

		internal Grid fw1Grid;

		internal TextBox offsetFW1;

		internal TextBox fw1FileName;

		internal CheckBox verifyFW1;

		internal CheckBox defaultFW1Check;

		internal Grid gridFactoryBottom;

		internal GroupBox tabSysData;

		internal Grid sysDataGrid;

		internal ComboBox trig1PinCombo;

		internal ComboBox trig1PortCombo;

		internal CheckBox trig1Check;

		internal CheckBox trig2Check;

		internal ComboBox trig2PortCombo;

		internal ComboBox trig2PinCombo;

		internal ComboBox trig1LevelCombo;

		internal ComboBox trig2LevelCombo;

		internal GroupBox tabInfo;

		internal Rectangle hiddenFunc2;

		internal Label addrBootStart;

		internal Label addrBootEnd;

		internal Label addrFW2Start;

		internal Label addrFW2End;

		internal Label addrUSERStart;

		internal Label addrUSEREnd;

		internal Label addrFW1Start;

		internal Label addrFW1End;

		internal GroupBox tabFlash;

		internal Grid flashOptGrid;

		internal ComboBox eraseCombo;

		internal Button eraseFlashButton;

		internal ComboBox flashSizeCombo;

		internal Button dumpButton;

		internal TextBox eraseRangeStart;

		internal TextBox eraseRangeEnd;

		internal Grid advFuncGrid;

		internal Button editSysButton;

		internal Button viewCalButton;

		internal Button loadCfgButton;

		internal Button factoryButton;

		internal Canvas canvasFlash;

		internal Rectangle rectFlash;

		internal Rectangle rectBoot;

		internal Rectangle rectFW1;

		internal Rectangle rectUSER;

		internal Rectangle rectFW2;

		internal Grid gridFactoryCenter;

		internal Image hiddenFunc;

		internal GroupBox tabUSERimg;

		internal Button downloadUSERButton;

		internal Grid gridFactoryUserOff;

		internal TextBox offsetUSER;

		internal TextBox userFileName;

		internal CheckBox verifyUSER;

		internal CheckBox skipCalibrationCheck;

		internal TabItem tabGenerate;

		internal TextBox defaultFileName;

		internal TextBox offsetOTA;

		internal TextBox otaFileName;

		internal TextBox saveFileName;

		internal TextBox offsetUSER1;

		internal TextBox user1FileName;

		internal ComboBox flashSizeCombo1;

		internal ComboBox trig1PinCombo1;

		internal ComboBox trig1PortCombo1;

		internal CheckBox trig1Check1;

		internal CheckBox trig2Check1;

		internal ComboBox trig2PortCombo1;

		internal ComboBox trig2PinCombo1;

		internal ComboBox trig1LevelCombo1;

		internal ComboBox trig2LevelCombo1;

		internal ComboBox bootCombo;

		internal Rectangle hiddenFunc1;

		internal Label addrDefaultStart;

		internal Label addrDefaultEnd;

		internal Label addrOTAStart;

		internal Label addrOTAEnd;

		internal Label addrUSER1Start;

		internal Label addrUSER1End;

		internal Button generateButton;

		internal Image hiddenFunc3;

		internal Button refreshButton;

		internal Canvas canvasFlash_Copy;

		internal Rectangle rectFlash1;

		internal Rectangle rectDefault;

		internal Rectangle rectOTA;

		internal Rectangle rectUSER1;
       
		private bool _contentLoaded;
 */
		public MainWindow()
		{
			this.InitializeComponent();
		}

		[DllImport("User32.dll")]
		private static extern bool RegisterHotKey([In] IntPtr hWnd, [In] int id, [In] uint fsModifiers, [In] uint vk);

		[DllImport("User32.dll")]
		private static extern bool UnregisterHotKey([In] IntPtr hWnd, [In] int id);

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			WindowInteropHelper windowInteropHelper = new WindowInteropHelper(this);
			this._source = HwndSource.FromHwnd(windowInteropHelper.Handle);
			this._source.AddHook(new HwndSourceHook(this.HwndHook));
			this.RegisterHotKey();
		}

		protected override void OnClosed(EventArgs e)
		{
			this._source.RemoveHook(new HwndSourceHook(this.HwndHook));
			this._source = null;
			this.UnregisterHotKey();
			base.OnClosed(e);
		}

		private void RegisterHotKey()
		{
			WindowInteropHelper windowInteropHelper = new WindowInteropHelper(this);
			for (char c = '0'; c < '9'; c += '\u0001')
			{
				MainWindow.RegisterHotKey(windowInteropHelper.Handle, (int)('ь' + c), 10u, (uint)c);
			}
			for (char c = 'A'; c < 'Z'; c += '\u0001')
			{
				MainWindow.RegisterHotKey(windowInteropHelper.Handle, (int)('ь' + c), 10u, (uint)c);
			}
		}

		private void UnregisterHotKey()
		{
			WindowInteropHelper windowInteropHelper = new WindowInteropHelper(this);
			for (int i = 1148; i <= 1157; i++)
			{
				MainWindow.UnregisterHotKey(windowInteropHelper.Handle, i);
			}
			for (int i = 1165; i <= 1190; i++)
			{
				MainWindow.UnregisterHotKey(windowInteropHelper.Handle, i);
			}
		}

		private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == 786)
			{
				int num = wParam.ToInt32();
				if (num <= 1169)
				{
					if (num == 1165)
					{
						handled = true;
						if (this.last_hotkey == 1102)
						{
							this.last_hotkey = 1103;
						}
						else
						{
							this.last_hotkey = 0;
						}
						goto IL_1B0;
					}
					if (num == 1169)
					{
						handled = true;
						if (this.last_hotkey == 1101 || this.last_hotkey == 1105)
						{
							this.last_hotkey = 1102;
						}
						else
						{
							this.last_hotkey = 0;
						}
						goto IL_1B0;
					}
				}
				else
				{
					switch (num)
					{
					case 1175:
						handled = true;
						if (this.last_hotkey == 1102 && this.pass_cnt >= 6)
						{
							if (this.factory_mode)
							{
								this.factory_mode = false;
								this.LeaveFactoryMode();
							}
						}
						this.pass_cnt = 0;
						this.last_hotkey = 0;
						goto IL_1B0;
					case 1176:
						handled = true;
						if (this.last_hotkey == 1103)
						{
							this.last_hotkey = 1104;
						}
						else
						{
							this.last_hotkey = 0;
						}
						goto IL_1B0;
					default:
						switch (num)
						{
						case 1182:
							handled = true;
							this.last_hotkey = 1101;
							goto IL_1B0;
						case 1184:
							handled = true;
							if (this.last_hotkey == 1104)
							{
								this.last_hotkey = 1105;
							}
							else
							{
								this.last_hotkey = 0;
							}
							goto IL_1B0;
						}
						break;
					}
				}
				this.pass_cnt = 0;
				this.last_hotkey = 0;
				IL_1B0:
				if (this.last_hotkey == 0)
				{
					this.pass_cnt = 0;
				}
				else
				{
					this.pass_cnt++;
				}
			}
			return IntPtr.Zero;
		}

		private void OnActivated(object sender, EventArgs e)
		{
			string text = null;
			this.rectBoot.Visibility = Visibility.Hidden;
			this.rectFW1.Visibility = Visibility.Hidden;
			this.rectFW2.Visibility = Visibility.Hidden;
			this.rectUSER.Visibility = Visibility.Hidden;
			this.addrBootEnd.Visibility = Visibility.Hidden;
			this.addrFW1End.Visibility = Visibility.Hidden;
			this.addrFW2End.Visibility = Visibility.Hidden;
			this.addrUSEREnd.Visibility = Visibility.Hidden;
			this.addrBootStart.Visibility = Visibility.Hidden;
			this.addrFW1Start.Visibility = Visibility.Hidden;
			this.addrFW2Start.Visibility = Visibility.Hidden;
			this.addrUSERStart.Visibility = Visibility.Hidden;
			this.dumpButton.Visibility = Visibility.Hidden;
			this.initFlashButton.Visibility = Visibility.Hidden;
			this.rectDefault.Visibility = Visibility.Hidden;
			this.rectOTA.Visibility = Visibility.Hidden;
			this.rectUSER1.Visibility = Visibility.Hidden;
			this.addrDefaultEnd.Visibility = Visibility.Hidden;
			this.addrOTAEnd.Visibility = Visibility.Hidden;
			this.addrUSER1End.Visibility = Visibility.Hidden;
			this.addrDefaultStart.Visibility = Visibility.Hidden;
			this.addrOTAStart.Visibility = Visibility.Hidden;
			this.addrUSER1Start.Visibility = Visibility.Hidden;
			try
			{
				XmlReader xmlReader = XmlReader.Create("setting.xml", new XmlReaderSettings
				{
					IgnoreWhitespace = true
				});
				xmlReader.MoveToContent();
				xmlReader.ReadStartElement("setting", "");
				xmlReader.ReadStartElement("download", "");
				string value = xmlReader.ReadElementContentAsString("FlashSize", "");
				this.flashSizeCombo.SelectedIndex = Convert.ToInt32(value);
				this.flash_size = this.flash_table[this.flashSizeCombo.SelectedIndex];
				this.bootFileName.Text = xmlReader.ReadElementContentAsString("BOOTPath", "");
				this.fw1FileName.Text = xmlReader.ReadElementContentAsString("FW1Path", "");
				this.fw2FileName.Text = xmlReader.ReadElementContentAsString("FW2Path", "");
				this.offsetFW2.Text = xmlReader.ReadElementContentAsString("FW2Offset", "");
				this.userFileName.Text = xmlReader.ReadElementContentAsString("USERPath", "");
				this.offsetUSER.Text = xmlReader.ReadElementContentAsString("USEROffset", "");
				value = xmlReader.ReadElementContentAsString("Trigger1Ena", "");
				this.trig1Check.IsChecked = new bool?(Convert.ToBoolean(value));
				value = xmlReader.ReadElementContentAsString("Trigger1Level", "");
				this.trig1LevelCombo.SelectedIndex = Convert.ToInt32(value);
				value = xmlReader.ReadElementContentAsString("Trigger1Port", "");
				this.trig1PortCombo.SelectedIndex = Convert.ToInt32(value);
				value = xmlReader.ReadElementContentAsString("Trigger1Pin", "");
				this.trig1PinCombo.SelectedIndex = Convert.ToInt32(value);
				value = xmlReader.ReadElementContentAsString("Trigger2Ena", "");
				this.trig2Check.IsChecked = new bool?(Convert.ToBoolean(value));
				value = xmlReader.ReadElementContentAsString("Trigger2Level", "");
				this.trig2LevelCombo.SelectedIndex = Convert.ToInt32(value);
				value = xmlReader.ReadElementContentAsString("Trigger2Port", "");
				this.trig2PortCombo.SelectedIndex = Convert.ToInt32(value);
				value = xmlReader.ReadElementContentAsString("Trigger2Pin", "");
				this.trig2PinCombo.SelectedIndex = Convert.ToInt32(value);
				value = xmlReader.ReadElementContentAsString("VerifyBOOT", "");
				this.verifyBoot.IsChecked = new bool?(Convert.ToBoolean(value));
				value = xmlReader.ReadElementContentAsString("VerifyFW1", "");
				this.verifyFW1.IsChecked = new bool?(Convert.ToBoolean(value));
				value = xmlReader.ReadElementContentAsString("VerifyFW2", "");
				this.verifyFW2.IsChecked = new bool?(Convert.ToBoolean(value));
				value = xmlReader.ReadElementContentAsString("VerifyUSER", "");
				this.verifyUSER.IsChecked = new bool?(Convert.ToBoolean(value));
				text = xmlReader.ReadElementContentAsString("JlinkPath", "");
				value = xmlReader.ReadElementContentAsString("AdvMode", "");
				this.advModeCheck.IsChecked = new bool?(Convert.ToBoolean(value));
				value = xmlReader.ReadElementContentAsString("FactoryMode", "");
				this.factory_mode = Convert.ToBoolean(value);
				value = xmlReader.ReadElementContentAsString("EraseSkipCal", "");
				this.skipCalibrationCheck.IsChecked = new bool?(Convert.ToBoolean(value));
				xmlReader.ReadToNextSibling("generate", "");
				xmlReader.ReadStartElement("generate", "");
				value = xmlReader.ReadElementContentAsString("FlashSize", "");
				this.flashSizeCombo1.SelectedIndex = Convert.ToInt32(value);
				this.flash1_size = this.flash_table[this.flashSizeCombo1.SelectedIndex];
				this.defaultFileName.Text = xmlReader.ReadElementContentAsString("DefaultPath", "");
				this.otaFileName.Text = xmlReader.ReadElementContentAsString("OTAPath", "");
				this.offsetOTA.Text = xmlReader.ReadElementContentAsString("OTAOffset", "");
				this.user1FileName.Text = xmlReader.ReadElementContentAsString("USER1Path", "");
				this.offsetUSER1.Text = xmlReader.ReadElementContentAsString("USER1Offset", "");
				this.saveFileName.Text = xmlReader.ReadElementContentAsString("SAVEPath", "");
				value = xmlReader.ReadElementContentAsString("Trigger1Ena", "");
				this.trig1Check1.IsChecked = new bool?(Convert.ToBoolean(value));
				value = xmlReader.ReadElementContentAsString("Trigger1Level", "");
				this.trig1LevelCombo1.SelectedIndex = Convert.ToInt32(value);
				value = xmlReader.ReadElementContentAsString("Trigger1Port", "");
				this.trig1PortCombo1.SelectedIndex = Convert.ToInt32(value);
				value = xmlReader.ReadElementContentAsString("Trigger1Pin", "");
				this.trig1PinCombo1.SelectedIndex = Convert.ToInt32(value);
				value = xmlReader.ReadElementContentAsString("Trigger2Ena", "");
				this.trig2Check1.IsChecked = new bool?(Convert.ToBoolean(value));
				value = xmlReader.ReadElementContentAsString("Trigger2Level", "");
				this.trig2LevelCombo1.SelectedIndex = Convert.ToInt32(value);
				value = xmlReader.ReadElementContentAsString("Trigger2Port", "");
				this.trig2PortCombo1.SelectedIndex = Convert.ToInt32(value);
				value = xmlReader.ReadElementContentAsString("Trigger2Pin", "");
				this.trig2PinCombo1.SelectedIndex = Convert.ToInt32(value);
				value = xmlReader.ReadElementContentAsString("BootOption", "");
				this.bootCombo.SelectedIndex = Convert.ToInt32(value);
			}
			catch
			{
			}
			try
			{
				string name = "Software\\SEGGER\\J-Link";
				RegistryKey registryKey = Registry.CurrentUser;
				RegistryKey registryKey2 = registryKey.OpenSubKey(name);
				if (registryKey2 == null)
				{
					registryKey = Registry.LocalMachine;
					registryKey2 = registryKey.OpenSubKey(name);
				}
				if (registryKey2 != null)
				{
					this.jlinkpathString = (string)registryKey2.GetValue("InstallPath");
					string arg_758_0 = this.jlinkpathString;
					char[] trimChars = new char[1];
					this.jlinkpathString = arg_758_0.Trim(trimChars);
					this.jlinkpathString = this.jlinkpathString.Replace("\0", "");
				}
				this.tmppathString = System.IO.Path.GetTempPath();
			}
			catch
			{
			}
			if (this.jlinkpathString == null)
			{
				FileInfo fileInfo = new FileInfo(text + "JLink.exe");
				if (!fileInfo.Exists)
				{
					text = null;
				}
				if (text == null)
				{
					JlinkPath jlinkPath = new JlinkPath();
					jlinkPath.BringToFront();
					jlinkPath.ShowDialog();
					this.jlinkpathString = jlinkPath.selectJlinkPath;
				}
				else
				{
					this.jlinkpathString = text;
				}
				fileInfo = new FileInfo(this.jlinkpathString + "JLink.exe");
				if (!fileInfo.Exists)
				{
					MessageBox.Show("Invalid J-Link path");
				}
			}
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			if (this.advModeCheck.IsChecked == false)
			{
				this.flashOptGrid.IsEnabled = false;
				this.fw1Grid.IsEnabled = false;
				this.fw2Grid.IsEnabled = false;
				this.bootGrid.IsEnabled = false;
				this.sysDataGrid.IsEnabled = false;
				this.advFuncGrid.IsEnabled = false;
				this.clearButton.IsEnabled = false;
			}
			if (this.factory_mode)
			{
				this.EnterFactoryMode();
			}
		}

		private void OnClosed(object sender, EventArgs e)
		{
			XmlWriter xmlWriter = XmlWriter.Create("setting.xml", new XmlWriterSettings
			{
				Indent = true,
				OmitXmlDeclaration = true,
				NewLineOnAttributes = true
			});
			xmlWriter.WriteStartElement("setting");
			xmlWriter.WriteStartElement("download");
			xmlWriter.WriteElementString("FlashSize", this.flashSizeCombo.SelectedIndex.ToString());
			xmlWriter.WriteElementString("BOOTPath", (this.bootFileName.Text == null) ? "" : this.bootFileName.Text);
			xmlWriter.WriteElementString("FW1Path", (this.fw1FileName.Text == null) ? "" : this.fw1FileName.Text);
			xmlWriter.WriteElementString("FW2Path", (this.fw2FileName.Text == null) ? "" : this.fw2FileName.Text);
			xmlWriter.WriteElementString("FW2Offset", (this.offsetFW2.Text == null) ? "" : this.offsetFW2.Text);
			xmlWriter.WriteElementString("USERPath", (this.userFileName.Text == null) ? "" : this.userFileName.Text);
			xmlWriter.WriteElementString("USEROffset", (this.offsetUSER.Text == null) ? "" : this.offsetUSER.Text);
			xmlWriter.WriteElementString("Trigger1Ena", this.trig1Check.IsChecked.ToString());
			xmlWriter.WriteElementString("Trigger1Level", this.trig1LevelCombo.SelectedIndex.ToString());
			xmlWriter.WriteElementString("Trigger1Port", this.trig1PortCombo.SelectedIndex.ToString());
			xmlWriter.WriteElementString("Trigger1Pin", this.trig1PinCombo.SelectedIndex.ToString());
			xmlWriter.WriteElementString("Trigger2Ena", this.trig2Check.IsChecked.ToString());
			xmlWriter.WriteElementString("Trigger2Level", this.trig2LevelCombo.SelectedIndex.ToString());
			xmlWriter.WriteElementString("Trigger2Port", this.trig2PortCombo.SelectedIndex.ToString());
			xmlWriter.WriteElementString("Trigger2Pin", this.trig2PinCombo.SelectedIndex.ToString());
			xmlWriter.WriteElementString("VerifyBOOT", this.verifyBoot.IsChecked.ToString());
			xmlWriter.WriteElementString("VerifyFW1", this.verifyFW1.IsChecked.ToString());
			xmlWriter.WriteElementString("VerifyFW2", this.verifyFW2.IsChecked.ToString());
			xmlWriter.WriteElementString("VerifyUSER", this.verifyUSER.IsChecked.ToString());
			xmlWriter.WriteElementString("JlinkPath", this.jlinkpathString);
			xmlWriter.WriteElementString("AdvMode", this.advModeCheck.IsChecked.ToString());
			xmlWriter.WriteElementString("FactoryMode", this.factory_mode.ToString());
			xmlWriter.WriteElementString("EraseSkipCal", this.skipCalibrationCheck.IsChecked.ToString());
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("generate");
			xmlWriter.WriteElementString("FlashSize", this.flashSizeCombo1.SelectedIndex.ToString());
			xmlWriter.WriteElementString("DefaultPath", (this.defaultFileName.Text == null) ? "" : this.defaultFileName.Text);
			xmlWriter.WriteElementString("OTAPath", (this.otaFileName.Text == null) ? "" : this.otaFileName.Text);
			xmlWriter.WriteElementString("OTAOffset", (this.offsetOTA.Text == null) ? "" : this.offsetOTA.Text);
			xmlWriter.WriteElementString("USER1Path", (this.user1FileName.Text == null) ? "" : this.user1FileName.Text);
			xmlWriter.WriteElementString("USER1Offset", (this.offsetUSER1.Text == null) ? "" : this.offsetUSER1.Text);
			xmlWriter.WriteElementString("SAVEPath", (this.saveFileName.Text == null) ? "" : this.saveFileName.Text);
			xmlWriter.WriteElementString("Trigger1Ena", this.trig1Check1.IsChecked.ToString());
			xmlWriter.WriteElementString("Trigger1Level", this.trig1LevelCombo1.SelectedIndex.ToString());
			xmlWriter.WriteElementString("Trigger1Port", this.trig1PortCombo1.SelectedIndex.ToString());
			xmlWriter.WriteElementString("Trigger1Pin", this.trig1PinCombo1.SelectedIndex.ToString());
			xmlWriter.WriteElementString("Trigger2Ena", this.trig2Check1.IsChecked.ToString());
			xmlWriter.WriteElementString("Trigger2Level", this.trig2LevelCombo1.SelectedIndex.ToString());
			xmlWriter.WriteElementString("Trigger2Port", this.trig2PortCombo1.SelectedIndex.ToString());
			xmlWriter.WriteElementString("Trigger2Pin", this.trig2PinCombo1.SelectedIndex.ToString());
			xmlWriter.WriteElementString("BootOption", this.bootCombo.SelectedIndex.ToString());
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
			xmlWriter.Flush();
		}

		private void updateBootInfoUI()
		{
			try
			{
				Canvas.SetLeft(this.rectBoot, this.bootImgOffset * this.rectFlash.Width / (double)this.flash_size);
				this.rectBoot.Width = (double)this.bootImgLen * this.rectFlash.Width / (double)this.flash_size;
				this.rectBoot.Visibility = Visibility.Visible;
				this.addrBootEnd.Content = "0x" + ((long)this.bootImgLen + (long)((ulong)this.bootImgOffset)).ToString("X");
				this.addrBootEnd.Visibility = Visibility.Visible;
				this.addrBootStart.Content = this.offsetBoot.Text;
				this.addrBootStart.Visibility = Visibility.Visible;
			}
			catch
			{
				if (this.addrBootStart != null)
				{
					this.addrBootStart.Visibility = Visibility.Hidden;
				}
				if (this.addrBootEnd != null)
				{
					this.addrBootEnd.Visibility = Visibility.Hidden;
				}
			}
		}

		private void updateFW2InfoUI()
		{
			try
			{
				Canvas.SetLeft(this.rectFW2, this.fw2ImgOffset * this.rectFlash.Width / (double)this.flash_size);
				this.rectFW2.Width = (double)this.fw2ImgLen * this.rectFlash.Width / (double)this.flash_size;
				this.rectFW2.Visibility = Visibility.Visible;
				if ((ulong)this.fw2ImgOffset + (ulong)((long)this.fw2ImgLen) > (ulong)this.flash_size)
				{
					this.fw2ImgOffset = (uint)(this.flash_size - (long)this.fw2ImgLen);
					this.fw2ImgOffset &= 4294963200u;
					this.offsetFW2.Text = "0x" + this.fw2ImgOffset.ToString("X");
				}
				this.addrFW2End.Content = "0x" + ((long)this.fw2ImgLen + (long)((ulong)this.fw2ImgOffset)).ToString("X");
				this.addrFW2End.Visibility = Visibility.Visible;
				this.addrFW2Start.Content = this.offsetFW2.Text;
				this.addrFW2Start.Visibility = Visibility.Visible;
			}
			catch
			{
				if (this.addrFW2Start != null)
				{
					this.addrFW2Start.Visibility = Visibility.Hidden;
				}
				if (this.addrFW2End != null)
				{
					this.addrFW2End.Visibility = Visibility.Hidden;
				}
			}
		}

		private void updateFW1InfoUI()
		{
			try
			{
				Canvas.SetLeft(this.rectFW1, this.fw1ImgOffset * this.rectFlash.Width / (double)this.flash_size);
				this.rectFW1.Width = (double)this.fw1ImgLen * this.rectFlash.Width / (double)this.flash_size;
				this.rectFW1.Visibility = Visibility.Visible;
				if ((ulong)this.fw1ImgOffset + (ulong)((long)this.fw1ImgLen) > (ulong)this.flash_size)
				{
					this.fw1ImgOffset = (uint)(this.flash_size - (long)this.fw1ImgLen);
					this.fw1ImgOffset &= 4294963200u;
					this.offsetFW1.Text = "0x" + this.fw1ImgOffset.ToString("X");
				}
				this.addrFW1End.Content = "0x" + ((long)this.fw1ImgLen + (long)((ulong)this.fw1ImgOffset)).ToString("X");
				this.addrFW1End.Visibility = Visibility.Visible;
				this.addrFW1Start.Content = this.offsetFW1.Text;
				this.addrFW1Start.Visibility = Visibility.Visible;
			}
			catch
			{
				if (this.addrFW1Start != null)
				{
					this.addrFW1Start.Visibility = Visibility.Hidden;
				}
				if (this.addrFW1End != null)
				{
					this.addrFW1End.Visibility = Visibility.Hidden;
				}
			}
		}

		private void updateUSERInfoUI()
		{
			try
			{
				FileInfo fileInfo = new FileInfo(this.userFileName.Text);
				Canvas.SetLeft(this.rectUSER, this.userImgOffset * this.rectFlash.Width / (double)this.flash_size);
				this.rectUSER.Width = (double)this.userImgLen * this.rectFlash.Width / (double)this.flash_size;
				this.rectUSER.Visibility = Visibility.Visible;
				if ((ulong)this.userImgOffset + (ulong)((long)this.userImgLen) > (ulong)this.flash_size)
				{
					this.userImgOffset = (uint)(this.flash_size - (long)this.userImgLen);
					this.userImgOffset &= 4294963200u;
					this.offsetUSER.Text = "0x" + this.userImgOffset.ToString("X");
				}
				long num = (long)(this.userImgLen + 4095 & -4096);
				this.addrUSEREnd.Content = "0x" + (num + (long)((ulong)this.userImgOffset)).ToString("X");
				this.addrUSEREnd.Visibility = Visibility.Visible;
				this.addrUSERStart.Content = this.offsetUSER.Text;
				this.addrUSERStart.Visibility = Visibility.Visible;
			}
			catch
			{
				try
				{
					this.rectUSER.Visibility = Visibility.Hidden;
					this.addrUSEREnd.Visibility = Visibility.Hidden;
					this.addrUSERStart.Visibility = Visibility.Hidden;
				}
				catch
				{
				}
			}
		}

		private void userMouseDown(object sender, MouseButtonEventArgs e)
		{
			this.handleUserMouse = true;
		}

		private void fw2MouseDown(object sender, MouseButtonEventArgs e)
		{
			this.handleFW2Mouse = true;
		}

		private void fw1MouseDown(object sender, MouseButtonEventArgs e)
		{
		}

		private void user1MouseDown(object sender, MouseButtonEventArgs e)
		{
			this.handleUser1Mouse = true;
		}

		private void otaMouseDown(object sender, MouseButtonEventArgs e)
		{
			this.handleOTAMouse = true;
		}

		private void infoMouseLeave(object sender, MouseEventArgs e)
		{
			this.handleFW1Mouse = false;
			this.handleFW2Mouse = false;
			this.handleUserMouse = false;
			this.handleOTAMouse = false;
			this.handleUser1Mouse = false;
		}

		private void infoMouseUp(object sender, MouseButtonEventArgs e)
		{
			this.handleFW1Mouse = false;
			this.handleFW2Mouse = false;
			this.handleUserMouse = false;
			this.handleOTAMouse = false;
			this.handleUser1Mouse = false;
		}

		private void infoMouseMove(object sender, MouseEventArgs e)
		{
			if (this.handleFW1Mouse)
			{
				double num = e.GetPosition(this.rectFlash).X - this.rectFW1.Width / 2.0;
				if (num < 0.0)
				{
					num = 0.0;
				}
				if (num > this.rectFlash.Width - this.rectFW1.Width)
				{
					num = this.rectFlash.Width - this.rectFW1.Width;
				}
				int num2 = Convert.ToInt32(num * (double)this.flash_size / this.rectFlash.Width);
				num2 = (num2 + 4095 & -4096);
				this.offsetFW1.Text = "0x" + num2.ToString("X");
				this.fw1ImgOffset = (uint)num2;
				this.updateFW1InfoUI();
			}
			if (this.handleFW2Mouse)
			{
				double num = e.GetPosition(this.rectFlash).X - this.rectFW2.Width / 2.0;
				if (num < 0.0)
				{
					num = 0.0;
				}
				if (num > this.rectFlash.Width - this.rectFW2.Width)
				{
					num = this.rectFlash.Width - this.rectFW2.Width;
				}
				int num2 = Convert.ToInt32(num * (double)this.flash_size / this.rectFlash.Width);
				num2 = (num2 + 4095 & -4096);
				this.offsetFW2.Text = "0x" + num2.ToString("X");
				this.fw2ImgOffset = (uint)num2;
				this.updateFW2InfoUI();
			}
			if (this.handleUserMouse)
			{
				double num = e.GetPosition(this.rectFlash).X - this.rectUSER.Width / 2.0;
				if (num < 0.0)
				{
					num = 0.0;
				}
				if (num > this.rectFlash.Width - this.rectUSER.Width)
				{
					num = this.rectFlash.Width - this.rectUSER.Width;
				}
				int num2 = Convert.ToInt32(num * (double)this.flash_size / this.rectFlash.Width);
				num2 = (num2 + 4095 & -4096);
				this.offsetUSER.Text = "0x" + num2.ToString("X");
				this.userImgOffset = (uint)num2;
				this.updateUSERInfoUI();
			}
			if (this.handleOTAMouse)
			{
				double num = e.GetPosition(this.rectFlash1).X - this.rectOTA.Width / 2.0;
				if (num < 0.0)
				{
					num = 0.0;
				}
				if (num > this.rectFlash1.Width - this.rectOTA.Width)
				{
					num = this.rectFlash1.Width - this.rectOTA.Width;
				}
				int num2 = Convert.ToInt32(num * (double)this.flash1_size / this.rectFlash1.Width);
				num2 = (num2 + 4095 & -4096);
				this.offsetOTA.Text = "0x" + num2.ToString("X");
				this.otaImgOffset = (uint)num2;
				this.updateOTAInfoUI();
			}
			if (this.handleUser1Mouse)
			{
				double num = e.GetPosition(this.rectFlash1).X - this.rectUSER1.Width / 2.0;
				if (num < 0.0)
				{
					num = 0.0;
				}
				if (num > this.rectFlash1.Width - this.rectUSER1.Width)
				{
					num = this.rectFlash1.Width - this.rectUSER1.Width;
				}
				int num2 = Convert.ToInt32(num * (double)this.flash1_size / this.rectFlash1.Width);
				num2 = (num2 + 4095 & -4096);
				this.offsetUSER1.Text = "0x" + num2.ToString("X");
				this.user1ImgOffset = (uint)num2;
				this.updateUSER1InfoUI();
			}
		}

		public void writeImage1Sig()
		{
			string[] contents = new string[]
			{
				"speed auto",
				"w4 0x98000034 0x88167923",
				"Sleep 10",
				"exit"
			};
			File.WriteAllLines(this.tmppathString + "sigature.jlink", contents, Encoding.UTF8);
			Process process = Process.Start(new ProcessStartInfo
			{
				FileName = this.jlinkpathString + "JLink.exe",
				Arguments = "-CommanderScript " + this.tmppathString + "sigature.jlink",
				UseShellExecute = false,
				RedirectStandardOutput = true
			});
			try
			{
				string text = process.StandardOutput.ReadToEnd();
			}
			catch
			{
			}
			process.WaitForExit();
			process.Close();
		}

		public void downloadBuffer(byte[] buffer, bool is_verify, uint targetAddr)
		{
			FileStream fileStream = File.Open("tmp.bin", FileMode.Create);
			fileStream.Write(buffer, 0, buffer.Length);
			fileStream.Close();
			this.downloadFile("tmp.bin", is_verify, targetAddr);
			FileInfo fileInfo = new FileInfo("tmp.bin");
			fileInfo.Delete();
		}

		private bool downloadFile(string filename, bool is_verify, uint targetAddr)
		{
			bool result;
			if (this.jlinkpathString == null)
			{
				result = false;
			}
			else
			{
				bool flag = true;
				string[] array = new string[]
				{
					"speed auto",
					"w4 0x40000210 0x8001157",
					"Sleep 10",
					"w4 0x400002c0 0x110011",
					"Sleep 10",
					"w4 0x98000000 0x96969999",
					"w4 0x98000004 0xFC66CC3F",
					"w4 0x98000008 0x03CC33C0",
					"w4 0x9800000C 0x6231DCE5",
					"loadbin \"" + this.tmppathString + "loader.bin\" 0x10000bc8",
					"r",
					"SetPC 0x100",
					"g",
					"Sleep 100",
					"h"
				};
				string[] array2 = new string[]
				{
					"speed 300",
					"loadbin \"" + filename + "\", 0x" + targetAddr.ToString("X"),
					"exit"
				};
				StreamWriter streamWriter = new StreamWriter(this.tmppathString + "download.jlink", false, Encoding.UTF8);
				for (int i = 0; i < array.Length; i++)
				{
					streamWriter.WriteLine(array[i]);
				}
				for (int i = 0; i < array2.Length; i++)
				{
					streamWriter.WriteLine(array2[i]);
				}
				streamWriter.Close();
				Process process = Process.Start(new ProcessStartInfo
				{
					FileName = this.jlinkpathString + "JLink.exe",
					Arguments = "-CommanderScript " + this.tmppathString + "download.jlink"
				});
				process.WaitForExit();
				process.Close();
				if (is_verify)
				{
					FileInfo fileInfo = new FileInfo(filename);
					string[] contents = new string[]
					{
						"speed auto",
						"w4 0x40000210 0x8001157",
						"Sleep 10",
						"w4 0x400002c0 0x110011",
						"Sleep 10",
						"w4 0x98000000 0x96969999",
						"w4 0x98000004 0xFC66CC3F",
						"w4 0x98000008 0x03CC33C0",
						"w4 0x9800000C 0x6231DCE5",
						"loadbin \"" + this.tmppathString + "loader.bin\" 0x10000bc8",
						"r",
						"SetPC 0x100",
						"g",
						"Sleep 100",
						"h",
						"speed auto",
						string.Concat(new string[]
						{
							"savebin \"",
							this.tmppathString,
							"tmp.bin\", 0x",
							targetAddr.ToString("X"),
							", ",
							fileInfo.Length.ToString("X")
						}),
						"exit"
					};
					File.WriteAllLines(this.tmppathString + "verify.jlink", contents, Encoding.UTF8);
					process = Process.Start(new ProcessStartInfo
					{
						FileName = this.jlinkpathString + "JLink.exe",
						Arguments = "-CommanderScript " + this.tmppathString + "verify.jlink",
						UseShellExecute = false,
						RedirectStandardOutput = true
					});
					try
					{
						string text = process.StandardOutput.ReadToEnd();
					}
					catch
					{
					}
					process.WaitForExit();
					process.Close();
					flag = this.compareImage(filename, this.tmppathString + "tmp.bin", this.skipCalibrationCheck.IsChecked.Value, targetAddr);
				}
				result = flag;
			}
			return result;
		}

		private void initFlash(object sender, RoutedEventArgs e)
		{
			string[] contents = new string[]
			{
				"speed auto",
				"w4 0x40000210 0x8001157",
				"Sleep 10",
				"w4 0x400002c0 0x110011",
				"Sleep 10",
				"w4 0x98000000 0x96969999",
				"w4 0x98000004 0xFC66CC3F",
				"w4 0x98000008 0x03CC33C0",
				"w4 0x9800000C 0x6231DCE5",
				"loadbin \"" + this.tmppathString + "loader.bin\" 0x10000bc8",
				"r",
				"SetPC 0x100",
				"g",
				"Sleep 100",
				"h",
				"exit"
			};
			File.WriteAllLines(this.tmppathString + "initflash.jlink", contents, Encoding.UTF8);
			Process process = Process.Start(new ProcessStartInfo
			{
				FileName = this.jlinkpathString + "JLink.exe",
				Arguments = "-CommanderScript " + this.tmppathString + "initflash.jlink",
				UseShellExecute = false,
				RedirectStandardOutput = true
			});
			try
			{
				string text = process.StandardOutput.ReadToEnd();
			}
			catch
			{
			}
			process.WaitForExit();
			process.Close();
		}

		private int arrayCompare(byte[] a, byte[] b, int start, int end)
		{
			int result;
			for (int i = start; i < end; i++)
			{
				if (a[i] != b[i])
				{
					result = i;
					return result;
				}
			}
			result = -1;
			return result;
		}

		private bool compareImage(string orig, string flash, bool skipK, uint start)
		{
			int num = -1;
			uint num2 = start - 2550136832u;
			bool result;
			try
			{
				FileStream fileStream = File.Open(orig, FileMode.Open);
				FileStream fileStream2 = File.Open(flash, FileMode.Open);
				byte[] array = new byte[fileStream.Length];
				byte[] array2 = new byte[fileStream2.Length];
				fileStream.Read(array, 0, (int)fileStream.Length);
				fileStream2.Read(array2, 0, (int)fileStream2.Length);
				if (!skipK || num2 > 40960u || (num2 < 40960u && (ulong)(40960u - num2) >= (ulong)fileStream.Length))
				{
					num = this.arrayCompare(array, array2, 0, array.Length);
				}
				else
				{
					num = this.arrayCompare(array, array2, 0, (int)(36992u - num2));
					if (num < 0)
					{
						num = this.arrayCompare(array, array2, (int)(45056u - num2), array.Length);
					}
				}
				if (num < 0)
				{
					MessageBox.Show("verify success, image correct");
				}
				else
				{
					MessageBox.Show("verify fail!!\nfirst error " + array2[num].ToString("X") + " @ " + num.ToString("X"));
				}
				fileStream.Close();
				fileStream2.Close();
			}
			catch
			{
				MessageBox.Show("Cannot verify!!");
				result = false;
				return result;
			}
			result = (num < 0);
			return result;
		}

		private void eraseFlash(bool is_chiperase, uint startAddr, int length, bool reset_message)
		{
			if (this.jlinkpathString != null)
			{
				byte[] loader = MPbinary.Properties.Resources.loader;
				FileStream fileStream = File.Open(this.tmppathString + "loader.bin", FileMode.Create);
				fileStream.Write(loader, 0, loader.Length);
				fileStream.Close();
				string[] array = new string[]
				{
					"speed auto",
					"w4 0x40000210 0x8001157",
					"Sleep 10",
					"w4 0x400002c0 0x110011",
					"Sleep 10",
					"w4 0x98000000 0x96969999",
					"w4 0x98000004 0xFC66CC3F",
					"w4 0x98000008 0x03CC33C0",
					"w4 0x9800000C 0x6231DCE5",
					"loadbin \"" + this.tmppathString + "loader.bin\" 0x10000bc8",
					"r",
					"SetPC 0x100",
					"g",
					"Sleep 100",
					"h"
				};
				string[] array2 = new string[]
				{
					"w4 0x40006008 0x0",
					"w4 0x40006118 0x00000000",
					"w4 0x40006000 0x01000000",
					"w1 0x40006060 0x06",
					"w4 0x40006008 0x1",
					"Sleep 10",
					"w4 0x40006008 0x0",
					"Sleep 10",
					"w4 0x40006008 0x0",
					"w4 0x40006118 0x00000003",
					"w4 0x40006000 0x01000000",
					"w1 0x40006060 0x20",
					"w1 0x40006060 0x00",
					"w1 0x40006060 0x00",
					"w1 0x40006060 0x00",
					"w4 0x40006008 0x1",
					"Sleep 10",
					"w4 0x40006008 0x0",
					"Sleep 50"
				};
				string[] array3 = new string[]
				{
					"qc",
					"exit"
				};
				string[] contents = new string[]
				{
					"speed auto",
					"w4 0x40000210 0x8001157",
					"Sleep 10",
					"w4 0x400002c0 0x110011",
					"Sleep 10",
					"w4 0x98000000 0x96969999",
					"w4 0x98000004 0xFC66CC3F",
					"w4 0x98000008 0x03CC33C0",
					"w4 0x9800000C 0x6231DCE5",
					"loadbin \"" + this.tmppathString + "loader.bin\" 0x10000bc8",
					"r",
					"SetPC 0x100",
					"g",
					"Sleep 100",
					"h",
					"w4 0x40006008 0x0",
					"w4 0x40006118 0x00000000",
					"w4 0x40006000 0x01000000",
					"w1 0x40006060 0x06",
					"w4 0x40006008 0x1",
					"Sleep 10",
					"w4 0x40006008 0x0",
					"Sleep 10",
					"w4 0x40006008 0x0",
					"w4 0x40006118 0x00000000",
					"w4 0x40006000 0x01000000",
					"w1 0x40006060 0x60",
					"w4 0x40006008 0x1",
					"Sleep 10",
					"w4 0x40006008 0x0",
					"Sleep " + (5000L * this.flash_size / 1048576L).ToString(),
					"qc",
					"exit"
				};
				StreamWriter streamWriter = new StreamWriter(this.tmppathString + "erase_se.jlink");
				for (int i = 0; i < array.Length; i++)
				{
					streamWriter.WriteLine(array[i]);
				}
				uint num = startAddr;
				while ((ulong)num < (ulong)startAddr + (ulong)((long)length))
				{
					array2[12] = string.Format("w1 0x40006060 0x{0:x2}", num >> 16 & 255u);
					array2[13] = string.Format("w1 0x40006060 0x{0:x2}", num >> 8 & 255u);
					array2[14] = string.Format("w1 0x40006060 0x{0:x2}", num & 255u);
					for (int i = 0; i < array2.Length; i++)
					{
						streamWriter.WriteLine(array2[i]);
					}
					num += 4096u;
				}
				for (int i = 0; i < array3.Length; i++)
				{
					streamWriter.WriteLine(array3[i]);
				}
				streamWriter.Close();
				File.WriteAllLines(this.tmppathString + "erase_ce.jlink", contents, Encoding.UTF8);
				ProcessStartInfo processStartInfo = new ProcessStartInfo();
				processStartInfo.FileName = this.jlinkpathString + "JLink.exe";
				if (is_chiperase)
				{
					processStartInfo.Arguments = "-CommanderScript " + this.tmppathString + "erase_ce.jlink";
				}
				else
				{
					processStartInfo.Arguments = "-CommanderScript " + this.tmppathString + "erase_se.jlink";
				}
				Process process = Process.Start(processStartInfo);
				process.WaitForExit();
				if (reset_message && this.enalbe_reset_msg)
				{
					MessageBox.Show("Please RESET device");
				}
			}
		}

		private void dumpClick(object sender, RoutedEventArgs e)
		{
			if (this.jlinkpathString != null)
			{
				string[] contents = new string[]
				{
					"speed auto",
					"w4 0x400002c0 0x110011",
					"Sleep 10",
					"savebin \"dump.bin\", 0x98000000, " + this.flash_size.ToString("X"),
					"qc",
					"exit"
				};
				File.WriteAllLines(this.tmppathString + "dump.jlink", contents, Encoding.UTF8);
				Process process = Process.Start(new ProcessStartInfo
				{
					FileName = this.jlinkpathString + "JLink.exe",
					Arguments = "-CommanderScript " + this.tmppathString + "dump.jlink"
				});
				process.Close();
			}
		}

		private void readDataArea(uint flash_offset, byte[] buf)
		{
			if (this.jlinkpathString != null)
			{
				uint num = flash_offset + 2550136832u;
				string[] contents = new string[]
				{
					"speed auto",
					"w4 0x400002c0 0x110011",
					"Sleep 10",
					string.Concat(new string[]
					{
						"savebin ",
						flash_offset.ToString("X"),
						".dat, 0x",
						num.ToString("X"),
						", 1000"
					}),
					"qc",
					"exit"
				};
				File.WriteAllLines(this.tmppathString + "ota_addr.jlink", contents, Encoding.UTF8);
				Process process = Process.Start(new ProcessStartInfo
				{
					FileName = this.jlinkpathString + "JLink.exe",
					Arguments = "-CommanderScript " + this.tmppathString + "ota_addr.jlink"
				});
				process.WaitForExit();
				process.Close();
				FileStream fileStream = File.Open(flash_offset.ToString("X") + ".dat", FileMode.Open);
				fileStream.Read(buf, 0, 4096);
				fileStream.Close();
			}
		}

		private void fillSystemData(byte[] buf)
		{
			int num = 0;
			int num2 = 0;
			if (this.trig1Check.IsChecked == true)
			{
				num |= this.trig1LevelCombo.SelectedIndex << 7;
				num |= this.trig1PortCombo.SelectedIndex << 4;
				num |= this.trig1PinCombo.SelectedIndex;
			}
			else
			{
				num = 255;
			}
			if (this.trig2Check.IsChecked == true)
			{
				num2 |= this.trig2LevelCombo.SelectedIndex << 7;
				num2 |= this.trig2PortCombo.SelectedIndex << 4;
				num2 |= this.trig2PinCombo.SelectedIndex;
			}
			else
			{
				num2 = 255;
			}
			long num3 = 0L;
			try
			{
				num3 = (long)Convert.ToInt32(this.offsetFW2.Text);
			}
			catch
			{
				try
				{
					num3 = (long)Convert.ToInt32(this.offsetFW2.Text, 16);
				}
				catch
				{
				}
			}
			buf[8] = (byte)num;
			buf[9] = (byte)num2;
			buf[0] = (byte)(num3 & 255L);
			buf[1] = (byte)(num3 >> 8 & 255L);
			buf[2] = (byte)(num3 >> 16 & 255L);
			buf[3] = (byte)(num3 >> 24 & 255L);
		}

		private void parseSystemData(byte[] buf)
		{
			uint num = (uint)((int)buf[3] << 24 | (int)buf[2] << 16 | (int)buf[1] << 8 | (int)buf[0]);
			if (num > 67108864u)
			{
				num = 0u;
			}
			this.offsetFW2.Text = "0x" + num.ToString("X");
			if (buf[8] != 255)
			{
				this.trig1Check.IsChecked = new bool?(true);
				this.trig1LevelCombo.SelectedIndex = (buf[8] >> 7 & 1);
				this.trig1PortCombo.SelectedIndex = (buf[8] >> 4 & 7);
				this.trig1PinCombo.SelectedIndex = (int)(buf[8] & 15);
			}
			else
			{
				this.trig1Check.IsChecked = new bool?(false);
				this.trig1LevelCombo.SelectedIndex = 0;
				this.trig1PortCombo.SelectedIndex = 0;
				this.trig1PinCombo.SelectedIndex = 0;
			}
			if (buf[9] != 255)
			{
				this.trig2Check.IsChecked = new bool?(true);
				this.trig2LevelCombo.SelectedIndex = (buf[9] >> 7 & 1);
				this.trig2PortCombo.SelectedIndex = (buf[9] >> 4 & 7);
				this.trig2PinCombo.SelectedIndex = (int)(buf[9] & 15);
			}
			else
			{
				this.trig2Check.IsChecked = new bool?(false);
				this.trig2LevelCombo.SelectedIndex = 0;
				this.trig2PortCombo.SelectedIndex = 0;
				this.trig2PinCombo.SelectedIndex = 0;
			}
		}

		private void flashSizeChanged(object sender, EventArgs e)
		{
			this.flash_size = this.flash_table[this.flashSizeCombo.SelectedIndex];
			if (this.bootImgLen > 0)
			{
				Canvas.SetLeft(this.rectBoot, this.bootImgOffset * this.rectFlash.Width / (double)this.flash_size);
				this.rectBoot.Width = (double)this.bootImgLen * this.rectFlash.Width / (double)this.flash_size;
			}
			if (this.fw1ImgLen > 0)
			{
				if ((long)this.fw1ImgLen + (long)((ulong)this.fw1ImgOffset) > this.flash_size)
				{
					this.fw1ImgOffset = (uint)(this.flash_size - (long)this.fw1ImgLen);
					this.fw1ImgOffset &= 4294963200u;
					this.offsetFW1.Text = "0x" + this.fw1ImgOffset.ToString("X");
				}
				Canvas.SetLeft(this.rectFW1, this.fw1ImgOffset * this.rectFlash.Width / (double)this.flash_size);
				this.rectFW1.Width = (double)this.fw1ImgLen * this.rectFlash.Width / (double)this.flash_size;
			}
			if (this.fw2ImgLen > 0)
			{
				if ((long)this.fw2ImgLen + (long)((ulong)this.fw2ImgOffset) > this.flash_size)
				{
					this.fw2ImgOffset = (uint)(this.flash_size - (long)this.fw2ImgLen);
					this.fw2ImgOffset &= 4294963200u;
					this.offsetFW2.Text = "0x" + this.fw2ImgOffset.ToString("X");
				}
				Canvas.SetLeft(this.rectFW2, this.fw2ImgOffset * this.rectFlash.Width / (double)this.flash_size);
				this.rectFW2.Width = (double)this.fw2ImgLen * this.rectFlash.Width / (double)this.flash_size;
			}
			if (this.userImgLen > 0)
			{
				if ((long)this.userImgLen + (long)((ulong)this.userImgOffset) > this.flash_size)
				{
					this.userImgOffset = (uint)(this.flash_size - (long)this.userImgLen);
					this.userImgOffset &= 4294963200u;
					this.offsetUSER.Text = "0x" + this.userImgOffset.ToString("X");
				}
				Canvas.SetLeft(this.rectUSER, this.userImgOffset * this.rectFlash.Width / (double)this.flash_size);
				this.rectUSER.Width = (double)this.userImgLen * this.rectFlash.Width / (double)this.flash_size;
			}
		}

		private void enableHiddenFunc(object sender, MouseButtonEventArgs e)
		{
			if (this.hidden_cnt2 > 2)
			{
				if (this.hidden_cnt > 10)
				{
					int num = this.hidden_cnt;
					if (num == 11)
					{
						MessageBox.Show("Realtek 2015\n\nContact Info:\nchangyi.tsai@realtek.com\nsychou@realtek.com");
					}
				}
				if (this.hidden_cnt > 20)
				{
					this.dumpButton.Visibility = Visibility.Visible;
					this.initFlashButton.Visibility = Visibility.Visible;
				}
				this.hidden_cnt++;
			}
		}

		private void hiddenFuncEna2(object sender, MouseButtonEventArgs e)
		{
			this.hidden_cnt2++;
		}

		private bool fileExist(string filename)
		{
			bool result = false;
			try
			{
				FileInfo fileInfo = new FileInfo(filename);
				result = fileInfo.Exists;
			}
			catch
			{
				result = false;
			}
			return result;
		}

		private void openBoot(string filename)
		{
			if (this.fileExist(filename))
			{
				try
				{
					int num = 0;
					byte[] array = new byte[]
					{
						153,
						153,
						150,
						150,
						63,
						204,
						102,
						252,
						192,
						51,
						204,
						3,
						229,
						220,
						49,
						98
					};
					byte[] array2 = new byte[64];
					FileStream fileStream = File.Open(filename, FileMode.Open);
					fileStream.Read(array2, 0, 64);
					fileStream.Close();
					for (int i = 0; i < 16; i++)
					{
						num += (int)(array[i] - array2[i]);
					}
					if (num != 0)
					{
						MessageBox.Show("boot is not valid");
						this.bootFileName.Text = "";
					}
					this.fw1ImgOffset = (uint)(((int)array2[25] << 8 | (int)array2[24]) * 1024);
					this.offsetFW1.Text = "0x" + this.fw1ImgOffset.ToString("x");
					this.bootImgLen = ((int)array2[19] << 24 | (int)array2[18] << 16 | (int)array2[17] << 8 | (int)array2[16]);
				}
				catch
				{
				}
			}
			else
			{
				this.bootImgLen = 0;
			}
			this.updateBootInfoUI();
		}

		private void openFileBoot(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Binary Image|*.bin";
			openFileDialog.ShowDialog();
			this.bootFileName.Text = openFileDialog.FileName;
		}

		private bool checkFirwmareValid(string filename, int offset)
		{
			int num = 0;
			try
			{
				FileStream fileStream = File.Open(filename, FileMode.Open);
				FileInfo fileInfo = new FileInfo(filename);
				char[] array = new char[]
				{
					'R',
					'T',
					'K',
					'W',
					'i',
					'n'
				};
				byte[] array2 = new byte[64];
				fileStream.Seek((long)(offset + 20), SeekOrigin.Begin);
				fileStream.Read(array2, 0, 32);
				for (int i = 0; i < 6; i++)
				{
					num += (int)(array[i] - (char)array2[i]);
				}
				fileStream.Close();
			}
			catch
			{
				num = 255;
			}
			return num == 0;
		}

		private void openFW1(string filename)
		{
			int num = 0;
			if (this.fileExist(filename))
			{
				try
				{
					int num2 = 0;
					byte[] array = new byte[]
					{
						153,
						153,
						150,
						150,
						63,
						204,
						102,
						252,
						192,
						51,
						204,
						3,
						229,
						220,
						49,
						98
					};
					byte[] array2 = new byte[64];
					FileStream fileStream = File.Open(filename, FileMode.Open);
					fileStream.Read(array2, 0, 64);
					for (int i = 0; i < 16; i++)
					{
						num2 += (int)(array[i] - array2[i]);
					}
					if (num2 == 0)
					{
						this.fw1IsRamAll = true;
						num = ((int)array2[25] << 8 | (int)array2[24]) * 1024;
					}
					else
					{
						this.fw1IsRamAll = false;
						num = 0;
					}
					fileStream.Close();
				}
				catch
				{
				}
				if (this.checkFirwmareValid(filename, num))
				{
					FileInfo fileInfo = new FileInfo(filename);
					this.fw1ImgLen = (int)fileInfo.Length - num;
				}
				else
				{
					MessageBox.Show("Upgraded firmware is not valid");
					this.fw1FileName.Text = "";
					this.fw1ImgLen = 0;
				}
			}
			else
			{
				this.fw1ImgLen = 0;
			}
			this.updateFW1InfoUI();
		}

		private void openFileFW1(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Binary Image|*.bin";
			openFileDialog.ShowDialog();
			this.fw1FileName.Text = openFileDialog.FileName;
		}

		private void openFW2(string filename)
		{
			if (this.fileExist(filename))
			{
				if (this.checkFirwmareValid(filename, 0))
				{
					FileInfo fileInfo = new FileInfo(filename);
					this.fw2ImgLen = (int)fileInfo.Length;
				}
				else
				{
					MessageBox.Show("Upgraded firmware is not valid");
					this.fw2FileName.Text = "";
					this.fw2ImgLen = 0;
				}
			}
			else
			{
				this.fw2ImgLen = 0;
			}
			this.updateFW2InfoUI();
		}

		private void openFileFW2(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Binary Image|*.bin";
			openFileDialog.ShowDialog();
			this.fw2FileName.Text = openFileDialog.FileName;
		}

		private void openUser(string filename)
		{
			try
			{
				FileInfo fileInfo = new FileInfo(filename);
				this.userImgLen = (int)fileInfo.Length;
			}
			catch
			{
				this.userImgLen = 0;
			}
			this.updateUSERInfoUI();
		}

		private void openFileUSER(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Binary Image|*.bin";
			openFileDialog.ShowDialog();
			this.userFileName.Text = openFileDialog.FileName;
		}

		private void eraseBoot(object sender, RoutedEventArgs e)
		{
			if (this.bootImgLen != 0)
			{
				this.eraseFlash(false, this.bootImgOffset, this.bootImgLen, true);
			}
		}

		private void eraseFW1(object sender, RoutedEventArgs e)
		{
			if (this.fw1ImgOffset == 0u)
			{
				this.fw1ImgOffset = 45056u;
			}
			if (this.fw1ImgLen != 0)
			{
				this.eraseFlash(false, this.fw1ImgOffset, this.fw1ImgLen, true);
			}
		}

		private void eraseFW2(object sender, RoutedEventArgs e)
		{
			if (this.fw2ImgOffset != 0u)
			{
				if (this.fw2ImgLen != 0)
				{
					this.eraseFlash(false, this.fw2ImgOffset, this.fw2ImgLen, true);
				}
			}
		}

		private void eraseUSER(object sender, RoutedEventArgs e)
		{
			if (this.userImgLen != 0)
			{
				if (this.skipCalibrationCheck.IsChecked == true)
				{
					if (this.userImgOffset < 40960u)
					{
						if ((long)this.userImgLen > (long)((ulong)(45056u - this.userImgOffset)))
						{
							this.eraseFlash(false, this.userImgOffset, (int)(40960u - this.userImgOffset), false);
							this.eraseFlash(false, 45056u, this.userImgLen - (int)(45056u - this.userImgOffset), true);
						}
						else
						{
							this.eraseFlash(false, this.userImgOffset, this.userImgLen, true);
						}
					}
					else
					{
						this.eraseFlash(false, this.userImgOffset, this.userImgLen, true);
					}
				}
				else
				{
					this.eraseFlash(false, this.userImgOffset, this.userImgLen, true);
				}
			}
		}

		private void downloadBoot(object sender, RoutedEventArgs e)
		{
			if (this.bootImgLen != 0)
			{
				this.downloadFile(this.bootFileName.Text, this.verifyBoot.IsChecked.Value, this.bootImgOffset + 2550136832u);
			}
		}

		private void setSignature(string filename, int sig_type)
		{
			byte[] array = new byte[8];
			if (sig_type == 0)
			{
				array[0] = 56;
			}
			else
			{
				array[0] = 48;
			}
			array[1] = 49;
			array[2] = 57;
			array[3] = 53;
			array[4] = 56;
			array[5] = 55;
			array[6] = 49;
			array[7] = 49;
			BinaryWriter binaryWriter = new BinaryWriter(File.Open(filename, FileMode.Open, FileAccess.ReadWrite));
			binaryWriter.BaseStream.Seek(8L, SeekOrigin.Begin);
			binaryWriter.Write(array);
			binaryWriter.Close();
		}

		private void downloadFW1(object sender, RoutedEventArgs e)
		{
			if (this.fw1ImgOffset == 0u)
			{
				this.fw1ImgOffset = 45056u;
			}
			if (this.fw1ImgLen != 0)
			{
				if (this.defaultFW1Check.IsChecked == true)
				{
					this.setSignature(this.fw1FileName.Text, 0);
				}
				else
				{
					this.setSignature(this.fw1FileName.Text, 1);
				}
				this.downloadFile(this.fw1FileName.Text, this.verifyFW1.IsChecked.Value, this.fw1ImgOffset + 2550136832u);
			}
		}

		private void downloadFW2(object sender, RoutedEventArgs e)
		{
			if (this.fw2ImgOffset != 0u)
			{
				if (this.fw2ImgOffset != 0u)
				{
					if (this.defaultFW2Check.IsChecked == true)
					{
						this.setSignature(this.fw2FileName.Text, 0);
					}
					else
					{
						this.setSignature(this.fw2FileName.Text, 1);
					}
					this.downloadFile(this.fw2FileName.Text, this.verifyFW2.IsChecked.Value, this.fw2ImgOffset + 2550136832u);
				}
			}
		}

		private void downloadUSER(object sender, RoutedEventArgs e)
		{
			bool flag = false;
			if (this.userImgLen != 0)
			{
				if (this.userImgOffset == 0u)
				{
					FileStream fileStream = File.Open(this.userFileName.Text, FileMode.Open);
					byte[] array = new byte[fileStream.Length];
					fileStream.Read(array, 0, (int)fileStream.Length);
					fileStream.Close();
					int num = 0;
					byte[] array2 = new byte[]
					{
						153,
						153,
						150,
						150,
						63,
						204,
						102,
						252,
						192,
						51,
						204,
						3,
						229,
						220,
						49,
						98
					};
					for (int i = 0; i < 16; i++)
					{
						num += (int)(array2[i] - array[i]);
					}
					if (num != 0)
					{
						MessageBox.Show("Image is not valid");
						return;
					}
					if (array[52] != 35 || array[53] != 121 || array[54] != 22 || array[55] != 136)
					{
						MessageBox.Show("Image is not valid");
						return;
					}
					array[52] = 255;
					array[53] = 255;
					array[54] = 255;
					array[55] = 255;
					flag = true;
					FileStream fileStream2 = File.Open(this.tmppathString + "target.bin", FileMode.Create);
					fileStream2.Write(array, 0, array.Length);
					fileStream2.Close();
				}
				if (this.factory_mode)
				{
					this.eraseUSER(sender, e);
				}
				bool flag2;
				if (flag)
				{
					flag2 = this.downloadFile(this.tmppathString + "target.bin", this.verifyUSER.IsChecked.Value, this.userImgOffset + 2550136832u);
				}
				else
				{
					flag2 = this.downloadFile(this.userFileName.Text, this.verifyUSER.IsChecked.Value, this.userImgOffset + 2550136832u);
				}
				if (flag && flag2)
				{
					this.writeImage1Sig();
				}
			}
		}

		private void saveCfgClick(object sender, RoutedEventArgs e)
		{
			if (this.system_data == null)
			{
				this.system_data = new byte[4096];
				for (int i = 0; i < 4096; i++)
				{
					this.system_data[i] = 255;
				}
			}
			this.fillSystemData(this.system_data);
			this.eraseFlash(false, 36864u, 4096, false);
			this.downloadBuffer(this.system_data, false, 2550173696u);
		}

		private void loadCfgClick(object sender, RoutedEventArgs e)
		{
			if (this.system_data == null)
			{
				this.system_data = new byte[4096];
			}
			this.readDataArea(36864u, this.system_data);
			this.parseSystemData(this.system_data);
		}

		private void editSysClick(object sender, RoutedEventArgs e)
		{
			if (this.system_data == null)
			{
				this.system_data = new byte[4096];
			}
			this.readDataArea(36864u, this.system_data);
			Table0 table = new Table0();
			table.SetBaseAddr(36864);
			table.SetMemoryBuffer(this.system_data);
			table.ShowDialog();
			if (table.force_sync)
			{
				if (table.need_erase)
				{
					this.eraseFlash(false, 36864u, 4096, false);
				}
				this.downloadBuffer(this.system_data, false, 2550173696u);
			}
		}

		private void viewCalClick(object sender, RoutedEventArgs e)
		{
			if (this.calibrate_data == null)
			{
				this.calibrate_data = new byte[4096];
			}
			this.readDataArea(40960u, this.calibrate_data);
			Table0 table = new Table0();
			table.SetBaseAddr(40960);
			table.SetMemoryBuffer(this.calibrate_data);
			table.DisableEdit();
			table.ShowDialog();
		}

		private void eraseFlash(object sender, RoutedEventArgs e)
		{
			uint num = 0u;
			uint num2 = 0u;
			switch (this.eraseCombo.SelectedIndex)
			{
			case 0:
				this.eraseFlash(true, 0u, 0, true);
				break;
			case 1:
				this.eraseFlash(false, 0u, 40960, false);
				this.eraseFlash(false, 45056u, (int)(this.flash_size - 40960L), true);
				break;
			case 2:
				this.eraseFlash(false, 40960u, 4096, true);
				break;
			case 3:
				this.eraseFlash(false, 36864u, 4096, true);
				break;
			case 4:
				try
				{
					num = Convert.ToUInt32(this.eraseRangeStart.Text);
				}
				catch
				{
					try
					{
						num = Convert.ToUInt32(this.eraseRangeStart.Text, 16);
					}
					catch
					{
						num = 0u;
					}
				}
				try
				{
					num2 = Convert.ToUInt32(this.eraseRangeEnd.Text);
				}
				catch
				{
					try
					{
						num2 = Convert.ToUInt32(this.eraseRangeEnd.Text, 16);
					}
					catch
					{
						num2 = 0u;
					}
				}
				if (num2 > num && (ulong)num2 < (ulong)this.flash_size)
				{
					this.eraseFlash(false, num, (int)(num2 - num), true);
				}
				else
				{
					MessageBox.Show("Invalid range");
				}
				break;
			}
		}

		private void offsetBOOTChanged(object sender, TextChangedEventArgs e)
		{
		}

		private void offsetFW1Changed(object sender, TextChangedEventArgs e)
		{
			try
			{
				this.fw1ImgOffset = Convert.ToUInt32(this.offsetFW1.Text);
			}
			catch
			{
				try
				{
					this.fw1ImgOffset = Convert.ToUInt32(this.offsetFW1.Text, 16);
				}
				catch
				{
					this.fw1ImgOffset = 0u;
				}
			}
			if (this.fw1ImgLen > 0)
			{
				this.updateFW1InfoUI();
			}
		}

		private void offsetFW2Changed(object sender, TextChangedEventArgs e)
		{
			try
			{
				this.fw2ImgOffset = Convert.ToUInt32(this.offsetFW2.Text);
			}
			catch
			{
				try
				{
					this.fw2ImgOffset = Convert.ToUInt32(this.offsetFW2.Text, 16);
				}
				catch
				{
					this.fw2ImgOffset = 0u;
				}
			}
			if (this.fw2ImgLen > 0)
			{
				this.updateFW2InfoUI();
			}
		}

		private void offsetUSERChange(object sender, TextChangedEventArgs e)
		{
			try
			{
				this.userImgOffset = Convert.ToUInt32(this.offsetUSER.Text);
			}
			catch
			{
				try
				{
					this.userImgOffset = Convert.ToUInt32(this.offsetUSER.Text, 16);
				}
				catch
				{
					this.userImgOffset = 0u;
				}
			}
			if (this.userImgLen > 0)
			{
				this.updateUSERInfoUI();
			}
		}

		private void eraseSelectChanged(object sender, SelectionChangedEventArgs e)
		{
			if (this.eraseRangeEnd != null)
			{
				if (this.eraseRangeStart != null)
				{
					if (this.eraseCombo.SelectedIndex == 4)
					{
						this.eraseRangeStart.IsEnabled = true;
						this.eraseRangeEnd.IsEnabled = true;
					}
					else
					{
						this.eraseRangeStart.IsEnabled = false;
						this.eraseRangeEnd.IsEnabled = false;
					}
				}
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this.bootFileName.Text = "";
			this.fw1FileName.Text = "";
			this.fw2FileName.Text = "";
			this.userFileName.Text = "";
			this.offsetFW2.Text = "0";
			this.offsetUSER.Text = "0";
		}

		private void bootImgChanged(object sender, TextChangedEventArgs e)
		{
			this.openBoot(this.bootFileName.Text);
		}

		private void fw1ImgChanged(object sender, TextChangedEventArgs e)
		{
			this.openFW1(this.fw1FileName.Text);
		}

		private void fw2ImgChanged(object sender, TextChangedEventArgs e)
		{
			this.openFW2(this.fw2FileName.Text);
		}

		private void userImgChanged(object sender, TextChangedEventArgs e)
		{
			this.openUser(this.userFileName.Text);
		}

		private void fw1BootClicked(object sender, RoutedEventArgs e)
		{
			if (this.defaultFW1Check.IsChecked == true)
			{
				this.defaultFW2Check.IsEnabled = false;
			}
			else
			{
				this.defaultFW2Check.IsEnabled = true;
			}
		}

		private void fw2BootClicked(object sender, RoutedEventArgs e)
		{
			if (this.defaultFW2Check.IsChecked == true)
			{
				this.defaultFW1Check.IsEnabled = false;
			}
			else
			{
				this.defaultFW1Check.IsEnabled = true;
			}
		}

		private void keepCalClicked(object sender, RoutedEventArgs e)
		{
		}

		private void AdvModeClick(object sender, RoutedEventArgs e)
		{
			if (this.advModeCheck.IsChecked == true)
			{
				this.flashOptGrid.IsEnabled = true;
				this.fw1Grid.IsEnabled = true;
				this.fw2Grid.IsEnabled = true;
				this.bootGrid.IsEnabled = true;
				this.sysDataGrid.IsEnabled = true;
				this.advFuncGrid.IsEnabled = true;
				this.clearButton.IsEnabled = true;
			}
			else
			{
				this.flashOptGrid.IsEnabled = false;
				this.fw1Grid.IsEnabled = false;
				this.fw2Grid.IsEnabled = false;
				this.bootGrid.IsEnabled = false;
				this.sysDataGrid.IsEnabled = false;
				this.advFuncGrid.IsEnabled = false;
				this.clearButton.IsEnabled = false;
			}
		}

		private void EnterFactoryMode()
		{
			this.gridFactoryUpper.Visibility = Visibility.Hidden;
			this.gridFactoryBottom.Visibility = Visibility.Hidden;
			this.tabGenerate.Visibility = Visibility.Hidden;
			this.gridFactoryUserOff.IsEnabled = false;
			Thickness margin = this.gridFactoryCenter.Margin;
			margin.Top = this.gridFactoryUpper.Margin.Top;
			this.gridFactoryCenter.Margin = margin;
			base.Height -= this.pageDownload.ActualHeight - this.gridFactoryCenter.Height - this.tabGenerate.ActualHeight;
			base.Title += " - Factory";
			this.downloadUSERButton.ToolTip = "Erase and Download";
		}

		private void LeaveFactoryMode()
		{
			this.gridFactoryUpper.Visibility = Visibility.Visible;
			this.gridFactoryBottom.Visibility = Visibility.Visible;
			this.tabGenerate.Visibility = Visibility.Visible;
			this.gridFactoryUserOff.IsEnabled = true;
			Thickness margin = this.gridFactoryCenter.Margin;
			margin.Top = this.gridFactoryUpper.Margin.Top + this.gridFactoryUpper.Height;
			this.gridFactoryCenter.Margin = margin;
			base.Height += this.pageDownload.ActualHeight - this.gridFactoryCenter.Height - this.tabGenerate.ActualHeight;
			base.Title = "Ameba Image Tool";
			this.downloadUSERButton.ToolTip = "Only Download";
		}

		private void factoryClick(object sender, RoutedEventArgs e)
		{
			MessageBoxResult messageBoxResult = MessageBox.Show("Enter facotry mode will hide all advanced functions,\nwant to continue?", "Factory Mode", MessageBoxButton.YesNo);
			if (messageBoxResult == MessageBoxResult.Yes)
			{
				this.factory_mode = true;
				this.EnterFactoryMode();
			}
		}

		private void apply_signature(byte[] target)
		{
			if ((long)target.Length > (long)((ulong)this.otaImgOffset))
			{
				if (this.bootCombo.SelectedIndex == 1)
				{
					target[(int)((UIntPtr)(this.otaImgOffset + 8u))] = 56;
				}
				else
				{
					target[(int)((UIntPtr)(this.otaImgOffset + 8u))] = 48;
				}
				target[(int)((UIntPtr)(this.otaImgOffset + 9u))] = 49;
				target[(int)((UIntPtr)(this.otaImgOffset + 10u))] = 57;
				target[(int)((UIntPtr)(this.otaImgOffset + 11u))] = 53;
				target[(int)((UIntPtr)(this.otaImgOffset + 12u))] = 56;
				target[(int)((UIntPtr)(this.otaImgOffset + 13u))] = 55;
				target[(int)((UIntPtr)(this.otaImgOffset + 14u))] = 49;
				target[(int)((UIntPtr)(this.otaImgOffset + 15u))] = 49;
			}
			int num = ((int)target[25] << 8 | (int)target[24]) * 1024;
			if ((long)target.Length > (long)((ulong)this.otaImgOffset))
			{
				if (this.bootCombo.SelectedIndex == 1)
				{
					target[num + 8] = 48;
				}
				else
				{
					target[num + 8] = 56;
				}
			}
			else
			{
				target[num + 8] = 56;
			}
			target[num + 9] = 49;
			target[num + 10] = 57;
			target[num + 11] = 53;
			target[num + 12] = 56;
			target[num + 13] = 55;
			target[num + 14] = 49;
			target[num + 15] = 49;
		}

		private void openDefault(string filename)
		{
			this.defaultImgLen = 0;
			if (this.fileExist(filename))
			{
				try
				{
					int num = 0;
					byte[] array = new byte[]
					{
						153,
						153,
						150,
						150,
						63,
						204,
						102,
						252,
						192,
						51,
						204,
						3,
						229,
						220,
						49,
						98
					};
					byte[] array2 = new byte[64];
					FileStream fileStream = File.Open(filename, FileMode.Open);
					FileInfo fileInfo = new FileInfo(filename);
					fileStream.Read(array2, 0, 64);
					fileStream.Close();
					for (int i = 0; i < 16; i++)
					{
						num += (int)(array[i] - array2[i]);
					}
					if (num != 0)
					{
						MessageBox.Show("Default firmware is not valid");
					}
					this.defaultImgLen = (int)fileInfo.Length;
				}
				catch
				{
				}
			}
			this.updateDefaultInfoUI();
		}

		private void openFileDefault(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Binary Image|*.bin";
			openFileDialog.ShowDialog();
			this.defaultFileName.Text = openFileDialog.FileName;
		}

		private void defaultChanged(object sender, TextChangedEventArgs e)
		{
			this.openDefault(this.defaultFileName.Text);
		}

		private void openOTA(string filename)
		{
			this.otaImgLen = 0;
			if (this.fileExist(filename))
			{
				try
				{
					FileStream fileStream = File.Open(filename, FileMode.Open);
					FileInfo fileInfo = new FileInfo(filename);
					char[] array = new char[]
					{
						'R',
						'T',
						'K',
						'W',
						'i',
						'n'
					};
					byte[] array2 = new byte[64];
					int num = 0;
					fileStream.Seek(20L, SeekOrigin.Begin);
					fileStream.Read(array2, 0, 32);
					fileStream.Close();
					for (int i = 0; i < 6; i++)
					{
						num += (int)(array[i] - (char)array2[i]);
					}
					if (num != 0)
					{
						MessageBox.Show("Upgraded firmware is not valid");
					}
					this.otaImgLen = (int)fileInfo.Length;
				}
				catch
				{
				}
			}
			this.updateOTAInfoUI();
		}

		private void openFileOTA(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Binary Image|*.bin";
			openFileDialog.ShowDialog();
			this.otaFileName.Text = openFileDialog.FileName;
		}

		private void otaChanged(object sender, TextChangedEventArgs e)
		{
			this.openOTA(this.otaFileName.Text);
		}

		private void openFileUSER1(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Binary Image|*.bin";
			openFileDialog.ShowDialog();
			this.user1FileName.Text = openFileDialog.FileName;
			try
			{
				FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
				this.user1ImgLen = (int)fileInfo.Length;
			}
			catch
			{
				this.user1ImgLen = 0;
			}
			this.updateUSER1InfoUI();
		}

		private void openFileSave(object sender, RoutedEventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "Binary Image|*.bin";
			saveFileDialog.OverwritePrompt = false;
			saveFileDialog.ShowDialog();
			this.saveFileName.Text = saveFileDialog.FileName;
		}

		private void flash1SizeChanged(object sender, EventArgs e)
		{
			this.flash1_size = this.flash_table[this.flashSizeCombo1.SelectedIndex];
			if (this.defaultImgLen > 0)
			{
				Canvas.SetLeft(this.rectDefault, this.defaultImgOffset * this.rectFlash1.Width / (double)this.flash1_size);
				this.rectDefault.Width = (double)this.defaultImgLen * this.rectFlash1.Width / (double)this.flash1_size;
			}
			if (this.otaImgLen > 0)
			{
				if ((long)this.otaImgLen + (long)((ulong)this.otaImgOffset) > this.flash1_size)
				{
					this.otaImgOffset = (uint)(this.flash1_size - (long)this.otaImgLen);
					this.otaImgOffset &= 4294963200u;
					this.offsetOTA.Text = "0x" + this.otaImgOffset.ToString("X");
				}
				Canvas.SetLeft(this.rectOTA, this.otaImgOffset * this.rectFlash1.Width / (double)this.flash1_size);
				this.rectOTA.Width = (double)this.otaImgLen * this.rectFlash1.Width / (double)this.flash1_size;
			}
			if (this.user1ImgLen > 0)
			{
				if ((long)this.user1ImgLen + (long)((ulong)this.user1ImgOffset) > this.flash1_size)
				{
					this.user1ImgOffset = (uint)(this.flash1_size - (long)this.user1ImgLen);
					this.user1ImgOffset &= 4294963200u;
					this.offsetUSER1.Text = "0x" + this.user1ImgOffset.ToString("X");
				}
				Canvas.SetLeft(this.rectUSER1, this.user1ImgOffset * this.rectFlash1.Width / (double)this.flash1_size);
				this.rectUSER1.Width = (double)this.user1ImgLen * this.rectFlash1.Width / (double)this.flash1_size;
			}
		}

		private void checkOverlap()
		{
			if ((ulong)this.otaImgOffset < (ulong)((long)this.defaultImgLen))
			{
				if (this.otaImgLen > 0)
				{
					this.offsetOTA.Background = Brushes.Red;
				}
			}
			else if (this.otaImgOffset > this.user1ImgOffset && (ulong)this.otaImgOffset < (ulong)this.user1ImgOffset + (ulong)((long)this.user1ImgLen))
			{
				if (this.otaImgLen > 0)
				{
					this.offsetOTA.Background = Brushes.Red;
				}
				if (this.user1ImgLen > 0)
				{
					this.offsetUSER1.Background = Brushes.Red;
				}
			}
			else if (this.user1ImgOffset > this.otaImgOffset && (ulong)this.user1ImgOffset < (ulong)this.otaImgOffset + (ulong)((long)this.otaImgLen))
			{
				if (this.otaImgLen > 0)
				{
					this.offsetOTA.Background = Brushes.Red;
				}
				if (this.user1ImgLen > 0)
				{
					this.offsetUSER1.Background = Brushes.Red;
				}
			}
			else
			{
				this.offsetOTA.Background = Brushes.White;
				if ((this.user1ImgOffset < this.otaImgOffset || (ulong)this.user1ImgOffset > (ulong)this.otaImgOffset + (ulong)((long)this.otaImgLen)) && (ulong)this.user1ImgOffset > (ulong)((long)this.defaultImgLen))
				{
					this.offsetUSER1.Background = Brushes.White;
				}
			}
			if ((ulong)this.user1ImgOffset < (ulong)((long)this.defaultImgLen))
			{
				if (this.user1ImgLen > 0)
				{
					this.offsetUSER1.Background = Brushes.Red;
				}
			}
			else if (this.otaImgOffset > this.user1ImgOffset && (ulong)this.otaImgOffset < (ulong)this.user1ImgOffset + (ulong)((long)this.user1ImgLen))
			{
				if (this.otaImgLen > 0)
				{
					this.offsetOTA.Background = Brushes.Red;
				}
				if (this.user1ImgLen > 0)
				{
					this.offsetUSER1.Background = Brushes.Red;
				}
			}
			else if (this.user1ImgOffset > this.otaImgOffset && (ulong)this.user1ImgOffset < (ulong)this.otaImgOffset + (ulong)((long)this.otaImgLen))
			{
				if (this.otaImgLen > 0)
				{
					this.offsetOTA.Background = Brushes.Red;
				}
				if (this.user1ImgLen > 0)
				{
					this.offsetUSER1.Background = Brushes.Red;
				}
			}
			else
			{
				this.offsetUSER.Background = Brushes.White;
				if ((this.otaImgOffset < this.user1ImgOffset || (ulong)this.otaImgOffset > (ulong)this.user1ImgOffset + (ulong)((long)this.user1ImgLen)) && (ulong)this.otaImgOffset > (ulong)((long)this.defaultImgLen))
				{
					this.offsetOTA.Background = Brushes.White;
				}
			}
		}

		private void updateDefaultInfoUI()
		{
			try
			{
				Canvas.SetLeft(this.rectDefault, this.defaultImgOffset * this.rectFlash1.Width / (double)this.flash1_size);
				this.rectDefault.Width = (double)this.defaultImgLen * this.rectFlash1.Width / (double)this.flash1_size;
				if (this.defaultImgLen > 0)
				{
					this.rectDefault.Visibility = Visibility.Visible;
				}
				else
				{
					this.rectDefault.Visibility = Visibility.Hidden;
				}
				this.addrDefaultEnd.Content = "0x" + ((long)this.defaultImgLen + (long)((ulong)this.defaultImgOffset)).ToString("X");
				this.addrDefaultEnd.Visibility = Visibility.Visible;
				this.addrDefaultStart.Content = "0x0000";
				this.addrDefaultStart.Visibility = Visibility.Visible;
			}
			catch
			{
				if (this.addrDefaultStart != null)
				{
					this.addrDefaultStart.Visibility = Visibility.Hidden;
				}
				if (this.addrDefaultEnd != null)
				{
					this.addrDefaultEnd.Visibility = Visibility.Hidden;
				}
			}
		}

		private void updateOTAInfoUI()
		{
			this.checkOverlap();
			try
			{
				Canvas.SetLeft(this.rectOTA, this.otaImgOffset * this.rectFlash1.Width / (double)this.flash1_size);
				this.rectOTA.Width = (double)this.otaImgLen * this.rectFlash1.Width / (double)this.flash1_size;
				if (this.otaImgLen > 0)
				{
					this.rectOTA.Visibility = Visibility.Visible;
				}
				else
				{
					this.rectOTA.Visibility = Visibility.Hidden;
				}
				if ((ulong)this.otaImgOffset + (ulong)((long)this.otaImgLen) > (ulong)this.flash_size)
				{
					this.otaImgOffset = (uint)(this.flash_size - (long)this.otaImgLen);
					this.otaImgOffset &= 4294963200u;
					this.offsetOTA.Text = "0x" + this.otaImgOffset.ToString("X");
				}
				this.addrOTAEnd.Content = "0x" + ((long)this.otaImgLen + (long)((ulong)this.otaImgOffset)).ToString("X");
				this.addrOTAEnd.Visibility = Visibility.Visible;
				this.addrOTAStart.Content = this.offsetOTA.Text;
				this.addrOTAStart.Visibility = Visibility.Visible;
			}
			catch
			{
				if (this.addrOTAStart != null)
				{
					this.addrOTAStart.Visibility = Visibility.Hidden;
				}
				if (this.addrOTAEnd != null)
				{
					this.addrOTAEnd.Visibility = Visibility.Hidden;
				}
			}
		}

		private void updateUSER1InfoUI()
		{
			this.checkOverlap();
			try
			{
				Canvas.SetLeft(this.rectUSER1, this.user1ImgOffset * this.rectFlash1.Width / (double)this.flash1_size);
				this.rectUSER1.Width = (double)this.user1ImgLen * this.rectFlash1.Width / (double)this.flash1_size;
				if (this.user1ImgLen > 0)
				{
					this.rectUSER1.Visibility = Visibility.Visible;
				}
				else
				{
					this.rectUSER1.Visibility = Visibility.Hidden;
				}
				if ((ulong)this.user1ImgOffset + (ulong)((long)this.user1ImgLen) > (ulong)this.flash1_size)
				{
					this.user1ImgOffset = (uint)(this.flash1_size - (long)this.user1ImgLen);
					this.user1ImgOffset &= 4294963200u;
					this.offsetUSER1.Text = "0x" + this.user1ImgOffset.ToString("X");
				}
				this.addrUSER1End.Content = "0x" + ((long)this.user1ImgLen + (long)((ulong)this.user1ImgOffset)).ToString("X");
				this.addrUSER1End.Visibility = Visibility.Visible;
				this.addrUSER1Start.Content = this.offsetUSER1.Text;
				this.addrUSER1Start.Visibility = Visibility.Visible;
			}
			catch
			{
				if (this.addrUSER1Start != null)
				{
					this.addrUSER1Start.Visibility = Visibility.Hidden;
				}
				if (this.addrUSER1End != null)
				{
					this.addrUSER1End.Visibility = Visibility.Hidden;
				}
			}
		}

		private void offsetOTAChanged(object sender, TextChangedEventArgs e)
		{
			try
			{
				this.otaImgOffset = Convert.ToUInt32(this.offsetOTA.Text);
			}
			catch
			{
				try
				{
					this.otaImgOffset = Convert.ToUInt32(this.offsetOTA.Text, 16);
				}
				catch
				{
				}
			}
			this.updateOTAInfoUI();
		}

		private void offsetUSER1Changed(object sender, TextChangedEventArgs e)
		{
			try
			{
				this.user1ImgOffset = Convert.ToUInt32(this.offsetUSER1.Text);
			}
			catch
			{
				try
				{
					this.user1ImgOffset = Convert.ToUInt32(this.offsetUSER1.Text, 16);
				}
				catch
				{
				}
			}
			this.updateUSER1InfoUI();
		}

		private void generateClick(object sender, RoutedEventArgs e)
		{
			bool flag = true;
			FileStream fileStream = null;
			FileStream fileStream2 = null;
			FileStream fileStream3 = null;
			FileStream fileStream4 = null;
			try
			{
				fileStream = File.Open(this.defaultFileName.Text, FileMode.Open);
				fileStream3 = File.Open(this.saveFileName.Text, FileMode.Create);
			}
			catch
			{
				MessageBox.Show("file name invalid");
				flag = false;
			}
			try
			{
				fileStream2 = File.Open(this.otaFileName.Text, FileMode.Open);
			}
			catch
			{
				fileStream2 = null;
			}
			try
			{
				fileStream4 = File.Open(this.user1FileName.Text, FileMode.Open);
			}
			catch
			{
				fileStream4 = null;
			}
			if (flag)
			{
				int num = this.defaultImgLen + 4095 & -4096;
				if ((ulong)this.otaImgOffset < (ulong)((long)num))
				{
					MessageBox.Show("OTA Image overlap Default Image");
				}
				else
				{
					long num2 = (long)((ulong)this.otaImgOffset + (ulong)((long)this.otaImgLen));
					long num3 = (long)((ulong)this.user1ImgOffset + (ulong)((long)this.user1ImgLen));
					long num4 = (num2 > num3) ? num2 : num3;
					byte[] array = new byte[num4];
					int i = 0;
					while ((long)i < num4)
					{
						array[i] = 255;
						i++;
					}
					fileStream.Read(array, 0, this.defaultImgLen);
					if (fileStream2 != null)
					{
						fileStream2.Read(array, (int)this.otaImgOffset, this.otaImgLen);
					}
					if (fileStream4 != null)
					{
						fileStream4.Read(array, (int)this.user1ImgOffset, this.user1ImgLen);
					}
					if (this.system_data_gen == null)
					{
						this.system_data_gen = new byte[4096];
						for (i = 0; i < 4096; i++)
						{
							this.system_data_gen[i] = 255;
						}
						this.fillSystemData1(this.system_data_gen);
					}
					Array.Copy(this.system_data_gen, 0, array, 36864, 4096);
					this.apply_signature(array);
					fileStream3.Write(array, 0, (int)num4);
					if (fileStream4 != null)
					{
						fileStream4.Close();
					}
					if (fileStream != null)
					{
						fileStream.Close();
					}
					if (fileStream2 != null)
					{
						fileStream2.Close();
					}
					if (fileStream3 != null)
					{
						fileStream3.Close();
					}
					MessageBox.Show("Done");
				}
			}
		}

		private void parseSystemData1(byte[] buf)
		{
			uint num = (uint)((int)buf[3] << 24 | (int)buf[2] << 16 | (int)buf[1] << 8 | (int)buf[0]);
			if (num > 67108864u)
			{
				num = 0u;
			}
			this.offsetOTA.Text = "0x" + num.ToString("X");
			if (buf[8] != 255)
			{
				this.trig1Check1.IsChecked = new bool?(true);
				this.trig1LevelCombo1.SelectedIndex = (buf[8] >> 7 & 1);
				this.trig1PortCombo1.SelectedIndex = (buf[8] >> 4 & 7);
				this.trig1PinCombo1.SelectedIndex = (int)(buf[8] & 15);
			}
			else
			{
				this.trig1Check1.IsChecked = new bool?(false);
				this.trig1LevelCombo1.SelectedIndex = 0;
				this.trig1PortCombo1.SelectedIndex = 0;
				this.trig1PinCombo1.SelectedIndex = 0;
			}
			if (buf[9] != 255)
			{
				this.trig2Check1.IsChecked = new bool?(true);
				this.trig2LevelCombo1.SelectedIndex = (buf[9] >> 7 & 1);
				this.trig2PortCombo1.SelectedIndex = (buf[9] >> 4 & 7);
				this.trig2PinCombo1.SelectedIndex = (int)(buf[9] & 15);
			}
			else
			{
				this.trig2Check1.IsChecked = new bool?(false);
				this.trig2LevelCombo1.SelectedIndex = 0;
				this.trig2PortCombo1.SelectedIndex = 0;
				this.trig2PinCombo1.SelectedIndex = 0;
			}
		}

		private void fillSystemData1(byte[] buf)
		{
			int num = 0;
			int num2 = 0;
			if (this.trig1Check1.IsChecked == true)
			{
				num |= this.trig1LevelCombo1.SelectedIndex << 7;
				num |= this.trig1PortCombo1.SelectedIndex << 4;
				num |= this.trig1PinCombo1.SelectedIndex;
			}
			else
			{
				num = 255;
			}
			if (this.trig2Check1.IsChecked == true)
			{
				num2 |= this.trig2LevelCombo1.SelectedIndex << 7;
				num2 |= this.trig2PortCombo1.SelectedIndex << 4;
				num2 |= this.trig2PinCombo1.SelectedIndex;
			}
			else
			{
				num2 = 255;
			}
			long num3 = 0L;
			try
			{
				num3 = (long)Convert.ToInt32(this.offsetOTA.Text);
			}
			catch
			{
				try
				{
					num3 = (long)Convert.ToInt32(this.offsetOTA.Text, 16);
				}
				catch
				{
				}
			}
			buf[8] = (byte)num;
			buf[9] = (byte)num2;
			buf[0] = (byte)(num3 & 255L);
			buf[1] = (byte)(num3 >> 8 & 255L);
			buf[2] = (byte)(num3 >> 16 & 255L);
			buf[3] = (byte)(num3 >> 24 & 255L);
		}

		private void loadSysDataClick(object sender, RoutedEventArgs e)
		{
			if (this.system_data_gen == null)
			{
				this.system_data_gen = new byte[4096];
			}
			this.readDataArea(36864u, this.system_data_gen);
			this.parseSystemData1(this.system_data_gen);
		}

		private void editSystemDataGen(object sender, RoutedEventArgs e)
		{
			if (this.system_data_gen == null)
			{
				this.system_data_gen = new byte[4096];
			}
			for (int i = 0; i < 4096; i++)
			{
				this.system_data_gen[i] = 255;
			}
			this.fillSystemData1(this.system_data_gen);
			Table0 table = new Table0();
			table.SetBaseAddr(36864);
			table.SetMemoryBuffer(this.system_data_gen);
			table.HideSync();
			table.ShowDialog();
			this.parseSystemData1(this.system_data_gen);
		}

		private void enableHiddenFunc2(object sender, MouseButtonEventArgs e)
		{
		}

		private void hiddenFuncEnaGen(object sender, MouseButtonEventArgs e)
		{
		}
        /*
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), DebuggerNonUserCode]
		public void InitializeComponent()
		{
			if (!this._contentLoaded)
			{
				this._contentLoaded = true;
				Uri resourceLocator = new Uri("/ImageTool;component/mainwindow.xaml", UriKind.Relative);
				Application.LoadComponent(this, resourceLocator);
			}
		}

		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never), DebuggerNonUserCode]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				this.Window1 = (MainWindow)target;
				this.Window1.Closed += new EventHandler(this.OnClosed);
				this.Window1.Initialized += new EventHandler(this.OnActivated);
				this.Window1.Loaded += new RoutedEventHandler(this.OnLoaded);
				break;
			case 2:
				((Grid)target).MouseMove += new MouseEventHandler(this.infoMouseMove);
				((Grid)target).MouseLeftButtonUp += new MouseButtonEventHandler(this.infoMouseUp);
				((Grid)target).MouseLeave += new MouseEventHandler(this.infoMouseLeave);
				break;
			case 3:
				this.pageDownload = (Grid)target;
				break;
			case 4:
				this.gridFactoryUpper = (Grid)target;
				break;
			case 5:
				this.initFlashButton = (Button)target;
				this.initFlashButton.Click += new RoutedEventHandler(this.initFlash);
				break;
			case 6:
				this.clearButton = (Button)target;
				this.clearButton.Click += new RoutedEventHandler(this.Button_Click);
				break;
			case 7:
				this.advModeCheck = (CheckBox)target;
				this.advModeCheck.Click += new RoutedEventHandler(this.AdvModeClick);
				break;
			case 8:
				this.tabBootImg = (GroupBox)target;
				break;
			case 9:
				this.bootGrid = (Grid)target;
				break;
			case 10:
				this.bootFileName = (TextBox)target;
				this.bootFileName.TextChanged += new TextChangedEventHandler(this.bootImgChanged);
				break;
			case 11:
				((Button)target).Click += new RoutedEventHandler(this.openFileBoot);
				break;
			case 12:
				((Button)target).Click += new RoutedEventHandler(this.eraseBoot);
				break;
			case 13:
				((Button)target).Click += new RoutedEventHandler(this.downloadBoot);
				break;
			case 14:
				this.verifyBoot = (CheckBox)target;
				break;
			case 15:
				this.offsetBoot = (TextBox)target;
				this.offsetBoot.TextChanged += new TextChangedEventHandler(this.offsetBOOTChanged);
				break;
			case 16:
				this.tabFW2Img = (GroupBox)target;
				break;
			case 17:
				this.fw2Grid = (Grid)target;
				break;
			case 18:
				((Button)target).Click += new RoutedEventHandler(this.openFileFW2);
				break;
			case 19:
				this.offsetFW2 = (TextBox)target;
				this.offsetFW2.TextChanged += new TextChangedEventHandler(this.offsetFW2Changed);
				break;
			case 20:
				this.fw2FileName = (TextBox)target;
				this.fw2FileName.TextChanged += new TextChangedEventHandler(this.fw2ImgChanged);
				break;
			case 21:
				((Button)target).Click += new RoutedEventHandler(this.eraseFW2);
				break;
			case 22:
				((Button)target).Click += new RoutedEventHandler(this.downloadFW2);
				break;
			case 23:
				this.verifyFW2 = (CheckBox)target;
				break;
			case 24:
				this.defaultFW2Check = (CheckBox)target;
				this.defaultFW2Check.Click += new RoutedEventHandler(this.fw2BootClicked);
				break;
			case 25:
				this.tabFW1Img = (GroupBox)target;
				break;
			case 26:
				this.fw1Grid = (Grid)target;
				break;
			case 27:
				((Button)target).Click += new RoutedEventHandler(this.openFileFW1);
				break;
			case 28:
				this.offsetFW1 = (TextBox)target;
				this.offsetFW1.TextChanged += new TextChangedEventHandler(this.offsetFW1Changed);
				break;
			case 29:
				this.fw1FileName = (TextBox)target;
				this.fw1FileName.TextChanged += new TextChangedEventHandler(this.fw1ImgChanged);
				break;
			case 30:
				((Button)target).Click += new RoutedEventHandler(this.eraseFW1);
				break;
			case 31:
				((Button)target).Click += new RoutedEventHandler(this.downloadFW1);
				break;
			case 32:
				this.verifyFW1 = (CheckBox)target;
				break;
			case 33:
				this.defaultFW1Check = (CheckBox)target;
				this.defaultFW1Check.Click += new RoutedEventHandler(this.fw1BootClicked);
				break;
			case 34:
				this.gridFactoryBottom = (Grid)target;
				break;
			case 35:
				this.tabSysData = (GroupBox)target;
				break;
			case 36:
				this.sysDataGrid = (Grid)target;
				break;
			case 37:
				this.trig1PinCombo = (ComboBox)target;
				break;
			case 38:
				this.trig1PortCombo = (ComboBox)target;
				break;
			case 39:
				this.trig1Check = (CheckBox)target;
				break;
			case 40:
				this.trig2Check = (CheckBox)target;
				break;
			case 41:
				this.trig2PortCombo = (ComboBox)target;
				break;
			case 42:
				this.trig2PinCombo = (ComboBox)target;
				break;
			case 43:
				this.trig1LevelCombo = (ComboBox)target;
				break;
			case 44:
				this.trig2LevelCombo = (ComboBox)target;
				break;
			case 45:
				this.tabInfo = (GroupBox)target;
				break;
			case 46:
				this.hiddenFunc2 = (Rectangle)target;
				this.hiddenFunc2.MouseRightButtonDown += new MouseButtonEventHandler(this.hiddenFuncEna2);
				break;
			case 47:
				this.addrBootStart = (Label)target;
				break;
			case 48:
				this.addrBootEnd = (Label)target;
				break;
			case 49:
				this.addrFW2Start = (Label)target;
				break;
			case 50:
				this.addrFW2End = (Label)target;
				break;
			case 51:
				this.addrUSERStart = (Label)target;
				break;
			case 52:
				this.addrUSEREnd = (Label)target;
				break;
			case 53:
				this.addrFW1Start = (Label)target;
				break;
			case 54:
				this.addrFW1End = (Label)target;
				break;
			case 55:
				this.tabFlash = (GroupBox)target;
				break;
			case 56:
				this.flashOptGrid = (Grid)target;
				break;
			case 57:
				this.eraseCombo = (ComboBox)target;
				this.eraseCombo.SelectionChanged += new SelectionChangedEventHandler(this.eraseSelectChanged);
				break;
			case 58:
				this.eraseFlashButton = (Button)target;
				this.eraseFlashButton.Click += new RoutedEventHandler(this.eraseFlash);
				break;
			case 59:
				this.flashSizeCombo = (ComboBox)target;
				this.flashSizeCombo.DropDownClosed += new EventHandler(this.flashSizeChanged);
				break;
			case 60:
				this.dumpButton = (Button)target;
				this.dumpButton.Click += new RoutedEventHandler(this.dumpClick);
				break;
			case 61:
				this.eraseRangeStart = (TextBox)target;
				break;
			case 62:
				this.eraseRangeEnd = (TextBox)target;
				break;
			case 63:
				this.advFuncGrid = (Grid)target;
				break;
			case 64:
				this.editSysButton = (Button)target;
				this.editSysButton.Click += new RoutedEventHandler(this.editSysClick);
				break;
			case 65:
				this.viewCalButton = (Button)target;
				this.viewCalButton.Click += new RoutedEventHandler(this.viewCalClick);
				break;
			case 66:
				this.loadCfgButton = (Button)target;
				this.loadCfgButton.Click += new RoutedEventHandler(this.loadCfgClick);
				break;
			case 67:
				((Button)target).Click += new RoutedEventHandler(this.saveCfgClick);
				break;
			case 68:
				this.factoryButton = (Button)target;
				this.factoryButton.Click += new RoutedEventHandler(this.factoryClick);
				break;
			case 69:
				this.canvasFlash = (Canvas)target;
				break;
			case 70:
				this.rectFlash = (Rectangle)target;
				break;
			case 71:
				this.rectBoot = (Rectangle)target;
				break;
			case 72:
				this.rectFW1 = (Rectangle)target;
				this.rectFW1.MouseLeftButtonDown += new MouseButtonEventHandler(this.fw1MouseDown);
				break;
			case 73:
				this.rectUSER = (Rectangle)target;
				this.rectUSER.MouseLeftButtonDown += new MouseButtonEventHandler(this.userMouseDown);
				break;
			case 74:
				this.rectFW2 = (Rectangle)target;
				this.rectFW2.MouseLeftButtonDown += new MouseButtonEventHandler(this.fw2MouseDown);
				break;
			case 75:
				this.gridFactoryCenter = (Grid)target;
				break;
			case 76:
				this.hiddenFunc = (Image)target;
				this.hiddenFunc.MouseRightButtonDown += new MouseButtonEventHandler(this.enableHiddenFunc);
				break;
			case 77:
				this.tabUSERimg = (GroupBox)target;
				break;
			case 78:
				this.downloadUSERButton = (Button)target;
				this.downloadUSERButton.Click += new RoutedEventHandler(this.downloadUSER);
				break;
			case 79:
				this.gridFactoryUserOff = (Grid)target;
				break;
			case 80:
				((Button)target).Click += new RoutedEventHandler(this.openFileUSER);
				break;
			case 81:
				this.offsetUSER = (TextBox)target;
				this.offsetUSER.TextChanged += new TextChangedEventHandler(this.offsetUSERChange);
				break;
			case 82:
				this.userFileName = (TextBox)target;
				this.userFileName.TextChanged += new TextChangedEventHandler(this.userImgChanged);
				break;
			case 83:
				this.verifyUSER = (CheckBox)target;
				break;
			case 84:
				this.skipCalibrationCheck = (CheckBox)target;
				this.skipCalibrationCheck.Click += new RoutedEventHandler(this.keepCalClicked);
				break;
			case 85:
				((Button)target).Click += new RoutedEventHandler(this.eraseUSER);
				break;
			case 86:
				this.tabGenerate = (TabItem)target;
				break;
			case 87:
				this.defaultFileName = (TextBox)target;
				this.defaultFileName.TextChanged += new TextChangedEventHandler(this.defaultChanged);
				break;
			case 88:
				((Button)target).Click += new RoutedEventHandler(this.openFileDefault);
				break;
			case 89:
				((Button)target).Click += new RoutedEventHandler(this.openFileOTA);
				break;
			case 90:
				this.offsetOTA = (TextBox)target;
				this.offsetOTA.TextChanged += new TextChangedEventHandler(this.offsetOTAChanged);
				break;
			case 91:
				this.otaFileName = (TextBox)target;
				this.otaFileName.TextChanged += new TextChangedEventHandler(this.otaChanged);
				break;
			case 92:
				this.saveFileName = (TextBox)target;
				break;
			case 93:
				((Button)target).Click += new RoutedEventHandler(this.openFileSave);
				break;
			case 94:
				((Button)target).Click += new RoutedEventHandler(this.openFileUSER1);
				break;
			case 95:
				this.offsetUSER1 = (TextBox)target;
				this.offsetUSER1.TextChanged += new TextChangedEventHandler(this.offsetUSER1Changed);
				break;
			case 96:
				this.user1FileName = (TextBox)target;
				break;
			case 97:
				this.flashSizeCombo1 = (ComboBox)target;
				this.flashSizeCombo1.DropDownClosed += new EventHandler(this.flash1SizeChanged);
				break;
			case 98:
				this.trig1PinCombo1 = (ComboBox)target;
				break;
			case 99:
				this.trig1PortCombo1 = (ComboBox)target;
				break;
			case 100:
				this.trig1Check1 = (CheckBox)target;
				break;
			case 101:
				this.trig2Check1 = (CheckBox)target;
				break;
			case 102:
				this.trig2PortCombo1 = (ComboBox)target;
				break;
			case 103:
				this.trig2PinCombo1 = (ComboBox)target;
				break;
			case 104:
				this.trig1LevelCombo1 = (ComboBox)target;
				break;
			case 105:
				this.trig2LevelCombo1 = (ComboBox)target;
				break;
			case 106:
				this.bootCombo = (ComboBox)target;
				break;
			case 107:
				this.hiddenFunc1 = (Rectangle)target;
				this.hiddenFunc1.MouseRightButtonDown += new MouseButtonEventHandler(this.hiddenFuncEnaGen);
				break;
			case 108:
				this.addrDefaultStart = (Label)target;
				break;
			case 109:
				this.addrDefaultEnd = (Label)target;
				break;
			case 110:
				this.addrOTAStart = (Label)target;
				break;
			case 111:
				this.addrOTAEnd = (Label)target;
				break;
			case 112:
				this.addrUSER1Start = (Label)target;
				break;
			case 113:
				this.addrUSER1End = (Label)target;
				break;
			case 114:
				this.generateButton = (Button)target;
				this.generateButton.Click += new RoutedEventHandler(this.generateClick);
				break;
			case 115:
				this.hiddenFunc3 = (Image)target;
				this.hiddenFunc3.MouseRightButtonDown += new MouseButtonEventHandler(this.enableHiddenFunc2);
				break;
			case 116:
				this.refreshButton = (Button)target;
				this.refreshButton.Click += new RoutedEventHandler(this.loadSysDataClick);
				break;
			case 117:
				((Button)target).Click += new RoutedEventHandler(this.editSystemDataGen);
				break;
			case 118:
				this.canvasFlash_Copy = (Canvas)target;
				break;
			case 119:
				this.rectFlash1 = (Rectangle)target;
				break;
			case 120:
				this.rectDefault = (Rectangle)target;
				break;
			case 121:
				this.rectOTA = (Rectangle)target;
				this.rectOTA.MouseLeftButtonDown += new MouseButtonEventHandler(this.otaMouseDown);
				break;
			case 122:
				this.rectUSER1 = (Rectangle)target;
				this.rectUSER1.MouseLeftButtonDown += new MouseButtonEventHandler(this.user1MouseDown);
				break;
			default:
				this._contentLoaded = true;
				break;
			}
		}
        */
	}
}
