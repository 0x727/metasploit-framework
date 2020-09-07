using System;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x020000B4 RID: 180
	internal class SmbTree
	{
		// Token: 0x060005D0 RID: 1488 RVA: 0x000201E0 File Offset: 0x0001E3E0
		internal SmbTree(SmbSession session, string share, string service)
		{
			this.Session = session;
			this.Share = share.ToUpper();
			bool flag = service != null && !service.StartsWith("??");
			if (flag)
			{
				this.Service = service;
			}
			this.Service0 = this.Service;
			this.ConnectionState = 0;
		}

		// Token: 0x060005D1 RID: 1489 RVA: 0x00020248 File Offset: 0x0001E448
		internal virtual bool Matches(string share, string service)
		{
			return Runtime.EqualsIgnoreCase(this.Share, share) && (service == null || service.StartsWith("??") || Runtime.EqualsIgnoreCase(this.Service, service));
		}

		// Token: 0x060005D2 RID: 1490 RVA: 0x0002028C File Offset: 0x0001E48C
		public override bool Equals(object obj)
		{
			bool flag = obj is SmbTree;
			bool result;
			if (flag)
			{
				SmbTree smbTree = (SmbTree)obj;
				result = this.Matches(smbTree.Share, smbTree.Service);
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x060005D3 RID: 1491 RVA: 0x000202CC File Offset: 0x0001E4CC
		internal virtual void Send(ServerMessageBlock request, ServerMessageBlock response)
		{
			SmbTransport obj = this.Session.Transport();
			lock (obj)
			{
				bool flag2 = response != null;
				if (flag2)
				{
					response.Received = false;
				}
				this.TreeConnect(request, response);
				bool flag3 = request == null || (response != null && response.Received);
				if (!flag3)
				{
					bool flag4 = !this.Service.Equals("A:");
					if (flag4)
					{
						byte command = request.Command;
						if (command <= 37)
						{
							if (command != 4)
							{
								if (command != 37)
								{
									goto IL_154;
								}
								goto IL_CC;
							}
						}
						else
						{
							switch (command)
							{
							case 45:
							case 46:
							case 47:
								break;
							case 48:
							case 49:
								goto IL_154;
							case 50:
								goto IL_CC;
							default:
								if (command != 113 && command != 162)
								{
									goto IL_154;
								}
								break;
							}
						}
						goto IL_183;
						IL_CC:
						int num = (int)(((SmbComTransaction)request).SubCommand & byte.MaxValue);
						if (num <= 38)
						{
							if (num <= 16)
							{
								if (num != 0 && num != 16)
								{
									goto IL_136;
								}
							}
							else if (num != 35 && num != 38)
							{
								goto IL_136;
							}
						}
						else if (num <= 84)
						{
							if (num != 83 && num != 84)
							{
								goto IL_136;
							}
						}
						else if (num != 104 && num != 215)
						{
							goto IL_136;
						}
						goto IL_183;
						IL_136:
						throw new SmbException("Invalid operation for " + this.Service + " service");
						IL_154:
						throw new SmbException(string.Concat(new object[]
						{
							"Invalid operation for ",
							this.Service,
							" service",
							request
						}));
						IL_183:;
					}
					request.Tid = this.Tid;
					bool flag5 = this.InDfs && !this.Service.Equals("IPC") && !string.IsNullOrEmpty(request.Path);
					if (flag5)
					{
						request.Flags2 = SmbConstants.Flags2ResolvePathsInDfs;
						request.Path = string.Concat(new string[]
						{
							"\\",
							this.Session.Transport().TconHostName,
							"\\",
							this.Share,
							request.Path
						});
					}
					try
					{
						this.Session.Send(request, response);
					}
					catch (SmbException ex)
					{
						bool flag6 = ex.GetNtStatus() == -1073741623;
						if (flag6)
						{
							this.TreeDisconnect(true);
						}
						throw;
					}
				}
			}
		}

		// Token: 0x060005D4 RID: 1492 RVA: 0x00020568 File Offset: 0x0001E768
		internal virtual void TreeConnect(ServerMessageBlock andx, ServerMessageBlock andxResponse)
		{
			SmbTransport obj = this.Session.Transport();
			lock (obj)
			{
				while (this.ConnectionState != 0)
				{
					bool flag2 = this.ConnectionState == 2 || this.ConnectionState == 3;
					if (flag2)
					{
						return;
					}
					try
					{
						Runtime.Wait(this.Session.transport);
					}
					catch (Exception ex)
					{
						throw new SmbException(ex.Message, ex);
					}
				}
				this.ConnectionState = 1;
				try
				{
					this.Session.transport.Connect();
					string text = "\\\\" + this.Session.transport.TconHostName + "\\" + this.Share;
					this.Service = this.Service0;
					bool flag3 = this.Session.transport.Log.Level >= 4;
					if (flag3)
					{
						this.Session.transport.Log.WriteLine("treeConnect: unc=" + text + ",service=" + this.Service);
					}
					SmbComTreeConnectAndXResponse smbComTreeConnectAndXResponse = new SmbComTreeConnectAndXResponse(andxResponse);
					SmbComTreeConnectAndX request = new SmbComTreeConnectAndX(this.Session, text, this.Service, andx);
					this.Session.Send(request, smbComTreeConnectAndXResponse);
					this.Tid = smbComTreeConnectAndXResponse.Tid;
					this.Service = smbComTreeConnectAndXResponse.Service;
					this.InDfs = smbComTreeConnectAndXResponse.ShareIsInDfs;
					this.TreeNum = SmbTree._treeConnCounter++;
					this.ConnectionState = 2;
				}
				catch (SmbException ex2)
				{
					this.TreeDisconnect(true);
					this.ConnectionState = 0;
					throw;
				}
			}
		}

		// Token: 0x060005D5 RID: 1493 RVA: 0x00020764 File Offset: 0x0001E964
		internal virtual void TreeDisconnect(bool inError)
		{
			SmbTransport obj = this.Session.Transport();
			lock (obj)
			{
				bool flag2 = this.ConnectionState != 2;
				if (!flag2)
				{
					this.ConnectionState = 3;
					bool flag3 = !inError && this.Tid != 0;
					if (flag3)
					{
						try
						{
							this.Send(new SmbComTreeDisconnect(), null);
						}
						catch (SmbException ex)
						{
							bool flag4 = this.Session.transport.Log.Level > 1;
							if (flag4)
							{
								Runtime.PrintStackTrace(ex, this.Session.transport.Log);
							}
						}
					}
					this.InDfs = false;
					this.InDomainDfs = false;
					this.ConnectionState = 0;
					Runtime.NotifyAll(this.Session.transport);
				}
			}
		}

		// Token: 0x060005D6 RID: 1494 RVA: 0x0002085C File Offset: 0x0001EA5C
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"SmbTree[share=",
				this.Share,
				",service=",
				this.Service,
				",tid=",
				this.Tid,
				",inDfs=",
				this.InDfs.ToString(),
				",inDomainDfs=",
				this.InDomainDfs.ToString(),
				",connectionState=",
				this.ConnectionState,
				"]"
			});
		}

		// Token: 0x04000378 RID: 888
		private static int _treeConnCounter;

		// Token: 0x04000379 RID: 889
		internal int ConnectionState;

		// Token: 0x0400037A RID: 890
		internal int Tid;

		// Token: 0x0400037B RID: 891
		internal string Share;

		// Token: 0x0400037C RID: 892
		internal string Service = "?????";

		// Token: 0x0400037D RID: 893
		internal string Service0;

		// Token: 0x0400037E RID: 894
		internal SmbSession Session;

		// Token: 0x0400037F RID: 895
		internal bool InDfs;

		// Token: 0x04000380 RID: 896
		internal bool InDomainDfs;

		// Token: 0x04000381 RID: 897
		internal int TreeNum;
	}
}
