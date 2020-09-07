using System;
using SharpCifs.Util;
using SharpCifs.Util.Sharpen;
using SharpCifs.Util.Transport;

namespace SharpCifs.Smb
{
	// Token: 0x02000084 RID: 132
	public abstract class ServerMessageBlock : Response
	{
		// Token: 0x060003BA RID: 954 RVA: 0x0001080B File Offset: 0x0000EA0B
		internal static void WriteInt2(long val, byte[] dst, int dstIndex)
		{
			dst[dstIndex] = (byte)val;
			dst[++dstIndex] = (byte)(val >> 8);
		}

		// Token: 0x060003BB RID: 955 RVA: 0x0001081F File Offset: 0x0000EA1F
		internal static void WriteInt4(long val, byte[] dst, int dstIndex)
		{
			dst[dstIndex] = (byte)val;
			dst[++dstIndex] = (byte)(val >>= 8);
			dst[++dstIndex] = (byte)(val >>= 8);
			dst[++dstIndex] = (byte)(val >> 8);
		}

		// Token: 0x060003BC RID: 956 RVA: 0x00010854 File Offset: 0x0000EA54
		internal static int ReadInt2(byte[] src, int srcIndex)
		{
			return (int)(src[srcIndex] & byte.MaxValue) + ((int)(src[srcIndex + 1] & byte.MaxValue) << 8);
		}

		// Token: 0x060003BD RID: 957 RVA: 0x00010880 File Offset: 0x0000EA80
		internal static int ReadInt4(byte[] src, int srcIndex)
		{
			return (int)(src[srcIndex] & byte.MaxValue) + ((int)(src[srcIndex + 1] & byte.MaxValue) << 8) + ((int)(src[srcIndex + 2] & byte.MaxValue) << 16) + ((int)(src[srcIndex + 3] & byte.MaxValue) << 24);
		}

		// Token: 0x060003BE RID: 958 RVA: 0x000108C8 File Offset: 0x0000EAC8
		internal static long ReadInt8(byte[] src, int srcIndex)
		{
			return ((long)ServerMessageBlock.ReadInt4(src, srcIndex) & (long)(-1)) + ((long)ServerMessageBlock.ReadInt4(src, srcIndex + 4) << 32);
		}

		// Token: 0x060003BF RID: 959 RVA: 0x000108F4 File Offset: 0x0000EAF4
		internal static void WriteInt8(long val, byte[] dst, int dstIndex)
		{
			dst[dstIndex] = (byte)val;
			dst[++dstIndex] = (byte)(val >>= 8);
			dst[++dstIndex] = (byte)(val >>= 8);
			dst[++dstIndex] = (byte)(val >>= 8);
			dst[++dstIndex] = (byte)(val >>= 8);
			dst[++dstIndex] = (byte)(val >>= 8);
			dst[++dstIndex] = (byte)(val >>= 8);
			dst[++dstIndex] = (byte)(val >> 8);
		}

		// Token: 0x060003C0 RID: 960 RVA: 0x00010970 File Offset: 0x0000EB70
		internal static long ReadTime(byte[] src, int srcIndex)
		{
			int num = ServerMessageBlock.ReadInt4(src, srcIndex);
			int num2 = ServerMessageBlock.ReadInt4(src, srcIndex + 4);
			long num3 = (long)num2 << 32 | ((long)num & (long)(-1));
			return num3 / 10000L - SmbConstants.MillisecondsBetween1970And1601;
		}

		// Token: 0x060003C1 RID: 961 RVA: 0x000109B0 File Offset: 0x0000EBB0
		internal static void WriteTime(long t, byte[] dst, int dstIndex)
		{
			bool flag = t != 0L;
			if (flag)
			{
				t = (t + SmbConstants.MillisecondsBetween1970And1601) * 10000L;
			}
			ServerMessageBlock.WriteInt8(t, dst, dstIndex);
		}

		// Token: 0x060003C2 RID: 962 RVA: 0x000109E4 File Offset: 0x0000EBE4
		internal static long ReadUTime(byte[] buffer, int bufferIndex)
		{
			return (long)ServerMessageBlock.ReadInt4(buffer, bufferIndex) * 1000L;
		}

		// Token: 0x060003C3 RID: 963 RVA: 0x00010A08 File Offset: 0x0000EC08
		internal static void WriteUTime(long t, byte[] dst, int dstIndex)
		{
			bool flag = t == 0L || t == -1L;
			if (flag)
			{
				ServerMessageBlock.WriteInt4(-1L, dst, dstIndex);
			}
			else
			{
				ServerMessageBlock.WriteInt4((long)((int)(t / 1000L)), dst, dstIndex);
			}
		}

