using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using LumiSoft.Net.Media.Codec;

namespace LumiSoft.Net.RTP.Debug
{
	// Token: 0x020000E3 RID: 227
	public partial class wfrm_RTP_Debug : Form
	{
		// Token: 0x0600091A RID: 2330 RVA: 0x000367B0 File Offset: 0x000357B0
		public wfrm_RTP_Debug(RTP_MultimediaSession session)
		{
			bool flag = session == null;
			if (flag)
			{
				throw new ArgumentNullException("session");
			}
			this.m_pSession = session;
			this.InitUI();
			base.Visible = true;
			this.m_pSession.Error += this.m_pSession_Error;
			this.m_pSession.SessionCreated += this.m_pSession_SessionCreated;
			this.m_pSession.NewParticipant += this.m_pSession_NewParticipant;
			this.m_pSession.LocalParticipant.SourceAdded += this.Participant_SourceAdded;
			this.m_pSession.LocalParticipant.SourceRemoved += this.Participant_SourceRemoved;
			this.m_pTimer = new Timer();
			this.m_pTimer.Interval = 1000;
			this.m_pTimer.Tick += this.m_pTimer_Tick;
			this.m_pTimer.Enabled = true;
			foreach (RTP_Session rtp_Session in this.m_pSession.Sessions)
			{
				wfrm_RTP_Debug.ComboBoxItem item = new wfrm_RTP_Debug.ComboBoxItem("Session: " + rtp_Session.GetHashCode(), new wfrm_RTP_Debug.RTP_SessionStatistics(rtp_Session));
				this.m_pSessions.Items.Add(item);
			}
			bool flag2 = this.m_pSessions.Items.Count > 0;
			if (flag2)
			{
				this.m_pSessions.SelectedIndex = 0;
			}
		}

		// Token: 0x0600091B RID: 2331 RVA: 0x00036978 File Offset: 0x00035978
		private void InitUI()
		{
			base.ClientSize = new Size(400, 500);
			this.Text = "RTP debug";
			base.FormClosing += this.wfrm_RTP_Debug_FormClosing;
			this.m_pTab = new TabControl();
			this.m_pTab.Dock = DockStyle.Fill;
			this.m_pTab.TabPages.Add("participants", "Participants");
			this.m_pParticipantsSplitter = new SplitContainer();
			this.m_pParticipantsSplitter.Dock = DockStyle.Fill;
			this.m_pParticipantsSplitter.Orientation = Orientation.Vertical;
			this.m_pParticipantsSplitter.SplitterDistance = 60;
			this.m_pTab.TabPages["participants"].Controls.Add(this.m_pParticipantsSplitter);
			this.m_pParticipants = new TreeView();
			this.m_pParticipants.Dock = DockStyle.Fill;
			this.m_pParticipants.BorderStyle = BorderStyle.None;
			this.m_pParticipants.FullRowSelect = true;
			this.m_pParticipants.HideSelection = false;
			this.m_pParticipants.AfterSelect += this.m_pParticipants_AfterSelect;
			TreeNode treeNode = new TreeNode(this.m_pSession.LocalParticipant.CNAME);
			treeNode.Tag = new wfrm_RTP_Debug.RTP_ParticipantInfo(this.m_pSession.LocalParticipant);
			treeNode.Nodes.Add("Sources");
			this.m_pParticipants.Nodes.Add(treeNode);
			this.m_pParticipantsSplitter.Panel1.Controls.Add(this.m_pParticipants);
			this.m_pParticipantData = new PropertyGrid();
			this.m_pParticipantData.Dock = DockStyle.Fill;
			this.m_pParticipantsSplitter.Panel2.Controls.Add(this.m_pParticipantData);
			this.m_pTab.TabPages.Add("global_statistics", "Global statistics");
			this.m_pGlobalSessionInfo = new PropertyGrid();
			this.m_pGlobalSessionInfo.Dock = DockStyle.Fill;
			this.m_pTab.TabPages["global_statistics"].Controls.Add(this.m_pGlobalSessionInfo);
			this.m_pSessions = new ComboBox();
			this.m_pSessions.Size = new Size(200, 20);
			this.m_pSessions.Location = new Point(100, 2);
			this.m_pSessions.DropDownStyle = ComboBoxStyle.DropDownList;
			this.m_pSessions.SelectedIndexChanged += this.m_pSessions_SelectedIndexChanged;
			this.m_pTab.TabPages["global_statistics"].Controls.Add(this.m_pSessions);
			this.m_pSessions.BringToFront();
			this.m_pTab.TabPages.Add("errors", "Errors");
			this.m_pErrors = new ListView();
			this.m_pErrors.Dock = DockStyle.Fill;
			this.m_pErrors.View = View.Details;
			this.m_pErrors.FullRowSelect = true;
			this.m_pErrors.HideSelection = false;
			this.m_pErrors.Columns.Add("Message", 300);
			this.m_pErrors.DoubleClick += this.m_pErrors_DoubleClick;
			this.m_pTab.TabPages["errors"].Controls.Add(this.m_pErrors);
			base.Controls.Add(this.m_pTab);
		}

