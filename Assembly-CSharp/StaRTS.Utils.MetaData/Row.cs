using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Utils.MetaData
{
	public class Row
	{
		private Sheet dataSheet;

		private Sheet masterSheet;

		private int startIndex;

		private List<Row> patchRows;

		private bool hasPatches;

		private bool remapColumns;

		public string Uid
		{
			get;
			private set;
		}

		public Row(string uid, Sheet sheet, int rowStartIndex)
		{
			this.Uid = uid;
			this.dataSheet = sheet;
			this.masterSheet = sheet;
			this.startIndex = rowStartIndex;
			this.patchRows = null;
			this.hasPatches = false;
			this.remapColumns = false;
		}

		public void PatchColumns(Row row)
		{
			if (row.hasPatches)
			{
				Service.Logger.ErrorFormat("Cannot patch with a row {0} that has patches", new object[]
				{
					row.Uid
				});
				return;
			}
			if (!this.hasPatches)
			{
				this.patchRows = new List<Row>();
				this.hasPatches = true;
			}
			this.patchRows.Add(row);
		}

		public void InternalSetMasterSheet(Sheet sheet)
		{
			this.masterSheet = sheet;
			this.remapColumns = (this.masterSheet != this.dataSheet);
		}

		public void Invalidate()
		{
			this.dataSheet = null;
			this.masterSheet = null;
			this.patchRows = null;
		}

		public string TryGetString(int column)
		{
			return this.TryGetString(column, null);
		}

		public string TryGetString(int column, string fallback)
		{
			string text;
			if (this.hasPatches)
			{
				for (int i = this.patchRows.Count - 1; i >= 0; i--)
				{
					Row row = this.patchRows[i];
					int columnIndex = column;
					if (this.RemapColumn(row, ref columnIndex) && row.dataSheet.InternalGetString(row.startIndex, columnIndex, out text))
					{
						return text;
					}
				}
			}
			if (this.remapColumns && !this.RemapColumn(this, ref column))
			{
				return fallback;
			}
			return (!this.dataSheet.InternalGetString(this.startIndex, column, out text)) ? fallback : text;
		}

		public bool TryGetBool(int column)
		{
			bool result;
			if (this.hasPatches)
			{
				for (int i = this.patchRows.Count - 1; i >= 0; i--)
				{
					Row row = this.patchRows[i];
					int columnIndex = column;
					if (this.RemapColumn(row, ref columnIndex) && row.dataSheet.InternalGetBool(row.startIndex, columnIndex, out result))
					{
						return result;
					}
				}
			}
			if (this.remapColumns && !this.RemapColumn(this, ref column))
			{
				return false;
			}
			this.dataSheet.InternalGetBool(this.startIndex, column, out result);
			return result;
		}

		public int TryGetInt(int column)
		{
			return this.TryGetInt(column, 0);
		}

		public int TryGetInt(int column, int fallback)
		{
			int num;
			if (this.hasPatches)
			{
				for (int i = this.patchRows.Count - 1; i >= 0; i--)
				{
					Row row = this.patchRows[i];
					int columnIndex = column;
					if (this.RemapColumn(row, ref columnIndex) && row.dataSheet.InternalGetInt(row.startIndex, columnIndex, out num))
					{
						return num;
					}
				}
			}
			if (this.remapColumns && !this.RemapColumn(this, ref column))
			{
				return fallback;
			}
			return (!this.dataSheet.InternalGetInt(this.startIndex, column, out num)) ? fallback : num;
		}

		public uint TryGetUint(int column)
		{
			return this.TryGetUint(column, 0u);
		}

		public uint TryGetUint(int column, uint fallback)
		{
			uint num;
			if (this.hasPatches)
			{
				for (int i = this.patchRows.Count - 1; i >= 0; i--)
				{
					Row row = this.patchRows[i];
					int columnIndex = column;
					if (this.RemapColumn(row, ref columnIndex) && row.dataSheet.InternalGetUint(row.startIndex, columnIndex, out num))
					{
						return num;
					}
				}
			}
			if (this.remapColumns && !this.RemapColumn(this, ref column))
			{
				return fallback;
			}
			return (!this.dataSheet.InternalGetUint(this.startIndex, column, out num)) ? fallback : num;
		}

		public float TryGetFloat(int column)
		{
			return this.TryGetFloat(column, 0f);
		}

		public float TryGetFloat(int column, float fallback)
		{
			float num;
			if (this.hasPatches)
			{
				for (int i = this.patchRows.Count - 1; i >= 0; i--)
				{
					Row row = this.patchRows[i];
					int columnIndex = column;
					if (this.RemapColumn(row, ref columnIndex) && row.dataSheet.InternalGetFloat(row.startIndex, columnIndex, out num))
					{
						return num;
					}
				}
			}
			if (this.remapColumns && !this.RemapColumn(this, ref column))
			{
				return fallback;
			}
			return (!this.dataSheet.InternalGetFloat(this.startIndex, column, out num)) ? fallback : num;
		}

		public string[] TryGetStringArray(int column)
		{
			string[] result;
			if (this.hasPatches)
			{
				for (int i = this.patchRows.Count - 1; i >= 0; i--)
				{
					Row row = this.patchRows[i];
					int columnIndex = column;
					if (this.RemapColumn(row, ref columnIndex) && row.dataSheet.InternalGetStringArray(row.startIndex, columnIndex, out result))
					{
						return result;
					}
				}
			}
			if (this.remapColumns && !this.RemapColumn(this, ref column))
			{
				return null;
			}
			this.dataSheet.InternalGetStringArray(this.startIndex, column, out result);
			return result;
		}

		public int[] TryGetIntArray(int column)
		{
			string text = this.TryGetString(column);
			if (text == null)
			{
				return null;
			}
			string[] array = text.Split(new char[]
			{
				','
			});
			int num = array.Length;
			int[] array2 = new int[num];
			for (int i = 0; i < num; i++)
			{
				int num2;
				if (int.TryParse(array[i], out num2))
				{
					array2[i] = num2;
				}
				else
				{
					array2[i] = 0;
				}
			}
			return array2;
		}

		public float[] TryGetFloatArray(int column)
		{
			string text = this.TryGetString(column);
			if (text == null)
			{
				return null;
			}
			return this.ExtractFloatArrayFromString(text);
		}

		private float[] ExtractFloatArrayFromString(string raw)
		{
			string[] array = raw.Split(new char[]
			{
				','
			});
			int num = array.Length;
			float[] array2 = new float[num];
			for (int i = 0; i < num; i++)
			{
				float num2;
				if (float.TryParse(array[i], out num2))
				{
					array2[i] = num2;
				}
				else
				{
					array2[i] = 0f;
				}
			}
			return array2;
		}

		public Vector3 TryGetVector3(int column)
		{
			return this.TryGetVector3(column, Vector3.zero);
		}

		public Vector3 TryGetVector3(int column, Vector3 fallback)
		{
			float[] a = this.TryGetFloatArray(column);
			return this.AssembleVector3FromFloatArray(a, fallback);
		}

		private Vector3 AssembleVector3FromFloatArray(float[] a, Vector3 fallback)
		{
			if (a == null || a.Length != 3)
			{
				return fallback;
			}
			return new Vector3(a[0], a[1], a[2]);
		}

		public Vector3[] TryGetVector3Array(int column)
		{
			string text = this.TryGetString(column);
			if (string.IsNullOrEmpty(text))
			{
				return new Vector3[0];
			}
			string[] array = text.Split(new char[]
			{
				' '
			});
			int num = array.Length;
			Vector3[] array2 = new Vector3[num];
			for (int i = 0; i < num; i++)
			{
				array2[i] = this.AssembleVector3FromFloatArray(this.ExtractFloatArrayFromString(array[i]), Vector3.zero);
			}
			return array2;
		}

		public string TryGetHexValueString(int column)
		{
			string text = this.TryGetString(column);
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}
			int length = text.Length;
			if (length < 6)
			{
				int count = 6 - length;
				text = new string('0', count) + text;
			}
			return text;
		}

		private bool RemapColumn(Row row, ref int columnIndex)
		{
			string columnName = this.masterSheet.GetColumnName(columnIndex);
			columnIndex = row.dataSheet.GetColumnIndex(columnName);
			return columnIndex >= 0;
		}
	}
}
