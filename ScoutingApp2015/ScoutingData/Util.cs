﻿using ScoutingData.Analysis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace ScoutingData
{
	public enum LogLevel
	{
		Info,
		Warning,
		Error,
		Critical
	}

	/// <summary>
	/// Utility class containing various functions with no other purpose.
	/// </summary>
	public static class Util
	{
		public static readonly string USERPROFILE =
			Environment.GetEnvironmentVariable("USERPROFILE");

		internal static readonly IFormatProvider DEF_FORMAT = 
			CultureInfo.CurrentCulture.NumberFormat;

		/// <summary>
		/// Length of time for an FRC 2015 match, in the form of a TimeSpan object
		/// </summary>
		public static readonly TimeSpan MATCH_LENGTH = new TimeSpan(0, 1, 30);
		/// <summary>
		/// Time left when teleop starts
		/// </summary>
		public static readonly TimeSpan TELEOP = new TimeSpan(0, 1, 15);

		/// <summary>
		/// Subscribe to add an additional output location for Util.DebugLog().
		/// </summary>
		public static event Action<int, string> OnPrint;/// <summary>
		/// Clamps a value between a min and max (inclusively), and returns if clamping was necessary.
		/// </summary>
		/// <typeparam name="T">Type being compared when clamping. Must extend IComparable</typeparam>
		/// <param name="val">Value to clamp</param>
		/// <param name="min">Minimum</param>
		/// <param name="max">Maximum</param>
		/// <returns></returns>
		public static bool Clamp<T>(T val, T min, T max) where T : IComparable<T>
		{
			bool needed = false;

			if (val.CompareTo(min) < 0)
			{
				needed = true;
				val = min;
			}
			if (val.CompareTo(max) > 0)
			{
				needed = true;
				val = max;
			}

			return needed;
		}

		/// <summary>
		/// Shortcut for Debug Logging
		/// </summary>
		/// <param name="level">Seriousness of message</param>
		/// <param name="message">Message logged</param>
		public static void DebugLog(LogLevel level, string message)
		{
			string output = "\n[" + level.ToString().ToUpper() + "] " + message;
			System.Diagnostics.Debugger.Log((int)level, "SCOUTING", output);
			OnPrint((int)level, output);
		}
		/// <summary>
		/// Shortcut for Debug Logging, with category
		/// </summary>
		/// <param name="level">Seriousness of message</param>
		/// <param name="category">Section of app with log</param>
		/// <param name="message">Message logged</param>
		public static void DebugLog(LogLevel level, string category, string message)
		{
			DebugLog(level, "[" + category + "] " + message);
		}

		///////////////////////
		// Extension Methods //
		///////////////////////

		/// <summary>
		/// Extension method, converting timespans into seconds left, for convenience
		/// </summary>
		/// <param name="ts">Object to convert</param>
		/// <returns>Total seconds left in the timespan</returns>
		public static int CountedSeconds(this TimeSpan ts)
		{
			return (3600 * ts.Hours) + (60 * ts.Minutes) + ts.Seconds;
		}

		/// <summary>
		/// Adds a value to the dictionary, overwriting if the key is already set
		/// </summary>
		/// <typeparam name="TKey">Key type in the Dictionary</typeparam>
		/// <typeparam name="TVal">Value type in the Dictionary</typeparam>
		/// <param name="dict">IDictionary to add stuff to</param>
		/// <param name="key">Key to set or add</param>
		/// <param name="val">Value the key is set to</param>
		public static void AddSet<TKey, TVal>(this IDictionary<TKey, TVal> dict, TKey key, TVal val)
		{
			if (dict.ContainsKey(key))
			{
				dict[key] = val;
			}
			else
			{
				dict.Add(key, val);
			}
		}

		/// <summary>
		/// Adds a value to the dictionary, incrementing by an amount if the key is already set
		/// </summary>
		/// <typeparam name="TKey">Key type of dictionary</typeparam>
		/// <typeparam name="TVal">Value type of dictionary. Must be numeric.</typeparam>
		/// <param name="dict">Dictionary to do this with</param>
		/// <param name="key">Key for whose value to increment</param>
		/// <param name="incVal">Amount to increment by</param>
		/// <param name="startVal">Value to start at when adding new pairs</param>
		public static void AddIncrement<TKey, TVal>(this IDictionary<TKey, TVal> dict, 
			TKey key, double incVal, double startVal) where TVal : IConvertible, new()
		{
			if (dict.ContainsKey(key))
			{
				// this is how you increment generic types
				IConvertible conv = dict[key].ToDouble(DEF_FORMAT) + 1.0;
				dict[key] = (TVal)conv.ToType(typeof(TVal), DEF_FORMAT);
			}
			else
			{
				// this is how you instantiate generic types (that implement IConvertible)
				IConvertible start = (IConvertible)startVal;
				dict.Add(key, (TVal)start.ToType(typeof(TVal), DEF_FORMAT));
			}
		}

		/// <summary>
		/// Adds a value to the dictionary with a key of zero, incrementing by one if the 
		/// key is already set
		/// </summary>
		/// <typeparam name="TKey">Key type of dictionary</typeparam>
		/// <typeparam name="TVal">Value type of dictionary. Must be numeric.</typeparam>
		/// <param name="dict">Dictionary to do this with</param>
		/// <param name="key">Key for whose value to increment</param>
		public static void AddIncrement<TKey, TVal>(this IDictionary<TKey, TVal> dict, TKey key)
			where TVal : IConvertible, new()
		{
			dict.AddIncrement(key, 1, 0);
		}/// <summary>
		/// Quick function for converting numbers from 0 to 1 into percentages
		/// </summary>
		/// <param name="n">number to convert</param>
		/// <returns>percentage string</returns>
		public static string ToStringPct(this double n)
		{
			return (n * 100).ToString() + "%";
		}
	}
}
