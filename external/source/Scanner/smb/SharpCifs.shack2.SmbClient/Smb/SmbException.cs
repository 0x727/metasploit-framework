using System;
using System.IO;
using SharpCifs.Util;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x020000A8 RID: 168
	public class SmbException : IOException
	{
		// Token: 0x060004DA RID: 1242 RVA: 0x00017BF0 File Offset: 0x00015DF0
		internal static string GetMessageByCode(int errcode)
		{
			bool flag = errcode == 0;
			string result;
			if (flag)
			{
				result = "NT_STATUS_SUCCESS";
			}
			else
			{
				bool flag2 = (errcode & -1073741824) == -1073741824;
				if (flag2)
				{
					int num = 1;
					int i = NtStatus.NtStatusCodes.Length - 1;
					while (i >= num)
					{
						int num2 = (num + i) / 2;
						bool flag3 = errcode > NtStatus.NtStatusCodes[num2];
						if (flag3)
						{
							num = num2 + 1;
						}
						else
						{
							bool flag4 = errcode < NtStatus.NtStatusCodes[num2];
							if (!flag4)
							{
								return NtStatus.NtStatusMessages[num2];
							}
							i = num2 - 1;
						}
					}
				}
				else
				{
					int num3 = 0;
					int j = DosError.DosErrorCodes.Length - 1;
					while (j >= num3)
					{
						int num4 = (num3 + j) / 2;
						bool flag5 = errcode > DosError.DosErrorCodes[num4][0];
						if (flag5)
						{
							num3 = num4 + 1;
						}
						else
						{
							bool flag6 = errcode < DosError.DosErrorCodes[num4][0];
							if (!flag6)
							{
								return DosError.DosErrorMessages[num4];
							}
							j = num4 - 1;
						}
					}
				}
				result = "0x" + Hexdump.ToHexString(errcode, 8);
			}
			return result;
		}

		// Token: 0x060004DB RID: 1243 RVA: 0x00017D20 File Offset: 0x00015F20
		internal static int GetStatusByCode(int errcode)
		{
			bool flag = (errcode & -1073741824) != 0;
			int result;
			if (flag)
			{
				result = errcode;
			}
			else
			{
				int num = 0;
				int i = DosError.DosErrorCodes.Length - 1;
				while (i >= num)
				{
					int num2 = (num + i) / 2;
					bool flag2 = errcode > DosError.DosErrorCodes[num2][0];
					if (flag2)
					{
						num = num2 + 1;
					}
					else
					{
						bool flag3 = errcode < DosError.DosErrorCodes[num2][0];
						if (!flag3)
						{
							return DosError.DosErrorCodes[num2][1];
						}
						i = num2 - 1;
					}
				}
				result = -1073741823;
			}
			return result;
		}

		// Token: 0x060004DC RID: 1244 RVA: 0x00017DB8 File Offset: 0x00015FB8
		internal static string GetMessageByWinerrCode(int errcode)
		{
			int num = 0;
			int i = WinError.WinerrCodes.Length - 1;
			while (i >= num)
			{
				int num2 = (num + i) / 2;
				bool flag = errcode > WinError.WinerrCodes[num2];
				if (flag)
				{
					num = num2 + 1;
				}
				else
				{
					bool flag2 = errcode < WinError.WinerrCodes[num2];
					if (!flag2)
					{
						return WinError.WinerrMessages[num2];
					}
					i = num2 - 1;
				}
			}
			return errcode + string.Empty;
		}

		// Token: 0x060004DD RID: 1245 RVA: 0x000062B2 File Offset: 0x000044B2
		public SmbException()
		{
		}

		// Token: 0x060004DE RID: 1246 RVA: 0x00017E3B File Offset: 0x0001603B
		internal SmbException(int errcode, Exception rootCause) : base(SmbException.GetMessageByCode(errcode))
		{
			this._status = SmbException.GetStatusByCode(errcode);
			this._rootCause = rootCause;
		}

		// Token: 0x060004DF RID: 1247 RVA: 0x00017E5E File Offset: 0x0001605E
		public SmbException(string msg) : base(msg)
		{
			this._status = -1073741823;
		}

		// Token: 0x060004E0 RID: 1248 RVA: 0x00017E74 File Offset: 0x00016074
		public SmbException(string msg, Exception rootCause) : base(msg)
		{
			this._rootCause = rootCause;
			this._status = -1073741823;
		}

		// Token: 0x060004E1 RID: 1249 RVA: 0x00017E91 File Offset: 0x00016091
		public SmbException(int errcode, bool winerr) : base(winerr ? SmbException.GetMessageByWinerrCode(errcode) : SmbException.GetMessageByCode(errcode))
		{
			this._status = (winerr ? errcode : SmbException.GetStatusByCode(errcode));
		}

		// Token: 0x060004E2 RID: 1250 RVA: 0x00017EC0 File Offset: 0x000160C0
		public virtual int GetNtStatus()
		{
			return this._status;
		}

		// Token: 0x060004E3 RID: 1251 RVA: 0x00017ED8 File Offset: 0x000160D8
		public virtual Exception GetRootCause()
		{
			return this._rootCause;
		}

		// Token: 0x060004E4 RID: 1252 RVA: 0x00017EF0 File Offset: 0x000160F0
		public override string ToString()
		{
			bool flag = this._rootCause != null;
			string result;
			if (flag)
			{
				StringWriter stringWriter = new StringWriter();
				PrintWriter tw = new PrintWriter(stringWriter);
				Runtime.PrintStackTrace(this._rootCause, tw);
				result = base.ToString() + "\n" + stringWriter;
			}
			else
			{
				result = base.ToString();
			}
			return result;
		}

		// Token: 0x040002E6 RID: 742
		private int _status;

		// Token: 0x040002E7 RID: 743
		private Exception _rootCause;
	}
}
