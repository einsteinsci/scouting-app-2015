﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace ScoutingData.Analysis
{
	/// <summary>
	/// Display Mode for Distributions.
	/// </summary>
	[Obsolete]
	public enum DistributionDisplayMode
	{
		[Description("Mean & SD")]
		MeanSD,
		//[Description("Five-number Summary")]
		//FiveNumSummary
	}

	/// <summary>
	/// Wrapper class for Normal Model
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public class Distribution
	{
		//// <summary>
		//// Preferred display mode as set by the dev. Defaults to Mean & SD
		//// </summary>
		//[JsonIgnore]
		//public static DistributionDisplayMode DisplayMode = DistributionDisplayMode.MeanSD;

		public bool FakeNull
		{ get; private set; }

		/// <summary>
		/// Shortcut to the model's mean.
		/// </summary>
		public double Mean
		{
			get
			{
				return FakeNull ? -1 : Model.Mean;
			}
		}
		
		/// <summary>
		/// Shortcut to the model's SD.
		/// </summary>
		public double SD
		{
			get
			{
				return FakeNull ? -1 : Model.SD;
			}
		}

		/// <summary>
		/// Normal Model of this distribution
		/// </summary>
		[JsonProperty]
		public NormalModel Model
		{ get; set; }

		/// <summary>
		/// Z-score of model's mean compared to other distributions
		/// </summary>
		[JsonProperty]
		public double CenterZScore
		{ get; set; }

		/// <summary>
		/// Creates a new instance of Distribution
		/// </summary>
		/// <param name="norm">Normal Model</param>
		/// <param name="fiveNum">Five-Number Summary</param>
		public Distribution(NormalModel norm)
		{
			Model = norm;
		}
		public Distribution(double mean, double sd) : 
			this(new NormalModel(mean, sd))
		{ }
		public Distribution(bool makeNull)
		{
			FakeNull = makeNull;

			Model = new NormalModel(-1, 0.01);
			CenterZScore = -99;
		}

		/// <summary>
		/// Json-friendly constructor
		/// </summary>
		public Distribution() : this(true)
		{ }

		/// <summary>
		/// Calculates Z-score of center based on other data
		/// </summary>
		/// <param name="all">Other data to compare to</param>
		public void CalculateZ(IEnumerable<Distribution> all)
		{
			IEnumerable<double> means = from d in all
										select d.Model.Mean;

			Distribution bigBoy = means.ToList().MakeDistribution();
			CenterZScore = bigBoy.Model.ZScore(Model.Mean);
		}

		/// <summary>
		/// Summarizes the distribution in the form of a string
		/// </summary>
		/// <returns>String based on the preferred display mode for distributions</returns>
		public override string ToString()
		{
			if (FakeNull)
			{
				return "NULL";
			}

			return Model.ToString() + " [z = " + CenterZScore.ToString() + "]";
		}
	}
}
