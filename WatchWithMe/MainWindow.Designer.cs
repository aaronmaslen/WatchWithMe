namespace WatchWithMe
{
	partial class MainWindow
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
			this.selectMpcLocationLabel = new System.Windows.Forms.Label();
			this.mpcLocationTextBox = new System.Windows.Forms.TextBox();
			this.selectMpcLocationButton = new System.Windows.Forms.Button();
			this.OpenMpcFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.openMpcButton = new System.Windows.Forms.Button();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.mpcStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.statusStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// selectMpcLocationLabel
			// 
			this.selectMpcLocationLabel.AutoSize = true;
			this.selectMpcLocationLabel.Location = new System.Drawing.Point(13, 13);
			this.selectMpcLocationLabel.Name = "selectMpcLocationLabel";
			this.selectMpcLocationLabel.Size = new System.Drawing.Size(232, 13);
			this.selectMpcLocationLabel.TabIndex = 0;
			this.selectMpcLocationLabel.Text = "Location of Media Player Classic Home Cinema:";
			// 
			// mpcLocationTextBox
			// 
			this.mpcLocationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mpcLocationTextBox.Location = new System.Drawing.Point(12, 29);
			this.mpcLocationTextBox.Name = "mpcLocationTextBox";
			this.mpcLocationTextBox.Size = new System.Drawing.Size(194, 20);
			this.mpcLocationTextBox.TabIndex = 1;
			this.mpcLocationTextBox.TextChanged += new System.EventHandler(this.mpcLocationTextBox_TextChanged);
			// 
			// selectMpcLocationButton
			// 
			this.selectMpcLocationButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.selectMpcLocationButton.Location = new System.Drawing.Point(212, 29);
			this.selectMpcLocationButton.Name = "selectMpcLocationButton";
			this.selectMpcLocationButton.Size = new System.Drawing.Size(89, 20);
			this.selectMpcLocationButton.TabIndex = 2;
			this.selectMpcLocationButton.Text = "Select Location";
			this.selectMpcLocationButton.UseVisualStyleBackColor = true;
			this.selectMpcLocationButton.Click += new System.EventHandler(this.selectMpcLocationButton_Click);
			// 
			// OpenMpcFileDialog
			// 
			this.OpenMpcFileDialog.DefaultExt = "exe";
			this.OpenMpcFileDialog.Filter = "Media Player Classic Home Cinema|*.exe";
			// 
			// openMpcButton
			// 
			this.openMpcButton.Enabled = false;
			this.openMpcButton.Location = new System.Drawing.Point(12, 55);
			this.openMpcButton.Name = "openMpcButton";
			this.openMpcButton.Size = new System.Drawing.Size(142, 23);
			this.openMpcButton.TabIndex = 3;
			this.openMpcButton.Text = "Open Media Player Classic";
			this.openMpcButton.UseVisualStyleBackColor = true;
			this.openMpcButton.Click += new System.EventHandler(this.openMpcButton_Click);
			// 
			// statusStrip
			// 
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mpcStatusLabel});
			this.statusStrip.Location = new System.Drawing.Point(0, 239);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Size = new System.Drawing.Size(313, 22);
			this.statusStrip.TabIndex = 4;
			// 
			// mpcStatusLabel
			// 
			this.mpcStatusLabel.Name = "mpcStatusLabel";
			this.mpcStatusLabel.Size = new System.Drawing.Size(112, 17);
			this.mpcStatusLabel.Text = "Waiting for MPCHC";
			// 
			// MainWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(313, 261);
			this.Controls.Add(this.statusStrip);
			this.Controls.Add(this.openMpcButton);
			this.Controls.Add(this.selectMpcLocationButton);
			this.Controls.Add(this.mpcLocationTextBox);
			this.Controls.Add(this.selectMpcLocationLabel);
			this.Name = "MainWindow";
			this.Load += new System.EventHandler(this.MainWindow_Load);
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label selectMpcLocationLabel;
		private System.Windows.Forms.TextBox mpcLocationTextBox;
		private System.Windows.Forms.Button selectMpcLocationButton;
		private System.Windows.Forms.OpenFileDialog OpenMpcFileDialog;
		private System.Windows.Forms.Button openMpcButton;
		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.ToolStripStatusLabel mpcStatusLabel;

	}
}

