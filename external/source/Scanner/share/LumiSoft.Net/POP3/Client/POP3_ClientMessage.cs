using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using LumiSoft.Net.IO;

namespace LumiSoft.Net.POP3.Client
{
	// Token: 0x020000E5 RID: 229
	public class POP3_ClientMessage
	{
		// Token: 0x0600092D RID: 2349 RVA: 0x00037240 File Offset: 0x00036240
		internal POP3_ClientMessage(POP3_Client pop3, int seqNumber, int size)
		{
			this.m_Pop3Client = pop3;
			this.m_SequenceNumber = seqNumber;
			this.m_Size = size;
		}

		// Token: 0x0600092E RID: 2350 RVA: 0x00037298 File Offset: 0x00036298
		public void MarkForDeletion()
		{
			bool isDisposed = this.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool isMarkedForDeletion = this.IsMarkedForDeletion;
			if (!isMarkedForDeletion)
			{
				using (POP3_ClientMessage.MarkForDeletionAsyncOP markForDeletionAsyncOP = new POP3_ClientMessage.MarkForDeletionAsyncOP())
				{
					using (ManualResetEvent wait = new ManualResetEvent(false))
					{
						markForDeletionAsyncOP.CompletedAsync += delegate(object s1, EventArgs<POP3_ClientMessage.MarkForDeletionAsyncOP> e1)
						{
							wait.Set();
						};
						bool flag = !this.MarkForDeletionAsync(markForDeletionAsyncOP);
						if (flag)
						{
							wait.Set();
						}
						wait.WaitOne();
						wait.Close();
						bool flag2 = markForDeletionAsyncOP.Error != null;
						if (flag2)
						{
							throw markForDeletionAsyncOP.Error;
						}
					}
				}
			}
		}

		// Token: 0x0600092F RID: 2351 RVA: 0x00037390 File Offset: 0x00036390
		public bool MarkForDeletionAsync(POP3_ClientMessage.MarkForDeletionAsyncOP op)
		{
			bool isDisposed = this.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool isMarkedForDeletion = this.IsMarkedForDeletion;
			if (isMarkedForDeletion)
			{
				throw new InvalidOperationException("Message is already marked for deletion.");
			}
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

		// Token: 0x06000930 RID: 2352 RVA: 0x00037410 File Offset: 0x00036410
		public string HeaderToString()
		{
			bool isDisposed = this.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool isMarkedForDeletion = this.IsMarkedForDeletion;
			if (isMarkedForDeletion)
			{
				throw new InvalidOperationException("Can't access message, it's marked for deletion.");
			}
			return Encoding.Default.GetString(this.HeaderToByte());
		}

		// Token: 0x06000931 RID: 2353 RVA: 0x00037464 File Offset: 0x00036464
		public byte[] HeaderToByte()
		{
			bool isDisposed = this.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool isMarkedForDeletion = this.IsMarkedForDeletion;
			if (isMarkedForDeletion)
			{
				throw new InvalidOperationException("Can't access message, it's marked for deletion.");
			}
			MemoryStream memoryStream = new MemoryStream();
			this.MessageTopLinesToStream(memoryStream, 0);
			return memoryStream.ToArray();
		}

		// Token: 0x06000932 RID: 2354 RVA: 0x000374C0 File Offset: 0x000364C0
		public void HeaderToStream(Stream stream)
		{
			bool isDisposed = this.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("Argument 'stream' value can't be null.");
			}
			bool isMarkedForDeletion = this.IsMarkedForDeletion;
			if (isMarkedForDeletion)
			{
				throw new InvalidOperationException("Can't access message, it's marked for deletion.");
			}
			this.MessageTopLinesToStream(stream, 0);
		}

		// Token: 0x06000933 RID: 2355 RVA: 0x00037520 File Offset: 0x00036520
		public byte[] MessageToByte()
		{
			bool isDisposed = this.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool isMarkedForDeletion = this.IsMarkedForDeletion;
			if (isMarkedForDeletion)
			{
				throw new InvalidOperationException("Can't access message, it's marked for deletion.");
			}
			MemoryStream memoryStream = new MemoryStream();
			this.MessageToStream(memoryStream);
			return memoryStream.ToArray();
		}

