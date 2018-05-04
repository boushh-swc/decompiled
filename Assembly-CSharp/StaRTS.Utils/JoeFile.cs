using StaRTS.Utils.Core;
using StaRTS.Utils.MetaData;
using System;
using System.Text;

namespace StaRTS.Utils
{
	public class JoeFile
	{
		private const int MAX_SUPPORTED_VERSION = 0;

		private Sheet[] sheets;

		private byte[] bytes;

		private int pos;

		private int rem;

		private int ver = -1;

		private string[] strings;

		private int[] ints;

		private float[] floats;

		private string[][] arrays;

		private int stringCount;

		private int intCount;

		private int floatCount;

		private int arrayCount;

		public JoeFile(byte[] rawFileBytes)
		{
			this.bytes = rawFileBytes;
			if (this.bytes != null)
			{
				this.Parse();
				this.bytes = null;
			}
		}

		public Sheet GetSheet(int i)
		{
			return (this.sheets != null && i >= 0 && i < this.sheets.Length) ? this.sheets[i] : null;
		}

		public Sheet[] GetAllSheets()
		{
			return this.sheets;
		}

		private bool FatalError(string error)
		{
			this.sheets = null;
			this.bytes = null;
			this.strings = null;
			this.ints = null;
			this.floats = null;
			this.arrays = null;
			Service.Logger.Error("JoeFile: " + error);
			return false;
		}

		private bool RangeError(string kind, int index, int count)
		{
			return this.FatalError(string.Format("invalid {0} index: {1}:{2}", kind, index, count));
		}

		private bool Parse()
		{
			int num = this.bytes.Length;
			this.pos = 0;
			this.rem = num;
			this.ver = -1;
			if (!this.ParseSignature())
			{
				return this.FatalError("invalid signature");
			}
			if (!this.ParseVersion())
			{
				return this.FatalError("unsupported version: " + this.ver);
			}
			if (!this.ParseStringTable())
			{
				return this.FatalError("unable to parse string table");
			}
			if (!this.ParseIntegerTable())
			{
				return this.FatalError("unable to parse integer table");
			}
			if (!this.ParseFloatTable())
			{
				return this.FatalError("unable to parse float table");
			}
			if (!this.ParseStringArrayTable())
			{
				return this.FatalError("unable to parse string array table");
			}
			if (!this.ParseSheetNames())
			{
				return this.FatalError("unable to parse sheet names");
			}
			int i = 0;
			int num2 = this.sheets.Length;
			while (i < num2)
			{
				Sheet sheet = this.sheets[i];
				if (!this.ParseSheetColumns(sheet))
				{
					return this.FatalError("unable to parse sheet columns: " + sheet.SheetName);
				}
				if (!this.ParseSheetCells(sheet))
				{
					return this.FatalError("unable to parse sheet cells: " + sheet.SheetName);
				}
				sheet.SetupComplete();
				i++;
			}
			if (!this.ParseFinalByte())
			{
				return this.FatalError(string.Format("unable to parse final byte {0},{1},{2}", this.rem, this.pos, num));
			}
			if (this.rem != 0 || this.pos != num)
			{
				Service.Logger.WarnFormat("JoeFile: trailing bytes found {0},{1},{2}", new object[]
				{
					this.rem,
					this.pos,
					num
				});
			}
			return true;
		}

		private bool ParseSignature()
		{
			if ((this.rem -= 3) < 0)
			{
				return this.FatalError("not enough bytes for signature");
			}
			if (this.bytes[this.pos++] != 74)
			{
				return this.FatalError("invalid first signature byte");
			}
			if (this.bytes[this.pos++] != 79)
			{
				return this.FatalError("invalid second signature byte");
			}
			return this.bytes[this.pos++] == 69 || this.FatalError("invalid third signature byte");
		}

		private bool ParseVersion()
		{
			if (--this.rem < 0)
			{
				return this.FatalError("not enough bytes for version");
			}
			this.ver = (int)this.bytes[this.pos++];
			return this.ver <= 0 || this.FatalError(string.Format("unsupported version {0} > {1}", this.ver, 0));
		}

