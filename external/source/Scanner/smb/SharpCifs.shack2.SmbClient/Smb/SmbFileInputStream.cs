using System;
using System.IO;
using SharpCifs.Util.Sharpen;
using SharpCifs.Util.Transport;

namespace SharpCifs.Smb
{
	// Token: 0x020000AC RID: 172
	public class SmbFileInputStream : InputStream
	{
		// Token: 0x06000557 RID: 1367 RVA: 0x0001C03E File Offset: 0x0001A23E
		public SmbFileInputStream(string url) : this(new SmbFile(url))
		{
		}

		// Token: 0x06000558 RID: 1368 RVA: 0x0001C04E File Offset: 0x0001A24E
		public SmbFileInputStream(SmbFile file) : this(file, 1)
		{
		}

		// Token: 0x06000559 RID: 1369 RVA: 0x0001C05C File Offset: 0x0001A25C
		internal SmbFileInputStream(SmbFile file, int openFlags)
		{
			this.File = file;
			this._openFlags = (openFlags & 65535);
			this._access = (int)((uint)openFlags >> 16 & 65535U);
			bool flag = file.Type != 16;
			if (flag)
			{
				file.Open(openFlags, this._access, 128, 0);
				this._openFlags &= -81;
			}
			else
			{
				file.Connect0();
			}
			this._readSize = Math.Min(file.Tree.Session.transport.RcvBufSize - 70, file.Tree.Session.transport.Server.MaxBufferSize - 70);
		}

		// Token: 0x0600055A RID: 1370 RVA: 0x0001C124 File Offset: 0x0001A324
		protected internal virtual IOException SeToIoe(SmbException se)
		{
			IOException ex = se;
			Exception rootCause = se.GetRootCause();
			bool flag = rootCause is TransportException;
			if (flag)
			{
				ex = (TransportException)rootCause;
				rootCause = ((TransportException)ex).GetRootCause();
			}
			bool flag2 = rootCause != null;
			if (flag2)
			{
				ex = new IOException(rootCause.Message);
				ex.InitCause(rootCause);
			}
			return ex;
		}

		// Token: 0x0600055B RID: 1371 RVA: 0x0001C184 File Offset: 0x0001A384
		public override void Close()
		{
			try
			{
				this.File.Close();
				this._tmp = null;
			}
			catch (SmbException se)
			{
				throw this.SeToIoe(se);
			}
		}

		// Token: 0x0600055C RID: 1372 RVA: 0x0001C1C4 File Offset: 0x0001A3C4
		public override int Read()
		{
			bool flag = this.Read(this._tmp, 0, 1) == -1;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				result = (int)(this._tmp[0] & byte.MaxValue);
			}
			return result;
		}

		// Token: 0x0600055D RID: 1373 RVA: 0x0001C200 File Offset: 0x0001A400
		public override int Read(byte[] b)
		{
			return this.Read(b, 0, b.Length);
		}

		// Token: 0x0600055E RID: 1374 RVA: 0x0001C220 File Offset: 0x0001A420
		public override int Read(byte[] b, int off, int len)
		{
			return this.ReadDirect(b, off, len);
		}

		// Token: 0x0600055F RID: 1375 RVA: 0x0001C23C File Offset: 0x0001A43C
		public virtual int ReadDirect(byte[] b, int off, int len)
		{
			bool flag = len <= 0;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				long fp = this._fp;
				bool flag2 = this._tmp == null;
				if (flag2)
				{
					throw new IOException("Bad file descriptor");
				}
				this.File.Open(this._openFlags, this._access, 128, 0);
				bool flag3 = this.File.Log.Level >= 4;
				if (flag3)
				{
					this.File.Log.WriteLine(string.Concat(new object[]
					{
						"read: fid=",
						this.File.Fid,
						",off=",
						off,
						",len=",
						len
					}));
				}
				SmbComReadAndXResponse smbComReadAndXResponse = new SmbComReadAndXResponse(b, off);
				bool flag4 = this.File.Type == 16;
				if (flag4)
				{
					smbComReadAndXResponse.ResponseTimeout = 0L;
				}
				for (;;)
				{
					int num = (len > this._readSize) ? this._readSize : len;
					bool flag5 = this.File.Log.Level >= 4;
					if (flag5)
					{
						this.File.Log.WriteLine(string.Concat(new object[]
						{
							"read: len=",
							len,
							",r=",
							num,
							",fp=",
							this._fp
						}));
					}
					try
					{
						SmbComReadAndX smbComReadAndX = new SmbComReadAndX(this.File.Fid, this._fp, num, null);
						bool flag6 = this.File.Type == 16;
						if (flag6)
						{
							smbComReadAndX.MinCount = (smbComReadAndX.MaxCount = (smbComReadAndX.Remaining = 1024));
						}
						this.File.Send(smbComReadAndX, smbComReadAndXResponse);
					}
					catch (SmbException ex)
					{
						bool flag7 = this.File.Type == 16 && ex.GetNtStatus() == -1073741493;
						if (flag7)
						{
							return -1;
						}
						throw this.SeToIoe(ex);
					}
					int dataLength;
					bool flag8 = (dataLength = smbComReadAndXResponse.DataLength) <= 0;
					if (flag8)
					{
						break;
					}
					this._fp += (long)dataLength;
					len -= dataLength;
					smbComReadAndXResponse.Off += dataLength;
					if (len <= 0 || dataLength != num)
					{
						goto Block_11;
					}
				}
				return (int)((this._fp - fp > 0L) ? (this._fp - fp) : -1L);
				Block_11:
				result = (int)(this._fp - fp);
			}
			return result;
		}

		// Token: 0x06000560 RID: 1376 RVA: 0x0001C4F0 File Offset: 0x0001A6F0
		public override int Available()
		{
			bool flag = this.File.Type != 16;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				try
				{
					SmbNamedPipe smbNamedPipe = (SmbNamedPipe)this.File;
					this.File.Open(32, smbNamedPipe.PipeType & 16711680, 128, 0);
					TransPeekNamedPipe request = new TransPeekNamedPipe(this.File.Unc, this.File.Fid);
					TransPeekNamedPipeResponse transPeekNamedPipeResponse = new TransPeekNamedPipeResponse(smbNamedPipe);
					smbNamedPipe.Send(request, transPeekNamedPipeResponse);
					bool flag2 = transPeekNamedPipeResponse.status == 1 || transPeekNamedPipeResponse.status == 4;
					if (flag2)
					{
						this.File.Opened = false;
						result = 0;
					}
					else
					{
						result = transPeekNamedPipeResponse.Available;
					}
				}
				catch (SmbException se)
				{
					throw this.SeToIoe(se);
				}
			}
			return result;
		}

		// Token: 0x06000561 RID: 1377 RVA: 0x0001C5CC File Offset: 0x0001A7CC
		public override long Skip(long n)
		{
			bool flag = n > 0L;
			long result;
			if (flag)
			{
				this._fp += n;
				result = n;
			}
			else
			{
				result = 0L;
			}
			return result;
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000562 RID: 1378 RVA: 0x0001C5FC File Offset: 0x0001A7FC
		public override long Length
		{
			get
			{
				return this.File.Length();
			}
		}

		// Token: 0x04000321 RID: 801
		private long _fp;

		// Token: 0x04000322 RID: 802
		private int _readSize;

		// Token: 0x04000323 RID: 803
		private int _openFlags;

		// Token: 0x04000324 RID: 804
		private int _access;

		// Token: 0x04000325 RID: 805
		private byte[] _tmp = new byte[1];

		// Token: 0x04000326 RID: 806
		internal SmbFile File;
	}
}
