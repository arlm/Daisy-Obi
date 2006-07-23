using System;
using System.IO;
using System.Windows.Forms;
using System.Collections;

namespace VirtualAudioBackend
{
	/// <summary>
	/// Audio clips.
	/// </summary>
	public class AudioClip
	{

// Member variables
private string m_sPath ;
		private double m_dBeginTime ;
		private double m_dEndTime ;

private int m_ClipChannels ;
private int m_ClipBitDepth ;
private int m_ClipSamplingRate ;
private int m_ClipFrameSize ;
		private double m_dLengthInTime ;

private long m_lBeginByte ;
		private long m_lEndByte;
private long m_lLengthInBytes ;

private long m_lFileAudioLengthInBytes ;
		private double m_dFileAudioLengthInTime  ;

internal static  Hashtable static_htClipExists = new Hashtable () ;
		internal static long static_lNameCount ;
private string m_sName ;
// finalise as internal instead of public
		public string Name
		{
			get
			{
return m_sName ;
			}
		}
		/// <summary>
		/// The path of the audio file that this clip is part of.
		/// </summary>
		public string Path
		{
			get
			{
				return m_sPath ;
			}
		}

		/// <summary>
		/// Begin time of the clip in milliseconds.
		/// </summary>
		public double BeginTime
		{
			get
			{
				return m_dBeginTime ;
			}
		}

		/// <summary>
		/// End time of the clip in milliseconds.
		/// </summary>
		public double EndTime
		{
			get
			{
				return m_dEndTime ;
			}
		}

		internal long BeginByte
		{
			get
			{
return m_lBeginByte ;
			}
		}

		internal long EndByte
		{
			get
			{
return m_lEndByte ;
			}
		}

		internal long LengthInBytes
		{
			get
			{
return m_lLengthInBytes ;
			}
		}

		public int Channels
		{
			get
			{
return m_ClipChannels ;
			}
		}

		public int BitDepth
		{
			get
			{
return m_ClipBitDepth;
			}
		}

		public int SampleRate
		{
			get
			{
return m_ClipSamplingRate ;
			}
		}

		internal int FrameSize
		{
			get
			{
return m_ClipFrameSize ;
			}
		}

		internal double LengthInTime
		{
			get
			{
return m_dLengthInTime ;
			}
		}

CalculationFunctions Calc = new CalculationFunctions () ;
		/// <summary>
		/// Create a new AudioClip object from an existing audio file.
		/// </summary>
		/// <param name="path">Path of the audio file.</param>
		/// <param name="beginTime">Begin time of the clip in milliseconds.</param>
		/// <param name="endTime">End time of the clip in milliseconds.</param>
		public AudioClip(string path, double beginTime, double endTime)
		{

			Init (path) ;
			if (beginTime < 0 || endTime > m_dFileAudioLengthInTime )
			{
				throw new Exception ("time parameters of clip are out of bound of file size") ;
			}
			else if (beginTime >= endTime)
			{
throw new Exception ("BeginTime  more than EndTime of clip ") ;
			}
			else
			{
				m_sPath = path ;
				m_dBeginTime = beginTime ;
				m_dEndTime = endTime ;
				m_dLengthInTime  = m_dEndTime  - m_dBeginTime ;


				m_lBeginByte = Calc.ConvertTimeToByte (m_dBeginTime , m_ClipSamplingRate , m_ClipFrameSize) ;
				m_lBeginByte  = Calc.AdaptToFrame (m_lBeginByte  , m_ClipFrameSize) ;

				m_lEndByte = Calc.ConvertTimeToByte (m_dEndTime , m_ClipSamplingRate , m_ClipFrameSize) ;
				m_lEndByte  = Calc.AdaptToFrame (m_lEndByte  , m_ClipFrameSize) ;

				m_lLengthInBytes = m_lEndByte - m_lBeginByte ;

m_sName =GenerateClipName () ;
static_htClipExists.Add (m_sName , this) ;
			}
		}

		public AudioClip(string path )
		{

			Init (path) ;
			if (m_dFileAudioLengthInTime <= 0)
			{
				throw new Exception ("File has no audio data") ;
			}
			
			else
			{
				m_sPath = path ;
				m_dBeginTime = 0;
				m_dEndTime = m_dFileAudioLengthInTime ;
				m_dLengthInTime  = m_dFileAudioLengthInTime ;


				m_lBeginByte = Calc.ConvertTimeToByte (m_dBeginTime , m_ClipSamplingRate , m_ClipFrameSize) ;
				m_lBeginByte  = Calc.AdaptToFrame (m_lBeginByte  , m_ClipFrameSize) ;

				m_lEndByte = Calc.ConvertTimeToByte (m_dEndTime , m_ClipSamplingRate , m_ClipFrameSize) ;
				m_lEndByte  = Calc.AdaptToFrame (m_lEndByte  , m_ClipFrameSize) ;

				m_lLengthInBytes = m_lEndByte - m_lBeginByte ;

				m_sName =GenerateClipName () ;
				static_htClipExists.Add (m_sName , this) ;

			}
		}

