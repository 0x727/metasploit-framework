using System;
using System.Net;
using SharpCifs.Util;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Netbios
{
	// Token: 0x020000D5 RID: 213
	internal abstract class NameServicePacket
	{
		// Token: 0x060006FE RID: 1790 RVA: 0x000265AA File Offset: 0x000247AA
		internal static void WriteInt2(int val, byte[] dst, int dstIndex)
		{
			dst[dstIndex++] = (byte)(val >> 8 & 255);
			dst[dstIndex] = (byte)(val & 255);
		}

		// Token: 0x060006FF RID: 1791 RVA: 0x000265CC File Offset: 0x000247CC
		internal static void WriteInt4(int val, byte[] dst, int dstIndex)
		{
			dst[dstIndex++] = (byte)(val >> 24 & 255);
			dst[dstIndex++] = (byte)(val >> 16 & 255);
			dst[dstIndex++] = (byte)(val >> 8 & 255);
			dst[dstIndex] = (byte)(val & 255);
		}

		// Token: 0x06000700 RID: 1792 RVA: 0x00026620 File Offset: 0x00024820
		internal static int ReadInt2(byte[] src, int srcIndex)
		{
			return ((int)(src[srcIndex] & byte.MaxValue) << 8) + (int)(src[srcIndex + 1] & byte.MaxValue);
		}

		// Token: 0x06000701 RID: 1793 RVA: 0x0002664C File Offset: 0x0002484C
		internal static int ReadInt4(byte[] src, int srcIndex)
		{
			return ((int)(src[srcIndex] & byte.MaxValue) << 24) + ((int)(src[srcIndex + 1] & byte.MaxValue) << 16) + ((int)(src[srcIndex + 2] & byte.MaxValue) << 8) + (int)(src[srcIndex + 3] & byte.MaxValue);
		}

		// Token: 0x06000702 RID: 1794 RVA: 0x00026694 File Offset: 0x00024894
		internal static int ReadNameTrnId(byte[] src, int srcIndex)
		{
			return NameServicePacket.ReadInt2(src, srcIndex);
		}

		// Token: 0x06000703 RID: 1795 RVA: 0x000266AD File Offset: 0x000248AD
		public NameServicePacket()
		{
			this.IsRecurDesired = true;
			this.IsBroadcast = true;
			this.QuestionCount = 1;
			this.QuestionClass = 1;
		}

		// Token: 0x06000704 RID: 1796 RVA: 0x000266D4 File Offset: 0x000248D4
		internal virtual int WriteWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			dstIndex += this.WriteHeaderWireFormat(dst, dstIndex);
			dstIndex += this.WriteBodyWireFormat(dst, dstIndex);
			return dstIndex - num;
		}

		// Token: 0x06000705 RID: 1797 RVA: 0x00026704 File Offset: 0x00024904
		internal virtual int ReadWireFormat(byte[] src, int srcIndex)
		{
			int num = srcIndex;
			srcIndex += this.ReadHeaderWireFormat(src, srcIndex);
			srcIndex += this.ReadBodyWireFormat(src, srcIndex);
			return srcIndex - num;
		}

		// Token: 0x06000706 RID: 1798 RVA: 0x00026734 File Offset: 0x00024934
		internal virtual int WriteHeaderWireFormat(byte[] dst, int dstIndex)
		{
			NameServicePacket.WriteInt2(this.NameTrnId, dst, dstIndex);
			dst[dstIndex + 2] = (byte)((this.IsResponse ? 128 : 0) + (this.OpCode << 3 & 120) + (this.IsAuthAnswer ? 4 : 0) + (this.IsTruncated ? 2 : 0) + (this.IsRecurDesired ? 1 : 0));
			dst[dstIndex + 2 + 1] = (byte)((this.IsRecurAvailable ? 128 : 0) + (this.IsBroadcast ? 16 : 0) + (this.ResultCode & 15));
			NameServicePacket.WriteInt2(this.QuestionCount, dst, dstIndex + 4);
			NameServicePacket.WriteInt2(this.AnswerCount, dst, dstIndex + 6);
			NameServicePacket.WriteInt2(this.AuthorityCount, dst, dstIndex + 8);
			NameServicePacket.WriteInt2(this.AdditionalCount, dst, dstIndex + 10);
			return 12;
		}

		// Token: 0x06000707 RID: 1799 RVA: 0x00026814 File Offset: 0x00024A14
		internal virtual int ReadHeaderWireFormat(byte[] src, int srcIndex)
		{
			this.NameTrnId = NameServicePacket.ReadInt2(src, srcIndex);
			this.IsResponse = ((src[srcIndex + 2] & 128) != 0);
			this.OpCode = (src[srcIndex + 2] & 120) >> 3;
			this.IsAuthAnswer = ((src[srcIndex + 2] & 4) != 0);
			this.IsTruncated = ((src[srcIndex + 2] & 2) != 0);
			this.IsRecurDesired = ((src[srcIndex + 2] & 1) != 0);
			this.IsRecurAvailable = ((src[srcIndex + 2 + 1] & 128) != 0);
			this.IsBroadcast = ((src[srcIndex + 2 + 1] & 16) != 0);
			this.ResultCode = (int)(src[srcIndex + 2 + 1] & 15);
			this.QuestionCount = NameServicePacket.ReadInt2(src, srcIndex + 4);
			this.AnswerCount = NameServicePacket.ReadInt2(src, srcIndex + 6);
			this.AuthorityCount = NameServicePacket.ReadInt2(src, srcIndex + 8);
			this.AdditionalCount = NameServicePacket.ReadInt2(src, srcIndex + 10);
			return 12;
		}

		// Token: 0x06000708 RID: 1800 RVA: 0x00026914 File Offset: 0x00024B14
		internal virtual int WriteQuestionSectionWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			dstIndex += this.QuestionName.WriteWireFormat(dst, dstIndex);
			NameServicePacket.WriteInt2(this.QuestionType, dst, dstIndex);
			dstIndex += 2;
			NameServicePacket.WriteInt2(this.QuestionClass, dst, dstIndex);
			dstIndex += 2;
			return dstIndex - num;
		}

		// Token: 0x06000709 RID: 1801 RVA: 0x00026964 File Offset: 0x00024B64
		internal virtual int ReadQuestionSectionWireFormat(byte[] src, int srcIndex)
		{
			int num = srcIndex;
			srcIndex += this.QuestionName.ReadWireFormat(src, srcIndex);
			this.QuestionType = NameServicePacket.ReadInt2(src, srcIndex);
			srcIndex += 2;
			this.QuestionClass = NameServicePacket.ReadInt2(src, srcIndex);
			srcIndex += 2;
			return srcIndex - num;
		}

		// Token: 0x0600070A RID: 1802 RVA: 0x000269B0 File Offset: 0x00024BB0
		internal virtual int WriteResourceRecordWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			bool flag = this.RecordName == this.QuestionName;
			if (flag)
			{
				dst[dstIndex++] = 192;
				dst[dstIndex++] = 12;
			}
			else
			{
				dstIndex += this.RecordName.WriteWireFormat(dst, dstIndex);
			}
			NameServicePacket.WriteInt2(this.RecordType, dst, dstIndex);
			dstIndex += 2;
			NameServicePacket.WriteInt2(this.RecordClass, dst, dstIndex);
			dstIndex += 2;
			NameServicePacket.WriteInt4(this.Ttl, dst, dstIndex);
			dstIndex += 4;
			this.RDataLength = this.WriteRDataWireFormat(dst, dstIndex + 2);
			NameServicePacket.WriteInt2(this.RDataLength, dst, dstIndex);
			dstIndex += 2 + this.RDataLength;
			return dstIndex - num;
		}

		// Token: 0x0600070B RID: 1803 RVA: 0x00026A6C File Offset: 0x00024C6C
		internal virtual int ReadResourceRecordWireFormat(byte[] src, int srcIndex)
		{
			int num = srcIndex;
			bool flag = (src[srcIndex] & 192) == 192;
			if (flag)
			{
				this.RecordName = this.QuestionName;
				srcIndex += 2;
			}
			else
			{
				srcIndex += this.RecordName.ReadWireFormat(src, srcIndex);
			}
			this.RecordType = NameServicePacket.ReadInt2(src, srcIndex);
			srcIndex += 2;
			this.RecordClass = NameServicePacket.ReadInt2(src, srcIndex);
			srcIndex += 2;
			this.Ttl = NameServicePacket.ReadInt4(src, srcIndex);
			srcIndex += 4;
			this.RDataLength = NameServicePacket.ReadInt2(src, srcIndex);
			srcIndex += 2;
			this.AddrEntry = new NbtAddress[this.RDataLength / 6];
			int num2 = srcIndex + this.RDataLength;
			this.AddrIndex = 0;
			while (srcIndex < num2)
			{
				srcIndex += this.ReadRDataWireFormat(src, srcIndex);
				this.AddrIndex++;
			}
			return srcIndex - num;
		}

		// Token: 0x0600070C RID: 1804
		internal abstract int WriteBodyWireFormat(byte[] dst, int dstIndex);

		// Token: 0x0600070D RID: 1805
		internal abstract int ReadBodyWireFormat(byte[] src, int srcIndex);

		// Token: 0x0600070E RID: 1806
		internal abstract int WriteRDataWireFormat(byte[] dst, int dstIndex);

		// Token: 0x0600070F RID: 1807
		internal abstract int ReadRDataWireFormat(byte[] src, int srcIndex);

		// Token: 0x06000710 RID: 1808 RVA: 0x00026B54 File Offset: 0x00024D54
		public override string ToString()
		{
			int opCode = this.OpCode;
			string text;
			if (opCode != 0)
			{
				if (opCode != 7)
				{
					text = Extensions.ToString(this.OpCode);
				}
				else
				{
					text = "WACK";
				}
			}
			else
			{
				text = "QUERY";
			}
			switch (this.ResultCode)
			{
			case 1:
				goto IL_BB;
			case 2:
				goto IL_BB;
			case 4:
				goto IL_BB;
			case 5:
				goto IL_BB;
			case 6:
				goto IL_BB;
			case 7:
				goto IL_BB;
			}
			string text2 = "0x" + Hexdump.ToHexString(this.ResultCode, 1);
			IL_BB:
			int questionType = this.QuestionType;
			string text3;
			if (questionType != 32)
			{
				if (questionType != 33)
				{
					text3 = "0x" + Hexdump.ToHexString(this.QuestionType, 4);
				}
				else
				{
					text3 = "NBSTAT";
				}
			}
			else
			{
				text3 = "NB";
			}
			int recordType = this.RecordType;
			string text4;
			if (recordType <= 2)
			{
				if (recordType == 1)
				{
					text4 = "A";
					goto IL_179;
				}
				if (recordType == 2)
				{
					text4 = "NS";
					goto IL_179;
				}
			}
			else
			{
				if (recordType == 10)
				{
					text4 = "NULL";
					goto IL_179;
				}
				if (recordType == 32)
				{
					text4 = "NB";
					goto IL_179;
				}
				if (recordType == 33)
				{
					text4 = "NBSTAT";
					goto IL_179;
				}
			}
			text4 = "0x" + Hexdump.ToHexString(this.RecordType, 4);
			IL_179:
			return string.Concat(new object[]
			{
				"nameTrnId=",
				this.NameTrnId,
				",isResponse=",
				this.IsResponse.ToString(),
				",opCode=",
				text,
				",isAuthAnswer=",
				this.IsAuthAnswer.ToString(),
				",isTruncated=",
				this.IsTruncated.ToString(),
				",isRecurAvailable=",
				this.IsRecurAvailable.ToString(),
				",isRecurDesired=",
				this.IsRecurDesired.ToString(),
				",isBroadcast=",
				this.IsBroadcast.ToString(),
				",resultCode=",
				this.ResultCode,
				",questionCount=",
				this.QuestionCount,
				",answerCount=",
				this.AnswerCount,
				",authorityCount=",
				this.AuthorityCount,
				",additionalCount=",
				this.AdditionalCount,
				",questionName=",
				this.QuestionName,
				",questionType=",
				text3,
				",questionClass=",
				(this.QuestionClass == 1) ? "IN" : ("0x" + Hexdump.ToHexString(this.QuestionClass, 4)),
				",recordName=",
				this.RecordName,
				",recordType=",
				text4,
				",recordClass=",
				(this.RecordClass == 1) ? "IN" : ("0x" + Hexdump.ToHexString(this.RecordClass, 4)),
				",ttl=",
				this.Ttl,
				",rDataLength=",
				this.RDataLength
			});
		}

		// Token: 0x0400044A RID: 1098
		internal const int Query = 0;

		// Token: 0x0400044B RID: 1099
		internal const int Wack = 7;

		// Token: 0x0400044C RID: 1100
		internal const int FmtErr = 1;

		// Token: 0x0400044D RID: 1101
		internal const int SrvErr = 2;

		// Token: 0x0400044E RID: 1102
		internal const int ImpErr = 4;

		// Token: 0x0400044F RID: 1103
		internal const int RfsErr = 5;

		// Token: 0x04000450 RID: 1104
		internal const int ActErr = 6;

		// Token: 0x04000451 RID: 1105
		internal const int CftErr = 7;

		// Token: 0x04000452 RID: 1106
		internal const int NbIn = 2097153;

		// Token: 0x04000453 RID: 1107
		internal const int NbstatIn = 2162689;

		// Token: 0x04000454 RID: 1108
		internal const int Nb = 32;

		// Token: 0x04000455 RID: 1109
		internal const int Nbstat = 33;

		// Token: 0x04000456 RID: 1110
		internal const int In = 1;

		// Token: 0x04000457 RID: 1111
		internal const int A = 1;

		// Token: 0x04000458 RID: 1112
		internal const int Ns = 2;

		// Token: 0x04000459 RID: 1113
		internal const int Null = 10;

		// Token: 0x0400045A RID: 1114
		internal const int HeaderLength = 12;

		// Token: 0x0400045B RID: 1115
		internal const int OpcodeOffset = 2;

		// Token: 0x0400045C RID: 1116
		internal const int QuestionOffset = 4;

		// Token: 0x0400045D RID: 1117
		internal const int AnswerOffset = 6;

		// Token: 0x0400045E RID: 1118
		internal const int AuthorityOffset = 8;

		// Token: 0x0400045F RID: 1119
		internal const int AdditionalOffset = 10;

		// Token: 0x04000460 RID: 1120
		internal int AddrIndex;

		// Token: 0x04000461 RID: 1121
		internal NbtAddress[] AddrEntry;

		// Token: 0x04000462 RID: 1122
		internal int NameTrnId;

		// Token: 0x04000463 RID: 1123
		internal int OpCode;

		// Token: 0x04000464 RID: 1124
		internal int ResultCode;

		// Token: 0x04000465 RID: 1125
		internal int QuestionCount;

		// Token: 0x04000466 RID: 1126
		internal int AnswerCount;

		// Token: 0x04000467 RID: 1127
		internal int AuthorityCount;

		// Token: 0x04000468 RID: 1128
		internal int AdditionalCount;

		// Token: 0x04000469 RID: 1129
		internal bool Received;

		// Token: 0x0400046A RID: 1130
		internal bool IsResponse;

		// Token: 0x0400046B RID: 1131
		internal bool IsAuthAnswer;

		// Token: 0x0400046C RID: 1132
		internal bool IsTruncated;

		// Token: 0x0400046D RID: 1133
		internal bool IsRecurDesired;

		// Token: 0x0400046E RID: 1134
		internal bool IsRecurAvailable;

		// Token: 0x0400046F RID: 1135
		internal bool IsBroadcast;

		// Token: 0x04000470 RID: 1136
		internal Name QuestionName;

		// Token: 0x04000471 RID: 1137
		internal Name RecordName;

		// Token: 0x04000472 RID: 1138
		internal int QuestionType;

		// Token: 0x04000473 RID: 1139
		internal int QuestionClass;

		// Token: 0x04000474 RID: 1140
		internal int RecordType;

		// Token: 0x04000475 RID: 1141
		internal int RecordClass;

		// Token: 0x04000476 RID: 1142
		internal int Ttl;

		// Token: 0x04000477 RID: 1143
		internal int RDataLength;

		// Token: 0x04000478 RID: 1144
		internal IPAddress Addr;
	}
}
