using System;
using System.Diagnostics;

namespace LumiSoft.Net.SIP.Stack
{
	// Token: 0x02000081 RID: 129
	public class SIP_Dialog_Refer : SIP_Dialog
	{
		// Token: 0x060004BF RID: 1215 RVA: 0x0001808C File Offset: 0x0001708C
		internal SIP_Dialog_Refer()
		{
		}

		// Token: 0x060004C0 RID: 1216 RVA: 0x000091B8 File Offset: 0x000081B8
		private void CreateNotify(string statusLine)
		{
		}

		// Token: 0x060004C1 RID: 1217 RVA: 0x000180A0 File Offset: 0x000170A0
		protected internal override bool ProcessRequest(SIP_RequestReceivedEventArgs e)
		{
			bool flag = e == null;
			if (flag)
			{
				throw new ArgumentNullException("e");
			}
			bool flag2 = base.ProcessRequest(e);
			bool result;
			if (flag2)
			{
				result = true;
			}
			else
			{
				bool flag3 = e.Request.RequestLine.Method == "NOTIFY";
				if (flag3)
				{
					this.OnNotify(e);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x1400000E RID: 14
		// (add) Token: 0x060004C2 RID: 1218 RVA: 0x00018104 File Offset: 0x00017104
		// (remove) Token: 0x060004C3 RID: 1219 RVA: 0x0001813C File Offset: 0x0001713C
		
		public event EventHandler<SIP_RequestReceivedEventArgs> Notify = null;

		// Token: 0x060004C4 RID: 1220 RVA: 0x00018174 File Offset: 0x00017174
		private void OnNotify(SIP_RequestReceivedEventArgs e)
		{
			bool flag = this.Notify != null;
			if (flag)
			{
				this.Notify(this, e);
			}
		}
	}
}