		// Token: 0x060003C4 RID: 964 RVA: 0x00010A44 File Offset: 0x0000EC44
		public ServerMessageBlock()
		{
			this.Flags = (byte)(SmbConstants.FlagsPathNamesCaseless | SmbConstants.FlagsPathNamesCanonicalized);
			this.Pid = SmbConstants.Pid;
			this.BatchLevel = 0;
		}

		// Token: 0x060003C5 RID: 965 RVA: 0x00010A81 File Offset: 0x0000EC81
		internal virtual void Reset()
		{
			this.Flags = (byte)(SmbConstants.FlagsPathNamesCaseless | SmbConstants.FlagsPathNamesCanonicalized);
			this.Flags2 = 0;
			this.ErrorCode = 0;
			this.Received = false;
			this.Digest = null;
		}

		// Token: 0x060003C6 RID: 966 RVA: 0x00010AB4 File Offset: 0x0000ECB4
		internal virtual int WriteString(string str, byte[] dst, int dstIndex)
		{
			return this.WriteString(str, dst, dstIndex, this.UseUnicode);
		}

		// Token: 0x060003C7 RID: 967 RVA: 0x00010AD8 File Offset: 0x0000ECD8
		internal virtual int WriteString(string str, byte[] dst, int dstIndex, bool useUnicode)
		{
			int num = dstIndex;
			try
			{
				if (useUnicode)
				{
					bool flag = (dstIndex - this.HeaderStart) % 2 != 0;
					if (flag)
					{
						dst[dstIndex++] = 0;
					}
					Array.Copy(Runtime.GetBytesForString(str, SmbConstants.UniEncoding), 0, dst, dstIndex, str.Length * 2);
					dstIndex += str.Length * 2;
					dst[dstIndex++] = 0;
					dst[dstIndex++] = 0;
				}
				else
				{
					byte[] bytesForString = Runtime.GetBytesForString(str, SmbConstants.OemEncoding);
					Array.Copy(bytesForString, 0, dst, dstIndex, bytesForString.Length);
					dstIndex += bytesForString.Length;
					dst[dstIndex++] = 0;
				}
			}
			catch (UnsupportedEncodingException ex)
			{
				bool flag2 = ServerMessageBlock.Log.Level > 1;
				if (flag2)
				{
					Runtime.PrintStackTrace(ex, ServerMessageBlock.Log);
				}
			}
			return dstIndex - num;
		}

		// Token: 0x060003C8 RID: 968 RVA: 0x00010BB8 File Offset: 0x0000EDB8
		internal virtual string ReadString(byte[] src, int srcIndex)
		{
			return this.ReadString(src, srcIndex, 256, this.UseUnicode);
		}

		// Token: 0x060003C9 RID: 969 RVA: 0x00010BE0 File Offset: 0x0000EDE0
		internal virtual string ReadString(byte[] src, int srcIndex, int maxLen, bool useUnicode)
		{
			int num = 0;
			string result = null;
			try
			{
				if (useUnicode)
				{
					bool flag = (srcIndex - this.HeaderStart) % 2 != 0;
					if (flag)
					{
						srcIndex++;
					}
					while (src[srcIndex + num] != 0 || src[srcIndex + num + 1] > 0)
					{
						num += 2;
						bool flag2 = num > maxLen;
						if (flag2)
						{
							bool flag3 = ServerMessageBlock.Log.Level > 0;
							if (flag3)
							{
								Hexdump.ToHexdump(Console.Error, src, srcIndex, (maxLen < 128) ? (maxLen + 8) : 128);
							}
							throw new RuntimeException("zero termination not found");
						}
					}
					result = Runtime.GetStringForBytes(src, srcIndex, num, SmbConstants.UniEncoding);
				}
				else
				{
					while (src[srcIndex + num] > 0)
					{
						num++;
						bool flag4 = num > maxLen;
						if (flag4)
						{
							bool flag5 = ServerMessageBlock.Log.Level > 0;
							if (flag5)
							{
								Hexdump.ToHexdump(Console.Error, src, srcIndex, (maxLen < 128) ? (maxLen + 8) : 128);
							}
							throw new RuntimeException("zero termination not found");
						}
					}
					result = Runtime.GetStringForBytes(src, srcIndex, num, SmbConstants.OemEncoding);
				}
			}
			catch (UnsupportedEncodingException ex)
			{
				bool flag6 = ServerMessageBlock.Log.Level > 1;
				if (flag6)
				{
					Runtime.PrintStackTrace(ex, ServerMessageBlock.Log);
				}
			}
			return result;
		}

