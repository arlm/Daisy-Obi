namespace Obi.UserControls
{
    partial class StructureBlock
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
            this.mLabelBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // mLabelBox
            // 
            this.mLabelBox.BackColor = System.Drawing.Color.PowderBlue;
            this.mLabelBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mLabelBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.mLabelBox.Location = new System.Drawing.Point(3, 3);
            this.mLabelBox.Name = "mLabelBox";
            this.mLabelBox.ReadOnly = true;
            this.mLabelBox.Size = new System.Drawing.Size(144, 12);
            this.mLabelBox.TabIndex = 0;
            this.mLabelBox.TabStop = false;
            this.mLabelBox.Click += new System.EventHandler(this.mLabelBox_Click);
            this.mLabelBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mLabelBox_KeyDown);
            // 
            // StructureBlock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.PowderBlue;
            this.Controls.Add(this.mLabelBox);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.Name = "StructureBlock";
            this.Size = new System.Drawing.Size(150, 18);
            this.Click += new System.EventHandler(this.StructureBlock_Click);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox mLabelBox;

    }
}
