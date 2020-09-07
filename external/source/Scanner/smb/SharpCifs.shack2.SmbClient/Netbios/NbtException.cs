using System;
using System.IO;

namespace SharpCifs.Netbios
{
	// Token: 0x020000D7 RID: 215
	public class NbtException : IOException
	{
		// Token: 0x0600073B RID: 1851 RVA: 0x00027E88 File Offset: 0x00026088
		public static string GetErrorString(int errorClass, int errorCode)
		{
			string text = string.Empty;
			switch (errorClass)
			{
			case 0:
				text += "SUCCESS";
				break;
			case 1:
				text += "ERR_NAM_SRVC/";
				if (errorCode == 1)
				{
					text += "FMT_ERR: Format Error";
				}
				text = text + "Unknown error code: " + errorCode;
				break;
			case 2:
				text += "ERR_SSN_SRVC/";
				if (errorCode != -1)
				{
					switch (errorCode)
					{
					case 128:
						text += "Not listening on called name";
						break;
					case 129:
						text += "Not listening for calling name";
						break;
					case 130:
						text += "Called name not present";
						break;
					case 131:
						text += "Called name present, but insufficient resources";
						break;
					default:
						if (errorCode != 143)
						{
							text = text + "Unknown error code: " + errorCode;
						}
						else
						{
							text += "Unspecified error";
						}
						break;
					}
				}
				else
				{
					text += "Connection refused";
				}
				break;
			default:
				text = text + "unknown error class: " + errorClass;
				break;
			}
			return text;
		}

		// Token: 0x0600073C RID: 1852 RVA: 0x00027FCF File Offset: 0x000261CF
		public NbtException(int errorClass, int errorCode) : base(NbtException.GetErrorString(errorClass, errorCode))
		{
			this.ErrorClass = errorClass;
			this.ErrorCode = errorCode;
		}

		// Token: 0x0600073D RID: 1853 RVA: 0x00027FF0 File Offset: 0x000261F0
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"errorClass=",
				this.ErrorClass,
				",errorCode=",
				this.ErrorCode,
				",errorString=",
				NbtException.GetErrorString(this.ErrorClass, this.ErrorCode)
			});
		}

		// Token: 0x04000497 RID: 1175
		public const int Success = 0;

		// Token: 0x04000498 RID: 1176
		public const int ErrNamSrvc = 1;

		// Token: 0x04000499 RID: 1177
		public const int ErrSsnSrvc = 2;

		// Token: 0x0400049A RID: 1178
		public const int FmtErr = 1;

		// Token: 0x0400049B RID: 1179
		public const int SrvErr = 2;

		// Token: 0x0400049C RID: 1180
		public const int ImpErr = 4;

		// Token: 0x0400049D RID: 1181
		public const int RfsErr = 5;

		// Token: 0x0400049E RID: 1182
		public const int ActErr = 6;

		// Token: 0x0400049F RID: 1183
		public const int CftErr = 7;

		// Token: 0x040004A0 RID: 1184
		public const int ConnectionRefused = -1;

		// Token: 0x040004A1 RID: 1185
		public const int NotListeningCalled = 128;

		// Token: 0x040004A2 RID: 1186
		public const int NotListeningCalling = 129;

		// Token: 0x040004A3 RID: 1187
		public const int CalledNotPresent = 130;

		// Token: 0x040004A4 RID: 1188
		public const int NoResources = 131;

		// Token: 0x040004A5 RID: 1189
		public const int Unspecified = 143;

		// Token: 0x040004A6 RID: 1190
		public int ErrorClass;

		// Token: 0x040004A7 RID: 1191
		public int ErrorCode;
	}
}