		// Token: 0x060003CA RID: 970 RVA: 0x00010D50 File Offset: 0x0000EF50
		internal virtual string ReadString(byte[] src, int srcIndex, int srcEnd, int maxLen, bool useUnicode)
		{
			string result = null;
			try
			{
				if (useUnicode)
				{
					bool flag = (srcIndex - this.HeaderStart) % 2 != 0;
					if (flag)
					{
						srcIndex++;
					}
					int num = 0;
					while (srcIndex + num + 1 < srcEnd)
					{
						bool flag2 = src[srcIndex + num] == 0 && src[srcIndex + num + 1] == 0;
						if (flag2)
						{
							break;
						}
						bool flag3 = num > maxLen;
						if (flag3)
						{
							bool flag4 = ServerMessageBlock.Log.Level > 0;
							if (flag4)
							{
								Hexdump.ToHexdump(Console.Error, src, srcIndex, (maxLen < 128) ? (maxLen + 8) : 128);
							}
							throw new RuntimeException("zero termination not found");
						}
						num += 2;
					}
					result = Runtime.GetStringForBytes(src, srcIndex, num, SmbConstants.UniEncoding);
				}
				else
				{
					int num = 0;
					while (srcIndex < srcEnd)
					{
						bool flag5 = src[srcIndex + num] == 0;
						if (flag5)
						{
							break;
						}
						bool flag6 = num > maxLen;
						if (flag6)
						{
							bool flag7 = ServerMessageBlock.Log.Level > 0;
							if (flag7)
							{
								Hexdump.ToHexdump(Console.Error, src, srcIndex, (maxLen < 128) ? (maxLen + 8) : 128);
							}
							throw new RuntimeException("zero termination not found");
						}
						num++;
					}
					result = Runtime.GetStringForBytes(src, srcIndex, num, SmbConstants.OemEncoding);
				}
			}
			catch (UnsupportedEncodingException ex)
			{
				bool flag8 = ServerMessageBlock.Log.Level > 1;
				if (flag8)
				{
					Runtime.PrintStackTrace(ex, ServerMessageBlock.Log);
				}
			}
			return result;
		}

		// Token: 0x060003CB RID: 971 RVA: 0x00010EEC File Offset: 0x0000F0EC
		internal virtual int StringWireLength(string str, int offset)
		{
			int num = str.Length + 1;
			bool useUnicode = this.UseUnicode;
			if (useUnicode)
			{
				num = str.Length * 2 + 2;
				num = ((offset % 2 != 0) ? (num + 1) : num);
			}
			return num;
		}

		// Token: 0x060003CC RID: 972 RVA: 0x00010F2C File Offset: 0x0000F12C
		internal virtual int ReadStringLength(byte[] src, int srcIndex, int max)
		{
			int num = 0;
			while (src[srcIndex + num] > 0)
			{
				bool flag = num++ > max;
				if (flag)
				{
					throw new RuntimeException("zero termination not found: " + this);
				}
			}
			return num;
		}

		// Token: 0x060003CD RID: 973 RVA: 0x00010F70 File Offset: 0x0000F170
		internal virtual int Encode(byte[] dst, int dstIndex)
		{
			int num = this.HeaderStart = dstIndex;
			dstIndex += this.WriteHeaderWireFormat(dst, dstIndex);
			this.WordCount = this.WriteParameterWordsWireFormat(dst, dstIndex + 1);
			dst[dstIndex++] = (byte)(this.WordCount / 2 & 255);
			dstIndex += this.WordCount;
			this.WordCount /= 2;
			this.ByteCount = this.WriteBytesWireFormat(dst, dstIndex + 2);
			dst[dstIndex++] = (byte)(this.ByteCount & 255);
			dst[dstIndex++] = (byte)(this.ByteCount >> 8 & 255);
			dstIndex += this.ByteCount;
			this.Length = dstIndex - num;
			bool flag = this.Digest != null;
			if (flag)
			{
				this.Digest.Sign(dst, this.HeaderStart, this.Length, this, this.Response);
			}
			return this.Length;
		}