		/// <summary>
		/// Make a copy of the audio clip.
		/// </summary>
		/// <returns>A copy of the clip.</returns>
		public AudioClip Copy()
		{
			AudioClip  ob_AudioClip  = new AudioClip (this.Path , this.BeginTime , this.EndTime) ;
			
			return ob_AudioClip  ;
		}

		/// <summary>
		/// Split an audio clip at the given time (in millisecond.)
		/// </summary>
		/// <param name="time"></param>
		/// <returns>The new clip (second half); the first clip has been modified.</returns>
		public AudioClip Split(double time)
		{
			time = m_dBeginTime + time ;
			AudioClip ob_AudioClip = new AudioClip ( this.Path , time, this.EndTime) ;
m_dEndTime = time ;

			return ob_AudioClip ;
		}

		/// <summary>
		/// Merge two audio clips. The end time of this clip must match the begin time of the next, and of course the files must match.
		/// </summary>
		/// <param name="next">The next clip to merge with.</param>
		public void MergeWith(AudioClip next)
		{
			if (m_sPath == next.Path  && m_ClipChannels == next.Channels && m_ClipBitDepth == next.BitDepth && m_ClipSamplingRate == next.SampleRate&& 																m_dEndTime <= next.BeginTime )
																								{
m_dEndTime = next.EndTime ;
m_dLengthInTime  = m_dEndTime - m_dBeginTime ;
																								}
			else
throw new Exception ("Clips of different formats cannot be merged") ;				
		}

// takes relative time as parameters
		internal AudioClip CopyClipPart (double BeginTime , double EndTime)
		{
			BeginTime =m_dBeginTime + BeginTime ;
EndTime = m_dBeginTime + EndTime ;


		// temp checks are as follows
/*
if (BeginTime < this.BeginTime ) 
MessageBox.Show ("error begin time") ;

if (EndTime > this.EndTime  )
			MessageBox.Show ("error in end time") ;

if (BeginTime>= EndTime)
MessageBox.Show ("both") ;
*/

			if (BeginTime >= this.BeginTime && EndTime <= this.EndTime  && BeginTime< EndTime)
			{
				AudioClip ob_AudioClip = new AudioClip ( this.Path ,BeginTime , EndTime ) ;
				return ob_AudioClip ;
			}
			else
			throw new Exception ("Partial clip cannot be created: Invalid time parameters") ;
		}

		void Init (string sPath)
		{

			//declare   array variable of size 4 as the max chunk in header is 4 bytes long
			int [] Ar = new int[4] ;
			Ar [0] = Ar [1] = Ar[2] = Ar [3] = 0 ;
			BinaryReader br = new  BinaryReader (File.OpenRead(sPath)) ;
						

			
			// channels
			Ar [0] = Ar [1] = Ar[2] = Ar [3] = 0 ;

			br.BaseStream.Position = 22 ;
			for (int i= 0 ; i<2 ; i++)
			{
				Ar [i] = br.ReadByte() ;
			}

			m_ClipChannels = Convert.ToInt32 (ConvertToDecimal (Ar)) ;

			// Sampling rate
			Ar [0] = Ar [1] = Ar[2] = Ar [3] = 0 ;

			br.BaseStream.Position = 24 ;
			for (int i= 0 ; i<4 ; i++)
			{
				Ar [i] = br.ReadByte() ;
			}

			m_ClipSamplingRate = Convert.ToInt32 (ConvertToDecimal (Ar)) ;
		
			//Frame size
			Ar [0] = Ar [1] = Ar[2] = Ar [3] = 0 ;

			br.BaseStream.Position = 32 ;
			for (int i= 0 ; i<2 ; i++)
			{
				Ar [i] = br.ReadByte() ;
			}

			m_ClipFrameSize  = Convert.ToInt32 (ConvertToDecimal (Ar)) ;

			//bit depth
			Ar [0] = Ar [1] = Ar[2] = Ar [3] = 0 ;

			br.BaseStream.Position = 34 ;
			for (int i= 0 ; i<2 ; i++)
			{
				Ar [i] = br.ReadByte() ;
			}

			m_ClipBitDepth = Convert.ToInt32 (ConvertToDecimal (Ar)) ;

			// AudioLengthBytes

			br.BaseStream.Position = 40 ;

			for (int i = 0; i<4 ; i++)
			{
				Ar [i] = Convert.ToInt32(br.ReadByte() );

			}
			m_lFileAudioLengthInBytes= ConvertToDecimal (Ar) ;
			m_dFileAudioLengthInTime = Calc.ConvertByteToTime (m_lFileAudioLengthInBytes, m_ClipSamplingRate , m_ClipFrameSize) -10;



/*
			//size in time
			if (m_AudioLengthBytes  != 0)
				m_LengthTime = Convert.ToDouble ((m_AudioLengthBytes * 1000)/ (m_SamplingRate * m_FrameSize));
			else
				m_LengthTime  =  0 ;
*/

			br.Close() ;
		}

