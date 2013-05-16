﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using urakawa.media.timing;
using AudioLib;

namespace Obi.ProjectView
{
    public partial class ZoomWaveform : UserControl, ISelectableInContentViewWithColors
    {
        private ContentView m_ContentView = null;
        private Strip m_Strip;
        private  EmptyNode m_Node;
        private AudioBlock m_AudioBlock;
        private int initialWaveformWidth = 0;
        private float m_ZoomFactor = 0;
        private ProjectView m_ProjectView;
        private bool m_IsPanelSizeMax = false;
        private int m_count = 0;
       
        private ZoomWaveform()
        {
            
            InitializeComponent();
            this.Controls.Add(panelZooomWaveform);       
            this.Controls.Add(txtZoomSelected);
            
        }
        public void SetSelectionFromContentView(NodeSelection selection) 
        {
            if (m_AudioBlock != null) m_AudioBlock.SetSelectionFromContentView(selection);
        }
        private bool m_Highlighted;
        public bool Highlighted { get { return m_Highlighted; } set { m_Highlighted =  value; } }
        public ObiNode ObiNode { get { return m_Node; } }
        ColorSettings m_ColorSettings ;
        public ColorSettings ColorSettings { get { return m_ColorSettings; } set { m_ColorSettings = value; } }
        public ContentView ContentView { get { return m_ContentView; } }
        public Strip Strip { get { return m_Strip; } }
        public string ToMatch() { return null; }

        public void UpdateCursorTime (double time ) 
        {
            if( m_AudioBlock != null ) m_AudioBlock.UpdateCursorTime (time) ;
            
        }
        public ZoomWaveform ZoomWaveformObj
        {
            get { return this; }
        }
        