		// Token: 0x060003CE RID: 974 RVA: 0x0001105C File Offset: 0x0000F25C
		internal virtual int Decode(byte[] buffer, int bufferIndex)
		{
			int num = this.HeaderStart = bufferIndex;
			bufferIndex += this.ReadHeaderWireFormat(buffer, bufferIndex);
			this.WordCount = (int)buffer[bufferIndex++];
			bool flag = this.WordCount != 0;
			if (flag)
			{
				int num2;
				bool flag2 = (num2 = this.ReadParameterWordsWireFormat(buffer, bufferIndex)) != this.WordCount * 2;
				if (flag2)
				{
					bool flag3 = ServerMessageBlock.Log.Level >= 5;
					if (flag3)
					{
						ServerMessageBlock.Log.WriteLine(string.Concat(new object[]
						{
							"wordCount * 2=",
							this.WordCount * 2,
							" but readParameterWordsWireFormat returned ",
							num2
						}));
					}
				}
				bufferIndex += this.WordCount * 2;
			}
			this.ByteCount = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			bool flag4 = this.ByteCount != 0;
			if (flag4)
			{
				int num3;
				bool flag5 = (num3 = this.ReadBytesWireFormat(buffer, bufferIndex)) != this.ByteCount;
				if (flag5)
				{
					bool flag6 = ServerMessageBlock.Log.Level >= 5;
					if (flag6)
					{
						ServerMessageBlock.Log.WriteLine(string.Concat(new object[]
						{
							"byteCount=",
							this.ByteCount,
							" but readBytesWireFormat returned ",
							num3
						}));
					}
				}
				bufferIndex += this.ByteCount;
			}
			this.Length = bufferIndex - num;
			return this.Length;
		}

		// Token: 0x060003CF RID: 975 RVA: 0x000111DC File Offset: 0x0000F3DC
		internal virtual int WriteHeaderWireFormat(byte[] dst, int dstIndex)
		{
			Array.Copy(ServerMessageBlock.Header, 0, dst, dstIndex, ServerMessageBlock.Header.Length);
			dst[dstIndex + SmbConstants.CmdOffset] = this.Command;
			dst[dstIndex + SmbConstants.FlagsOffset] = this.Flags;
			ServerMessageBlock.WriteInt2((long)this.Flags2, dst, dstIndex + SmbConstants.FlagsOffset + 1);
			dstIndex += SmbConstants.TidOffset;
			ServerMessageBlock.WriteInt2((long)this.Tid, dst, dstIndex);
			ServerMessageBlock.WriteInt2((long)this.Pid, dst, dstIndex + 2);
			ServerMessageBlock.WriteInt2((long)this.Uid, dst, dstIndex + 4);
			ServerMessageBlock.WriteInt2((long)this.Mid, dst, dstIndex + 6);
			return SmbConstants.HeaderLength;
		}

		// Token: 0x060003D0 RID: 976 RVA: 0x00011288 File Offset: 0x0000F488
		internal virtual int ReadHeaderWireFormat(byte[] buffer, int bufferIndex)
		{
			this.Command = buffer[bufferIndex + SmbConstants.CmdOffset];
			this.ErrorCode = ServerMessageBlock.ReadInt4(buffer, bufferIndex + SmbConstants.ErrorCodeOffset);
			this.Flags = buffer[bufferIndex + SmbConstants.FlagsOffset];
			this.Flags2 = ServerMessageBlock.ReadInt2(buffer, bufferIndex + SmbConstants.FlagsOffset + 1);
			this.Tid = ServerMessageBlock.ReadInt2(buffer, bufferIndex + SmbConstants.TidOffset);
			this.Pid = ServerMessageBlock.ReadInt2(buffer, bufferIndex + SmbConstants.TidOffset + 2);
			this.Uid = ServerMessageBlock.ReadInt2(buffer, bufferIndex + SmbConstants.TidOffset + 4);
			this.Mid = ServerMessageBlock.ReadInt2(buffer, bufferIndex + SmbConstants.TidOffset + 6);
			return SmbConstants.HeaderLength;
		}

		// Token: 0x060003D1 RID: 977 RVA: 0x00011338 File Offset: 0x0000F538
		internal virtual bool IsResponse()
		{
			return ((int)this.Flags & SmbConstants.FlagsResponse) == SmbConstants.FlagsResponse;
		}

		// Token: 0x060003D2 RID: 978
		internal abstract int WriteParameterWordsWireFormat(byte[] dst, int dstIndex);

		// Token: 0x060003D3 RID: 979
		internal abstract int WriteBytesWireFormat(byte[] dst, int dstIndex);

		// Token: 0x060003D4 RID: 980
		internal abstract int ReadParameterWordsWireFormat(byte[] buffer, int bufferIndex);

