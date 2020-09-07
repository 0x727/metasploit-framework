using System;
using System.Threading.Tasks;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x020000AA RID: 170
	public static class SmbFileExtensions
	{
		// Token: 0x0600054C RID: 1356 RVA: 0x0001BD38 File Offset: 0x00019F38
		public static DateTime GetLocalCreateTime(this SmbFile smbFile)
		{
			return TimeZoneInfo.ConvertTime(Extensions.CreateDateFromUTC(smbFile.CreateTime()), TimeZoneInfo.Local);
		}

		// Token: 0x0600054D RID: 1357 RVA: 0x0001BD60 File Offset: 0x00019F60
		public static DateTime GetLocalLastModified(this SmbFile smbFile)
		{
			return TimeZoneInfo.ConvertTime(Extensions.CreateDateFromUTC(smbFile.LastModified()), TimeZoneInfo.Local);
		}

		// Token: 0x0600054E RID: 1358 RVA: 0x0001BD88 File Offset: 0x00019F88
		public static Task<SmbFile[]> ListFilesAsync(this SmbFile smbFile)
		{
			Task task = new Task(delegate()
			{
				smbFile.ListFiles();
			});
			task.Start();
			Task.WaitAll(new Task[]
			{
				task
			});
			return (Task<SmbFile[]>)task.AsyncState;
		}

		// Token: 0x0600054F RID: 1359 RVA: 0x0001BDDC File Offset: 0x00019FDC
		public static Task<SmbFile[]> ListFilesAsync(this SmbFile smbFile, string wildcard)
		{
			Task task = new Task(delegate()
			{
				smbFile.ListFiles(wildcard);
			});
			task.Start();
			Task.WaitAll(new Task[]
			{
				task
			});
			return (Task<SmbFile[]>)task.AsyncState;
		}

		// Token: 0x06000550 RID: 1360 RVA: 0x0001BE38 File Offset: 0x0001A038
		public static Task<string[]> ListAsync(this SmbFile smbFile)
		{
			Task task = new Task(delegate()
			{
				smbFile.List();
			});
			task.Start();
			Task.WaitAll(new Task[]
			{
				task
			});
			return (Task<string[]>)task.AsyncState;
		}

		// Token: 0x06000551 RID: 1361 RVA: 0x0001BE8C File Offset: 0x0001A08C
		public static Task MkDirAsync(this SmbFile smbFile)
		{
			Task task = new Task(delegate()
			{
				smbFile.Mkdir();
			});
			task.Start();
			Task.WaitAll(new Task[]
			{
				task
			});
			return (Task)task.AsyncState;
		}

		// Token: 0x06000552 RID: 1362 RVA: 0x0001BEE0 File Offset: 0x0001A0E0
		public static Task DeleteAsync(this SmbFile smbFile)
		{
			Task task = new Task(delegate()
			{
				smbFile.Delete();
			});
			task.Start();
			Task.WaitAll(new Task[]
			{
				task
			});
			return (Task)task.AsyncState;
		}

		// Token: 0x06000553 RID: 1363 RVA: 0x0001BF34 File Offset: 0x0001A134
		public static Task RenameToAsync(this SmbFile smbFile, SmbFile destination)
		{
			Task task = new Task(delegate()
			{
				smbFile.RenameTo(destination);
			});
			task.Start();
			Task.WaitAll(new Task[]
			{
				task
			});
			return (Task)task.AsyncState;
		}

		// Token: 0x06000554 RID: 1364 RVA: 0x0001BF90 File Offset: 0x0001A190
		public static Task<InputStream> GetInputStreamAsync(this SmbFile smbFile)
		{
			Task task = new Task(delegate()
			{
				smbFile.GetInputStream();
			});
			task.Start();
			Task.WaitAll(new Task[]
			{
				task
			});
			return (Task<InputStream>)task.AsyncState;
		}

		// Token: 0x06000555 RID: 1365 RVA: 0x0001BFE4 File Offset: 0x0001A1E4
		public static Task<OutputStream> GetOutputStreamAsync(this SmbFile smbFile, bool append = false)
		{
			Task task = new Task(delegate()
			{
				new SmbFileOutputStream(smbFile, append);
			});
			task.Start();
			Task.WaitAll(new Task[]
			{
				task
			});
			return (Task<OutputStream>)task.AsyncState;
		}
	}
}
