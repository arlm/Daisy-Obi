using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;

namespace Obi.Dialogs
{
    /// <summary>
    /// The record dialog.
    /// Start listening as soon as it is open.
    /// 
    /// </summary>
    /// <remarks>JQ</remarks>
    public partial class Record : Form
    {
        private int mChannels;                  // required number of channels
        private int mSampleRate;                // required sample rate
        private int mBitDepth;                  // required bit depth
        private Assets.AssetManager mAssManager;       // the asset manager (for creating new assets)
        private Audio.VuMeter ob_VuMeter = new Audio.VuMeter();

        private List<Assets.AudioMediaAsset> mAssets;  // the list of assets created while recording

        public event Events.Audio.Recorder.StartingPhraseHandler StartingPhrase;
        public event Events.Audio.Recorder.ContinuingPhraseHandler ContinuingPhrase;
        public event Events.Audio.Recorder.FinishingPhraseHandler FinishingPhrase;

        /// <summary>
        /// The list of assets created.
        /// </summary>
        public List<Assets.AudioMediaAsset> Assets
        {
            get
            {
                return mAssets;
            }
        }

        public Record(int channels, int sampleRate, int bitDepth, Assets.AssetManager assManager)
        {
            InitializeComponent();
            mChannels = channels;
            mSampleRate = sampleRate;
            mBitDepth = bitDepth;
            mAssManager = assManager;
            mAssets = new List<Assets.AudioMediaAsset>();
            Audio.AudioRecorder.Instance.StateChanged += new Events.Audio.Recorder.StateChangedHandler(AudioRecorder_StateChanged);
            Audio.AudioRecorder.Instance.UpdateVuMeterFromRecorder +=
                new Events.Audio.Recorder.UpdateVuMeterHandler(AudioRecorder_UpdateVuMeter);
        }

        private void AudioRecorder_StateChanged(object sender, Events.Audio.Recorder.StateChangedEventArgs state)
        {
        }

        private void AudioRecorder_UpdateVuMeter(Object sender, Events.Audio.Recorder.UpdateVuMeterEventArgs update)
        {
        }

        private void Record_Load(object sender, EventArgs e)
        {
            //suman
            //the state changed events do not trigger
            //the text of the Record button changes to an empty string on using localizer.message
            //on closing the record form the recording does not stop
            //phrase marker and volume control are incomplete
            ArrayList arDevices = new ArrayList();
            arDevices = Audio.AudioRecorder.Instance.GetInputDevices();

            Audio.AudioRecorder.Instance.InitDirectSound(1);
            mRecordButton.Text = Localizer.Message("record");
            //              mRecordButton.Text = "&Pause";
            ob_VuMeter.ScaleFactor = 2;
            ob_VuMeter.SampleTimeLength = 2000;
            ob_VuMeter.UpperThreshold = 150;
            ob_VuMeter.LowerThreshold = 100;
            Audio.AudioRecorder.Instance.VuMeterObject = ob_VuMeter;
            ob_VuMeter.ShowForm();
            // Assets.AudioMediaAsset mAudioAsset = new Assets.AudioMediaAsset(mChannels, mBitDepth, mSampleRate);
            // mAssManager.AddAsset(mAudioAsset);
            Assets.AudioMediaAsset mAudioAsset = mAssManager.NewAudioMediaAsset(mChannels, mBitDepth, mSampleRate);
            Audio.AudioRecorder.Instance.StartListening(mAudioAsset);
            timer1.Enabled = true;
        }


        private void btnRecordAndPause_Click(object sender, EventArgs e)
        {
            // AudioRecorder.Instance.InitDirectSound(mIndex);
            // Assets.AudioMediaAsset mRecordAsset = new Assets.AudioMediaAsset(mChannels, mBitDepth, mSampleRate);
            // mAssManager.AddAsset(mRecordAsset);
            Assets.AudioMediaAsset mRecordAsset = mAssManager.NewAudioMediaAsset(mChannels, mBitDepth, mSampleRate);
            Audio.AudioRecorder.Instance.StopRecording();
            timer1.Enabled = false;
            mRecordButton.Text = Localizer.Message("record");

            if (Audio.AudioRecorder.Instance.State.Equals(Audio.AudioRecorderState.Idle))
            {
                timer1.Enabled = true;
                mRecordButton.Text = Localizer.Message("pause");
                Audio.AudioRecorder.Instance.StartRecording(mRecordAsset);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (Audio.AudioRecorder.Instance.State == Audio.AudioRecorderState.Recording ||
                Audio.AudioRecorder.Instance.State == Audio.AudioRecorderState.Listening)
            {
                Audio.AudioRecorder.Instance.StopRecording();
            }
            Audio.AudioRecorder.Instance.VuMeterObject.CloseVuMeterForm();
            timer1.Enabled = false;
            this.Close();
        }





        private void timer1_Tick(object sender, EventArgs e)
        {
            double dMiliSeconds = Audio.AudioRecorder.Instance.CurrentTime;
            int Seconds = Convert.ToInt32(dMiliSeconds / 1000);
            string sSeconds;
            if (Seconds > 9)
                sSeconds = Seconds.ToString();
            else
                sSeconds = "0" + Seconds.ToString();

            int Minutes = Convert.ToInt32(Seconds / 60);
            string sMinutes;
            if (Minutes > 9)
                sMinutes = Minutes.ToString();
            else
                sMinutes = "0" + Minutes.ToString();


            int Hours = Minutes / 60;
            string sHours;
            if (Hours > 9)
                sHours = Hours.ToString();
            else
                sHours = "0" + Hours.ToString();



            string sDisplayTime = sHours + ":" + sMinutes + ":" + sSeconds;
            mTimeTextBox.Text = sDisplayTime;

        }
    }
}