		private bool ParseStringTable()
		{
			if (!this.DecodeDword(out this.stringCount) || this.stringCount < 0)
			{
				return this.FatalError("invalid string table size");
			}
			this.strings = new string[this.stringCount];
			for (int i = 0; i < this.stringCount; i++)
			{
				uint num;
				if (!this.DecodeVariableLength(out num))
				{
					return this.FatalError("unable to decode string length");
				}
				int num2 = (int)num;
				if (num2 < 0)
				{
					return this.FatalError("invalid string length");
				}
				if ((this.rem -= num2) < 0)
				{
					return this.FatalError("not enough bytes for string");
				}
				this.strings[i] = Encoding.UTF8.GetString(this.bytes, this.pos, num2);
				this.pos += num2;
			}
			return true;
		}

		private bool ParseIntegerTable()
		{
			if (!this.DecodeDword(out this.intCount) || this.intCount < 0)
			{
				return this.FatalError("invalid integer table size");
			}
			this.ints = new int[this.intCount];
			for (int i = 0; i < this.intCount; i++)
			{
				if ((this.rem -= 4) < 0)
				{
					return this.FatalError("not enough bytes for integer");
				}
				this.ints[i] = BitConverter.ToInt32(this.bytes, this.pos);
				this.pos += 4;
			}
			return true;
		}

		private bool ParseFloatTable()
		{
			if (!this.DecodeDword(out this.floatCount) || this.floatCount < 0)
			{
				return this.FatalError("invalid float table size");
			}
			this.floats = new float[this.floatCount];
			for (int i = 0; i < this.floatCount; i++)
			{
				if ((this.rem -= 4) < 0)
				{
					return this.FatalError("not enough bytes for float");
				}
				this.floats[i] = BitConverter.ToSingle(this.bytes, this.pos);
				this.pos += 4;
			}
			return true;
		}

		private bool ParseStringArrayTable()
		{
			if (!this.DecodeDword(out this.arrayCount) || this.arrayCount < 0)
			{
				return this.FatalError("invalid array table size");
			}
			this.arrays = new string[this.arrayCount][];
			for (int i = 0; i < this.arrayCount; i++)
			{
				uint num;
				if (!this.DecodeVariableLength(out num))
				{
					return this.FatalError("unable to decode array length");
				}
				int num2 = (int)num;
				if (num2 < 0)
				{
					return this.FatalError("invalid array length");
				}
				string[] array = new string[num2];
				for (int j = 0; j < num2; j++)
				{
					if (!this.DecodeVariableLength(out num))
					{
						return this.FatalError("unable to decode array string index");
					}
					int num3 = (int)num;
					if (num3 < 0 || num3 > this.stringCount)
					{
						return this.RangeError("array string", num3, this.stringCount);
					}
					array[j] = this.strings[num3];
				}
				this.arrays[i] = array;
			}
			return true;
		}

		private bool ParseSheetNames()
		{
			uint num;
			if (!this.DecodeVariableLength(out num))
			{
				return this.FatalError("unable to decode sheet count");
			}
			int num2 = (int)num;
			if (num2 < 0)
			{
				return this.FatalError("invalid sheet count");
			}
			this.sheets = new Sheet[num2];
			for (int i = 0; i < num2; i++)
			{
				if (!this.DecodeVariableLength(out num))
				{
					return this.FatalError("unable to decode sheet string index");
				}
				int num3 = (int)num;
				if (num3 < 0 || num3 > this.stringCount)
				{
					return this.RangeError("sheet string", num3, this.stringCount);
				}
				string sheetName = this.strings[num3];
				Sheet sheet = new Sheet(sheetName, this.strings, this.floats, this.arrays);
				this.sheets[i] = sheet;
			}
			return true;
		}

		private bool ParseSheetColumns(Sheet sheet)
		{
			uint num;
			if (!this.DecodeVariableLength(out num))
			{
				return this.FatalError("unable to decode column count");
			}
			int num2 = (int)num;
			if (num2 < 0)
			{
				return this.FatalError("invalid column count");
			}
			Column[] array = new Column[num2];
			for (int i = 0; i < num2; i++)
			{
				if (--this.rem < 0)
				{
					return this.FatalError("not enough bytes for column type");
				}
				ColumnType colType = (ColumnType)this.bytes[this.pos++];
				if (!this.DecodeVariableLength(out num))
				{
					return this.FatalError("unable to decode column string index");
				}
				int num3 = (int)num;
				if (num3 < 0 || num3 > this.stringCount)
				{
					return this.RangeError("column string", num3, this.stringCount);
				}
				string colName = this.strings[num3];
				array[i] = new Column(colName, colType);
			}
			sheet.SetupColumns(array);
			return true;
		}

