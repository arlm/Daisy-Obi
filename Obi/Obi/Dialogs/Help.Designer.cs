namespace Obi.Dialogs
{
    partial class Help
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Help));
            this.mWebBrowser = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // mWebBrowser
            // 
            this.mWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mWebBrowser.Location = new System.Drawing.Point(0, 0);
            this.mWebBrowser.MinimumSize = new System.Drawing.Size(20, 22);
            this.mWebBrowser.Name = "mWebBrowser";
            this.mWebBrowser.Size = new System.Drawing.Size(860, 613);
            this.mWebBrowser.TabIndex = 0;
            this.mWebBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.mWebBrowser_Navigating);
            this.mWebBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.mWebBrowser_DocumentCompleted);
            // 
            // Help
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 613);
            this.Controls.Add(this.mWebBrowser);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Help";
            this.Text = "Help";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser mWebBrowser;
    }
}