		internal long ConvertToDecimal (int [] Ar)
		{
			//convert from mod 256 to mod 10
			return Ar[0] + (Ar[1] * 256) + (Ar[2] *256 *256) + (Ar[3] *256 *256 *256) ;
		}



		internal ArrayList DetectPhrases ( long SilVal, long PhraseLength , long BeforePhrase) 
		{

			// adapt values to frame size
			PhraseLength = Calc.AdaptToFrame (PhraseLength, this.FrameSize) ;
			BeforePhrase = Calc.AdaptToFrame (BeforePhrase, this.FrameSize) ;

			// Block size of audio chunck which is least count of detection
			int Block ;

			// determine the Block  size
			if (this.SampleRate >22500)
			{
				Block = 96 ;
			}
			else
			{
				Block = 48 ;
			}


			// Detection starts here

			BinaryReader br = new BinaryReader (File.OpenRead (this.Path)) ;		
			


			// Gets the count of file size
			long lSize = this.EndByte + 44;


			br.BaseStream.Position = 44 + this.BeginByte;

			// count chunck of silence which trigger phrase detection
			long  lCountSilGap = PhraseLength / Block;
			long lSum = 0 ;
			ArrayList alPhrases = new ArrayList () ;
			long lCheck= 0 ;

			// adjustment to prevent end of file exception
			lSize = ((lSize / Block) * Block) - 4;

// flags to indicate phrases and silence
bool boolPhraseDetected = false ;
bool boolBeginPhraseDetected = false ;

			// scanning of file starts
			for (long j =  this.BeginByte/ Block ; j< (lSize / Block); j++)
			{
				// decodes audio chunck inside block
				lSum = BlockSum (br, (j*Block) + 44, Block, this.FrameSize, this.Channels) ;

				// conditional triggering of phrase detection
				if (lSum < SilVal)
				{
					lCheck ++ ;
					//lHighAmpCheck = 0 ;  
				}
				else
				{				
					if ( (j- (this.BeginByte/Block )) < lCountSilGap&& boolBeginPhraseDetected == false)
					{
boolBeginPhraseDetected  = true ;
						alPhrases.Add( Convert.ToInt64 (0)) ;
						boolPhraseDetected = true ;
												lCheck = 0 ;
						//lHighAmpCheck = 0 ;
					}

					// checks the length of silence
					if (lCheck > lCountSilGap)
					{
						//sets the detection flag
						boolPhraseDetected = true ;

							alPhrases.Add((j * Block) - BeforePhrase- this.BeginByte) ;
						
						lCheck = 0 ;
						//lHighAmpCheck = 0 ;
					}
				}

				// end outer For
			}

			br.Close () ;

double dBeginTime ;
double dEndTime ;

ArrayList alClipList = new ArrayList () ;

alClipList.Add (boolPhraseDetected) ;


			if (boolPhraseDetected == false)
			{
				alClipList.Add (this) ;
				

			}
			else
			{

				for (int i = 0 ; i < alPhrases.Count- 1 ; i++)
				{
dBeginTime = Calc.ConvertByteToTime (Convert.ToInt64(alPhrases [i]) , m_ClipSamplingRate , m_ClipFrameSize );
dEndTime = Calc.ConvertByteToTime (Convert.ToInt64(alPhrases [i+1]) , m_ClipSamplingRate , m_ClipFrameSize );
					
alClipList.Add (CopyClipPart (dBeginTime , dEndTime) );
				}
				dBeginTime = Calc.ConvertByteToTime (Convert.ToInt64(alPhrases [alPhrases.Count - 1]) , m_ClipSamplingRate , m_ClipFrameSize );
alClipList.Add (CopyClipPart ( dBeginTime , m_dEndTime) );
			}

			return alClipList ;
			
		}

