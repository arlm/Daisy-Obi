namespace Obi.Dialogs
{
    partial class EditSimpleMetadata
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
            this.mOKButton = new System.Windows.Forms.Button();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.mTitleBox = new System.Windows.Forms.TextBox();
            this.mAuthorBox = new System.Windows.Forms.TextBox();
            this.mPublisherBox = new System.Windows.Forms.TextBox();
            this.mIdentiferBox = new System.Windows.Forms.TextBox();
            this.mLanguageBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // mOKButton
            // 
            this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOKButton.Location = new System.Drawing.Point(174, 171);
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.Size = new System.Drawing.Size(75, 23);
            this.mOKButton.TabIndex = 10;
            this.mOKButton.Text = "&OK";
            this.mOKButton.UseVisualStyleBackColor = true;
            this.mOKButton.Click += new System.EventHandler(this.mOKButton_Click);
            // 
            // mCancelButton
            // 
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelButton.Location = new System.Drawing.Point(255, 171);
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.Size = new System.Drawing.Size(75, 23);
            this.mCancelButton.TabIndex = 11;
            this.mCancelButton.Text = "&Cancel";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(21, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Title:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "&Author:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "&Identifier:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 69);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "&Publisher:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 119);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "&Language:";
            // 
            // mTitleBox
            // 
            this.mTitleBox.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.mTitleBox.Location = new System.Drawing.Point(73, 12);
            this.mTitleBox.Name = "mTitleBox";
            this.mTitleBox.Size = new System.Drawing.Size(419, 23);
            this.mTitleBox.TabIndex = 1;
            // 
            // mAuthorBox
            // 
            this.mAuthorBox.Location = new System.Drawing.Point(73, 41);
            this.mAuthorBox.Name = "mAuthorBox";
            this.mAuthorBox.Size = new System.Drawing.Size(419, 19);
            this.mAuthorBox.TabIndex = 3;
            // 
            // mPublisherBox
            // 
            this.mPublisherBox.Location = new System.Drawing.Point(73, 66);
            this.mPublisherBox.Name = "mPublisherBox";
            this.mPublisherBox.Size = new System.Drawing.Size(419, 19);
            this.mPublisherBox.TabIndex = 5;
            // 
            // mIdentiferBox
            // 
            this.mIdentiferBox.Location = new System.Drawing.Point(73, 91);
            this.mIdentiferBox.Name = "mIdentiferBox";
            this.mIdentiferBox.Size = new System.Drawing.Size(419, 19);
            this.mIdentiferBox.TabIndex = 7;
            // 
            // mLanguageBox
            // 
            this.mLanguageBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mLanguageBox.FormattingEnabled = true;
            this.mLanguageBox.Location = new System.Drawing.Point(73, 116);
            this.mLanguageBox.Name = "mLanguageBox";
            this.mLanguageBox.Size = new System.Drawing.Size(419, 20);
            this.mLanguageBox.Sorted = true;
            this.mLanguageBox.TabIndex = 9;
            // 
            // EditSimpleMetadata
            // 
            this.AcceptButton = this.mOKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 206);
            this.Controls.Add(this.mLanguageBox);
            this.Controls.Add(this.mIdentiferBox);
            this.Controls.Add(this.mPublisherBox);
            this.Controls.Add(this.mAuthorBox);
            this.Controls.Add(this.mTitleBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mOKButton);
            this.Name = "EditSimpleMetadata";
            this.Text = "Edit project metadata";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button mOKButton;
        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox mTitleBox;
        private System.Windows.Forms.TextBox mAuthorBox;
        private System.Windows.Forms.TextBox mPublisherBox;
        private System.Windows.Forms.TextBox mIdentiferBox;
        private System.Windows.Forms.ComboBox mLanguageBox;
    }
}