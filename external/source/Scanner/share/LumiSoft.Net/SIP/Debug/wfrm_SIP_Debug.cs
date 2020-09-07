using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using LumiSoft.Net.Log;
using LumiSoft.Net.SIP.Stack;

namespace LumiSoft.Net.SIP.Debug
{
	// Token: 0x020000B8 RID: 184
	public partial class wfrm_SIP_Debug : Form
	{
		// Token: 0x06000713 RID: 1811 RVA: 0x0002ACA0 File Offset: 0x00029CA0
		public wfrm_SIP_Debug(SIP_Stack stack)
		{
			bool flag = stack == null;
			if (flag)
			{
				throw new ArgumentNullException("stack");
			}
			this.m_pStack = stack;
			this.m_pStack.Logger.WriteLog += this.Logger_WriteLog;
			this.InitUI();
		}

		// Token: 0x06000714 RID: 1812 RVA: 0x0002AD44 File Offset: 0x00029D44
		private void InitUI()
		{
			base.ClientSize = new Size(600, 300);
			this.Text = "SIP Debug";
			base.FormClosed += this.wfrm_Debug_FormClosed;
			this.m_pTab = new TabControl();
			this.m_pTab.Dock = DockStyle.Fill;
			this.m_pTab.TabPages.Add("log", "Log");
			this.m_pTabLog_Toolbar = new ToolStrip();
			this.m_pTabLog_Toolbar.Dock = DockStyle.Top;
			this.m_pTab.TabPages["log"].Controls.Add(this.m_pTabLog_Toolbar);
			ToolStripButton tabLog_Toolbar_Log = new ToolStripButton("Log");
			tabLog_Toolbar_Log.Name = "log";
			tabLog_Toolbar_Log.Tag = "log";
			tabLog_Toolbar_Log.Checked = true;
			tabLog_Toolbar_Log.Click += delegate(object sender, EventArgs e)
			{
				tabLog_Toolbar_Log.Checked = !tabLog_Toolbar_Log.Checked;
			};
			this.m_pTabLog_Toolbar.Items.Add(tabLog_Toolbar_Log);
			ToolStripButton tabLog_Toolbar_LogData = new ToolStripButton("Log Data");
			tabLog_Toolbar_LogData.Name = "logdata";
			tabLog_Toolbar_LogData.Tag = "logdata";
			tabLog_Toolbar_LogData.Checked = true;
			tabLog_Toolbar_LogData.Click += delegate(object sender, EventArgs e)
			{
				tabLog_Toolbar_LogData.Checked = !tabLog_Toolbar_LogData.Checked;
			};
			this.m_pTabLog_Toolbar.Items.Add(tabLog_Toolbar_LogData);
			ToolStripButton toolStripButton = new ToolStripButton("Clear");
			toolStripButton.Tag = "clear";
			toolStripButton.Click += this.m_pTabLog_Toolbar_Click;
			this.m_pTabLog_Toolbar.Items.Add(toolStripButton);
			this.m_pTabLog_Toolbar.Items.Add(new ToolStripLabel("Filter:"));
			ToolStripTextBox toolStripTextBox = new ToolStripTextBox();
			toolStripTextBox.Name = "filter";
			toolStripTextBox.AutoSize = false;
			toolStripTextBox.Size = new Size(150, 20);
			this.m_pTabLog_Toolbar.Items.Add(toolStripTextBox);
			this.m_pTabLog_Text = new RichTextBox();
			this.m_pTabLog_Text.Size = new Size(this.m_pTab.TabPages["log"].Width, this.m_pTab.TabPages["log"].Height - 25);
			this.m_pTabLog_Text.Location = new Point(0, 25);
			this.m_pTabLog_Text.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pTabLog_Text.BorderStyle = BorderStyle.None;
			this.m_pTab.TabPages["log"].Controls.Add(this.m_pTabLog_Text);
			this.m_pTab.TabPages.Add("transactions", "Transactions");
			this.m_pTabTransactions_Toolbar = new ToolStrip();
			this.m_pTabTransactions_Toolbar.Dock = DockStyle.Top;
			ToolStripButton toolStripButton2 = new ToolStripButton("Refresh");
			toolStripButton2.Tag = "refresh";
			toolStripButton2.Click += this.m_pTabTransactions_Toolbar_Click;
			this.m_pTabTransactions_Toolbar.Items.Add(toolStripButton2);
			this.m_pTab.TabPages["transactions"].Controls.Add(this.m_pTabTransactions_Toolbar);
			this.m_pTabTransactions_List = new ListView();
			this.m_pTabTransactions_List.Size = new Size(this.m_pTab.TabPages["transactions"].Width, this.m_pTab.TabPages["transactions"].Height - 25);
			this.m_pTabTransactions_List.Location = new Point(0, 25);
			this.m_pTabTransactions_List.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pTabTransactions_List.View = View.Details;
			this.m_pTabTransactions_List.Columns.Add("Is Server");
			this.m_pTabTransactions_List.Columns.Add("Method", 80);
			this.m_pTabTransactions_List.Columns.Add("State", 80);
			this.m_pTabTransactions_List.Columns.Add("Create Time", 80);
			this.m_pTabTransactions_List.Columns.Add("ID", 100);
			this.m_pTab.TabPages["transactions"].Controls.Add(this.m_pTabTransactions_List);
			this.m_pTab.TabPages.Add("dialogs", "Dialogs");
			this.m_pTabDialogs_Toolbar = new ToolStrip();
			this.m_pTabDialogs_Toolbar.Dock = DockStyle.Top;
			ToolStripButton toolStripButton3 = new ToolStripButton("Refresh");
			toolStripButton3.Tag = "refresh";
			toolStripButton3.Click += this.m_pTabDialogs_Toolbar_Click;
			this.m_pTabDialogs_Toolbar.Items.Add(toolStripButton3);
			this.m_pTab.TabPages["dialogs"].Controls.Add(this.m_pTabDialogs_Toolbar);
			this.m_pTabDialogs_List = new ListView();
			this.m_pTabDialogs_List.Size = new Size(this.m_pTab.TabPages["dialogs"].Width, this.m_pTab.TabPages["dialogs"].Height - 25);
			this.m_pTabDialogs_List.Location = new Point(0, 25);
			this.m_pTabDialogs_List.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pTabDialogs_List.View = View.Details;
			this.m_pTabDialogs_List.Columns.Add("Type", 80);
			this.m_pTabDialogs_List.Columns.Add("State", 80);
			this.m_pTabDialogs_List.Columns.Add("Create Time", 100);
			this.m_pTabDialogs_List.Columns.Add("ID", 120);
			this.m_pTabDialogs_List.DoubleClick += this.m_pTabDialogs_List_DoubleClick;
			this.m_pTab.TabPages["dialogs"].Controls.Add(this.m_pTabDialogs_List);
			this.m_pTab.TabPages.Add("flows", "Flows");
			this.m_pTabFlows_Toolbar = new ToolStrip();
			this.m_pTabFlows_Toolbar.Dock = DockStyle.Top;
			ToolStripButton toolStripButton4 = new ToolStripButton("Refresh");
			toolStripButton4.Tag = "refresh";
			toolStripButton4.Click += this.m_pTabFlows_Toolbar_Click;
			this.m_pTabFlows_Toolbar.Items.Add(toolStripButton4);
			this.m_pTab.TabPages["flows"].Controls.Add(this.m_pTabFlows_Toolbar);
			this.m_pTabFlows_List = new ListView();
			this.m_pTabFlows_List.Size = new Size(this.m_pTab.TabPages["flows"].Width, this.m_pTab.TabPages["flows"].Height - 25);
			this.m_pTabFlows_List.Location = new Point(0, 25);
			this.m_pTabFlows_List.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.m_pTabFlows_List.View = View.Details;
			this.m_pTabFlows_List.Columns.Add("Transport");
			this.m_pTabFlows_List.Columns.Add("Local EP", 130);
			this.m_pTabFlows_List.Columns.Add("Remote EP", 130);
			this.m_pTabFlows_List.Columns.Add("Last Activity", 80);
			this.m_pTabFlows_List.Columns.Add("Public EP", 130);
			this.m_pTab.TabPages["flows"].Controls.Add(this.m_pTabFlows_List);
			base.Controls.Add(this.m_pTab);
		}

