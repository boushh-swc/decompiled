using System;
using System.IO;

namespace StaRTS.Utils.IO
{
	public static class FileUtils
	{
		public static string Read(string absFilePath)
		{
			string result;
			try
			{
				result = File.ReadAllText(absFilePath);
			}
			catch (Exception)
			{
				throw new Exception("Failed to read file: " + absFilePath);
			}
			return result;
		}

		public static void Write(string absFilePath, string text)
		{
			StreamWriter streamWriter = null;
			try
			{
				streamWriter = File.CreateText(absFilePath);
				streamWriter.WriteLine(text);
			}
			catch (Exception)
			{
				throw new Exception("Failed to write file: " + absFilePath);
			}
			finally
			{
				if (streamWriter != null)
				{
					streamWriter.Close();
					streamWriter.Dispose();
				}
			}
		}

		public static string GetAbsFilePathInMyDocuments(string fileName, string dir)
		{
			return FileUtils.GetAbsDirPathInMyDocuments(dir) + "/" + fileName;
		}

		public static string GetAbsDirPathInMyDocuments(string dir)
		{
			dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + dir;
			return dir;
		}
	}
}
