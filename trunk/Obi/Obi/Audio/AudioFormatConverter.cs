using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
//using AudioLib;
using AudioLib;
using urakawa;
using urakawa.data;

namespace Obi.Audio
{
    //sdk2
    public class AudioFormatConverter
    {
        private static bool m_IsRequestCancellation;
        public static bool IsRequestCancellation
        {
            get { return m_IsRequestCancellation; }
            set { m_IsRequestCancellation = value; }
        }

        public static string[] ConvertFiles(string[] fileName, Presentation presentation)
        {
            m_IsRequestCancellation = false;
            int numberOfFiles = fileName.Length;
            string convertedFile = null;
            List<string> listOfConvertedFiles = new List<string>();


            for (int i = 0; i < numberOfFiles; i++)
            {
                if (IsRequestCancellation) return null;
                convertedFile = ConvertedFile(fileName[i], presentation);
                if (convertedFile != null) listOfConvertedFiles.Add(convertedFile);
            }
            string[] returnArray = new string[listOfConvertedFiles.Count];
            for (int i = 0; i < listOfConvertedFiles.Count; i++) returnArray[i] = listOfConvertedFiles[i];

            return returnArray;
        }
        public static string ConvertedFile(string filePath, Presentation pres)
        {
            AudioLib.WavFormatConverter audioConverter = new WavFormatConverter(true, true);
            int samplingRate = (int)pres.MediaDataManager.DefaultPCMFormat.Data.SampleRate;
            int channels = pres.MediaDataManager.DefaultPCMFormat.Data.NumberOfChannels;
            int bitDepth = pres.MediaDataManager.DefaultPCMFormat.Data.BitDepth;
            string directoryPath = pres.DataProviderManager.DataFileDirectoryFullPath;
            string convertedFile = null;
            try
            {
                if (Path.GetExtension(filePath).ToLower() == ".wav")
                {
                    Stream wavStream = null;
                    wavStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    uint dataLength;
                    AudioLibPCMFormat newFilePCMInfo = AudioLibPCMFormat.RiffHeaderParse(wavStream, out dataLength);
                    if (wavStream != null) wavStream.Close();
                    if (newFilePCMInfo.SampleRate == samplingRate && newFilePCMInfo.NumberOfChannels == channels && newFilePCMInfo.BitDepth == bitDepth)
                    {
                        convertedFile = filePath;
                    }
                    else
                    {
                        AudioLibPCMFormat pcmFormat = new AudioLibPCMFormat((ushort)channels, (uint)samplingRate, (ushort)bitDepth);
                        convertedFile = audioConverter.ConvertSampleRate(filePath, directoryPath, pcmFormat);
                    }
                }
                else if (Path.GetExtension(filePath).ToLower() == ".mp3")
                {
                    AudioLibPCMFormat pcmFormat = new AudioLibPCMFormat((ushort)channels, (uint)samplingRate, (ushort)bitDepth);
                    convertedFile = audioConverter.UnCompressMp3File(filePath, directoryPath, pcmFormat);
                }
                else
                {
                    MessageBox.Show(string.Format(Localizer.Message("AudioFormatConverter_Error_FileExtentionNodSupported"), filePath), Localizer.Message("Caption_Error"));
                    return null;
                }
                // rename converted file to original file if names are different
                if (Path.GetFileName(filePath) != Path.GetFileName(convertedFile))
                {
                    string newConvertedFilePath = Path.Combine(Path.GetDirectoryName(convertedFile), Path.GetFileNameWithoutExtension(filePath) + ".wav");
                    for (int i = 0; i < 99999 && File.Exists(newConvertedFilePath); i++)
                    {
                        newConvertedFilePath = Path.Combine(Path.GetDirectoryName(convertedFile), i.ToString() + Path.GetFileNameWithoutExtension(filePath) + ".wav");
                        if (!File.Exists(newConvertedFilePath))
                        {
                            MessageBox.Show(string.Format(Localizer.Message("Import_AudioFormat_RenameFile"), Path.GetFileNameWithoutExtension(filePath) + ".wav", Path.GetFileName(newConvertedFilePath)),
                                Localizer.Message("Caption_Information"),
                                MessageBoxButtons.OK);
                            break;
                        }
                    }
                    File.Move(convertedFile, newConvertedFilePath);
                    convertedFile = newConvertedFilePath;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
            return convertedFile;
        }

        private static System.ComponentModel.BackgroundWorker m_SpeechWorker = null;
        private static Settings m_Settings;
        private static AudioLib.TextToSpeech m_Tts;

        public static void  Speak( string    text, string filePath, Settings settings)
        {
            if (m_Tts != null && m_Tts.Synthesizer.State == System.Speech.Synthesis.SynthesizerState.Speaking )
            {
                if (!string.IsNullOrEmpty(filePath))
                    return;
                else
                m_Tts.Synthesizer.Pause();
            }
            m_Settings = settings;
            if (m_SpeechWorker != null) m_SpeechWorker.DoWork -= new System.ComponentModel.DoWorkEventHandler(m_SpeechWorker_DoWork); 
            m_SpeechWorker = new System.ComponentModel.BackgroundWorker();

            AudioLib.AudioLibPCMFormat audioFormat = new AudioLibPCMFormat((ushort)m_Settings.AudioChannels, (uint)m_Settings.SampleRate, (ushort)m_Settings.BitDepth);
            m_Tts = new TextToSpeech(audioFormat, null);
            m_SpeechWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(m_SpeechWorker_DoWork);

            List<string> inputStrings = new List<string>();
            inputStrings.Add(text);
            inputStrings.Add(filePath);
            m_SpeechWorker.RunWorkerAsync(inputStrings);

        }

        static void m_SpeechWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            
            foreach (string s in m_Tts.InstalledVoices) Console.WriteLine(s);
            if (m_Tts.InstalledVoices.Count == 0) return;
            List<string> inputStrings = (List<string>)e.Argument;
            m_Tts.SpeakString(m_Tts.InstalledVoices[0], inputStrings[0], inputStrings[1]);
            
        }


    }
}
