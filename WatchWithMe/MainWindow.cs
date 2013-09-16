using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WatchWithMe
{
	public partial class MainWindow : Form
	{
		private MpchcLocalMediaPlayer _localMediaPlayer;

		public MainWindow()
		{
			InitializeComponent();

			_localMediaPlayer = new MpchcLocalMediaPlayer(Handle);
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == (uint) WM.COPYDATA)
			{
				_localMediaPlayer.MpcMessageReceivedHandler(m);
			}

			base.WndProc(ref m);
		}

		private void MainWindow_Load(object sender, EventArgs e)
		{
			_localMediaPlayer.MpcMessageReceived += MpcMessageReceived;
		}

		private void MpcMessageReceived(MpchcLocalMediaPlayer.MpcMessage m)
		{
			if (_localMediaPlayer.State == PlayState.Playing)
				mpcStatusLabel.Text = "Playing";

			if (_localMediaPlayer.State == PlayState.Stopped)
				mpcStatusLabel.Text = "Stopped";

			if (_localMediaPlayer.State == PlayState.Paused)
				mpcStatusLabel.Text = "Paused";
		}

		private void selectMpcLocationButton_Click(object sender, EventArgs e)
		{
			OpenMpcFileDialog.ShowDialog(this);
			mpcLocationTextBox.Text = OpenMpcFileDialog.FileName;
		}

		private void mpcLocationTextBox_TextChanged(object sender, EventArgs e)
		{
			openMpcButton.Enabled = mpcLocationTextBox.Text != "" && File.Exists(mpcLocationTextBox.Text);
		}

		private void openMpcButton_Click(object sender, EventArgs e)
		{
			Process.Start(mpcLocationTextBox.Text, "/slave " + Handle);
		}
	}
}
