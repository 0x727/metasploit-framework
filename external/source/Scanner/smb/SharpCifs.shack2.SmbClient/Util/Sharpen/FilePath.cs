using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x0200003E RID: 62
	public class FilePath
	{
		// Token: 0x06000180 RID: 384 RVA: 0x00003195 File Offset: 0x00001395
		public FilePath()
		{
		}

		// Token: 0x06000181 RID: 385 RVA: 0x00008404 File Offset: 0x00006604
		public FilePath(string path) : this((string)null, path)
		{
		}

		// Token: 0x06000182 RID: 386 RVA: 0x00008410 File Offset: 0x00006610
		public FilePath(FilePath other, string child) : this((string)other, child)
		{
		}

		// Token: 0x06000183 RID: 387 RVA: 0x00008424 File Offset: 0x00006624
		public FilePath(string other, string child)
		{
			bool flag = other == null;
			if (flag)
			{
				this._path = child;
			}
			else
			{
				while (!string.IsNullOrEmpty(child) && (child[0] == Path.DirectorySeparatorChar || child[0] == Path.AltDirectorySeparatorChar))
				{
					child = child.Substring(1);
				}
				bool flag2 = !string.IsNullOrEmpty(other) && other[other.Length - 1] == Path.VolumeSeparatorChar;
				if (flag2)
				{
					other += Path.DirectorySeparatorChar.ToString();
				}
				this._path = Path.Combine(other, child);
			}
		}

		// Token: 0x06000184 RID: 388 RVA: 0x000084D0 File Offset: 0x000066D0
		public static implicit operator FilePath(string name)
		{
			return new FilePath(name);
		}

		// Token: 0x06000185 RID: 389 RVA: 0x000084E8 File Offset: 0x000066E8
		public static implicit operator string(FilePath filePath)
		{
			return (filePath == null) ? null : filePath._path;
		}

		// Token: 0x06000186 RID: 390 RVA: 0x00008508 File Offset: 0x00006708
		public override bool Equals(object obj)
		{
			FilePath filePath = obj as FilePath;
			bool flag = filePath == null;
			return !flag && this.GetCanonicalPath() == filePath.GetCanonicalPath();
		}

		// Token: 0x06000187 RID: 391 RVA: 0x00008540 File Offset: 0x00006740
		public override int GetHashCode()
		{
			return this._path.GetHashCode();
		}

		// Token: 0x06000188 RID: 392 RVA: 0x00008560 File Offset: 0x00006760
		public bool CreateNewFile()
		{
			bool result;
			try
			{
				File.Open(this._path, FileMode.CreateNew).Close();
				result = true;
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000189 RID: 393 RVA: 0x0000859C File Offset: 0x0000679C
		public static FilePath CreateTempFile()
		{
			return new FilePath(Path.GetTempFileName());
		}

		// Token: 0x0600018A RID: 394 RVA: 0x000085B8 File Offset: 0x000067B8
		public static FilePath CreateTempFile(string prefix, string suffix)
		{
			return FilePath.CreateTempFile(prefix, suffix, null);
		}

		// Token: 0x0600018B RID: 395 RVA: 0x000085D4 File Offset: 0x000067D4
		public static FilePath CreateTempFile(string prefix, string suffix, FilePath directory)
		{
			bool flag = prefix == null;
			if (flag)
			{
				throw new ArgumentNullException("prefix");
			}
			bool flag2 = prefix.Length < 3;
			if (flag2)
			{
				throw new ArgumentException("prefix must have at least 3 characters");
			}
			string path = (directory == null) ? Path.GetTempPath() : directory.GetPath();
			string text;
			do
			{
				text = Path.Combine(path, prefix + Interlocked.Increment(ref FilePath._tempCounter) + suffix);
			}
			while (File.Exists(text));
			new FileOutputStream(text).Close();
			return new FilePath(text);
		}

		// Token: 0x0600018C RID: 396 RVA: 0x00008663 File Offset: 0x00006863
		public void DeleteOnExit()
		{
		}

		// Token: 0x0600018D RID: 397 RVA: 0x00008668 File Offset: 0x00006868
		public FilePath GetAbsoluteFile()
		{
			return new FilePath(Path.GetFullPath(this._path));
		}

		// Token: 0x0600018E RID: 398 RVA: 0x0000868C File Offset: 0x0000688C
		public string GetAbsolutePath()
		{
			return Path.GetFullPath(this._path);
		}

		// Token: 0x0600018F RID: 399 RVA: 0x000086AC File Offset: 0x000068AC
		public FilePath GetCanonicalFile()
		{
			return new FilePath(this.GetCanonicalPath());
		}

		// Token: 0x06000190 RID: 400 RVA: 0x000086CC File Offset: 0x000068CC
		public string GetCanonicalPath()
		{
			string fullPath = Path.GetFullPath(this._path);
			fullPath.TrimEnd(new char[]
			{
				Path.DirectorySeparatorChar
			});
			return fullPath;
		}

		// Token: 0x06000191 RID: 401 RVA: 0x00008700 File Offset: 0x00006900
		public string GetName()
		{
			return Path.GetFileName(this._path);
		}

		// Token: 0x06000192 RID: 402 RVA: 0x00008720 File Offset: 0x00006920
		public FilePath GetParentFile()
		{
			return new FilePath(Path.GetDirectoryName(this._path));
		}

		// Token: 0x06000193 RID: 403 RVA: 0x00008744 File Offset: 0x00006944
		public string GetPath()
		{
			return this._path;
		}

		// Token: 0x06000194 RID: 404 RVA: 0x0000875C File Offset: 0x0000695C
		public bool IsAbsolute()
		{
			return Path.IsPathRooted(this._path);
		}

		// Token: 0x06000195 RID: 405 RVA: 0x0000877C File Offset: 0x0000697C
		public bool IsDirectory()
		{
			return false;
		}

		// Token: 0x06000196 RID: 406 RVA: 0x00008790 File Offset: 0x00006990
		public bool IsFile()
		{
			return false;
		}

		// Token: 0x06000197 RID: 407 RVA: 0x000087A4 File Offset: 0x000069A4
		public long LastModified()
		{
			return 0L;
		}

		// Token: 0x06000198 RID: 408 RVA: 0x000087B8 File Offset: 0x000069B8
		public long Length()
		{
			return 0L;
		}

		// Token: 0x06000199 RID: 409 RVA: 0x000087CC File Offset: 0x000069CC
		public string[] List()
		{
			return this.List(null);
		}

		// Token: 0x0600019A RID: 410 RVA: 0x000087E8 File Offset: 0x000069E8
		public string[] List(IFilenameFilter filter)
		{
			string[] result;
			try
			{
				bool flag = this.IsFile();
				if (flag)
				{
					result = null;
				}
				else
				{
					List<string> list = new List<string>();
					foreach (string path in Directory.GetFileSystemEntries(this._path))
					{
						string fileName = Path.GetFileName(path);
						bool flag2 = filter == null || filter.Accept(this, fileName);
						if (flag2)
						{
							list.Add(fileName);
						}
					}
					result = list.ToArray();
				}
			}
			catch
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600019B RID: 411 RVA: 0x0000887C File Offset: 0x00006A7C
		public FilePath[] ListFiles()
		{
			FilePath[] result;
			try
			{
				bool flag = this.IsFile();
				if (flag)
				{
					result = null;
				}
				else
				{
					List<FilePath> list = new List<FilePath>();
					foreach (string path in Directory.GetFileSystemEntries(this._path))
					{
						list.Add(new FilePath(path));
					}
					result = list.ToArray();
				}
			}
			catch
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600019C RID: 412 RVA: 0x00008663 File Offset: 0x00006863
		private static void MakeDirWritable(string dir)
		{
		}

		// Token: 0x0600019D RID: 413 RVA: 0x00008663 File Offset: 0x00006863
		private static void MakeFileWritable(string file)
		{
		}

		// Token: 0x0600019E RID: 414 RVA: 0x000088F4 File Offset: 0x00006AF4
		public bool Mkdir()
		{
			bool result;
			try
			{
				bool flag = Directory.Exists(this._path);
				if (flag)
				{
					result = false;
				}
				else
				{
					Directory.CreateDirectory(this._path);
					result = true;
				}
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600019F RID: 415 RVA: 0x00008940 File Offset: 0x00006B40
		public bool Mkdirs()
		{
			bool result;
			try
			{
				bool flag = Directory.Exists(this._path);
				if (flag)
				{
					result = false;
				}
				else
				{
					Directory.CreateDirectory(this._path);
					result = true;
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x0000898C File Offset: 0x00006B8C
		public bool RenameTo(FilePath file)
		{
			return this.RenameTo(file._path);
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x000089AC File Offset: 0x00006BAC
		public bool RenameTo(string name)
		{
			return false;
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x000089C0 File Offset: 0x00006BC0
		public bool SetLastModified(long milis)
		{
			return false;
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x000089D4 File Offset: 0x00006BD4
		public bool SetReadOnly()
		{
			return false;
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x000089E8 File Offset: 0x00006BE8
		public Uri ToUri()
		{
			return new Uri(this._path);
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x00008A08 File Offset: 0x00006C08
		public bool CanExecute()
		{
			return false;
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x00008A1C File Offset: 0x00006C1C
		public bool SetExecutable(bool exec)
		{
			return false;
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x00008A30 File Offset: 0x00006C30
		public string GetParent()
		{
			string directoryName = Path.GetDirectoryName(this._path);
			bool flag = string.IsNullOrEmpty(directoryName) || directoryName == this._path;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = directoryName;
			}
			return result;
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x00008A70 File Offset: 0x00006C70
		public override string ToString()
		{
			return this._path;
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x060001A9 RID: 425 RVA: 0x00008A88 File Offset: 0x00006C88
		internal static string PathSeparator
		{
			get
			{
				return Path.PathSeparator.ToString();
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x060001AA RID: 426 RVA: 0x00008AA8 File Offset: 0x00006CA8
		internal static char PathSeparatorChar
		{
			get
			{
				return Path.PathSeparator;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x060001AB RID: 427 RVA: 0x00008AC0 File Offset: 0x00006CC0
		internal static char SeparatorChar
		{
			get
			{
				return Path.DirectorySeparatorChar;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x060001AC RID: 428 RVA: 0x00008AD8 File Offset: 0x00006CD8
		internal static string Separator
		{
			get
			{
				return Path.DirectorySeparatorChar.ToString();
			}
		}

		// Token: 0x04000059 RID: 89
		private string _path;

		// Token: 0x0400005A RID: 90
		private static long _tempCounter;
	}
}
