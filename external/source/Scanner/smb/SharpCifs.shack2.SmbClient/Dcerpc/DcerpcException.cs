using System;
using System.IO;
using SharpCifs.Util;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Dcerpc
{
	// Token: 0x020000E1 RID: 225
	public class DcerpcException : IOException
	{
		// Token: 0x06000770 RID: 1904 RVA: 0x00028E24 File Offset: 0x00027024
		internal static string GetMessageByDcerpcError(int errcode)
		{
			int num = 0;
			int i = DcerpcError.DcerpcFaultCodes.Length;
			while (i >= num)
			{
				int num2 = (num + i) / 2;
				bool flag = errcode > DcerpcError.DcerpcFaultCodes[num2];
				if (flag)
				{
					num = num2 + 1;
				}
				else
				{
					bool flag2 = errcode < DcerpcError.DcerpcFaultCodes[num2];
					if (!flag2)
					{
						return DcerpcError.DcerpcFaultMessages[num2];
					}
					i = num2 - 1;
				}
			}
			return "0x" + Hexdump.ToHexString(errcode, 8);
		}

		// Token: 0x06000771 RID: 1905 RVA: 0x00028EA6 File Offset: 0x000270A6
		internal DcerpcException(int error) : base(DcerpcException.GetMessageByDcerpcError(error))
		{
			this._error = error;
		}

		// Token: 0x06000772 RID: 1906 RVA: 0x000062BC File Offset: 0x000044BC
		public DcerpcException(string msg) : base(msg)
		{
		}

		// Token: 0x06000773 RID: 1907 RVA: 0x00028EBD File Offset: 0x000270BD
		public DcerpcException(string msg, Exception rootCause) : base(msg)
		{
			this._rootCause = rootCause;
		}

		// Token: 0x06000774 RID: 1908 RVA: 0x00028ED0 File Offset: 0x000270D0
		public virtual int GetErrorCode()
		{
			return this._error;
		}

		// Token: 0x06000775 RID: 1909 RVA: 0x00028EE8 File Offset: 0x000270E8
		public virtual Exception GetRootCause()
		{
			return this._rootCause;
		}

		// Token: 0x06000776 RID: 1910 RVA: 0x00028F00 File Offset: 0x00027100
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

		// Token: 0x040004DB RID: 1243
		private int _error;

		// Token: 0x040004DC RID: 1244
		private Exception _rootCause;
	}
}
