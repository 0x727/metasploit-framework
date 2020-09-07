using System;
using System.IO;
using LumiSoft.Net.IO;

namespace LumiSoft.Net.Mail
{
	// Token: 0x02000178 RID: 376
	public class Mail_t_Attachment
	{
		// Token: 0x06000F4F RID: 3919 RVA: 0x0005F0C7 File Offset: 0x0005E0C7
		public Mail_t_Attachment(string fileName) : this(null, fileName)
		{
		}

		// Token: 0x06000F50 RID: 3920 RVA: 0x0005F0D3 File Offset: 0x0005E0D3
		public Mail_t_Attachment(string fileName, bool zipCompress) : this(null, fileName, zipCompress)
		{
		}

		// Token: 0x06000F51 RID: 3921 RVA: 0x0005F0E0 File Offset: 0x0005E0E0
		public Mail_t_Attachment(string attachmentName, string fileName) : this(null, fileName, false)
		{
		}

		// Token: 0x06000F52 RID: 3922 RVA: 0x0005F0F0 File Offset: 0x0005E0F0
		public Mail_t_Attachment(string attachmentName, string fileName, bool zipCompress)
		{
			this.m_Name = null;
			this.m_FileName = null;
			this.m_ZipCompress = false;
			this.m_CloseStream = true;
			this.m_pStream = null;
			//base..ctor();
			bool flag = fileName == null;
			if (flag)
			{
				throw new ArgumentNullException("fileName");
			}
			bool flag2 = string.IsNullOrEmpty(fileName);
			if (flag2)
			{
				throw new ArgumentException("Argument 'fileName' value must be specified.", "fileName");
			}
			bool flag3 = string.IsNullOrEmpty(attachmentName);
			if (flag3)
			{
				this.m_Name = Path.GetFileName(fileName);
			}
			else
			{
				this.m_Name = attachmentName;
			}
			this.m_FileName = fileName;
			this.m_ZipCompress = zipCompress;
			this.m_CloseStream = true;
		}

		// Token: 0x06000F53 RID: 3923 RVA: 0x0005F18F File Offset: 0x0005E18F
		public Mail_t_Attachment(string attachmentName, byte[] data) : this(attachmentName, data, false)
		{
		}

		// Token: 0x06000F54 RID: 3924 RVA: 0x0005F19C File Offset: 0x0005E19C
		public Mail_t_Attachment(string attachmentName, byte[] data, bool zipCompress)
		{
			this.m_Name = null;
			this.m_FileName = null;
			this.m_ZipCompress = false;
			this.m_CloseStream = true;
			this.m_pStream = null;
			//base..ctor();
			bool flag = attachmentName == null;
			if (flag)
			{
				throw new ArgumentNullException("attachmentName");
			}
			bool flag2 = data == null;
			if (flag2)
			{
				throw new ArgumentNullException("data");
			}
			this.m_Name = attachmentName;
			this.m_pStream = new MemoryStream(data);
		}

		// Token: 0x06000F55 RID: 3925 RVA: 0x0005F20F File Offset: 0x0005E20F
		public Mail_t_Attachment(string attachmentName, Stream stream) : this(attachmentName, stream, false)
		{
		}

		// Token: 0x06000F56 RID: 3926 RVA: 0x0005F21C File Offset: 0x0005E21C
		public Mail_t_Attachment(string attachmentName, Stream stream, bool zipCompress)
		{
			this.m_Name = null;
			this.m_FileName = null;
			this.m_ZipCompress = false;
			this.m_CloseStream = true;
			this.m_pStream = null;
			//base..ctor();
			bool flag = attachmentName == null;
			if (flag)
			{
				throw new ArgumentNullException("attachmentName");
			}
			bool flag2 = stream == null;
			if (flag2)
			{
				throw new ArgumentNullException("stream");
			}
			this.m_Name = attachmentName;
			this.m_pStream = stream;
		}

		// Token: 0x06000F57 RID: 3927 RVA: 0x0005F28C File Offset: 0x0005E28C
		internal Stream GetStream()
		{
			bool flag = this.m_pStream == null;
			if (flag)
			{
				this.m_pStream = File.OpenRead(this.m_FileName);
			}
			bool zipCompress = this.m_ZipCompress;
			Stream result;
			if (zipCompress)
			{
				MemoryStreamEx memoryStreamEx = new MemoryStreamEx();
				memoryStreamEx.Position = 0L;
				this.CloseStream();
				result = memoryStreamEx;
			}
			else
			{
				result = this.m_pStream;
			}
			return result;
		}

		// Token: 0x06000F58 RID: 3928 RVA: 0x0005F2EC File Offset: 0x0005E2EC
		internal void CloseStream()
		{
			bool closeStream = this.m_CloseStream;
			if (closeStream)
			{
				this.m_pStream.Dispose();
			}
		}

		// Token: 0x17000518 RID: 1304
		// (get) Token: 0x06000F59 RID: 3929 RVA: 0x0005F314 File Offset: 0x0005E314
		public string Name
		{
			get
			{
				bool zipCompress = this.m_ZipCompress;
				string result;
				if (zipCompress)
				{
					result = Path.GetFileNameWithoutExtension(this.m_Name) + ".zip";
				}
				else
				{
					result = this.m_Name;
				}
				return result;
			}
		}

		// Token: 0x0400065D RID: 1629
		private string m_Name;

		// Token: 0x0400065E RID: 1630
		private string m_FileName;

		// Token: 0x0400065F RID: 1631
		private bool m_ZipCompress;

		// Token: 0x04000660 RID: 1632
		private bool m_CloseStream;

		// Token: 0x04000661 RID: 1633
		private Stream m_pStream;
	}
}
