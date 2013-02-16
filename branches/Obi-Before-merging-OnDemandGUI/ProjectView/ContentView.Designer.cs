namespace Obi.ProjectView
{
    partial class ContentView
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
            this.mContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Context_AddSectionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_InsertSectionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_SplitSectionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_MergeSectionWithNextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.Context_AddBlankPhraseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_AddEmptyPagesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_ImportAudioFilesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_SplitPhraseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_MergePhraseWithNextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_CropAudioMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_PhraseIsTODOMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_PhraseIsUsedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.Context_AssignRoleMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_AssignRole_PlainMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_AssignRole_HeadingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_AssignRole_PageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_AssignRole_SilenceMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.Context_AssignRole_NewCustomRoleMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_ClearRoleMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_ApplyPhraseDetectionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_AudioSelectionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_AudioSelection_BeginMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_AudioSelection_EndMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.Context_CutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_CopyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_PasteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_PasteBeforeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_PasteInsideMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Context_DeleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.Context_PropertiesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mStripsPanel = new System.Windows.Forms.Panel();
            this.mHScrollBar = new System.Windows.Forms.HScrollBar();
            this.mVScrollBar = new System.Windows.Forms.VScrollBar();
            this.mCornerPanel = new System.Windows.Forms.Panel();
            this.mContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mContextMenuStrip
            // 
            this.mContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Context_AddSectionMenuItem,
            this.Context_InsertSectionMenuItem,
            this.Context_SplitSectionMenuItem,
            this.Context_MergeSectionWithNextMenuItem,
            this.toolStripSeparator1,
            this.Context_AddBlankPhraseMenuItem,
            this.Context_AddEmptyPagesMenuItem,
            this.Context_ImportAudioFilesMenuItem,
            this.Context_SplitPhraseMenuItem,
            this.Context_MergePhraseWithNextMenuItem,
            this.Context_CropAudioMenuItem,
            this.Context_PhraseIsTODOMenuItem,
            this.Context_PhraseIsUsedMenuItem,
            this.toolStripSeparator2,
            this.Context_AssignRoleMenuItem,
            this.Context_ClearRoleMenuItem,
            this.Context_ApplyPhraseDetectionMenuItem,
            this.Context_AudioSelectionMenuItem,
            this.toolStripSeparator4,
            this.Context_CutMenuItem,
            this.Context_CopyMenuItem,
            this.Context_PasteMenuItem,
            this.Context_PasteBeforeMenuItem,
            this.Context_PasteInsideMenuItem,
            this.Context_DeleteMenuItem,
            this.toolStripSeparator5,
            this.Context_PropertiesMenuItem});
            this.mContextMenuStrip.Name = "mContextMenuStrip";
            this.mContextMenuStrip.Size = new System.Drawing.Size(190, 534);
            // 
            // Context_AddSectionMenuItem
            // 
            this.Context_AddSectionMenuItem.Name = "Context_AddSectionMenuItem";
            this.Context_AddSectionMenuItem.Size = new System.Drawing.Size(189, 22);
            this.Context_AddSectionMenuItem.Text = "Add &section";
            this.Context_AddSectionMenuItem.Click += new System.EventHandler(this.Context_AddSectionMenuItem_Click);
            // 
            // Context_InsertSectionMenuItem
            // 
            this.Context_InsertSectionMenuItem.Name = "Context_InsertSectionMenuItem";
            this.Context_InsertSectionMenuItem.Size = new System.Drawing.Size(189, 22);
            this.Context_InsertSectionMenuItem.Text = "&Insert section";
            this.Context_InsertSectionMenuItem.Click += new System.EventHandler(this.Context_InsertSectionMenuItem_Click);
            // 
            // Context_SplitSectionMenuItem
            // 
            this.Context_SplitSectionMenuItem.Name = "Context_SplitSectionMenuItem";
            this.Context_SplitSectionMenuItem.Size = new System.Drawing.Size(189, 22);
            this.Context_SplitSectionMenuItem.Text = "Sp&lit section";
            this.Context_SplitSectionMenuItem.Click += new System.EventHandler(this.Context_SplitSectionMenuItem_Click);
            // 
            // Context_MergeSectionWithNextMenuItem
            // 
            this.Context_MergeSectionWithNextMenuItem.Name = "Context_MergeSectionWithNextMenuItem";
            this.Context_MergeSectionWithNextMenuItem.Size = new System.Drawing.Size(189, 22);
            this.Context_MergeSectionWithNextMenuItem.Text = "Mer&ge section with next";
            this.Context_MergeSectionWithNextMenuItem.Click += new System.EventHandler(this.Context_MergeSectionWithNextMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(186, 6);
            // 
            // Context_AddBlankPhraseMenuItem
            // 
            this.Context_AddBlankPhraseMenuItem.Name = "Context_AddBlankPhraseMenuItem";
            this.Context_AddBlankPhraseMenuItem.Size = new System.Drawing.Size(189, 22);
            this.Context_AddBlankPhraseMenuItem.Text = "&Add blank phrase";
            this.Context_AddBlankPhraseMenuItem.Click += new System.EventHandler(this.Context_AddBlankPhraseMenuItem_Click);
            // 
            // Context_AddEmptyPagesMenuItem
            // 
            this.Context_AddEmptyPagesMenuItem.Name = "Context_AddEmptyPagesMenuItem";
            this.Context_AddEmptyPagesMenuItem.Size = new System.Drawing.Size(189, 22);
            this.Context_AddEmptyPagesMenuItem.Text = "Add &empty pages";
            this.Context_AddEmptyPagesMenuItem.Click += new System.EventHandler(this.Context_AddEmptyPagesMenuItem_Click);
            // 
            // Context_ImportAudioFilesMenuItem
            // 
            this.Context_ImportAudioFilesMenuItem.Name = "Context_ImportAudioFilesMenuItem";
            this.Context_ImportAudioFilesMenuItem.Size = new System.Drawing.Size(189, 22);
            this.Context_ImportAudioFilesMenuItem.Text = "I&mport audio files";
            this.Context_ImportAudioFilesMenuItem.Click += new System.EventHandler(this.Context_ImportAudioFilesMenuItem_Click);
            // 
            // Context_SplitPhraseMenuItem
            // 
            this.Context_SplitPhraseMenuItem.Name = "Context_SplitPhraseMenuItem";
            this.Context_SplitPhraseMenuItem.Size = new System.Drawing.Size(189, 22);
            this.Context_SplitPhraseMenuItem.Text = "Split p&hrase";
            this.Context_SplitPhraseMenuItem.Click += new System.EventHandler(this.Context_SplitPhraseMenuItem_Click);
            // 
            // Context_MergePhraseWithNextMenuItem
            // 
            this.Context_MergePhraseWithNextMenuItem.Name = "Context_MergePhraseWithNextMenuItem";
            this.Context_MergePhraseWithNextMenuItem.Size = new System.Drawing.Size(189, 22);
            this.Context_MergePhraseWithNextMenuItem.Text = "Merge phrase with ne&xt";
            this.Context_MergePhraseWithNextMenuItem.Click += new System.EventHandler(this.Context_MergePhraseWithNextMenuItem_Click);
            // 
            // Context_CropAudioMenuItem
            // 
            this.Context_CropAudioMenuItem.Name = "Context_CropAudioMenuItem";
            this.Context_CropAudioMenuItem.Size = new System.Drawing.Size(189, 22);
            this.Context_CropAudioMenuItem.Text = "Crop au&dio";
            this.Context_CropAudioMenuItem.Click += new System.EventHandler(this.Context_CropAudioMenuItem_Click);
            // 
            // Context_PhraseIsTODOMenuItem
            // 
            this.Context_PhraseIsTODOMenuItem.Checked = true;
            this.Context_PhraseIsTODOMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Context_PhraseIsTODOMenuItem.Name = "Context_PhraseIsTODOMenuItem";
            this.Context_PhraseIsTODOMenuItem.Size = new System.Drawing.Size(189, 22);
            this.Context_PhraseIsTODOMenuItem.Text = "Phrase is T&ODO";
            this.Context_PhraseIsTODOMenuItem.Click += new System.EventHandler(this.Context_PhraseIsTODOMenuItem_Click);
            // 
            // Context_PhraseIsUsedMenuItem
            // 
            this.Context_PhraseIsUsedMenuItem.Checked = true;
            this.Context_PhraseIsUsedMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Context_PhraseIsUsedMenuItem.Name = "Context_PhraseIsUsedMenuItem";
            this.Context_PhraseIsUsedMenuItem.Size = new System.Drawing.Size(189, 22);
            this.Context_PhraseIsUsedMenuItem.Text = "Phras&e is used";
            this.Context_PhraseIsUsedMenuItem.Click += new System.EventHandler(this.Context_PhraseIsUsedMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(186, 6);
            // 
            // Context_AssignRoleMenuItem
            // 
            this.Context_AssignRoleMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Context_AssignRole_PlainMenuItem,
            this.Context_AssignRole_HeadingMenuItem,
            this.Context_AssignRole_PageMenuItem,
            this.Context_AssignRole_SilenceMenuItem,
            this.toolStripSeparator3,
            this.Context_AssignRole_NewCustomRoleMenuItem});
            this.Context_AssignRoleMenuItem.Name = "Context_AssignRoleMenuItem";
            this.Context_AssignRoleMenuItem.Size = new System.Drawing.Size(189, 22);
            this.Context_AssignRoleMenuItem.Text = "Assign ro&le";
            // 
            // Context_AssignRole_PlainMenuItem
            // 
            this.Context_AssignRole_PlainMenuItem.Name = "Context_AssignRole_PlainMenuItem";
            this.Context_AssignRole_PlainMenuItem.Size = new System.Drawing.Size(173, 22);
            this.Context_AssignRole_PlainMenuItem.Text = "P&lain";
            this.Context_AssignRole_PlainMenuItem.Click += new System.EventHandler(this.Context_AssignRole_PlainMenuItem_Click);
            // 
            // Context_AssignRole_HeadingMenuItem
            // 
            this.Context_AssignRole_HeadingMenuItem.Name = "Context_AssignRole_HeadingMenuItem";
            this.Context_AssignRole_HeadingMenuItem.Size = new System.Drawing.Size(173, 22);
            this.Context_AssignRole_HeadingMenuItem.Text = "&Heading";
            this.Context_AssignRole_HeadingMenuItem.Click += new System.EventHandler(this.Context_AssignRole_HeadingMenuItem_Click);
            // 
            // Context_AssignRole_PageMenuItem
            // 
            this.Context_AssignRole_PageMenuItem.Name = "Context_AssignRole_PageMenuItem";
            this.Context_AssignRole_PageMenuItem.Size = new System.Drawing.Size(173, 22);
            this.Context_AssignRole_PageMenuItem.Text = "&Page";
            this.Context_AssignRole_PageMenuItem.Click += new System.EventHandler(this.Context_AssignRole_PageMenuItem_Click);
            // 
            // Context_AssignRole_SilenceMenuItem
            // 
            this.Context_AssignRole_SilenceMenuItem.Name = "Context_AssignRole_SilenceMenuItem";
            this.Context_AssignRole_SilenceMenuItem.Size = new System.Drawing.Size(173, 22);
            this.Context_AssignRole_SilenceMenuItem.Text = "&Silence";
            this.Context_AssignRole_SilenceMenuItem.Click += new System.EventHandler(this.Context_AssignRole_SilenceMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(170, 6);
            // 
            // Context_AssignRole_NewCustomRoleMenuItem
            // 
            this.Context_AssignRole_NewCustomRoleMenuItem.Name = "Context_AssignRole_NewCustomRoleMenuItem";
            this.Context_AssignRole_NewCustomRoleMenuItem.Size = new System.Drawing.Size(173, 22);
            this.Context_AssignRole_NewCustomRoleMenuItem.Text = "(New custom role...)";
            this.Context_AssignRole_NewCustomRoleMenuItem.Click += new System.EventHandler(this.Context_AssignRole_NewCustomRoleMenuItem_Click);
            // 
            // Context_ClearRoleMenuItem
            // 
            this.Context_ClearRoleMenuItem.Name = "Context_ClearRoleMenuItem";
            this.Context_ClearRoleMenuItem.Size = new System.Drawing.Size(189, 22);
            this.Context_ClearRoleMenuItem.Text = "Clear role";
            this.Context_ClearRoleMenuItem.Click += new System.EventHandler(this.Context_ClearRoleMenuItem_Click);
            // 
            // Context_ApplyPhraseDetectionMenuItem
            // 
            this.Context_ApplyPhraseDetectionMenuItem.Name = "Context_ApplyPhraseDetectionMenuItem";
            this.Context_ApplyPhraseDetectionMenuItem.Size = new System.Drawing.Size(189, 22);
            this.Context_ApplyPhraseDetectionMenuItem.Text = "Apply p&hrase detection";
            this.Context_ApplyPhraseDetectionMenuItem.Click += new System.EventHandler(this.Context_ApplyPhraseDetectionMenuItem_Click);
            // 
            // Context_AudioSelectionMenuItem
            // 
            this.Context_AudioSelectionMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Context_AudioSelection_BeginMenuItem,
            this.Context_AudioSelection_EndMenuItem});
            this.Context_AudioSelectionMenuItem.Name = "Context_AudioSelectionMenuItem";
            this.Context_AudioSelectionMenuItem.Size = new System.Drawing.Size(189, 22);
            this.Context_AudioSelectionMenuItem.Text = "Audi&o selection";
            // 
            // Context_AudioSelection_BeginMenuItem
            // 
            this.Context_AudioSelection_BeginMenuItem.Name = "Context_AudioSelection_BeginMenuItem";
            this.Context_AudioSelection_BeginMenuItem.Size = new System.Drawing.Size(174, 22);
            this.Context_AudioSelection_BeginMenuItem.Text = "&Begin audio selection";
            this.Context_AudioSelection_BeginMenuItem.Click += new System.EventHandler(this.Context_AudioSelection_BeginMenuItem_Click);
            // 
            // Context_AudioSelection_EndMenuItem
            // 
            this.Context_AudioSelection_EndMenuItem.Name = "Context_AudioSelection_EndMenuItem";
            this.Context_AudioSelection_EndMenuItem.Size = new System.Drawing.Size(174, 22);
            this.Context_AudioSelection_EndMenuItem.Text = "&End audio selection";
            this.Context_AudioSelection_EndMenuItem.Click += new System.EventHandler(this.Context_AudioSelection_EndMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(186, 6);
            // 
            // Context_CutMenuItem
            // 
            this.Context_CutMenuItem.Name = "Context_CutMenuItem";
            this.Context_CutMenuItem.Size = new System.Drawing.Size(189, 22);
            this.Context_CutMenuItem.Text = "&Cut";
            this.Context_CutMenuItem.Click += new System.EventHandler(this.Context_CutMenuItem_Click);
            // 
            // Context_CopyMenuItem
            // 
            this.Context_CopyMenuItem.Name = "Context_CopyMenuItem";
            this.Context_CopyMenuItem.Size = new System.Drawing.Size(189, 22);
            this.Context_CopyMenuItem.Text = "Cop&y";
            this.Context_CopyMenuItem.Click += new System.EventHandler(this.Context_CopyMenuItem_Click);
            // 
            // Context_PasteMenuItem
            // 
            this.Context_PasteMenuItem.Name = "Context_PasteMenuItem";
            this.Context_PasteMenuItem.Size = new System.Drawing.Size(189, 22);
            this.Context_PasteMenuItem.Text = "&Paste";
            this.Context_PasteMenuItem.Click += new System.EventHandler(this.Context_PasteMenuItem_Click);
            // 
            // Context_PasteBeforeMenuItem
            // 
            this.Context_PasteBeforeMenuItem.Name = "Context_PasteBeforeMenuItem";
            this.Context_PasteBeforeMenuItem.Size = new System.Drawing.Size(189, 22);
            this.Context_PasteBeforeMenuItem.Text = "Paste &before";
            this.Context_PasteBeforeMenuItem.Click += new System.EventHandler(this.Context_PasteBeforeMenuItem_Click);
            // 
            // Context_PasteInsideMenuItem
            // 
            this.Context_PasteInsideMenuItem.Name = "Context_PasteInsideMenuItem";
            this.Context_PasteInsideMenuItem.Size = new System.Drawing.Size(189, 22);
            this.Context_PasteInsideMenuItem.Text = "Paste &inside";
            this.Context_PasteInsideMenuItem.Click += new System.EventHandler(this.Context_PasteInsideMenuItem_Click);
            // 
            // Context_DeleteMenuItem
            // 
            this.Context_DeleteMenuItem.Name = "Context_DeleteMenuItem";
            this.Context_DeleteMenuItem.Size = new System.Drawing.Size(189, 22);
            this.Context_DeleteMenuItem.Text = "&Delete";
            this.Context_DeleteMenuItem.Click += new System.EventHandler(this.Context_DeleteMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(186, 6);
            // 
            // Context_PropertiesMenuItem
            // 
            this.Context_PropertiesMenuItem.Name = "Context_PropertiesMenuItem";
            this.Context_PropertiesMenuItem.Size = new System.Drawing.Size(189, 22);
            this.Context_PropertiesMenuItem.Text = "Pr&operties";
            this.Context_PropertiesMenuItem.Click += new System.EventHandler(this.Context_PropertiesMenuItem_Click);
            // 
            // mStripsPanel
            // 
            this.mStripsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mStripsPanel.Location = new System.Drawing.Point(0, 0);
            this.mStripsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.mStripsPanel.Name = "mStripsPanel";
            this.mStripsPanel.Size = new System.Drawing.Size(522, 557);
            this.mStripsPanel.TabIndex = 1;
            // 
            // mHScrollBar
            // 
            this.mHScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mHScrollBar.LargeChange = 64;
            this.mHScrollBar.Location = new System.Drawing.Point(0, 557);
            this.mHScrollBar.Name = "mHScrollBar";
            this.mHScrollBar.Size = new System.Drawing.Size(522, 16);
            this.mHScrollBar.TabIndex = 2;
            this.mHScrollBar.ValueChanged += new System.EventHandler(this.mHScrollBar_ValueChanged);
            // 
            // mVScrollBar
            // 
            this.mVScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mVScrollBar.LargeChange = 64;
            this.mVScrollBar.Location = new System.Drawing.Point(522, 0);
            this.mVScrollBar.Name = "mVScrollBar";
            this.mVScrollBar.Size = new System.Drawing.Size(16, 557);
            this.mVScrollBar.TabIndex = 3;
            this.mVScrollBar.ValueChanged += new System.EventHandler(this.mVScrollBar_ValueChanged);
            // 
            // mCornerPanel
            // 
            this.mCornerPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mCornerPanel.Location = new System.Drawing.Point(522, 557);
            this.mCornerPanel.Name = "mCornerPanel";
            this.mCornerPanel.Size = new System.Drawing.Size(16, 16);
            this.mCornerPanel.TabIndex = 4;
            // 
            // ContentView
            // 
            this.ContextMenuStrip = this.mContextMenuStrip;
            this.Controls.Add(this.mVScrollBar);
            this.Controls.Add(this.mHScrollBar);
            this.Controls.Add(this.mCornerPanel);
            this.Controls.Add(this.mStripsPanel);
            this.Name = "ContentView";
            this.Size = new System.Drawing.Size(538, 573);
            this.Click += new System.EventHandler(this.ContentView_Click);
            this.Enter += new System.EventHandler(this.StripsView_Enter);
            this.mContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip mContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem Context_AddBlankPhraseMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_AddEmptyPagesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_AddSectionMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem Context_InsertSectionMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_SplitSectionMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_MergeSectionWithNextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_ImportAudioFilesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_SplitPhraseMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_MergePhraseWithNextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_CropAudioMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_PhraseIsTODOMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_PhraseIsUsedMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem Context_AssignRoleMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_AssignRole_PlainMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_AssignRole_HeadingMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_AssignRole_PageMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_AssignRole_SilenceMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem Context_AssignRole_NewCustomRoleMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_ApplyPhraseDetectionMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_AudioSelectionMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_AudioSelection_BeginMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_AudioSelection_EndMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem Context_CutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_CopyMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_PasteMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_PasteBeforeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_PasteInsideMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_DeleteMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem Context_PropertiesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Context_ClearRoleMenuItem;
        private System.Windows.Forms.Panel mStripsPanel;
        private System.Windows.Forms.HScrollBar mHScrollBar;
        private System.Windows.Forms.VScrollBar mVScrollBar;
        private System.Windows.Forms.Panel mCornerPanel;
    }
}