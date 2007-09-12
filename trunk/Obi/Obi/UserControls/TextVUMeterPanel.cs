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
        private bool IsPlay ;
        private Playlist mPlaylist = null;
        private RecordingSession mRecordingSession = null;
        public bool Enable = false;
        private String m_StrLeftOverloadIndicator;
        private String m_StrRightOverloadIndicator;
        private bool m_BeepEnabled = false;


        public TextVUMeterPanel( )
        {
            InitializeComponent();
            
            mResetButton.Enabled = false;
            m_StrLeftOverloadIndicator = "";
            m_StrRightOverloadIndicator = "";


        }
        
    public Playlist PlayListObj
    {
        get
        {
            return mPlaylist;
        }
        set
        {
            if ( Enable )
            SetPlayer(value);

        }
    }

        public RecordingSession RecordingSessionObj
        {
            get
            {
                return mRecordingSession;
            }
            set
            {
                if (Enable)
                SetRecorder(value);

            }
        }


        private void SetPlayer( Playlist PlayListArg )
        {
            
            mPlaylist = PlayListArg ;
            mRecordingSession = null;
            IsPlay = true;

            // Associate events
            mPlaylist.Audioplayer.VuMeter.PeakOverload += new Obi.Events.Audio.VuMeter.PeakOverloadHandler( CatchPeakOverloadEvent  );
            mPlaylist.Audioplayer.StateChanged += new Obi.Events.Audio.Player.StateChangedHandler( CatchStateChangedEvent );
mPlaylist.Audioplayer.VuMeter.ResetEvent += new Obi.Events.Audio.VuMeter.ResetHandler(VuMeter_ResetEvent);

            Enable = false;
        }


        private void SetRecorder(RecordingSession RecordingSessionArg)
        {
            mRecordingSession = RecordingSessionArg ;
            mPlaylist = null;
            IsPlay = false;

            // Associate events
            mRecordingSession.AudioRecorder.VuMeterObject.PeakOverload += new Obi.Events.Audio.VuMeter.PeakOverloadHandler ( CatchPeakOverloadEvent ) ; 
            mRecordingSession.AudioRecorder.StateChanged += new Obi.Events.Audio.Recorder.StateChangedHandler ( CatchStateChangedEvent );

            Enable = false;
        }   


        private void tmUpdateText_Tick(object sender, EventArgs e)
        {
            if (IsPlay == true && mPlaylist != null )
            {
                if (mPlaylist.Audioplayer.State == Audio.AudioPlayerState.Playing)
                {
                    double LeftDb = 20 *  Math.Log10 (  mPlaylist.Audioplayer.VuMeter.m_MeanValueLeft * 256  ) ;
                    double RightDb = 20 * Math.Log10(mPlaylist.Audioplayer.VuMeter.m_MeanValueRight * 256);

                    if (LeftDb < 1)
                        LeftDb = 0.0;

                    if (RightDb < 1)
                        RightDb = 0.0;

                    //double LeftDb = mPlaylist.Audioplayer.VuMeter.m_MeanValueLeft ;
                    //double RightDb = (mPlaylist.Audioplayer.VuMeter.m_MeanValueRight );

                    mLeftBox.Text = m_StrLeftOverloadIndicator +  LeftDb.ToString();
                    mRightBox.Text = m_StrRightOverloadIndicator +  RightDb.ToString();
                                    }
            }
            else if (IsPlay == false && mRecordingSession != null )
            {
                if ( mRecordingSession.AudioRecorder.State == Audio.AudioRecorderState.Recording || mRecordingSession.AudioRecorder.State == Obi.Audio.AudioRecorderState.Listening )
                {
                    double LeftDb = 20 * Math.Log10 ( mRecordingSession.AudioRecorder.VuMeterObject.m_MeanValueLeft   * 256 ) ;
                    double RightDb =  20 * Math.Log10(mRecordingSession.AudioRecorder.VuMeterObject.m_MeanValueRight * 256);

                    if ( LeftDb < 1 )
                        LeftDb =  0.0 ;

                    if ( RightDb < 1)
                        RightDb = 0.0 ;

                    mLeftBox.Text = m_StrLeftOverloadIndicator +  LeftDb.ToString();
                    mRightBox.Text = m_StrRightOverloadIndicator +  RightDb.ToString();
                }
            }
            if (m_BeepEnabled)
            {
                PlayBeep();
                // only this function can set beep enable to false 
                m_BeepEnabled = false;
            }
        }

        // load the beep file and plays it once in case of overload
        private void PlayBeep()
        {
            FileInfo BeepFile = new FileInfo("Beep.wav");
            if (BeepFile.Exists)
            {
                System.Media.SoundPlayer PlayBeep = new System.Media.SoundPlayer("Beep.wav");
                PlayBeep.Play();
            }
        }

        void CatchPeakOverloadEvent(object sender, EventArgs e)
        {
            Obi.Events.Audio.VuMeter.PeakOverloadEventArgs EventOb = e as Obi.Events.Audio.VuMeter.PeakOverloadEventArgs;
            if (EventOb.Channel == 1)
                m_StrLeftOverloadIndicator = "OL ";
            else
                m_StrRightOverloadIndicator = "OL ";

            Overload();
            Audio.VuMeter ob_VuMeter = sender as Audio.VuMeter;


            // beep enabled false means this is first peak overload after text timer tick, so play beep
            if (m_BeepEnabled == false)
            {
                PlayBeep();
                m_BeepEnabled = true;
            }
        }

        private delegate void OverloadCallBack () ;

        private void Overload () 
        {
            if (InvokeRequired)
            {
                Invoke(new OverloadCallBack ( Overload ) )  ;
            }
            else
            {
                mResetButton.Enabled = true;
            }

        }


        private void CatchStateChangedEvent(object sender, EventArgs e)
        {
        }

            delegate     void StateChangeCallBack () ;

        private void StateChangeFunction ()
        {
            if (InvokeRequired)
            {
                Invoke(new StateChangeCallBack ( StateChangeFunction ));
            }
            else
            {
                
            }
        }

        private void StateChangeOperations()
        {
            if (IsPlay == true)
            {

            }
            else if (IsPlay == false)
            {

            }
        }

        private void mResetButton_Click(object sender, EventArgs e)
        {
            mResetButton.Enabled = false;
            m_StrLeftOverloadIndicator = "";
            m_StrRightOverloadIndicator = "";

            if (IsPlay)
            {
                mPlaylist.Audioplayer.VuMeter.Reset();
            }
            else if (IsPlay == false)
            {
                mRecordingSession.AudioRecorder.VuMeterObject.Reset();
            }
        }

        private delegate  void   SetTextBoxCallBack  () ;

        private void VuMeter_ResetEvent( object sender  , EventArgs e )
        {
            SetResetText();
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


    }// end of class
}
