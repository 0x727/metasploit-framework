using System;
using System.IO;
using System.Text;
using SharpCifs.Util;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x020000B0 RID: 176
	public class SmbRandomAccessFile
	{
		// Token: 0x06000576 RID: 1398 RVA: 0x0001CC12 File Offset: 0x0001AE12
		public SmbRandomAccessFile(string url, string mode, int shareAccess) : this(new SmbFile(url, string.Empty, null, shareAccess), mode)
		{
		}

		// Token: 0x06000577 RID: 1399 RVA: 0x0001CC2C File Offset: 0x0001AE2C
		public SmbRandomAccessFile(SmbFile file, string mode)
		{
			this._file = file;
			bool flag = mode.Equals("r");
			if (flag)
			{
				this._openFlags = 17;
			}
			else
			{
				bool flag2 = mode.Equals("rw");
				if (!flag2)
				{
					throw new ArgumentException("Invalid mode");
				}
				this._openFlags = 23;
				this._writeAndxResp = new SmbComWriteAndXResponse();
				this._options = 2114;
				this._access = (SmbConstants.FileReadData | SmbConstants.FileWriteData);
			}
			file.Open(this._openFlags, this._access, 128, this._options);
			this._readSize = file.Tree.Session.transport.RcvBufSize - 70;
			this._writeSize = file.Tree.Session.transport.SndBufSize - 70;
			this._fp = 0L;
		}

		// Token: 0x06000578 RID: 1400 RVA: 0x0001CD24 File Offset: 0x0001AF24
		public virtual int Read()
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

		// Token: 0x06000579 RID: 1401 RVA: 0x0001CD60 File Offset: 0x0001AF60
		public virtual int Read(byte[] b)
		{
			return this.Read(b, 0, b.Length);
		}

		// Token: 0x0600057A RID: 1402 RVA: 0x0001CD80 File Offset: 0x0001AF80
		public virtual int Read(byte[] b, int off, int len)
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
				bool flag2 = !this._file.IsOpen();
				if (flag2)
				{
					this._file.Open(this._openFlags, 0, 128, this._options);
				}
				SmbComReadAndXResponse smbComReadAndXResponse = new SmbComReadAndXResponse(b, off);
				for (;;)
				{
					int num = (len > this._readSize) ? this._readSize : len;
					this._file.Send(new SmbComReadAndX(this._file.Fid, this._fp, num, null), smbComReadAndXResponse);
					int dataLength;
					bool flag3 = (dataLength = smbComReadAndXResponse.DataLength) <= 0;
					if (flag3)
					{
						break;
					}
					this._fp += (long)dataLength;
					len -= dataLength;
					smbComReadAndXResponse.Off += dataLength;
					if (len <= 0 || dataLength != num)
					{
						goto Block_7;
					}
				}
				return (int)((this._fp - fp > 0L) ? (this._fp - fp) : -1L);
				Block_7:
				result = (int)(this._fp - fp);
			}
			return result;
		}

		// Token: 0x0600057B RID: 1403 RVA: 0x0001CE98 File Offset: 0x0001B098
		public void ReadFully(byte[] b)
		{
			this.ReadFully(b, 0, b.Length);
		}

		// Token: 0x0600057C RID: 1404 RVA: 0x0001CEA8 File Offset: 0x0001B0A8
		public void ReadFully(byte[] b, int off, int len)
		{
			int num = 0;
			for (;;)
			{
				int num2 = this.Read(b, off + num, len - num);
				bool flag = num2 < 0;
				if (flag)
				{
					break;
				}
				num += num2;
				this._fp += (long)num2;
				if (num >= len)
				{
					return;
				}
			}
			throw new SmbException("EOF");
		}

		// Token: 0x0600057D RID: 1405 RVA: 0x0001CEF8 File Offset: 0x0001B0F8
		public virtual int SkipBytes(int n)
		{
			bool flag = n > 0;
			int result;
			if (flag)
			{
				this._fp += (long)n;
				result = n;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x0600057E RID: 1406 RVA: 0x0001CF27 File Offset: 0x0001B127
		public virtual void Write(int b)
		{
			this._tmp[0] = (byte)b;
			this.Write(this._tmp, 0, 1);
		}

		// Token: 0x0600057F RID: 1407 RVA: 0x0001CF43 File Offset: 0x0001B143
		public virtual void Write(byte[] b)
		{
			this.Write(b, 0, b.Length);
		}

		// Token: 0x06000580 RID: 1408 RVA: 0x0001CF54 File Offset: 0x0001B154
		public virtual void Write(byte[] b, int off, int len)
		{
			bool flag = len <= 0;
			if (!flag)
			{
				bool flag2 = !this._file.IsOpen();
				if (flag2)
				{
					this._file.Open(this._openFlags, 0, 128, this._options);
				}
				do
				{
					int num = (len > this._writeSize) ? this._writeSize : len;
					this._file.Send(new SmbComWriteAndX(this._file.Fid, this._fp, len - num, b, off, num, null), this._writeAndxResp);
					this._fp += this._writeAndxResp.Count;
					len -= (int)this._writeAndxResp.Count;
					off += (int)this._writeAndxResp.Count;
				}
				while (len > 0);
			}
		}

		// Token: 0x06000581 RID: 1409 RVA: 0x0001D02C File Offset: 0x0001B22C
		public virtual long GetFilePointer()
		{
			return this._fp;
		}

		// Token: 0x06000582 RID: 1410 RVA: 0x0001D044 File Offset: 0x0001B244
		public virtual void Seek(long pos)
		{
			this._fp = pos;
		}

		// Token: 0x06000583 RID: 1411 RVA: 0x0001D050 File Offset: 0x0001B250
		public virtual long Length()
		{
			return this._file.Length();
		}

		// Token: 0x06000584 RID: 1412 RVA: 0x0001D070 File Offset: 0x0001B270
		public virtual void SetLength(long newLength)
		{
			bool flag = !this._file.IsOpen();
			if (flag)
			{
				this._file.Open(this._openFlags, 0, 128, this._options);
			}
			SmbComWriteResponse response = new SmbComWriteResponse();
			this._file.Send(new SmbComWrite(this._file.Fid, (int)(newLength & (long)(-1)), 0, this._tmp, 0, 0), response);
		}

		// Token: 0x06000585 RID: 1413 RVA: 0x0001D0E1 File Offset: 0x0001B2E1
		public virtual void Close()
		{
			this._file.Close();
		}

		// Token: 0x06000586 RID: 1414 RVA: 0x0001D0F0 File Offset: 0x0001B2F0
		public bool ReadBoolean()
		{
			bool flag = this.Read(this._tmp, 0, 1) < 0;
			if (flag)
			{
				throw new SmbException("EOF");
			}
			return this._tmp[0] > 0;
		}

		// Token: 0x06000587 RID: 1415 RVA: 0x0001D130 File Offset: 0x0001B330
		public byte ReadByte()
		{
			bool flag = this.Read(this._tmp, 0, 1) < 0;
			if (flag)
			{
				throw new SmbException("EOF");
			}
			return this._tmp[0];
		}

		// Token: 0x06000588 RID: 1416 RVA: 0x0001D16C File Offset: 0x0001B36C
		public int ReadUnsignedByte()
		{
			bool flag = this.Read(this._tmp, 0, 1) < 0;
			if (flag)
			{
				throw new SmbException("EOF");
			}
			return (int)(this._tmp[0] & byte.MaxValue);
		}

		// Token: 0x06000589 RID: 1417 RVA: 0x0001D1B0 File Offset: 0x0001B3B0
		public short ReadShort()
		{
			bool flag = this.Read(this._tmp, 0, 2) < 0;
			if (flag)
			{
				throw new SmbException("EOF");
			}
			return Encdec.Dec_uint16be(this._tmp, 0);
		}

		// Token: 0x0600058A RID: 1418 RVA: 0x0001D1F0 File Offset: 0x0001B3F0
		public int ReadUnsignedShort()
		{
			bool flag = this.Read(this._tmp, 0, 2) < 0;
			if (flag)
			{
				throw new SmbException("EOF");
			}
			return (int)Encdec.Dec_uint16be(this._tmp, 0) & 65535;
		}

		// Token: 0x0600058B RID: 1419 RVA: 0x0001D238 File Offset: 0x0001B438
		public char ReadChar()
		{
			bool flag = this.Read(this._tmp, 0, 2) < 0;
			if (flag)
			{
				throw new SmbException("EOF");
			}
			return (char)Encdec.Dec_uint16be(this._tmp, 0);
		}

		// Token: 0x0600058C RID: 1420 RVA: 0x0001D278 File Offset: 0x0001B478
		public int ReadInt()
		{
			bool flag = this.Read(this._tmp, 0, 4) < 0;
			if (flag)
			{
				throw new SmbException("EOF");
			}
			return Encdec.Dec_uint32be(this._tmp, 0);
		}

		// Token: 0x0600058D RID: 1421 RVA: 0x0001D2B8 File Offset: 0x0001B4B8
		public long ReadLong()
		{
			bool flag = this.Read(this._tmp, 0, 8) < 0;
			if (flag)
			{
				throw new SmbException("EOF");
			}
			return Encdec.Dec_uint64be(this._tmp, 0);
		}

		// Token: 0x0600058E RID: 1422 RVA: 0x0001D2F8 File Offset: 0x0001B4F8
		public float ReadFloat()
		{
			bool flag = this.Read(this._tmp, 0, 4) < 0;
			if (flag)
			{
				throw new SmbException("EOF");
			}
			return Encdec.Dec_floatbe(this._tmp, 0);
		}

		// Token: 0x0600058F RID: 1423 RVA: 0x0001D338 File Offset: 0x0001B538
		public double ReadDouble()
		{
			bool flag = this.Read(this._tmp, 0, 8) < 0;
			if (flag)
			{
				throw new SmbException("EOF");
			}
			return Encdec.Dec_doublebe(this._tmp, 0);
		}

		// Token: 0x06000590 RID: 1424 RVA: 0x0001D378 File Offset: 0x0001B578
		public string ReadLine()
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = -1;
			bool flag = false;
			while (!flag)
			{
				int num2;
				num = (num2 = this.Read());
				if (num2 != -1 && num2 != 10)
				{
					if (num2 != 13)
					{
						stringBuilder.Append((char)num);
					}
					else
					{
						flag = true;
						long fp = this._fp;
						bool flag2 = this.Read() != 10;
						if (flag2)
						{
							this._fp = fp;
						}
					}
				}
				else
				{
					flag = true;
				}
			}
			bool flag3 = num == -1 && stringBuilder.Length == 0;
			string result;
			if (flag3)
			{
				result = null;
			}
			else
			{
				result = stringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x06000591 RID: 1425 RVA: 0x0001D420 File Offset: 0x0001B620
		public string ReadUtf()
		{
			int num = this.ReadUnsignedShort();
			byte[] array = new byte[num];
			this.Read(array, 0, num);
			string result;
			try
			{
				result = Encdec.Dec_utf8(array, 0, num);
			}
			catch (IOException rootCause)
			{
				throw new SmbException(string.Empty, rootCause);
			}
			return result;
		}

		// Token: 0x06000592 RID: 1426 RVA: 0x0001D474 File Offset: 0x0001B674
		public void WriteBoolean(bool v)
		{
			this._tmp[0] = (byte)(v ? 1 : 0);
			this.Write(this._tmp, 0, 1);
		}

		// Token: 0x06000593 RID: 1427 RVA: 0x0001CF27 File Offset: 0x0001B127
		public void WriteByte(int v)
		{
			this._tmp[0] = (byte)v;
			this.Write(this._tmp, 0, 1);
		}

		// Token: 0x06000594 RID: 1428 RVA: 0x0001D496 File Offset: 0x0001B696
		public void WriteShort(int v)
		{
			Encdec.Enc_uint16be((short)v, this._tmp, 0);
			this.Write(this._tmp, 0, 2);
		}

		// Token: 0x06000595 RID: 1429 RVA: 0x0001D496 File Offset: 0x0001B696
		public void WriteChar(int v)
		{
			Encdec.Enc_uint16be((short)v, this._tmp, 0);
			this.Write(this._tmp, 0, 2);
		}

		// Token: 0x06000596 RID: 1430 RVA: 0x0001D4B7 File Offset: 0x0001B6B7
		public void WriteInt(int v)
		{
			Encdec.Enc_uint32be(v, this._tmp, 0);
			this.Write(this._tmp, 0, 4);
		}

		// Token: 0x06000597 RID: 1431 RVA: 0x0001D4D7 File Offset: 0x0001B6D7
		public void WriteLong(long v)
		{
			Encdec.Enc_uint64be(v, this._tmp, 0);
			this.Write(this._tmp, 0, 8);
		}

		// Token: 0x06000598 RID: 1432 RVA: 0x0001D4F7 File Offset: 0x0001B6F7
		public void WriteFloat(float v)
		{
			Encdec.Enc_floatbe(v, this._tmp, 0);
			this.Write(this._tmp, 0, 4);
		}

		// Token: 0x06000599 RID: 1433 RVA: 0x0001D517 File Offset: 0x0001B717
		public void WriteDouble(double v)
		{
			Encdec.Enc_doublebe(v, this._tmp, 0);
			this.Write(this._tmp, 0, 8);
		}

		// Token: 0x0600059A RID: 1434 RVA: 0x0001D538 File Offset: 0x0001B738
		public void WriteBytes(string s)
		{
			byte[] bytesForString = Runtime.GetBytesForString(s);
			this.Write(bytesForString, 0, bytesForString.Length);
		}

		// Token: 0x0600059B RID: 1435 RVA: 0x0001D55C File Offset: 0x0001B75C
		public void WriteUtf(string str)
		{
			int length = str.Length;
			int num = 0;
			for (int i = 0; i < length; i++)
			{
				int num2 = (int)str[i];
				num += ((num2 > 127) ? ((num2 > 2047) ? 3 : 2) : 1);
			}
			byte[] array = new byte[num];
			this.WriteShort(num);
			try
			{
				Encdec.Enc_utf8(str, array, 0, num);
			}
			catch (IOException rootCause)
			{
				throw new SmbException(string.Empty, rootCause);
			}
			this.Write(array, 0, num);
		}

		// Token: 0x0400033C RID: 828
		private const int WriteOptions = 2114;

		// Token: 0x0400033D RID: 829
		private SmbFile _file;

		// Token: 0x0400033E RID: 830
		private long _fp;

		// Token: 0x0400033F RID: 831
		private int _openFlags;

		// Token: 0x04000340 RID: 832
		private int _access;

		// Token: 0x04000341 RID: 833
		private int _readSize;

		// Token: 0x04000342 RID: 834
		private int _writeSize;

		// Token: 0x04000343 RID: 835
		private int _ch;

		// Token: 0x04000344 RID: 836
		private int _options;

		// Token: 0x04000345 RID: 837
		private byte[] _tmp = new byte[8];

		// Token: 0x04000346 RID: 838
		private SmbComWriteAndXResponse _writeAndxResp;
	}
}
