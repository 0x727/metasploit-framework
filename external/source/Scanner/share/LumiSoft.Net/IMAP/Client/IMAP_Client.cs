using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Security.Principal;
using System.Text;
using System.Threading;
using LumiSoft.Net.AUTH;
using LumiSoft.Net.IO;
using LumiSoft.Net.MIME;
using LumiSoft.Net.TCP;

namespace LumiSoft.Net.IMAP.Client
{
	// Token: 0x0200022B RID: 555
	public class IMAP_Client : TCP_Client
	{
		// Token: 0x060013AC RID: 5036 RVA: 0x00078404 File Offset: 0x00077404
		public IMAP_Client()
		{
			this.m_pSettings = new IMAP_Client.SettingsHolder();
		}

		// Token: 0x060013AD RID: 5037 RVA: 0x0007847C File Offset: 0x0007747C
		public override void Disconnect()
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("IMAP client is not connected.");
			}
			try
			{
				int commandIndex = this.m_CommandIndex;
				this.m_CommandIndex = commandIndex + 1;
				base.WriteLine(commandIndex.ToString("d5") + " LOGOUT");
			}
			catch
			{
			}
			try
			{
				base.Disconnect();
			}
			catch
			{
			}
			this.m_pAuthenticatedUser = null;
			this.m_GreetingText = "";
			this.m_CommandIndex = 1;
			this.m_pCapabilities = null;
			this.m_pSelectedFolder = null;
			this.m_MailboxEncoding = IMAP_Mailbox_Encoding.ImapUtf7;
			this.m_pSettings = null;
		}

		// Token: 0x060013AE RID: 5038 RVA: 0x00078558 File Offset: 0x00077558
		public void StartTls()
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool isSecureConnection = this.IsSecureConnection;
			if (isSecureConnection)
			{
				throw new InvalidOperationException("Connection is already secure.");
			}
			bool isAuthenticated = base.IsAuthenticated;
			if (isAuthenticated)
			{
				throw new InvalidOperationException("STARTTLS is only valid in not-authenticated state.");
			}
			bool flag2 = this.m_pIdle != null;
			if (flag2)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			using (IMAP_Client.StartTlsAsyncOP startTlsAsyncOP = new IMAP_Client.StartTlsAsyncOP(null, null))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					startTlsAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.StartTlsAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag3 = !this.StartTlsAsync(startTlsAsyncOP);
					if (flag3)
					{
						wait.Set();
					}
					wait.WaitOne();
					bool flag4 = startTlsAsyncOP.Error != null;
					if (flag4)
					{
						throw startTlsAsyncOP.Error;
					}
				}
			}
		}

		// Token: 0x060013AF RID: 5039 RVA: 0x000786A4 File Offset: 0x000776A4
		public bool StartTlsAsync(IMAP_Client.StartTlsAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool isSecureConnection = this.IsSecureConnection;
			if (isSecureConnection)
			{
				throw new InvalidOperationException("Connection is already secure.");
			}
			bool isAuthenticated = base.IsAuthenticated;
			if (isAuthenticated)
			{
				throw new InvalidOperationException("STARTTLS is only valid in not-authenticated state.");
			}
			bool flag2 = this.m_pIdle != null;
			if (flag2)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag3 = op == null;
			if (flag3)
			{
				throw new ArgumentNullException("op");
			}
			bool flag4 = op.State > AsyncOP_State.WaitingForStart;
			if (flag4)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013B0 RID: 5040 RVA: 0x00078774 File Offset: 0x00077774
		public void Login(string user, string password)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool isAuthenticated = base.IsAuthenticated;
			if (isAuthenticated)
			{
				throw new InvalidOperationException("Re-authentication error, you are already authenticated.");
			}
			bool flag2 = this.m_pIdle != null;
			if (flag2)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag3 = user == null;
			if (flag3)
			{
				throw new ArgumentNullException("user");
			}
			bool flag4 = user == string.Empty;
			if (flag4)
			{
				throw new ArgumentException("Argument 'user' value must be specified.");
			}
			using (IMAP_Client.LoginAsyncOP loginAsyncOP = new IMAP_Client.LoginAsyncOP(user, password, null))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					loginAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.LoginAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag5 = !this.LoginAsync(loginAsyncOP);
					if (flag5)
					{
						wait.Set();
					}
					wait.WaitOne();
					wait.Close();
					bool flag6 = loginAsyncOP.Error != null;
					if (flag6)
					{
						throw loginAsyncOP.Error;
					}
				}
			}
		}

		// Token: 0x060013B1 RID: 5041 RVA: 0x000788E8 File Offset: 0x000778E8
		public bool LoginAsync(IMAP_Client.LoginAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool isAuthenticated = base.IsAuthenticated;
			if (isAuthenticated)
			{
				throw new InvalidOperationException("Connection is already authenticated.");
			}
			bool flag2 = this.m_pIdle != null;
			if (flag2)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag3 = op == null;
			if (flag3)
			{
				throw new ArgumentNullException("op");
			}
			bool flag4 = op.State > AsyncOP_State.WaitingForStart;
			if (flag4)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013B2 RID: 5042 RVA: 0x000789A0 File Offset: 0x000779A0
		public void Authenticate(AUTH_SASL_Client sasl)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool isAuthenticated = base.IsAuthenticated;
			if (isAuthenticated)
			{
				throw new InvalidOperationException("Connection is already authenticated.");
			}
			bool flag2 = this.m_pIdle != null;
			if (flag2)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag3 = sasl == null;
			if (flag3)
			{
				throw new ArgumentNullException("sasl");
			}
			using (IMAP_Client.AuthenticateAsyncOP authenticateAsyncOP = new IMAP_Client.AuthenticateAsyncOP(sasl))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					authenticateAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.AuthenticateAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag4 = !this.AuthenticateAsync(authenticateAsyncOP);
					if (flag4)
					{
						wait.Set();
					}
					wait.WaitOne();
					bool flag5 = authenticateAsyncOP.Error != null;
					if (flag5)
					{
						throw authenticateAsyncOP.Error;
					}
				}
			}
		}

		// Token: 0x060013B3 RID: 5043 RVA: 0x00078AE8 File Offset: 0x00077AE8
		public bool AuthenticateAsync(IMAP_Client.AuthenticateAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool isAuthenticated = base.IsAuthenticated;
			if (isAuthenticated)
			{
				throw new InvalidOperationException("Connection is already authenticated.");
			}
			bool flag2 = this.m_pIdle != null;
			if (flag2)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag3 = op == null;
			if (flag3)
			{
				throw new ArgumentNullException("op");
			}
			bool flag4 = op.State > AsyncOP_State.WaitingForStart;
			if (flag4)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013B4 RID: 5044 RVA: 0x00078BA0 File Offset: 0x00077BA0
		public IMAP_r_u_Namespace[] GetNamespaces()
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			List<IMAP_r_u_Namespace> retVal = new List<IMAP_r_u_Namespace>();
			EventHandler<EventArgs<IMAP_r_u>> callback = delegate(object sender, EventArgs<IMAP_r_u> e)
			{
				bool flag6 = e.Value is IMAP_r_u_Namespace;
				if (flag6)
				{
					retVal.Add((IMAP_r_u_Namespace)e.Value);
				}
			};
			using (IMAP_Client.GetNamespacesAsyncOP getNamespacesAsyncOP = new IMAP_Client.GetNamespacesAsyncOP(callback))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					getNamespacesAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.GetNamespacesAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag4 = !this.GetNamespacesAsync(getNamespacesAsyncOP);
					if (flag4)
					{
						wait.Set();
					}
					wait.WaitOne();
					bool flag5 = getNamespacesAsyncOP.Error != null;
					if (flag5)
					{
						throw getNamespacesAsyncOP.Error;
					}
				}
			}
			return retVal.ToArray();
		}

		// Token: 0x060013B5 RID: 5045 RVA: 0x00078D08 File Offset: 0x00077D08
		public bool GetNamespacesAsync(IMAP_Client.GetNamespacesAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = op == null;
			if (flag4)
			{
				throw new ArgumentNullException("op");
			}
			bool flag5 = op.State > AsyncOP_State.WaitingForStart;
			if (flag5)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013B6 RID: 5046 RVA: 0x00078DC0 File Offset: 0x00077DC0
		public IMAP_r_u_List[] GetFolders(string filter)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			List<IMAP_r_u_List> retVal = new List<IMAP_r_u_List>();
			EventHandler<EventArgs<IMAP_r_u>> callback = delegate(object sender, EventArgs<IMAP_r_u> e)
			{
				bool flag6 = e.Value is IMAP_r_u_List;
				if (flag6)
				{
					retVal.Add((IMAP_r_u_List)e.Value);
				}
			};
			using (IMAP_Client.GetFoldersAsyncOP getFoldersAsyncOP = new IMAP_Client.GetFoldersAsyncOP(filter, callback))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					getFoldersAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.GetFoldersAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag4 = !this.GetFoldersAsync(getFoldersAsyncOP);
					if (flag4)
					{
						wait.Set();
					}
					wait.WaitOne();
					bool flag5 = getFoldersAsyncOP.Error != null;
					if (flag5)
					{
						throw getFoldersAsyncOP.Error;
					}
				}
			}
			return retVal.ToArray();
		}

		// Token: 0x060013B7 RID: 5047 RVA: 0x00078F28 File Offset: 0x00077F28
		public bool GetFoldersAsync(IMAP_Client.GetFoldersAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = op == null;
			if (flag4)
			{
				throw new ArgumentNullException("op");
			}
			bool flag5 = op.State > AsyncOP_State.WaitingForStart;
			if (flag5)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013B8 RID: 5048 RVA: 0x00078FE0 File Offset: 0x00077FE0
		public void CreateFolder(string folder)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = folder == null;
			if (flag4)
			{
				throw new ArgumentNullException("folder");
			}
			bool flag5 = folder == string.Empty;
			if (flag5)
			{
				throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
			}
			using (IMAP_Client.CreateFolderAsyncOP createFolderAsyncOP = new IMAP_Client.CreateFolderAsyncOP(folder, null))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					createFolderAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.CreateFolderAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag6 = !this.CreateFolderAsync(createFolderAsyncOP);
					if (flag6)
					{
						wait.Set();
					}
					wait.WaitOne();
					bool flag7 = createFolderAsyncOP.Error != null;
					if (flag7)
					{
						throw createFolderAsyncOP.Error;
					}
				}
			}
		}

		// Token: 0x060013B9 RID: 5049 RVA: 0x00079150 File Offset: 0x00078150
		public bool CreateFolderAsync(IMAP_Client.CreateFolderAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = op == null;
			if (flag4)
			{
				throw new ArgumentNullException("op");
			}
			bool flag5 = op.State > AsyncOP_State.WaitingForStart;
			if (flag5)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013BA RID: 5050 RVA: 0x00079208 File Offset: 0x00078208
		public void DeleteFolder(string folder)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = folder == null;
			if (flag4)
			{
				throw new ArgumentNullException("folder");
			}
			bool flag5 = folder == string.Empty;
			if (flag5)
			{
				throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
			}
			using (IMAP_Client.DeleteFolderAsyncOP deleteFolderAsyncOP = new IMAP_Client.DeleteFolderAsyncOP(folder, null))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					deleteFolderAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.DeleteFolderAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag6 = !this.DeleteFolderAsync(deleteFolderAsyncOP);
					if (flag6)
					{
						wait.Set();
					}
					wait.WaitOne();
					bool flag7 = deleteFolderAsyncOP.Error != null;
					if (flag7)
					{
						throw deleteFolderAsyncOP.Error;
					}
				}
			}
		}

		// Token: 0x060013BB RID: 5051 RVA: 0x00079378 File Offset: 0x00078378
		public bool DeleteFolderAsync(IMAP_Client.DeleteFolderAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = op == null;
			if (flag4)
			{
				throw new ArgumentNullException("op");
			}
			bool flag5 = op.State > AsyncOP_State.WaitingForStart;
			if (flag5)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013BC RID: 5052 RVA: 0x00079430 File Offset: 0x00078430
		public void RenameFolder(string folder, string newFolder)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = folder == null;
			if (flag4)
			{
				throw new ArgumentNullException("folder");
			}
			bool flag5 = folder == string.Empty;
			if (flag5)
			{
				throw new ArgumentException("Argument 'folder' name must be specified.", "folder");
			}
			bool flag6 = newFolder == null;
			if (flag6)
			{
				throw new ArgumentNullException("newFolder");
			}
			bool flag7 = newFolder == string.Empty;
			if (flag7)
			{
				throw new ArgumentException("Argument 'newFolder' name must be specified.", "newFolder");
			}
			using (IMAP_Client.RenameFolderAsyncOP renameFolderAsyncOP = new IMAP_Client.RenameFolderAsyncOP(folder, newFolder, null))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					renameFolderAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.RenameFolderAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag8 = !this.RenameFolderAsync(renameFolderAsyncOP);
					if (flag8)
					{
						wait.Set();
					}
					wait.WaitOne();
					bool flag9 = renameFolderAsyncOP.Error != null;
					if (flag9)
					{
						throw renameFolderAsyncOP.Error;
					}
				}
			}
		}

		// Token: 0x060013BD RID: 5053 RVA: 0x000795D8 File Offset: 0x000785D8
		public bool RenameFolderAsync(IMAP_Client.RenameFolderAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = op == null;
			if (flag4)
			{
				throw new ArgumentNullException("op");
			}
			bool flag5 = op.State > AsyncOP_State.WaitingForStart;
			if (flag5)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013BE RID: 5054 RVA: 0x00079690 File Offset: 0x00078690
		public IMAP_r_u_LSub[] GetSubscribedFolders(string filter)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			List<IMAP_r_u_LSub> retVal = new List<IMAP_r_u_LSub>();
			EventHandler<EventArgs<IMAP_r_u>> callback = delegate(object sender, EventArgs<IMAP_r_u> e)
			{
				bool flag6 = e.Value is IMAP_r_u_LSub;
				if (flag6)
				{
					retVal.Add((IMAP_r_u_LSub)e.Value);
				}
			};
			using (IMAP_Client.GetSubscribedFoldersAsyncOP getSubscribedFoldersAsyncOP = new IMAP_Client.GetSubscribedFoldersAsyncOP(filter, callback))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					getSubscribedFoldersAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.GetSubscribedFoldersAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag4 = !this.GetSubscribedFoldersAsync(getSubscribedFoldersAsyncOP);
					if (flag4)
					{
						wait.Set();
					}
					wait.WaitOne();
					bool flag5 = getSubscribedFoldersAsyncOP.Error != null;
					if (flag5)
					{
						throw getSubscribedFoldersAsyncOP.Error;
					}
				}
			}
			return retVal.ToArray();
		}

		// Token: 0x060013BF RID: 5055 RVA: 0x000797F8 File Offset: 0x000787F8
		public bool GetSubscribedFoldersAsync(IMAP_Client.GetSubscribedFoldersAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = op == null;
			if (flag4)
			{
				throw new ArgumentNullException("op");
			}
			bool flag5 = op.State > AsyncOP_State.WaitingForStart;
			if (flag5)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013C0 RID: 5056 RVA: 0x000798B0 File Offset: 0x000788B0
		public void SubscribeFolder(string folder)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = folder == null;
			if (flag4)
			{
				throw new ArgumentNullException("folder");
			}
			bool flag5 = folder == string.Empty;
			if (flag5)
			{
				throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
			}
			using (IMAP_Client.SubscribeFolderAsyncOP subscribeFolderAsyncOP = new IMAP_Client.SubscribeFolderAsyncOP(folder, null))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					subscribeFolderAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.SubscribeFolderAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag6 = !this.SubscribeFolderAsync(subscribeFolderAsyncOP);
					if (flag6)
					{
						wait.Set();
					}
					wait.WaitOne();
					bool flag7 = subscribeFolderAsyncOP.Error != null;
					if (flag7)
					{
						throw subscribeFolderAsyncOP.Error;
					}
				}
			}
		}

		// Token: 0x060013C1 RID: 5057 RVA: 0x00079A20 File Offset: 0x00078A20
		public bool SubscribeFolderAsync(IMAP_Client.SubscribeFolderAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = op == null;
			if (flag4)
			{
				throw new ArgumentNullException("op");
			}
			bool flag5 = op.State > AsyncOP_State.WaitingForStart;
			if (flag5)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013C2 RID: 5058 RVA: 0x00079AD8 File Offset: 0x00078AD8
		public void UnsubscribeFolder(string folder)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = folder == null;
			if (flag4)
			{
				throw new ArgumentNullException("folder");
			}
			bool flag5 = folder == string.Empty;
			if (flag5)
			{
				throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
			}
			using (IMAP_Client.UnsubscribeFolderAsyncOP unsubscribeFolderAsyncOP = new IMAP_Client.UnsubscribeFolderAsyncOP(folder, null))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					unsubscribeFolderAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.UnsubscribeFolderAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag6 = !this.UnsubscribeFolderAsync(unsubscribeFolderAsyncOP);
					if (flag6)
					{
						wait.Set();
					}
					wait.WaitOne();
					bool flag7 = unsubscribeFolderAsyncOP.Error != null;
					if (flag7)
					{
						throw unsubscribeFolderAsyncOP.Error;
					}
				}
			}
		}

		// Token: 0x060013C3 RID: 5059 RVA: 0x00079C48 File Offset: 0x00078C48
		public bool UnsubscribeFolderAsync(IMAP_Client.UnsubscribeFolderAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = op == null;
			if (flag4)
			{
				throw new ArgumentNullException("op");
			}
			bool flag5 = op.State > AsyncOP_State.WaitingForStart;
			if (flag5)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013C4 RID: 5060 RVA: 0x00079D00 File Offset: 0x00078D00
		public IMAP_r_u_Status[] FolderStatus(string folder)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = folder == null;
			if (flag4)
			{
				throw new ArgumentNullException("folder");
			}
			bool flag5 = folder == string.Empty;
			if (flag5)
			{
				throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
			}
			List<IMAP_r_u_Status> retVal = new List<IMAP_r_u_Status>();
			EventHandler<EventArgs<IMAP_r_u>> callback = delegate(object sender, EventArgs<IMAP_r_u> e)
			{
				bool flag8 = e.Value is IMAP_r_u_Status;
				if (flag8)
				{
					retVal.Add((IMAP_r_u_Status)e.Value);
				}
			};
			using (IMAP_Client.FolderStatusAsyncOP folderStatusAsyncOP = new IMAP_Client.FolderStatusAsyncOP(folder, callback))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					folderStatusAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.FolderStatusAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag6 = !this.FolderStatusAsync(folderStatusAsyncOP);
					if (flag6)
					{
						wait.Set();
					}
					wait.WaitOne();
					bool flag7 = folderStatusAsyncOP.Error != null;
					if (flag7)
					{
						throw folderStatusAsyncOP.Error;
					}
				}
			}
			return retVal.ToArray();
		}

		// Token: 0x060013C5 RID: 5061 RVA: 0x00079EA0 File Offset: 0x00078EA0
		public bool FolderStatusAsync(IMAP_Client.FolderStatusAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = op == null;
			if (flag4)
			{
				throw new ArgumentNullException("op");
			}
			bool flag5 = op.State > AsyncOP_State.WaitingForStart;
			if (flag5)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013C6 RID: 5062 RVA: 0x00079F58 File Offset: 0x00078F58
		public void SelectFolder(string folder)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = folder == null;
			if (flag4)
			{
				throw new ArgumentNullException("folder");
			}
			bool flag5 = folder == string.Empty;
			if (flag5)
			{
				throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
			}
			using (IMAP_Client.SelectFolderAsyncOP selectFolderAsyncOP = new IMAP_Client.SelectFolderAsyncOP(folder, null))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					selectFolderAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.SelectFolderAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag6 = !this.SelectFolderAsync(selectFolderAsyncOP);
					if (flag6)
					{
						wait.Set();
					}
					wait.WaitOne();
					bool flag7 = selectFolderAsyncOP.Error != null;
					if (flag7)
					{
						throw selectFolderAsyncOP.Error;
					}
				}
			}
		}

		// Token: 0x060013C7 RID: 5063 RVA: 0x0007A0C8 File Offset: 0x000790C8
		public bool SelectFolderAsync(IMAP_Client.SelectFolderAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = op == null;
			if (flag4)
			{
				throw new ArgumentNullException("op");
			}
			bool flag5 = op.State > AsyncOP_State.WaitingForStart;
			if (flag5)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013C8 RID: 5064 RVA: 0x0007A180 File Offset: 0x00079180
		public void ExamineFolder(string folder)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = folder == null;
			if (flag4)
			{
				throw new ArgumentNullException("folder");
			}
			bool flag5 = folder == string.Empty;
			if (flag5)
			{
				throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
			}
			using (IMAP_Client.ExamineFolderAsyncOP examineFolderAsyncOP = new IMAP_Client.ExamineFolderAsyncOP(folder, null))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					examineFolderAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.ExamineFolderAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag6 = !this.ExamineFolderAsync(examineFolderAsyncOP);
					if (flag6)
					{
						wait.Set();
					}
					wait.WaitOne();
					bool flag7 = examineFolderAsyncOP.Error != null;
					if (flag7)
					{
						throw examineFolderAsyncOP.Error;
					}
				}
			}
		}

		// Token: 0x060013C9 RID: 5065 RVA: 0x0007A2F0 File Offset: 0x000792F0
		public bool ExamineFolderAsync(IMAP_Client.ExamineFolderAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = op == null;
			if (flag4)
			{
				throw new ArgumentNullException("op");
			}
			bool flag5 = op.State > AsyncOP_State.WaitingForStart;
			if (flag5)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013CA RID: 5066 RVA: 0x0007A3A8 File Offset: 0x000793A8
		public IMAP_r[] GetFolderQuotaRoots(string folder)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = folder == null;
			if (flag4)
			{
				throw new ArgumentNullException("folder");
			}
			bool flag5 = folder == string.Empty;
			if (flag5)
			{
				throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
			}
			List<IMAP_r> retVal = new List<IMAP_r>();
			EventHandler<EventArgs<IMAP_r_u>> callback = delegate(object sender, EventArgs<IMAP_r_u> e)
			{
				bool flag8 = e.Value is IMAP_r_u_Quota;
				if (flag8)
				{
					retVal.Add((IMAP_r_u_Quota)e.Value);
				}
				else
				{
					bool flag9 = e.Value is IMAP_r_u_QuotaRoot;
					if (flag9)
					{
						retVal.Add((IMAP_r_u_QuotaRoot)e.Value);
					}
				}
			};
			using (IMAP_Client.GetFolderQuotaRootsAsyncOP getFolderQuotaRootsAsyncOP = new IMAP_Client.GetFolderQuotaRootsAsyncOP(folder, callback))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					getFolderQuotaRootsAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.GetFolderQuotaRootsAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag6 = !this.GetFolderQuotaRootsAsync(getFolderQuotaRootsAsyncOP);
					if (flag6)
					{
						wait.Set();
					}
					wait.WaitOne();
					bool flag7 = getFolderQuotaRootsAsyncOP.Error != null;
					if (flag7)
					{
						throw getFolderQuotaRootsAsyncOP.Error;
					}
				}
			}
			return retVal.ToArray();
		}

		// Token: 0x060013CB RID: 5067 RVA: 0x0007A548 File Offset: 0x00079548
		public bool GetFolderQuotaRootsAsync(IMAP_Client.GetFolderQuotaRootsAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = op == null;
			if (flag4)
			{
				throw new ArgumentNullException("op");
			}
			bool flag5 = op.State > AsyncOP_State.WaitingForStart;
			if (flag5)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013CC RID: 5068 RVA: 0x0007A600 File Offset: 0x00079600
		public IMAP_r_u_Quota[] GetQuota(string quotaRootName)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = quotaRootName == null;
			if (flag4)
			{
				throw new ArgumentNullException("quotaRootName");
			}
			List<IMAP_r_u_Quota> retVal = new List<IMAP_r_u_Quota>();
			EventHandler<EventArgs<IMAP_r_u>> callback = delegate(object sender, EventArgs<IMAP_r_u> e)
			{
				bool flag7 = e.Value is IMAP_r_u_Quota;
				if (flag7)
				{
					retVal.Add((IMAP_r_u_Quota)e.Value);
				}
			};
			using (IMAP_Client.GetQuotaAsyncOP getQuotaAsyncOP = new IMAP_Client.GetQuotaAsyncOP(quotaRootName, callback))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					getQuotaAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.GetQuotaAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag5 = !this.GetQuotaAsync(getQuotaAsyncOP);
					if (flag5)
					{
						wait.Set();
					}
					wait.WaitOne();
					bool flag6 = getQuotaAsyncOP.Error != null;
					if (flag6)
					{
						throw getQuotaAsyncOP.Error;
					}
				}
			}
			return retVal.ToArray();
		}

		// Token: 0x060013CD RID: 5069 RVA: 0x0007A780 File Offset: 0x00079780
		public bool GetQuotaAsync(IMAP_Client.GetQuotaAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = op == null;
			if (flag4)
			{
				throw new ArgumentNullException("op");
			}
			bool flag5 = op.State > AsyncOP_State.WaitingForStart;
			if (flag5)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013CE RID: 5070 RVA: 0x000091B8 File Offset: 0x000081B8
		private void SetQuota()
		{
		}

		// Token: 0x060013CF RID: 5071 RVA: 0x0007A838 File Offset: 0x00079838
		public IMAP_r_u_Acl[] GetFolderAcl(string folder)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = folder == null;
			if (flag4)
			{
				throw new ArgumentNullException("folder");
			}
			bool flag5 = folder == string.Empty;
			if (flag5)
			{
				throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
			}
			List<IMAP_r_u_Acl> retVal = new List<IMAP_r_u_Acl>();
			EventHandler<EventArgs<IMAP_r_u>> callback = delegate(object sender, EventArgs<IMAP_r_u> e)
			{
				bool flag8 = e.Value is IMAP_r_u_Acl;
				if (flag8)
				{
					retVal.Add((IMAP_r_u_Acl)e.Value);
				}
			};
			using (IMAP_Client.GetFolderAclAsyncOP getFolderAclAsyncOP = new IMAP_Client.GetFolderAclAsyncOP(folder, callback))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					getFolderAclAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.GetFolderAclAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag6 = !this.GetFolderAclAsync(getFolderAclAsyncOP);
					if (flag6)
					{
						wait.Set();
					}
					wait.WaitOne();
					bool flag7 = getFolderAclAsyncOP.Error != null;
					if (flag7)
					{
						throw getFolderAclAsyncOP.Error;
					}
				}
			}
			return retVal.ToArray();
		}

		// Token: 0x060013D0 RID: 5072 RVA: 0x0007A9D8 File Offset: 0x000799D8
		public bool GetFolderAclAsync(IMAP_Client.GetFolderAclAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = op == null;
			if (flag4)
			{
				throw new ArgumentNullException("op");
			}
			bool flag5 = op.State > AsyncOP_State.WaitingForStart;
			if (flag5)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013D1 RID: 5073 RVA: 0x0007AA90 File Offset: 0x00079A90
		public void SetFolderAcl(string folder, string user, IMAP_Flags_SetType setType, IMAP_ACL_Flags permissions)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = folder == null;
			if (flag4)
			{
				throw new ArgumentNullException("folder");
			}
			bool flag5 = folder == string.Empty;
			if (flag5)
			{
				throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
			}
			bool flag6 = user == null;
			if (flag6)
			{
				throw new ArgumentNullException("user");
			}
			bool flag7 = user == string.Empty;
			if (flag7)
			{
				throw new ArgumentException("Argument 'user' value must be specified.", "user");
			}
			using (IMAP_Client.SetFolderAclAsyncOP setFolderAclAsyncOP = new IMAP_Client.SetFolderAclAsyncOP(folder, user, setType, permissions, null))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					setFolderAclAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.SetFolderAclAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag8 = !this.SetFolderAclAsync(setFolderAclAsyncOP);
					if (flag8)
					{
						wait.Set();
					}
					wait.WaitOne();
					bool flag9 = setFolderAclAsyncOP.Error != null;
					if (flag9)
					{
						throw setFolderAclAsyncOP.Error;
					}
				}
			}
		}

		// Token: 0x060013D2 RID: 5074 RVA: 0x0007AC3C File Offset: 0x00079C3C
		public bool SetFolderAclAsync(IMAP_Client.SetFolderAclAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = op == null;
			if (flag4)
			{
				throw new ArgumentNullException("op");
			}
			bool flag5 = op.State > AsyncOP_State.WaitingForStart;
			if (flag5)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013D3 RID: 5075 RVA: 0x0007ACF4 File Offset: 0x00079CF4
		public void DeleteFolderAcl(string folder, string user)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = folder == null;
			if (flag4)
			{
				throw new ArgumentNullException("folder");
			}
			bool flag5 = folder == string.Empty;
			if (flag5)
			{
				throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
			}
			bool flag6 = user == null;
			if (flag6)
			{
				throw new ArgumentNullException("user");
			}
			bool flag7 = user == string.Empty;
			if (flag7)
			{
				throw new ArgumentException("Argument 'user' value must be specified.", "user");
			}
			using (IMAP_Client.DeleteFolderAclAsyncOP deleteFolderAclAsyncOP = new IMAP_Client.DeleteFolderAclAsyncOP(folder, user, null))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					deleteFolderAclAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.DeleteFolderAclAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag8 = !this.DeleteFolderAclAsync(deleteFolderAclAsyncOP);
					if (flag8)
					{
						wait.Set();
					}
					wait.WaitOne();
					bool flag9 = deleteFolderAclAsyncOP.Error != null;
					if (flag9)
					{
						throw deleteFolderAclAsyncOP.Error;
					}
				}
			}
		}

		// Token: 0x060013D4 RID: 5076 RVA: 0x0007AE9C File Offset: 0x00079E9C
		public bool DeleteFolderAclAsync(IMAP_Client.DeleteFolderAclAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = op == null;
			if (flag4)
			{
				throw new ArgumentNullException("op");
			}
			bool flag5 = op.State > AsyncOP_State.WaitingForStart;
			if (flag5)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013D5 RID: 5077 RVA: 0x0007AF54 File Offset: 0x00079F54
		public IMAP_r_u_ListRights[] GetFolderRights(string folder, string identifier)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = folder == null;
			if (flag4)
			{
				throw new ArgumentNullException("folder");
			}
			bool flag5 = folder == string.Empty;
			if (flag5)
			{
				throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
			}
			bool flag6 = identifier == null;
			if (flag6)
			{
				throw new ArgumentNullException("identifier");
			}
			bool flag7 = identifier == string.Empty;
			if (flag7)
			{
				throw new ArgumentException("Argument 'identifier' value must be specified.", "identifier");
			}
			List<IMAP_r_u_ListRights> retVal = new List<IMAP_r_u_ListRights>();
			EventHandler<EventArgs<IMAP_r_u>> callback = delegate(object sender, EventArgs<IMAP_r_u> e)
			{
				bool flag10 = e.Value is IMAP_r_u_ListRights;
				if (flag10)
				{
					retVal.Add((IMAP_r_u_ListRights)e.Value);
				}
			};
			using (IMAP_Client.GetFolderRightsAsyncOP getFolderRightsAsyncOP = new IMAP_Client.GetFolderRightsAsyncOP(folder, identifier, callback))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					getFolderRightsAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.GetFolderRightsAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag8 = !this.GetFolderRightsAsync(getFolderRightsAsyncOP);
					if (flag8)
					{
						wait.Set();
					}
					wait.WaitOne();
					bool flag9 = getFolderRightsAsyncOP.Error != null;
					if (flag9)
					{
						throw getFolderRightsAsyncOP.Error;
					}
				}
			}
			return retVal.ToArray();
		}

		// Token: 0x060013D6 RID: 5078 RVA: 0x0007B130 File Offset: 0x0007A130
		public bool GetFolderRightsAsync(IMAP_Client.GetFolderRightsAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = op == null;
			if (flag4)
			{
				throw new ArgumentNullException("op");
			}
			bool flag5 = op.State > AsyncOP_State.WaitingForStart;
			if (flag5)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013D7 RID: 5079 RVA: 0x0007B1E8 File Offset: 0x0007A1E8
		public IMAP_r_u_MyRights[] GetFolderMyRights(string folder)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = folder == null;
			if (flag4)
			{
				throw new ArgumentNullException("folder");
			}
			bool flag5 = folder == string.Empty;
			if (flag5)
			{
				throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
			}
			List<IMAP_r_u_MyRights> retVal = new List<IMAP_r_u_MyRights>();
			EventHandler<EventArgs<IMAP_r_u>> callback = delegate(object sender, EventArgs<IMAP_r_u> e)
			{
				bool flag8 = e.Value is IMAP_r_u_MyRights;
				if (flag8)
				{
					retVal.Add((IMAP_r_u_MyRights)e.Value);
				}
			};
			using (IMAP_Client.GetFolderMyRightsAsyncOP getFolderMyRightsAsyncOP = new IMAP_Client.GetFolderMyRightsAsyncOP(folder, callback))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					getFolderMyRightsAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.GetFolderMyRightsAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag6 = !this.GetFolderMyRightsAsync(getFolderMyRightsAsyncOP);
					if (flag6)
					{
						wait.Set();
					}
					wait.WaitOne();
					bool flag7 = getFolderMyRightsAsyncOP.Error != null;
					if (flag7)
					{
						throw getFolderMyRightsAsyncOP.Error;
					}
				}
			}
			return retVal.ToArray();
		}

		// Token: 0x060013D8 RID: 5080 RVA: 0x0007B388 File Offset: 0x0007A388
		public bool GetFolderMyRightsAsync(IMAP_Client.GetFolderMyRightsAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = op == null;
			if (flag4)
			{
				throw new ArgumentNullException("op");
			}
			bool flag5 = op.State > AsyncOP_State.WaitingForStart;
			if (flag5)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013D9 RID: 5081 RVA: 0x0007B440 File Offset: 0x0007A440
		public void StoreMessage(string folder, string[] flags, DateTime internalDate, Stream message, int count)
		{
			this.StoreMessage(folder, (flags != null) ? new IMAP_t_MsgFlags(flags) : new IMAP_t_MsgFlags(new string[0]), internalDate, message, count);
		}

		// Token: 0x060013DA RID: 5082 RVA: 0x0007B468 File Offset: 0x0007A468
		public void StoreMessage(string folder, IMAP_t_MsgFlags flags, DateTime internalDate, Stream message, int count)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = folder == null;
			if (flag4)
			{
				throw new ArgumentNullException("folder");
			}
			bool flag5 = folder == string.Empty;
			if (flag5)
			{
				throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
			}
			bool flag6 = flags == null;
			if (flag6)
			{
				throw new ArgumentNullException("flags");
			}
			bool flag7 = message == null;
			if (flag7)
			{
				throw new ArgumentNullException("message");
			}
			bool flag8 = count < 1;
			if (flag8)
			{
				throw new ArgumentException("Argument 'count' value must be >= 1.", "count");
			}
			using (IMAP_Client.StoreMessageAsyncOP storeMessageAsyncOP = new IMAP_Client.StoreMessageAsyncOP(folder, flags, internalDate, message, (long)count, null))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					storeMessageAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.StoreMessageAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag9 = !this.StoreMessageAsync(storeMessageAsyncOP);
					if (flag9)
					{
						wait.Set();
					}
					wait.WaitOne();
					bool flag10 = storeMessageAsyncOP.Error != null;
					if (flag10)
					{
						throw storeMessageAsyncOP.Error;
					}
				}
			}
		}

		// Token: 0x060013DB RID: 5083 RVA: 0x0007B628 File Offset: 0x0007A628
		public void StoreMessage(IMAP_Client.StoreMessageAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = op == null;
			if (flag4)
			{
				throw new ArgumentNullException("op");
			}
			using (ManualResetEvent wait = new ManualResetEvent(false))
			{
				op.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.StoreMessageAsyncOP> e1)
				{
					wait.Set();
				};
				bool flag5 = !this.StoreMessageAsync(op);
				if (flag5)
				{
					wait.Set();
				}
				wait.WaitOne();
				bool flag6 = op.Error != null;
				if (flag6)
				{
					throw op.Error;
				}
			}
		}

		// Token: 0x060013DC RID: 5084 RVA: 0x0007B74C File Offset: 0x0007A74C
		public bool StoreMessageAsync(IMAP_Client.StoreMessageAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = op == null;
			if (flag4)
			{
				throw new ArgumentNullException("op");
			}
			bool flag5 = op.State > AsyncOP_State.WaitingForStart;
			if (flag5)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013DD RID: 5085 RVA: 0x0007B804 File Offset: 0x0007A804
		public IMAP_r_u_Enable[] Enable(string[] capabilities)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.SelectedFolder != null;
			if (flag3)
			{
				throw new InvalidOperationException("The 'ENABLE' command MUST only be used in the authenticated state.");
			}
			bool flag4 = this.m_pIdle != null;
			if (flag4)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag5 = capabilities == null;
			if (flag5)
			{
				throw new ArgumentNullException("capabilities");
			}
			bool flag6 = capabilities.Length < 1;
			if (flag6)
			{
				throw new ArgumentException("Argument 'capabilities' must contain at least 1 value.", "capabilities");
			}
			List<IMAP_r_u_Enable> retVal = new List<IMAP_r_u_Enable>();
			EventHandler<EventArgs<IMAP_r_u>> callback = delegate(object sender, EventArgs<IMAP_r_u> e)
			{
				bool flag9 = e.Value is IMAP_r_u_Enable;
				if (flag9)
				{
					retVal.Add((IMAP_r_u_Enable)e.Value);
				}
			};
			using (IMAP_Client.EnableAsyncOP enableAsyncOP = new IMAP_Client.EnableAsyncOP(capabilities, callback))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					enableAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.EnableAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag7 = !this.EnableAsync(enableAsyncOP);
					if (flag7)
					{
						wait.Set();
					}
					wait.WaitOne();
					bool flag8 = enableAsyncOP.Error != null;
					if (flag8)
					{
						throw enableAsyncOP.Error;
					}
				}
			}
			return retVal.ToArray();
		}

		// Token: 0x060013DE RID: 5086 RVA: 0x0007B9BC File Offset: 0x0007A9BC
		public bool EnableAsync(IMAP_Client.EnableAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.SelectedFolder != null;
			if (flag3)
			{
				throw new InvalidOperationException("The 'ENABLE' command MUST only be used in the authenticated state.");
			}
			bool flag4 = this.m_pIdle != null;
			if (flag4)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag5 = op == null;
			if (flag5)
			{
				throw new ArgumentNullException("op");
			}
			bool flag6 = op.State > AsyncOP_State.WaitingForStart;
			if (flag6)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013DF RID: 5087 RVA: 0x0007BA90 File Offset: 0x0007AA90
		public void EnableUtf8()
		{
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.SelectedFolder != null;
			if (flag3)
			{
				throw new InvalidOperationException("The 'ENABLE UTF8=ACCEPT' command MUST only be used in the authenticated state.");
			}
			bool flag4 = this.m_pIdle != null;
			if (flag4)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			IMAP_r_u_Enable[] array = this.Enable(new string[]
			{
				"UTF8=ACCEPT"
			});
			this.m_MailboxEncoding = IMAP_Mailbox_Encoding.ImapUtf8;
		}

		// Token: 0x060013E0 RID: 5088 RVA: 0x0007BB20 File Offset: 0x0007AB20
		public void CloseFolder()
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pSelectedFolder == null;
			if (flag3)
			{
				throw new InvalidOperationException("Not selected state, you need to select some folder first.");
			}
			bool flag4 = this.m_pIdle != null;
			if (flag4)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			using (IMAP_Client.CloseFolderAsyncOP closeFolderAsyncOP = new IMAP_Client.CloseFolderAsyncOP(null))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					closeFolderAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.CloseFolderAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag5 = !this.CloseFolderAsync(closeFolderAsyncOP);
					if (flag5)
					{
						wait.Set();
					}
					wait.WaitOne();
					bool flag6 = closeFolderAsyncOP.Error != null;
					if (flag6)
					{
						throw closeFolderAsyncOP.Error;
					}
				}
			}
		}

		// Token: 0x060013E1 RID: 5089 RVA: 0x0007BC70 File Offset: 0x0007AC70
		public bool CloseFolderAsync(IMAP_Client.CloseFolderAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pSelectedFolder == null;
			if (flag3)
			{
				throw new InvalidOperationException("Not selected state, you need to select some folder first.");
			}
			bool flag4 = this.m_pIdle != null;
			if (flag4)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag5 = op == null;
			if (flag5)
			{
				throw new ArgumentNullException("op");
			}
			bool flag6 = op.State > AsyncOP_State.WaitingForStart;
			if (flag6)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013E2 RID: 5090 RVA: 0x0007BD44 File Offset: 0x0007AD44
		public void Fetch(bool uid, IMAP_t_SeqSet seqSet, IMAP_t_Fetch_i[] items, EventHandler<EventArgs<IMAP_r_u>> callback)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pSelectedFolder == null;
			if (flag3)
			{
				throw new InvalidOperationException("Not selected state, you need to select some folder first.");
			}
			bool flag4 = this.m_pIdle != null;
			if (flag4)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag5 = seqSet == null;
			if (flag5)
			{
				throw new ArgumentNullException("seqSet");
			}
			bool flag6 = items == null;
			if (flag6)
			{
				throw new ArgumentNullException("items");
			}
			bool flag7 = items.Length < 1;
			if (flag7)
			{
				throw new ArgumentException("Argument 'items' must conatain at least 1 value.", "items");
			}
			using (IMAP_Client.FetchAsyncOP fetchAsyncOP = new IMAP_Client.FetchAsyncOP(uid, seqSet, items, callback))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					fetchAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.FetchAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag8 = !this.FetchAsync(fetchAsyncOP);
					if (flag8)
					{
						wait.Set();
					}
					wait.WaitOne();
					bool flag9 = fetchAsyncOP.Error != null;
					if (flag9)
					{
						throw fetchAsyncOP.Error;
					}
				}
			}
		}

		// Token: 0x060013E3 RID: 5091 RVA: 0x0007BEE0 File Offset: 0x0007AEE0
		public bool FetchAsync(IMAP_Client.FetchAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pSelectedFolder == null;
			if (flag3)
			{
				throw new InvalidOperationException("Not selected state, you need to select some folder first.");
			}
			bool flag4 = this.m_pIdle != null;
			if (flag4)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag5 = op == null;
			if (flag5)
			{
				throw new ArgumentNullException("op");
			}
			bool flag6 = op.State > AsyncOP_State.WaitingForStart;
			if (flag6)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013E4 RID: 5092 RVA: 0x0007BFB4 File Offset: 0x0007AFB4
		public int[] Search(bool uid, Encoding charset, IMAP_Search_Key criteria)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pSelectedFolder == null;
			if (flag3)
			{
				throw new InvalidOperationException("Not selected state, you need to select some folder first.");
			}
			bool flag4 = this.m_pIdle != null;
			if (flag4)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag5 = criteria == null;
			if (flag5)
			{
				throw new ArgumentNullException("criteria");
			}
			List<int> retVal = new List<int>();
			EventHandler<EventArgs<IMAP_r_u>> callback = delegate(object sender, EventArgs<IMAP_r_u> e)
			{
				bool flag8 = e.Value is IMAP_r_u_Search;
				if (flag8)
				{
					retVal.AddRange(((IMAP_r_u_Search)e.Value).Values);
				}
			};
			using (IMAP_Client.SearchAsyncOP searchAsyncOP = new IMAP_Client.SearchAsyncOP(uid, charset, criteria, callback))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					searchAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.SearchAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag6 = !this.SearchAsync(searchAsyncOP);
					if (flag6)
					{
						wait.Set();
					}
					wait.WaitOne();
					bool flag7 = searchAsyncOP.Error != null;
					if (flag7)
					{
						throw searchAsyncOP.Error;
					}
				}
			}
			return retVal.ToArray();
		}

		// Token: 0x060013E5 RID: 5093 RVA: 0x0007C150 File Offset: 0x0007B150
		public bool SearchAsync(IMAP_Client.SearchAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pSelectedFolder == null;
			if (flag3)
			{
				throw new InvalidOperationException("Not selected state, you need to select some folder first.");
			}
			bool flag4 = this.m_pIdle != null;
			if (flag4)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag5 = op == null;
			if (flag5)
			{
				throw new ArgumentNullException("op");
			}
			bool flag6 = op.State > AsyncOP_State.WaitingForStart;
			if (flag6)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013E6 RID: 5094 RVA: 0x0007C224 File Offset: 0x0007B224
		public void StoreMessageFlags(bool uid, IMAP_t_SeqSet seqSet, IMAP_Flags_SetType setType, IMAP_t_MsgFlags flags)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pSelectedFolder == null;
			if (flag3)
			{
				throw new InvalidOperationException("Not selected state, you need to select some folder first.");
			}
			bool flag4 = this.m_pIdle != null;
			if (flag4)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag5 = seqSet == null;
			if (flag5)
			{
				throw new ArgumentNullException("seqSet");
			}
			bool flag6 = flags == null;
			if (flag6)
			{
				throw new ArgumentNullException("flags");
			}
			using (IMAP_Client.StoreMessageFlagsAsyncOP storeMessageFlagsAsyncOP = new IMAP_Client.StoreMessageFlagsAsyncOP(uid, seqSet, true, setType, flags, null))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					storeMessageFlagsAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.StoreMessageFlagsAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag7 = !this.StoreMessageFlagsAsync(storeMessageFlagsAsyncOP);
					if (flag7)
					{
						wait.Set();
					}
					wait.WaitOne();
					bool flag8 = storeMessageFlagsAsyncOP.Error != null;
					if (flag8)
					{
						throw storeMessageFlagsAsyncOP.Error;
					}
				}
			}
		}

		// Token: 0x060013E7 RID: 5095 RVA: 0x0007C3A8 File Offset: 0x0007B3A8
		public bool StoreMessageFlagsAsync(IMAP_Client.StoreMessageFlagsAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pSelectedFolder == null;
			if (flag3)
			{
				throw new InvalidOperationException("Not selected state, you need to select some folder first.");
			}
			bool flag4 = this.m_pIdle != null;
			if (flag4)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag5 = op == null;
			if (flag5)
			{
				throw new ArgumentNullException("op");
			}
			bool flag6 = op.State > AsyncOP_State.WaitingForStart;
			if (flag6)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013E8 RID: 5096 RVA: 0x0007C47C File Offset: 0x0007B47C
		public void CopyMessages(bool uid, IMAP_t_SeqSet seqSet, string targetFolder)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pSelectedFolder == null;
			if (flag3)
			{
				throw new InvalidOperationException("Not selected state, you need to select some folder first.");
			}
			bool flag4 = this.m_pIdle != null;
			if (flag4)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag5 = seqSet == null;
			if (flag5)
			{
				throw new ArgumentNullException("seqSet");
			}
			bool flag6 = targetFolder == null;
			if (flag6)
			{
				throw new ArgumentNullException("folder");
			}
			bool flag7 = targetFolder == string.Empty;
			if (flag7)
			{
				throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
			}
			using (IMAP_Client.CopyMessagesAsyncOP copyMessagesAsyncOP = new IMAP_Client.CopyMessagesAsyncOP(uid, seqSet, targetFolder, null))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					copyMessagesAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.CopyMessagesAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag8 = !this.CopyMessagesAsync(copyMessagesAsyncOP);
					if (flag8)
					{
						wait.Set();
					}
					wait.WaitOne();
					bool flag9 = copyMessagesAsyncOP.Error != null;
					if (flag9)
					{
						throw copyMessagesAsyncOP.Error;
					}
				}
			}
		}

		// Token: 0x060013E9 RID: 5097 RVA: 0x0007C61C File Offset: 0x0007B61C
		public void CopyMessages(IMAP_Client.CopyMessagesAsyncOP op)
		{
			bool flag = op == null;
			if (flag)
			{
				throw new ArgumentNullException("op");
			}
			using (ManualResetEvent wait = new ManualResetEvent(false))
			{
				op.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.CopyMessagesAsyncOP> e1)
				{
					wait.Set();
				};
				bool flag2 = !this.CopyMessagesAsync(op);
				if (flag2)
				{
					wait.Set();
				}
				wait.WaitOne();
				bool flag3 = op.Error != null;
				if (flag3)
				{
					throw op.Error;
				}
			}
		}

		// Token: 0x060013EA RID: 5098 RVA: 0x0007C6CC File Offset: 0x0007B6CC
		public bool CopyMessagesAsync(IMAP_Client.CopyMessagesAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pSelectedFolder == null;
			if (flag3)
			{
				throw new InvalidOperationException("Not selected state, you need to select some folder first.");
			}
			bool flag4 = this.m_pIdle != null;
			if (flag4)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag5 = op == null;
			if (flag5)
			{
				throw new ArgumentNullException("op");
			}
			bool flag6 = op.State > AsyncOP_State.WaitingForStart;
			if (flag6)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013EB RID: 5099 RVA: 0x0007C7A0 File Offset: 0x0007B7A0
		public void MoveMessages(bool uid, IMAP_t_SeqSet seqSet, string targetFolder, bool expunge)
		{
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pSelectedFolder == null;
			if (flag3)
			{
				throw new InvalidOperationException("Not selected state, you need to select some folder first.");
			}
			bool flag4 = this.m_pIdle != null;
			if (flag4)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag5 = seqSet == null;
			if (flag5)
			{
				throw new ArgumentNullException("seqSet");
			}
			bool flag6 = targetFolder == null;
			if (flag6)
			{
				throw new ArgumentNullException("folder");
			}
			bool flag7 = targetFolder == string.Empty;
			if (flag7)
			{
				throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
			}
			this.CopyMessages(uid, seqSet, targetFolder);
			this.StoreMessageFlags(uid, seqSet, IMAP_Flags_SetType.Add, IMAP_t_MsgFlags.Parse(IMAP_t_MsgFlags.Deleted));
			if (expunge)
			{
				this.Expunge();
			}
		}

		// Token: 0x060013EC RID: 5100 RVA: 0x0007C890 File Offset: 0x0007B890
		public void Expunge()
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pSelectedFolder == null;
			if (flag3)
			{
				throw new InvalidOperationException("Not selected state, you need to select some folder first.");
			}
			bool flag4 = this.m_pIdle != null;
			if (flag4)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			using (IMAP_Client.ExpungeAsyncOP expungeAsyncOP = new IMAP_Client.ExpungeAsyncOP(null))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					expungeAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.ExpungeAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag5 = !this.ExpungeAsync(expungeAsyncOP);
					if (flag5)
					{
						wait.Set();
					}
					wait.WaitOne();
					bool flag6 = expungeAsyncOP.Error != null;
					if (flag6)
					{
						throw expungeAsyncOP.Error;
					}
				}
			}
		}

		// Token: 0x060013ED RID: 5101 RVA: 0x0007C9E0 File Offset: 0x0007B9E0
		public bool ExpungeAsync(IMAP_Client.ExpungeAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pSelectedFolder == null;
			if (flag3)
			{
				throw new InvalidOperationException("Not selected state, you need to select some folder first.");
			}
			bool flag4 = this.m_pIdle != null;
			if (flag4)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag5 = op == null;
			if (flag5)
			{
				throw new ArgumentNullException("op");
			}
			bool flag6 = op.State > AsyncOP_State.WaitingForStart;
			if (flag6)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013EE RID: 5102 RVA: 0x0007CAB4 File Offset: 0x0007BAB4
		public bool IdleAsync(IMAP_Client.IdleAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pSelectedFolder == null;
			if (flag3)
			{
				throw new InvalidOperationException("Not selected state, you need to select some folder first.");
			}
			bool flag4 = this.m_pIdle != null;
			if (flag4)
			{
				throw new InvalidOperationException("Already idling !");
			}
			bool flag5 = op == null;
			if (flag5)
			{
				throw new ArgumentNullException("op");
			}
			bool flag6 = op.State > AsyncOP_State.WaitingForStart;
			if (flag6)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013EF RID: 5103 RVA: 0x0007CB88 File Offset: 0x0007BB88
		public IMAP_r_u_Capability[] Capability()
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = this.m_pIdle != null;
			if (flag2)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			List<IMAP_r_u_Capability> retVal = new List<IMAP_r_u_Capability>();
			EventHandler<EventArgs<IMAP_r_u>> callback = delegate(object sender, EventArgs<IMAP_r_u> e)
			{
				bool flag5 = e.Value is IMAP_r_u_Capability;
				if (flag5)
				{
					retVal.Add((IMAP_r_u_Capability)e.Value);
				}
			};
			using (IMAP_Client.CapabilityAsyncOP capabilityAsyncOP = new IMAP_Client.CapabilityAsyncOP(callback))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					capabilityAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.CapabilityAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag3 = !this.CapabilityAsync(capabilityAsyncOP);
					if (flag3)
					{
						wait.Set();
					}
					wait.WaitOne();
					bool flag4 = capabilityAsyncOP.Error != null;
					if (flag4)
					{
						throw capabilityAsyncOP.Error;
					}
				}
			}
			return retVal.ToArray();
		}

		// Token: 0x060013F0 RID: 5104 RVA: 0x0007CCD4 File Offset: 0x0007BCD4
		public bool CapabilityAsync(IMAP_Client.CapabilityAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = this.m_pIdle != null;
			if (flag2)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag3 = op == null;
			if (flag3)
			{
				throw new ArgumentNullException("op");
			}
			bool flag4 = op.State > AsyncOP_State.WaitingForStart;
			if (flag4)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013F1 RID: 5105 RVA: 0x0007CD74 File Offset: 0x0007BD74
		public void Noop()
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = this.m_pIdle != null;
			if (flag2)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			using (IMAP_Client.NoopAsyncOP noopAsyncOP = new IMAP_Client.NoopAsyncOP(null))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					noopAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.NoopAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag3 = !this.NoopAsync(noopAsyncOP);
					if (flag3)
					{
						wait.Set();
					}
					wait.WaitOne();
					bool flag4 = noopAsyncOP.Error != null;
					if (flag4)
					{
						throw noopAsyncOP.Error;
					}
				}
			}
		}

		// Token: 0x060013F2 RID: 5106 RVA: 0x0007CE88 File Offset: 0x0007BE88
		public bool NoopAsync(IMAP_Client.NoopAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = this.m_pIdle != null;
			if (flag2)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag3 = op == null;
			if (flag3)
			{
				throw new ArgumentNullException("op");
			}
			bool flag4 = op.State > AsyncOP_State.WaitingForStart;
			if (flag4)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013F3 RID: 5107 RVA: 0x0007CF28 File Offset: 0x0007BF28
		protected override void OnConnected(TCP_Client.CompleteConnectCallback callback)
		{
			IMAP_Client.ReadResponseAsyncOP op = new IMAP_Client.ReadResponseAsyncOP();
			op.CompletedAsync += delegate(object sender, EventArgs<IMAP_Client.ReadResponseAsyncOP> e)
			{
				this.ProcessGreetingResult(op, callback);
			};
			bool flag = !this.ReadResponseAsync(op);
			if (flag)
			{
				this.ProcessGreetingResult(op, callback);
			}
		}

		// Token: 0x060013F4 RID: 5108 RVA: 0x0007CF98 File Offset: 0x0007BF98
		private void ProcessGreetingResult(IMAP_Client.ReadResponseAsyncOP op, TCP_Client.CompleteConnectCallback connectCallback)
		{
			Exception error = null;
			try
			{
				bool flag = op.Error != null;
				if (flag)
				{
					error = op.Error;
				}
				else
				{
					bool flag2 = op.Response is IMAP_r_u_ServerStatus;
					if (flag2)
					{
						IMAP_r_u_ServerStatus imap_r_u_ServerStatus = (IMAP_r_u_ServerStatus)op.Response;
						bool isError = imap_r_u_ServerStatus.IsError;
						if (isError)
						{
							error = new IMAP_ClientException(imap_r_u_ServerStatus.ResponseCode, imap_r_u_ServerStatus.ResponseText);
						}
						else
						{
							this.m_GreetingText = imap_r_u_ServerStatus.ResponseText;
						}
					}
					else
					{
						error = new Exception("Unexpected IMAP server greeting response: " + op.Response.ToString());
					}
				}
			}
			catch (Exception ex)
			{
				error = ex;
			}
			connectCallback(error);
		}

		// Token: 0x060013F5 RID: 5109 RVA: 0x0007D058 File Offset: 0x0007C058
		private bool SendCmdAndReadRespAsync(IMAP_Client.SendCmdAndReadRespAsyncOP op)
		{
			bool flag = op == null;
			if (flag)
			{
				throw new ArgumentNullException("op");
			}
			bool flag2 = op.State > AsyncOP_State.WaitingForStart;
			if (flag2)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013F6 RID: 5110 RVA: 0x0007D0A4 File Offset: 0x0007C0A4
		private bool ReadResponseAsync(IMAP_Client.ReadResponseAsyncOP op)
		{
			bool flag = op == null;
			if (flag)
			{
				throw new ArgumentNullException("op");
			}
			bool flag2 = op.State > AsyncOP_State.WaitingForStart;
			if (flag2)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013F7 RID: 5111 RVA: 0x0007D0F0 File Offset: 0x0007C0F0
		private bool ReadFinalResponseAsync(IMAP_Client.ReadFinalResponseAsyncOP op)
		{
			bool flag = op == null;
			if (flag)
			{
				throw new ArgumentNullException("op");
			}
			bool flag2 = op.State > AsyncOP_State.WaitingForStart;
			if (flag2)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013F8 RID: 5112 RVA: 0x0007D13C File Offset: 0x0007C13C
		internal bool ReadStringLiteralAsync(IMAP_Client.ReadStringLiteralAsyncOP op)
		{
			bool flag = op == null;
			if (flag)
			{
				throw new ArgumentNullException("op");
			}
			bool flag2 = op.State > AsyncOP_State.WaitingForStart;
			if (flag2)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060013F9 RID: 5113 RVA: 0x0007D188 File Offset: 0x0007C188
		private bool SupportsCapability(string capability)
		{
			bool flag = capability == null;
			if (flag)
			{
				throw new ArgumentNullException("capability");
			}
			bool flag2 = this.m_pCapabilities == null;
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				foreach (string a in this.m_pCapabilities)
				{
					bool flag3 = string.Equals(a, capability, StringComparison.InvariantCultureIgnoreCase);
					if (flag3)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x1700068A RID: 1674
		// (get) Token: 0x060013FA RID: 5114 RVA: 0x0007D21C File Offset: 0x0007C21C
		public override GenericIdentity AuthenticatedUserIdentity
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = !this.IsConnected;
				if (flag)
				{
					throw new InvalidOperationException("You must connect first.");
				}
				return this.m_pAuthenticatedUser;
			}
		}

		// Token: 0x1700068B RID: 1675
		// (get) Token: 0x060013FB RID: 5115 RVA: 0x0007D26C File Offset: 0x0007C26C
		public string GreetingText
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = !this.IsConnected;
				if (flag)
				{
					throw new InvalidOperationException("You must connect first.");
				}
				return this.m_GreetingText;
			}
		}

		// Token: 0x1700068C RID: 1676
		// (get) Token: 0x060013FC RID: 5116 RVA: 0x0007D2BC File Offset: 0x0007C2BC
		public string[] Capabilities
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = !this.IsConnected;
				if (flag)
				{
					throw new InvalidOperationException("You must connect first.");
				}
				bool flag2 = this.m_pCapabilities == null;
				string[] result;
				if (flag2)
				{
					result = new string[0];
				}
				else
				{
					result = this.m_pCapabilities.ToArray();
				}
				return result;
			}
		}

		// Token: 0x1700068D RID: 1677
		// (get) Token: 0x060013FD RID: 5117 RVA: 0x0007D328 File Offset: 0x0007C328
		public char FolderSeparator
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = !this.IsConnected;
				if (flag)
				{
					throw new InvalidOperationException("You must connect first.");
				}
				IMAP_r_u_List[] folders = this.GetFolders("");
				bool flag2 = folders.Length == 0;
				if (flag2)
				{
					throw new Exception("Unexpected result: IMAP server didn't return LIST response for [... LIST \"\" \"\"].");
				}
				return folders[0].HierarchyDelimiter;
			}
		}

		// Token: 0x1700068E RID: 1678
		// (get) Token: 0x060013FE RID: 5118 RVA: 0x0007D39C File Offset: 0x0007C39C
		public IMAP_Client_SelectedFolder SelectedFolder
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = !this.IsConnected;
				if (flag)
				{
					throw new InvalidOperationException("You must connect first.");
				}
				return this.m_pSelectedFolder;
			}
		}

		// Token: 0x1700068F RID: 1679
		// (get) Token: 0x060013FF RID: 5119 RVA: 0x0007D3EC File Offset: 0x0007C3EC
		public IMAP_Client.IdleAsyncOP IdleOP
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = !this.IsConnected;
				if (flag)
				{
					throw new InvalidOperationException("You must connect first.");
				}
				return this.m_pIdle;
			}
		}

		// Token: 0x17000690 RID: 1680
		// (get) Token: 0x06001400 RID: 5120 RVA: 0x0007D43C File Offset: 0x0007C43C
		public IMAP_Client.SettingsHolder Settings
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pSettings;
			}
		}

		// Token: 0x14000079 RID: 121
		// (add) Token: 0x06001401 RID: 5121 RVA: 0x0007D470 File Offset: 0x0007C470
		// (remove) Token: 0x06001402 RID: 5122 RVA: 0x0007D4A8 File Offset: 0x0007C4A8
		
		public event EventHandler<EventArgs<IMAP_r_u>> UntaggedStatusResponse = null;

		// Token: 0x06001403 RID: 5123 RVA: 0x0007D4E0 File Offset: 0x0007C4E0
		private void OnUntaggedStatusResponse(IMAP_r_u response)
		{
			bool flag = this.UntaggedStatusResponse != null;
			if (flag)
			{
				this.UntaggedStatusResponse(this, new EventArgs<IMAP_r_u>(response));
			}
		}

		// Token: 0x1400007A RID: 122
		// (add) Token: 0x06001404 RID: 5124 RVA: 0x0007D510 File Offset: 0x0007C510
		// (remove) Token: 0x06001405 RID: 5125 RVA: 0x0007D548 File Offset: 0x0007C548
		
		public event EventHandler<EventArgs<IMAP_r_u>> UntaggedResponse = null;

		// Token: 0x06001406 RID: 5126 RVA: 0x0007D580 File Offset: 0x0007C580
		private void OnUntaggedResponse(IMAP_r_u response)
		{
			bool flag = this.UntaggedResponse != null;
			if (flag)
			{
				this.UntaggedResponse(this, new EventArgs<IMAP_r_u>(response));
			}
		}

		// Token: 0x1400007B RID: 123
		// (add) Token: 0x06001407 RID: 5127 RVA: 0x0007D5B0 File Offset: 0x0007C5B0
		// (remove) Token: 0x06001408 RID: 5128 RVA: 0x0007D5E8 File Offset: 0x0007C5E8
		
		public event EventHandler<EventArgs<IMAP_r_u_Expunge>> MessageExpunged = null;

		// Token: 0x06001409 RID: 5129 RVA: 0x0007D620 File Offset: 0x0007C620
		private void OnMessageExpunged(IMAP_r_u_Expunge response)
		{
			bool flag = this.MessageExpunged != null;
			if (flag)
			{
				this.MessageExpunged(this, new EventArgs<IMAP_r_u_Expunge>(response));
			}
		}

		// Token: 0x1400007C RID: 124
		// (add) Token: 0x0600140A RID: 5130 RVA: 0x0007D650 File Offset: 0x0007C650
		// (remove) Token: 0x0600140B RID: 5131 RVA: 0x0007D688 File Offset: 0x0007C688
		
		public event EventHandler<IMAP_Client_e_FetchGetStoreStream> FetchGetStoreStream = null;

		// Token: 0x0600140C RID: 5132 RVA: 0x0007D6C0 File Offset: 0x0007C6C0
		internal void OnFetchGetStoreStream(IMAP_Client_e_FetchGetStoreStream e)
		{
			bool flag = this.FetchGetStoreStream != null;
			if (flag)
			{
				this.FetchGetStoreStream(this, e);
			}
		}

		// Token: 0x0600140D RID: 5133 RVA: 0x0007D6EC File Offset: 0x0007C6EC
		[Obsolete("deprecated")]
		private IMAP_r_ServerStatus ReadResponse(List<IMAP_r_u_Capability> capability, IMAP_Client_SelectedFolder folderInfo, List<int> search, List<IMAP_r_u_List> list, List<IMAP_r_u_LSub> lsub, List<IMAP_r_u_Acl> acl, List<IMAP_Response_MyRights> myRights, List<IMAP_r_u_ListRights> listRights, List<IMAP_r_u_Status> status, List<IMAP_r_u_Quota> quota, List<IMAP_r_u_QuotaRoot> quotaRoot, List<IMAP_r_u_Namespace> nspace, IMAP_Client_FetchHandler fetchHandler, List<IMAP_r_u_Enable> enable)
		{
			SmartStream.ReadLineAsyncOP readLineAsyncOP = new SmartStream.ReadLineAsyncOP(new byte[32000], SizeExceededAction.JunkAndThrowException);
			string lineUtf;
			for (;;)
			{
				this.TcpStream.ReadLine(readLineAsyncOP, false);
				bool flag = readLineAsyncOP.Error != null;
				if (flag)
				{
					break;
				}
				lineUtf = readLineAsyncOP.LineUtf8;
				base.LogAddRead((long)readLineAsyncOP.BytesInBuffer, lineUtf);
				bool flag2 = lineUtf.StartsWith("*");
				if (!flag2)
				{
					goto IL_719;
				}
				string[] array = lineUtf.Split(new char[]
				{
					' '
				}, 4);
				string text = lineUtf.Split(new char[]
				{
					' '
				})[1];
				bool flag3 = text.Equals("OK", StringComparison.InvariantCultureIgnoreCase);
				if (flag3)
				{
					IMAP_r_u_ServerStatus imap_r_u_ServerStatus = IMAP_r_u_ServerStatus.Parse(lineUtf);
					bool flag4 = !string.IsNullOrEmpty(imap_r_u_ServerStatus.OptionalResponseCode);
					if (flag4)
					{
						bool flag5 = imap_r_u_ServerStatus.OptionalResponseCode.Equals("PERMANENTFLAGS", StringComparison.InvariantCultureIgnoreCase);
						if (flag5)
						{
							bool flag6 = folderInfo != null;
							if (flag6)
							{
								StringReader stringReader = new StringReader(imap_r_u_ServerStatus.OptionalResponseArgs);
								folderInfo.SetPermanentFlags(stringReader.ReadParenthesized().Split(new char[]
								{
									' '
								}));
							}
						}
						else
						{
							bool flag7 = imap_r_u_ServerStatus.OptionalResponseCode.Equals("READ-ONLY", StringComparison.InvariantCultureIgnoreCase);
							if (flag7)
							{
								bool flag8 = folderInfo != null;
								if (flag8)
								{
									folderInfo.SetReadOnly(true);
								}
							}
							else
							{
								bool flag9 = imap_r_u_ServerStatus.OptionalResponseCode.Equals("READ-WRITE", StringComparison.InvariantCultureIgnoreCase);
								if (flag9)
								{
									bool flag10 = folderInfo != null;
									if (flag10)
									{
										folderInfo.SetReadOnly(true);
									}
								}
								else
								{
									bool flag11 = imap_r_u_ServerStatus.OptionalResponseCode.Equals("UIDNEXT", StringComparison.InvariantCultureIgnoreCase);
									if (flag11)
									{
										bool flag12 = folderInfo != null;
										if (flag12)
										{
											folderInfo.SetUidNext(Convert.ToInt64(imap_r_u_ServerStatus.OptionalResponseArgs));
										}
									}
									else
									{
										bool flag13 = imap_r_u_ServerStatus.OptionalResponseCode.Equals("UIDVALIDITY", StringComparison.InvariantCultureIgnoreCase);
										if (flag13)
										{
											bool flag14 = folderInfo != null;
											if (flag14)
											{
												folderInfo.SetUidValidity(Convert.ToInt64(imap_r_u_ServerStatus.OptionalResponseArgs));
											}
										}
										else
										{
											bool flag15 = imap_r_u_ServerStatus.OptionalResponseCode.Equals("UNSEEN", StringComparison.InvariantCultureIgnoreCase);
											if (flag15)
											{
												bool flag16 = folderInfo != null;
												if (flag16)
												{
													folderInfo.SetFirstUnseen(Convert.ToInt32(imap_r_u_ServerStatus.OptionalResponseArgs));
												}
											}
										}
									}
								}
							}
						}
					}
					this.OnUntaggedStatusResponse(imap_r_u_ServerStatus);
				}
				else
				{
					bool flag17 = text.Equals("NO", StringComparison.InvariantCultureIgnoreCase);
					if (flag17)
					{
						this.OnUntaggedStatusResponse(IMAP_r_u_ServerStatus.Parse(lineUtf));
					}
					else
					{
						bool flag18 = text.Equals("BAD", StringComparison.InvariantCultureIgnoreCase);
						if (flag18)
						{
							this.OnUntaggedStatusResponse(IMAP_r_u_ServerStatus.Parse(lineUtf));
						}
						else
						{
							bool flag19 = text.Equals("PREAUTH", StringComparison.InvariantCultureIgnoreCase);
							if (flag19)
							{
								this.OnUntaggedStatusResponse(IMAP_r_u_ServerStatus.Parse(lineUtf));
							}
							else
							{
								bool flag20 = text.Equals("BYE", StringComparison.InvariantCultureIgnoreCase);
								if (flag20)
								{
									this.OnUntaggedStatusResponse(IMAP_r_u_ServerStatus.Parse(lineUtf));
								}
								else
								{
									bool flag21 = text.Equals("CAPABILITY", StringComparison.InvariantCultureIgnoreCase);
									if (flag21)
									{
										bool flag22 = capability != null;
										if (flag22)
										{
											capability.Add(IMAP_r_u_Capability.Parse(lineUtf));
										}
									}
									else
									{
										bool flag23 = text.Equals("LIST", StringComparison.InvariantCultureIgnoreCase);
										if (flag23)
										{
											bool flag24 = list != null;
											if (flag24)
											{
												list.Add(IMAP_r_u_List.Parse(lineUtf));
											}
										}
										else
										{
											bool flag25 = text.Equals("LSUB", StringComparison.InvariantCultureIgnoreCase);
											if (flag25)
											{
												bool flag26 = lsub != null;
												if (flag26)
												{
													lsub.Add(IMAP_r_u_LSub.Parse(lineUtf));
												}
											}
											else
											{
												bool flag27 = text.Equals("STATUS", StringComparison.InvariantCultureIgnoreCase);
												if (flag27)
												{
													bool flag28 = status != null;
													if (flag28)
													{
														status.Add(IMAP_r_u_Status.Parse(lineUtf));
													}
												}
												else
												{
													bool flag29 = text.Equals("SEARCH", StringComparison.InvariantCultureIgnoreCase);
													if (flag29)
													{
														bool flag30 = search != null;
														if (flag30)
														{
															bool flag31 = lineUtf.Split(new char[]
															{
																' '
															}).Length > 2;
															if (flag31)
															{
																foreach (string value in lineUtf.Split(new char[]
																{
																	' '
																}, 3)[2].Split(new char[]
																{
																	' '
																}))
																{
																	search.Add(Convert.ToInt32(value));
																}
															}
														}
													}
													else
													{
														bool flag32 = text.Equals("FLAGS", StringComparison.InvariantCultureIgnoreCase);
														if (flag32)
														{
															bool flag33 = folderInfo != null;
															if (flag33)
															{
																StringReader stringReader2 = new StringReader(lineUtf.Split(new char[]
																{
																	' '
																}, 3)[2]);
																folderInfo.SetFlags(stringReader2.ReadParenthesized().Split(new char[]
																{
																	' '
																}));
															}
														}
														else
														{
															bool flag34 = Net_Utils.IsInteger(text) && array[2].Equals("EXISTS", StringComparison.InvariantCultureIgnoreCase);
															if (flag34)
															{
																bool flag35 = folderInfo != null;
																if (flag35)
																{
																	folderInfo.SetMessagesCount(Convert.ToInt32(text));
																}
															}
															else
															{
																bool flag36 = Net_Utils.IsInteger(text) && array[2].Equals("RECENT", StringComparison.InvariantCultureIgnoreCase);
																if (flag36)
																{
																	bool flag37 = folderInfo != null;
																	if (flag37)
																	{
																		folderInfo.SetRecentMessagesCount(Convert.ToInt32(text));
																	}
																}
																else
																{
																	bool flag38 = Net_Utils.IsInteger(text) && array[2].Equals("EXPUNGE", StringComparison.InvariantCultureIgnoreCase);
																	if (flag38)
																	{
																		this.OnMessageExpunged(IMAP_r_u_Expunge.Parse(lineUtf));
																	}
																	else
																	{
																		bool flag39 = Net_Utils.IsInteger(text) && array[2].Equals("FETCH", StringComparison.InvariantCultureIgnoreCase);
																		if (flag39)
																		{
																			bool flag40 = fetchHandler == null;
																			if (flag40)
																			{
																				fetchHandler = new IMAP_Client_FetchHandler();
																			}
																			IMAP_Client._FetchResponseReader fetchResponseReader = new IMAP_Client._FetchResponseReader(this, lineUtf, fetchHandler);
																			fetchResponseReader.Start();
																		}
																		else
																		{
																			bool flag41 = text.Equals("ACL", StringComparison.InvariantCultureIgnoreCase);
																			if (flag41)
																			{
																				bool flag42 = acl != null;
																				if (flag42)
																				{
																					acl.Add(IMAP_r_u_Acl.Parse(lineUtf));
																				}
																			}
																			else
																			{
																				bool flag43 = text.Equals("LISTRIGHTS", StringComparison.InvariantCultureIgnoreCase);
																				if (flag43)
																				{
																					bool flag44 = listRights != null;
																					if (flag44)
																					{
																						listRights.Add(IMAP_r_u_ListRights.Parse(lineUtf));
																					}
																				}
																				else
																				{
																					bool flag45 = text.Equals("MYRIGHTS", StringComparison.InvariantCultureIgnoreCase);
																					if (flag45)
																					{
																						bool flag46 = myRights != null;
																						if (flag46)
																						{
																							myRights.Add(IMAP_Response_MyRights.Parse(lineUtf));
																						}
																					}
																					else
																					{
																						bool flag47 = text.Equals("QUOTA", StringComparison.InvariantCultureIgnoreCase);
																						if (flag47)
																						{
																							bool flag48 = quota != null;
																							if (flag48)
																							{
																								quota.Add(IMAP_r_u_Quota.Parse(lineUtf));
																							}
																						}
																						else
																						{
																							bool flag49 = text.Equals("QUOTAROOT", StringComparison.InvariantCultureIgnoreCase);
																							if (flag49)
																							{
																								bool flag50 = quotaRoot != null;
																								if (flag50)
																								{
																									quotaRoot.Add(IMAP_r_u_QuotaRoot.Parse(lineUtf));
																								}
																							}
																							else
																							{
																								bool flag51 = text.Equals("NAMESPACE", StringComparison.InvariantCultureIgnoreCase);
																								if (flag51)
																								{
																									bool flag52 = nspace != null;
																									if (flag52)
																									{
																										nspace.Add(IMAP_r_u_Namespace.Parse(lineUtf));
																									}
																								}
																								else
																								{
																									bool flag53 = text.Equals("ENABLED", StringComparison.InvariantCultureIgnoreCase);
																									if (flag53)
																									{
																										bool flag54 = enable != null;
																										if (flag54)
																										{
																											enable.Add(IMAP_r_u_Enable.Parse(lineUtf));
																										}
																									}
																								}
																							}
																						}
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			throw readLineAsyncOP.Error;
			IL_719:
			bool flag55 = lineUtf.StartsWith("+");
			IMAP_r_ServerStatus result;
			if (flag55)
			{
				result = new IMAP_r_ServerStatus("+", "+", "+");
			}
			else
			{
				result = IMAP_r_ServerStatus.Parse(lineUtf);
			}
			return result;
		}

		// Token: 0x0600140E RID: 5134 RVA: 0x0007DE54 File Offset: 0x0007CE54
		[Obsolete("Use Search(bool uid,Encoding charset,IMAP_Search_Key criteria) instead.")]
		public int[] Search(bool uid, string charset, string criteria)
		{
			bool flag = criteria == null;
			if (flag)
			{
				throw new ArgumentNullException("criteria");
			}
			bool flag2 = criteria == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("Argument 'criteria' value must be specified.", "criteria");
			}
			bool flag3 = !this.IsConnected;
			if (flag3)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag4 = !base.IsAuthenticated;
			if (flag4)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag5 = this.m_pSelectedFolder == null;
			if (flag5)
			{
				throw new InvalidOperationException("Not selected state, you need to select some folder first.");
			}
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = stringBuilder;
			int commandIndex = this.m_CommandIndex;
			this.m_CommandIndex = commandIndex + 1;
			stringBuilder2.Append(commandIndex.ToString("d5"));
			if (uid)
			{
				stringBuilder.Append(" UID");
			}
			stringBuilder.Append(" SEARCH");
			bool flag6 = !string.IsNullOrEmpty(charset);
			if (flag6)
			{
				stringBuilder.Append(" CHARSET " + charset);
			}
			stringBuilder.Append(" " + criteria + "\r\n");
			this.SendCommand(stringBuilder.ToString());
			List<int> retVal = new List<int>();
			IMAP_r_ServerStatus imap_r_ServerStatus = this.ReadFinalResponse(delegate(object sender, EventArgs<IMAP_r_u> e)
			{
				bool flag8 = e.Value is IMAP_r_u_Search;
				if (flag8)
				{
					retVal.AddRange(((IMAP_r_u_Search)e.Value).Values);
				}
			});
			bool flag7 = !imap_r_ServerStatus.ResponseCode.Equals("OK", StringComparison.InvariantCultureIgnoreCase);
			if (flag7)
			{
				throw new IMAP_ClientException(imap_r_ServerStatus.ResponseCode, imap_r_ServerStatus.ResponseText);
			}
			return retVal.ToArray();
		}

		// Token: 0x0600140F RID: 5135 RVA: 0x0007DFE0 File Offset: 0x0007CFE0
		[Obsolete("Use Fetch(bool uid,IMAP_t_SeqSet seqSet,IMAP_Fetch_DataItem[] items,EventHandler<EventArgs<IMAP_r_u>> callback) intead.")]
		public void Fetch(bool uid, IMAP_SequenceSet seqSet, IMAP_Fetch_DataItem[] items, IMAP_Client_FetchHandler handler)
		{
			bool flag = seqSet == null;
			if (flag)
			{
				throw new ArgumentNullException("seqSet");
			}
			bool flag2 = items == null;
			if (flag2)
			{
				throw new ArgumentNullException("items");
			}
			bool flag3 = items.Length < 1;
			if (flag3)
			{
				throw new ArgumentException("Argument 'items' must conatain at least 1 value.", "items");
			}
			bool flag4 = handler == null;
			if (flag4)
			{
				throw new ArgumentNullException("handler");
			}
			bool flag5 = !this.IsConnected;
			if (flag5)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag6 = !base.IsAuthenticated;
			if (flag6)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag7 = this.m_pSelectedFolder == null;
			if (flag7)
			{
				throw new InvalidOperationException("Not selected state, you need to select some folder first.");
			}
			bool flag8 = this.m_pIdle != null;
			if (flag8)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = stringBuilder;
			int commandIndex = this.m_CommandIndex;
			this.m_CommandIndex = commandIndex + 1;
			stringBuilder2.Append(commandIndex.ToString("d5"));
			if (uid)
			{
				stringBuilder.Append(" UID");
			}
			stringBuilder.Append(" FETCH " + seqSet.ToSequenceSetString() + " (");
			for (int i = 0; i < items.Length; i++)
			{
				bool flag9 = i > 0;
				if (flag9)
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(items[i].ToString());
			}
			stringBuilder.Append(")\r\n");
			this.SendCommand(stringBuilder.ToString());
			IMAP_r_ServerStatus imap_r_ServerStatus = this.ReadResponse(null, null, null, null, null, null, null, null, null, null, null, null, handler, null);
			bool flag10 = !imap_r_ServerStatus.ResponseCode.Equals("OK", StringComparison.InvariantCultureIgnoreCase);
			if (flag10)
			{
				throw new IMAP_ClientException(imap_r_ServerStatus.ResponseCode, imap_r_ServerStatus.ResponseText);
			}
		}

		// Token: 0x06001410 RID: 5136 RVA: 0x0007E1B1 File Offset: 0x0007D1B1
		[Obsolete("Use method StoreMessage(string folder,IMAP_t_MsgFlags flags,DateTime internalDate,Stream message,int count) instead.")]
		public void StoreMessage(string folder, IMAP_MessageFlags flags, DateTime internalDate, Stream message, int count)
		{
			this.StoreMessage(folder, IMAP_Utils.MessageFlagsToStringArray(flags), internalDate, message, count);
		}

		// Token: 0x06001411 RID: 5137 RVA: 0x0007E1C8 File Offset: 0x0007D1C8
		[Obsolete("Use method public void StoreMessageFlags(bool uid,IMAP_t_SeqSet seqSet,IMAP_Flags_SetType setType,IMAP_t_MsgFlags flags) instead.")]
		public void StoreMessageFlags(bool uid, IMAP_SequenceSet seqSet, IMAP_Flags_SetType setType, string[] flags)
		{
			bool flag = seqSet == null;
			if (flag)
			{
				throw new ArgumentNullException("seqSet");
			}
			bool flag2 = flags == null;
			if (flag2)
			{
				throw new ArgumentNullException("flags");
			}
			this.StoreMessageFlags(uid, IMAP_t_SeqSet.Parse(seqSet.ToSequenceSetString()), setType, new IMAP_t_MsgFlags(flags));
		}

		// Token: 0x06001412 RID: 5138 RVA: 0x0007E21A File Offset: 0x0007D21A
		[Obsolete("Use method public void StoreMessageFlags(bool uid,IMAP_t_SeqSet seqSet,IMAP_Flags_SetType setType,IMAP_t_MsgFlags flags) instead.")]
		public void StoreMessageFlags(bool uid, IMAP_SequenceSet seqSet, IMAP_Flags_SetType setType, IMAP_MessageFlags flags)
		{
			this.StoreMessageFlags(uid, seqSet, setType, IMAP_Utils.MessageFlagsToStringArray(flags));
		}

		// Token: 0x06001413 RID: 5139 RVA: 0x0007E230 File Offset: 0x0007D230
		[Obsolete("Use method 'CopyMessages(bool uid,IMAP_t_SeqSet seqSet,string targetFolder)' instead.")]
		public void CopyMessages(bool uid, IMAP_SequenceSet seqSet, string targetFolder)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pSelectedFolder == null;
			if (flag3)
			{
				throw new InvalidOperationException("Not selected state, you need to select some folder first.");
			}
			bool flag4 = this.m_pIdle != null;
			if (flag4)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag5 = seqSet == null;
			if (flag5)
			{
				throw new ArgumentNullException("seqSet");
			}
			bool flag6 = targetFolder == null;
			if (flag6)
			{
				throw new ArgumentNullException("folder");
			}
			bool flag7 = targetFolder == string.Empty;
			if (flag7)
			{
				throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
			}
			this.CopyMessages(uid, IMAP_t_SeqSet.Parse(seqSet.ToSequenceSetString()), targetFolder);
		}

		// Token: 0x06001414 RID: 5140 RVA: 0x0007E324 File Offset: 0x0007D324
		[Obsolete("Use method 'MoveMessages(bool uid,IMAP_t_SeqSet seqSet,string targetFolder,bool expunge)' instead.")]
		public void MoveMessages(bool uid, IMAP_SequenceSet seqSet, string targetFolder, bool expunge)
		{
			bool flag = seqSet == null;
			if (flag)
			{
				throw new ArgumentNullException("seqSet");
			}
			bool flag2 = targetFolder == null;
			if (flag2)
			{
				throw new ArgumentNullException("folder");
			}
			bool flag3 = targetFolder == string.Empty;
			if (flag3)
			{
				throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
			}
			bool flag4 = !this.IsConnected;
			if (flag4)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag5 = !base.IsAuthenticated;
			if (flag5)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag6 = this.m_pSelectedFolder == null;
			if (flag6)
			{
				throw new InvalidOperationException("Not selected state, you need to select some folder first.");
			}
			bool flag7 = this.m_pIdle != null;
			if (flag7)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			this.MoveMessages(uid, IMAP_t_SeqSet.Parse(seqSet.ToSequenceSetString()), targetFolder, expunge);
		}

		// Token: 0x06001415 RID: 5141 RVA: 0x0007E3FC File Offset: 0x0007D3FC
		[Obsolete("Use method 'GetQuota' instead.")]
		public IMAP_r_u_Quota[] GetFolderQuota(string quotaRootName)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("Not connected, you need to connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("Not authenticated, you need to authenticate first.");
			}
			bool flag3 = this.m_pIdle != null;
			if (flag3)
			{
				throw new InvalidOperationException("This command is not valid in IDLE state, you need stop idling before calling this command.");
			}
			bool flag4 = quotaRootName == null;
			if (flag4)
			{
				throw new ArgumentNullException("quotaRootName");
			}
			List<IMAP_r_u_Quota> retVal = new List<IMAP_r_u_Quota>();
			EventHandler<EventArgs<IMAP_r_u>> callback = delegate(object sender, EventArgs<IMAP_r_u> e)
			{
				bool flag7 = e.Value is IMAP_r_u_Quota;
				if (flag7)
				{
					retVal.Add((IMAP_r_u_Quota)e.Value);
				}
			};
			using (IMAP_Client.GetQuotaAsyncOP getQuotaAsyncOP = new IMAP_Client.GetQuotaAsyncOP(quotaRootName, callback))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					getQuotaAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.GetQuotaAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag5 = !this.GetQuotaAsync(getQuotaAsyncOP);
					if (flag5)
					{
						wait.Set();
					}
					wait.WaitOne();
					bool flag6 = getQuotaAsyncOP.Error != null;
					if (flag6)
					{
						throw getQuotaAsyncOP.Error;
					}
				}
			}
			return retVal.ToArray();
		}

		// Token: 0x06001416 RID: 5142 RVA: 0x0007E57C File Offset: 0x0007D57C
		[Obsolete("deprecated")]
		private string ReadStringLiteral(int count)
		{
			string result = this.TcpStream.ReadFixedCountString(count);
			base.LogAddRead((long)count, "Readed string-literal " + count.ToString() + " bytes.");
			return result;
		}

		// Token: 0x06001417 RID: 5143 RVA: 0x0007E5BC File Offset: 0x0007D5BC
		[Obsolete("deprecated")]
		private void ReadStringLiteral(int count, Stream stream)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			this.TcpStream.ReadFixedCount(stream, (long)count);
			base.LogAddRead((long)count, "Readed string-literal " + count.ToString() + " bytes.");
		}

		// Token: 0x06001418 RID: 5144 RVA: 0x0007E60C File Offset: 0x0007D60C
		[Obsolete("Deprecated.")]
		private void SendCommand(string command)
		{
			bool flag = command == null;
			if (flag)
			{
				throw new ArgumentNullException("command");
			}
			byte[] bytes = Encoding.UTF8.GetBytes(command);
			this.TcpStream.Write(bytes, 0, bytes.Length);
			base.LogAddWrite((long)command.TrimEnd(new char[0]).Length, command.TrimEnd(new char[0]));
		}

		// Token: 0x06001419 RID: 5145 RVA: 0x0007E670 File Offset: 0x0007D670
		[Obsolete("deprecated")]
		private IMAP_r_ServerStatus ReadFinalResponse(EventHandler<EventArgs<IMAP_r_u>> callback)
		{
			ManualResetEvent wait = new ManualResetEvent(false);
			IMAP_r_ServerStatus finalResponse;
			using (IMAP_Client.ReadFinalResponseAsyncOP readFinalResponseAsyncOP = new IMAP_Client.ReadFinalResponseAsyncOP(callback))
			{
				readFinalResponseAsyncOP.CompletedAsync += delegate(object s1, EventArgs<IMAP_Client.ReadFinalResponseAsyncOP> e1)
				{
					wait.Set();
				};
				bool flag = !this.ReadFinalResponseAsync(readFinalResponseAsyncOP);
				if (flag)
				{
					wait.Set();
				}
				wait.WaitOne();
				wait.Close();
				bool flag2 = readFinalResponseAsyncOP.Error != null;
				if (flag2)
				{
					throw readFinalResponseAsyncOP.Error;
				}
				finalResponse = readFinalResponseAsyncOP.FinalResponse;
			}
			return finalResponse;
		}

		// Token: 0x040007C7 RID: 1991
		private GenericIdentity m_pAuthenticatedUser = null;

		// Token: 0x040007C8 RID: 1992
		private string m_GreetingText = "";

		// Token: 0x040007C9 RID: 1993
		private int m_CommandIndex = 1;

		// Token: 0x040007CA RID: 1994
		private List<string> m_pCapabilities = null;

		// Token: 0x040007CB RID: 1995
		private IMAP_Client_SelectedFolder m_pSelectedFolder = null;

		// Token: 0x040007CC RID: 1996
		private IMAP_Mailbox_Encoding m_MailboxEncoding = IMAP_Mailbox_Encoding.ImapUtf7;

		// Token: 0x040007CD RID: 1997
		private IMAP_Client.IdleAsyncOP m_pIdle = null;

		// Token: 0x040007CE RID: 1998
		private IMAP_Client.SettingsHolder m_pSettings = null;

		// Token: 0x02000323 RID: 803
		public class SettingsHolder
		{
			// Token: 0x06001A4C RID: 6732 RVA: 0x000A26FC File Offset: 0x000A16FC
			internal SettingsHolder()
			{
			}

			// Token: 0x17000863 RID: 2147
			// (get) Token: 0x06001A4D RID: 6733 RVA: 0x000A2714 File Offset: 0x000A1714
			// (set) Token: 0x06001A4E RID: 6734 RVA: 0x000A272C File Offset: 0x000A172C
			public int ResponseLineSize
			{
				get
				{
					return this.m_ResponseLineSize;
				}
				set
				{
					bool flag = value < 32000;
					if (flag)
					{
						throw new ArgumentException("Value must be >= 32000(32kb).", "value");
					}
					this.m_ResponseLineSize = value;
				}
			}

			// Token: 0x04000BF8 RID: 3064
			private int m_ResponseLineSize = 512000;
		}

		// Token: 0x02000324 RID: 804
		internal class CmdLine
		{
			// Token: 0x06001A4F RID: 6735 RVA: 0x000A2760 File Offset: 0x000A1760
			public CmdLine(byte[] data, string logText)
			{
				bool flag = data == null;
				if (flag)
				{
					throw new ArgumentNullException("data");
				}
				bool flag2 = logText == null;
				if (flag2)
				{
					throw new ArgumentNullException("logText");
				}
				this.m_pData = data;
				this.m_LogText = logText;
			}

			// Token: 0x17000864 RID: 2148
			// (get) Token: 0x06001A50 RID: 6736 RVA: 0x000A27BC File Offset: 0x000A17BC
			public byte[] Data
			{
				get
				{
					return this.m_pData;
				}
			}

			// Token: 0x17000865 RID: 2149
			// (get) Token: 0x06001A51 RID: 6737 RVA: 0x000A27D4 File Offset: 0x000A17D4
			public string LogText
			{
				get
				{
					return this.m_LogText;
				}
			}

			// Token: 0x04000BF9 RID: 3065
			private byte[] m_pData = null;

			// Token: 0x04000BFA RID: 3066
			private string m_LogText = null;
		}

		// Token: 0x02000325 RID: 805
		public abstract class CmdAsyncOP<T> : IDisposable, IAsyncOP where T : IAsyncOP
		{
			// Token: 0x06001A52 RID: 6738 RVA: 0x000A27EC File Offset: 0x000A17EC
			public CmdAsyncOP(EventHandler<EventArgs<IMAP_r_u>> callback)
			{
				this.m_pCallback = callback;
				this.m_pCmdLines = new List<IMAP_Client.CmdLine>();
			}

			// Token: 0x06001A53 RID: 6739 RVA: 0x000A2858 File Offset: 0x000A1858
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pImapClient = null;
					this.m_pFinalResponse = null;
					this.m_pCallback = null;
					this.m_pCmdLines = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x06001A54 RID: 6740 RVA: 0x000A28A8 File Offset: 0x000A18A8
			internal bool Start(IMAP_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pImapClient = owner;
				this.SetState(AsyncOP_State.Active);
				try
				{
					this.OnInitCmdLine(owner);
					IMAP_Client.SendCmdAndReadRespAsyncOP op = new IMAP_Client.SendCmdAndReadRespAsyncOP(this.m_pCmdLines.ToArray(), this.m_pCallback);
					op.CompletedAsync += delegate(object sender, EventArgs<IMAP_Client.SendCmdAndReadRespAsyncOP> e)
					{
						try
						{
							bool flag5 = op.Error != null;
							if (flag5)
							{
								this.m_pException = e.Value.Error;
								this.m_pImapClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
							}
							else
							{
								this.m_pFinalResponse = op.FinalResponse;
								bool isError2 = op.FinalResponse.IsError;
								if (isError2)
								{
									this.m_pException = new IMAP_ClientException(op.FinalResponse);
								}
							}
							this.SetState(AsyncOP_State.Completed);
						}
						finally
						{
							op.Dispose();
						}
					};
					bool flag2 = !this.m_pImapClient.SendCmdAndReadRespAsync(op);
					if (flag2)
					{
						try
						{
							bool flag3 = op.Error != null;
							if (flag3)
							{
								this.m_pException = op.Error;
								this.m_pImapClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
							}
							else
							{
								this.m_pFinalResponse = op.FinalResponse;
								bool isError = op.FinalResponse.IsError;
								if (isError)
								{
									this.m_pException = new IMAP_ClientException(op.FinalResponse);
								}
							}
							this.SetState(AsyncOP_State.Completed);
						}
						finally
						{
							op.Dispose();
						}
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.m_pImapClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					this.SetState(AsyncOP_State.Completed);
				}
				object pLock = this.m_pLock;
				bool result;
				lock (pLock)
				{
					this.m_RiseCompleted = true;
					result = (this.m_State == AsyncOP_State.Active);
				}
				return result;
			}

			// Token: 0x06001A55 RID: 6741
			protected abstract void OnInitCmdLine(IMAP_Client imap);

			// Token: 0x06001A56 RID: 6742 RVA: 0x000A2AB0 File Offset: 0x000A1AB0
			private void SetState(AsyncOP_State state)
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					object pLock = this.m_pLock;
					lock (pLock)
					{
						this.m_State = state;
						bool flag3 = this.m_State == AsyncOP_State.Completed && this.m_RiseCompleted;
						if (flag3)
						{
							this.OnCompletedAsync();
						}
					}
				}
			}

			// Token: 0x17000866 RID: 2150
			// (get) Token: 0x06001A57 RID: 6743 RVA: 0x000A2B28 File Offset: 0x000A1B28
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x17000867 RID: 2151
			// (get) Token: 0x06001A58 RID: 6744 RVA: 0x000A2B40 File Offset: 0x000A1B40
			public Exception Error
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Error' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					return this.m_pException;
				}
			}

			// Token: 0x17000868 RID: 2152
			// (get) Token: 0x06001A59 RID: 6745 RVA: 0x000A2B94 File Offset: 0x000A1B94
			public IMAP_r_ServerStatus FinalResponse
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Response' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					return this.m_pFinalResponse;
				}
			}

			// Token: 0x17000869 RID: 2153
			// (get) Token: 0x06001A5A RID: 6746 RVA: 0x000A2BE8 File Offset: 0x000A1BE8
			internal List<IMAP_Client.CmdLine> CmdLines
			{
				get
				{
					return this.m_pCmdLines;
				}
			}

			// Token: 0x140000C1 RID: 193
			// (add) Token: 0x06001A5B RID: 6747 RVA: 0x000A2C00 File Offset: 0x000A1C00
			// (remove) Token: 0x06001A5C RID: 6748 RVA: 0x000A2C38 File Offset: 0x000A1C38
			
			public event EventHandler<EventArgs<T>> CompletedAsync = null;

			// Token: 0x06001A5D RID: 6749 RVA: 0x000A2C70 File Offset: 0x000A1C70
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<T>((T)((object)this)));
				}
			}

			// Token: 0x04000BFB RID: 3067
			private object m_pLock = new object();

			// Token: 0x04000BFC RID: 3068
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000BFD RID: 3069
			private Exception m_pException = null;

			// Token: 0x04000BFE RID: 3070
			private IMAP_r_ServerStatus m_pFinalResponse = null;

			// Token: 0x04000BFF RID: 3071
			private IMAP_Client m_pImapClient = null;

			// Token: 0x04000C00 RID: 3072
			private bool m_RiseCompleted = false;

			// Token: 0x04000C01 RID: 3073
			private List<IMAP_Client.CmdLine> m_pCmdLines = null;

			// Token: 0x04000C02 RID: 3074
			private EventHandler<EventArgs<IMAP_r_u>> m_pCallback = null;
		}

		// Token: 0x02000326 RID: 806
		public class StartTlsAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x06001A5E RID: 6750 RVA: 0x000A2CA8 File Offset: 0x000A1CA8
			public StartTlsAsyncOP(RemoteCertificateValidationCallback certCallback, EventHandler<EventArgs<IMAP_r_u>> callback)
			{
				this.m_pCertCallback = certCallback;
				this.m_pCallback = callback;
			}

			// Token: 0x06001A5F RID: 6751 RVA: 0x000A2D10 File Offset: 0x000A1D10
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pImapClient = null;
					this.m_pFinalResponse = null;
					this.m_pCallback = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x06001A60 RID: 6752 RVA: 0x000A2D5C File Offset: 0x000A1D5C
			internal bool Start(IMAP_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pImapClient = owner;
				this.SetState(AsyncOP_State.Active);
				try
				{
					Encoding utf = Encoding.UTF8;
					IMAP_Client pImapClient = this.m_pImapClient;
					int commandIndex = pImapClient.m_CommandIndex;
					pImapClient.m_CommandIndex = commandIndex + 1;
					byte[] bytes = utf.GetBytes(commandIndex.ToString("d5") + " STARTTLS\r\n");
					string cmdLineLogText = Encoding.UTF8.GetString(bytes).TrimEnd(new char[0]);
					IMAP_Client.SendCmdAndReadRespAsyncOP sendCmdAndReadRespAsyncOP = new IMAP_Client.SendCmdAndReadRespAsyncOP(bytes, cmdLineLogText, this.m_pCallback);
					sendCmdAndReadRespAsyncOP.CompletedAsync += delegate(object sender, EventArgs<IMAP_Client.SendCmdAndReadRespAsyncOP> e)
					{
						this.ProcessCmdResult(e.Value);
					};
					bool flag2 = !this.m_pImapClient.SendCmdAndReadRespAsync(sendCmdAndReadRespAsyncOP);
					if (flag2)
					{
						this.ProcessCmdResult(sendCmdAndReadRespAsyncOP);
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.m_pImapClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					this.SetState(AsyncOP_State.Completed);
				}
				object pLock = this.m_pLock;
				bool result;
				lock (pLock)
				{
					this.m_RiseCompleted = true;
					result = (this.m_State == AsyncOP_State.Active);
				}
				return result;
			}

			// Token: 0x06001A61 RID: 6753 RVA: 0x000A2EB4 File Offset: 0x000A1EB4
			private void SetState(AsyncOP_State state)
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					object pLock = this.m_pLock;
					lock (pLock)
					{
						this.m_State = state;
						bool flag3 = this.m_State == AsyncOP_State.Completed && this.m_RiseCompleted;
						if (flag3)
						{
							this.OnCompletedAsync();
						}
					}
				}
			}

			// Token: 0x06001A62 RID: 6754 RVA: 0x000A2F2C File Offset: 0x000A1F2C
			private void ProcessCmdResult(IMAP_Client.SendCmdAndReadRespAsyncOP op)
			{
				try
				{
					bool flag = op.Error != null;
					if (flag)
					{
						this.m_pException = op.Error;
					}
					else
					{
						this.m_pFinalResponse = op.FinalResponse;
						bool isError = op.FinalResponse.IsError;
						if (isError)
						{
							this.m_pException = new IMAP_ClientException(op.FinalResponse);
							this.SetState(AsyncOP_State.Completed);
						}
						else
						{
							TCP_Client.SwitchToSecureAsyncOP switchToSecureAsyncOP = new TCP_Client.SwitchToSecureAsyncOP(this.m_pCertCallback);
							switchToSecureAsyncOP.CompletedAsync += delegate(object sender, EventArgs<TCP_Client.SwitchToSecureAsyncOP> e)
							{
								bool flag4 = e.Value.Error != null;
								if (flag4)
								{
									this.m_pException = e.Value.Error;
								}
								this.SetState(AsyncOP_State.Completed);
							};
							bool flag2 = !this.m_pImapClient.SwitchToSecureAsync(switchToSecureAsyncOP);
							if (flag2)
							{
								bool flag3 = switchToSecureAsyncOP.Error != null;
								if (flag3)
								{
									this.m_pException = switchToSecureAsyncOP.Error;
								}
								this.SetState(AsyncOP_State.Completed);
							}
						}
					}
				}
				finally
				{
					op.Dispose();
				}
			}

			// Token: 0x1700086A RID: 2154
			// (get) Token: 0x06001A63 RID: 6755 RVA: 0x000A3010 File Offset: 0x000A2010
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x1700086B RID: 2155
			// (get) Token: 0x06001A64 RID: 6756 RVA: 0x000A3028 File Offset: 0x000A2028
			public Exception Error
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Error' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					return this.m_pException;
				}
			}

			// Token: 0x1700086C RID: 2156
			// (get) Token: 0x06001A65 RID: 6757 RVA: 0x000A307C File Offset: 0x000A207C
			public IMAP_r_ServerStatus FinalResponse
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Response' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					return this.m_pFinalResponse;
				}
			}

			// Token: 0x140000C2 RID: 194
			// (add) Token: 0x06001A66 RID: 6758 RVA: 0x000A30D0 File Offset: 0x000A20D0
			// (remove) Token: 0x06001A67 RID: 6759 RVA: 0x000A3108 File Offset: 0x000A2108
			
			public event EventHandler<EventArgs<IMAP_Client.StartTlsAsyncOP>> CompletedAsync = null;

			// Token: 0x06001A68 RID: 6760 RVA: 0x000A3140 File Offset: 0x000A2140
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<IMAP_Client.StartTlsAsyncOP>(this));
				}
			}

			// Token: 0x04000C04 RID: 3076
			private object m_pLock = new object();

			// Token: 0x04000C05 RID: 3077
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000C06 RID: 3078
			private Exception m_pException = null;

			// Token: 0x04000C07 RID: 3079
			private RemoteCertificateValidationCallback m_pCertCallback = null;

			// Token: 0x04000C08 RID: 3080
			private IMAP_r_ServerStatus m_pFinalResponse = null;

			// Token: 0x04000C09 RID: 3081
			private IMAP_Client m_pImapClient = null;

			// Token: 0x04000C0A RID: 3082
			private bool m_RiseCompleted = false;

			// Token: 0x04000C0B RID: 3083
			private EventHandler<EventArgs<IMAP_r_u>> m_pCallback = null;
		}

		// Token: 0x02000327 RID: 807
		public class LoginAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x06001A6B RID: 6763 RVA: 0x000A31BC File Offset: 0x000A21BC
			public LoginAsyncOP(string user, string password, EventHandler<EventArgs<IMAP_r_u>> callback)
			{
				bool flag = user == null;
				if (flag)
				{
					throw new ArgumentNullException("user");
				}
				bool flag2 = string.IsNullOrEmpty(user);
				if (flag2)
				{
					throw new ArgumentException("Argument 'user' value must be specified.", "user");
				}
				bool flag3 = password == null;
				if (flag3)
				{
					throw new ArgumentNullException("password");
				}
				this.m_User = user;
				this.m_Password = password;
				this.m_pCallback = callback;
			}

			// Token: 0x06001A6C RID: 6764 RVA: 0x000A3274 File Offset: 0x000A2274
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pImapClient = null;
					this.m_pFinalResponse = null;
					this.m_pCallback = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x06001A6D RID: 6765 RVA: 0x000A32C0 File Offset: 0x000A22C0
			internal bool Start(IMAP_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pImapClient = owner;
				this.SetState(AsyncOP_State.Active);
				try
				{
					Encoding utf = Encoding.UTF8;
					string[] array = new string[6];
					int num = 0;
					IMAP_Client pImapClient = this.m_pImapClient;
					int commandIndex = pImapClient.m_CommandIndex;
					pImapClient.m_CommandIndex = commandIndex + 1;
					array[num] = commandIndex.ToString("d5");
					array[1] = " LOGIN ";
					array[2] = TextUtils.QuoteString(this.m_User);
					array[3] = " ";
					array[4] = TextUtils.QuoteString(this.m_Password);
					array[5] = "\r\n";
					byte[] bytes = utf.GetBytes(string.Concat(array));
					string cmdLineLogText = (this.m_pImapClient.m_CommandIndex - 1).ToString("d5") + " LOGIN " + TextUtils.QuoteString(this.m_User) + " <PASSWORD-REMOVED>";
					IMAP_Client.SendCmdAndReadRespAsyncOP args = new IMAP_Client.SendCmdAndReadRespAsyncOP(bytes, cmdLineLogText, this.m_pCallback);
					args.CompletedAsync += delegate(object sender, EventArgs<IMAP_Client.SendCmdAndReadRespAsyncOP> e)
					{
						try
						{
							bool flag5 = args.Error != null;
							if (flag5)
							{
								this.m_pException = e.Value.Error;
							}
							else
							{
								this.m_pFinalResponse = args.FinalResponse;
								bool isError2 = args.FinalResponse.IsError;
								if (isError2)
								{
									this.m_pException = new IMAP_ClientException(args.FinalResponse);
								}
								else
								{
									this.m_pImapClient.m_pAuthenticatedUser = new GenericIdentity(this.m_User, "IMAP-LOGIN");
								}
							}
							this.SetState(AsyncOP_State.Completed);
						}
						finally
						{
							args.Dispose();
						}
					};
					bool flag2 = !this.m_pImapClient.SendCmdAndReadRespAsync(args);
					if (flag2)
					{
						try
						{
							bool flag3 = args.Error != null;
							if (flag3)
							{
								this.m_pException = args.Error;
							}
							else
							{
								this.m_pFinalResponse = args.FinalResponse;
								bool isError = args.FinalResponse.IsError;
								if (isError)
								{
									this.m_pException = new IMAP_ClientException(args.FinalResponse);
								}
								else
								{
									this.m_pImapClient.m_pAuthenticatedUser = new GenericIdentity(this.m_User, "IMAP-LOGIN");
								}
							}
							this.SetState(AsyncOP_State.Completed);
						}
						finally
						{
							args.Dispose();
						}
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.m_pImapClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					this.SetState(AsyncOP_State.Completed);
				}
				object pLock = this.m_pLock;
				bool result;
				lock (pLock)
				{
					this.m_RiseCompleted = true;
					result = (this.m_State == AsyncOP_State.Active);
				}
				return result;
			}

			// Token: 0x06001A6E RID: 6766 RVA: 0x000A3558 File Offset: 0x000A2558
			private void SetState(AsyncOP_State state)
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					object pLock = this.m_pLock;
					lock (pLock)
					{
						this.m_State = state;
						bool flag3 = this.m_State == AsyncOP_State.Completed && this.m_RiseCompleted;
						if (flag3)
						{
							this.OnCompletedAsync();
						}
					}
				}
			}

			// Token: 0x1700086D RID: 2157
			// (get) Token: 0x06001A6F RID: 6767 RVA: 0x000A35D0 File Offset: 0x000A25D0
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x1700086E RID: 2158
			// (get) Token: 0x06001A70 RID: 6768 RVA: 0x000A35E8 File Offset: 0x000A25E8
			public Exception Error
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Error' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					return this.m_pException;
				}
			}

			// Token: 0x1700086F RID: 2159
			// (get) Token: 0x06001A71 RID: 6769 RVA: 0x000A363C File Offset: 0x000A263C
			public IMAP_r_ServerStatus FinalResponse
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Response' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					return this.m_pFinalResponse;
				}
			}

			// Token: 0x140000C3 RID: 195
			// (add) Token: 0x06001A72 RID: 6770 RVA: 0x000A3690 File Offset: 0x000A2690
			// (remove) Token: 0x06001A73 RID: 6771 RVA: 0x000A36C8 File Offset: 0x000A26C8
			
			public event EventHandler<EventArgs<IMAP_Client.LoginAsyncOP>> CompletedAsync = null;

			// Token: 0x06001A74 RID: 6772 RVA: 0x000A3700 File Offset: 0x000A2700
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<IMAP_Client.LoginAsyncOP>(this));
				}
			}

			// Token: 0x04000C0D RID: 3085
			private object m_pLock = new object();

			// Token: 0x04000C0E RID: 3086
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000C0F RID: 3087
			private Exception m_pException = null;

			// Token: 0x04000C10 RID: 3088
			private IMAP_r_ServerStatus m_pFinalResponse = null;

			// Token: 0x04000C11 RID: 3089
			private IMAP_Client m_pImapClient = null;

			// Token: 0x04000C12 RID: 3090
			private bool m_RiseCompleted = false;

			// Token: 0x04000C13 RID: 3091
			private string m_User = null;

			// Token: 0x04000C14 RID: 3092
			private string m_Password = null;

			// Token: 0x04000C15 RID: 3093
			private EventHandler<EventArgs<IMAP_r_u>> m_pCallback = null;
		}

		// Token: 0x02000328 RID: 808
		public class AuthenticateAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x06001A75 RID: 6773 RVA: 0x000A3730 File Offset: 0x000A2730
			public AuthenticateAsyncOP(AUTH_SASL_Client sasl)
			{
				bool flag = sasl == null;
				if (flag)
				{
					throw new ArgumentNullException("sasl");
				}
				this.m_pSASL = sasl;
			}

			// Token: 0x06001A76 RID: 6774 RVA: 0x000A3798 File Offset: 0x000A2798
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pImapClient = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x06001A77 RID: 6775 RVA: 0x000A37D4 File Offset: 0x000A27D4
			internal bool Start(IMAP_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pImapClient = owner;
				this.SetState(AsyncOP_State.Active);
				try
				{
					bool flag2 = this.m_pSASL.SupportsInitialResponse && this.m_pImapClient.SupportsCapability("SASL-IR");
					if (flag2)
					{
						Encoding utf = Encoding.UTF8;
						string[] array = new string[6];
						int num = 0;
						IMAP_Client pImapClient = this.m_pImapClient;
						int commandIndex = pImapClient.m_CommandIndex;
						pImapClient.m_CommandIndex = commandIndex + 1;
						array[num] = commandIndex.ToString("d5");
						array[1] = " AUTHENTICATE ";
						array[2] = this.m_pSASL.Name;
						array[3] = " ";
						array[4] = Convert.ToBase64String(this.m_pSASL.Continue(null));
						array[5] = "\r\n";
						byte[] bytes = utf.GetBytes(string.Concat(array));
						this.m_pImapClient.LogAddWrite((long)bytes.Length, Encoding.UTF8.GetString(bytes).TrimEnd(new char[0]));
						this.m_pImapClient.TcpStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(this.AuthenticateCommandSendingCompleted), null);
					}
					else
					{
						Encoding utf2 = Encoding.UTF8;
						IMAP_Client pImapClient2 = this.m_pImapClient;
						int commandIndex = pImapClient2.m_CommandIndex;
						pImapClient2.m_CommandIndex = commandIndex + 1;
						byte[] bytes2 = utf2.GetBytes(commandIndex.ToString("d5") + " AUTHENTICATE " + this.m_pSASL.Name + "\r\n");
						TCP_Client pImapClient3 = this.m_pImapClient;
						long size = (long)bytes2.Length;
						IMAP_Client pImapClient4 = this.m_pImapClient;
						commandIndex = pImapClient4.m_CommandIndex;
						pImapClient4.m_CommandIndex = commandIndex + 1;
						pImapClient3.LogAddWrite(size, commandIndex.ToString("d5") + " AUTHENTICATE " + this.m_pSASL.Name);
						this.m_pImapClient.TcpStream.BeginWrite(bytes2, 0, bytes2.Length, new AsyncCallback(this.AuthenticateCommandSendingCompleted), null);
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pImapClient.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
				object pLock = this.m_pLock;
				bool result;
				lock (pLock)
				{
					this.m_RiseCompleted = true;
					result = (this.m_State == AsyncOP_State.Active);
				}
				return result;
			}

			// Token: 0x06001A78 RID: 6776 RVA: 0x000A3A4C File Offset: 0x000A2A4C
			private void SetState(AsyncOP_State state)
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					object pLock = this.m_pLock;
					lock (pLock)
					{
						this.m_State = state;
						bool flag3 = this.m_State == AsyncOP_State.Completed && this.m_RiseCompleted;
						if (flag3)
						{
							this.OnCompletedAsync();
						}
					}
				}
			}

			// Token: 0x06001A79 RID: 6777 RVA: 0x000A3AC4 File Offset: 0x000A2AC4
			private void AuthenticateCommandSendingCompleted(IAsyncResult ar)
			{
				try
				{
					this.m_pImapClient.TcpStream.EndWrite(ar);
					IMAP_Client.ReadFinalResponseAsyncOP op = new IMAP_Client.ReadFinalResponseAsyncOP(null);
					op.CompletedAsync += delegate(object s, EventArgs<IMAP_Client.ReadFinalResponseAsyncOP> e)
					{
						this.AuthenticateReadResponseCompleted(op);
					};
					bool flag = !this.m_pImapClient.ReadFinalResponseAsync(op);
					if (flag)
					{
						this.AuthenticateReadResponseCompleted(op);
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pImapClient.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x06001A7A RID: 6778 RVA: 0x000A3B84 File Offset: 0x000A2B84
			private void AuthenticateReadResponseCompleted(IMAP_Client.ReadFinalResponseAsyncOP op)
			{
				try
				{
					bool isContinue = op.FinalResponse.IsContinue;
					if (isContinue)
					{
						byte[] serverResponse = Convert.FromBase64String(op.FinalResponse.ResponseText);
						byte[] inArray = this.m_pSASL.Continue(serverResponse);
						byte[] bytes = Encoding.UTF8.GetBytes(Convert.ToBase64String(inArray) + "\r\n");
						this.m_pImapClient.LogAddWrite((long)bytes.Length, Convert.ToBase64String(inArray));
						this.m_pImapClient.TcpStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(this.AuthenticateCommandSendingCompleted), null);
					}
					else
					{
						bool flag = !op.FinalResponse.IsError;
						if (flag)
						{
							this.m_pImapClient.m_pAuthenticatedUser = new GenericIdentity(this.m_pSASL.UserName, this.m_pSASL.Name);
							this.SetState(AsyncOP_State.Completed);
						}
						else
						{
							this.m_pException = new IMAP_ClientException(op.FinalResponse);
							this.SetState(AsyncOP_State.Completed);
						}
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pImapClient.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x17000870 RID: 2160
			// (get) Token: 0x06001A7B RID: 6779 RVA: 0x000A3CC4 File Offset: 0x000A2CC4
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x17000871 RID: 2161
			// (get) Token: 0x06001A7C RID: 6780 RVA: 0x000A3CDC File Offset: 0x000A2CDC
			public Exception Error
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Error' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					return this.m_pException;
				}
			}

			// Token: 0x140000C4 RID: 196
			// (add) Token: 0x06001A7D RID: 6781 RVA: 0x000A3D30 File Offset: 0x000A2D30
			// (remove) Token: 0x06001A7E RID: 6782 RVA: 0x000A3D68 File Offset: 0x000A2D68
			
			public event EventHandler<EventArgs<IMAP_Client.AuthenticateAsyncOP>> CompletedAsync = null;

			// Token: 0x06001A7F RID: 6783 RVA: 0x000A3DA0 File Offset: 0x000A2DA0
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<IMAP_Client.AuthenticateAsyncOP>(this));
				}
			}

			// Token: 0x04000C17 RID: 3095
			private object m_pLock = new object();

			// Token: 0x04000C18 RID: 3096
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000C19 RID: 3097
			private Exception m_pException = null;

			// Token: 0x04000C1A RID: 3098
			private IMAP_Client m_pImapClient = null;

			// Token: 0x04000C1B RID: 3099
			private AUTH_SASL_Client m_pSASL = null;

			// Token: 0x04000C1C RID: 3100
			private bool m_RiseCompleted = false;
		}

		// Token: 0x02000329 RID: 809
		public class GetNamespacesAsyncOP : IMAP_Client.CmdAsyncOP<IMAP_Client.GetNamespacesAsyncOP>
		{
			// Token: 0x06001A80 RID: 6784 RVA: 0x000A3DD0 File Offset: 0x000A2DD0
			public GetNamespacesAsyncOP(EventHandler<EventArgs<IMAP_r_u>> callback) : base(callback)
			{
			}

			// Token: 0x06001A81 RID: 6785 RVA: 0x000A3DDC File Offset: 0x000A2DDC
			protected override void OnInitCmdLine(IMAP_Client imap)
			{
				Encoding utf = Encoding.UTF8;
				int commandIndex = imap.m_CommandIndex;
				imap.m_CommandIndex = commandIndex + 1;
				byte[] bytes = utf.GetBytes(commandIndex.ToString("d5") + " NAMESPACE\r\n");
				base.CmdLines.Add(new IMAP_Client.CmdLine(bytes, Encoding.UTF8.GetString(bytes).TrimEnd(new char[0])));
			}
		}

		// Token: 0x0200032A RID: 810
		public class GetFoldersAsyncOP : IMAP_Client.CmdAsyncOP<IMAP_Client.GetFoldersAsyncOP>
		{
			// Token: 0x06001A82 RID: 6786 RVA: 0x000A3E43 File Offset: 0x000A2E43
			public GetFoldersAsyncOP(string filter, EventHandler<EventArgs<IMAP_r_u>> callback) : base(callback)
			{
				this.m_Filter = filter;
			}

			// Token: 0x06001A83 RID: 6787 RVA: 0x000A3E5C File Offset: 0x000A2E5C
			protected override void OnInitCmdLine(IMAP_Client imap)
			{
				bool flag = this.m_Filter != null;
				if (flag)
				{
					Encoding utf = Encoding.UTF8;
					int commandIndex = imap.m_CommandIndex;
					imap.m_CommandIndex = commandIndex + 1;
					byte[] bytes = utf.GetBytes(commandIndex.ToString("d5") + " LIST \"\" " + IMAP_Utils.EncodeMailbox(this.m_Filter, imap.m_MailboxEncoding) + "\r\n");
					base.CmdLines.Add(new IMAP_Client.CmdLine(bytes, Encoding.UTF8.GetString(bytes).TrimEnd(new char[0])));
				}
				else
				{
					Encoding utf2 = Encoding.UTF8;
					int commandIndex = imap.m_CommandIndex;
					imap.m_CommandIndex = commandIndex + 1;
					byte[] bytes2 = utf2.GetBytes(commandIndex.ToString("d5") + " LIST \"\" \"*\"\r\n");
					base.CmdLines.Add(new IMAP_Client.CmdLine(bytes2, Encoding.UTF8.GetString(bytes2).TrimEnd(new char[0])));
				}
			}

			// Token: 0x04000C1E RID: 3102
			private string m_Filter = null;
		}

		// Token: 0x0200032B RID: 811
		public class CreateFolderAsyncOP : IMAP_Client.CmdAsyncOP<IMAP_Client.CreateFolderAsyncOP>
		{
			// Token: 0x06001A84 RID: 6788 RVA: 0x000A3F48 File Offset: 0x000A2F48
			public CreateFolderAsyncOP(string folder, EventHandler<EventArgs<IMAP_r_u>> callback) : base(callback)
			{
				bool flag = folder == null;
				if (flag)
				{
					throw new ArgumentNullException("folder");
				}
				bool flag2 = string.IsNullOrEmpty(folder);
				if (flag2)
				{
					throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
				}
				this.m_Folder = folder;
			}

			// Token: 0x06001A85 RID: 6789 RVA: 0x000A3F9C File Offset: 0x000A2F9C
			protected override void OnInitCmdLine(IMAP_Client imap)
			{
				Encoding utf = Encoding.UTF8;
				int commandIndex = imap.m_CommandIndex;
				imap.m_CommandIndex = commandIndex + 1;
				byte[] bytes = utf.GetBytes(commandIndex.ToString("d5") + " CREATE " + IMAP_Utils.EncodeMailbox(this.m_Folder, imap.m_MailboxEncoding) + "\r\n");
				base.CmdLines.Add(new IMAP_Client.CmdLine(bytes, Encoding.UTF8.GetString(bytes).TrimEnd(new char[0])));
			}

			// Token: 0x04000C1F RID: 3103
			private string m_Folder = null;
		}

		// Token: 0x0200032C RID: 812
		public class DeleteFolderAsyncOP : IMAP_Client.CmdAsyncOP<IMAP_Client.DeleteFolderAsyncOP>
		{
			// Token: 0x06001A86 RID: 6790 RVA: 0x000A401C File Offset: 0x000A301C
			public DeleteFolderAsyncOP(string folder, EventHandler<EventArgs<IMAP_r_u>> callback) : base(callback)
			{
				bool flag = folder == null;
				if (flag)
				{
					throw new ArgumentNullException("folder");
				}
				bool flag2 = string.IsNullOrEmpty(folder);
				if (flag2)
				{
					throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
				}
				this.m_Folder = folder;
			}

			// Token: 0x06001A87 RID: 6791 RVA: 0x000A4070 File Offset: 0x000A3070
			protected override void OnInitCmdLine(IMAP_Client imap)
			{
				Encoding utf = Encoding.UTF8;
				int commandIndex = imap.m_CommandIndex;
				imap.m_CommandIndex = commandIndex + 1;
				byte[] bytes = utf.GetBytes(commandIndex.ToString("d5") + " DELETE " + IMAP_Utils.EncodeMailbox(this.m_Folder, imap.m_MailboxEncoding) + "\r\n");
				base.CmdLines.Add(new IMAP_Client.CmdLine(bytes, Encoding.UTF8.GetString(bytes).TrimEnd(new char[0])));
			}

			// Token: 0x04000C20 RID: 3104
			private string m_Folder = null;
		}

		// Token: 0x0200032D RID: 813
		public class RenameFolderAsyncOP : IMAP_Client.CmdAsyncOP<IMAP_Client.RenameFolderAsyncOP>
		{
			// Token: 0x06001A88 RID: 6792 RVA: 0x000A40F0 File Offset: 0x000A30F0
			public RenameFolderAsyncOP(string folder, string newFolder, EventHandler<EventArgs<IMAP_r_u>> callback) : base(callback)
			{
				bool flag = folder == null;
				if (flag)
				{
					throw new ArgumentNullException("folder");
				}
				bool flag2 = string.IsNullOrEmpty(folder);
				if (flag2)
				{
					throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
				}
				bool flag3 = newFolder == null;
				if (flag3)
				{
					throw new ArgumentNullException("newFolder");
				}
				bool flag4 = string.IsNullOrEmpty(newFolder);
				if (flag4)
				{
					throw new ArgumentException("Argument 'newFolder' value must be specified.", "newFolder");
				}
				this.m_Folder = folder;
				this.m_NewFolder = newFolder;
			}

			// Token: 0x06001A89 RID: 6793 RVA: 0x000A4180 File Offset: 0x000A3180
			protected override void OnInitCmdLine(IMAP_Client imap)
			{
				Encoding utf = Encoding.UTF8;
				string[] array = new string[6];
				int num = 0;
				int commandIndex = imap.m_CommandIndex;
				imap.m_CommandIndex = commandIndex + 1;
				array[num] = commandIndex.ToString("d5");
				array[1] = " RENAME ";
				array[2] = IMAP_Utils.EncodeMailbox(this.m_Folder, imap.m_MailboxEncoding);
				array[3] = " ";
				array[4] = IMAP_Utils.EncodeMailbox(this.m_NewFolder, imap.m_MailboxEncoding);
				array[5] = "\r\n";
				byte[] bytes = utf.GetBytes(string.Concat(array));
				base.CmdLines.Add(new IMAP_Client.CmdLine(bytes, Encoding.UTF8.GetString(bytes).TrimEnd(new char[0])));
			}

			// Token: 0x04000C21 RID: 3105
			private string m_Folder = null;

			// Token: 0x04000C22 RID: 3106
			private string m_NewFolder = null;
		}

		// Token: 0x0200032E RID: 814
		public class GetSubscribedFoldersAsyncOP : IMAP_Client.CmdAsyncOP<IMAP_Client.GetSubscribedFoldersAsyncOP>
		{
			// Token: 0x06001A8A RID: 6794 RVA: 0x000A422B File Offset: 0x000A322B
			public GetSubscribedFoldersAsyncOP(string filter, EventHandler<EventArgs<IMAP_r_u>> callback) : base(callback)
			{
				this.m_Filter = filter;
			}

			// Token: 0x06001A8B RID: 6795 RVA: 0x000A4244 File Offset: 0x000A3244
			protected override void OnInitCmdLine(IMAP_Client imap)
			{
				bool flag = this.m_Filter != null;
				if (flag)
				{
					Encoding utf = Encoding.UTF8;
					int commandIndex = imap.m_CommandIndex;
					imap.m_CommandIndex = commandIndex + 1;
					byte[] bytes = utf.GetBytes(commandIndex.ToString("d5") + " LSUB \"\" " + IMAP_Utils.EncodeMailbox(this.m_Filter, imap.m_MailboxEncoding) + "\r\n");
					base.CmdLines.Add(new IMAP_Client.CmdLine(bytes, Encoding.UTF8.GetString(bytes).TrimEnd(new char[0])));
				}
				else
				{
					Encoding utf2 = Encoding.UTF8;
					int commandIndex = imap.m_CommandIndex;
					imap.m_CommandIndex = commandIndex + 1;
					byte[] bytes2 = utf2.GetBytes(commandIndex.ToString("d5") + " LSUB \"\" \"*\"\r\n");
					base.CmdLines.Add(new IMAP_Client.CmdLine(bytes2, Encoding.UTF8.GetString(bytes2).TrimEnd(new char[0])));
				}
			}

			// Token: 0x04000C23 RID: 3107
			private string m_Filter = null;
		}

		// Token: 0x0200032F RID: 815
		public class SubscribeFolderAsyncOP : IMAP_Client.CmdAsyncOP<IMAP_Client.SubscribeFolderAsyncOP>
		{
			// Token: 0x06001A8C RID: 6796 RVA: 0x000A4330 File Offset: 0x000A3330
			public SubscribeFolderAsyncOP(string folder, EventHandler<EventArgs<IMAP_r_u>> callback) : base(callback)
			{
				bool flag = folder == null;
				if (flag)
				{
					throw new ArgumentNullException("folder");
				}
				bool flag2 = string.IsNullOrEmpty(folder);
				if (flag2)
				{
					throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
				}
				this.m_Folder = folder;
			}

			// Token: 0x06001A8D RID: 6797 RVA: 0x000A4384 File Offset: 0x000A3384
			protected override void OnInitCmdLine(IMAP_Client imap)
			{
				Encoding utf = Encoding.UTF8;
				int commandIndex = imap.m_CommandIndex;
				imap.m_CommandIndex = commandIndex + 1;
				byte[] bytes = utf.GetBytes(commandIndex.ToString("d5") + " SUBSCRIBE " + IMAP_Utils.EncodeMailbox(this.m_Folder, imap.m_MailboxEncoding) + "\r\n");
				base.CmdLines.Add(new IMAP_Client.CmdLine(bytes, Encoding.UTF8.GetString(bytes).TrimEnd(new char[0])));
			}

			// Token: 0x04000C24 RID: 3108
			private string m_Folder = null;
		}

		// Token: 0x02000330 RID: 816
		public class UnsubscribeFolderAsyncOP : IMAP_Client.CmdAsyncOP<IMAP_Client.UnsubscribeFolderAsyncOP>
		{
			// Token: 0x06001A8E RID: 6798 RVA: 0x000A4404 File Offset: 0x000A3404
			public UnsubscribeFolderAsyncOP(string folder, EventHandler<EventArgs<IMAP_r_u>> callback) : base(callback)
			{
				bool flag = folder == null;
				if (flag)
				{
					throw new ArgumentNullException("folder");
				}
				bool flag2 = string.IsNullOrEmpty(folder);
				if (flag2)
				{
					throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
				}
				this.m_Folder = folder;
			}

			// Token: 0x06001A8F RID: 6799 RVA: 0x000A4458 File Offset: 0x000A3458
			protected override void OnInitCmdLine(IMAP_Client imap)
			{
				Encoding utf = Encoding.UTF8;
				int commandIndex = imap.m_CommandIndex;
				imap.m_CommandIndex = commandIndex + 1;
				byte[] bytes = utf.GetBytes(commandIndex.ToString("d5") + " UNSUBSCRIBE " + IMAP_Utils.EncodeMailbox(this.m_Folder, imap.m_MailboxEncoding) + "\r\n");
				base.CmdLines.Add(new IMAP_Client.CmdLine(bytes, Encoding.UTF8.GetString(bytes).TrimEnd(new char[0])));
			}

			// Token: 0x04000C25 RID: 3109
			private string m_Folder = null;
		}

		// Token: 0x02000331 RID: 817
		public class FolderStatusAsyncOP : IMAP_Client.CmdAsyncOP<IMAP_Client.FolderStatusAsyncOP>
		{
			// Token: 0x06001A90 RID: 6800 RVA: 0x000A44D8 File Offset: 0x000A34D8
			public FolderStatusAsyncOP(string folder, EventHandler<EventArgs<IMAP_r_u>> callback) : base(callback)
			{
				bool flag = folder == null;
				if (flag)
				{
					throw new ArgumentNullException("folder");
				}
				bool flag2 = string.IsNullOrEmpty(folder);
				if (flag2)
				{
					throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
				}
				this.m_Folder = folder;
			}

			// Token: 0x06001A91 RID: 6801 RVA: 0x000A452C File Offset: 0x000A352C
			protected override void OnInitCmdLine(IMAP_Client imap)
			{
				Encoding utf = Encoding.UTF8;
				int commandIndex = imap.m_CommandIndex;
				imap.m_CommandIndex = commandIndex + 1;
				byte[] bytes = utf.GetBytes(commandIndex.ToString("d5") + " STATUS " + IMAP_Utils.EncodeMailbox(this.m_Folder, imap.m_MailboxEncoding) + " (MESSAGES RECENT UIDNEXT UIDVALIDITY UNSEEN)\r\n");
				base.CmdLines.Add(new IMAP_Client.CmdLine(bytes, Encoding.UTF8.GetString(bytes).TrimEnd(new char[0])));
			}

			// Token: 0x04000C26 RID: 3110
			private string m_Folder = null;
		}

		// Token: 0x02000332 RID: 818
		public class SelectFolderAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x06001A92 RID: 6802 RVA: 0x000A45AC File Offset: 0x000A35AC
			public SelectFolderAsyncOP(string folder, EventHandler<EventArgs<IMAP_r_u>> callback)
			{
				bool flag = folder == null;
				if (flag)
				{
					throw new ArgumentNullException("folder");
				}
				bool flag2 = string.IsNullOrEmpty(folder);
				if (flag2)
				{
					throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
				}
				this.m_Folder = folder;
				this.m_pCallback = callback;
			}

			// Token: 0x06001A93 RID: 6803 RVA: 0x000A4644 File Offset: 0x000A3644
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pImapClient = null;
					this.m_pFinalResponse = null;
					this.m_pCallback = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x06001A94 RID: 6804 RVA: 0x000A4690 File Offset: 0x000A3690
			internal bool Start(IMAP_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pImapClient = owner;
				this.SetState(AsyncOP_State.Active);
				try
				{
					this.m_pImapClient.m_pSelectedFolder = new IMAP_Client_SelectedFolder(this.m_Folder);
					Encoding utf = Encoding.UTF8;
					IMAP_Client pImapClient = this.m_pImapClient;
					int commandIndex = pImapClient.m_CommandIndex;
					pImapClient.m_CommandIndex = commandIndex + 1;
					byte[] bytes = utf.GetBytes(commandIndex.ToString("d5") + " SELECT " + IMAP_Utils.EncodeMailbox(this.m_Folder, this.m_pImapClient.m_MailboxEncoding) + "\r\n");
					string cmdLineLogText = Encoding.UTF8.GetString(bytes).TrimEnd(new char[0]);
					IMAP_Client.SendCmdAndReadRespAsyncOP sendCmdAndReadRespAsyncOP = new IMAP_Client.SendCmdAndReadRespAsyncOP(bytes, cmdLineLogText, this.m_pCallback);
					sendCmdAndReadRespAsyncOP.CompletedAsync += delegate(object sender, EventArgs<IMAP_Client.SendCmdAndReadRespAsyncOP> e)
					{
						this.ProecessCmdResult(e.Value);
					};
					bool flag2 = !this.m_pImapClient.SendCmdAndReadRespAsync(sendCmdAndReadRespAsyncOP);
					if (flag2)
					{
						this.ProecessCmdResult(sendCmdAndReadRespAsyncOP);
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.m_pImapClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					this.SetState(AsyncOP_State.Completed);
				}
				object pLock = this.m_pLock;
				bool result;
				lock (pLock)
				{
					this.m_RiseCompleted = true;
					result = (this.m_State == AsyncOP_State.Active);
				}
				return result;
			}

			// Token: 0x06001A95 RID: 6805 RVA: 0x000A4818 File Offset: 0x000A3818
			private void SetState(AsyncOP_State state)
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					object pLock = this.m_pLock;
					lock (pLock)
					{
						this.m_State = state;
						bool flag3 = this.m_State == AsyncOP_State.Completed && this.m_RiseCompleted;
						if (flag3)
						{
							this.OnCompletedAsync();
						}
					}
				}
			}

			// Token: 0x06001A96 RID: 6806 RVA: 0x000A4890 File Offset: 0x000A3890
			private void ProecessCmdResult(IMAP_Client.SendCmdAndReadRespAsyncOP op)
			{
				try
				{
					bool flag = op.Error != null;
					if (flag)
					{
						this.m_pException = op.Error;
						this.m_pImapClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					}
					else
					{
						this.m_pFinalResponse = op.FinalResponse;
						bool isError = op.FinalResponse.IsError;
						if (isError)
						{
							this.m_pException = new IMAP_ClientException(op.FinalResponse);
							this.m_pImapClient.m_pSelectedFolder = null;
						}
						else
						{
							bool flag2 = this.m_pFinalResponse.OptionalResponse != null && this.m_pFinalResponse.OptionalResponse is IMAP_t_orc_ReadOnly;
							if (flag2)
							{
								this.m_pImapClient.m_pSelectedFolder.SetReadOnly(true);
							}
						}
					}
					this.SetState(AsyncOP_State.Completed);
				}
				finally
				{
					op.Dispose();
				}
			}

			// Token: 0x17000872 RID: 2162
			// (get) Token: 0x06001A97 RID: 6807 RVA: 0x000A4984 File Offset: 0x000A3984
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x17000873 RID: 2163
			// (get) Token: 0x06001A98 RID: 6808 RVA: 0x000A499C File Offset: 0x000A399C
			public Exception Error
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Error' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					return this.m_pException;
				}
			}

			// Token: 0x17000874 RID: 2164
			// (get) Token: 0x06001A99 RID: 6809 RVA: 0x000A49F0 File Offset: 0x000A39F0
			public IMAP_r_ServerStatus FinalResponse
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Response' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					return this.m_pFinalResponse;
				}
			}

			// Token: 0x140000C5 RID: 197
			// (add) Token: 0x06001A9A RID: 6810 RVA: 0x000A4A44 File Offset: 0x000A3A44
			// (remove) Token: 0x06001A9B RID: 6811 RVA: 0x000A4A7C File Offset: 0x000A3A7C
			
			public event EventHandler<EventArgs<IMAP_Client.SelectFolderAsyncOP>> CompletedAsync = null;

			// Token: 0x06001A9C RID: 6812 RVA: 0x000A4AB4 File Offset: 0x000A3AB4
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<IMAP_Client.SelectFolderAsyncOP>(this));
				}
			}

			// Token: 0x04000C27 RID: 3111
			private object m_pLock = new object();

			// Token: 0x04000C28 RID: 3112
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000C29 RID: 3113
			private Exception m_pException = null;

			// Token: 0x04000C2A RID: 3114
			private IMAP_r_ServerStatus m_pFinalResponse = null;

			// Token: 0x04000C2B RID: 3115
			private IMAP_Client m_pImapClient = null;

			// Token: 0x04000C2C RID: 3116
			private bool m_RiseCompleted = false;

			// Token: 0x04000C2D RID: 3117
			private string m_Folder = null;

			// Token: 0x04000C2E RID: 3118
			private EventHandler<EventArgs<IMAP_r_u>> m_pCallback = null;
		}

		// Token: 0x02000333 RID: 819
		public class ExamineFolderAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x06001A9E RID: 6814 RVA: 0x000A4AF4 File Offset: 0x000A3AF4
			public ExamineFolderAsyncOP(string folder, EventHandler<EventArgs<IMAP_r_u>> callback)
			{
				bool flag = folder == null;
				if (flag)
				{
					throw new ArgumentNullException("folder");
				}
				bool flag2 = string.IsNullOrEmpty(folder);
				if (flag2)
				{
					throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
				}
				this.m_Folder = folder;
				this.m_pCallback = callback;
			}

			// Token: 0x06001A9F RID: 6815 RVA: 0x000A4B8C File Offset: 0x000A3B8C
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pImapClient = null;
					this.m_pFinalResponse = null;
					this.m_pCallback = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x06001AA0 RID: 6816 RVA: 0x000A4BD8 File Offset: 0x000A3BD8
			internal bool Start(IMAP_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pImapClient = owner;
				this.SetState(AsyncOP_State.Active);
				try
				{
					this.m_pImapClient.m_pSelectedFolder = new IMAP_Client_SelectedFolder(this.m_Folder);
					Encoding utf = Encoding.UTF8;
					IMAP_Client pImapClient = this.m_pImapClient;
					int commandIndex = pImapClient.m_CommandIndex;
					pImapClient.m_CommandIndex = commandIndex + 1;
					byte[] bytes = utf.GetBytes(commandIndex.ToString("d5") + " EXAMINE " + IMAP_Utils.EncodeMailbox(this.m_Folder, this.m_pImapClient.m_MailboxEncoding) + "\r\n");
					string cmdLineLogText = Encoding.UTF8.GetString(bytes).TrimEnd(new char[0]);
					IMAP_Client.SendCmdAndReadRespAsyncOP sendCmdAndReadRespAsyncOP = new IMAP_Client.SendCmdAndReadRespAsyncOP(bytes, cmdLineLogText, this.m_pCallback);
					sendCmdAndReadRespAsyncOP.CompletedAsync += delegate(object sender, EventArgs<IMAP_Client.SendCmdAndReadRespAsyncOP> e)
					{
						this.ProecessCmdResult(e.Value);
					};
					bool flag2 = !this.m_pImapClient.SendCmdAndReadRespAsync(sendCmdAndReadRespAsyncOP);
					if (flag2)
					{
						this.ProecessCmdResult(sendCmdAndReadRespAsyncOP);
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.m_pImapClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					this.SetState(AsyncOP_State.Completed);
				}
				object pLock = this.m_pLock;
				bool result;
				lock (pLock)
				{
					this.m_RiseCompleted = true;
					result = (this.m_State == AsyncOP_State.Active);
				}
				return result;
			}

			// Token: 0x06001AA1 RID: 6817 RVA: 0x000A4D60 File Offset: 0x000A3D60
			private void SetState(AsyncOP_State state)
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					object pLock = this.m_pLock;
					lock (pLock)
					{
						this.m_State = state;
						bool flag3 = this.m_State == AsyncOP_State.Completed && this.m_RiseCompleted;
						if (flag3)
						{
							this.OnCompletedAsync();
						}
					}
				}
			}

			// Token: 0x06001AA2 RID: 6818 RVA: 0x000A4DD8 File Offset: 0x000A3DD8
			private void ProecessCmdResult(IMAP_Client.SendCmdAndReadRespAsyncOP op)
			{
				try
				{
					bool flag = op.Error != null;
					if (flag)
					{
						this.m_pException = op.Error;
						this.m_pImapClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					}
					else
					{
						this.m_pFinalResponse = op.FinalResponse;
						bool isError = op.FinalResponse.IsError;
						if (isError)
						{
							this.m_pException = new IMAP_ClientException(op.FinalResponse);
							this.m_pImapClient.m_pSelectedFolder = null;
						}
						else
						{
							bool flag2 = this.m_pFinalResponse.OptionalResponse != null && this.m_pFinalResponse.OptionalResponse is IMAP_t_orc_ReadOnly;
							if (flag2)
							{
								this.m_pImapClient.m_pSelectedFolder.SetReadOnly(true);
							}
						}
					}
					this.SetState(AsyncOP_State.Completed);
				}
				finally
				{
					op.Dispose();
				}
			}

			// Token: 0x17000875 RID: 2165
			// (get) Token: 0x06001AA3 RID: 6819 RVA: 0x000A4ECC File Offset: 0x000A3ECC
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x17000876 RID: 2166
			// (get) Token: 0x06001AA4 RID: 6820 RVA: 0x000A4EE4 File Offset: 0x000A3EE4
			public Exception Error
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Error' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					return this.m_pException;
				}
			}

			// Token: 0x17000877 RID: 2167
			// (get) Token: 0x06001AA5 RID: 6821 RVA: 0x000A4F38 File Offset: 0x000A3F38
			public IMAP_r_ServerStatus FinalResponse
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Response' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					return this.m_pFinalResponse;
				}
			}

			// Token: 0x140000C6 RID: 198
			// (add) Token: 0x06001AA6 RID: 6822 RVA: 0x000A4F8C File Offset: 0x000A3F8C
			// (remove) Token: 0x06001AA7 RID: 6823 RVA: 0x000A4FC4 File Offset: 0x000A3FC4
			
			public event EventHandler<EventArgs<IMAP_Client.ExamineFolderAsyncOP>> CompletedAsync = null;

			// Token: 0x06001AA8 RID: 6824 RVA: 0x000A4FFC File Offset: 0x000A3FFC
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<IMAP_Client.ExamineFolderAsyncOP>(this));
				}
			}

			// Token: 0x04000C30 RID: 3120
			private object m_pLock = new object();

			// Token: 0x04000C31 RID: 3121
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000C32 RID: 3122
			private Exception m_pException = null;

			// Token: 0x04000C33 RID: 3123
			private IMAP_r_ServerStatus m_pFinalResponse = null;

			// Token: 0x04000C34 RID: 3124
			private IMAP_Client m_pImapClient = null;

			// Token: 0x04000C35 RID: 3125
			private bool m_RiseCompleted = false;

			// Token: 0x04000C36 RID: 3126
			private string m_Folder = null;

			// Token: 0x04000C37 RID: 3127
			private EventHandler<EventArgs<IMAP_r_u>> m_pCallback = null;
		}

		// Token: 0x02000334 RID: 820
		public class GetFolderQuotaRootsAsyncOP : IMAP_Client.CmdAsyncOP<IMAP_Client.GetFolderQuotaRootsAsyncOP>
		{
			// Token: 0x06001AAA RID: 6826 RVA: 0x000A503C File Offset: 0x000A403C
			public GetFolderQuotaRootsAsyncOP(string folder, EventHandler<EventArgs<IMAP_r_u>> callback) : base(callback)
			{
				bool flag = folder == null;
				if (flag)
				{
					throw new ArgumentNullException("folder");
				}
				bool flag2 = string.IsNullOrEmpty(folder);
				if (flag2)
				{
					throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
				}
				this.m_Folder = folder;
			}

			// Token: 0x06001AAB RID: 6827 RVA: 0x000A5090 File Offset: 0x000A4090
			protected override void OnInitCmdLine(IMAP_Client imap)
			{
				Encoding utf = Encoding.UTF8;
				int commandIndex = imap.m_CommandIndex;
				imap.m_CommandIndex = commandIndex + 1;
				byte[] bytes = utf.GetBytes(commandIndex.ToString("d5") + " GETQUOTAROOT " + IMAP_Utils.EncodeMailbox(this.m_Folder, imap.m_MailboxEncoding) + "\r\n");
				base.CmdLines.Add(new IMAP_Client.CmdLine(bytes, Encoding.UTF8.GetString(bytes).TrimEnd(new char[0])));
			}

			// Token: 0x04000C39 RID: 3129
			private string m_Folder = null;
		}

		// Token: 0x02000335 RID: 821
		public class GetQuotaAsyncOP : IMAP_Client.CmdAsyncOP<IMAP_Client.GetQuotaAsyncOP>
		{
			// Token: 0x06001AAC RID: 6828 RVA: 0x000A5110 File Offset: 0x000A4110
			public GetQuotaAsyncOP(string quotaRootName, EventHandler<EventArgs<IMAP_r_u>> callback) : base(callback)
			{
				bool flag = quotaRootName == null;
				if (flag)
				{
					throw new ArgumentNullException("quotaRootName");
				}
				this.m_QuotaRootName = quotaRootName;
			}

			// Token: 0x06001AAD RID: 6829 RVA: 0x000A5148 File Offset: 0x000A4148
			protected override void OnInitCmdLine(IMAP_Client imap)
			{
				Encoding utf = Encoding.UTF8;
				int commandIndex = imap.m_CommandIndex;
				imap.m_CommandIndex = commandIndex + 1;
				byte[] bytes = utf.GetBytes(commandIndex.ToString("d5") + " GETQUOTA " + IMAP_Utils.EncodeMailbox(this.m_QuotaRootName, imap.m_MailboxEncoding) + "\r\n");
				base.CmdLines.Add(new IMAP_Client.CmdLine(bytes, Encoding.UTF8.GetString(bytes).TrimEnd(new char[0])));
			}

			// Token: 0x04000C3A RID: 3130
			private string m_QuotaRootName = null;
		}

		// Token: 0x02000336 RID: 822
		public class GetFolderAclAsyncOP : IMAP_Client.CmdAsyncOP<IMAP_Client.GetFolderAclAsyncOP>
		{
			// Token: 0x06001AAE RID: 6830 RVA: 0x000A51C8 File Offset: 0x000A41C8
			public GetFolderAclAsyncOP(string folder, EventHandler<EventArgs<IMAP_r_u>> callback) : base(callback)
			{
				bool flag = folder == null;
				if (flag)
				{
					throw new ArgumentNullException("folder");
				}
				this.m_Folder = folder;
			}

			// Token: 0x06001AAF RID: 6831 RVA: 0x000A5200 File Offset: 0x000A4200
			protected override void OnInitCmdLine(IMAP_Client imap)
			{
				Encoding utf = Encoding.UTF8;
				int commandIndex = imap.m_CommandIndex;
				imap.m_CommandIndex = commandIndex + 1;
				byte[] bytes = utf.GetBytes(commandIndex.ToString("d5") + " GETACL " + IMAP_Utils.EncodeMailbox(this.m_Folder, imap.m_MailboxEncoding) + "\r\n");
				base.CmdLines.Add(new IMAP_Client.CmdLine(bytes, Encoding.UTF8.GetString(bytes).TrimEnd(new char[0])));
			}

			// Token: 0x04000C3B RID: 3131
			private string m_Folder = null;
		}

		// Token: 0x02000337 RID: 823
		public class SetFolderAclAsyncOP : IMAP_Client.CmdAsyncOP<IMAP_Client.SetFolderAclAsyncOP>
		{
			// Token: 0x06001AB0 RID: 6832 RVA: 0x000A5280 File Offset: 0x000A4280
			public SetFolderAclAsyncOP(string folder, string identifier, IMAP_Flags_SetType setType, IMAP_ACL_Flags permissions, EventHandler<EventArgs<IMAP_r_u>> callback) : base(callback)
			{
				bool flag = folder == null;
				if (flag)
				{
					throw new ArgumentNullException("folder");
				}
				bool flag2 = string.IsNullOrEmpty(folder);
				if (flag2)
				{
					throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
				}
				bool flag3 = identifier == null;
				if (flag3)
				{
					throw new ArgumentNullException("identifier");
				}
				bool flag4 = string.IsNullOrEmpty(identifier);
				if (flag4)
				{
					throw new ArgumentException("Argument 'identifier' value must be specified.", "identifier");
				}
				this.m_Folder = folder;
				this.m_Identifier = identifier;
				this.m_FlagsSetType = setType;
				this.m_Permissions = permissions;
			}

			// Token: 0x06001AB1 RID: 6833 RVA: 0x000A5330 File Offset: 0x000A4330
			protected override void OnInitCmdLine(IMAP_Client imap)
			{
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = stringBuilder;
				int commandIndex = imap.m_CommandIndex;
				imap.m_CommandIndex = commandIndex + 1;
				stringBuilder2.Append(commandIndex.ToString("d5"));
				stringBuilder.Append(" SETACL");
				stringBuilder.Append(" " + IMAP_Utils.EncodeMailbox(this.m_Folder, imap.m_MailboxEncoding));
				stringBuilder.Append(" " + TextUtils.QuoteString(this.m_Identifier));
				bool flag = this.m_FlagsSetType == IMAP_Flags_SetType.Add;
				if (flag)
				{
					stringBuilder.Append(" +" + IMAP_Utils.ACL_to_String(this.m_Permissions));
				}
				else
				{
					bool flag2 = this.m_FlagsSetType == IMAP_Flags_SetType.Remove;
					if (flag2)
					{
						stringBuilder.Append(" -" + IMAP_Utils.ACL_to_String(this.m_Permissions));
					}
					else
					{
						bool flag3 = this.m_FlagsSetType == IMAP_Flags_SetType.Replace;
						if (!flag3)
						{
							throw new NotSupportedException("Not supported argument 'setType' value '" + this.m_FlagsSetType.ToString() + "'.");
						}
						stringBuilder.Append(" " + IMAP_Utils.ACL_to_String(this.m_Permissions));
					}
				}
				byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
				base.CmdLines.Add(new IMAP_Client.CmdLine(bytes, Encoding.UTF8.GetString(bytes).TrimEnd(new char[0])));
			}

			// Token: 0x04000C3C RID: 3132
			private string m_Folder = null;

			// Token: 0x04000C3D RID: 3133
			private string m_Identifier = null;

			// Token: 0x04000C3E RID: 3134
			private IMAP_Flags_SetType m_FlagsSetType = IMAP_Flags_SetType.Replace;

			// Token: 0x04000C3F RID: 3135
			private IMAP_ACL_Flags m_Permissions = IMAP_ACL_Flags.None;
		}

		// Token: 0x02000338 RID: 824
		public class DeleteFolderAclAsyncOP : IMAP_Client.CmdAsyncOP<IMAP_Client.DeleteFolderAclAsyncOP>
		{
			// Token: 0x06001AB2 RID: 6834 RVA: 0x000A54A0 File Offset: 0x000A44A0
			public DeleteFolderAclAsyncOP(string folder, string identifier, EventHandler<EventArgs<IMAP_r_u>> callback) : base(callback)
			{
				bool flag = folder == null;
				if (flag)
				{
					throw new ArgumentNullException("folder");
				}
				bool flag2 = string.IsNullOrEmpty(folder);
				if (flag2)
				{
					throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
				}
				bool flag3 = identifier == null;
				if (flag3)
				{
					throw new ArgumentNullException("identifier");
				}
				this.m_Folder = folder;
				this.m_Identifier = identifier;
			}

			// Token: 0x06001AB3 RID: 6835 RVA: 0x000A5518 File Offset: 0x000A4518
			protected override void OnInitCmdLine(IMAP_Client imap)
			{
				Encoding utf = Encoding.UTF8;
				string[] array = new string[6];
				int num = 0;
				int commandIndex = imap.m_CommandIndex;
				imap.m_CommandIndex = commandIndex + 1;
				array[num] = commandIndex.ToString("d5");
				array[1] = " DELETEACL ";
				array[2] = IMAP_Utils.EncodeMailbox(this.m_Folder, imap.m_MailboxEncoding);
				array[3] = " ";
				array[4] = TextUtils.QuoteString(this.m_Identifier);
				array[5] = "\r\n";
				byte[] bytes = utf.GetBytes(string.Concat(array));
				base.CmdLines.Add(new IMAP_Client.CmdLine(bytes, Encoding.UTF8.GetString(bytes).TrimEnd(new char[0])));
			}

			// Token: 0x04000C40 RID: 3136
			private string m_Folder = null;

			// Token: 0x04000C41 RID: 3137
			private string m_Identifier = null;
		}

		// Token: 0x02000339 RID: 825
		public class GetFolderRightsAsyncOP : IMAP_Client.CmdAsyncOP<IMAP_Client.GetFolderRightsAsyncOP>
		{
			// Token: 0x06001AB4 RID: 6836 RVA: 0x000A55C0 File Offset: 0x000A45C0
			public GetFolderRightsAsyncOP(string folder, string identifier, EventHandler<EventArgs<IMAP_r_u>> callback) : base(callback)
			{
				bool flag = folder == null;
				if (flag)
				{
					throw new ArgumentNullException("folder");
				}
				bool flag2 = string.IsNullOrEmpty(folder);
				if (flag2)
				{
					throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
				}
				bool flag3 = identifier == null;
				if (flag3)
				{
					throw new ArgumentNullException("identifier");
				}
				this.m_Folder = folder;
				this.m_Identifier = identifier;
			}

			// Token: 0x06001AB5 RID: 6837 RVA: 0x000A5638 File Offset: 0x000A4638
			protected override void OnInitCmdLine(IMAP_Client imap)
			{
				Encoding utf = Encoding.UTF8;
				string[] array = new string[6];
				int num = 0;
				int commandIndex = imap.m_CommandIndex;
				imap.m_CommandIndex = commandIndex + 1;
				array[num] = commandIndex.ToString("d5");
				array[1] = " LISTRIGHTS ";
				array[2] = IMAP_Utils.EncodeMailbox(this.m_Folder, imap.m_MailboxEncoding);
				array[3] = " ";
				array[4] = TextUtils.QuoteString(this.m_Identifier);
				array[5] = "\r\n";
				byte[] bytes = utf.GetBytes(string.Concat(array));
				base.CmdLines.Add(new IMAP_Client.CmdLine(bytes, Encoding.UTF8.GetString(bytes).TrimEnd(new char[0])));
			}

			// Token: 0x04000C42 RID: 3138
			private string m_Folder = null;

			// Token: 0x04000C43 RID: 3139
			private string m_Identifier = null;
		}

		// Token: 0x0200033A RID: 826
		public class GetFolderMyRightsAsyncOP : IMAP_Client.CmdAsyncOP<IMAP_Client.GetFolderMyRightsAsyncOP>
		{
			// Token: 0x06001AB6 RID: 6838 RVA: 0x000A56E0 File Offset: 0x000A46E0
			public GetFolderMyRightsAsyncOP(string folder, EventHandler<EventArgs<IMAP_r_u>> callback) : base(callback)
			{
				bool flag = folder == null;
				if (flag)
				{
					throw new ArgumentNullException("folder");
				}
				this.m_Folder = folder;
			}

			// Token: 0x06001AB7 RID: 6839 RVA: 0x000A5718 File Offset: 0x000A4718
			protected override void OnInitCmdLine(IMAP_Client imap)
			{
				Encoding utf = Encoding.UTF8;
				int commandIndex = imap.m_CommandIndex;
				imap.m_CommandIndex = commandIndex + 1;
				byte[] bytes = utf.GetBytes(commandIndex.ToString("d5") + " MYRIGHTS " + IMAP_Utils.EncodeMailbox(this.m_Folder, imap.m_MailboxEncoding) + "\r\n");
				base.CmdLines.Add(new IMAP_Client.CmdLine(bytes, Encoding.UTF8.GetString(bytes).TrimEnd(new char[0])));
			}

			// Token: 0x04000C44 RID: 3140
			private string m_Folder = null;
		}

		// Token: 0x0200033B RID: 827
		public class StoreMessageAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x06001AB8 RID: 6840 RVA: 0x000A5798 File Offset: 0x000A4798
			public StoreMessageAsyncOP(string folder, IMAP_t_MsgFlags flags, DateTime internalDate, Stream message, long count, EventHandler<EventArgs<IMAP_r_u>> callback)
			{
				bool flag = folder == null;
				if (flag)
				{
					throw new ArgumentNullException("folder");
				}
				bool flag2 = string.IsNullOrEmpty(folder);
				if (flag2)
				{
					throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
				}
				bool flag3 = message == null;
				if (flag3)
				{
					throw new ArgumentNullException("message");
				}
				bool flag4 = count < 1L;
				if (flag4)
				{
					throw new ArgumentException("Argument 'count' value must be >= 1.", "count");
				}
				this.m_Folder = folder;
				this.m_pFlags = flags;
				this.m_InternalDate = internalDate;
				this.m_pStream = message;
				this.m_Count = count;
				this.m_pCallback = callback;
			}

			// Token: 0x06001AB9 RID: 6841 RVA: 0x000A5894 File Offset: 0x000A4894
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pImapClient = null;
					this.m_pFinalResponse = null;
					this.m_pCallback = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x06001ABA RID: 6842 RVA: 0x000A58E0 File Offset: 0x000A48E0
			internal bool Start(IMAP_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pImapClient = owner;
				this.SetState(AsyncOP_State.Active);
				try
				{
					StringBuilder stringBuilder = new StringBuilder();
					StringBuilder stringBuilder2 = stringBuilder;
					IMAP_Client pImapClient = this.m_pImapClient;
					int commandIndex = pImapClient.m_CommandIndex;
					pImapClient.m_CommandIndex = commandIndex + 1;
					stringBuilder2.Append(commandIndex.ToString("d5"));
					stringBuilder.Append(" APPEND");
					stringBuilder.Append(" " + IMAP_Utils.EncodeMailbox(this.m_Folder, this.m_pImapClient.m_MailboxEncoding));
					bool flag2 = this.m_pFlags != null;
					if (flag2)
					{
						stringBuilder.Append(" (");
						string[] array = this.m_pFlags.ToArray();
						for (int i = 0; i < array.Length; i++)
						{
							bool flag3 = i > 0;
							if (flag3)
							{
								stringBuilder.Append(" ");
							}
							stringBuilder.Append(array[i]);
						}
						stringBuilder.Append(")");
					}
					bool flag4 = this.m_InternalDate != DateTime.MinValue;
					if (flag4)
					{
						stringBuilder.Append(" " + TextUtils.QuoteString(IMAP_Utils.DateTimeToString(this.m_InternalDate)));
					}
					stringBuilder.Append(" {" + this.m_Count + "}\r\n");
					byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
					string cmdLineLogText = Encoding.UTF8.GetString(bytes).TrimEnd(new char[0]);
					IMAP_Client.SendCmdAndReadRespAsyncOP args = new IMAP_Client.SendCmdAndReadRespAsyncOP(bytes, cmdLineLogText, this.m_pCallback);
					args.CompletedAsync += delegate(object sender, EventArgs<IMAP_Client.SendCmdAndReadRespAsyncOP> e)
					{
						this.ProcessCmdSendingResult(args);
					};
					bool flag5 = !this.m_pImapClient.SendCmdAndReadRespAsync(args);
					if (flag5)
					{
						this.ProcessCmdSendingResult(args);
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.m_pImapClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					this.SetState(AsyncOP_State.Completed);
				}
				object pLock = this.m_pLock;
				bool result;
				lock (pLock)
				{
					this.m_RiseCompleted = true;
					result = (this.m_State == AsyncOP_State.Active);
				}
				return result;
			}

			// Token: 0x06001ABB RID: 6843 RVA: 0x000A5B80 File Offset: 0x000A4B80
			private void SetState(AsyncOP_State state)
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					object pLock = this.m_pLock;
					lock (pLock)
					{
						this.m_State = state;
						bool flag3 = this.m_State == AsyncOP_State.Completed && this.m_RiseCompleted;
						if (flag3)
						{
							this.OnCompletedAsync();
						}
					}
				}
			}

			// Token: 0x06001ABC RID: 6844 RVA: 0x000A5BF8 File Offset: 0x000A4BF8
			private void ProcessCmdSendingResult(IMAP_Client.SendCmdAndReadRespAsyncOP op)
			{
				try
				{
					bool flag = op.Error != null;
					if (flag)
					{
						this.m_pException = op.Error;
					}
					else
					{
						bool isContinue = op.FinalResponse.IsContinue;
						if (isContinue)
						{
							SmartStream.WriteStreamAsyncOP writeOP = new SmartStream.WriteStreamAsyncOP(this.m_pStream, this.m_Count);
							writeOP.CompletedAsync += delegate(object sender, EventArgs<SmartStream.WriteStreamAsyncOP> e)
							{
								this.ProcessMsgSendingResult(writeOP);
							};
							bool flag2 = !this.m_pImapClient.TcpStream.WriteStreamAsync(writeOP);
							if (flag2)
							{
								this.ProcessMsgSendingResult(writeOP);
							}
						}
						else
						{
							this.m_pFinalResponse = op.FinalResponse;
							this.m_pException = new IMAP_ClientException(op.FinalResponse);
							this.SetState(AsyncOP_State.Completed);
						}
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.m_pImapClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					this.SetState(AsyncOP_State.Completed);
				}
				finally
				{
					op.Dispose();
				}
			}

			// Token: 0x06001ABD RID: 6845 RVA: 0x000A5D4C File Offset: 0x000A4D4C
			private void ProcessMsgSendingResult(SmartStream.WriteStreamAsyncOP writeOP)
			{
				try
				{
					bool flag = writeOP.Error != null;
					if (flag)
					{
						this.m_pException = writeOP.Error;
						this.m_pImapClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
						this.SetState(AsyncOP_State.Completed);
					}
					else
					{
						this.m_pImapClient.LogAddWrite(this.m_Count, "Wrote " + this.m_Count + " bytes.");
						IMAP_Client.SendCmdAndReadRespAsyncOP args = new IMAP_Client.SendCmdAndReadRespAsyncOP(new byte[]
						{
							13,
							10
						}, "", this.m_pCallback);
						args.CompletedAsync += delegate(object sender, EventArgs<IMAP_Client.SendCmdAndReadRespAsyncOP> e)
						{
							bool flag4 = args.Error != null;
							if (flag4)
							{
								this.m_pException = args.Error;
							}
							else
							{
								bool isError2 = args.FinalResponse.IsError;
								if (isError2)
								{
									this.m_pException = new IMAP_ClientException(args.FinalResponse);
								}
								this.m_pFinalResponse = args.FinalResponse;
							}
							this.SetState(AsyncOP_State.Completed);
						};
						bool flag2 = !this.m_pImapClient.SendCmdAndReadRespAsync(args);
						if (flag2)
						{
							bool flag3 = args.Error != null;
							if (flag3)
							{
								this.m_pException = args.Error;
							}
							else
							{
								bool isError = args.FinalResponse.IsError;
								if (isError)
								{
									this.m_pException = new IMAP_ClientException(args.FinalResponse);
								}
								this.m_pFinalResponse = args.FinalResponse;
							}
							this.SetState(AsyncOP_State.Completed);
						}
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.m_pImapClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					this.SetState(AsyncOP_State.Completed);
				}
				finally
				{
					writeOP.Dispose();
				}
			}

			// Token: 0x17000878 RID: 2168
			// (get) Token: 0x06001ABE RID: 6846 RVA: 0x000A5F30 File Offset: 0x000A4F30
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x17000879 RID: 2169
			// (get) Token: 0x06001ABF RID: 6847 RVA: 0x000A5F48 File Offset: 0x000A4F48
			public Exception Error
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Error' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					return this.m_pException;
				}
			}

			// Token: 0x1700087A RID: 2170
			// (get) Token: 0x06001AC0 RID: 6848 RVA: 0x000A5F9C File Offset: 0x000A4F9C
			public IMAP_r_ServerStatus FinalResponse
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Response' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					return this.m_pFinalResponse;
				}
			}

			// Token: 0x1700087B RID: 2171
			// (get) Token: 0x06001AC1 RID: 6849 RVA: 0x000A5FF0 File Offset: 0x000A4FF0
			public IMAP_t_orc_AppendUid AppendUid
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Response' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					bool flag3 = this.m_pFinalResponse != null && this.m_pFinalResponse.OptionalResponse != null && this.m_pFinalResponse.OptionalResponse is IMAP_t_orc_AppendUid;
					IMAP_t_orc_AppendUid result;
					if (flag3)
					{
						result = (IMAP_t_orc_AppendUid)this.m_pFinalResponse.OptionalResponse;
					}
					else
					{
						result = null;
					}
					return result;
				}
			}

			// Token: 0x140000C7 RID: 199
			// (add) Token: 0x06001AC2 RID: 6850 RVA: 0x000A6084 File Offset: 0x000A5084
			// (remove) Token: 0x06001AC3 RID: 6851 RVA: 0x000A60BC File Offset: 0x000A50BC
			
			public event EventHandler<EventArgs<IMAP_Client.StoreMessageAsyncOP>> CompletedAsync = null;

			// Token: 0x06001AC4 RID: 6852 RVA: 0x000A60F4 File Offset: 0x000A50F4
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<IMAP_Client.StoreMessageAsyncOP>(this));
				}
			}

			// Token: 0x04000C45 RID: 3141
			private object m_pLock = new object();

			// Token: 0x04000C46 RID: 3142
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000C47 RID: 3143
			private Exception m_pException = null;

			// Token: 0x04000C48 RID: 3144
			private IMAP_r_ServerStatus m_pFinalResponse = null;

			// Token: 0x04000C49 RID: 3145
			private IMAP_Client m_pImapClient = null;

			// Token: 0x04000C4A RID: 3146
			private bool m_RiseCompleted = false;

			// Token: 0x04000C4B RID: 3147
			private string m_Folder = null;

			// Token: 0x04000C4C RID: 3148
			private IMAP_t_MsgFlags m_pFlags = null;

			// Token: 0x04000C4D RID: 3149
			private DateTime m_InternalDate;

			// Token: 0x04000C4E RID: 3150
			private Stream m_pStream = null;

			// Token: 0x04000C4F RID: 3151
			private long m_Count = 0L;

			// Token: 0x04000C50 RID: 3152
			private EventHandler<EventArgs<IMAP_r_u>> m_pCallback = null;
		}

		// Token: 0x0200033C RID: 828
		public class EnableAsyncOP : IMAP_Client.CmdAsyncOP<IMAP_Client.EnableAsyncOP>
		{
			// Token: 0x06001AC5 RID: 6853 RVA: 0x000A6124 File Offset: 0x000A5124
			public EnableAsyncOP(string[] capabilities, EventHandler<EventArgs<IMAP_r_u>> callback) : base(callback)
			{
				bool flag = capabilities == null;
				if (flag)
				{
					throw new ArgumentNullException("capabilities");
				}
				bool flag2 = capabilities.Length < 1;
				if (flag2)
				{
					throw new ArgumentException("Argument 'capabilities' must contain at least 1 value.", "capabilities");
				}
				this.m_pCapabilities = capabilities;
			}

			// Token: 0x06001AC6 RID: 6854 RVA: 0x000A6178 File Offset: 0x000A5178
			protected override void OnInitCmdLine(IMAP_Client imap)
			{
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = stringBuilder;
				int commandIndex = imap.m_CommandIndex;
				imap.m_CommandIndex = commandIndex + 1;
				stringBuilder2.Append(commandIndex.ToString("d5") + " ENABLE");
				foreach (string str in this.m_pCapabilities)
				{
					stringBuilder.Append(" " + str);
				}
				stringBuilder.Append("\r\n");
				byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
				base.CmdLines.Add(new IMAP_Client.CmdLine(bytes, Encoding.UTF8.GetString(bytes).TrimEnd(new char[0])));
			}

			// Token: 0x04000C52 RID: 3154
			private string[] m_pCapabilities = null;
		}

		// Token: 0x0200033D RID: 829
		public class CloseFolderAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x06001AC7 RID: 6855 RVA: 0x000A6234 File Offset: 0x000A5234
			public CloseFolderAsyncOP(EventHandler<EventArgs<IMAP_r_u>> callback)
			{
				this.m_pCallback = callback;
			}

			// Token: 0x06001AC8 RID: 6856 RVA: 0x000A628C File Offset: 0x000A528C
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pImapClient = null;
					this.m_pFinalResponse = null;
					this.m_pCallback = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x06001AC9 RID: 6857 RVA: 0x000A62D8 File Offset: 0x000A52D8
			internal bool Start(IMAP_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pImapClient = owner;
				this.SetState(AsyncOP_State.Active);
				try
				{
					Encoding utf = Encoding.UTF8;
					IMAP_Client pImapClient = this.m_pImapClient;
					int commandIndex = pImapClient.m_CommandIndex;
					pImapClient.m_CommandIndex = commandIndex + 1;
					byte[] bytes = utf.GetBytes(commandIndex.ToString("d5") + " CLOSE\r\n");
					string cmdLineLogText = Encoding.UTF8.GetString(bytes).TrimEnd(new char[0]);
					IMAP_Client.SendCmdAndReadRespAsyncOP sendCmdAndReadRespAsyncOP = new IMAP_Client.SendCmdAndReadRespAsyncOP(bytes, cmdLineLogText, this.m_pCallback);
					sendCmdAndReadRespAsyncOP.CompletedAsync += delegate(object sender, EventArgs<IMAP_Client.SendCmdAndReadRespAsyncOP> e)
					{
						this.ProecessCmdResult(e.Value);
					};
					bool flag2 = !this.m_pImapClient.SendCmdAndReadRespAsync(sendCmdAndReadRespAsyncOP);
					if (flag2)
					{
						this.ProecessCmdResult(sendCmdAndReadRespAsyncOP);
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.m_pImapClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					this.SetState(AsyncOP_State.Completed);
				}
				object pLock = this.m_pLock;
				bool result;
				lock (pLock)
				{
					this.m_RiseCompleted = true;
					result = (this.m_State == AsyncOP_State.Active);
				}
				return result;
			}

			// Token: 0x06001ACA RID: 6858 RVA: 0x000A6430 File Offset: 0x000A5430
			private void SetState(AsyncOP_State state)
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					object pLock = this.m_pLock;
					lock (pLock)
					{
						this.m_State = state;
						bool flag3 = this.m_State == AsyncOP_State.Completed && this.m_RiseCompleted;
						if (flag3)
						{
							this.OnCompletedAsync();
						}
					}
				}
			}

			// Token: 0x06001ACB RID: 6859 RVA: 0x000A64A8 File Offset: 0x000A54A8
			private void ProecessCmdResult(IMAP_Client.SendCmdAndReadRespAsyncOP op)
			{
				try
				{
					bool flag = op.Error != null;
					if (flag)
					{
						this.m_pException = op.Error;
						this.m_pImapClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					}
					else
					{
						this.m_pFinalResponse = op.FinalResponse;
						bool isError = op.FinalResponse.IsError;
						if (isError)
						{
							this.m_pException = new IMAP_ClientException(op.FinalResponse);
						}
						else
						{
							this.m_pImapClient.m_pSelectedFolder = null;
						}
					}
					this.SetState(AsyncOP_State.Completed);
				}
				finally
				{
					op.Dispose();
				}
			}

			// Token: 0x1700087C RID: 2172
			// (get) Token: 0x06001ACC RID: 6860 RVA: 0x000A6560 File Offset: 0x000A5560
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x1700087D RID: 2173
			// (get) Token: 0x06001ACD RID: 6861 RVA: 0x000A6578 File Offset: 0x000A5578
			public Exception Error
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Error' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					return this.m_pException;
				}
			}

			// Token: 0x1700087E RID: 2174
			// (get) Token: 0x06001ACE RID: 6862 RVA: 0x000A65CC File Offset: 0x000A55CC
			public IMAP_r_ServerStatus FinalResponse
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Response' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					return this.m_pFinalResponse;
				}
			}

			// Token: 0x140000C8 RID: 200
			// (add) Token: 0x06001ACF RID: 6863 RVA: 0x000A6620 File Offset: 0x000A5620
			// (remove) Token: 0x06001AD0 RID: 6864 RVA: 0x000A6658 File Offset: 0x000A5658
			
			public event EventHandler<EventArgs<IMAP_Client.CloseFolderAsyncOP>> CompletedAsync = null;

			// Token: 0x06001AD1 RID: 6865 RVA: 0x000A6690 File Offset: 0x000A5690
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<IMAP_Client.CloseFolderAsyncOP>(this));
				}
			}

			// Token: 0x04000C53 RID: 3155
			private object m_pLock = new object();

			// Token: 0x04000C54 RID: 3156
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000C55 RID: 3157
			private Exception m_pException = null;

			// Token: 0x04000C56 RID: 3158
			private IMAP_r_ServerStatus m_pFinalResponse = null;

			// Token: 0x04000C57 RID: 3159
			private IMAP_Client m_pImapClient = null;

			// Token: 0x04000C58 RID: 3160
			private bool m_RiseCompleted = false;

			// Token: 0x04000C59 RID: 3161
			private EventHandler<EventArgs<IMAP_r_u>> m_pCallback = null;
		}

		// Token: 0x0200033E RID: 830
		public class FetchAsyncOP : IMAP_Client.CmdAsyncOP<IMAP_Client.FetchAsyncOP>
		{
			// Token: 0x06001AD3 RID: 6867 RVA: 0x000A66D0 File Offset: 0x000A56D0
			public FetchAsyncOP(bool uid, IMAP_t_SeqSet seqSet, IMAP_t_Fetch_i[] items, EventHandler<EventArgs<IMAP_r_u>> callback) : base(callback)
			{
				bool flag = seqSet == null;
				if (flag)
				{
					throw new ArgumentNullException("seqSet");
				}
				bool flag2 = items == null;
				if (flag2)
				{
					throw new ArgumentNullException("items");
				}
				bool flag3 = items.Length < 1;
				if (flag3)
				{
					throw new ArgumentException("Argument 'items' must conatain at least 1 value.", "items");
				}
				this.m_Uid = uid;
				this.m_pSeqSet = seqSet;
				this.m_pDataItems = items;
			}

			// Token: 0x06001AD4 RID: 6868 RVA: 0x000A6754 File Offset: 0x000A5754
			protected override void OnInitCmdLine(IMAP_Client imap)
			{
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = stringBuilder;
				int commandIndex = imap.m_CommandIndex;
				imap.m_CommandIndex = commandIndex + 1;
				stringBuilder2.Append(commandIndex.ToString("d5"));
				bool uid = this.m_Uid;
				if (uid)
				{
					stringBuilder.Append(" UID");
				}
				stringBuilder.Append(" FETCH " + this.m_pSeqSet.ToString() + " (");
				for (int i = 0; i < this.m_pDataItems.Length; i++)
				{
					bool flag = i > 0;
					if (flag)
					{
						stringBuilder.Append(" ");
					}
					stringBuilder.Append(this.m_pDataItems[i].ToString());
				}
				stringBuilder.Append(")\r\n");
				byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
				base.CmdLines.Add(new IMAP_Client.CmdLine(bytes, Encoding.UTF8.GetString(bytes).TrimEnd(new char[0])));
			}

			// Token: 0x04000C5B RID: 3163
			private bool m_Uid = false;

			// Token: 0x04000C5C RID: 3164
			private IMAP_t_SeqSet m_pSeqSet = null;

			// Token: 0x04000C5D RID: 3165
			private IMAP_t_Fetch_i[] m_pDataItems = null;
		}

		// Token: 0x0200033F RID: 831
		public class SearchAsyncOP : IMAP_Client.CmdAsyncOP<IMAP_Client.SearchAsyncOP>
		{
			// Token: 0x06001AD5 RID: 6869 RVA: 0x000A6858 File Offset: 0x000A5858
			public SearchAsyncOP(bool uid, Encoding charset, IMAP_Search_Key criteria, EventHandler<EventArgs<IMAP_r_u>> callback) : base(callback)
			{
				bool flag = criteria == null;
				if (flag)
				{
					throw new ArgumentNullException("criteria");
				}
				this.m_Uid = uid;
				this.m_pCharset = charset;
				this.m_pCriteria = criteria;
			}

			// Token: 0x06001AD6 RID: 6870 RVA: 0x000A68B0 File Offset: 0x000A58B0
			protected override void OnInitCmdLine(IMAP_Client imap)
			{
				ByteBuilder byteBuilder = new ByteBuilder();
				List<ByteBuilder> list = new List<ByteBuilder>();
				list.Add(byteBuilder);
				ByteBuilder byteBuilder2 = byteBuilder;
				int commandIndex = imap.m_CommandIndex;
				imap.m_CommandIndex = commandIndex + 1;
				byteBuilder2.Append(commandIndex.ToString("d5"));
				bool uid = this.m_Uid;
				if (uid)
				{
					byteBuilder.Append(" UID");
				}
				byteBuilder.Append(" SEARCH");
				bool flag = this.m_pCharset != null;
				if (flag)
				{
					byteBuilder.Append(" CHARSET " + this.m_pCharset.WebName.ToUpper());
				}
				byteBuilder.Append(" ");
				List<IMAP_Client_CmdPart> list2 = new List<IMAP_Client_CmdPart>();
				this.m_pCriteria.ToCmdParts(list2);
				foreach (IMAP_Client_CmdPart imap_Client_CmdPart in list2)
				{
					bool flag2 = imap_Client_CmdPart.Type == IMAP_Client_CmdPart_Type.Constant;
					if (flag2)
					{
						byteBuilder.Append(imap_Client_CmdPart.Value);
					}
					else
					{
						bool flag3 = IMAP_Utils.MustUseLiteralString(imap_Client_CmdPart.Value, this.m_pCharset == null && imap.m_MailboxEncoding == IMAP_Mailbox_Encoding.ImapUtf8);
						if (flag3)
						{
							byteBuilder.Append("{" + this.m_pCharset.GetByteCount(imap_Client_CmdPart.Value) + "}\r\n");
							byteBuilder = new ByteBuilder();
							list.Add(byteBuilder);
							byteBuilder.Append(this.m_pCharset, imap_Client_CmdPart.Value);
						}
						else
						{
							bool flag4 = this.m_pCharset == null && imap.m_MailboxEncoding == IMAP_Mailbox_Encoding.ImapUtf8;
							if (flag4)
							{
								byteBuilder.Append(IMAP_Utils.EncodeMailbox(imap_Client_CmdPart.Value, imap.m_MailboxEncoding));
							}
							else
							{
								byteBuilder.Append(TextUtils.QuoteString(imap_Client_CmdPart.Value));
							}
						}
					}
				}
				byteBuilder.Append("\r\n");
				List<string> list3 = new List<string>();
				foreach (ByteBuilder byteBuilder3 in list)
				{
					base.CmdLines.Add(new IMAP_Client.CmdLine(byteBuilder3.ToByte(), Encoding.UTF8.GetString(byteBuilder3.ToByte()).TrimEnd(new char[0])));
				}
			}

			// Token: 0x04000C5E RID: 3166
			private bool m_Uid = false;

			// Token: 0x04000C5F RID: 3167
			private Encoding m_pCharset = null;

			// Token: 0x04000C60 RID: 3168
			private IMAP_Search_Key m_pCriteria = null;
		}

		// Token: 0x02000340 RID: 832
		public class StoreMessageFlagsAsyncOP : IMAP_Client.CmdAsyncOP<IMAP_Client.StoreMessageFlagsAsyncOP>
		{
			// Token: 0x06001AD7 RID: 6871 RVA: 0x000A6B3C File Offset: 0x000A5B3C
			public StoreMessageFlagsAsyncOP(bool uid, IMAP_t_SeqSet seqSet, bool silent, IMAP_Flags_SetType setType, IMAP_t_MsgFlags msgFlags, EventHandler<EventArgs<IMAP_r_u>> callback) : base(callback)
			{
				bool flag = seqSet == null;
				if (flag)
				{
					throw new ArgumentNullException("seqSet");
				}
				bool flag2 = msgFlags == null;
				if (flag2)
				{
					throw new ArgumentNullException("msgFlags");
				}
				this.m_Uid = uid;
				this.m_pSeqSet = seqSet;
				this.m_Silent = silent;
				this.m_FlagsSetType = setType;
				this.m_pMsgFlags = msgFlags;
			}

			// Token: 0x06001AD8 RID: 6872 RVA: 0x000A6BC4 File Offset: 0x000A5BC4
			protected override void OnInitCmdLine(IMAP_Client imap)
			{
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = stringBuilder;
				int commandIndex = imap.m_CommandIndex;
				imap.m_CommandIndex = commandIndex + 1;
				stringBuilder2.Append(commandIndex.ToString("d5"));
				bool uid = this.m_Uid;
				if (uid)
				{
					stringBuilder.Append(" UID");
				}
				stringBuilder.Append(" STORE");
				stringBuilder.Append(" " + this.m_pSeqSet.ToString());
				bool flag = this.m_FlagsSetType == IMAP_Flags_SetType.Add;
				if (flag)
				{
					stringBuilder.Append(" +FLAGS");
				}
				else
				{
					bool flag2 = this.m_FlagsSetType == IMAP_Flags_SetType.Remove;
					if (flag2)
					{
						stringBuilder.Append(" -FLAGS");
					}
					else
					{
						bool flag3 = this.m_FlagsSetType == IMAP_Flags_SetType.Replace;
						if (!flag3)
						{
							throw new NotSupportedException("Not supported argument 'setType' value '" + this.m_FlagsSetType.ToString() + "'.");
						}
						stringBuilder.Append(" FLAGS");
					}
				}
				bool silent = this.m_Silent;
				if (silent)
				{
					stringBuilder.Append(".SILENT");
				}
				bool flag4 = this.m_pMsgFlags != null;
				if (flag4)
				{
					stringBuilder.Append(" (");
					string[] array = this.m_pMsgFlags.ToArray();
					for (int i = 0; i < array.Length; i++)
					{
						bool flag5 = i > 0;
						if (flag5)
						{
							stringBuilder.Append(" ");
						}
						stringBuilder.Append(array[i]);
					}
					stringBuilder.Append(")\r\n");
				}
				else
				{
					stringBuilder.Append(" ()\r\n");
				}
				byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
				base.CmdLines.Add(new IMAP_Client.CmdLine(bytes, Encoding.UTF8.GetString(bytes).TrimEnd(new char[0])));
			}

			// Token: 0x04000C61 RID: 3169
			private bool m_Uid = false;

			// Token: 0x04000C62 RID: 3170
			private IMAP_t_SeqSet m_pSeqSet = null;

			// Token: 0x04000C63 RID: 3171
			private bool m_Silent = true;

			// Token: 0x04000C64 RID: 3172
			private IMAP_Flags_SetType m_FlagsSetType = IMAP_Flags_SetType.Replace;

			// Token: 0x04000C65 RID: 3173
			private IMAP_t_MsgFlags m_pMsgFlags = null;
		}

		// Token: 0x02000341 RID: 833
		public class CopyMessagesAsyncOP : IMAP_Client.CmdAsyncOP<IMAP_Client.CopyMessagesAsyncOP>
		{
			// Token: 0x06001AD9 RID: 6873 RVA: 0x000A6D98 File Offset: 0x000A5D98
			public CopyMessagesAsyncOP(bool uid, IMAP_t_SeqSet seqSet, string targetFolder, EventHandler<EventArgs<IMAP_r_u>> callback) : base(callback)
			{
				bool flag = seqSet == null;
				if (flag)
				{
					throw new ArgumentNullException("seqSet");
				}
				bool flag2 = targetFolder == null;
				if (flag2)
				{
					throw new ArgumentNullException("targetFolder");
				}
				bool flag3 = string.IsNullOrEmpty(targetFolder);
				if (flag3)
				{
					throw new ArgumentException("Argument 'targetFolder' value must be specified.", "targetFolder");
				}
				this.m_Uid = uid;
				this.m_pSeqSet = seqSet;
				this.m_TargetFolder = targetFolder;
			}

			// Token: 0x06001ADA RID: 6874 RVA: 0x000A6E1C File Offset: 0x000A5E1C
			protected override void OnInitCmdLine(IMAP_Client imap)
			{
				bool uid = this.m_Uid;
				if (uid)
				{
					Encoding utf = Encoding.UTF8;
					string[] array = new string[6];
					int num = 0;
					int commandIndex = imap.m_CommandIndex;
					imap.m_CommandIndex = commandIndex + 1;
					array[num] = commandIndex.ToString("d5");
					array[1] = " UID COPY ";
					array[2] = this.m_pSeqSet.ToString();
					array[3] = " ";
					array[4] = IMAP_Utils.EncodeMailbox(this.m_TargetFolder, imap.m_MailboxEncoding);
					array[5] = "\r\n";
					byte[] bytes = utf.GetBytes(string.Concat(array));
					base.CmdLines.Add(new IMAP_Client.CmdLine(bytes, Encoding.UTF8.GetString(bytes).TrimEnd(new char[0])));
				}
				else
				{
					Encoding utf2 = Encoding.UTF8;
					string[] array2 = new string[6];
					int num2 = 0;
					int commandIndex = imap.m_CommandIndex;
					imap.m_CommandIndex = commandIndex + 1;
					array2[num2] = commandIndex.ToString("d5");
					array2[1] = " COPY ";
					array2[2] = this.m_pSeqSet.ToString();
					array2[3] = " ";
					array2[4] = IMAP_Utils.EncodeMailbox(this.m_TargetFolder, imap.m_MailboxEncoding);
					array2[5] = "\r\n";
					byte[] bytes2 = utf2.GetBytes(string.Concat(array2));
					base.CmdLines.Add(new IMAP_Client.CmdLine(bytes2, Encoding.UTF8.GetString(bytes2).TrimEnd(new char[0])));
				}
			}

			// Token: 0x1700087F RID: 2175
			// (get) Token: 0x06001ADB RID: 6875 RVA: 0x000A6F70 File Offset: 0x000A5F70
			public IMAP_t_orc_CopyUid CopyUid
			{
				get
				{
					bool flag = base.State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = base.State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Response' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					bool flag3 = base.FinalResponse != null && base.FinalResponse.OptionalResponse != null && base.FinalResponse.OptionalResponse is IMAP_t_orc_CopyUid;
					IMAP_t_orc_CopyUid result;
					if (flag3)
					{
						result = (IMAP_t_orc_CopyUid)base.FinalResponse.OptionalResponse;
					}
					else
					{
						result = null;
					}
					return result;
				}
			}

			// Token: 0x04000C66 RID: 3174
			private bool m_Uid = false;

			// Token: 0x04000C67 RID: 3175
			private IMAP_t_SeqSet m_pSeqSet = null;

			// Token: 0x04000C68 RID: 3176
			private string m_TargetFolder = null;
		}

		// Token: 0x02000342 RID: 834
		public class ExpungeAsyncOP : IMAP_Client.CmdAsyncOP<IMAP_Client.ExpungeAsyncOP>
		{
			// Token: 0x06001ADC RID: 6876 RVA: 0x000A7002 File Offset: 0x000A6002
			public ExpungeAsyncOP(EventHandler<EventArgs<IMAP_r_u>> callback) : base(callback)
			{
			}

			// Token: 0x06001ADD RID: 6877 RVA: 0x000A7010 File Offset: 0x000A6010
			protected override void OnInitCmdLine(IMAP_Client imap)
			{
				Encoding utf = Encoding.UTF8;
				int commandIndex = imap.m_CommandIndex;
				imap.m_CommandIndex = commandIndex + 1;
				byte[] bytes = utf.GetBytes(commandIndex.ToString("d5") + " EXPUNGE\r\n");
				base.CmdLines.Add(new IMAP_Client.CmdLine(bytes, Encoding.UTF8.GetString(bytes).TrimEnd(new char[0])));
			}
		}

		// Token: 0x02000343 RID: 835
		public class IdleAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x06001ADE RID: 6878 RVA: 0x000A7078 File Offset: 0x000A6078
			public IdleAsyncOP(EventHandler<EventArgs<IMAP_r_u>> callback)
			{
				this.m_pCallback = callback;
			}

			// Token: 0x06001ADF RID: 6879 RVA: 0x000A70D8 File Offset: 0x000A60D8
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pImapClient = null;
					this.m_pFinalResponse = null;
					this.m_pCallback = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x06001AE0 RID: 6880 RVA: 0x000A7124 File Offset: 0x000A6124
			public void Done()
			{
				bool flag = this.State != AsyncOP_State.Active;
				if (flag)
				{
					throw new InvalidOperationException("Mehtod 'Done' can be called only AsyncOP_State.Active state.");
				}
				bool doneSent = this.m_DoneSent;
				if (doneSent)
				{
					throw new InvalidOperationException("Mehtod 'Done' already called, Done is in progress.");
				}
				this.m_DoneSent = true;
				byte[] bytes = Encoding.ASCII.GetBytes("DONE\r\n");
				this.m_pImapClient.LogAddWrite((long)bytes.Length, "DONE");
				this.m_pImapClient.TcpStream.BeginWrite(bytes, 0, bytes.Length, delegate(IAsyncResult ar)
				{
					try
					{
						this.m_pImapClient.TcpStream.EndWrite(ar);
					}
					catch (Exception pException)
					{
						this.m_pException = pException;
						this.m_pImapClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
						this.SetState(AsyncOP_State.Completed);
					}
				}, null);
			}

			// Token: 0x06001AE1 RID: 6881 RVA: 0x000A71B4 File Offset: 0x000A61B4
			internal bool Start(IMAP_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pImapClient = owner;
				this.SetState(AsyncOP_State.Active);
				try
				{
					this.m_pImapClient.m_pIdle = this;
					Encoding utf = Encoding.UTF8;
					IMAP_Client pImapClient = this.m_pImapClient;
					int commandIndex = pImapClient.m_CommandIndex;
					pImapClient.m_CommandIndex = commandIndex + 1;
					byte[] bytes = utf.GetBytes(commandIndex.ToString("d5") + " IDLE\r\n");
					string cmdLineLogText = Encoding.UTF8.GetString(bytes).TrimEnd(new char[0]);
					IMAP_Client.SendCmdAndReadRespAsyncOP sendCmdAndReadRespAsyncOP = new IMAP_Client.SendCmdAndReadRespAsyncOP(bytes, cmdLineLogText, this.m_pCallback);
					sendCmdAndReadRespAsyncOP.CompletedAsync += delegate(object sender, EventArgs<IMAP_Client.SendCmdAndReadRespAsyncOP> e)
					{
						this.ProecessCmdResult(e.Value);
					};
					bool flag2 = !this.m_pImapClient.SendCmdAndReadRespAsync(sendCmdAndReadRespAsyncOP);
					if (flag2)
					{
						this.ProecessCmdResult(sendCmdAndReadRespAsyncOP);
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.m_pImapClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					this.SetState(AsyncOP_State.Completed);
				}
				object pLock = this.m_pLock;
				bool result;
				lock (pLock)
				{
					this.m_RiseCompleted = true;
					result = (this.m_State == AsyncOP_State.Active);
				}
				return result;
			}

			// Token: 0x06001AE2 RID: 6882 RVA: 0x000A7318 File Offset: 0x000A6318
			private void SetState(AsyncOP_State state)
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					object pLock = this.m_pLock;
					lock (pLock)
					{
						this.m_State = state;
						bool flag3 = this.m_State == AsyncOP_State.Completed && this.m_RiseCompleted;
						if (flag3)
						{
							this.OnCompletedAsync();
						}
					}
				}
			}

			// Token: 0x06001AE3 RID: 6883 RVA: 0x000A7390 File Offset: 0x000A6390
			private void ProecessCmdResult(IMAP_Client.SendCmdAndReadRespAsyncOP op)
			{
				try
				{
					bool flag = op.Error != null;
					if (flag)
					{
						this.m_pException = op.Error;
						this.m_pImapClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					}
					else
					{
						bool isError = op.FinalResponse.IsError;
						if (isError)
						{
							this.m_pException = new IMAP_ClientException(op.FinalResponse);
							this.SetState(AsyncOP_State.Completed);
						}
						else
						{
							bool isContinue = op.FinalResponse.IsContinue;
							if (isContinue)
							{
								IMAP_Client.ReadFinalResponseAsyncOP readFinalResponseAsyncOP = new IMAP_Client.ReadFinalResponseAsyncOP(this.m_pCallback);
								readFinalResponseAsyncOP.CompletedAsync += delegate(object sender, EventArgs<IMAP_Client.ReadFinalResponseAsyncOP> e)
								{
									this.ProcessReadFinalResponseResult(e.Value);
								};
								bool flag2 = !this.m_pImapClient.ReadFinalResponseAsync(readFinalResponseAsyncOP);
								if (flag2)
								{
									this.ProcessReadFinalResponseResult(readFinalResponseAsyncOP);
								}
							}
							else
							{
								this.m_pFinalResponse = op.FinalResponse;
								this.SetState(AsyncOP_State.Completed);
							}
						}
					}
				}
				finally
				{
					op.Dispose();
				}
			}

			// Token: 0x06001AE4 RID: 6884 RVA: 0x000A7498 File Offset: 0x000A6498
			private void ProcessReadFinalResponseResult(IMAP_Client.ReadFinalResponseAsyncOP op)
			{
				try
				{
					bool flag = op.Error != null;
					if (flag)
					{
						this.m_pException = op.Error;
						this.m_pImapClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					}
					else
					{
						bool isError = op.FinalResponse.IsError;
						if (isError)
						{
							this.m_pException = new IMAP_ClientException(op.FinalResponse);
							this.SetState(AsyncOP_State.Completed);
						}
						else
						{
							this.m_pImapClient.m_pIdle = null;
							this.m_pFinalResponse = op.FinalResponse;
							this.SetState(AsyncOP_State.Completed);
						}
					}
				}
				finally
				{
					op.Dispose();
				}
			}

			// Token: 0x17000880 RID: 2176
			// (get) Token: 0x06001AE5 RID: 6885 RVA: 0x000A7558 File Offset: 0x000A6558
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x17000881 RID: 2177
			// (get) Token: 0x06001AE6 RID: 6886 RVA: 0x000A7570 File Offset: 0x000A6570
			public Exception Error
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Error' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					return this.m_pException;
				}
			}

			// Token: 0x17000882 RID: 2178
			// (get) Token: 0x06001AE7 RID: 6887 RVA: 0x000A75C4 File Offset: 0x000A65C4
			public IMAP_r_ServerStatus FinalResponse
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Response' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					return this.m_pFinalResponse;
				}
			}

			// Token: 0x140000C9 RID: 201
			// (add) Token: 0x06001AE8 RID: 6888 RVA: 0x000A7618 File Offset: 0x000A6618
			// (remove) Token: 0x06001AE9 RID: 6889 RVA: 0x000A7650 File Offset: 0x000A6650
			
			public event EventHandler<EventArgs<IMAP_Client.IdleAsyncOP>> CompletedAsync = null;

			// Token: 0x06001AEA RID: 6890 RVA: 0x000A7688 File Offset: 0x000A6688
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<IMAP_Client.IdleAsyncOP>(this));
				}
			}

			// Token: 0x04000C69 RID: 3177
			private object m_pLock = new object();

			// Token: 0x04000C6A RID: 3178
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000C6B RID: 3179
			private Exception m_pException = null;

			// Token: 0x04000C6C RID: 3180
			private IMAP_r_ServerStatus m_pFinalResponse = null;

			// Token: 0x04000C6D RID: 3181
			private IMAP_Client m_pImapClient = null;

			// Token: 0x04000C6E RID: 3182
			private bool m_RiseCompleted = false;

			// Token: 0x04000C6F RID: 3183
			private EventHandler<EventArgs<IMAP_r_u>> m_pCallback = null;

			// Token: 0x04000C70 RID: 3184
			private bool m_DoneSent = false;
		}

		// Token: 0x02000344 RID: 836
		public class CapabilityAsyncOP : IMAP_Client.CmdAsyncOP<IMAP_Client.CapabilityAsyncOP>
		{
			// Token: 0x06001AEE RID: 6894 RVA: 0x000A7748 File Offset: 0x000A6748
			public CapabilityAsyncOP(EventHandler<EventArgs<IMAP_r_u>> callback) : base(callback)
			{
			}

			// Token: 0x06001AEF RID: 6895 RVA: 0x000A7754 File Offset: 0x000A6754
			protected override void OnInitCmdLine(IMAP_Client imap)
			{
				Encoding utf = Encoding.UTF8;
				int commandIndex = imap.m_CommandIndex;
				imap.m_CommandIndex = commandIndex + 1;
				byte[] bytes = utf.GetBytes(commandIndex.ToString("d5") + " CAPABILITY\r\n");
				base.CmdLines.Add(new IMAP_Client.CmdLine(bytes, Encoding.UTF8.GetString(bytes).TrimEnd(new char[0])));
			}
		}

		// Token: 0x02000345 RID: 837
		public class NoopAsyncOP : IMAP_Client.CmdAsyncOP<IMAP_Client.NoopAsyncOP>
		{
			// Token: 0x06001AF0 RID: 6896 RVA: 0x000A77BB File Offset: 0x000A67BB
			public NoopAsyncOP(EventHandler<EventArgs<IMAP_r_u>> callback) : base(callback)
			{
			}

			// Token: 0x06001AF1 RID: 6897 RVA: 0x000A77C8 File Offset: 0x000A67C8
			protected override void OnInitCmdLine(IMAP_Client imap)
			{
				Encoding utf = Encoding.UTF8;
				int commandIndex = imap.m_CommandIndex;
				imap.m_CommandIndex = commandIndex + 1;
				byte[] bytes = utf.GetBytes(commandIndex.ToString("d5") + " NOOP\r\n");
				base.CmdLines.Add(new IMAP_Client.CmdLine(bytes, Encoding.UTF8.GetString(bytes).TrimEnd(new char[0])));
			}
		}

		// Token: 0x02000346 RID: 838
		private class SendCmdAndReadRespAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x06001AF2 RID: 6898 RVA: 0x000A7830 File Offset: 0x000A6830
			public SendCmdAndReadRespAsyncOP(byte[] cmdLine, string cmdLineLogText, EventHandler<EventArgs<IMAP_r_u>> callback)
			{
				bool flag = cmdLine == null;
				if (flag)
				{
					throw new ArgumentNullException("cmdLine");
				}
				bool flag2 = cmdLine.Length < 1;
				if (flag2)
				{
					throw new ArgumentException("Argument 'cmdLine' value must be specified.", "cmdLine");
				}
				bool flag3 = cmdLineLogText == null;
				if (flag3)
				{
					throw new ArgumentNullException("cmdLineLogText");
				}
				this.m_pCallback = callback;
				this.m_pCmdLines = new Queue<IMAP_Client.CmdLine>();
				this.m_pCmdLines.Enqueue(new IMAP_Client.CmdLine(cmdLine, cmdLineLogText));
			}

			// Token: 0x06001AF3 RID: 6899 RVA: 0x000A78F0 File Offset: 0x000A68F0
			public SendCmdAndReadRespAsyncOP(IMAP_Client.CmdLine[] cmdLines, EventHandler<EventArgs<IMAP_r_u>> callback)
			{
				bool flag = cmdLines == null;
				if (flag)
				{
					throw new ArgumentNullException("cmdLines");
				}
				this.m_pCmdLines = new Queue<IMAP_Client.CmdLine>(cmdLines);
				this.m_pCallback = callback;
			}

			// Token: 0x06001AF4 RID: 6900 RVA: 0x000A7970 File Offset: 0x000A6970
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pImapClient = null;
					this.m_pFinalResponse = null;
					this.m_pCmdLines = null;
					this.m_pCallback = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x06001AF5 RID: 6901 RVA: 0x000A79C0 File Offset: 0x000A69C0
			internal bool Start(IMAP_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pImapClient = owner;
				this.SetState(AsyncOP_State.Active);
				this.SendCmdLine();
				object pLock = this.m_pLock;
				bool result;
				lock (pLock)
				{
					this.m_RiseCompleted = true;
					result = (this.m_State == AsyncOP_State.Active);
				}
				return result;
			}

			// Token: 0x06001AF6 RID: 6902 RVA: 0x000A7A3C File Offset: 0x000A6A3C
			private void SetState(AsyncOP_State state)
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					object pLock = this.m_pLock;
					lock (pLock)
					{
						this.m_State = state;
						bool flag3 = this.m_State == AsyncOP_State.Completed && this.m_RiseCompleted;
						if (flag3)
						{
							this.OnCompletedAsync();
						}
					}
				}
			}

			// Token: 0x06001AF7 RID: 6903 RVA: 0x000A7AB4 File Offset: 0x000A6AB4
			private void SendCmdLine()
			{
				try
				{
					bool flag = this.m_pCmdLines.Count == 0;
					if (flag)
					{
						throw new Exception("Internal error: No next IMAP command line.");
					}
					IMAP_Client.CmdLine cmdLine = this.m_pCmdLines.Dequeue();
					this.m_pImapClient.LogAddWrite((long)cmdLine.Data.Length, cmdLine.LogText);
					this.m_pImapClient.TcpStream.BeginWrite(cmdLine.Data, 0, cmdLine.Data.Length, new AsyncCallback(this.ProcessCmdLineSendResult), null);
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.m_pImapClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x06001AF8 RID: 6904 RVA: 0x000A7B84 File Offset: 0x000A6B84
			private void ProcessCmdLineSendResult(IAsyncResult ar)
			{
				try
				{
					this.m_pImapClient.TcpStream.EndWrite(ar);
					IMAP_Client.ReadFinalResponseAsyncOP args = new IMAP_Client.ReadFinalResponseAsyncOP(this.m_pCallback);
					args.CompletedAsync += delegate(object sender, EventArgs<IMAP_Client.ReadFinalResponseAsyncOP> e)
					{
						try
						{
							bool flag3 = args.Error != null;
							if (flag3)
							{
								this.m_pException = e.Value.Error;
								this.SetState(AsyncOP_State.Completed);
							}
							else
							{
								bool flag4 = args.FinalResponse.IsContinue && this.m_pCmdLines.Count > 0;
								if (flag4)
								{
									this.SendCmdLine();
								}
								else
								{
									this.m_pFinalResponse = args.FinalResponse;
									this.SetState(AsyncOP_State.Completed);
								}
							}
						}
						finally
						{
							args.Dispose();
						}
					};
					bool flag = !this.m_pImapClient.ReadFinalResponseAsync(args);
					if (flag)
					{
						try
						{
							bool flag2 = args.Error != null;
							if (flag2)
							{
								this.m_pException = args.Error;
							}
							else
							{
								this.m_pFinalResponse = args.FinalResponse;
							}
							this.SetState(AsyncOP_State.Completed);
						}
						finally
						{
							args.Dispose();
						}
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.m_pImapClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x17000883 RID: 2179
			// (get) Token: 0x06001AF9 RID: 6905 RVA: 0x000A7CA4 File Offset: 0x000A6CA4
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x17000884 RID: 2180
			// (get) Token: 0x06001AFA RID: 6906 RVA: 0x000A7CBC File Offset: 0x000A6CBC
			public Exception Error
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Error' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					return this.m_pException;
				}
			}

			// Token: 0x17000885 RID: 2181
			// (get) Token: 0x06001AFB RID: 6907 RVA: 0x000A7D10 File Offset: 0x000A6D10
			public IMAP_r_ServerStatus FinalResponse
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Response' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					return this.m_pFinalResponse;
				}
			}

			// Token: 0x140000CA RID: 202
			// (add) Token: 0x06001AFC RID: 6908 RVA: 0x000A7D64 File Offset: 0x000A6D64
			// (remove) Token: 0x06001AFD RID: 6909 RVA: 0x000A7D9C File Offset: 0x000A6D9C
			
			public event EventHandler<EventArgs<IMAP_Client.SendCmdAndReadRespAsyncOP>> CompletedAsync = null;

			// Token: 0x06001AFE RID: 6910 RVA: 0x000A7DD4 File Offset: 0x000A6DD4
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<IMAP_Client.SendCmdAndReadRespAsyncOP>(this));
				}
			}

			// Token: 0x04000C72 RID: 3186
			private object m_pLock = new object();

			// Token: 0x04000C73 RID: 3187
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000C74 RID: 3188
			private Exception m_pException = null;

			// Token: 0x04000C75 RID: 3189
			private IMAP_r_ServerStatus m_pFinalResponse = null;

			// Token: 0x04000C76 RID: 3190
			private IMAP_Client m_pImapClient = null;

			// Token: 0x04000C77 RID: 3191
			private bool m_RiseCompleted = false;

			// Token: 0x04000C78 RID: 3192
			private Queue<IMAP_Client.CmdLine> m_pCmdLines = null;

			// Token: 0x04000C79 RID: 3193
			private EventHandler<EventArgs<IMAP_r_u>> m_pCallback = null;
		}

		// Token: 0x02000347 RID: 839
		private class ReadResponseAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x06001B00 RID: 6912 RVA: 0x000A7E58 File Offset: 0x000A6E58
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pImapClient = null;
					this.m_pResponse = null;
					bool flag2 = this.m_pReadLineOP != null;
					if (flag2)
					{
						this.m_pReadLineOP.Dispose();
					}
					this.m_pReadLineOP = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x06001B01 RID: 6913 RVA: 0x000A7EBC File Offset: 0x000A6EBC
			internal bool Start(IMAP_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pImapClient = owner;
				this.m_pReadLineOP = new SmartStream.ReadLineAsyncOP(new byte[this.m_pImapClient.Settings.ResponseLineSize], SizeExceededAction.JunkAndThrowException);
				this.m_pReadLineOP.CompletedAsync += this.m_pReadLineOP_Completed;
				this.SetState(AsyncOP_State.Active);
				try
				{
					bool flag2 = owner.TcpStream.ReadLine(this.m_pReadLineOP, true);
					if (flag2)
					{
						this.ReadLineCompleted(this.m_pReadLineOP);
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.m_pImapClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					this.SetState(AsyncOP_State.Completed);
				}
				object pLock = this.m_pLock;
				bool result;
				lock (pLock)
				{
					this.m_RiseCompleted = true;
					result = (this.m_State == AsyncOP_State.Active);
				}
				return result;
			}

			// Token: 0x06001B02 RID: 6914 RVA: 0x000A7FDC File Offset: 0x000A6FDC
			public void Reuse()
			{
				bool flag = this.m_State != AsyncOP_State.Completed;
				if (flag)
				{
					throw new InvalidOperationException("Reuse is valid only in Completed state.");
				}
				this.m_State = AsyncOP_State.WaitingForStart;
				this.m_pException = null;
				this.m_pResponse = null;
				this.m_pImapClient = null;
				this.m_RiseCompleted = false;
			}

			// Token: 0x06001B03 RID: 6915 RVA: 0x000A802C File Offset: 0x000A702C
			private void SetState(AsyncOP_State state)
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					object pLock = this.m_pLock;
					lock (pLock)
					{
						this.m_State = state;
						bool flag3 = this.m_State == AsyncOP_State.Completed && this.m_RiseCompleted;
						if (flag3)
						{
							this.OnCompletedAsync();
						}
					}
				}
			}

			// Token: 0x06001B04 RID: 6916 RVA: 0x000A80A4 File Offset: 0x000A70A4
			private void m_pReadLineOP_Completed(object sender, EventArgs<SmartStream.ReadLineAsyncOP> e)
			{
				try
				{
					this.ReadLineCompleted(this.m_pReadLineOP);
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x06001B05 RID: 6917 RVA: 0x000A80E8 File Offset: 0x000A70E8
			private void ReadLineCompleted(SmartStream.ReadLineAsyncOP op)
			{
				bool flag = op == null;
				if (flag)
				{
					throw new ArgumentNullException("op");
				}
				try
				{
					bool flag2 = op.Error != null;
					if (flag2)
					{
						this.m_pException = op.Error;
					}
					else
					{
						bool flag3 = op.BytesInBuffer == 0;
						if (flag3)
						{
							this.m_pException = new IOException("The remote host shut-down socket.");
						}
						else
						{
							string lineUtf = op.LineUtf8;
							this.m_pImapClient.LogAddRead((long)op.BytesInBuffer, lineUtf);
							bool flag4 = lineUtf.StartsWith("*");
							if (flag4)
							{
								string[] array = lineUtf.Split(new char[]
								{
									' '
								}, 4);
								string text = lineUtf.Split(new char[]
								{
									' '
								})[1];
								bool flag5 = text.Equals("OK", StringComparison.InvariantCultureIgnoreCase);
								if (flag5)
								{
									IMAP_r_u_ServerStatus imap_r_u_ServerStatus = IMAP_r_u_ServerStatus.Parse(lineUtf);
									this.m_pResponse = imap_r_u_ServerStatus;
									bool flag6 = imap_r_u_ServerStatus.OptionalResponse != null;
									if (flag6)
									{
										bool flag7 = imap_r_u_ServerStatus.OptionalResponse is IMAP_t_orc_PermanentFlags;
										if (flag7)
										{
											bool flag8 = this.m_pImapClient.SelectedFolder != null;
											if (flag8)
											{
												this.m_pImapClient.SelectedFolder.SetPermanentFlags(((IMAP_t_orc_PermanentFlags)imap_r_u_ServerStatus.OptionalResponse).Flags);
											}
										}
										else
										{
											bool flag9 = imap_r_u_ServerStatus.OptionalResponse is IMAP_t_orc_ReadOnly;
											if (flag9)
											{
												bool flag10 = this.m_pImapClient.SelectedFolder != null;
												if (flag10)
												{
													this.m_pImapClient.SelectedFolder.SetReadOnly(true);
												}
											}
											else
											{
												bool flag11 = imap_r_u_ServerStatus.OptionalResponse is IMAP_t_orc_ReadWrite;
												if (flag11)
												{
													bool flag12 = this.m_pImapClient.SelectedFolder != null;
													if (flag12)
													{
														this.m_pImapClient.SelectedFolder.SetReadOnly(true);
													}
												}
												else
												{
													bool flag13 = imap_r_u_ServerStatus.OptionalResponse is IMAP_t_orc_UidNext;
													if (flag13)
													{
														bool flag14 = this.m_pImapClient.SelectedFolder != null;
														if (flag14)
														{
															this.m_pImapClient.SelectedFolder.SetUidNext((long)((IMAP_t_orc_UidNext)imap_r_u_ServerStatus.OptionalResponse).UidNext);
														}
													}
													else
													{
														bool flag15 = imap_r_u_ServerStatus.OptionalResponse is IMAP_t_orc_UidValidity;
														if (flag15)
														{
															bool flag16 = this.m_pImapClient.SelectedFolder != null;
															if (flag16)
															{
																this.m_pImapClient.SelectedFolder.SetUidValidity(((IMAP_t_orc_UidValidity)imap_r_u_ServerStatus.OptionalResponse).Uid);
															}
														}
														else
														{
															bool flag17 = imap_r_u_ServerStatus.OptionalResponse is IMAP_t_orc_Unseen;
															if (flag17)
															{
																bool flag18 = this.m_pImapClient.SelectedFolder != null;
																if (flag18)
																{
																	this.m_pImapClient.SelectedFolder.SetFirstUnseen(((IMAP_t_orc_Unseen)imap_r_u_ServerStatus.OptionalResponse).SeqNo);
																}
															}
														}
													}
												}
											}
										}
									}
									this.m_pImapClient.OnUntaggedStatusResponse((IMAP_r_u)this.m_pResponse);
								}
								else
								{
									bool flag19 = text.Equals("NO", StringComparison.InvariantCultureIgnoreCase);
									if (flag19)
									{
										this.m_pResponse = IMAP_r_u_ServerStatus.Parse(lineUtf);
										this.m_pImapClient.OnUntaggedStatusResponse((IMAP_r_u)this.m_pResponse);
									}
									else
									{
										bool flag20 = text.Equals("BAD", StringComparison.InvariantCultureIgnoreCase);
										if (flag20)
										{
											this.m_pResponse = IMAP_r_u_ServerStatus.Parse(lineUtf);
											this.m_pImapClient.OnUntaggedStatusResponse((IMAP_r_u)this.m_pResponse);
										}
										else
										{
											bool flag21 = text.Equals("PREAUTH", StringComparison.InvariantCultureIgnoreCase);
											if (flag21)
											{
												this.m_pResponse = IMAP_r_u_ServerStatus.Parse(lineUtf);
												this.m_pImapClient.OnUntaggedStatusResponse((IMAP_r_u)this.m_pResponse);
											}
											else
											{
												bool flag22 = text.Equals("BYE", StringComparison.InvariantCultureIgnoreCase);
												if (flag22)
												{
													this.m_pResponse = IMAP_r_u_ServerStatus.Parse(lineUtf);
													this.m_pImapClient.OnUntaggedStatusResponse((IMAP_r_u)this.m_pResponse);
												}
												else
												{
													bool flag23 = text.Equals("CAPABILITY", StringComparison.InvariantCultureIgnoreCase);
													if (flag23)
													{
														this.m_pResponse = IMAP_r_u_Capability.Parse(lineUtf);
														this.m_pImapClient.m_pCapabilities = new List<string>();
														this.m_pImapClient.m_pCapabilities.AddRange(((IMAP_r_u_Capability)this.m_pResponse).Capabilities);
													}
													else
													{
														bool flag24 = text.Equals("LIST", StringComparison.InvariantCultureIgnoreCase);
														if (flag24)
														{
															this.m_pResponse = IMAP_r_u_List.Parse(lineUtf);
														}
														else
														{
															bool flag25 = text.Equals("LSUB", StringComparison.InvariantCultureIgnoreCase);
															if (flag25)
															{
																this.m_pResponse = IMAP_r_u_LSub.Parse(lineUtf);
															}
															else
															{
																bool flag26 = text.Equals("STATUS", StringComparison.InvariantCultureIgnoreCase);
																if (flag26)
																{
																	this.m_pResponse = IMAP_r_u_Status.Parse(lineUtf);
																}
																else
																{
																	bool flag27 = text.Equals("SEARCH", StringComparison.InvariantCultureIgnoreCase);
																	if (flag27)
																	{
																		this.m_pResponse = IMAP_r_u_Search.Parse(lineUtf);
																	}
																	else
																	{
																		bool flag28 = text.Equals("FLAGS", StringComparison.InvariantCultureIgnoreCase);
																		if (flag28)
																		{
																			this.m_pResponse = IMAP_r_u_Flags.Parse(lineUtf);
																			bool flag29 = this.m_pImapClient.m_pSelectedFolder != null;
																			if (flag29)
																			{
																				this.m_pImapClient.m_pSelectedFolder.SetFlags(((IMAP_r_u_Flags)this.m_pResponse).Flags);
																			}
																		}
																		else
																		{
																			bool flag30 = Net_Utils.IsInteger(text) && array[2].Equals("EXISTS", StringComparison.InvariantCultureIgnoreCase);
																			if (flag30)
																			{
																				this.m_pResponse = IMAP_r_u_Exists.Parse(lineUtf);
																				bool flag31 = this.m_pImapClient.m_pSelectedFolder != null;
																				if (flag31)
																				{
																					this.m_pImapClient.m_pSelectedFolder.SetMessagesCount(((IMAP_r_u_Exists)this.m_pResponse).MessageCount);
																				}
																			}
																			else
																			{
																				bool flag32 = Net_Utils.IsInteger(text) && array[2].Equals("RECENT", StringComparison.InvariantCultureIgnoreCase);
																				if (flag32)
																				{
																					this.m_pResponse = IMAP_r_u_Recent.Parse(lineUtf);
																					bool flag33 = this.m_pImapClient.m_pSelectedFolder != null;
																					if (flag33)
																					{
																						this.m_pImapClient.m_pSelectedFolder.SetRecentMessagesCount(((IMAP_r_u_Recent)this.m_pResponse).MessageCount);
																					}
																				}
																				else
																				{
																					bool flag34 = Net_Utils.IsInteger(text) && array[2].Equals("EXPUNGE", StringComparison.InvariantCultureIgnoreCase);
																					if (flag34)
																					{
																						this.m_pResponse = IMAP_r_u_Expunge.Parse(lineUtf);
																						this.m_pImapClient.OnMessageExpunged((IMAP_r_u_Expunge)this.m_pResponse);
																					}
																					else
																					{
																						bool flag35 = Net_Utils.IsInteger(text) && array[2].Equals("FETCH", StringComparison.InvariantCultureIgnoreCase);
																						if (flag35)
																						{
																							IMAP_r_u_Fetch imap_r_u_Fetch = new IMAP_r_u_Fetch(1);
																							this.m_pResponse = imap_r_u_Fetch;
																							imap_r_u_Fetch.ParseAsync(this.m_pImapClient, lineUtf, new EventHandler<EventArgs<Exception>>(this.FetchParsingCompleted));
																							return;
																						}
																						bool flag36 = text.Equals("ACL", StringComparison.InvariantCultureIgnoreCase);
																						if (flag36)
																						{
																							this.m_pResponse = IMAP_r_u_Acl.Parse(lineUtf);
																						}
																						else
																						{
																							bool flag37 = text.Equals("LISTRIGHTS", StringComparison.InvariantCultureIgnoreCase);
																							if (flag37)
																							{
																								this.m_pResponse = IMAP_r_u_ListRights.Parse(lineUtf);
																							}
																							else
																							{
																								bool flag38 = text.Equals("MYRIGHTS", StringComparison.InvariantCultureIgnoreCase);
																								if (flag38)
																								{
																									this.m_pResponse = IMAP_r_u_MyRights.Parse(lineUtf);
																								}
																								else
																								{
																									bool flag39 = text.Equals("QUOTA", StringComparison.InvariantCultureIgnoreCase);
																									if (flag39)
																									{
																										this.m_pResponse = IMAP_r_u_Quota.Parse(lineUtf);
																									}
																									else
																									{
																										bool flag40 = text.Equals("QUOTAROOT", StringComparison.InvariantCultureIgnoreCase);
																										if (flag40)
																										{
																											this.m_pResponse = IMAP_r_u_QuotaRoot.Parse(lineUtf);
																										}
																										else
																										{
																											bool flag41 = text.Equals("NAMESPACE", StringComparison.InvariantCultureIgnoreCase);
																											if (flag41)
																											{
																												this.m_pResponse = IMAP_r_u_Namespace.Parse(lineUtf);
																											}
																											else
																											{
																												bool flag42 = text.Equals("ENABLED", StringComparison.InvariantCultureIgnoreCase);
																												if (flag42)
																												{
																													this.m_pResponse = IMAP_r_u_Enable.Parse(lineUtf);
																												}
																											}
																										}
																									}
																								}
																							}
																						}
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
								this.m_pImapClient.OnUntaggedResponse((IMAP_r_u)this.m_pResponse);
							}
							else
							{
								bool flag43 = lineUtf.StartsWith("+");
								if (flag43)
								{
									this.m_pResponse = IMAP_r_ServerStatus.Parse(lineUtf);
								}
								else
								{
									this.m_pResponse = IMAP_r_ServerStatus.Parse(lineUtf);
								}
							}
						}
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
				}
				this.SetState(AsyncOP_State.Completed);
			}

			// Token: 0x06001B06 RID: 6918 RVA: 0x000A8904 File Offset: 0x000A7904
			private void FetchParsingCompleted(object sender, EventArgs<Exception> e)
			{
				try
				{
					bool flag = e.Value != null;
					if (flag)
					{
						this.m_pException = e.Value;
					}
					this.m_pImapClient.OnUntaggedResponse((IMAP_r_u)this.m_pResponse);
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
				}
				this.SetState(AsyncOP_State.Completed);
			}

			// Token: 0x17000886 RID: 2182
			// (get) Token: 0x06001B07 RID: 6919 RVA: 0x000A896C File Offset: 0x000A796C
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x17000887 RID: 2183
			// (get) Token: 0x06001B08 RID: 6920 RVA: 0x000A8984 File Offset: 0x000A7984
			public Exception Error
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Error' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					return this.m_pException;
				}
			}

			// Token: 0x17000888 RID: 2184
			// (get) Token: 0x06001B09 RID: 6921 RVA: 0x000A89D8 File Offset: 0x000A79D8
			public IMAP_r Response
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Response' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					return this.m_pResponse;
				}
			}

			// Token: 0x140000CB RID: 203
			// (add) Token: 0x06001B0A RID: 6922 RVA: 0x000A8A2C File Offset: 0x000A7A2C
			// (remove) Token: 0x06001B0B RID: 6923 RVA: 0x000A8A64 File Offset: 0x000A7A64
			
			public event EventHandler<EventArgs<IMAP_Client.ReadResponseAsyncOP>> CompletedAsync = null;

			// Token: 0x06001B0C RID: 6924 RVA: 0x000A8A9C File Offset: 0x000A7A9C
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<IMAP_Client.ReadResponseAsyncOP>(this));
				}
			}

			// Token: 0x04000C7B RID: 3195
			private object m_pLock = new object();

			// Token: 0x04000C7C RID: 3196
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000C7D RID: 3197
			private Exception m_pException = null;

			// Token: 0x04000C7E RID: 3198
			private IMAP_r m_pResponse = null;

			// Token: 0x04000C7F RID: 3199
			private IMAP_Client m_pImapClient = null;

			// Token: 0x04000C80 RID: 3200
			private bool m_RiseCompleted = false;

			// Token: 0x04000C81 RID: 3201
			private SmartStream.ReadLineAsyncOP m_pReadLineOP = null;
		}

		// Token: 0x02000348 RID: 840
		private class ReadFinalResponseAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x06001B0D RID: 6925 RVA: 0x000A8ACC File Offset: 0x000A7ACC
			public ReadFinalResponseAsyncOP(EventHandler<EventArgs<IMAP_r_u>> callback)
			{
				this.m_pCallback = callback;
			}

			// Token: 0x06001B0E RID: 6926 RVA: 0x000A8B24 File Offset: 0x000A7B24
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pImapClient = null;
					this.m_pFinalResponse = null;
					this.m_pCallback = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x06001B0F RID: 6927 RVA: 0x000A8B70 File Offset: 0x000A7B70
			internal bool Start(IMAP_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pImapClient = owner;
				this.SetState(AsyncOP_State.Active);
				try
				{
					IMAP_Client.ReadResponseAsyncOP args = new IMAP_Client.ReadResponseAsyncOP();
					args.CompletedAsync += delegate(object sender, EventArgs<IMAP_Client.ReadResponseAsyncOP> e)
					{
						try
						{
							this.ResponseReadingCompleted(e.Value);
							args.Reuse();
							while (this.m_State == AsyncOP_State.Active && !this.m_pImapClient.ReadResponseAsync(args))
							{
								this.ResponseReadingCompleted(args);
								args.Reuse();
							}
						}
						catch (Exception pException2)
						{
							this.m_pException = pException2;
							this.m_pImapClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
							this.SetState(AsyncOP_State.Completed);
						}
					};
					while (this.m_State == AsyncOP_State.Active && !this.m_pImapClient.ReadResponseAsync(args))
					{
						this.ResponseReadingCompleted(args);
						args.Reuse();
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.m_pImapClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					this.SetState(AsyncOP_State.Completed);
				}
				object pLock = this.m_pLock;
				bool result;
				lock (pLock)
				{
					this.m_RiseCompleted = true;
					result = (this.m_State == AsyncOP_State.Active);
				}
				return result;
			}

			// Token: 0x06001B10 RID: 6928 RVA: 0x000A8CA8 File Offset: 0x000A7CA8
			private void SetState(AsyncOP_State state)
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					object pLock = this.m_pLock;
					lock (pLock)
					{
						this.m_State = state;
						bool flag3 = this.m_State == AsyncOP_State.Completed && this.m_RiseCompleted;
						if (flag3)
						{
							this.OnCompletedAsync();
						}
					}
				}
			}

			// Token: 0x06001B11 RID: 6929 RVA: 0x000A8D20 File Offset: 0x000A7D20
			private void ResponseReadingCompleted(IMAP_Client.ReadResponseAsyncOP op)
			{
				bool flag = op == null;
				if (flag)
				{
					throw new ArgumentNullException("op");
				}
				try
				{
					bool flag2 = op.Error != null;
					if (flag2)
					{
						this.m_pException = op.Error;
						this.SetState(AsyncOP_State.Completed);
					}
					else
					{
						bool flag3 = op.Response is IMAP_r_ServerStatus;
						if (flag3)
						{
							this.m_pFinalResponse = (IMAP_r_ServerStatus)op.Response;
							this.SetState(AsyncOP_State.Completed);
						}
						else
						{
							bool flag4 = this.m_pCallback != null;
							if (flag4)
							{
								this.m_pCallback(this, new EventArgs<IMAP_r_u>((IMAP_r_u)op.Response));
							}
						}
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x17000889 RID: 2185
			// (get) Token: 0x06001B12 RID: 6930 RVA: 0x000A8DF4 File Offset: 0x000A7DF4
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x1700088A RID: 2186
			// (get) Token: 0x06001B13 RID: 6931 RVA: 0x000A8E0C File Offset: 0x000A7E0C
			public Exception Error
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Error' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					return this.m_pException;
				}
			}

			// Token: 0x1700088B RID: 2187
			// (get) Token: 0x06001B14 RID: 6932 RVA: 0x000A8E60 File Offset: 0x000A7E60
			public IMAP_r_ServerStatus FinalResponse
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Response' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					return this.m_pFinalResponse;
				}
			}

			// Token: 0x140000CC RID: 204
			// (add) Token: 0x06001B15 RID: 6933 RVA: 0x000A8EB4 File Offset: 0x000A7EB4
			// (remove) Token: 0x06001B16 RID: 6934 RVA: 0x000A8EEC File Offset: 0x000A7EEC
			
			public event EventHandler<EventArgs<IMAP_Client.ReadFinalResponseAsyncOP>> CompletedAsync = null;

			// Token: 0x06001B17 RID: 6935 RVA: 0x000A8F24 File Offset: 0x000A7F24
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<IMAP_Client.ReadFinalResponseAsyncOP>(this));
				}
			}

			// Token: 0x04000C83 RID: 3203
			private object m_pLock = new object();

			// Token: 0x04000C84 RID: 3204
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000C85 RID: 3205
			private Exception m_pException = null;

			// Token: 0x04000C86 RID: 3206
			private IMAP_r_ServerStatus m_pFinalResponse = null;

			// Token: 0x04000C87 RID: 3207
			private IMAP_Client m_pImapClient = null;

			// Token: 0x04000C88 RID: 3208
			private bool m_RiseCompleted = false;

			// Token: 0x04000C89 RID: 3209
			private EventHandler<EventArgs<IMAP_r_u>> m_pCallback = null;
		}

		// Token: 0x02000349 RID: 841
		internal class ReadStringLiteralAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x06001B18 RID: 6936 RVA: 0x000A8F54 File Offset: 0x000A7F54
			public ReadStringLiteralAsyncOP(Stream stream, int literalSize)
			{
				bool flag = stream == null;
				if (flag)
				{
					throw new ArgumentNullException("stream");
				}
				this.m_pStream = stream;
				this.m_LiteralSize = literalSize;
			}

			// Token: 0x06001B19 RID: 6937 RVA: 0x000A8FC8 File Offset: 0x000A7FC8
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pImapClient = null;
					this.m_pStream = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x06001B1A RID: 6938 RVA: 0x000A900C File Offset: 0x000A800C
			public bool Start(IMAP_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pImapClient = owner;
				this.SetState(AsyncOP_State.Active);
				owner.TcpStream.BeginReadFixedCount(this.m_pStream, (long)this.m_LiteralSize, new AsyncCallback(this.ReadingCompleted), null);
				object pLock = this.m_pLock;
				bool result;
				lock (pLock)
				{
					this.m_RiseCompleted = true;
					result = (this.m_State == AsyncOP_State.Active);
				}
				return result;
			}

			// Token: 0x06001B1B RID: 6939 RVA: 0x000A90A8 File Offset: 0x000A80A8
			private void SetState(AsyncOP_State state)
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					object pLock = this.m_pLock;
					lock (pLock)
					{
						this.m_State = state;
						bool flag3 = this.m_State == AsyncOP_State.Completed && this.m_RiseCompleted;
						if (flag3)
						{
							this.OnCompletedAsync();
						}
					}
				}
			}

			// Token: 0x06001B1C RID: 6940 RVA: 0x000A9120 File Offset: 0x000A8120
			private void ReadingCompleted(IAsyncResult result)
			{
				try
				{
					this.m_pImapClient.TcpStream.EndReadFixedCount(result);
					this.m_pImapClient.LogAddRead((long)this.m_LiteralSize, "Readed string-literal " + this.m_LiteralSize.ToString() + " bytes.");
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
				}
				this.SetState(AsyncOP_State.Completed);
			}

			// Token: 0x1700088C RID: 2188
			// (get) Token: 0x06001B1D RID: 6941 RVA: 0x000A9198 File Offset: 0x000A8198
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x1700088D RID: 2189
			// (get) Token: 0x06001B1E RID: 6942 RVA: 0x000A91B0 File Offset: 0x000A81B0
			public Exception Error
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Error' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					return this.m_pException;
				}
			}

			// Token: 0x1700088E RID: 2190
			// (get) Token: 0x06001B1F RID: 6943 RVA: 0x000A9204 File Offset: 0x000A8204
			public Stream Stream
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Error' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					return this.m_pStream;
				}
			}

			// Token: 0x140000CD RID: 205
			// (add) Token: 0x06001B20 RID: 6944 RVA: 0x000A9258 File Offset: 0x000A8258
			// (remove) Token: 0x06001B21 RID: 6945 RVA: 0x000A9290 File Offset: 0x000A8290
			
			public event EventHandler<EventArgs<IMAP_Client.ReadStringLiteralAsyncOP>> CompletedAsync = null;

			// Token: 0x06001B22 RID: 6946 RVA: 0x000A92C8 File Offset: 0x000A82C8
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<IMAP_Client.ReadStringLiteralAsyncOP>(this));
				}
			}

			// Token: 0x04000C8B RID: 3211
			private object m_pLock = new object();

			// Token: 0x04000C8C RID: 3212
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000C8D RID: 3213
			private Exception m_pException = null;

			// Token: 0x04000C8E RID: 3214
			private Stream m_pStream = null;

			// Token: 0x04000C8F RID: 3215
			private int m_LiteralSize = 0;

			// Token: 0x04000C90 RID: 3216
			private IMAP_Client m_pImapClient = null;

			// Token: 0x04000C91 RID: 3217
			private bool m_RiseCompleted = false;
		}

		// Token: 0x0200034A RID: 842
		[Obsolete("deprecated")]
		internal class _FetchResponseReader
		{
			// Token: 0x06001B23 RID: 6947 RVA: 0x000A92F8 File Offset: 0x000A82F8
			public _FetchResponseReader(IMAP_Client imap, string fetchLine, IMAP_Client_FetchHandler handler)
			{
				bool flag = imap == null;
				if (flag)
				{
					throw new ArgumentNullException("imap");
				}
				bool flag2 = fetchLine == null;
				if (flag2)
				{
					throw new ArgumentNullException("fetchLine");
				}
				bool flag3 = handler == null;
				if (flag3)
				{
					throw new ArgumentNullException("handler");
				}
				this.m_pImap = imap;
				this.m_FetchLine = fetchLine;
				this.m_pHandler = handler;
			}

			// Token: 0x06001B24 RID: 6948 RVA: 0x000A937C File Offset: 0x000A837C
			public void Start()
			{
				int currentSeqNo = Convert.ToInt32(this.m_FetchLine.Split(new char[]
				{
					' '
				})[1]);
				this.m_pHandler.SetCurrentSeqNo(currentSeqNo);
				this.m_pHandler.OnNextMessage();
				this.m_pFetchReader = new StringReader(this.m_FetchLine.Split(new char[]
				{
					' '
				}, 4)[3]);
				bool flag = this.m_pFetchReader.StartsWith("(");
				if (flag)
				{
					this.m_pFetchReader.ReadSpecifiedLength(1);
				}
				while (this.m_pFetchReader.Available > 0L)
				{
					this.m_pFetchReader.ReadToFirstChar();
					bool flag2 = this.m_pFetchReader.StartsWith("BODY ", false);
					if (!flag2)
					{
						bool flag3 = this.m_pFetchReader.StartsWith("BODY[", false);
						if (flag3)
						{
							this.m_pFetchReader.ReadWord();
							string bodySection = this.m_pFetchReader.ReadParenthesized();
							int offset = -1;
							bool flag4 = this.m_pFetchReader.StartsWith("<");
							if (flag4)
							{
								offset = Convert.ToInt32(this.m_pFetchReader.ReadParenthesized().Split(new char[]
								{
									' '
								})[0]);
							}
							IMAP_Client_Fetch_Body_EArgs imap_Client_Fetch_Body_EArgs = new IMAP_Client_Fetch_Body_EArgs(bodySection, offset);
							this.m_pHandler.OnBody(imap_Client_Fetch_Body_EArgs);
							this.m_pFetchReader.ReadToFirstChar();
							bool flag5 = this.m_pFetchReader.StartsWith("NIL", false);
							if (flag5)
							{
								this.m_pFetchReader.ReadWord();
							}
							else
							{
								bool flag6 = this.m_pFetchReader.StartsWith("{", false);
								if (flag6)
								{
									bool flag7 = imap_Client_Fetch_Body_EArgs.Stream == null;
									if (flag7)
									{
										this.m_pImap.ReadStringLiteral(Convert.ToInt32(this.m_pFetchReader.ReadParenthesized()), new JunkingStream());
									}
									else
									{
										this.m_pImap.ReadStringLiteral(Convert.ToInt32(this.m_pFetchReader.ReadParenthesized()), imap_Client_Fetch_Body_EArgs.Stream);
									}
									this.m_pFetchReader = new StringReader(this.m_pImap.ReadLine());
								}
								else
								{
									this.m_pFetchReader.ReadWord();
								}
							}
							imap_Client_Fetch_Body_EArgs.OnStoringCompleted();
						}
						else
						{
							bool flag8 = this.m_pFetchReader.StartsWith("BODYSTRUCTURE ", false);
							if (!flag8)
							{
								bool flag9 = this.m_pFetchReader.StartsWith("ENVELOPE ", false);
								if (flag9)
								{
									this.m_pHandler.OnEnvelope(IMAP_Envelope.Parse(this));
								}
								else
								{
									bool flag10 = this.m_pFetchReader.StartsWith("FLAGS ", false);
									if (flag10)
									{
										this.m_pFetchReader.ReadWord();
										string text = this.m_pFetchReader.ReadParenthesized();
										string[] flags = new string[0];
										bool flag11 = !string.IsNullOrEmpty(text);
										if (flag11)
										{
											flags = text.Split(new char[]
											{
												' '
											});
										}
										this.m_pHandler.OnFlags(flags);
									}
									else
									{
										bool flag12 = this.m_pFetchReader.StartsWith("INTERNALDATE ", false);
										if (flag12)
										{
											this.m_pFetchReader.ReadWord();
											this.m_pHandler.OnInternalDate(IMAP_Utils.ParseDate(this.m_pFetchReader.ReadWord()));
										}
										else
										{
											bool flag13 = this.m_pFetchReader.StartsWith("RFC822 ", false);
											if (flag13)
											{
												this.m_pFetchReader.ReadWord(false, new char[]
												{
													' '
												}, false);
												this.m_pFetchReader.ReadToFirstChar();
												IMAP_Client_Fetch_Rfc822_EArgs imap_Client_Fetch_Rfc822_EArgs = new IMAP_Client_Fetch_Rfc822_EArgs();
												this.m_pHandler.OnRfc822(imap_Client_Fetch_Rfc822_EArgs);
												bool flag14 = this.m_pFetchReader.StartsWith("NIL", false);
												if (flag14)
												{
													this.m_pFetchReader.ReadWord();
												}
												else
												{
													bool flag15 = this.m_pFetchReader.StartsWith("{", false);
													if (flag15)
													{
														bool flag16 = imap_Client_Fetch_Rfc822_EArgs.Stream == null;
														if (flag16)
														{
															this.m_pImap.ReadStringLiteral(Convert.ToInt32(this.m_pFetchReader.ReadParenthesized()), new JunkingStream());
														}
														else
														{
															this.m_pImap.ReadStringLiteral(Convert.ToInt32(this.m_pFetchReader.ReadParenthesized()), imap_Client_Fetch_Rfc822_EArgs.Stream);
														}
														this.m_pFetchReader = new StringReader(this.m_pImap.ReadLine());
													}
													else
													{
														this.m_pFetchReader.ReadWord();
													}
												}
												imap_Client_Fetch_Rfc822_EArgs.OnStoringCompleted();
											}
											else
											{
												bool flag17 = this.m_pFetchReader.StartsWith("RFC822.HEADER ", false);
												if (flag17)
												{
													this.m_pFetchReader.ReadWord(false, new char[]
													{
														' '
													}, false);
													this.m_pFetchReader.ReadToFirstChar();
													bool flag18 = this.m_pFetchReader.StartsWith("NIL", false);
													string header;
													if (flag18)
													{
														this.m_pFetchReader.ReadWord();
														header = null;
													}
													else
													{
														bool flag19 = this.m_pFetchReader.StartsWith("{", false);
														if (flag19)
														{
															header = this.m_pImap.ReadStringLiteral(Convert.ToInt32(this.m_pFetchReader.ReadParenthesized()));
															this.m_pFetchReader = new StringReader(this.m_pImap.ReadLine());
														}
														else
														{
															header = this.m_pFetchReader.ReadWord();
														}
													}
													this.m_pHandler.OnRfc822Header(header);
												}
												else
												{
													bool flag20 = this.m_pFetchReader.StartsWith("RFC822.SIZE ", false);
													if (flag20)
													{
														this.m_pFetchReader.ReadWord(false, new char[]
														{
															' '
														}, false);
														this.m_pHandler.OnSize(Convert.ToInt32(this.m_pFetchReader.ReadWord()));
													}
													else
													{
														bool flag21 = this.m_pFetchReader.StartsWith("RFC822.TEXT ", false);
														if (flag21)
														{
															this.m_pFetchReader.ReadWord(false, new char[]
															{
																' '
															}, false);
															this.m_pFetchReader.ReadToFirstChar();
															bool flag22 = this.m_pFetchReader.StartsWith("NIL", false);
															string text2;
															if (flag22)
															{
																this.m_pFetchReader.ReadWord();
																text2 = null;
															}
															else
															{
																bool flag23 = this.m_pFetchReader.StartsWith("{", false);
																if (flag23)
																{
																	text2 = this.m_pImap.ReadStringLiteral(Convert.ToInt32(this.m_pFetchReader.ReadParenthesized()));
																	this.m_pFetchReader = new StringReader(this.m_pImap.ReadLine());
																}
																else
																{
																	text2 = this.m_pFetchReader.ReadWord();
																}
															}
															this.m_pHandler.OnRfc822Text(text2);
														}
														else
														{
															bool flag24 = this.m_pFetchReader.StartsWith("UID ", false);
															if (flag24)
															{
																this.m_pFetchReader.ReadWord();
																this.m_pHandler.OnUID(Convert.ToInt64(this.m_pFetchReader.ReadWord()));
															}
															else
															{
																bool flag25 = this.m_pFetchReader.StartsWith("X-GM-MSGID ", false);
																if (flag25)
																{
																	this.m_pFetchReader.ReadWord();
																	this.m_pHandler.OnX_GM_MSGID(Convert.ToUInt64(this.m_pFetchReader.ReadWord()));
																}
																else
																{
																	bool flag26 = this.m_pFetchReader.StartsWith("X-GM-THRID ", false);
																	if (flag26)
																	{
																		this.m_pFetchReader.ReadWord();
																		this.m_pHandler.OnX_GM_THRID(Convert.ToUInt64(this.m_pFetchReader.ReadWord()));
																	}
																	else
																	{
																		bool flag27 = this.m_pFetchReader.StartsWith(")", false);
																		if (flag27)
																		{
																			break;
																		}
																		throw new NotSupportedException("Not supported IMAP FETCH data-item '" + this.m_pFetchReader.ReadToEnd() + "'.");
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}

			// Token: 0x06001B25 RID: 6949 RVA: 0x000A9AFC File Offset: 0x000A8AFC
			internal StringReader GetReader()
			{
				return this.m_pFetchReader;
			}

			// Token: 0x06001B26 RID: 6950 RVA: 0x000A9B14 File Offset: 0x000A8B14
			internal string ReadString()
			{
				this.m_pFetchReader.ReadToFirstChar();
				bool flag = this.m_pFetchReader.StartsWith("NIL", false);
				string result;
				if (flag)
				{
					this.m_pFetchReader.ReadWord();
					result = null;
				}
				else
				{
					bool flag2 = this.m_pFetchReader.StartsWith("{");
					if (flag2)
					{
						string text = this.m_pImap.ReadStringLiteral(Convert.ToInt32(this.m_pFetchReader.ReadParenthesized()));
						this.m_pFetchReader = new StringReader(this.m_pImap.ReadLine());
						result = text;
					}
					else
					{
						result = MIME_Encoding_EncodedWord.DecodeS(this.m_pFetchReader.ReadWord());
					}
				}
				return result;
			}

			// Token: 0x04000C93 RID: 3219
			private IMAP_Client m_pImap = null;

			// Token: 0x04000C94 RID: 3220
			private string m_FetchLine = null;

			// Token: 0x04000C95 RID: 3221
			private StringReader m_pFetchReader = null;

			// Token: 0x04000C96 RID: 3222
			private IMAP_Client_FetchHandler m_pHandler = null;
		}
	}
}
