namespace Obi.UserControls
{
    partial class StripManagerPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.mFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addStripToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameStripToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.importAssetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playAssetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameAssetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitAudioBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Strip manager";
            // 
            // mFlowLayoutPanel
            // 
            this.mFlowLayoutPanel.AutoScroll = true;
            this.mFlowLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.mFlowLayoutPanel.ContextMenuStrip = this.contextMenuStrip1;
            this.mFlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.mFlowLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.mFlowLayoutPanel.Name = "mFlowLayoutPanel";
            this.mFlowLayoutPanel.Size = new System.Drawing.Size(150, 150);
            this.mFlowLayoutPanel.TabIndex = 1;
            this.mFlowLayoutPanel.WrapContents = false;
            this.mFlowLayoutPanel.Click += new System.EventHandler(this.mFlowLayoutPanel_Click);
            this.mFlowLayoutPanel.Leave += new System.EventHandler(this.mFlowLayoutPanel_Leave);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addStripToolStripMenuItem,
            this.renameStripToolStripMenuItem,
            this.toolStripSeparator1,
            this.importAssetToolStripMenuItem,
            this.playAssetToolStripMenuItem,
            this.splitAudioBlockToolStripMenuItem,
            this.renameAssetToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowImageMargin = false;
            this.contextMenuStrip1.Size = new System.Drawing.Size(149, 164);
            // 
            // addStripToolStripMenuItem
            // 
            this.addStripToolStripMenuItem.Name = "addStripToolStripMenuItem";
            this.addStripToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.addStripToolStripMenuItem.Text = "&Add strip";
            this.addStripToolStripMenuItem.Click += new System.EventHandler(this.addStripToolStripMenuItem_Click);
            // 
            // renameStripToolStripMenuItem
            // 
            this.renameStripToolStripMenuItem.Name = "renameStripToolStripMenuItem";
            this.renameStripToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.renameStripToolStripMenuItem.Text = "&Rename strip";
            this.renameStripToolStripMenuItem.Click += new System.EventHandler(this.renameStripToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(145, 6);
            // 
            // importAssetToolStripMenuItem
            // 
            this.importAssetToolStripMenuItem.Name = "importAssetToolStripMenuItem";
            this.importAssetToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.importAssetToolStripMenuItem.Text = "&Import audio file";
            this.importAssetToolStripMenuItem.Click += new System.EventHandler(this.importAssetToolStripMenuItem_Click);
            // 
            // playAssetToolStripMenuItem
            // 
            this.playAssetToolStripMenuItem.Name = "playAssetToolStripMenuItem";
            this.playAssetToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.playAssetToolStripMenuItem.Text = "&Play audio block";
            this.playAssetToolStripMenuItem.Click += new System.EventHandler(this.playAssetToolStripMenuItem_Click);
            // 
            // renameAssetToolStripMenuItem
            // 
            this.renameAssetToolStripMenuItem.Name = "renameAssetToolStripMenuItem";
            this.renameAssetToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.renameAssetToolStripMenuItem.Text = "Re&name audio block";
            this.renameAssetToolStripMenuItem.Click += new System.EventHandler(this.renameAssetToolStripMenuItem_Click);
            // 
            // splitAudioBlockToolStripMenuItem
            // 
            this.splitAudioBlockToolStripMenuItem.Name = "splitAudioBlockToolStripMenuItem";
            this.splitAudioBlockToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.splitAudioBlockToolStripMenuItem.Text = "&Split audio block";
            this.splitAudioBlockToolStripMenuItem.Click += new System.EventHandler(this.splitAudioBlockToolStripMenuItem_Click);
            // 
            // StripManagerPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.mFlowLayoutPanel);
            this.Controls.Add(this.label1);
            this.Name = "StripManagerPanel";
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel mFlowLayoutPanel;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem addStripToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameStripToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem importAssetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playAssetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameAssetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem splitAudioBlockToolStripMenuItem;
    }
}
