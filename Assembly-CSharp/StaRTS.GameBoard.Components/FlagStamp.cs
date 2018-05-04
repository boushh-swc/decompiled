using System;

namespace StaRTS.GameBoard.Components
{
	public class FlagStamp
	{
		private delegate void AlterCell(int x, int z, uint flag);

		public const int STROKE_SAFETY_LIMIT = 1000;

		public int X;

		public int Z;

		public int Width;

		public int Depth;

		public uint[,] FlagsMatrix;

		public bool IsShieldFlag;

		public int Right
		{
			get
			{
				return this.X + this.Width;
			}
		}

		public int Bottom
		{
			get
			{
				return this.Z + this.Depth;
			}
		}

		public FlagStamp(int width, int depth, uint initialFlag, bool isShiledFlag)
		{
			this.Width = width;
			this.Depth = depth;
			this.FlagsMatrix = new uint[this.Width, this.Depth];
			this.IsShieldFlag = isShiledFlag;
			this.SetFlagsInRect(0, 0, this.Width, this.Depth, initialFlag);
		}

		public void FillCircle(int radius, uint fillFlag, bool paintOrErase)
		{
			FlagStamp.AlterCell alterCell = new FlagStamp.AlterCell(this.PaintCellWithFlag);
			if (!paintOrErase)
			{
				alterCell = new FlagStamp.AlterCell(this.EraseFlagFromCell);
			}
			int num = (this.Width - 1) / 2;
			int num2 = (this.Depth - 1) / 2;
			int i = radius - 1;
			int num3 = 0;
			int num4 = 1 - i;
			while (i >= num3)
			{
				if (i > num3)
				{
					for (int j = i; j >= -i; j--)
					{
						alterCell(j + num, num3 + num2, fillFlag);
					}
				}
				if (num3 > 0 && num3 < i)
				{
					for (int k = i; k >= -i; k--)
					{
						alterCell(k + num, -num3 + num2, fillFlag);
					}
				}
				if (num4 < 0)
				{
					num3++;
					num4 += 2 * num3 + 1;
				}
				else
				{
					for (int l = num3; l >= -num3; l--)
					{
						alterCell(l + num, i + num2, fillFlag);
					}
					for (int m = num3; m >= -num3; m--)
					{
						alterCell(m + num, -i + num2, fillFlag);
					}
					num3++;
					i--;
					num4 += 2 * (num3 - i + 1);
				}
			}
		}

		private void PaintCellWithFlag(int x, int z, uint flag)
		{
			this.FlagsMatrix[x, z] |= flag;
		}

		private void EraseFlagFromCell(int x, int z, uint flag)
		{
			this.FlagsMatrix[x, z] &= ~flag;
		}

		public void StrokeHull(uint strokeFlag, uint fillFlag)
		{
			Point[] array = new Point[]
			{
				new Point(0, 1),
				new Point(1, 0),
				new Point(0, -1),
				new Point(-1, 0)
			};
			int num = 0;
			int num2 = this.Width / 2 - 1;
			int num3 = this.Depth / 2 - 1;
			uint num4 = this.FlagsMatrix[num2 + array[num].x, num3 + array[num].z];
			while ((num4 & fillFlag) == fillFlag)
			{
				num2 += array[num].x;
				num3 += array[num].z;
				num4 = this.FlagsMatrix[num2 + array[num].x, num3 + array[num].z];
			}
			num = (num + 1) % array.Length;
			bool flag = false;
			int num5 = 0;
			while (!flag)
			{
				if (++num5 > 1000)
				{
					break;
				}
				this.PaintCellWithFlag(num2, num3, strokeFlag);
				for (int i = num - 1; i < num - 1 + array.Length; i++)
				{
					int num6 = (array.Length + i) % array.Length;
					uint num7 = this.FlagsMatrix[num2 + array[num6].x, num3 + array[num6].z];
					if ((num7 & strokeFlag) == strokeFlag)
					{
						flag = true;
						break;
					}
					if ((num7 & fillFlag) == fillFlag)
					{
						num = num6;
						num2 += array[num].x;
						num3 += array[num].z;
						break;
					}
				}
			}
		}

		public void Clear()
		{
			for (int i = 0; i < this.Width; i++)
			{
				for (int j = 0; j < this.Depth; j++)
				{
					this.FlagsMatrix[i, j] = 0u;
				}
			}
		}

		public void Fill(uint flags)
		{
			for (int i = 0; i < this.Width; i++)
			{
				for (int j = 0; j < this.Depth; j++)
				{
					this.FlagsMatrix[i, j] |= flags;
				}
			}
		}

		public void SetFlagsInRect(int x, int z, int width, int depth, uint flag)
		{
			int i = x;
			int num = x + width;
			while (i < num)
			{
				int j = z;
				int num2 = z + depth;
				while (j < num2)
				{
					this.FlagsMatrix[i, j] |= flag;
					j++;
				}
				i++;
			}
		}

		public void UnsetFlagsInRect(int x, int z, int width, int depth, uint flag)
		{
			int i = x;
			int num = x + width;
			while (i < num)
			{
				int j = z;
				int num2 = z + depth;
				while (j < num2)
				{
					this.FlagsMatrix[i, j] &= ~flag;
					j++;
				}
				i++;
			}
		}

		public void SetFlagsInSquare(int x, int z, int size, uint flag)
		{
			this.SetFlagsInRect(x, z, size, size, flag);
		}

		public void SetFlagsInRectCenter(int width, int depth, uint flag)
		{
			this.SetFlagsInRect((this.Width - width) / 2, (this.Depth - depth) / 2, width, depth, flag);
		}

		public void SetFlagsInSquareCenter(int size, uint flag)
		{
			this.SetFlagsInRectCenter(size, size, flag);
		}

		public void UnsetFlagsInSquare(int x, int z, int size, uint flag)
		{
			this.UnsetFlagsInRect(x, z, size, size, flag);
		}

		public void UnsetFlagsInRectCenter(int width, int depth, uint flag)
		{
			this.UnsetFlagsInRect((this.Width - width) / 2, (this.Depth - depth) / 2, width, depth, flag);
		}

		public void UnsetFlagsInSquareCenter(int size, uint flag)
		{
			this.UnsetFlagsInRectCenter(size, size, flag);
		}

		public void CenterTo(int x, int z)
		{
			this.X = x - this.Width / 2;
			this.Z = z - this.Depth / 2;
		}

		public uint GetFlagsForCell(int cellX, int cellZ)
		{
			int num = cellX - this.X;
			int num2 = cellZ - this.Z;
			return this.FlagsMatrix[num, num2];
		}
	}
}
