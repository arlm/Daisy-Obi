using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.UserControls
{
    public partial class MetadataPanel : UserControl
    {
        public delegate void MetadataPanelDeletedHandler(object sender, EventArgs e);
        public event MetadataPanelDeletedHandler Deleted; 

        public MetadataPanel()
        {
            InitializeComponent();
            mNameBox.Items.AddRange(MetadataEntryDescription.GetDAISYEntries().ToArray());
        }

        public string EntryName
        {
            get { return mNameBox.SelectedItem.ToString(); }
            set
            {
                foreach (object item in mNameBox.Items)
                {
                    if (item.ToString() == value)
                    {
                        mNameBox.SelectedItem = item;
                        break;
                    }
                }
            }
        }
        
        public string EntryContent
        {
            get { return mContentBox.Text; }
            set { mContentBox.Text = value; }
        }

        private void mDeleteButton_Click(object sender, EventArgs e)
        {
            if (Deleted != null) Deleted(this, new EventArgs());
        }
    }
}