		// Token: 0x06000715 RID: 1813 RVA: 0x0002B558 File Offset: 0x0002A558
		private void Logger_WriteLog(object sender, WriteLogEventArgs e)
		{
			bool flag = !base.Visible;
			if (!flag)
			{
				this.m_pTabLog_Text.BeginInvoke(new MethodInvoker(delegate()
				{
					bool flag2 = !((ToolStripButton)this.m_pTabLog_Toolbar.Items["log"]).Checked;
					if (!flag2)
					{
						string text = e.LogEntry.Text + "\n";
						bool flag3 = ((ToolStripButton)this.m_pTabLog_Toolbar.Items["logdata"]).Checked && e.LogEntry.Data != null;
						if (flag3)
						{
							text = text + "<begin>\r\n" + Encoding.Default.GetString(e.LogEntry.Data) + "<end>\r\n";
						}
						bool flag4 = !wfrm_SIP_Debug.IsAstericMatch(this.m_pTabLog_Toolbar.Items["filter"].Text, text);
						if (!flag4)
						{
							bool oddLogEntry = this.m_OddLogEntry;
							if (oddLogEntry)
							{
								this.m_OddLogEntry = false;
								this.m_pTabLog_Text.SelectionColor = Color.Gray;
							}
							else
							{
								this.m_OddLogEntry = true;
								this.m_pTabLog_Text.SelectionColor = Color.LightSeaGreen;
							}
							this.m_pTabLog_Text.AppendText(text);
						}
					}
				}));
			}
		}