		// Token: 0x0600091C RID: 2332 RVA: 0x00036CEA File Offset: 0x00035CEA
		private void m_pParticipants_AfterSelect(object sender, TreeViewEventArgs e)
		{
			this.m_pParticipantData.SelectedObject = e.Node.Tag;
		}

		// Token: 0x0600091D RID: 2333 RVA: 0x00036D04 File Offset: 0x00035D04
		private void m_pSessions_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.m_pGlobalSessionInfo.SelectedObject = ((wfrm_RTP_Debug.ComboBoxItem)this.m_pSessions.SelectedItem).Tag;
		}

		// Token: 0x0600091E RID: 2334 RVA: 0x00036D28 File Offset: 0x00035D28
		private void m_pErrors_DoubleClick(object sender, EventArgs e)
		{
			bool flag = this.m_pErrors.SelectedItems.Count > 0;
			if (flag)
			{
				MessageBox.Show(this, "Error: " + ((Exception)this.m_pErrors.SelectedItems[0].Tag).ToString(), "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}

		// Token: 0x0600091F RID: 2335 RVA: 0x00036D88 File Offset: 0x00035D88
		private void m_pSession_Error(object sender, ExceptionEventArgs e)
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				base.BeginInvoke(new MethodInvoker(delegate()
				{
					ListViewItem listViewItem = new ListViewItem(e.Exception.Message);
					listViewItem.Tag = e.Exception;
					this.m_pErrors.Items.Add(listViewItem);
				}));
			}
		}

		// Token: 0x06000920 RID: 2336 RVA: 0x00036DCC File Offset: 0x00035DCC
		private void m_pSession_SessionCreated(object sender, EventArgs<RTP_Session> e)
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				base.BeginInvoke(new MethodInvoker(delegate()
				{
					wfrm_RTP_Debug.ComboBoxItem item = new wfrm_RTP_Debug.ComboBoxItem("Session: " + e.Value.GetHashCode(), new wfrm_RTP_Debug.RTP_SessionStatistics(e.Value));
					this.m_pSessions.Items.Add(item);
					bool flag = this.m_pSessions.Items.Count > 0;
					if (flag)
					{
						this.m_pSessions.SelectedIndex = 0;
					}
				}));
			}
		}

		// Token: 0x06000921 RID: 2337 RVA: 0x00036E10 File Offset: 0x00035E10
		private void m_pSession_NewParticipant(object sender, RTP_ParticipantEventArgs e)
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				e.Participant.Removed += this.Participant_Removed;
				e.Participant.SourceAdded += this.Participant_SourceAdded;
				e.Participant.SourceRemoved += this.Participant_SourceRemoved;
				base.BeginInvoke(new MethodInvoker(delegate()
				{
					TreeNode treeNode = new TreeNode(e.Participant.CNAME);
					treeNode.Tag = new wfrm_RTP_Debug.RTP_ParticipantInfo(e.Participant);
					treeNode.Nodes.Add("Sources");
					this.m_pParticipants.Nodes.Add(treeNode);
				}));
			}
		}

		// Token: 0x06000922 RID: 2338 RVA: 0x00036EAC File Offset: 0x00035EAC
		private void Participant_Removed(object sender, EventArgs e)
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				base.BeginInvoke(new MethodInvoker(delegate()
				{
					TreeNode treeNode = this.FindParticipantNode((RTP_Participant)sender);
					bool flag = treeNode != null;
					if (flag)
					{
						treeNode.Remove();
					}
				}));
			}
		}

		// Token: 0x06000923 RID: 2339 RVA: 0x00036EF0 File Offset: 0x00035EF0
		private void Participant_SourceAdded(object sender, RTP_SourceEventArgs e)
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				e.Source.StateChanged += this.Source_StateChanged;
				base.BeginInvoke(new MethodInvoker(delegate()
				{
					bool flag = e.Source is RTP_Source_Remote;
					TreeNode treeNode;
					if (flag)
					{
						treeNode = this.FindParticipantNode(((RTP_Source_Remote)e.Source).Participant);
					}
					else
					{
						treeNode = this.FindParticipantNode(((RTP_Source_Local)e.Source).Participant);
					}
					TreeNode treeNode2 = treeNode.Nodes[0].Nodes.Add(e.Source.SSRC.ToString());
					treeNode2.Tag = new wfrm_RTP_Debug.RTP_SourceInfo(e.Source);
					bool flag2 = e.Source.State == RTP_SourceState.Active;
					if (flag2)
					{
						TreeNode treeNode3 = treeNode2.Nodes.Add("RTP Stream");
						bool flag3 = e.Source is RTP_Source_Local;
						if (flag3)
						{
							treeNode3.Tag = new wfrm_RTP_Debug.RTP_SendStreamInfo(((RTP_Source_Local)e.Source).Stream);
						}
						else
						{
							treeNode3.Tag = new wfrm_RTP_Debug.RTP_ReceiveStreamInfo(((RTP_Source_Remote)e.Source).Stream);
						}
					}
				}));
			}
		}

		// Token: 0x06000924 RID: 2340 RVA: 0x00036F50 File Offset: 0x00035F50
		private void Source_StateChanged(object sender, EventArgs e)
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				RTP_Source source = (RTP_Source)sender;
				bool flag = source.State == RTP_SourceState.Disposed;
				if (!flag)
				{
					base.BeginInvoke(new MethodInvoker(delegate()
					{
						bool flag2 = source is RTP_Source_Remote;
						TreeNode treeNode;
						if (flag2)
						{
							treeNode = this.FindParticipantNode(((RTP_Source_Remote)source).Participant);
						}
						else
						{
							treeNode = this.FindParticipantNode(((RTP_Source_Local)source).Participant);
						}
						bool flag3 = treeNode != null;
						if (flag3)
						{
							foreach (object obj in treeNode.Nodes[0].Nodes)
							{
								TreeNode treeNode2 = (TreeNode)obj;
								bool flag4 = treeNode2.Text == source.SSRC.ToString();
								if (flag4)
								{
									bool flag5 = source.State == RTP_SourceState.Active;
									if (flag5)
									{
										TreeNode treeNode3 = treeNode2.Nodes.Add("RTP Stream");
										bool flag6 = source is RTP_Source_Local;
										if (flag6)
										{
											treeNode3.Tag = new wfrm_RTP_Debug.RTP_SendStreamInfo(((RTP_Source_Local)source).Stream);
										}
										else
										{
											treeNode3.Tag = new wfrm_RTP_Debug.RTP_ReceiveStreamInfo(((RTP_Source_Remote)source).Stream);
										}
									}
									break;
								}
							}
						}
					}));
				}
			}
		}

		// Token: 0x06000925 RID: 2341 RVA: 0x00036FAC File Offset: 0x00035FAC
		private void Participant_SourceRemoved(object sender, RTP_SourceEventArgs e)
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				uint ssrc = e.Source.SSRC;
				base.BeginInvoke(new MethodInvoker(delegate()
				{
					TreeNode treeNode = this.FindParticipantNode((RTP_Participant)sender);
					bool flag = treeNode != null;
					if (flag)
					{
						foreach (object obj in treeNode.Nodes[0].Nodes)
						{
							TreeNode treeNode2 = (TreeNode)obj;
							bool flag2 = treeNode2.Text == ssrc.ToString();
							if (flag2)
							{
								treeNode2.Remove();
								break;
							}
						}
					}
				}));
			}
		}

		// Token: 0x06000926 RID: 2342 RVA: 0x00037000 File Offset: 0x00036000
		private void m_pTimer_Tick(object sender, EventArgs e)
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				bool isDisposed2 = this.m_pSession.IsDisposed;
				if (isDisposed2)
				{
					base.Visible = false;
				}
				else
				{
					this.m_pParticipantData.Refresh();
					this.m_pGlobalSessionInfo.Refresh();
				}
			}
		}

		// Token: 0x06000927 RID: 2343 RVA: 0x00037050 File Offset: 0x00036050
		private void wfrm_RTP_Debug_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.m_IsDisposed = true;
			this.m_pSession.Error -= this.m_pSession_Error;
			this.m_pSession.SessionCreated -= this.m_pSession_SessionCreated;
			this.m_pSession.NewParticipant -= this.m_pSession_NewParticipant;
			this.m_pSession.LocalParticipant.SourceAdded -= this.Participant_SourceAdded;
			this.m_pSession.LocalParticipant.SourceRemoved -= this.Participant_SourceRemoved;
			this.m_pTimer.Dispose();
		}

		// Token: 0x06000928 RID: 2344 RVA: 0x000370F4 File Offset: 0x000360F4
		private TreeNode FindParticipantNode(RTP_Participant participant)
		{
			bool flag = participant == null;
			if (flag)
			{
				throw new ArgumentNullException("participant");
			}
			foreach (object obj in this.m_pParticipants.Nodes)
			{
				TreeNode treeNode = (TreeNode)obj;
				bool flag2 = treeNode.Text == participant.CNAME;
				if (flag2)
				{
					return treeNode;
				}
			}
			return null;
		}

		// Token: 0x1700031C RID: 796
		// (get) Token: 0x06000929 RID: 2345 RVA: 0x0003718C File Offset: 0x0003618C
		public RTP_MultimediaSession Session
		{
			get
			{
				return this.m_pSession;
			}
		}

		// Token: 0x04000414 RID: 1044
		private TabControl m_pTab = null;

		// Token: 0x04000415 RID: 1045
		private SplitContainer m_pParticipantsSplitter = null;

		// Token: 0x04000416 RID: 1046
		private TreeView m_pParticipants = null;

		// Token: 0x04000417 RID: 1047
		private PropertyGrid m_pParticipantData = null;

		// Token: 0x04000418 RID: 1048
		private ComboBox m_pSessions = null;

		// Token: 0x04000419 RID: 1049
		private PropertyGrid m_pGlobalSessionInfo = null;

		// Token: 0x0400041A RID: 1050
		private ListView m_pErrors = null;

		// Token: 0x0400041B RID: 1051
		private bool m_IsDisposed = false;

		// Token: 0x0400041C RID: 1052
		private RTP_MultimediaSession m_pSession = null;

		// Token: 0x0400041D RID: 1053
		private Timer m_pTimer = null;

		// Token: 0x02000296 RID: 662
		private class ComboBoxItem
		{
			// Token: 0x06001736 RID: 5942 RVA: 0x0008FEAD File Offset: 0x0008EEAD
			public ComboBoxItem(string text, object tag)
			{
				this.m_Text = text;
				this.m_pTag = tag;
			}

			// Token: 0x06001737 RID: 5943 RVA: 0x0008FED8 File Offset: 0x0008EED8
			public override string ToString()
			{
				return this.m_Text;
			}

			// Token: 0x1700078E RID: 1934
			// (get) Token: 0x06001738 RID: 5944 RVA: 0x0008FEF0 File Offset: 0x0008EEF0
			public string Text
			{
				get
				{
					return this.m_Text;
				}
			}

			// Token: 0x1700078F RID: 1935
			// (get) Token: 0x06001739 RID: 5945 RVA: 0x0008FF08 File Offset: 0x0008EF08
			public object Tag
			{
				get
				{
					return this.m_pTag;
				}
			}

			// Token: 0x040009BA RID: 2490
			private string m_Text = "";

			// Token: 0x040009BB RID: 2491
			private object m_pTag = null;
		}

		// Token: 0x02000297 RID: 663
		private class RTP_SessionStatistics
		{
			// Token: 0x0600173A RID: 5946 RVA: 0x0008FF20 File Offset: 0x0008EF20
			public RTP_SessionStatistics(RTP_Session session)
			{
				bool flag = session == null;
				if (flag)
				{
					throw new ArgumentNullException("session");
				}
				this.m_pSession = session;
			}

			// Token: 0x17000790 RID: 1936
			// (get) Token: 0x0600173B RID: 5947 RVA: 0x0008FF58 File Offset: 0x0008EF58
			public long Members
			{
				get
				{
					return (long)this.m_pSession.Members.Length;
				}
			}

			// Token: 0x17000791 RID: 1937
			// (get) Token: 0x0600173C RID: 5948 RVA: 0x0008FF78 File Offset: 0x0008EF78
			public long Senders
			{
				get
				{
					return (long)this.m_pSession.Senders.Length;
				}
			}

			// Token: 0x17000792 RID: 1938
			// (get) Token: 0x0600173D RID: 5949 RVA: 0x0008FF98 File Offset: 0x0008EF98
			public long RtpPacketsSent
			{
				get
				{
					return this.m_pSession.RtpPacketsSent;
				}
			}

			// Token: 0x17000793 RID: 1939
			// (get) Token: 0x0600173E RID: 5950 RVA: 0x0008FFB8 File Offset: 0x0008EFB8
			public long RtpBytesSent
			{
				get
				{
					return this.m_pSession.RtpBytesSent;
				}
			}

			// Token: 0x17000794 RID: 1940
			// (get) Token: 0x0600173F RID: 5951 RVA: 0x0008FFD8 File Offset: 0x0008EFD8
			public long RtpPacketsReceived
			{
				get
				{
					return this.m_pSession.RtpPacketsReceived;
				}
			}

			// Token: 0x17000795 RID: 1941
			// (get) Token: 0x06001740 RID: 5952 RVA: 0x0008FFF8 File Offset: 0x0008EFF8
			public long RtpBytesReceived
			{
				get
				{
					return this.m_pSession.RtpBytesReceived;
				}
			}

			// Token: 0x17000796 RID: 1942
			// (get) Token: 0x06001741 RID: 5953 RVA: 0x00090018 File Offset: 0x0008F018
			public long RtpFailedTransmissions
			{
				get
				{
					return this.m_pSession.RtpFailedTransmissions;
				}
			}

			// Token: 0x17000797 RID: 1943
			// (get) Token: 0x06001742 RID: 5954 RVA: 0x00090038 File Offset: 0x0008F038
			public long RtcpPacketsSent
			{
				get
				{
					return this.m_pSession.RtcpPacketsSent;
				}
			}

			// Token: 0x17000798 RID: 1944
			// (get) Token: 0x06001743 RID: 5955 RVA: 0x00090058 File Offset: 0x0008F058
			public long RtcpBytesSent
			{
				get
				{
					return this.m_pSession.RtcpBytesSent;
				}
			}

			// Token: 0x17000799 RID: 1945
			// (get) Token: 0x06001744 RID: 5956 RVA: 0x00090078 File Offset: 0x0008F078
			public long RtcpPacketsReceived
			{
				get
				{
					return this.m_pSession.RtcpPacketsReceived;
				}
			}

			// Token: 0x1700079A RID: 1946
			// (get) Token: 0x06001745 RID: 5957 RVA: 0x00090098 File Offset: 0x0008F098
			public long RtcpBytesReceived
			{
				get
				{
					return this.m_pSession.RtcpBytesReceived;
				}
			}

			// Token: 0x1700079B RID: 1947
			// (get) Token: 0x06001746 RID: 5958 RVA: 0x000900B8 File Offset: 0x0008F0B8
			public long RtcpFailedTransmissions
			{
				get
				{
					return this.m_pSession.RtcpFailedTransmissions;
				}
			}

			// Token: 0x1700079C RID: 1948
			// (get) Token: 0x06001747 RID: 5959 RVA: 0x000900D8 File Offset: 0x0008F0D8
			public int RtcpInterval
			{
				get
				{
					return this.m_pSession.RtcpInterval;
				}
			}

			// Token: 0x1700079D RID: 1949
			// (get) Token: 0x06001748 RID: 5960 RVA: 0x000900F8 File Offset: 0x0008F0F8
			public string RtcpLastTransmission
			{
				get
				{
					return this.m_pSession.RtcpLastTransmission.ToString("HH:mm:ss");
				}
			}

			// Token: 0x1700079E RID: 1950
			// (get) Token: 0x06001749 RID: 5961 RVA: 0x00090124 File Offset: 0x0008F124
			public long LocalCollisions
			{
				get
				{
					return this.m_pSession.LocalCollisions;
				}
			}

			// Token: 0x1700079F RID: 1951
			// (get) Token: 0x0600174A RID: 5962 RVA: 0x00090144 File Offset: 0x0008F144
			public long RemoteCollisions
			{
				get
				{
					return this.m_pSession.RemoteCollisions;
				}
			}

			// Token: 0x170007A0 RID: 1952
			// (get) Token: 0x0600174B RID: 5963 RVA: 0x00090164 File Offset: 0x0008F164
			public long LocalPacketsLooped
			{
				get
				{
					return this.m_pSession.LocalPacketsLooped;
				}
			}

			// Token: 0x170007A1 RID: 1953
			// (get) Token: 0x0600174C RID: 5964 RVA: 0x00090184 File Offset: 0x0008F184
			public long RemotePacketsLooped
			{
				get
				{
					return this.m_pSession.RemotePacketsLooped;
				}
			}

			// Token: 0x170007A2 RID: 1954
			// (get) Token: 0x0600174D RID: 5965 RVA: 0x000901A4 File Offset: 0x0008F1A4
			public string Payload
			{
				get
				{
					int payload = this.m_pSession.Payload;
					Codec codec = null;
					this.m_pSession.Payloads.TryGetValue(payload, out codec);
					bool flag = codec == null;
					string result;
					if (flag)
					{
						result = payload.ToString();
					}
					else
					{
						result = payload.ToString() + " - " + codec.Name;
					}
					return result;
				}
			}

			// Token: 0x170007A3 RID: 1955
			// (get) Token: 0x0600174E RID: 5966 RVA: 0x00090204 File Offset: 0x0008F204
			public string[] Targets
			{
				get
				{
					List<string> list = new List<string>();
					foreach (RTP_Address rtp_Address in this.m_pSession.Targets)
					{
						list.Add(string.Concat(new object[]
						{
							rtp_Address.IP,
							":",
							rtp_Address.DataPort,
							"/",
							rtp_Address.ControlPort
						}));
					}
					return list.ToArray();
				}
			}

			// Token: 0x170007A4 RID: 1956
			// (get) Token: 0x0600174F RID: 5967 RVA: 0x0009028C File Offset: 0x0008F28C
			public string LocalEP
			{
				get
				{
					return string.Concat(new object[]
					{
						this.m_pSession.LocalEP.IP,
						":",
						this.m_pSession.LocalEP.DataPort,
						"/",
						this.m_pSession.LocalEP.ControlPort
					});
				}
			}

			// Token: 0x170007A5 RID: 1957
			// (get) Token: 0x06001750 RID: 5968 RVA: 0x000902FC File Offset: 0x0008F2FC
			public string StreamMode
			{
				get
				{
					return this.m_pSession.StreamMode.ToString();
				}
			}

			// Token: 0x040009BC RID: 2492
			private RTP_Session m_pSession = null;
		}

		// Token: 0x02000298 RID: 664
		private class RTP_ParticipantInfo
		{
			// Token: 0x06001751 RID: 5969 RVA: 0x00090328 File Offset: 0x0008F328
			public RTP_ParticipantInfo(RTP_Participant participant)
			{
				bool flag = participant == null;
				if (flag)
				{
					throw new ArgumentNullException("participant");
				}
				this.m_pParticipant = participant;
			}

			// Token: 0x170007A6 RID: 1958
			// (get) Token: 0x06001752 RID: 5970 RVA: 0x00090360 File Offset: 0x0008F360
			public string Name
			{
				get
				{
					bool flag = this.m_pParticipant is RTP_Participant_Local;
					string name;
					if (flag)
					{
						name = ((RTP_Participant_Local)this.m_pParticipant).Name;
					}
					else
					{
						name = ((RTP_Participant_Remote)this.m_pParticipant).Name;
					}
					return name;
				}
			}

			// Token: 0x170007A7 RID: 1959
			// (get) Token: 0x06001753 RID: 5971 RVA: 0x000903AC File Offset: 0x0008F3AC
			public string Email
			{
				get
				{
					bool flag = this.m_pParticipant is RTP_Participant_Local;
					string email;
					if (flag)
					{
						email = ((RTP_Participant_Local)this.m_pParticipant).Email;
					}
					else
					{
						email = ((RTP_Participant_Remote)this.m_pParticipant).Email;
					}
					return email;
				}
			}

			// Token: 0x170007A8 RID: 1960
			// (get) Token: 0x06001754 RID: 5972 RVA: 0x000903F8 File Offset: 0x0008F3F8
			public string Phone
			{
				get
				{
					bool flag = this.m_pParticipant is RTP_Participant_Local;
					string phone;
					if (flag)
					{
						phone = ((RTP_Participant_Local)this.m_pParticipant).Phone;
					}
					else
					{
						phone = ((RTP_Participant_Remote)this.m_pParticipant).Phone;
					}
					return phone;
				}
			}

			// Token: 0x170007A9 RID: 1961
			// (get) Token: 0x06001755 RID: 5973 RVA: 0x00090444 File Offset: 0x0008F444
			public string Location
			{
				get
				{
					bool flag = this.m_pParticipant is RTP_Participant_Local;
					string location;
					if (flag)
					{
						location = ((RTP_Participant_Local)this.m_pParticipant).Location;
					}
					else
					{
						location = ((RTP_Participant_Remote)this.m_pParticipant).Location;
					}
					return location;
				}
			}

			// Token: 0x170007AA RID: 1962
			// (get) Token: 0x06001756 RID: 5974 RVA: 0x00090490 File Offset: 0x0008F490
			public string Tool
			{
				get
				{
					bool flag = this.m_pParticipant is RTP_Participant_Local;
					string tool;
					if (flag)
					{
						tool = ((RTP_Participant_Local)this.m_pParticipant).Tool;
					}
					else
					{
						tool = ((RTP_Participant_Remote)this.m_pParticipant).Tool;
					}
					return tool;
				}
			}

			// Token: 0x170007AB RID: 1963
			// (get) Token: 0x06001757 RID: 5975 RVA: 0x000904DC File Offset: 0x0008F4DC
			public string Note
			{
				get
				{
					bool flag = this.m_pParticipant is RTP_Participant_Local;
					string note;
					if (flag)
					{
						note = ((RTP_Participant_Local)this.m_pParticipant).Note;
					}
					else
					{
						note = ((RTP_Participant_Remote)this.m_pParticipant).Note;
					}
					return note;
				}
			}

			// Token: 0x040009BD RID: 2493
			private RTP_Participant m_pParticipant = null;
		}

		// Token: 0x02000299 RID: 665
		private class RTP_SourceInfo
		{
			// Token: 0x06001758 RID: 5976 RVA: 0x00090528 File Offset: 0x0008F528
			public RTP_SourceInfo(RTP_Source source)
			{
				bool flag = source == null;
				if (flag)
				{
					throw new ArgumentNullException("source");
				}
				this.m_pSource = source;
			}

			// Token: 0x170007AC RID: 1964
			// (get) Token: 0x06001759 RID: 5977 RVA: 0x00090560 File Offset: 0x0008F560
			public RTP_SourceState State
			{
				get
				{
					return this.m_pSource.State;
				}
			}

			// Token: 0x170007AD RID: 1965
			// (get) Token: 0x0600175A RID: 5978 RVA: 0x00090580 File Offset: 0x0008F580
			public int Session
			{
				get
				{
					return this.m_pSource.Session.GetHashCode();
				}
			}

			// Token: 0x170007AE RID: 1966
			// (get) Token: 0x0600175B RID: 5979 RVA: 0x000905A4 File Offset: 0x0008F5A4
			public uint SSRC
			{
				get
				{
					return this.m_pSource.SSRC;
				}
			}

			// Token: 0x170007AF RID: 1967
			// (get) Token: 0x0600175C RID: 5980 RVA: 0x000905C4 File Offset: 0x0008F5C4
			public IPEndPoint RtcpEP
			{
				get
				{
					return this.m_pSource.RtcpEP;
				}
			}

			// Token: 0x170007B0 RID: 1968
			// (get) Token: 0x0600175D RID: 5981 RVA: 0x000905E4 File Offset: 0x0008F5E4
			public IPEndPoint RtpEP
			{
				get
				{
					return this.m_pSource.RtpEP;
				}
			}

			// Token: 0x170007B1 RID: 1969
			// (get) Token: 0x0600175E RID: 5982 RVA: 0x00090604 File Offset: 0x0008F604
			public string LastActivity
			{
				get
				{
					return this.m_pSource.LastActivity.ToString("HH:mm:ss");
				}
			}

			// Token: 0x170007B2 RID: 1970
			// (get) Token: 0x0600175F RID: 5983 RVA: 0x00090630 File Offset: 0x0008F630
			public string LastRtcpPacket
			{
				get
				{
					return this.m_pSource.LastRtcpPacket.ToString("HH:mm:ss");
				}
			}

			// Token: 0x170007B3 RID: 1971
			// (get) Token: 0x06001760 RID: 5984 RVA: 0x0009065C File Offset: 0x0008F65C
			public string LastRtpPacket
			{
				get
				{
					return this.m_pSource.LastRtpPacket.ToString("HH:mm:ss");
				}
			}

			// Token: 0x040009BE RID: 2494
			private RTP_Source m_pSource = null;
		}

		// Token: 0x0200029A RID: 666
		private class RTP_ReceiveStreamInfo
		{
			// Token: 0x06001761 RID: 5985 RVA: 0x00090688 File Offset: 0x0008F688
			public RTP_ReceiveStreamInfo(RTP_ReceiveStream stream)
			{
				bool flag = stream == null;
				if (flag)
				{
					throw new ArgumentNullException("stream");
				}
				this.m_pStream = stream;
			}

			// Token: 0x170007B4 RID: 1972
			// (get) Token: 0x06001762 RID: 5986 RVA: 0x000906C0 File Offset: 0x0008F6C0
			public int Session
			{
				get
				{
					return this.m_pStream.Session.GetHashCode();
				}
			}

			// Token: 0x170007B5 RID: 1973
			// (get) Token: 0x06001763 RID: 5987 RVA: 0x000906E4 File Offset: 0x0008F6E4
			public int SeqNoWrapCount
			{
				get
				{
					return this.m_pStream.SeqNoWrapCount;
				}
			}

			// Token: 0x170007B6 RID: 1974
			// (get) Token: 0x06001764 RID: 5988 RVA: 0x00090704 File Offset: 0x0008F704
			public int FirstSeqNo
			{
				get
				{
					return this.m_pStream.FirstSeqNo;
				}
			}

			// Token: 0x170007B7 RID: 1975
			// (get) Token: 0x06001765 RID: 5989 RVA: 0x00090724 File Offset: 0x0008F724
			public int MaxSeqNo
			{
				get
				{
					return this.m_pStream.MaxSeqNo;
				}
			}

			// Token: 0x170007B8 RID: 1976
			// (get) Token: 0x06001766 RID: 5990 RVA: 0x00090744 File Offset: 0x0008F744
			public long PacketsReceived
			{
				get
				{
					return this.m_pStream.PacketsReceived;
				}
			}

			// Token: 0x170007B9 RID: 1977
			// (get) Token: 0x06001767 RID: 5991 RVA: 0x00090764 File Offset: 0x0008F764
			public long PacketsMisorder
			{
				get
				{
					return this.m_pStream.PacketsMisorder;
				}
			}

			// Token: 0x170007BA RID: 1978
			// (get) Token: 0x06001768 RID: 5992 RVA: 0x00090784 File Offset: 0x0008F784
			public long BytesReceived
			{
				get
				{
					return this.m_pStream.BytesReceived;
				}
			}

			// Token: 0x170007BB RID: 1979
			// (get) Token: 0x06001769 RID: 5993 RVA: 0x000907A4 File Offset: 0x0008F7A4
			public long PacketsLost
			{
				get
				{
					return this.m_pStream.PacketsLost;
				}
			}

			// Token: 0x170007BC RID: 1980
			// (get) Token: 0x0600176A RID: 5994 RVA: 0x000907C4 File Offset: 0x0008F7C4
			public double Jitter
			{
				get
				{
					return this.m_pStream.Jitter;
				}
			}

			// Token: 0x170007BD RID: 1981
			// (get) Token: 0x0600176B RID: 5995 RVA: 0x000907E4 File Offset: 0x0008F7E4
			public string LastSRTime
			{
				get
				{
					return this.m_pStream.LastSRTime.ToString("HH:mm:ss");
				}
			}

			// Token: 0x170007BE RID: 1982
			// (get) Token: 0x0600176C RID: 5996 RVA: 0x00090810 File Offset: 0x0008F810
			public int DelaySinceLastSR
			{
				get
				{
					return this.m_pStream.DelaySinceLastSR / 1000;
				}
			}

			// Token: 0x040009BF RID: 2495
			private RTP_ReceiveStream m_pStream = null;
		}

		// Token: 0x0200029B RID: 667
		private class RTP_SendStreamInfo
		{
			// Token: 0x0600176D RID: 5997 RVA: 0x00090834 File Offset: 0x0008F834
			public RTP_SendStreamInfo(RTP_SendStream stream)
			{
				bool flag = stream == null;
				if (flag)
				{
					throw new ArgumentNullException("stream");
				}
				this.m_pStream = stream;
			}

			// Token: 0x170007BF RID: 1983
			// (get) Token: 0x0600176E RID: 5998 RVA: 0x0009086C File Offset: 0x0008F86C
			public int Session
			{
				get
				{
					return this.m_pStream.Session.GetHashCode();
				}
			}

			// Token: 0x170007C0 RID: 1984
			// (get) Token: 0x0600176F RID: 5999 RVA: 0x00090890 File Offset: 0x0008F890
			public int SeqNoWrapCount
			{
				get
				{
					return this.m_pStream.SeqNoWrapCount;
				}
			}

			// Token: 0x170007C1 RID: 1985
			// (get) Token: 0x06001770 RID: 6000 RVA: 0x000908B0 File Offset: 0x0008F8B0
			public int SeqNo
			{
				get
				{
					return this.m_pStream.SeqNo;
				}
			}

			// Token: 0x170007C2 RID: 1986
			// (get) Token: 0x06001771 RID: 6001 RVA: 0x000908D0 File Offset: 0x0008F8D0
			public string LastPacketTime
			{
				get
				{
					return this.m_pStream.LastPacketTime.ToString("HH:mm:ss");
				}
			}

			// Token: 0x170007C3 RID: 1987
			// (get) Token: 0x06001772 RID: 6002 RVA: 0x000908FC File Offset: 0x0008F8FC
			public uint LastPacketRtpTimestamp
			{
				get
				{
					return this.m_pStream.LastPacketRtpTimestamp;
				}
			}

			// Token: 0x170007C4 RID: 1988
			// (get) Token: 0x06001773 RID: 6003 RVA: 0x0009091C File Offset: 0x0008F91C
			public long RtpPacketsSent
			{
				get
				{
					return this.m_pStream.RtpPacketsSent;
				}
			}

			// Token: 0x170007C5 RID: 1989
			// (get) Token: 0x06001774 RID: 6004 RVA: 0x0009093C File Offset: 0x0008F93C
			public long RtpBytesSent
			{
				get
				{
					return this.m_pStream.RtpBytesSent;
				}
			}

			// Token: 0x170007C6 RID: 1990
			// (get) Token: 0x06001775 RID: 6005 RVA: 0x0009095C File Offset: 0x0008F95C
			public long RtpDataBytesSent
			{
				get
				{
					return this.m_pStream.RtpDataBytesSent;
				}
			}

			// Token: 0x040009C0 RID: 2496
			private RTP_SendStream m_pStream = null;
		}
	}
}