		// Token: 0x060003D5 RID: 981
		internal abstract int ReadBytesWireFormat(byte[] buffer, int bufferIndex);

		// Token: 0x060003D6 RID: 982 RVA: 0x00011360 File Offset: 0x0000F560
		public override int GetHashCode()
		{
			return this.Mid;
		}

		// Token: 0x060003D7 RID: 983 RVA: 0x00011378 File Offset: 0x0000F578
		public override bool Equals(object obj)
		{
			return obj is ServerMessageBlock && ((ServerMessageBlock)obj).Mid == this.Mid;
		}

		// Token: 0x060003D8 RID: 984 RVA: 0x000113A8 File Offset: 0x0000F5A8
		public override string ToString()
		{
			byte command = this.Command;
			string text;
			if (command <= 16)
			{
				switch (command)
				{
				case 0:
					text = "SMB_COM_CREATE_DIRECTORY";
					goto IL_1CC;
				case 1:
					text = "SMB_COM_DELETE_DIRECTORY";
					goto IL_1CC;
				case 2:
				case 3:
				case 5:
					break;
				case 4:
					text = "SMB_COM_CLOSE";
					goto IL_1CC;
				case 6:
					text = "SMB_COM_DELETE";
					goto IL_1CC;
				case 7:
					text = "SMB_COM_RENAME";
					goto IL_1CC;
				case 8:
					text = "SMB_COM_QUERY_INFORMATION";
					goto IL_1CC;
				default:
					if (command == 16)
					{
						text = "SMB_COM_CHECK_DIRECTORY";
						goto IL_1CC;
					}
					break;
				}
			}
			else
			{
				switch (command)
				{
				case 37:
					text = "SMB_COM_TRANSACTION";
					goto IL_1CC;
				case 38:
					text = "SMB_COM_TRANSACTION_SECONDARY";
					goto IL_1CC;
				case 39:
				case 40:
				case 41:
				case 44:
				case 48:
				case 49:
				case 51:
					break;
				case 42:
					text = "SMB_COM_MOVE";
					goto IL_1CC;
				case 43:
					text = "SMB_COM_ECHO";
					goto IL_1CC;
				case 45:
					text = "SMB_COM_OPEN_ANDX";
					goto IL_1CC;
				case 46:
					text = "SMB_COM_READ_ANDX";
					goto IL_1CC;
				case 47:
					text = "SMB_COM_WRITE_ANDX";
					goto IL_1CC;
				case 50:
					text = "SMB_COM_TRANSACTION2";
					goto IL_1CC;
				case 52:
					text = "SMB_COM_FIND_CLOSE2";
					goto IL_1CC;
				default:
					switch (command)
					{
					case 113:
						text = "SMB_COM_TREE_DISCONNECT";
						goto IL_1CC;
					case 114:
						text = "SMB_COM_NEGOTIATE";
						goto IL_1CC;
					case 115:
						text = "SMB_COM_SESSION_SETUP_ANDX";
						goto IL_1CC;
					case 116:
						text = "SMB_COM_LOGOFF_ANDX";
						goto IL_1CC;
					case 117:
						text = "SMB_COM_TREE_CONNECT_ANDX";
						goto IL_1CC;
					default:
						switch (command)
						{
						case 160:
							text = "SMB_COM_NT_TRANSACT";
							goto IL_1CC;
						case 161:
							text = "SMB_COM_NT_TRANSACT_SECONDARY";
							goto IL_1CC;
						case 162:
							text = "SMB_COM_NT_CREATE_ANDX";
							goto IL_1CC;
						}
						break;
					}
					break;
				}
			}
			text = "UNKNOWN";
			IL_1CC:
			string text2 = (this.ErrorCode == 0) ? "0" : SmbException.GetMessageByCode(this.ErrorCode);
			return string.Concat(new object[]
			{
				"command=",
				text,
				",received=",
				this.Received.ToString(),
				",errorCode=",
				text2,
				",flags=0x",
				Hexdump.ToHexString((int)(this.Flags & byte.MaxValue), 4),
				",flags2=0x",
				Hexdump.ToHexString(this.Flags2, 4),
				",signSeq=",
				this.SignSeq,
				",tid=",
				this.Tid,
				",pid=",
				this.Pid,
				",uid=",
				this.Uid,
				",mid=",
				this.Mid,
				",wordCount=",
				this.WordCount,
				",byteCount=",
				this.ByteCount
			});
		}

		// Token: 0x04000149 RID: 329
		internal static LogStream Log = LogStream.GetInstance();