		// Token: 0x06000716 RID: 1814 RVA: 0x0002B5A4 File Offset: 0x0002A5A4
		private void m_pTabLog_Toolbar_Click(object sender, EventArgs e)
		{
			ToolStripButton toolStripButton = (ToolStripButton)sender;
			bool flag = toolStripButton.Tag.ToString() == "clear";
			if (flag)
			{
				this.m_pTabLog_Text.Text = "";
			}
		}

		// Token: 0x06000717 RID: 1815 RVA: 0x0002B5E8 File Offset: 0x0002A5E8
		private void m_pTabTransactions_Toolbar_Click(object sender, EventArgs e)
		{
			ToolStripButton toolStripButton = (ToolStripButton)sender;
			bool flag = toolStripButton.Tag.ToString() == "refresh";
			if (flag)
			{
				this.m_pTabTransactions_List.Items.Clear();
				foreach (SIP_ClientTransaction sip_ClientTransaction in this.m_pStack.TransactionLayer.ClientTransactions)
				{
					try
					{
						ListViewItem listViewItem = new ListViewItem("false");
						listViewItem.SubItems.Add(sip_ClientTransaction.Method);
						listViewItem.SubItems.Add(sip_ClientTransaction.State.ToString());
						listViewItem.SubItems.Add(sip_ClientTransaction.CreateTime.ToString("HH:mm:ss"));
						listViewItem.SubItems.Add(sip_ClientTransaction.ID);
						this.m_pTabTransactions_List.Items.Add(listViewItem);
					}
					catch
					{
					}
				}
				foreach (SIP_ServerTransaction sip_ServerTransaction in this.m_pStack.TransactionLayer.ServerTransactions)
				{
					try
					{
						ListViewItem listViewItem2 = new ListViewItem("true");
						listViewItem2.SubItems.Add(sip_ServerTransaction.Method);
						listViewItem2.SubItems.Add(sip_ServerTransaction.State.ToString());
						listViewItem2.SubItems.Add(sip_ServerTransaction.CreateTime.ToString("HH:mm:ss"));
						listViewItem2.SubItems.Add(sip_ServerTransaction.ID);
						this.m_pTabTransactions_List.Items.Add(listViewItem2);
					}
					catch
					{
					}
				}
			}
		}

		// Token: 0x06000718 RID: 1816 RVA: 0x0002B7D4 File Offset: 0x0002A7D4
		private void m_pTabDialogs_Toolbar_Click(object sender, EventArgs e)
		{
			ToolStripButton toolStripButton = (ToolStripButton)sender;
			bool flag = toolStripButton.Tag.ToString() == "refresh";
			if (flag)
			{
				this.m_pTabDialogs_List.Items.Clear();
				foreach (SIP_Dialog sip_Dialog in this.m_pStack.TransactionLayer.Dialogs)
				{
					try
					{
						ListViewItem listViewItem = new ListViewItem((sip_Dialog is SIP_Dialog_Invite) ? "INVITE" : "");
						listViewItem.SubItems.Add(sip_Dialog.State.ToString());
						listViewItem.SubItems.Add(sip_Dialog.CreateTime.ToString());
						listViewItem.SubItems.Add(sip_Dialog.ID);
						listViewItem.Tag = sip_Dialog;
						this.m_pTabDialogs_List.Items.Add(listViewItem);
					}
					catch
					{
					}
				}
			}
		}

