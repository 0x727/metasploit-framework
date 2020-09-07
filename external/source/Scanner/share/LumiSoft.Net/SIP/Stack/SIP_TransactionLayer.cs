using System;
using System.Collections.Generic;
using System.Threading;
using LumiSoft.Net.SIP.Message;

namespace LumiSoft.Net.SIP.Stack
{
	// Token: 0x020000A2 RID: 162
	public class SIP_TransactionLayer
	{
		// Token: 0x06000630 RID: 1584 RVA: 0x00023488 File Offset: 0x00022488
		internal SIP_TransactionLayer(SIP_Stack stack)
		{
			bool flag = stack == null;
			if (flag)
			{
				throw new ArgumentNullException("stack");
			}
			this.m_pStack = stack;
			this.m_pClientTransactions = new Dictionary<string, SIP_ClientTransaction>();
			this.m_pServerTransactions = new Dictionary<string, SIP_ServerTransaction>();
			this.m_pDialogs = new Dictionary<string, SIP_Dialog>();
		}

		// Token: 0x06000631 RID: 1585 RVA: 0x000234FC File Offset: 0x000224FC
		internal void Dispose()
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				foreach (SIP_ClientTransaction sip_ClientTransaction in this.ClientTransactions)
				{
					try
					{
						sip_ClientTransaction.Dispose();
					}
					catch
					{
					}
				}
				foreach (SIP_ServerTransaction sip_ServerTransaction in this.ServerTransactions)
				{
					try
					{
						sip_ServerTransaction.Dispose();
					}
					catch
					{
					}
				}
				foreach (SIP_Dialog sip_Dialog in this.Dialogs)
				{
					try
					{
						sip_Dialog.Dispose();
					}
					catch
					{
					}
				}
				this.m_IsDisposed = true;
			}
		}

		// Token: 0x06000632 RID: 1586 RVA: 0x000235E4 File Offset: 0x000225E4
		public SIP_ClientTransaction CreateClientTransaction(SIP_Flow flow, SIP_Request request, bool addVia)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = flow == null;
			if (flag)
			{
				throw new ArgumentNullException("flow");
			}
			bool flag2 = request == null;
			if (flag2)
			{
				throw new ArgumentNullException("request");
			}
			if (addVia)
			{
				SIP_t_ViaParm sip_t_ViaParm = new SIP_t_ViaParm();
				sip_t_ViaParm.ProtocolName = "SIP";
				sip_t_ViaParm.ProtocolVersion = "2.0";
				sip_t_ViaParm.ProtocolTransport = flow.Transport;
				sip_t_ViaParm.SentBy = new HostEndPoint("transport_layer_will_replace_it", -1);
				sip_t_ViaParm.Branch = SIP_t_ViaParm.CreateBranch();
				sip_t_ViaParm.RPort = 0;
				request.Via.AddToTop(sip_t_ViaParm.ToStringValue());
			}
			Dictionary<string, SIP_ClientTransaction> pClientTransactions = this.m_pClientTransactions;
			bool flag3 = false;
			SIP_ClientTransaction transaction2;
			try
			{
				Monitor.Enter(pClientTransactions, ref flag3);
				SIP_ClientTransaction transaction = new SIP_ClientTransaction(this.m_pStack, flow, request);
				this.m_pClientTransactions.Add(transaction.Key, transaction);
				transaction.StateChanged += delegate(object s, EventArgs e)
				{
					bool flag5 = transaction.State == SIP_TransactionState.Terminated;
					if (flag5)
					{
						Dictionary<string, SIP_ClientTransaction> pClientTransactions2 = this.m_pClientTransactions;
						lock (pClientTransactions2)
						{
							this.m_pClientTransactions.Remove(transaction.Key);
						}
					}
				};
				SIP_Dialog sip_Dialog = this.MatchDialog(request);
				bool flag4 = sip_Dialog != null;
				if (flag4)
				{
					sip_Dialog.AddTransaction(transaction);
				}
				transaction2 = transaction;
			}
			finally
			{
				if (flag3)
				{
					Monitor.Exit(pClientTransactions);
				}
			}
			return transaction2;
		}

		// Token: 0x06000633 RID: 1587 RVA: 0x0002376C File Offset: 0x0002276C
		public SIP_ServerTransaction CreateServerTransaction(SIP_Flow flow, SIP_Request request)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = flow == null;
			if (flag)
			{
				throw new ArgumentNullException("flow");
			}
			bool flag2 = request == null;
			if (flag2)
			{
				throw new ArgumentNullException("request");
			}
			Dictionary<string, SIP_ServerTransaction> pServerTransactions = this.m_pServerTransactions;
			bool flag3 = false;
			SIP_ServerTransaction transaction2;
			try
			{
				Monitor.Enter(pServerTransactions, ref flag3);
				SIP_ServerTransaction transaction = new SIP_ServerTransaction(this.m_pStack, flow, request);
				this.m_pServerTransactions.Add(transaction.Key, transaction);
				transaction.StateChanged += delegate(object s, EventArgs e)
				{
					bool flag5 = transaction.State == SIP_TransactionState.Terminated;
					if (flag5)
					{
						Dictionary<string, SIP_ClientTransaction> pClientTransactions = this.m_pClientTransactions;
						lock (pClientTransactions)
						{
							this.m_pServerTransactions.Remove(transaction.Key);
						}
					}
				};
				SIP_Dialog sip_Dialog = this.MatchDialog(request);
				bool flag4 = sip_Dialog != null;
				if (flag4)
				{
					sip_Dialog.AddTransaction(transaction);
				}
				transaction2 = transaction;
			}
			finally
			{
				if (flag3)
				{
					Monitor.Exit(pServerTransactions);
				}
			}
			return transaction2;
		}

		// Token: 0x06000634 RID: 1588 RVA: 0x0002387C File Offset: 0x0002287C
		public SIP_ServerTransaction EnsureServerTransaction(SIP_Flow flow, SIP_Request request)
		{
			bool flag = flow == null;
			if (flag)
			{
				throw new ArgumentNullException("flow");
			}
			bool flag2 = request == null;
			if (flag2)
			{
				throw new ArgumentNullException("request");
			}
			bool flag3 = request.RequestLine.Method == "ACK";
			if (flag3)
			{
				throw new InvalidOperationException("ACK request is transaction less request, can't create transaction for it.");
			}
			string text = request.Via.GetTopMostValue().Branch + "-" + request.Via.GetTopMostValue().SentBy;
			bool flag4 = request.RequestLine.Method == "CANCEL";
			if (flag4)
			{
				text += "-CANCEL";
			}
			Dictionary<string, SIP_ServerTransaction> pServerTransactions = this.m_pServerTransactions;
			SIP_ServerTransaction result;
			lock (pServerTransactions)
			{
				SIP_ServerTransaction sip_ServerTransaction = null;
				this.m_pServerTransactions.TryGetValue(text, out sip_ServerTransaction);
				bool flag6 = sip_ServerTransaction == null;
				if (flag6)
				{
					sip_ServerTransaction = this.CreateServerTransaction(flow, request);
				}
				result = sip_ServerTransaction;
			}
			return result;
		}

		// Token: 0x06000635 RID: 1589 RVA: 0x00023990 File Offset: 0x00022990
		internal SIP_ClientTransaction MatchClientTransaction(SIP_Response response)
		{
			SIP_ClientTransaction result = null;
			string key = response.Via.GetTopMostValue().Branch + "-" + response.CSeq.RequestMethod;
			Dictionary<string, SIP_ClientTransaction> pClientTransactions = this.m_pClientTransactions;
			lock (pClientTransactions)
			{
				this.m_pClientTransactions.TryGetValue(key, out result);
			}
			return result;
		}

		// Token: 0x06000636 RID: 1590 RVA: 0x00023A10 File Offset: 0x00022A10
		internal SIP_ServerTransaction MatchServerTransaction(SIP_Request request)
		{
			SIP_ServerTransaction sip_ServerTransaction = null;
			string text = request.Via.GetTopMostValue().Branch + "-" + request.Via.GetTopMostValue().SentBy;
			bool flag = request.RequestLine.Method == "CANCEL";
			if (flag)
			{
				text += "-CANCEL";
			}
			Dictionary<string, SIP_ServerTransaction> pServerTransactions = this.m_pServerTransactions;
			lock (pServerTransactions)
			{
				this.m_pServerTransactions.TryGetValue(text, out sip_ServerTransaction);
			}
			bool flag3 = sip_ServerTransaction != null && request.RequestLine.Method == "ACK" && sip_ServerTransaction.State == SIP_TransactionState.Terminated;
			if (flag3)
			{
				sip_ServerTransaction = null;
			}
			return sip_ServerTransaction;
		}

		// Token: 0x06000637 RID: 1591 RVA: 0x00023AEC File Offset: 0x00022AEC
		public SIP_ServerTransaction MatchCancelToTransaction(SIP_Request cancelRequest)
		{
			bool flag = cancelRequest == null;
			if (flag)
			{
				throw new ArgumentNullException("cancelRequest");
			}
			bool flag2 = cancelRequest.RequestLine.Method != "CANCEL";
			if (flag2)
			{
				throw new ArgumentException("Argument 'cancelRequest' is not SIP CANCEL request.");
			}
			SIP_ServerTransaction result = null;
			string key = cancelRequest.Via.GetTopMostValue().Branch + "-" + cancelRequest.Via.GetTopMostValue().SentBy;
			Dictionary<string, SIP_ServerTransaction> pServerTransactions = this.m_pServerTransactions;
			lock (pServerTransactions)
			{
				this.m_pServerTransactions.TryGetValue(key, out result);
			}
			return result;
		}

		// Token: 0x06000638 RID: 1592 RVA: 0x00023BAC File Offset: 0x00022BAC
		public SIP_Dialog GetOrCreateDialog(SIP_Transaction transaction, SIP_Response response)
		{
			bool flag = transaction == null;
			if (flag)
			{
				throw new ArgumentNullException("transaction");
			}
			bool flag2 = response == null;
			if (flag2)
			{
				throw new ArgumentNullException("response");
			}
			bool flag3 = transaction is SIP_ServerTransaction;
			string key;
			if (flag3)
			{
				key = string.Concat(new string[]
				{
					response.CallID,
					"-",
					response.To.Tag,
					"-",
					response.From.Tag
				});
			}
			else
			{
				key = string.Concat(new string[]
				{
					response.CallID,
					"-",
					response.From.Tag,
					"-",
					response.To.Tag
				});
			}
			Dictionary<string, SIP_Dialog> pDialogs = this.m_pDialogs;
			bool flag4 = false;
			SIP_Dialog dialog2;
			try
			{
				Monitor.Enter(pDialogs, ref flag4);
				SIP_Dialog dialog = null;
				this.m_pDialogs.TryGetValue(key, out dialog);
				bool flag5 = dialog == null;
				if (flag5)
				{
					bool flag6 = response.CSeq.RequestMethod.ToUpper() == "INVITE";
					if (flag6)
					{
						dialog = new SIP_Dialog_Invite();
					}
					else
					{
						bool flag7 = response.CSeq.RequestMethod.ToUpper() == "REFER";
						if (!flag7)
						{
							throw new ArgumentException("Method '" + response.CSeq.RequestMethod + "' has no dialog handler.");
						}
						dialog = new SIP_Dialog_Refer();
					}
					dialog.Init(this.m_pStack, transaction, response);
					dialog.StateChanged += delegate(object s, EventArgs a)
					{
						bool flag8 = dialog.State == SIP_DialogState.Terminated;
						if (flag8)
						{
							this.m_pDialogs.Remove(dialog.ID);
						}
					};
					this.m_pDialogs.Add(dialog.ID, dialog);
				}
				dialog2 = dialog;
			}
			finally
			{
				if (flag4)
				{
					Monitor.Exit(pDialogs);
				}
			}
			return dialog2;
		}

		// Token: 0x06000639 RID: 1593 RVA: 0x00023DD8 File Offset: 0x00022DD8
		internal void RemoveDialog(SIP_Dialog dialog)
		{
			Dictionary<string, SIP_Dialog> pDialogs = this.m_pDialogs;
			lock (pDialogs)
			{
				this.m_pDialogs.Remove(dialog.ID);
			}
		}

		// Token: 0x0600063A RID: 1594 RVA: 0x00023E2C File Offset: 0x00022E2C
		internal SIP_Dialog MatchDialog(SIP_Request request)
		{
			bool flag = request == null;
			if (flag)
			{
				throw new ArgumentNullException("request");
			}
			SIP_Dialog result = null;
			try
			{
				string callID = request.CallID;
				string tag = request.To.Tag;
				string tag2 = request.From.Tag;
				bool flag2 = callID != null && tag != null && tag2 != null;
				if (flag2)
				{
					string key = string.Concat(new string[]
					{
						callID,
						"-",
						tag,
						"-",
						tag2
					});
					Dictionary<string, SIP_Dialog> pDialogs = this.m_pDialogs;
					lock (pDialogs)
					{
						this.m_pDialogs.TryGetValue(key, out result);
					}
				}
			}
			catch
			{
			}
			return result;
		}

		// Token: 0x0600063B RID: 1595 RVA: 0x00023F14 File Offset: 0x00022F14
		internal SIP_Dialog MatchDialog(SIP_Response response)
		{
			bool flag = response == null;
			if (flag)
			{
				throw new ArgumentNullException("response");
			}
			SIP_Dialog result = null;
			try
			{
				string callID = response.CallID;
				string tag = response.From.Tag;
				string tag2 = response.To.Tag;
				bool flag2 = callID != null && tag != null && tag2 != null;
				if (flag2)
				{
					string key = string.Concat(new string[]
					{
						callID,
						"-",
						tag,
						"-",
						tag2
					});
					Dictionary<string, SIP_Dialog> pDialogs = this.m_pDialogs;
					lock (pDialogs)
					{
						this.m_pDialogs.TryGetValue(key, out result);
					}
				}
			}
			catch
			{
			}
			return result;
		}

		// Token: 0x17000203 RID: 515
		// (get) Token: 0x0600063C RID: 1596 RVA: 0x00023FFC File Offset: 0x00022FFC
		public SIP_Transaction[] Transactions
		{
			get
			{
				List<SIP_Transaction> list = new List<SIP_Transaction>();
				list.AddRange(this.ClientTransactions);
				list.AddRange(this.ServerTransactions);
				return list.ToArray();
			}
		}

		// Token: 0x17000204 RID: 516
		// (get) Token: 0x0600063D RID: 1597 RVA: 0x00024034 File Offset: 0x00023034
		public SIP_ClientTransaction[] ClientTransactions
		{
			get
			{
				Dictionary<string, SIP_ClientTransaction> pClientTransactions = this.m_pClientTransactions;
				SIP_ClientTransaction[] result;
				lock (pClientTransactions)
				{
					SIP_ClientTransaction[] array = new SIP_ClientTransaction[this.m_pClientTransactions.Values.Count];
					this.m_pClientTransactions.Values.CopyTo(array, 0);
					result = array;
				}
				return result;
			}
		}

		// Token: 0x17000205 RID: 517
		// (get) Token: 0x0600063E RID: 1598 RVA: 0x000240A0 File Offset: 0x000230A0
		public SIP_ServerTransaction[] ServerTransactions
		{
			get
			{
				Dictionary<string, SIP_ServerTransaction> pServerTransactions = this.m_pServerTransactions;
				SIP_ServerTransaction[] result;
				lock (pServerTransactions)
				{
					SIP_ServerTransaction[] array = new SIP_ServerTransaction[this.m_pServerTransactions.Values.Count];
					this.m_pServerTransactions.Values.CopyTo(array, 0);
					result = array;
				}
				return result;
			}
		}

		// Token: 0x17000206 RID: 518
		// (get) Token: 0x0600063F RID: 1599 RVA: 0x0002410C File Offset: 0x0002310C
		public SIP_Dialog[] Dialogs
		{
			get
			{
				Dictionary<string, SIP_Dialog> pDialogs = this.m_pDialogs;
				SIP_Dialog[] result;
				lock (pDialogs)
				{
					SIP_Dialog[] array = new SIP_Dialog[this.m_pDialogs.Values.Count];
					this.m_pDialogs.Values.CopyTo(array, 0);
					result = array;
				}
				return result;
			}
		}

		// Token: 0x04000293 RID: 659
		private bool m_IsDisposed = false;

		// Token: 0x04000294 RID: 660
		private SIP_Stack m_pStack = null;

		// Token: 0x04000295 RID: 661
		private Dictionary<string, SIP_ClientTransaction> m_pClientTransactions = null;

		// Token: 0x04000296 RID: 662
		private Dictionary<string, SIP_ServerTransaction> m_pServerTransactions = null;

		// Token: 0x04000297 RID: 663
		private Dictionary<string, SIP_Dialog> m_pDialogs = null;
	}
}
