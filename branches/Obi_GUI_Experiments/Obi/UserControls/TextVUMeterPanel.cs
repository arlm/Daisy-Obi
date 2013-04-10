using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Obi.UserControls
{
    public partial class TextVUMeterPanel : UserControl
    {
        // member variables
        private Audio.VuMeter m_VuMeter;    // Instance of VuMeter 
                private String m_StrLeftOverloadIndicator;
        private String m_StrRightOverloadIndicator;
        private string m_strLeftLowLevelIndicator;
        private string m_strRightLowLevelIndicator;
        private bool m_BeepEnabled = false ;
        private bool m_OverLoadBeepEnabled = false;
        private bool mShowMaxMinValues;
        private double m_MaxLeftDB;
        private double m_MaxRightDB;
        private double m_MinLeftDB;
        private double m_MinRightDB;
        private int m_AfterGoodCount;
        

        public TextVUMeterPanel()
        {
            InitializeComponent();
                        mResetButton.Enabled = false;
            m_StrLeftOverloadIndicator = "";
            m_StrRightOverloadIndicator = "";
            m_strLeftLowLevelIndicator = "";
            m_strRightLowLevelIndicator = "";
            mShowMaxMinValues = false;
            m_BeepEnabled = false;
        }

        public Audio.VuMeter VuMeter
        {
            get
            {
                return m_VuMeter;
            }
            set
            {
                m_VuMeter = value;

                if (m_VuMeter  != null)
                {
                    m_VuMeter.PeakOverload += new Events.Audio.VuMeter.PeakOverloadHandler(CatchPeakOverloadEvent );
                    m_VuMeter.LevelTooLowEvent += new Obi.Events.Audio.VuMeter.LevelTooLowHandler(CatchLevelTooLowEvent);
                    //m_VuMeter.UpdateForms += new Events.Audio.VuMeter.UpdateFormsHandler(CatchUpdateForms);
                    m_VuMeter.ResetEvent += new Events.Audio.VuMeter.ResetHandler(VuMeter_ResetEvent);
                    m_VuMeter.LevelGoodEvent += new Obi.Events.Audio.VuMeter.LevelGoodHandler ( PlayLevelGoodSound );
                    m_MaxLeftDB = -100.00;
                    m_MaxRightDB = -100.00;
                    mResetButton.Enabled = mShowMaxMinValues;
                    m_AfterGoodCount = 0;
                                                        }
            }
        }

        public bool BeepEnable
        {
            get { return m_BeepEnabled; }
            set { m_BeepEnabled = value; }
        }

        public bool ShowMaxMinValues        
        {
            get { return mShowMaxMinValues; }
            set
            {
                mShowMaxMinValues = value;
                if (value) mResetButton.Enabled = true;
            }
        }


        private void tmUpdateText_Tick(object sender, EventArgs e)
        {
                        if (m_VuMeter != null)
            {
                                                                                    if (!m_VuMeter.IsLevelTooLow || mShowMaxMinValues)
                        UpdateRunningValues();
                    else 
                        UpdateRunningLowValues();
                                                               }
            
                                                           m_AfterGoodCount++;
        }

        private void UpdateRunningValues()
        {
            double LeftDb = 0;
            double RightDb = 0;
            if (VuMeter.PeakDbValue != null)
            {//1
                if (VuMeter.PeakDbValue.Length > 0)
                    LeftDb = VuMeter.PeakDbValue[0];

                if (VuMeter.PeakDbValue.Length > 1)
                    RightDb = VuMeter.PeakDbValue[1];

                if (LeftDb > 0)
                    LeftDb = 0.0;

                if (RightDb > 0)
                    RightDb = 0.0;

            }//-1
            if (!mShowMaxMinValues)
            {//1
                mLeftBox.Text = m_StrLeftOverloadIndicator + LeftDb.ToString();
                if (m_VuMeter.Channels == 2) mRightBox.Text = m_StrRightOverloadIndicator + RightDb.ToString();
                else mRightBox.Text  = "--";
            }//-1
            else   // show extreme high and expreme low
                SetExtremeValues(LeftDb, RightDb );

            if (m_OverLoadBeepEnabled)
            {
                PlayBeep();
                // only this function can set OverLoadbeep enable to false 
                m_OverLoadBeepEnabled = false;
            }
        }

        private void UpdateRunningLowValues()
        {
            //if (m_VuMeter != null && m_VuMeter.IsLevelTooLow)
            {
                mLeftBox.Text = m_strLeftLowLevelIndicator + m_VuMeter.AverageAmplitudeDBValue[0].ToString();
                if (m_VuMeter.Channels == 2) mRightBox.Text = m_strRightLowLevelIndicator + m_VuMeter.AverageAmplitudeDBValue[1].ToString();
                else mRightBox.Text = "--";
            }
        }


        // load the beep file and plays it once in case of overload
        private void PlayBeep()
        {
        FileInfo BeepFile = new FileInfo ( Path.Combine ( System.AppDomain.CurrentDomain.BaseDirectory, "hi.wav" ) );
        if (BeepFile.Exists && m_BeepEnabled)
            {
                System.Media.SoundPlayer PlayBeep = new System.Media.SoundPlayer(BeepFile.FullName);
                PlayBeep.Play();
            }
        }
        
        
        private void SetExtremeValues( double MaxLeftDB , double MaxRightDB ) 
        {
            // set textbox for left channel
            if (MaxLeftDB > m_MaxLeftDB)
                m_MaxLeftDB = MaxLeftDB;
            
                            string strMaxLeftDB = m_MaxLeftDB.ToString();  
            if ( strMaxLeftDB.Length > 5 )
                strMaxLeftDB = strMaxLeftDB.Substring(0, 5);


            string strMinLeftDB = m_MinLeftDB.ToString();
                        if ( strMinLeftDB.Length >5 )
                strMinLeftDB = strMinLeftDB.Substring(0, 5);

            mLeftBox.Text = m_StrLeftOverloadIndicator + strMaxLeftDB+ "/" + m_strLeftLowLevelIndicator + strMinLeftDB;
            
            // set text for right channel
            if (m_VuMeter.Channels == 2)
            {
                if (MaxRightDB > m_MaxRightDB)
                    m_MaxRightDB = MaxRightDB;

                string strMaxRightDB = m_MaxRightDB.ToString();
                if (strMaxRightDB.Length > 5)
                    strMaxRightDB = strMaxRightDB.Substring(0, 5);

                strMinLeftDB = m_MinRightDB.ToString();

                if (strMinLeftDB.Length > 5)
                    strMinLeftDB = strMinLeftDB.Substring(0, 5);

                mRightBox.Text = m_StrRightOverloadIndicator + strMaxRightDB + "/" + m_strLeftLowLevelIndicator + strMinLeftDB ;
            }
            else
                mRightBox.Text = "--" ;
        }

        void CatchPeakOverloadEvent(object sender, EventArgs e)
        {
            Obi.Events.Audio.VuMeter.PeakOverloadEventArgs EventOb = e as Obi.Events.Audio.VuMeter.PeakOverloadEventArgs;
            if (EventOb.Channel == 1)
                m_StrLeftOverloadIndicator = "OL ";
            else
                m_StrRightOverloadIndicator = "OL ";

            UpdateControls ();
            Audio.VuMeter ob_VuMeter = sender as Audio.VuMeter;


            // beep enabled false means this is first peak overload after text timer tick, so play beep
            if (m_OverLoadBeepEnabled== false)
            {
                PlayBeep();
                m_OverLoadBeepEnabled = true;
            }
        }

        private void CatchLevelTooLowEvent(object sender, Obi.Events.Audio.VuMeter.LevelTooLowEventArgs e)
        {
        if (m_AfterGoodCount >= 0)
            {
            m_MinLeftDB = e.LowLevelValue;
            if (m_VuMeter.Channels == 2) m_MinRightDB = e.LowLevelValue;


            m_strLeftLowLevelIndicator = "Low:";
            m_strRightLowLevelIndicator = "Low:";
            PlayLevelTooLowBeep ();
            }
        }


        private delegate void UpdateControlsCallBack () ;

        private void UpdateControls () 
        {
            if (InvokeRequired)
            {
                Invoke(new UpdateControlsCallBack( UpdateControls) )  ;
            }
            else
            {
                mResetButton.Enabled = true;
            }

        }


        private void CatchStateChangedEvent(object sender, EventArgs e){}
            delegate     void StateChangeCallBack () ;

        private void StateChangeFunction ()
        {
            if (InvokeRequired)
            {
                Invoke(new StateChangeCallBack(StateChangeFunction));
            }
            else
            { }
        }

        private void StateChangeOperations(){}

        private void mResetButton_Click(object sender, EventArgs e)
        {
            if ( !mShowMaxMinValues )
            mResetButton.Enabled = false ;

            m_StrLeftOverloadIndicator = "";
            m_StrRightOverloadIndicator = "";
            m_strLeftLowLevelIndicator = "";
            m_strRightLowLevelIndicator = "";

            if (m_VuMeter != null  )
            {
                m_VuMeter.Reset();
            }

            if (mShowMaxMinValues)
            {
                mLeftBox.Text = "";
                mRightBox.Text = "";
                m_MaxLeftDB = -100.00;
                m_MaxRightDB = -100.00;
                m_MinLeftDB = 0;
                m_MinRightDB = 0;
            }
        m_AfterGoodCount = 0;
                }

        private delegate  void   SetTextBoxCallBack  () ;

        private void VuMeter_ResetEvent( object sender  , EventArgs e )
        {
            m_MaxLeftDB = -100.00 ;
            m_MaxRightDB = -100;
            m_MinLeftDB = 0;
            m_MinRightDB = 0;
            
            SetResetText();
            m_AfterGoodCount = 0;
                    }

        private void SetResetText()
        {
            if (InvokeRequired)
            {
                Invoke(new  SetTextBoxCallBack  ( SetResetText ));
            }
            else
            {
                mLeftBox.Text = m_StrLeftOverloadIndicator;
                mRightBox.Text = m_StrRightOverloadIndicator;
            }
        }

        private void PlayLevelTooLowBeep()
        {
            string FilePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "low.wav");
                                    if (File.Exists(FilePath) && m_BeepEnabled)
            {
                System.Media.SoundPlayer LowBeepPlayer  = new System.Media.SoundPlayer(FilePath);
                LowBeepPlayer.Play();
            }
        }

        private void PlayLevelGoodSound ( object sender , EventArgs e)
            {
            string FilePath = Path.Combine ( System.AppDomain.CurrentDomain.BaseDirectory, "good.wav" );
            if (File.Exists ( FilePath ) && m_BeepEnabled)
                {
                System.Media.SoundPlayer LevelGoodSoundpPlayer = new System.Media.SoundPlayer ( FilePath );
                LevelGoodSoundpPlayer.Play ();
                }

            m_AfterGoodCount = -4 ;
                        }


    }// end of class
}