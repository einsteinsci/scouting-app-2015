﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace ScoutingData.Data
{
	/// <summary>
	/// Color of Alliance. -1 is indeterminate.
	/// </summary>
	public enum AllianceColor
	{
		Indeterminate = -1,
		Red = 1,
		Blue
	}

	/// <summary>
	/// Position within alliance (for indexing). A, B, or C.
	/// </summary>
	public enum AlliancePosition
	{
		A,
		B,
		C
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class AllianceGroup<T> : IEnumerable<T>
	{
		[JsonIgnore]
		public T this[AlliancePosition pos]
		{
			get
			{
				switch (pos)
				{
				case AlliancePosition.A:
					return A;
				case AlliancePosition.B:
					return B;
				case AlliancePosition.C:
					return C;
				default:
					throw new IndexOutOfRangeException("Invalid position: " + pos.ToString());
				}
			}
		}

		[JsonProperty]
		public AllianceColor Color
		{ get; set; }

		[JsonProperty]
		public virtual T A
		{ get; set; }

		[JsonProperty]
		public virtual T B
		{ get; set; }

		[JsonProperty]
		public virtual T C
		{ get; set; }

		public AllianceGroup(T a, T b, T c)
		{
			A = a;
			B = b;
			C = c;
		}

		public AlliancePosition GetPositionOf(T item)
		{
			if (A.Equals(item)) // apparently == isn't valid for comparing types 'T' and 'T'
			{					// why doesn't it default to object.operator==() ?
				return AlliancePosition.A;
			}
			else if (B.Equals(item))
			{
				return AlliancePosition.B;
			}
			else if (C.Equals(item))
			{
				return AlliancePosition.C;
			}
			else
			{
				return (AlliancePosition)(-1);
			}
		}

		public List<T> ToList()
		{
			List<T> res = new List<T>();
			res.Add(A);
			res.Add(B);
			res.Add(C);

			return res;
		}

		#region Interfaces

		public IEnumerator<T> GetEnumerator()
		{
			return new AllianceEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		internal class AllianceEnumerator : IEnumerator<T>
		{
			int position;
			AllianceGroup<T> alliance;

			public AllianceEnumerator(AllianceGroup<T> al)
			{
				alliance = al;
				position = 0;
			}

			public T Current
			{
				get
				{
					switch (position)
					{
					case 0:
						return alliance.A;
					case 1:
						return alliance.B;
					case 2:
						return alliance.C;
					default:
						return alliance.A;
					}
				}
			}

			public void Dispose()
			{ }

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public bool MoveNext()
			{
				position++;

				if (position > 2)
				{
					position = 2;
					return false;
				}

				return true;
			}

			public void Reset()
			{
				position = 0;
			}
		}

		#endregion
	}
}