        private void ProjectViewSelectionChanged ( object sender, EventArgs e )
        {
            if (m_ProjectView.Selection != null && m_ProjectView.Selection.Phrase != null && m_ProjectView.GetSelectedPhraseSection != null)
            {
                txtZoomSelected.Text=" ";
                txtZoomSelected.Text += " " + m_ProjectView.Selection.ToString();
                txtZoomSelected.Text +=" "+ m_ProjectView.GetSelectedPhraseSection.ToString();
                string temp = m_ProjectView.Selection.Node.ToString();
                if (m_AudioBlock.Node.ToString() != temp && m_ProjectView.Selection.EmptyNodeForSelection != null)
                    
                {
                   PhraseLoad(m_ProjectView.Selection.EmptyNodeForSelection);
                }
              
               // txtZoomSelected.Text += m_ProjectView.Selection.Phrase.ToString();
            }
        }
         private void ZoomPanelLostFocus(object sender,EventArgs e)
         {
            // this.ActiveControl = panelZooomWaveform;
             if(this.ActiveControl==txtZoomSelected)
             {
                 this.ActiveControl = txtZoomSelected;
             }
         }
        private void ZoomPanelResize(object sender,EventArgs e)
        {
            this.Height = m_ContentView.Height-22;
            this.Width = m_ContentView.Width;
            txtZoomSelected.Width = this.Width - 40;           
        }
         public void ZoomAudioFocus()
         {
             if(this.ActiveControl==btnClose || this.ActiveControl==txtZoomSelected)
             {
                m_AudioBlock.TabStop = false;
             }
             if(m_AudioBlock.FlagMouseDown)
             {
                 m_AudioBlock.FlagMouseDown = false;
                 this.ActiveControl = txtZoomSelected;
             }
            // this.ActiveControl = btnClose;
         }
        public void PhraseLoad(EmptyNode phrase)
        {
            //m_AudioBlock.Node.Parent;
            m_Node = phrase;
            if (m_Node is PhraseNode)
            {
                if (panelZooomWaveform.Controls.Contains(m_AudioBlock))
                {
                    panelZooomWaveform.Controls.Remove(m_AudioBlock);
                }
                m_AudioBlock = new AudioBlock((PhraseNode)m_Node, m_Strip);
                panelZooomWaveform.Controls.Add(m_AudioBlock);
                m_AudioBlock.Location = new Point(0, 0);
                float zoomFactor = panelZooomWaveform.Height / m_AudioBlock.Height;
                txtZoomSelected.Location = new Point(0, this.Height - 50);
                txtZoomSelected.BringToFront();
                m_ZoomFactor = zoomFactor;
                //   m_AudioBlock.Width = m_ContentView.Width;
              
                m_AudioBlock.SetZoomFactorAndHeight(zoomFactor, Height);
                
                initialWaveformWidth = m_AudioBlock.Waveform.Width;
                m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
                m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
                if (m_AudioBlock != null)
                {
                //    m_AudioBlock.MouseDown += new MouseEventHandler(m_AudioBlock_MouseDown);
                    m_AudioBlock.GotFocus += new EventHandler(m_AudioBlock_MouseDown);
                }
                m_AudioBlock.TabStop = false;
                //  m_AudioBlock.SetWaveformForZoom(m_Node as PhraseNode,zoomFactor);
                //int a=  m_AudioBlock.ComputeWaveformDefaultWidth();
                //m_AudioBlock.Waveform.Render();
                m_AudioBlock.InitCursor(0);
                
            }
        }
       public ZoomWaveform(ContentView contentView, Strip strip,EmptyNode node,ProjectView mProjectView ):this    ()
        {
           
            m_ContentView = contentView;
            m_ProjectView = mProjectView;
            m_ProjectView.SelectionChanged += new EventHandler(ProjectViewSelectionChanged);
            this.LostFocus += new EventHandler(ZoomPanelLostFocus);

          
            //panelZooomWaveform.LostFocus+=new EventHandler(ZoomPanelLostFocus);
            m_ContentView.Resize += new EventHandler(ZoomPanelResize);
            m_Strip = strip;
            m_Node = node;
            if (m_ProjectView.Selection.Phrase != null)
            {
                if (m_ContentView != null)
                {                    
                    this.Height = m_ContentView.Height - 22;
                    this.Width = m_ContentView.Width;
                    this.MouseWheel += new MouseEventHandler(ZoomWaveform_MouseWheel);
                  //  btnClose.Anchor = AnchorStyles.None;
                    btnClose.Location = new Point(btnClose.Location.X, this.Height - 25);
                   // btnNextPhrase.Anchor = AnchorStyles.None;
                    btnNextPhrase.Location = new Point(btnNextPhrase.Location.X, this.Height - 25);
                    btnPreviousPhrase.Location = new Point(btnPreviousPhrase.Location.X, this.Height - 25);
                    btnReset.Location = new Point(btnReset.Location.X, this.Height - 25);
                    btnZoomIn.Location = new Point(btnZoomIn.Location.X, this.Height - 25);
                    btnZoomOut.Location = new Point(btnZoomOut.Location.X, this.Height - 25);
                    panelZooomWaveform.Width = this.Width - 30;
                    panelZooomWaveform.Height = this.Height - 60;
                    txtZoomSelected.Width = this.Width - 40;
                }
            }
           //this.Width=m_ContentView.Width;
            if (m_Node is PhraseNode)
            {
               
                m_AudioBlock = new AudioBlock((PhraseNode)m_Node, m_Strip);
                panelZooomWaveform.Controls.Add(m_AudioBlock);
                m_AudioBlock.Location = new Point(0, 0);
                float zoomFactor = panelZooomWaveform.Height / m_AudioBlock.Height;
                txtZoomSelected.Location = new Point(0,this.Height-50);
                txtZoomSelected.BringToFront();
                m_ZoomFactor = zoomFactor;
              
                m_AudioBlock.SetZoomFactorAndHeight(zoomFactor, Height);     
         
                
                initialWaveformWidth = m_AudioBlock.Waveform.Width;
                m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
                m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);                         
                this.AutoScrollMinSize = new Size(this.Width, this.Height+15);                
                m_AudioBlock.InitCursor(0);
                m_AudioBlock.Focus();
                this.ActiveControl = btnClose;
            }
            m_count = 0;
            
        }