		// Token: 0x0400014A RID: 330
		internal static long Ticks1601 = new DateTime(1601, 1, 1).Ticks;

		// Token: 0x0400014B RID: 331
		internal static readonly byte[] Header = new byte[]
		{
			byte.MaxValue,
			83,
			77,
			66,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0
		};

		// Token: 0x0400014C RID: 332
		internal const byte SmbComCreateDirectory = 0;

		// Token: 0x0400014D RID: 333
		internal const byte SmbComDeleteDirectory = 1;

		// Token: 0x0400014E RID: 334
		internal const byte SmbComClose = 4;

		// Token: 0x0400014F RID: 335
		internal const byte SmbComDelete = 6;

		// Token: 0x04000150 RID: 336
		internal const byte SmbComRename = 7;

		// Token: 0x04000151 RID: 337
		internal const byte SmbComQueryInformation = 8;

		// Token: 0x04000152 RID: 338
		internal const byte SmbComWrite = 11;

		// Token: 0x04000153 RID: 339
		internal const byte SmbComCheckDirectory = 16;

		// Token: 0x04000154 RID: 340
		internal const byte SmbComTransaction = 37;

		// Token: 0x04000155 RID: 341
		internal const byte SmbComTransactionSecondary = 38;

		// Token: 0x04000156 RID: 342
		internal const byte SmbComMove = 42;

		// Token: 0x04000157 RID: 343
		internal const byte SmbComEcho = 43;

		// Token: 0x04000158 RID: 344
		internal const byte SmbComOpenAndx = 45;

		// Token: 0x04000159 RID: 345
		internal const byte SmbComReadAndx = 46;

		// Token: 0x0400015A RID: 346
		internal const byte SmbComWriteAndx = 47;

		// Token: 0x0400015B RID: 347
		internal const byte SmbComTransaction2 = 50;

		// Token: 0x0400015C RID: 348
		internal const byte SmbComFindClose2 = 52;

		// Token: 0x0400015D RID: 349
		internal const byte SmbComTreeDisconnect = 113;

		// Token: 0x0400015E RID: 350
		internal const byte SmbComNegotiate = 114;

		// Token: 0x0400015F RID: 351
		internal const byte SmbComSessionSetupAndx = 115;

		// Token: 0x04000160 RID: 352
		internal const byte SmbComLogoffAndx = 116;

		// Token: 0x04000161 RID: 353
		internal const byte SmbComTreeConnectAndx = 117;

		// Token: 0x04000162 RID: 354
		internal const byte SmbComNtTransact = 160;

		// Token: 0x04000163 RID: 355
		internal const byte SmbComNtTransactSecondary = 161;

		// Token: 0x04000164 RID: 356
		internal const byte SmbComNtCreateAndx = 162;

		// Token: 0x04000165 RID: 357
		internal byte Command;

		// Token: 0x04000166 RID: 358
		internal byte Flags;

		// Token: 0x04000167 RID: 359
		internal int HeaderStart;

		// Token: 0x04000168 RID: 360
		internal int Length;

		// Token: 0x04000169 RID: 361
		internal int BatchLevel;

		// Token: 0x0400016A RID: 362
		internal int ErrorCode;

		// Token: 0x0400016B RID: 363
		internal int Flags2;

		// Token: 0x0400016C RID: 364
		internal int Tid;

		// Token: 0x0400016D RID: 365
		internal int Pid;

		// Token: 0x0400016E RID: 366
		internal int Uid;

		// Token: 0x0400016F RID: 367
		internal int Mid;

		// Token: 0x04000170 RID: 368
		internal int WordCount;

		// Token: 0x04000171 RID: 369
		internal int ByteCount;

		// Token: 0x04000172 RID: 370
		internal bool UseUnicode;

		// Token: 0x04000173 RID: 371
		internal bool Received;

		// Token: 0x04000174 RID: 372
		internal bool ExtendedSecurity;

		// Token: 0x04000175 RID: 373
		internal long ResponseTimeout = 1L;

		// Token: 0x04000176 RID: 374
		internal int SignSeq;

		// Token: 0x04000177 RID: 375
		internal bool VerifyFailed;

		// Token: 0x04000178 RID: 376
		internal NtlmPasswordAuthentication Auth = null;

		// Token: 0x04000179 RID: 377
		internal string Path;

		// Token: 0x0400017A RID: 378
		internal SigningDigest Digest;

		// Token: 0x0400017B RID: 379
		internal ServerMessageBlock Response;
	}
}