		private bool ParseSheetCells(Sheet sheet)
		{
			uint num;
			if (!this.DecodeVariableLength(out num))
			{
				return this.FatalError("unable to decode cell count");
			}
			int num2 = (int)num;
			if (num2 < 0)
			{
				return this.FatalError("invalid cell count");
			}
			uint[] array = new uint[num2];
			Column[] array2 = sheet.InternalGetAllColumns();
			int num3 = array2.Length;
			int num4 = 0;
			if (num3 == 0 || num2 % num3 != 0)
			{
				return this.FatalError(string.Format("cell count {0} is not a multiple of column count {1}", num2, num3));
			}
			for (int i = 0; i < num2; i++)
			{
				if (this.rem == 0)
				{
					return this.FatalError("not enough bytes for cell");
				}
				if (this.bytes[this.pos] == 0)
				{
					array[i] = 0u;
					this.pos++;
					this.rem--;
				}
				else
				{
					uint num5;
					switch (array2[num4].ColType)
					{
					case ColumnType.String:
					{
						if (!this.DecodeVariableLength(out num5))
						{
							return this.FatalError("unable to decode cell string index");
						}
						int num6 = (int)num5;
						if (num6 <= 0 || num6 > this.stringCount)
						{
							return this.RangeError("cell string", num6, this.stringCount);
						}
						break;
					}
					case ColumnType.Boolean:
						if (--this.rem < 0)
						{
							return this.FatalError("not enough bytes for bool cell");
						}
						num5 = (uint)this.bytes[this.pos++];
						break;
					case ColumnType.NonNegativeInt:
						if (!this.DecodeVariableLength(out num5))
						{
							return this.FatalError("unable to decode non-negative int cell");
						}
						break;
					case ColumnType.RawInt:
					{
						if (!this.DecodeVariableLength(out num5))
						{
							return this.FatalError("unable to decode raw int index");
						}
						int num7 = (int)num5;
						if (num7 <= 0 || num7 > this.intCount)
						{
							return this.RangeError("raw int", num7, this.intCount);
						}
						num5 = (uint)this.ints[num7 - 1];
						if ((num5 & 2147483648u) == 0u)
						{
							num5 += 1u;
						}
						break;
					}
					case ColumnType.Float:
					{
						if (!this.DecodeVariableLength(out num5))
						{
							return this.FatalError("unable to decode float index");
						}
						int num8 = (int)num5;
						if (num8 <= 0 || num8 > this.floatCount)
						{
							return this.RangeError("float", num8, this.floatCount);
						}
						break;
					}
					case ColumnType.StringArray:
					{
						if (!this.DecodeVariableLength(out num5))
						{
							return this.FatalError("unable to decode array index");
						}
						int num9 = (int)num5;
						if (num9 <= 0 || num9 > this.arrayCount)
						{
							return this.RangeError("array", num9, this.arrayCount);
						}
						break;
					}
					default:
						return this.FatalError("unsupported column type");
					}
					array[i] = num5;
				}
				if (++num4 == num3)
				{
					num4 = 0;
				}
			}
			sheet.SetupCells(array);
			return true;
		}

		private bool ParseFinalByte()
		{
			if (--this.rem < 0)
			{
				return this.FatalError("not enough bytes for final byte");
			}
			byte b = this.bytes[this.pos++];
			return b == 0 || this.FatalError("invalid final byte");
		}

		private bool DecodeDword(out int val)
		{
			val = 0;
			if ((this.rem -= 4) < 0)
			{
				return this.FatalError("not enough bytes for dword");
			}
			val = BitConverter.ToInt32(this.bytes, this.pos);
			this.pos += 4;
			return true;
		}

		private bool DecodeVariableLength(out uint val)
		{
			val = 0u;
			while (--this.rem >= 0)
			{
				uint num = (uint)this.bytes[this.pos++];
				if ((num & 128u) == 0u)
				{
					val = (val << 7 | num);
					return true;
				}
				val = (val << 7 | (num & 127u));
			}
			return this.FatalError("not enough bytes for variable-length");
		}
	}
}