       void ZoomWaveform_MouseWheel(object sender, MouseEventArgs e)
       {
           Console.WriteLine("Delta Value of MouseWheel is{0}",e.Delta);
           if(e.Delta<0)
           {
               int tempWidth = m_AudioBlock.Waveform.Width - (int)(initialWaveformWidth * 0.5);
               if (tempWidth > (initialWaveformWidth / 10))
               {
                   m_AudioBlock.Waveform.Width = tempWidth;
                   m_AudioBlock.SetZoomFactorAndHeightForZoom(m_ZoomFactor, Height);
                   m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
                   m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
               }
               
           }
           if(e.Delta>0)
           {
               int tempWidth = m_AudioBlock.Waveform.Width + (int)(initialWaveformWidth * 0.5);
               if (tempWidth < (initialWaveformWidth * 60))
               {
                   m_AudioBlock.Waveform.Width = tempWidth;
                   m_AudioBlock.SetZoomFactorAndHeightForZoom(m_ZoomFactor, Height);
                   m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
                   m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
               }
           }
           //  throw new NotImplementedException();
       }

       private void m_AudioBlock_MouseDown(object sender, EventArgs e)
       {

          // throw new NotImplementedException();
       }



        
      

        private void btnClose_Click(object sender, EventArgs e)
        {
         m_ContentView.RemovePanel();
         m_ProjectView.SelectionChanged -= new EventHandler(ProjectViewSelectionChanged);
        }

        private void btnNextPhrase_Click(object sender, EventArgs e)
        {
                
                ObiNode nextNode = m_Node.FollowingNode;
            if (nextNode!=null && nextNode.Parent != null && m_AudioBlock.Node.Parent != null)
            {
                if (m_AudioBlock.Node.Parent == nextNode.Parent)
                {
                    if (m_Node.FollowingNode is PhraseNode)
                    {
                        m_Node = nextNode as PhraseNode;
                        if (panelZooomWaveform.Controls.Contains(m_AudioBlock))
                        {
                            panelZooomWaveform.Controls.Remove(m_AudioBlock);
                        }
                        m_AudioBlock = new AudioBlock((PhraseNode) nextNode, m_Strip);
                        panelRender();
                    }
                    txtZoomSelected.Text = " ";
                }
            }

        }


        private void btnPreviousPhrase_Click(object sender, EventArgs e)
        {
            ObiNode previousNode = m_Node.PrecedingNode;
            if (previousNode != null && previousNode.Parent != null && m_AudioBlock.Node.Parent != null)
            {
                if (m_AudioBlock.Node.Parent == previousNode.Parent)
                {
                    if (m_Node.PrecedingNode is PhraseNode)
                    {
                        m_Node = previousNode as PhraseNode;
                        if (panelZooomWaveform.Controls.Contains(m_AudioBlock))
                        {
                            panelZooomWaveform.Controls.Remove(m_AudioBlock);
                        }
                        m_AudioBlock = new AudioBlock((PhraseNode) previousNode, m_Strip);
                        panelRender();
                    }
                    txtZoomSelected.Text = " ";
                }

            }
        }

