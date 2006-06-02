using System;
using System.Collections;
using System.Text;
using System.Windows.Forms;

namespace UrakawaApplicationBackend
{
    public interface IVuMeter
    {
		/// <summary>
		/// Number of channels (1 or 2)
		/// </summary>
		int Channels
		{
			get;
			set;
		}

		/// <summary>
		/// Set the incoming buffer of bytes to analyze and render. Choose a better type than object.
		/// </summary>
		object Stream
		{
			set;
		}
		
		/// <summary>
		/// Get/set the control that is used to render the VU meter visually.
		/// </summary>
		UserControl VisualControl
		{
			get;
			set;
		}

		/// <summary>
		/// Get/set the control that is used to render the VU meter visually.
		/// </summary>
		UserControl TextControl
		{
			get;
			set;
		}

		/// <summary>
		/// Current peak values from the stream. There is a value for each channel.
		/// </summary>
		double[] PeakValue
		{
			get;
		}

		/// <summary>
		/// Flag that shows if the signal overloaded in each channel.
		/// </summary>
		bool[] Overloaded
		{
			get;
		}


// Properties added by app team, india

		VuMeter.Threshold AmplitudeThreshold
		{
get;
			set;
		}

// provides the scale facor for graph 
		double ScaleFactor
		{
get;
			set ;
		}

		double SampleTimeLength
		{
			get ;
			set ;
		}


        /// <summary>
        /// Resets statistics from the VU meter (peak values and overload.)
        /// </summary>
        void Reset();
		
    }
}
