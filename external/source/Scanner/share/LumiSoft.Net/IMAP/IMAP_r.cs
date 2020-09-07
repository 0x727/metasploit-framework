using System;
using System.IO;
using System.Text;
using LumiSoft.Net.IMAP.Server;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001F9 RID: 505
	public abstract class IMAP_r
	{
		// Token: 0x060011FA RID: 4602 RVA: 0x0006CD30 File Offset: 0x0006BD30
		public virtual string ToString(IMAP_Mailbox_Encoding encoding)
		{
			return this.ToString();
		}

		// Token: 0x060011FB RID: 4603 RVA: 0x0006CD48 File Offset: 0x0006BD48
		public bool ToStreamAsync(Stream stream, IMAP_Mailbox_Encoding mailboxEncoding, EventHandler<EventArgs<Exception>> completedAsyncCallback)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			return this.ToStreamAsync(null, stream, mailboxEncoding, completedAsyncCallback);
		}

		// Token: 0x060011FC RID: 4604 RVA: 0x0006CD78 File Offset: 0x0006BD78
		internal bool SendAsync(IMAP_Session session, EventHandler<EventArgs<Exception>> completedAsyncCallback)
		{
			bool flag = session == null;
			if (flag)
			{
				throw new ArgumentNullException("session");
			}
			return this.ToStreamAsync(session, session.TcpStream, session.MailboxEncoding, completedAsyncCallback);
		}

		// Token: 0x060011FD RID: 4605 RVA: 0x0006CDB4 File Offset: 0x0006BDB4
		protected virtual bool ToStreamAsync(IMAP_Session session, Stream stream, IMAP_Mailbox_Encoding mailboxEncoding, EventHandler<EventArgs<Exception>> completedAsyncCallback)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			string text = this.ToString(mailboxEncoding);
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			bool flag2 = session != null;
			if (flag2)
			{
				session.LogAddWrite((long)bytes.Length, text.TrimEnd(new char[0]));
			}
			IAsyncResult asyncResult = stream.BeginWrite(bytes, 0, bytes.Length, delegate(IAsyncResult r)
			{
				bool completedSynchronously2 = r.CompletedSynchronously;
				if (!completedSynchronously2)
				{
					try
					{
						stream.EndWrite(r);
						bool flag3 = completedAsyncCallback != null;
						if (flag3)
						{
							completedAsyncCallback(this, new EventArgs<Exception>(null));
						}
					}
					catch (Exception value)
					{
						bool flag4 = completedAsyncCallback != null;
						if (flag4)
						{
							completedAsyncCallback(this, new EventArgs<Exception>(value));
						}
					}
				}
			}, null);
			bool completedSynchronously = asyncResult.CompletedSynchronously;
			bool result;
			if (completedSynchronously)
			{
				stream.EndWrite(asyncResult);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}
	}
}
