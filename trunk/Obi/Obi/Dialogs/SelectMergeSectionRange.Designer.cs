namespace Obi.Dialogs
{
    partial class SelectMergeSectionRange
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectMergeSectionRange));
            this.m_btn_OK = new System.Windows.Forms.Button();
            this.m_btn_Cancel = new System.Windows.Forms.Button();
            this.m_statusStripForMergeSection = new System.Windows.Forms.StatusStrip();
            this.m_StatusLabelForMergeSection = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_tb_SelectedSection = new System.Windows.Forms.TextBox();
            this.m_btn_SelectAll = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.m_dataGridView_SectionNames = new System.Windows.Forms.DataGridView();
            this.SectionNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SectionLevelColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.m_grp_SectionAudioOperation = new System.Windows.Forms.GroupBox();
            this.m_btnPause = new System.Windows.Forms.Button();
            this.m_btn_Stop = new System.Windows.Forms.Button();
            this.m_btn_Play = new System.Windows.Forms.Button();
            this.m_lb_listofSectionsToMerge = new System.Windows.Forms.ListBox();
            this.m_rbMerge = new System.Windows.Forms.RadioButton();
            this.m_rbChangeLevel = new System.Windows.Forms.RadioButton();
            this.m_btn_IncreaseSectionLevel = new System.Windows.Forms.Button();
            this.m_btn_DecreaseSectionLevel = new System.Windows.Forms.Button();
            this.m_grp_SectionLevelOperation = new System.Windows.Forms.GroupBox();
            this.m_statusStripForMergeSection.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_dataGridView_SectionNames)).BeginInit();
            this.m_grp_SectionAudioOperation.SuspendLayout();
            this.m_grp_SectionLevelOperation.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_btn_OK
            // 
            resources.ApplyResources(this.m_btn_OK, "m_btn_OK");
            this.m_btn_OK.Name = "m_btn_OK";
            this.m_btn_OK.UseVisualStyleBackColor = true;
            this.m_btn_OK.Click += new System.EventHandler(this.m_btn_OK_Click);
            // 
            // m_btn_Cancel
            // 
            resources.ApplyResources(this.m_btn_Cancel, "m_btn_Cancel");
            this.m_btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btn_Cancel.Name = "m_btn_Cancel";
            this.m_btn_Cancel.UseVisualStyleBackColor = true;
            this.m_btn_Cancel.Click += new System.EventHandler(this.m_btn_Cancel_Click);
            // 
            // m_statusStripForMergeSection
            // 
            this.m_statusStripForMergeSection.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_StatusLabelForMergeSection});
            this.m_statusStripForMergeSection.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            resources.ApplyResources(this.m_statusStripForMergeSection, "m_statusStripForMergeSection");
            this.m_statusStripForMergeSection.Name = "m_statusStripForMergeSection";
            this.m_statusStripForMergeSection.ShowItemToolTips = true;
            // 
            // m_StatusLabelForMergeSection
            // 
            this.m_StatusLabelForMergeSection.AutoToolTip = true;
            this.m_StatusLabelForMergeSection.Name = "m_StatusLabelForMergeSection";
            resources.ApplyResources(this.m_StatusLabelForMergeSection, "m_StatusLabelForMergeSection");
            // 
            // m_tb_SelectedSection
            // 
            resources.ApplyResources(this.m_tb_SelectedSection, "m_tb_SelectedSection");
            this.m_tb_SelectedSection.Name = "m_tb_SelectedSection";
            this.m_tb_SelectedSection.ReadOnly = true;
            // 
            // m_btn_SelectAll
            // 
            resources.ApplyResources(this.m_btn_SelectAll, "m_btn_SelectAll");
            this.m_btn_SelectAll.Name = "m_btn_SelectAll";
            this.m_btn_SelectAll.UseVisualStyleBackColor = true;
            this.m_btn_SelectAll.Click += new System.EventHandler(this.m_btn_SelectAll_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.m_tb_SelectedSection);
            this.groupBox2.Controls.Add(this.m_btn_SelectAll);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // helpProvider1
            // 
            resources.ApplyResources(this.helpProvider1, "helpProvider1");
            // 
            // m_dataGridView_SectionNames
            // 
            this.m_dataGridView_SectionNames.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_dataGridView_SectionNames.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SectionNameColumn,
            this.SectionLevelColumn});
            resources.ApplyResources(this.m_dataGridView_SectionNames, "m_dataGridView_SectionNames");
            this.m_dataGridView_SectionNames.Name = "m_dataGridView_SectionNames";
            // 
            // SectionNameColumn
            // 
            resources.ApplyResources(this.SectionNameColumn, "SectionNameColumn");
            this.SectionNameColumn.Name = "SectionNameColumn";
            this.SectionNameColumn.ReadOnly = true;
            // 
            // SectionLevelColumn
            // 
            resources.ApplyResources(this.SectionLevelColumn, "SectionLevelColumn");
            this.SectionLevelColumn.Name = "SectionLevelColumn";
            this.SectionLevelColumn.ReadOnly = true;
            // 
            // m_grp_SectionAudioOperation
            // 
            this.m_grp_SectionAudioOperation.Controls.Add(this.m_btnPause);
            this.m_grp_SectionAudioOperation.Controls.Add(this.m_btn_Stop);
            this.m_grp_SectionAudioOperation.Controls.Add(this.m_btn_Play);
            resources.ApplyResources(this.m_grp_SectionAudioOperation, "m_grp_SectionAudioOperation");
            this.m_grp_SectionAudioOperation.Name = "m_grp_SectionAudioOperation";
            this.m_grp_SectionAudioOperation.TabStop = false;
            // 
            // m_btnPause
            // 
            resources.ApplyResources(this.m_btnPause, "m_btnPause");
            this.m_btnPause.Name = "m_btnPause";
            this.m_btnPause.UseVisualStyleBackColor = true;
            this.m_btnPause.Click += new System.EventHandler(this.m_btnPause_Click);
            // 
            // m_btn_Stop
            // 
            resources.ApplyResources(this.m_btn_Stop, "m_btn_Stop");
            this.m_btn_Stop.Name = "m_btn_Stop";
            this.m_btn_Stop.UseVisualStyleBackColor = true;
            this.m_btn_Stop.Click += new System.EventHandler(this.m_btn_Stop_Click);
            // 
            // m_btn_Play
            // 
            resources.ApplyResources(this.m_btn_Play, "m_btn_Play");
            this.m_btn_Play.Name = "m_btn_Play";
            this.m_btn_Play.UseVisualStyleBackColor = true;
            this.m_btn_Play.Click += new System.EventHandler(this.m_btn_Play_Click);
            // 
            // m_lb_listofSectionsToMerge
            // 
            resources.ApplyResources(this.m_lb_listofSectionsToMerge, "m_lb_listofSectionsToMerge");
            this.m_lb_listofSectionsToMerge.FormattingEnabled = true;
            this.m_lb_listofSectionsToMerge.Name = "m_lb_listofSectionsToMerge";
            this.m_lb_listofSectionsToMerge.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.m_lb_listofSectionsToMerge.SelectedIndexChanged += new System.EventHandler(this.m_lb_listofSectionsToMerge_SelectedIndexChanged);
            // 
            // m_rbMerge
            // 
            resources.ApplyResources(this.m_rbMerge, "m_rbMerge");
            this.m_rbMerge.Checked = true;
            this.m_rbMerge.Name = "m_rbMerge";
            this.m_rbMerge.TabStop = true;
            this.m_rbMerge.UseVisualStyleBackColor = true;
            this.m_rbMerge.CheckedChanged += new System.EventHandler(this.m_rbMerge_CheckedChanged);
            // 
            // m_rbChangeLevel
            // 
            resources.ApplyResources(this.m_rbChangeLevel, "m_rbChangeLevel");
            this.m_rbChangeLevel.Name = "m_rbChangeLevel";
            this.m_rbChangeLevel.UseVisualStyleBackColor = true;
            this.m_rbChangeLevel.CheckedChanged += new System.EventHandler(this.m_rbChangeLevel_CheckedChanged);
            // 
            // m_btn_IncreaseSectionLevel
            // 
            resources.ApplyResources(this.m_btn_IncreaseSectionLevel, "m_btn_IncreaseSectionLevel");
            this.m_btn_IncreaseSectionLevel.Name = "m_btn_IncreaseSectionLevel";
            this.helpProvider1.SetShowHelp(this.m_btn_IncreaseSectionLevel, ((bool)(resources.GetObject("m_btn_IncreaseSectionLevel.ShowHelp"))));
            this.m_btn_IncreaseSectionLevel.UseVisualStyleBackColor = true;
            this.m_btn_IncreaseSectionLevel.Click += new System.EventHandler(this.m_btn_IncreaseSectionLevel_Click);
            // 
            // m_btn_DecreaseSectionLevel
            // 
            resources.ApplyResources(this.m_btn_DecreaseSectionLevel, "m_btn_DecreaseSectionLevel");
            this.m_btn_DecreaseSectionLevel.Name = "m_btn_DecreaseSectionLevel";
            this.helpProvider1.SetShowHelp(this.m_btn_DecreaseSectionLevel, ((bool)(resources.GetObject("m_btn_DecreaseSectionLevel.ShowHelp"))));
            this.m_btn_DecreaseSectionLevel.UseVisualStyleBackColor = true;
            this.m_btn_DecreaseSectionLevel.Click += new System.EventHandler(this.m_btn_DecreaseSectionLevel_Click);
            // 
            // m_grp_SectionLevelOperation
            // 
            this.m_grp_SectionLevelOperation.Controls.Add(this.m_btn_DecreaseSectionLevel);
            this.m_grp_SectionLevelOperation.Controls.Add(this.m_btn_IncreaseSectionLevel);
            resources.ApplyResources(this.m_grp_SectionLevelOperation, "m_grp_SectionLevelOperation");
            this.m_grp_SectionLevelOperation.Name = "m_grp_SectionLevelOperation";
            this.helpProvider1.SetShowHelp(this.m_grp_SectionLevelOperation, ((bool)(resources.GetObject("m_grp_SectionLevelOperation.ShowHelp"))));
            this.m_grp_SectionLevelOperation.TabStop = false;
            // 
            // SelectMergeSectionRange
            // 
            this.AcceptButton = this.m_btn_OK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btn_Cancel;
            this.Controls.Add(this.m_rbChangeLevel);
            this.Controls.Add(this.m_rbMerge);
            this.Controls.Add(this.m_lb_listofSectionsToMerge);
            this.Controls.Add(this.m_grp_SectionAudioOperation);
            this.Controls.Add(this.m_grp_SectionLevelOperation);
            this.Controls.Add(this.m_dataGridView_SectionNames);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.m_statusStripForMergeSection);
            this.Controls.Add(this.m_btn_Cancel);
            this.Controls.Add(this.m_btn_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectMergeSectionRange";
            this.m_statusStripForMergeSection.ResumeLayout(false);
            this.m_statusStripForMergeSection.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_dataGridView_SectionNames)).EndInit();
            this.m_grp_SectionAudioOperation.ResumeLayout(false);
            this.m_grp_SectionLevelOperation.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button m_btn_OK;
        private System.Windows.Forms.Button m_btn_Cancel;
        private System.Windows.Forms.StatusStrip m_statusStripForMergeSection;
        private System.Windows.Forms.ToolStripStatusLabel m_StatusLabelForMergeSection;
        private System.Windows.Forms.TextBox m_tb_SelectedSection;
        private System.Windows.Forms.Button m_btn_SelectAll;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.DataGridView m_dataGridView_SectionNames;
        private System.Windows.Forms.GroupBox m_grp_SectionAudioOperation;
        private System.Windows.Forms.Button m_btn_Stop;
        private System.Windows.Forms.Button m_btn_Play;
        private System.Windows.Forms.DataGridViewTextBoxColumn SectionNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn SectionLevelColumn;
        private System.Windows.Forms.ListBox m_lb_listofSectionsToMerge;
        private System.Windows.Forms.Button m_btnPause;
        private System.Windows.Forms.RadioButton m_rbMerge;
        private System.Windows.Forms.RadioButton m_rbChangeLevel;
        private System.Windows.Forms.Button m_btn_IncreaseSectionLevel;
        private System.Windows.Forms.Button m_btn_DecreaseSectionLevel;
        private System.Windows.Forms.GroupBox m_grp_SectionLevelOperation;
    }
}