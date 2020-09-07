using System;
using System.IO;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Util.Transport
{
	// Token: 0x02000012 RID: 18
	public class TransportException : IOException
	{
		// Token: 0x0600009E RID: 158 RVA: 0x000062B2 File Offset: 0x000044B2
		public TransportException()
		{
		}

		// Token: 0x0600009F RID: 159 RVA: 0x000062BC File Offset: 0x000044BC
		public TransportException(string msg) : base(msg)
		{
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x000062C7 File Offset: 0x000044C7
		public TransportException(Exception rootCause)
		{
			this._rootCause = rootCause;
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x000062D8 File Offset: 0x000044D8
		public TransportException(string msg, Exception rootCause) : base(msg)
		{
			this._rootCause = rootCause;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x000062EC File Offset: 0x000044EC
		public virtual Exception GetRootCause()
		{
			return this._rootCause;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00006304 File Offset: 0x00004504
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

		// Token: 0x0400004B RID: 75
		private Exception _rootCause;
	}
}