		public long GetClipSilenceAmplitude()
		{
			


			BinaryReader brRef = new BinaryReader (File.OpenRead (this.Path  )) ;		
			


			// creates counter of size equal to clip size
			long lSize = this.EndByte + 44 ;

			// Block size of audio chunck which is least count of detection
			int Block ;

			// determine the Block  size
			if (this.SampleRate >22500)
			{
				Block = 96 ;
			}
			else
			{
				Block = 48 ;
			}

			//set reading position after the header
			brRef.BaseStream.Position = 44 + this.BeginByte;
			long lLargest = 0 ;
			long lBlockSum ;			

			// adjust the  lSize to avoid reading beyond file length
			lSize = ((lSize / Block)*Block)-4;

			// loop to end of file reading collective value of  samples in Block and determine highest value denoted by lLargest
			// Step size is the Block size
			for (long j = 44 + this.BeginByte;j < (lSize ); j = j + Block)
			{
				//  BlockSum is function to retrieve average amplitude in  Block
				lBlockSum = BlockSum(brRef , j , Block, this.FrameSize, this.Channels) ;	
				if (lLargest < lBlockSum)
				{
					lLargest = lBlockSum ;
				}
			}
			long SilVal = Convert.ToInt64(lLargest );
			
			brRef.Close () ;

			return SilVal ;
			
		}


		
		// function to compute the amplitude of a small chunck of samples

		long BlockSum (BinaryReader br,long Pos, int Block, int FrameSize, int 
			Channels) 
		{
			long sum = 0;
			long SubSum ;
			for (int i = 0 ; i< Block ; i = i + FrameSize)
			{
				br.BaseStream.Position = i+ Pos ;
				SubSum = 0 ;
				if (FrameSize == 1)
				{
					SubSum = Convert.ToInt64((br.ReadByte ()) );
					
					// FrameSize 1 ends
				}
				else if (FrameSize == 2)
				{
					if (Channels == 1)
					{
						SubSum = Convert.ToInt64(br.ReadByte() )  ;
						SubSum = SubSum + (Convert.ToInt64(br.ReadByte() ) * 256 );						SubSum = (SubSum * 256)/65792 ;
					}
					else if (Channels == 2)
					{
						SubSum = Convert.ToInt64(br.ReadByte() )  ;
						SubSum = SubSum + Convert.ToInt64(br.ReadByte() )  ;SubSum = SubSum/2 ;
					}
					// FrameSize 2 ends
				}
				else if (FrameSize == 4)
				{
					if (Channels == 1)
					{
						SubSum = Convert.ToInt64(br.ReadByte() )  ;
						SubSum = SubSum + 
							(Convert.ToInt64(br.ReadByte() ) * 256)  ;
						SubSum = SubSum + 
							(Convert.ToInt64(br.ReadByte() ) * 256 * 256)  ;
						SubSum = SubSum + 
							(Convert.ToInt64(br.ReadByte() ) * 256 * 256 * 256)  ;
					}
					else if (Channels == 2)
					{
						SubSum = Convert.ToInt64(br.ReadByte() )  ;
						
						SubSum = SubSum + (Convert.ToInt64(br.ReadByte() ) * 256)  ;
							
						// second channel
						SubSum = SubSum + Convert.ToInt64(br.ReadByte() )  ;												SubSum = SubSum + (Convert.ToInt64(br.ReadByte() ) * 256)  ;						
						SubSum = (SubSum * 256 ) / (65792  * 2);
						
					}
					// FrameSize 4 ends
				}
				sum = sum + SubSum ;
				

				// Outer, For ends
			}
			
			
			
			sum = sum / (Block / FrameSize) ;

			//MessageBox.Show(sum.ToString()) ;
			return sum ;
		}

		internal string GenerateClipName ()
		{
			if (static_lNameCount.Equals  (null ) )
			{
				long i = 0 ;

				string sTempName ;
				sTempName = i.ToString () ;

				while ( static_htClipExists.ContainsKey (sTempName)  && i<9000000)
				{

					i++;
					sTempName = i.ToString () ;
				}

				if (i<90000000)
				{
					static_lNameCount = i ;
					return sTempName ;
				}
				else
				{
					return null ;
				}
			}
			else
			{
static_lNameCount++ ;
				return static_lNameCount.ToString () ;
			}

		}

		internal void DeletePhysicalResource ()
		{
string sClipPath  ;
			sClipPath = m_sPath ;
m_sPath  = "  " ;
			IDictionaryEnumerator en = static_htClipExists.GetEnumerator ();

			long lCount = 0 ;
AudioClip ob_Clip ;
			while (en.MoveNext ())
			{
ob_Clip = en.Value as AudioClip ;
				if (sClipPath == ob_Clip.Path.ToString () )
					lCount++ ;
					
			}
			if (lCount == 0)
			{
				
				FileInfo ob_File = new FileInfo (sClipPath ) ;
				try
				{
					ob_File.Delete () ;
				}
				catch ( Exception Ex)
				{
MessageBox.Show (Ex.ToString ()	 ) ;
				}

			}
		}

	}// end of class
}