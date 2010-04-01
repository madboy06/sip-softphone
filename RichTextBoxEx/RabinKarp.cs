// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RichTextBoxEx
{
	class RabinKarp<T, U>
		where T : class
		where U : class
	{
		private const int size = 4;
		private Dictionary<uint, U> substrings = new Dictionary<uint, U>();
		private T[] starts = new T[size];
		private T[] ends = new T[size];
		private int pointer = 0;
		private uint key;

		public int EndSteps
		{
			get
			{
				return size - 2;
			}
		}

		public int MaxLength
		{
			get
			{
				return size;
			}
		}

		public void Add(string subsctring, U data)
		{
			if (subsctring.Length >= 2 && subsctring.Length <= size)
				substrings.Add(GetKey(subsctring), data);
		}

		public U Get(string subsctring)
		{
			U data = null;

			if (subsctring.Length >= 2 && subsctring.Length <= size)
				substrings.TryGetValue(GetKey(subsctring), out data);

			return data;
		}

		public void Reset()
		{
			key = 0;

			for (int i = 1; i < starts.Length; i++)
			{
				starts[i - 1] = starts[i];
				ends[i - 1] = ends[i];
			}
		}

		public bool Step(char simbol, T simbolStart, T simbolEnd, out U data, out T start, out T end)
		{
			key <<= 8;
			key |= (uint)simbol & 0xff;

			starts[pointer] = simbolStart;
			ends[pointer] = simbolEnd;

			pointer = (pointer + 1) % size;

			data = null;
			start = end = null;

			for (int i = 0; i < 3; i++)
			{
				if (substrings.TryGetValue(key & (0xffffffff << i * 8), out data))
				{
					key &= ~(0xffffffff << i * 8);

					start = starts[pointer % size];
					end = ends[(pointer + size - i - 1) % size];

					break;
				}
			}

			return data != null;
		}

		public bool Step(out U data, out T start, out T end)
		{
			return Step(' ', null, null, out data, out start, out end);
		}

		private static uint GetKey(string substring)
		{
			uint key = 0;

			for (int i = 0; i < size; i++)
			{
				key <<= 8;
				if (i < substring.Length)
					key |= substring[i];
			}

			return key;
		}
	}
}
