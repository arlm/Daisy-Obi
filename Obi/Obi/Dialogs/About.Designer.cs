namespace Obi.Dialogs
{
    partial class About
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.mWebBrowser = new System.Windows.Forms.WebBrowser();
            this.mbtnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mWebBrowser
            // 
            this.mWebBrowser.AllowWebBrowserDrop = false;
            resources.ApplyResources(this.mWebBrowser, "mWebBrowser");
            this.mWebBrowser.IsWebBrowserContextMenuEnabled = false;
            this.mWebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.mWebBrowser.Name = "mWebBrowser";
            this.mWebBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.mWebBrowser_Navigating);
            this.mWebBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.mWebBrowser_DocumentCompleted);
            // 
            // mbtnClose
            // 
            this.mbtnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.mbtnClose, "mbtnClose");
            this.mbtnClose.Name = "mbtnClose";
            this.mbtnClose.UseVisualStyleBackColor = true;
            this.mbtnClose.Click += new System.EventHandler(this.mbtnClose_Click);
            // 
            // About
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.mbtnClose;
            this.Controls.Add(this.mbtnClose);
            this.Controls.Add(this.mWebBrowser);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.About_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser mWebBrowser;
        private System.Windows.Forms.Button mbtnClose;

    }
}
