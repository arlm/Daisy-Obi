using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
{
    public partial class MetadataView : UserControl, IControlWithSelection
    {
        private ProjectView mView;             // parent project view
        private MetadataSelection mSelection;  // current selection
        
        public MetadataView()
        {
            InitializeComponent();
            mView = null;
            mSelection = null;
        }


        /// <summary>
        /// A new entry of the given kind can be added if this is not readonly, and if it is repeatable
        /// or there is not yet any occurrence of it.
        /// </summary>
        public bool CanAdd(MetadataEntryDescription d)
        {
            return !d.ReadOnly && (d.Repeatable || mView.Presentation.getListOfMetadata(d.Name).Count == 0);
        }

        /// <summary>
        /// A particular entry can be removed if it is not readonly and not the only occurrence in case of a required entry. 
        /// </summary>
        public bool CanRemove(MetadataEntryDescription d)
        {
            return !d.ReadOnly && (d.Occurrence != MetadataOccurrence.Required || mView.Presentation.getListOfMetadata(d.Name).Count > 1);
        }

        /// <summary>
        /// A metadata entry can be removed if it is selected. TODO: check that it is not mandatory!
        /// </summary>
        public bool CanRemoveMetadata { get { return mSelection != null && CanRemove(mSelection.Item.Description); } }

        /// <summary>
        /// The project view is showing a new presentation;
        /// reset the list of metadata panels.
        /// </summary>
        public void NewPresentation()
        {
            if (mView.Presentation != null)
            {
                ImportMetadata();
                //mView.Presentation.MetadataEntryAdded += new MetadataEventHandler(Presentation_MetadataEntryAdded);
                //mView.Presentation.MetadataEntryDeleted += new MetadataEventHandler(Presentation_MetadataEntryDeleted);
                //mView.Presentation.MetadataEntryContentChanged += new MetadataEventHandler(Presentation_MetadataEntryContentChanged);
                //mView.Presentation.MetadataEntryNameChanged += new MetadataEventHandler(Presentation_MetadataEntryNameChanged);
            }
        }

        /// <summary>
        /// The parent project view. Should be set ASAP, and only once.
        /// </summary>
        public ProjectView ProjectView
        {
            set
            {
                if (mView != null) throw new Exception("Cannot set the project view again!");
                mView = value;
            }
        }

        /// <summary>
        /// Get or set the current selection in the view.
        /// </summary>
        public NodeSelection Selection
        {
            get { return mSelection; }
            set
            {
                if (mSelection != null) mSelection.Item.Selected = false;
                mSelection = value as MetadataSelection;
                if (mSelection != null) mSelection.Item.Selected = true;
            }
        }


        // Clear the selection
        private void ClearSelection()
        {
            mSelection = null;
            mNameTextbox.Text = "";
            mContentTextbox.Text = "";
        }

        // Import metadata entries from the presentation
        private void ImportMetadata()
        {
            mMetadataListView.Items.Clear();
            string[] nameContent = new string[2];
            List<urakawa.metadata.Metadata> items = mView.Presentation.getListOfMetadata();
            items.Sort(delegate(urakawa.metadata.Metadata a, urakawa.metadata.Metadata b)
            {
                int names = a.getName().CompareTo(b.getName());
                return names == 0 ? a.getContent().CompareTo(b.getContent()) : names;
            });
            foreach (urakawa.metadata.Metadata m in items)
            {
                nameContent[0] = m.getName();
                nameContent[1] = m.getContent();
                ListViewItem item = new ListViewItem(nameContent);
                mMetadataListView.Items.Add(item);
                item.Checked = true;
                item.Tag = m;
            }
            List<string> addables = mView.AddableMetadataNames;
            addables.Sort();
            foreach (string name in addables)
            {
                nameContent[0] = name;
                nameContent[1] = "";
                ListViewItem item = new ListViewItem(nameContent);
                mMetadataListView.Items.Add(item);
                item.Checked = false;
                item.Tag = null;
            }
        }

        // Commit a change
        private void mCommitButton_Click(object sender, EventArgs e)
        {
            if (mSelection != null)
            {
                urakawa.metadata.Metadata entry = mSelection.Item.Entry;
                urakawa.undo.CompositeCommand command =
                    mView.Presentation.CreateCompositeCommand(Localizer.Message("modify_metadata_entry"));
                if (entry.getName() != mNameTextbox.Text)
                {
                    command.append(new Commands.Metadata.ModifyName(mView, entry, mNameTextbox.Text));
                }
                if (entry.getContent() != mContentTextbox.Text)
                {
                    command.append(new Commands.Metadata.ModifyContent(mView, entry, mContentTextbox.Text));
                }
                if (command.getCount() > 0) mView.Presentation.getUndoRedoManager().execute(command);
            }
        }

        private void mMetadataListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mMetadataListView.SelectedItems.Count == 0)
            {
                ClearSelection();
            }
            else
            {
                SetSelection(mMetadataListView.SelectedItems[0]);
            }
        }

        private void SetSelection(ListViewItem item)
        {
            mSelection = new MetadataSelection(mView.Presentation.RootNode, this,
                new MetadataItemSelection(item, MetadataEntryDescription.GetDAISYEntries()[item.Text]));
            mNameTextbox.Text = item.Text;
            mNameTextbox.AccessibleName = item.Text;
            bool editableName = !item.Checked || CanRemoveMetadata;
            mNameTextbox.TabStop = editableName;
            mNameTextbox.Enabled = editableName;
            mContentTextbox.Text = item.SubItems[1].Text;
            bool editableContent = !mSelection.Item.Description.ReadOnly;
            mContentTextbox.TabStop = editableContent;
            mContentTextbox.Enabled = editableContent;
        }










        /// <summary>
        /// Called from panels to modify the content of an entry
        /// </summary>
        public void ModifiedEntryContent(urakawa.metadata.Metadata entry, string content)
        {
            //if (entry.getContent() != content) 
            
            {
                mView.Presentation.getUndoRedoManager().execute(new Commands.Metadata.ModifyContent(mView, entry, content));
            }
        }
        
        protected override bool ProcessDialogKey(Keys KeyData)
        {
            if (KeyData == Keys.Tab
                && this.ActiveControl != null)
            {
                Control c = this.ActiveControl;
                this.SelectNextControl(c, true, true, true, true);
                if (this.ActiveControl != null && c.TabIndex > this.ActiveControl.TabIndex)
                    System.Media.SystemSounds.Beep.Play();

                return true;
            }
            else if (KeyData == (Keys)(Keys.Shift | Keys.Tab)
                && this.ActiveControl != null)
            {
                Control c = this.ActiveControl;
                this.SelectNextControl(c, false, true, true, true);
                if (this.ActiveControl != null && c.TabIndex < this.ActiveControl.TabIndex)
                    System.Media.SystemSounds.Beep.Play();

                return true;
            }
            else
                return base.ProcessDialogKey(KeyData);
        }

        private void mMetadataListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item.Checked)
            {
                if (GetMetadataFromList(e.Item.Text) == null)
                    mView.AddMetadataEntry(e.Item.Text);
            }
            else
            {
                // add delete metadata code here
            }
            //ItemSelectionAction();
        }

        /*
        private void ItemSelectionAction()
        {
            if (mMetadataListView.SelectedIndices.Count > 0)
            {
                int FocussedIndex = mMetadataListView.Items.IndexOf(mMetadataListView.FocusedItem);

                if (mMetadataListView.Items[FocussedIndex].Checked)
                {
                    mSelectedMetadata = GetMetadataFromList(mMetadataListView.Items[FocussedIndex].Text);
                    mContentTextbox.Text = mSelectedMetadata.getContent();
                    mContentTextbox.Enabled = true;
                    mCommitButton.Enabled = true;
                }
                else
                {
                    mContentTextbox.Enabled = false;
                    mCommitButton.Enabled = false;
                }

                mNameTextbox.Text = mMetadataListView.Items[FocussedIndex].SubItems[0].Text;
                mNameTextbox.TabStop = false;
                mNameTextbox.ReadOnly = true;
                mContentTextbox.AccessibleName = mNameTextbox.Text + ":";
            } // index check ends
            else
            {
                // nothing selected
                mNameTextbox.Text = "Metadata ";
                mContentTextbox.AccessibleName = mNameTextbox.Text + ":";
                mContentTextbox.Enabled = false;
            }
        }*/

        urakawa.metadata.Metadata GetMetadataFromList(string Name)
        {
            foreach (urakawa.metadata.Metadata m in mView.Presentation.getListOfMetadata())
                        {
                            if (m.getName() == Name)
                                return m;
            }
            return null;
        }


        /*private void UpdateCommitedMetadataInListView(string MetadataName)
        {
            ListViewItem item = mMetadataListView.FindItemWithText(MetadataName, true, 0);
            if (item == null)
                MessageBox.Show("Item null");

            string[] MetadataString = new string[2];
            MetadataString[0] = mSelectedMetadata.getName();
            MetadataString[1] = mSelectedMetadata.getContent();

            ListViewItem itemNew = new ListViewItem(MetadataString);
            itemNew.Checked = true;
            mMetadataListView.Items.Insert(item.Index + 1, itemNew);
            mMetadataListView.Items.RemoveAt(item.Index);
        }*/


    }

    public class MetadataItemSelection
    {
        private ListViewItem mItem;                     // item in the list view
        private MetadataEntryDescription mDescription;  // and corresponding description (may be null for free metadata)

        public MetadataItemSelection(ListViewItem item, MetadataEntryDescription description)
        {
            mItem = item;
            mDescription = description;
        }

        public urakawa.metadata.Metadata Entry { get { return (urakawa.metadata.Metadata)mItem.Tag; } }
        public MetadataEntryDescription Description { get { return mDescription; } }
        public bool Selected { set { mItem.Selected = true; } }
    }
}
