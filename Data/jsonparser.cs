using System.Globalization;

namespace jsonparser
{
	public static class parser
	{
		public static int arrayLength(string arrayResult)
		{
			int length = 0;
			while (true)
				if (arrayIndex(arrayResult, length) == " ") break;
				else length++;
			return length;
		}
		public static string arrayIndex(string arrayResult, int index)
		{
			string noValue = " ";
			try
			{
				string resultSTR = arrayResult.Trim();
				int count = 0; int laststop = 0;
				while (count <= index)
				{
					int start1 = right(laststop, "{");
					int start2 = right(laststop, "[");
					int start3 = right(laststop == 0 ? -1 : laststop, "\"");
					int start4 = right(laststop, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._");

					int start = getPositiveMin(new List<int>() { start1, start2, start3, start4 });
					if (start != -1)
					{
						if (start == start1)
						{
							laststop = rightPM(start, '{');
							if (count == index) return resultSTR[start..laststop].Trim();
						}
						else if (start == start2)
						{
							laststop = rightPM(start, '[');
							if (count == index) return resultSTR[start..laststop].Trim();
						}
						else if (start == start3)
						{
							laststop = rightPM(start, '"');
							if (count == index) return betweenStr(resultSTR[start..laststop], '"');
						}
						else if (start == start4)
						{
							laststop = right(start, ",}]");
							if (laststop == -1) laststop = resultSTR.Length;
							if (count == index) return resultSTR[start..laststop].Trim();
						}
					}
					else return noValue;
					count++;
				}
				return noValue;

				int getPositiveMin(List<int> values)
				{
					values.Sort();
					for (int i = 0; i < values.Count; i++)
						if (values[i] >= 0) return values[i];
					return -1;
				}
				int right(int index, string stop)
				{
					char[] stopchars = stop.ToCharArray();
					int ret = resultSTR.IndexOfAny(stopchars, index + 1);
					return ret;
				}
				int rightPM(int index, char pluschar)
				{
					try
					{
						char minuschar;
						switch (pluschar)
						{
							case '{': minuschar = '}'; break;
							case '[': minuschar = ']'; break;
							case '(': minuschar = ')'; break;
							case '"': minuschar = '"'; break;
							case '\'': minuschar = '\''; break;
							default: minuschar = pluschar; break;
						}
						char prev = resultSTR[index]; bool escaped = prev == '"';
						int ret = index + 1;
						int summ = 1;

						while (summ != 0)
						{
							if (resultSTR[ret] == '"' && prev != '\\')
								escaped = !escaped;
							prev = resultSTR[ret];
							if (!escaped)
							{
								if (minuschar == prev) summ--;
								else if (pluschar == prev) summ++;
							}
							ret++;
						}
						return ret;
					}
					catch { return resultSTR.Length; }
				}
			}
			catch { return noValue; }
		}
		public static string[] arrayFrom(string arrayResult) { return ListFrom(arrayResult).ToArray(); }
		public static List<string> ListFrom(string arrayResult)
		{
			int length = arrayLength(arrayResult);
			List<string> ret = new List<string>();
			for (int i = 0; i < length; i++)
				ret.Add(arrayIndex(arrayResult, i));
			return ret;
		}
		public static string betweenStr(string toredo, char exclude)
		{
			char excludeEnd;
			switch (exclude)
			{
				case '{': excludeEnd = '}'; break;
				case '[': excludeEnd = ']'; break;
				case '(': excludeEnd = ')'; break;
				case '"': excludeEnd = '"'; break;
				case '\'': excludeEnd = '\''; break;
				default: excludeEnd = exclude; break;
			}
			int start = toredo.IndexOf(exclude);
			int stop = toredo.LastIndexOf(excludeEnd);
			if (start != -1 && stop > start)
				return toredo[(start + 1)..stop].Trim();
			return toredo.Trim();
		}
		public static string parseValue(string result, object key) { return parseValue(result, key, 0); }
		public static string parseValue(string result, object key, int valueIndex)
		{
			string noValue = " ";
			try
			{
				if (key.GetType() == typeof(List<object>))
				{
					List<object> keys = (List<object>)key;
					if (keys.Count > 0)
					{
						string reti = parseValue(result, keys[0], keys.Count == 1 ? valueIndex : 0).Trim();
						for (int i = 1; i < keys.Count; i++)
							if (reti != String.Empty)
								reti = parseValue(reti, keys[i], i == keys.Count - 1 ? valueIndex : 0).Trim();
							else return noValue;
						return reti;
					}
					return noValue;
				}
				else if (key.GetType() == typeof(List<string>))
				{
					List<string> keys = (List<string>)key;
					if (keys.Count > 0)
					{
						string reti = parseValue(result, keys[0], keys.Count == 1 ? valueIndex : 0).Trim();
						for (int i = 1; i < keys.Count; i++)
							if (reti != String.Empty)
								reti = parseValue(reti, keys[i], i == keys.Count - 1 ? valueIndex : 0).Trim();
							else return noValue;
						return reti;
					}
					return noValue;
				}
				else if (key.GetType() == typeof(object[]))
				{
					object[] keys = (object[])key;
					if (keys.Length > 0)
					{
						string reti = parseValue(result, keys[0], keys.Length == 1 ? valueIndex : 0).Trim();
						for (int i = 1; i < keys.Length; i++)
							if (reti != String.Empty)
								reti = parseValue(reti, keys[i], i == keys.Length - 1 ? valueIndex : 0).Trim();
							else return noValue;
						return reti;
					}
					return noValue;
				}
				else if (key.GetType() == typeof(string[]))
				{
					string[] keys = (string[])key;
					if (keys.Length > 0)
					{
						string reti = parseValue(result, keys[0], keys.Length == 1 ? valueIndex : 0).Trim();
						for (int i = 1; i < keys.Length; i++)
							if (reti != String.Empty)
								reti = parseValue(reti, keys[i], i == keys.Length - 1 ? valueIndex : 0).Trim();
							else return noValue;
						return reti;
					}
					return noValue;
				}

				string resultSTR = result.Trim();
				string keySTR = (string)key;
				int laststop = 0; int count = 0;
				while (count <= valueIndex)
				{
					int start = resultSTR.IndexOf(keySTR, laststop);
					if (start == -1) { return noValue; }
					int stop = resultSTR.IndexOf('\"', start + 1);
					if (stop == -1) { return noValue; }
					laststop = stop;

					if (start > 0 && resultSTR[start - 1] != '\"') { continue; }
					if (start > 1 && resultSTR[start - 2] == '\\') { continue; }
					if (stop > 0 && resultSTR[stop - 1] == '\\') { continue; }

					string cut = resultSTR[start..stop];
					if (cut.ToLower().Trim() == keySTR.ToLower().Trim())
					{
						int start0 = right(stop, ":");
						int start1 = right(stop, "{");
						int start2 = right(stop, "[");
						int start3 = right(stop, "\"");
						int start4 = right(stop, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._"); // +!?|\/

						int next = getPositiveMin(new List<int>() { start1, start2, start3, start4 });
						int end = right(stop, ",}]");

						if (end >= next && next > start0)
						{
							start = next;
							if (next == start1)
							{
								stop = rightPM(start, '{');
								if (count == valueIndex) return resultSTR[start..stop].Trim();
							}
							else if (next == start2)
							{
								stop = rightPM(start, '[');
								if (count == valueIndex) return resultSTR[start..stop].Trim();
							}
							else if (next == start3)
							{
								stop = rightPM(start, '"');
								if (count == valueIndex) return betweenStr(resultSTR[start..stop], '"');
							}
							else if (next == start4)
							{
								stop = right(start, ",}]");
								if (count == valueIndex) return resultSTR[start..stop].Trim();
							}
							laststop = stop;
						}
						else return noValue;
						count++;
					}
				}
				return noValue;

				int getPositiveMin(List<int> values)
				{
					values.Sort();
					for (int i = 0; i < values.Count; i++)
						if (values[i] >= 0) return values[i];
					return -1;
				}
				int right(int index, string stop)
				{
					char[] stopchars = stop.ToCharArray();
					int ret = resultSTR.IndexOfAny(stopchars, index + 1);
					return ret;
				}
				int rightPM(int index, char pluschar)
				{
					try
					{
						char minuschar;
						switch (pluschar)
						{
							case '{': minuschar = '}'; break;
							case '[': minuschar = ']'; break;
							case '(': minuschar = ')'; break;
							case '"': minuschar = '"'; break;
							case '\'': minuschar = '\''; break;
							default: minuschar = pluschar; break;
						}
						char prev = resultSTR[index]; bool escaped = prev == '"';
						int ret = index + 1;
						int summ = 1;

						while (summ != 0)
						{
							if (resultSTR[ret] == '"' && prev != '\\')
								escaped = !escaped;
							prev = resultSTR[ret];
							if (!escaped)
							{
								if (minuschar == prev) summ--;
								else if (pluschar == prev) summ++;
							}
							ret++;
						}
						return ret;
					}
					catch { return resultSTR.Length; }
				}
			}
			catch { return noValue; }
		}
		public static double todouble(string json) { return todouble(json, ".", ","); }
		public static double todouble(string json, string decimals, string groups)
		{
			if (json == String.Empty) return 0.0;
			else return Convert.ToDouble(json.Trim(), new NumberFormatInfo { NumberDecimalSeparator = decimals, NumberGroupSeparator = groups });
		}
	}
}