        public void panelRender()
        {
            panelZooomWaveform.Controls.Add(m_AudioBlock);
            m_AudioBlock.Location = new Point(0, 0);
            initialWaveformWidth = m_AudioBlock.Waveform.Width;
            m_AudioBlock.SetZoomFactorAndHeight(m_ZoomFactor, panelZooomWaveform.Height);
            m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
            m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
            m_AudioBlock.InitCursor(0);
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            int tempWidth = m_AudioBlock.Waveform.Width + (int)(initialWaveformWidth * 0.5);
            if (tempWidth < (initialWaveformWidth * 60))
            {
                m_AudioBlock.Waveform.Width = m_AudioBlock.Waveform.Width + (int)(initialWaveformWidth * 0.5);
                m_AudioBlock.SetZoomFactorAndHeightForZoom(m_ZoomFactor, Height);
                m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
                m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
            }

        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            int tempWidth = m_AudioBlock.Waveform.Width - (int)(initialWaveformWidth * 0.5);
            if (tempWidth > (initialWaveformWidth / 10))
            {
                m_AudioBlock.Waveform.Width = m_AudioBlock.Waveform.Width - (int)(initialWaveformWidth * 0.5);
                m_AudioBlock.SetZoomFactorAndHeightForZoom(m_ZoomFactor, Height);
                m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
                m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
            }
            
 
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            m_AudioBlock.Waveform.Width = initialWaveformWidth;
            m_AudioBlock.SetZoomFactorAndHeightForZoom(m_ZoomFactor, Height);
            m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
            m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);

        }

 protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
     if ( m_ProjectView.ObiForm.KeyboardShortcuts == null ) return false ;
     KeyboardShortcuts_Settings keyboardShortcuts = m_ProjectView.ObiForm.KeyboardShortcuts;
            this.Focus();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Zoomfactor is {0}",m_ProjectView.ZoomFactor);
        

            if (m_ProjectView.ZoomFactor > 1.4)
            {
                m_ProjectView.ObiForm.ZoomFactor = 1.4f;
                return false;
            }
            
            //if (keyData == (Keys.Control | Keys.Alt | Keys.Oemplus))
            //{
            //    double tempZoomfactor = m_ProjectView.ZoomFactor;
            //    m_count++;
            //    if (m_count > 2)
            //    {
            //        m_count--;
            //        return false;
            //    }
              
               
            //}
            //if (keyData == (Keys.Control | Keys.Alt | Keys.OemMinus))
            //{
            //    double tempZoomfactor = m_ProjectView.ZoomFactor;
            //    m_count--;
            //    if (m_count > 2)
            //    {
            //        return false;
            //    }
               
            //}
            if (keyData == keyboardShortcuts.ContentView_SelectUp.Value)
            {
                IControlWithSelection tempControl;
                tempControl = m_ProjectView.Selection.Control; 
                m_ProjectView.Selection = new NodeSelection((ObiNode)  m_Node, tempControl);         
            }
            if (keyData == keyboardShortcuts.ContentView_ScrollDown_LargeIncrementWithSelection.Value || keyData == keyboardShortcuts.ContentView_ScrollUp_LargeIncrementWithSelection.Value
                || keyData == keyboardShortcuts.ContentView_SelectFollowingStripCursor.Value || keyData == keyboardShortcuts.ContentView_SelectPrecedingStripCursor.Value
                || keyData == keyboardShortcuts.ContentView_SelectFollowingStrip.Value || keyData == keyboardShortcuts.ContentView_SelectPrecedingStrip.Value
                || keyData == keyboardShortcuts.ContentView_SelectFirstStrip.Value || keyData == keyboardShortcuts.ContentView_SelectLastStrip.Value)
            // to do: handle global shortcuts after merging with trunck
            // || keyData == (Keys.Control | Keys.H)
            //|| keyData == (Keys.Control | Keys.Alt | Keys.H) || keyData == (Keys.Control | Keys.Shift | Keys.Q) || keyData == (Keys.Control | Keys.B)
            //|| keyData == (Keys.Control | Keys.Shift | Keys.B) || keyData == (Keys.Control | Keys.Shift | Keys.Space))
            {
                return false;
            }
            return base.ProcessCmdKey(ref msg, keyData);
            // return true;

        }
        protected override bool ProcessTabKey(bool forward)
        {
            Console.WriteLine("Active Control is {0}",this.ActiveControl);
            this.Focus();
            if (this.ActiveControl == txtZoomSelected)
            {
                this.ActiveControl = btnClose;
            }
            else
            {
                return base.ProcessTabKey(forward);
                
            }
            return forward;
        }
   

       // public Panel Panel_WaveForm { get { return panel_ZoomWaveform; } }
    }
}