		// Token: 0x06000719 RID: 1817 RVA: 0x0002B8EC File Offset: 0x0002A8EC
		private void m_pTabDialogs_List_DoubleClick(object sender, EventArgs e)
		{
			bool flag = this.m_pTabDialogs_List.SelectedItems.Count == 0;
			if (!flag)
			{
				Form form = new Form();
				form.Size = new Size(400, 500);
				form.StartPosition = FormStartPosition.CenterScreen;
				form.Text = "Dialog Properties";
				PropertyGrid propertyGrid = new PropertyGrid();
				propertyGrid.Dock = DockStyle.Fill;
				propertyGrid.SelectedObject = this.m_pTabDialogs_List.SelectedItems[0].Tag;
				form.Controls.Add(propertyGrid);
				form.Show();
			}
		}

		// Token: 0x0600071A RID: 1818 RVA: 0x0002B984 File Offset: 0x0002A984
		private void m_pTabFlows_Toolbar_Click(object sender, EventArgs e)
		{
			ToolStripButton toolStripButton = (ToolStripButton)sender;
			bool flag = toolStripButton.Tag.ToString() == "refresh";
			if (flag)
			{
				this.m_pTabFlows_List.Items.Clear();
				foreach (SIP_Flow sip_Flow in this.m_pStack.TransportLayer.Flows)
				{
					try
					{
						ListViewItem listViewItem = new ListViewItem(sip_Flow.Transport);
						listViewItem.SubItems.Add(sip_Flow.LocalEP.ToString());
						listViewItem.SubItems.Add(sip_Flow.RemoteEP.ToString());
						listViewItem.SubItems.Add(sip_Flow.LastActivity.ToString("HH:mm:ss"));
						listViewItem.SubItems.Add(sip_Flow.LocalPublicEP.ToString());
						this.m_pTabFlows_List.Items.Add(listViewItem);
					}
					catch
					{
					}
				}
			}
		}

		// Token: 0x0600071B RID: 1819 RVA: 0x0002BA9C File Offset: 0x0002AA9C
		private void wfrm_Debug_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.m_pStack.Logger.WriteLog -= this.Logger_WriteLog;
		}

		// Token: 0x0600071C RID: 1820 RVA: 0x0002BABC File Offset: 0x0002AABC
		public static bool IsAstericMatch(string pattern, string text)
		{
			pattern = pattern.ToLower();
			text = text.ToLower();
			bool flag = pattern == "";
			if (flag)
			{
				pattern = "*";
			}
			while (pattern.Length > 0)
			{
				bool flag2 = pattern.StartsWith("*");
				if (flag2)
				{
					bool flag3 = pattern.IndexOf("*", 1) > -1;
					if (!flag3)
					{
						return text.EndsWith(pattern.Substring(1));
					}
					string text2 = pattern.Substring(1, pattern.IndexOf("*", 1) - 1);
					bool flag4 = text.IndexOf(text2) == -1;
					if (flag4)
					{
						return false;
					}
					text = text.Substring(text.IndexOf(text2) + text2.Length);
					pattern = pattern.Substring(pattern.IndexOf("*", 1));
				}
				else
				{
					bool flag5 = pattern.IndexOfAny(new char[]
					{
						'*'
					}) > -1;
					if (!flag5)
					{
						return text == pattern;
					}
					string text3 = pattern.Substring(0, pattern.IndexOfAny(new char[]
					{
						'*'
					}));
					bool flag6 = !text.StartsWith(text3);
					if (flag6)
					{
						return false;
					}
					text = text.Substring(text.IndexOf(text3) + text3.Length);
					pattern = pattern.Substring(pattern.IndexOfAny(new char[]
					{
						'*'
					}));
				}
			}
			return true;
		}

		// Token: 0x040002FA RID: 762
		private TabControl m_pTab = null;

		// Token: 0x040002FB RID: 763
		private ToolStrip m_pTabLog_Toolbar = null;

		// Token: 0x040002FC RID: 764
		private RichTextBox m_pTabLog_Text = null;

		// Token: 0x040002FD RID: 765
		private ToolStrip m_pTabTransactions_Toolbar = null;

		// Token: 0x040002FE RID: 766
		private ListView m_pTabTransactions_List = null;

		// Token: 0x040002FF RID: 767
		private ToolStrip m_pTabDialogs_Toolbar = null;

		// Token: 0x04000300 RID: 768
		private ListView m_pTabDialogs_List = null;

		// Token: 0x04000301 RID: 769
		private ToolStrip m_pTabFlows_Toolbar = null;

		// Token: 0x04000302 RID: 770
		private ListView m_pTabFlows_List = null;

		// Token: 0x04000303 RID: 771
		private SIP_Stack m_pStack = null;

		// Token: 0x04000304 RID: 772
		private bool m_OddLogEntry = false;
	}
}