		// Token: 0x06000934 RID: 2356 RVA: 0x00037578 File Offset: 0x00036578
		public void MessageToStream(Stream stream)
		{
			bool isDisposed = this.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("Argument 'stream' value can't be null.");
			}
			bool isMarkedForDeletion = this.IsMarkedForDeletion;
			if (isMarkedForDeletion)
			{
				throw new InvalidOperationException("Can't access message, it's marked for deletion.");
			}
			using (POP3_ClientMessage.MessageToStreamAsyncOP messageToStreamAsyncOP = new POP3_ClientMessage.MessageToStreamAsyncOP(stream))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					messageToStreamAsyncOP.CompletedAsync += delegate(object s1, EventArgs<POP3_ClientMessage.MessageToStreamAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag2 = !this.MessageToStreamAsync(messageToStreamAsyncOP);
					if (flag2)
					{
						wait.Set();
					}
					wait.WaitOne();
					wait.Close();
					bool flag3 = messageToStreamAsyncOP.Error != null;
					if (flag3)
					{
						throw messageToStreamAsyncOP.Error;
					}
				}
			}
		}

		// Token: 0x06000935 RID: 2357 RVA: 0x00037694 File Offset: 0x00036694
		public bool MessageToStreamAsync(POP3_ClientMessage.MessageToStreamAsyncOP op)
		{
			bool isDisposed = this.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool isMarkedForDeletion = this.IsMarkedForDeletion;
			if (isMarkedForDeletion)
			{
				throw new InvalidOperationException("Message is already marked for deletion.");
			}
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

		// Token: 0x06000936 RID: 2358 RVA: 0x00037714 File Offset: 0x00036714
		public byte[] MessageTopLinesToByte(int lineCount)
		{
			bool isDisposed = this.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = lineCount < 0;
			if (flag)
			{
				throw new ArgumentException("Argument 'lineCount' value must be >= 0.");
			}
			bool isMarkedForDeletion = this.IsMarkedForDeletion;
			if (isMarkedForDeletion)
			{
				throw new InvalidOperationException("Can't access message, it's marked for deletion.");
			}
			MemoryStream memoryStream = new MemoryStream();
			this.MessageTopLinesToStream(memoryStream, lineCount);
			return memoryStream.ToArray();
		}

		// Token: 0x06000937 RID: 2359 RVA: 0x00037784 File Offset: 0x00036784
		public void MessageTopLinesToStream(Stream stream, int lineCount)
		{
			bool isDisposed = this.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("Argument 'stream' value can't be null.");
			}
			bool isMarkedForDeletion = this.IsMarkedForDeletion;
			if (isMarkedForDeletion)
			{
				throw new InvalidOperationException("Can't access message, it's marked for deletion.");
			}
			using (POP3_ClientMessage.MessageTopLinesToStreamAsyncOP messageTopLinesToStreamAsyncOP = new POP3_ClientMessage.MessageTopLinesToStreamAsyncOP(stream, lineCount))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					messageTopLinesToStreamAsyncOP.CompletedAsync += delegate(object s1, EventArgs<POP3_ClientMessage.MessageTopLinesToStreamAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag2 = !this.MessageTopLinesToStreamAsync(messageTopLinesToStreamAsyncOP);
					if (flag2)
					{
						wait.Set();
					}
					wait.WaitOne();
					wait.Close();
					bool flag3 = messageTopLinesToStreamAsyncOP.Error != null;
					if (flag3)
					{
						throw messageTopLinesToStreamAsyncOP.Error;
					}
				}
			}
		}

		// Token: 0x06000938 RID: 2360 RVA: 0x000378A0 File Offset: 0x000368A0
		public bool MessageTopLinesToStreamAsync(POP3_ClientMessage.MessageTopLinesToStreamAsyncOP op)
		{
			bool isDisposed = this.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool isMarkedForDeletion = this.IsMarkedForDeletion;
			if (isMarkedForDeletion)
			{
				throw new InvalidOperationException("Message is already marked for deletion.");
			}
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

		// Token: 0x06000939 RID: 2361 RVA: 0x00037920 File Offset: 0x00036920
		internal void Dispose()
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				this.m_IsDisposed = true;
				this.m_Pop3Client = null;
			}
		}

		// Token: 0x0600093A RID: 2362 RVA: 0x00037949 File Offset: 0x00036949
		internal void SetUID(string uid)
		{
			this.m_UID = uid;
		}

		// Token: 0x0600093B RID: 2363 RVA: 0x00037953 File Offset: 0x00036953
		internal void SetMarkedForDeletion(bool isMarkedForDeletion)
		{
			this.m_IsMarkedForDeletion = isMarkedForDeletion;
		}

		// Token: 0x1700031F RID: 799
		// (get) Token: 0x0600093C RID: 2364 RVA: 0x00037960 File Offset: 0x00036960
		public bool IsDisposed
		{
			get
			{
				return this.m_IsDisposed;
			}
		}

		// Token: 0x17000320 RID: 800
		// (get) Token: 0x0600093D RID: 2365 RVA: 0x00037978 File Offset: 0x00036978
		public int SequenceNumber
		{
			get
			{
				bool isDisposed = this.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_SequenceNumber;
			}
		}

		// Token: 0x17000321 RID: 801
		// (get) Token: 0x0600093E RID: 2366 RVA: 0x000379AC File Offset: 0x000369AC
		public string UID
		{
			get
			{
				bool isDisposed = this.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = !this.m_Pop3Client.IsUidlSupported;
				if (flag)
				{
					throw new NotSupportedException("POP3 server doesn't support UIDL command.");
				}
				return this.m_UID;
			}
		}

		// Token: 0x17000322 RID: 802
		// (get) Token: 0x0600093F RID: 2367 RVA: 0x00037A00 File Offset: 0x00036A00
		public int Size
		{
			get
			{
				bool isDisposed = this.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_Size;
			}
		}

		// Token: 0x17000323 RID: 803
		// (get) Token: 0x06000940 RID: 2368 RVA: 0x00037A34 File Offset: 0x00036A34
		public bool IsMarkedForDeletion
		{
			get
			{
				bool isDisposed = this.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_IsMarkedForDeletion;
			}
		}

		// Token: 0x04000420 RID: 1056
		private POP3_Client m_Pop3Client = null;

		// Token: 0x04000421 RID: 1057
		private int m_SequenceNumber = 1;

		// Token: 0x04000422 RID: 1058
		private string m_UID = "";

		// Token: 0x04000423 RID: 1059
		private int m_Size = 0;

		// Token: 0x04000424 RID: 1060
		private bool m_IsMarkedForDeletion = false;

		// Token: 0x04000425 RID: 1061
		private bool m_IsDisposed = false;

		// Token: 0x020002A3 RID: 675
		public class MarkForDeletionAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x06001785 RID: 6021 RVA: 0x00090E9C File Offset: 0x0008FE9C
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pOwner = null;
					this.m_pPop3Client = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x06001786 RID: 6022 RVA: 0x00090EE0 File Offset: 0x0008FEE0
			internal bool Start(POP3_ClientMessage owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pOwner = owner;
				this.m_pPop3Client = owner.m_Pop3Client;
				this.SetState(AsyncOP_State.Active);
				try
				{
					byte[] bytes = Encoding.UTF8.GetBytes("DELE " + owner.SequenceNumber.ToString() + "\r\n");
					this.m_pPop3Client.LogAddWrite((long)bytes.Length, "DELE " + owner.SequenceNumber.ToString());
					this.m_pPop3Client.TcpStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(this.DeleCommandSendingCompleted), null);
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
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

			// Token: 0x06001787 RID: 6023 RVA: 0x0009101C File Offset: 0x0009001C
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

			// Token: 0x06001788 RID: 6024 RVA: 0x00091094 File Offset: 0x00090094
			private void DeleCommandSendingCompleted(IAsyncResult ar)
			{
				try
				{
					this.m_pPop3Client.TcpStream.EndWrite(ar);
					SmartStream.ReadLineAsyncOP op = new SmartStream.ReadLineAsyncOP(new byte[8000], SizeExceededAction.JunkAndThrowException);
					op.CompletedAsync += delegate(object s, EventArgs<SmartStream.ReadLineAsyncOP> e)
					{
						this.DeleReadResponseCompleted(op);
					};
					bool flag = this.m_pPop3Client.TcpStream.ReadLine(op, true);
					if (flag)
					{
						this.DeleReadResponseCompleted(op);
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x06001789 RID: 6025 RVA: 0x00091160 File Offset: 0x00090160
			private void DeleReadResponseCompleted(SmartStream.ReadLineAsyncOP op)
			{
				try
				{
					bool flag = op.Error != null;
					if (flag)
					{
						this.m_pException = op.Error;
						this.m_pPop3Client.LogAddException("Exception: " + op.Error.Message, op.Error);
						this.SetState(AsyncOP_State.Completed);
					}
					else
					{
						this.m_pPop3Client.LogAddRead((long)op.BytesInBuffer, op.LineUtf8);
						bool flag2 = string.Equals(op.LineUtf8.Split(new char[]
						{
							' '
						}, 2)[0], "+OK", StringComparison.InvariantCultureIgnoreCase);
						if (flag2)
						{
							this.m_pOwner.m_IsMarkedForDeletion = true;
							this.SetState(AsyncOP_State.Completed);
						}
						else
						{
							this.m_pException = new POP3_ClientException(op.LineUtf8);
							this.SetState(AsyncOP_State.Completed);
						}
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
				op.Dispose();
			}

			// Token: 0x170007C7 RID: 1991
			// (get) Token: 0x0600178A RID: 6026 RVA: 0x0009127C File Offset: 0x0009027C
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x170007C8 RID: 1992
			// (get) Token: 0x0600178B RID: 6027 RVA: 0x00091294 File Offset: 0x00090294
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

			// Token: 0x140000A3 RID: 163
			// (add) Token: 0x0600178C RID: 6028 RVA: 0x000912E8 File Offset: 0x000902E8
			// (remove) Token: 0x0600178D RID: 6029 RVA: 0x00091320 File Offset: 0x00090320
			
			public event EventHandler<EventArgs<POP3_ClientMessage.MarkForDeletionAsyncOP>> CompletedAsync = null;

			// Token: 0x0600178E RID: 6030 RVA: 0x00091358 File Offset: 0x00090358
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<POP3_ClientMessage.MarkForDeletionAsyncOP>(this));
				}
			}

			// Token: 0x040009D0 RID: 2512
			private object m_pLock = new object();

			// Token: 0x040009D1 RID: 2513
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x040009D2 RID: 2514
			private Exception m_pException = null;

			// Token: 0x040009D3 RID: 2515
			private POP3_ClientMessage m_pOwner = null;

			// Token: 0x040009D4 RID: 2516
			private POP3_Client m_pPop3Client = null;

			// Token: 0x040009D5 RID: 2517
			private bool m_RiseCompleted = false;
		}

		// Token: 0x020002A4 RID: 676
		public class MessageToStreamAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x0600178F RID: 6031 RVA: 0x00091388 File Offset: 0x00090388
			public MessageToStreamAsyncOP(Stream stream)
			{
				bool flag = stream == null;
				if (flag)
				{
					throw new ArgumentNullException("stream");
				}
				this.m_pStream = stream;
			}

			// Token: 0x06001790 RID: 6032 RVA: 0x000913F4 File Offset: 0x000903F4
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pOwner = null;
					this.m_pPop3Client = null;
					this.m_pStream = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x06001791 RID: 6033 RVA: 0x00091440 File Offset: 0x00090440
			internal bool Start(POP3_ClientMessage owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pOwner = owner;
				this.m_pPop3Client = owner.m_Pop3Client;
				this.SetState(AsyncOP_State.Active);
				try
				{
					byte[] bytes = Encoding.UTF8.GetBytes("RETR " + owner.SequenceNumber.ToString() + "\r\n");
					this.m_pPop3Client.LogAddWrite((long)bytes.Length, "RETR " + owner.SequenceNumber.ToString());
					this.m_pPop3Client.TcpStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(this.RetrCommandSendingCompleted), null);
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
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

			// Token: 0x06001792 RID: 6034 RVA: 0x0009157C File Offset: 0x0009057C
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

			// Token: 0x06001793 RID: 6035 RVA: 0x000915F4 File Offset: 0x000905F4
			private void RetrCommandSendingCompleted(IAsyncResult ar)
			{
				try
				{
					this.m_pPop3Client.TcpStream.EndWrite(ar);
					SmartStream.ReadLineAsyncOP op = new SmartStream.ReadLineAsyncOP(new byte[8000], SizeExceededAction.JunkAndThrowException);
					op.CompletedAsync += delegate(object s, EventArgs<SmartStream.ReadLineAsyncOP> e)
					{
						this.RetrReadResponseCompleted(op);
					};
					bool flag = this.m_pPop3Client.TcpStream.ReadLine(op, true);
					if (flag)
					{
						this.RetrReadResponseCompleted(op);
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x06001794 RID: 6036 RVA: 0x000916C0 File Offset: 0x000906C0
			private void RetrReadResponseCompleted(SmartStream.ReadLineAsyncOP op)
			{
				try
				{
					bool flag = op.Error != null;
					if (flag)
					{
						this.m_pException = op.Error;
						this.m_pPop3Client.LogAddException("Exception: " + op.Error.Message, op.Error);
						this.SetState(AsyncOP_State.Completed);
					}
					else
					{
						this.m_pPop3Client.LogAddRead((long)op.BytesInBuffer, op.LineUtf8);
						bool flag2 = string.Equals(op.LineUtf8.Split(new char[]
						{
							' '
						}, 2)[0], "+OK", StringComparison.InvariantCultureIgnoreCase);
						if (flag2)
						{
							SmartStream.ReadPeriodTerminatedAsyncOP readMsgOP = new SmartStream.ReadPeriodTerminatedAsyncOP(this.m_pStream, long.MaxValue, SizeExceededAction.ThrowException);
							readMsgOP.CompletedAsync += delegate(object sender, EventArgs<SmartStream.ReadPeriodTerminatedAsyncOP> e)
							{
								this.MessageReadingCompleted(readMsgOP);
							};
							bool flag3 = this.m_pPop3Client.TcpStream.ReadPeriodTerminated(readMsgOP, true);
							if (flag3)
							{
								this.MessageReadingCompleted(readMsgOP);
							}
						}
						else
						{
							this.m_pException = new POP3_ClientException(op.LineUtf8);
							this.SetState(AsyncOP_State.Completed);
						}
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
				op.Dispose();
			}

			// Token: 0x06001795 RID: 6037 RVA: 0x00091848 File Offset: 0x00090848
			private void MessageReadingCompleted(SmartStream.ReadPeriodTerminatedAsyncOP op)
			{
				try
				{
					bool flag = op.Error != null;
					if (flag)
					{
						this.m_pException = op.Error;
						this.m_pPop3Client.LogAddException("Exception: " + op.Error.Message, op.Error);
						this.SetState(AsyncOP_State.Completed);
					}
					else
					{
						this.m_pPop3Client.LogAddRead(op.BytesStored, "Readed period-terminated message " + op.BytesStored.ToString() + " bytes.");
						this.SetState(AsyncOP_State.Completed);
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x170007C9 RID: 1993
			// (get) Token: 0x06001796 RID: 6038 RVA: 0x00091920 File Offset: 0x00090920
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x170007CA RID: 1994
			// (get) Token: 0x06001797 RID: 6039 RVA: 0x00091938 File Offset: 0x00090938
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

			// Token: 0x140000A4 RID: 164
			// (add) Token: 0x06001798 RID: 6040 RVA: 0x0009198C File Offset: 0x0009098C
			// (remove) Token: 0x06001799 RID: 6041 RVA: 0x000919C4 File Offset: 0x000909C4
			
			public event EventHandler<EventArgs<POP3_ClientMessage.MessageToStreamAsyncOP>> CompletedAsync = null;

			// Token: 0x0600179A RID: 6042 RVA: 0x000919FC File Offset: 0x000909FC
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<POP3_ClientMessage.MessageToStreamAsyncOP>(this));
				}
			}

			// Token: 0x040009D7 RID: 2519
			private object m_pLock = new object();

			// Token: 0x040009D8 RID: 2520
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x040009D9 RID: 2521
			private Exception m_pException = null;

			// Token: 0x040009DA RID: 2522
			private POP3_ClientMessage m_pOwner = null;

			// Token: 0x040009DB RID: 2523
			private POP3_Client m_pPop3Client = null;

			// Token: 0x040009DC RID: 2524
			private bool m_RiseCompleted = false;

			// Token: 0x040009DD RID: 2525
			private Stream m_pStream = null;
		}

		// Token: 0x020002A5 RID: 677
		public class MessageTopLinesToStreamAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x0600179B RID: 6043 RVA: 0x00091A2C File Offset: 0x00090A2C
			public MessageTopLinesToStreamAsyncOP(Stream stream, int lineCount)
			{
				bool flag = stream == null;
				if (flag)
				{
					throw new ArgumentNullException("stream");
				}
				bool flag2 = lineCount < 0;
				if (flag2)
				{
					throw new ArgumentException("Argument 'lineCount' value must be >= 0.", "lineCount");
				}
				this.m_pStream = stream;
				this.m_LineCount = lineCount;
			}

			// Token: 0x0600179C RID: 6044 RVA: 0x00091AC0 File Offset: 0x00090AC0
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pOwner = null;
					this.m_pPop3Client = null;
					this.m_pStream = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x0600179D RID: 6045 RVA: 0x00091B0C File Offset: 0x00090B0C
			internal bool Start(POP3_ClientMessage owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pOwner = owner;
				this.m_pPop3Client = owner.m_Pop3Client;
				this.SetState(AsyncOP_State.Active);
				try
				{
					byte[] bytes = Encoding.UTF8.GetBytes(string.Concat(new string[]
					{
						"TOP ",
						owner.SequenceNumber.ToString(),
						" ",
						this.m_LineCount.ToString(),
						"\r\n"
					}));
					this.m_pPop3Client.LogAddWrite((long)bytes.Length, "TOP " + owner.SequenceNumber.ToString() + " " + this.m_LineCount.ToString());
					this.m_pPop3Client.TcpStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(this.TopCommandSendingCompleted), null);
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
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

			// Token: 0x0600179E RID: 6046 RVA: 0x00091C7C File Offset: 0x00090C7C
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

			// Token: 0x0600179F RID: 6047 RVA: 0x00091CF4 File Offset: 0x00090CF4
			private void TopCommandSendingCompleted(IAsyncResult ar)
			{
				try
				{
					this.m_pPop3Client.TcpStream.EndWrite(ar);
					SmartStream.ReadLineAsyncOP op = new SmartStream.ReadLineAsyncOP(new byte[8000], SizeExceededAction.JunkAndThrowException);
					op.CompletedAsync += delegate(object s, EventArgs<SmartStream.ReadLineAsyncOP> e)
					{
						this.TopReadResponseCompleted(op);
					};
					bool flag = this.m_pPop3Client.TcpStream.ReadLine(op, true);
					if (flag)
					{
						this.TopReadResponseCompleted(op);
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x060017A0 RID: 6048 RVA: 0x00091DC0 File Offset: 0x00090DC0
			private void TopReadResponseCompleted(SmartStream.ReadLineAsyncOP op)
			{
				try
				{
					bool flag = op.Error != null;
					if (flag)
					{
						this.m_pException = op.Error;
						this.m_pPop3Client.LogAddException("Exception: " + op.Error.Message, op.Error);
						this.SetState(AsyncOP_State.Completed);
					}
					else
					{
						this.m_pPop3Client.LogAddRead((long)op.BytesInBuffer, op.LineUtf8);
						bool flag2 = string.Equals(op.LineUtf8.Split(new char[]
						{
							' '
						}, 2)[0], "+OK", StringComparison.InvariantCultureIgnoreCase);
						if (flag2)
						{
							SmartStream.ReadPeriodTerminatedAsyncOP readMsgOP = new SmartStream.ReadPeriodTerminatedAsyncOP(this.m_pStream, long.MaxValue, SizeExceededAction.ThrowException);
							readMsgOP.CompletedAsync += delegate(object sender, EventArgs<SmartStream.ReadPeriodTerminatedAsyncOP> e)
							{
								this.MessageReadingCompleted(readMsgOP);
							};
							bool flag3 = this.m_pPop3Client.TcpStream.ReadPeriodTerminated(readMsgOP, true);
							if (flag3)
							{
								this.MessageReadingCompleted(readMsgOP);
							}
						}
						else
						{
							this.m_pException = new POP3_ClientException(op.LineUtf8);
							this.SetState(AsyncOP_State.Completed);
						}
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
				op.Dispose();
			}

			// Token: 0x060017A1 RID: 6049 RVA: 0x00091F48 File Offset: 0x00090F48
			private void MessageReadingCompleted(SmartStream.ReadPeriodTerminatedAsyncOP op)
			{
				try
				{
					bool flag = op.Error != null;
					if (flag)
					{
						this.m_pException = op.Error;
						this.m_pPop3Client.LogAddException("Exception: " + op.Error.Message, op.Error);
						this.SetState(AsyncOP_State.Completed);
					}
					else
					{
						this.m_pPop3Client.LogAddRead(op.BytesStored, "Readed period-terminated message " + op.BytesStored.ToString() + " bytes.");
						this.SetState(AsyncOP_State.Completed);
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x170007CB RID: 1995
			// (get) Token: 0x060017A2 RID: 6050 RVA: 0x00092020 File Offset: 0x00091020
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x170007CC RID: 1996
			// (get) Token: 0x060017A3 RID: 6051 RVA: 0x00092038 File Offset: 0x00091038
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

			// Token: 0x140000A5 RID: 165
			// (add) Token: 0x060017A4 RID: 6052 RVA: 0x0009208C File Offset: 0x0009108C
			// (remove) Token: 0x060017A5 RID: 6053 RVA: 0x000920C4 File Offset: 0x000910C4
			
			public event EventHandler<EventArgs<POP3_ClientMessage.MessageTopLinesToStreamAsyncOP>> CompletedAsync = null;

			// Token: 0x060017A6 RID: 6054 RVA: 0x000920FC File Offset: 0x000910FC
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<POP3_ClientMessage.MessageTopLinesToStreamAsyncOP>(this));
				}
			}

			// Token: 0x040009DF RID: 2527
			private object m_pLock = new object();

			// Token: 0x040009E0 RID: 2528
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x040009E1 RID: 2529
			private Exception m_pException = null;

			// Token: 0x040009E2 RID: 2530
			private POP3_ClientMessage m_pOwner = null;

			// Token: 0x040009E3 RID: 2531
			private POP3_Client m_pPop3Client = null;

			// Token: 0x040009E4 RID: 2532
			private bool m_RiseCompleted = false;

			// Token: 0x040009E5 RID: 2533
			private Stream m_pStream = null;

			// Token: 0x040009E6 RID: 2534
			private int m_LineCount = 0;
		}
	}
}
