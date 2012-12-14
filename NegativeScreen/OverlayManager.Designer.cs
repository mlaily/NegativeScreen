namespace NegativeScreen
{
	partial class OverlayManager
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OverlayManager));
			this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.trayIconContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toggleInversionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.changeModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.editConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.trayIconContextMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// trayIcon
			// 
			this.trayIcon.ContextMenuStrip = this.trayIconContextMenuStrip;
			this.trayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("mainIcon")));
			this.trayIcon.Text = "NegativeScreen";
			this.trayIcon.Visible = true;
			this.trayIcon.MouseClick +=new System.Windows.Forms.MouseEventHandler(this.trayIcon_MouseClick);
			// 
			// trayIconContextMenuStrip
			// 
			this.trayIconContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toggleInversionToolStripMenuItem,
            this.changeModeToolStripMenuItem,
            this.toolStripSeparator1,
            this.editConfigurationToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
			this.trayIconContextMenuStrip.Name = "trayIconContextMenuStrip";
			this.trayIconContextMenuStrip.Size = new System.Drawing.Size(172, 126);
			// 
			// toggleInversionToolStripMenuItem
			// 
			this.toggleInversionToolStripMenuItem.Name = "toggleInversionToolStripMenuItem";
			this.toggleInversionToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.toggleInversionToolStripMenuItem.Text = "Toggle Inversion";
			this.toggleInversionToolStripMenuItem.Click += new System.EventHandler(this.toggleInversionToolStripMenuItem_Click);
			// 
			// changeModeToolStripMenuItem
			// 
			this.changeModeToolStripMenuItem.Name = "changeModeToolStripMenuItem";
			this.changeModeToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.changeModeToolStripMenuItem.Text = "Change Mode";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(168, 6);
			// 
			// editConfigurationToolStripMenuItem
			// 
			this.editConfigurationToolStripMenuItem.Name = "editConfigurationToolStripMenuItem";
			this.editConfigurationToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.editConfigurationToolStripMenuItem.Text = "Edit Configuration";
			this.editConfigurationToolStripMenuItem.Click += new System.EventHandler(this.editConfigurationToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(168, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.exitToolStripMenuItem.Text = "&Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// OverlayManager
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("mainIcon")));
			this.Name = "OverlayManager";
			this.Text = "NegativeScreen";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OverlayManager_FormClosed);
			this.trayIconContextMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.NotifyIcon trayIcon;
		private System.Windows.Forms.ContextMenuStrip trayIconContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toggleInversionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem changeModeToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem editConfigurationToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
	}
}