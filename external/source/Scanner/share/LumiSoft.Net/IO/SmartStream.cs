using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace LumiSoft.Net.IO
{
	// Token: 0x02000123 RID: 291
	public class SmartStream : Stream
	{
		// Token: 0x06000B89 RID: 2953 RVA: 0x00046C18 File Offset: 0x00045C18
		public SmartStream(Stream stream, bool owner)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			this.m_pStream = stream;
			this.m_IsOwner = owner;
			this.m_pReadBuffer = new byte[this.m_BufferSize];
			this.m_pReadBufferOP = new SmartStream.BufferReadAsyncOP(this);
			this.m_LastActivity = DateTime.Now;
		}

		// Token: 0x06000B8A RID: 2954 RVA: 0x00046CD8 File Offset: 0x00045CD8
		public new void Dispose()
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				this.m_IsDisposed = true;
				bool flag = this.m_pReadBufferOP != null;
				if (flag)
				{
					this.m_pReadBufferOP.Dispose();
				}
				this.m_pReadBufferOP = null;
				bool isOwner = this.m_IsOwner;
				if (isOwner)
				{
					this.m_pStream.Dispose();
				}
			}
		}

		// Token: 0x06000B8B RID: 2955 RVA: 0x00046D34 File Offset: 0x00045D34
		public bool ReadLine(SmartStream.ReadLineAsyncOP op, bool async)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = op == null;
			if (flag)
			{
				throw new ArgumentNullException("op");
			}
			return !op.Start(async, this);
		}

		// Token: 0x06000B8C RID: 2956 RVA: 0x00046D84 File Offset: 0x00045D84
		public IAsyncResult BeginReadHeader(Stream storeStream, int maxCount, SizeExceededAction exceededAction, AsyncCallback callback, object state)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = storeStream == null;
			if (flag)
			{
				throw new ArgumentNullException("storeStream");
			}
			bool flag2 = maxCount < 0;
			if (flag2)
			{
				throw new ArgumentException("Argument 'maxCount' must be >= 0.");
			}
			return new SmartStream.ReadToTerminatorAsyncOperation(this, "", storeStream, (long)maxCount, exceededAction, callback, state);
		}

		// Token: 0x06000B8D RID: 2957 RVA: 0x00046DF0 File Offset: 0x00045DF0
		public int EndReadHeader(IAsyncResult asyncResult)
		{
			bool flag = asyncResult == null;
			if (flag)
			{
				throw new ArgumentNullException("asyncResult");
			}
			bool flag2 = !(asyncResult is SmartStream.ReadToTerminatorAsyncOperation);
			if (flag2)
			{
				throw new ArgumentException("Argument 'asyncResult' was not returned by a call to the BeginReadHeader method.");
			}
			SmartStream.ReadToTerminatorAsyncOperation readToTerminatorAsyncOperation = (SmartStream.ReadToTerminatorAsyncOperation)asyncResult;
			bool isEndCalled = readToTerminatorAsyncOperation.IsEndCalled;
			if (isEndCalled)
			{
				throw new InvalidOperationException("EndReadHeader is already called for specified 'asyncResult'.");
			}
			readToTerminatorAsyncOperation.AsyncWaitHandle.WaitOne();
			readToTerminatorAsyncOperation.AsyncWaitHandle.Close();
			readToTerminatorAsyncOperation.IsEndCalled = true;
			bool flag3 = readToTerminatorAsyncOperation.Exception != null;
			if (flag3)
			{
				throw readToTerminatorAsyncOperation.Exception;
			}
			return (int)readToTerminatorAsyncOperation.BytesStored;
		}

		// Token: 0x06000B8E RID: 2958 RVA: 0x00046E90 File Offset: 0x00045E90
		public int ReadHeader(Stream storeStream, int maxCount, SizeExceededAction exceededAction)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = storeStream == null;
			if (flag)
			{
				throw new ArgumentNullException("storeStream");
			}
			bool flag2 = maxCount < 0;
			if (flag2)
			{
				throw new ArgumentException("Argument 'maxCount' must be >= 0.");
			}
			IAsyncResult asyncResult = this.BeginReadHeader(storeStream, maxCount, exceededAction, null, null);
			return this.EndReadHeader(asyncResult);
		}

		// Token: 0x06000B8F RID: 2959 RVA: 0x00046EFC File Offset: 0x00045EFC
		public bool ReadPeriodTerminated(SmartStream.ReadPeriodTerminatedAsyncOP op, bool async)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = op == null;
			if (flag)
			{
				throw new ArgumentNullException("op");
			}
			return !op.Start(async, this);
		}

		// Token: 0x06000B90 RID: 2960 RVA: 0x00046F4C File Offset: 0x00045F4C
		public IAsyncResult BeginReadFixedCount(Stream storeStream, long count, AsyncCallback callback, object state)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = storeStream == null;
			if (flag)
			{
				throw new ArgumentNullException("storeStream");
			}
			bool flag2 = count < 0L;
			if (flag2)
			{
				throw new ArgumentException("Argument 'count' value must be >= 0.");
			}
			return new SmartStream.ReadToStreamAsyncOperation(this, storeStream, count, callback, state);
		}

		// Token: 0x06000B91 RID: 2961 RVA: 0x00046FB0 File Offset: 0x00045FB0
		public void EndReadFixedCount(IAsyncResult asyncResult)
		{
			bool flag = asyncResult == null;
			if (flag)
			{
				throw new ArgumentNullException("asyncResult");
			}
			bool flag2 = !(asyncResult is SmartStream.ReadToStreamAsyncOperation);
			if (flag2)
			{
				throw new ArgumentException("Argument 'asyncResult' was not returned by a call to the BeginReadFixedCount method.");
			}
			SmartStream.ReadToStreamAsyncOperation readToStreamAsyncOperation = (SmartStream.ReadToStreamAsyncOperation)asyncResult;
			bool isEndCalled = readToStreamAsyncOperation.IsEndCalled;
			if (isEndCalled)
			{
				throw new InvalidOperationException("EndReadFixedCount is already called for specified 'asyncResult'.");
			}
			readToStreamAsyncOperation.AsyncWaitHandle.WaitOne();
			readToStreamAsyncOperation.AsyncWaitHandle.Close();
			readToStreamAsyncOperation.IsEndCalled = true;
			bool flag3 = readToStreamAsyncOperation.Exception != null;
			if (flag3)
			{
				throw readToStreamAsyncOperation.Exception;
			}
		}

		// Token: 0x06000B92 RID: 2962 RVA: 0x00047044 File Offset: 0x00046044
		public void ReadFixedCount(Stream storeStream, long count)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = storeStream == null;
			if (flag)
			{
				throw new ArgumentNullException("storeStream");
			}
			bool flag2 = count < 0L;
			if (flag2)
			{
				throw new ArgumentException("Argument 'count' value must be >= 0.");
			}
			IAsyncResult asyncResult = this.BeginReadFixedCount(storeStream, count, null, null);
			this.EndReadFixedCount(asyncResult);
		}

		// Token: 0x06000B93 RID: 2963 RVA: 0x000470AC File Offset: 0x000460AC
		public string ReadFixedCountString(int count)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = count < 0;
			if (flag)
			{
				throw new ArgumentException("Argument 'count' value must be >= 0.");
			}
			MemoryStream memoryStream = new MemoryStream();
			this.ReadFixedCount(memoryStream, (long)count);
			return this.m_pEncoding.GetString(memoryStream.ToArray());
		}

		// Token: 0x06000B94 RID: 2964 RVA: 0x00047110 File Offset: 0x00046110
		public void ReadAll(Stream stream)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			byte[] array = new byte[this.m_BufferSize];
			for (;;)
			{
				int num = this.Read(array, 0, array.Length);
				bool flag2 = num == 0;
				if (flag2)
				{
					break;
				}
				stream.Write(array, 0, num);
			}
		}

		// Token: 0x06000B95 RID: 2965 RVA: 0x00047188 File Offset: 0x00046188
		public int Peek()
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = this.BytesInReadBuffer == 0;
			if (flag)
			{
				this.BufferRead(false, null);
			}
			bool flag2 = this.BytesInReadBuffer == 0;
			int result;
			if (flag2)
			{
				result = -1;
			}
			else
			{
				result = (int)this.m_pReadBuffer[this.m_ReadBufferOffset];
			}
			return result;
		}

		// Token: 0x06000B96 RID: 2966 RVA: 0x000471F0 File Offset: 0x000461F0
		public void Write(string data)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			byte[] bytes = Encoding.Default.GetBytes(data);
			this.Write(bytes, 0, bytes.Length);
			this.Flush();
		}

		// Token: 0x06000B97 RID: 2967 RVA: 0x00047250 File Offset: 0x00046250
		public int WriteLine(string line)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = line == null;
			if (flag)
			{
				throw new ArgumentNullException("line");
			}
			bool flag2 = !line.EndsWith("\r\n");
			if (flag2)
			{
				line += "\r\n";
			}
			byte[] bytes = this.m_pEncoding.GetBytes(line);
			this.Write(bytes, 0, bytes.Length);
			this.Flush();
			return bytes.Length;
		}

		// Token: 0x06000B98 RID: 2968 RVA: 0x000472D8 File Offset: 0x000462D8
		public void WriteStream(Stream stream)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			byte[] array = new byte[this.m_BufferSize];
			for (;;)
			{
				int num = stream.Read(array, 0, array.Length);
				bool flag2 = num == 0;
				if (flag2)
				{
					break;
				}
				this.Write(array, 0, num);
			}
			this.Flush();
		}

		// Token: 0x06000B99 RID: 2969 RVA: 0x00047358 File Offset: 0x00046358
		public void WriteStream(Stream stream, long count)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			bool flag2 = count < 0L;
			if (flag2)
			{
				throw new ArgumentException("Argument 'count' value must be >= 0.");
			}
			byte[] array = new byte[this.m_BufferSize];
			long num = 0L;
			while (num < count)
			{
				int num2 = stream.Read(array, 0, (int)Math.Min((long)array.Length, count - num));
				num += (long)num2;
				this.Write(array, 0, num2);
			}
			this.Flush();
		}

		// Token: 0x06000B9A RID: 2970 RVA: 0x000473FC File Offset: 0x000463FC
		public bool WriteStreamAsync(SmartStream.WriteStreamAsyncOP op)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
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

		// Token: 0x06000B9B RID: 2971 RVA: 0x00047464 File Offset: 0x00046464
		public long WritePeriodTerminated(Stream stream)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			ManualResetEvent wait = new ManualResetEvent(false);
			SmartStream.WritePeriodTerminatedAsyncOP writePeriodTerminatedAsyncOP = new SmartStream.WritePeriodTerminatedAsyncOP(stream);
			writePeriodTerminatedAsyncOP.CompletedAsync += delegate(object s1, EventArgs<SmartStream.WritePeriodTerminatedAsyncOP> e1)
			{
				wait.Set();
			};
			bool flag2 = !this.WritePeriodTerminatedAsync(writePeriodTerminatedAsyncOP);
			if (flag2)
			{
				wait.Set();
			}
			wait.WaitOne();
			wait.Close();
			bool flag3 = writePeriodTerminatedAsyncOP.Error != null;
			if (flag3)
			{
				throw writePeriodTerminatedAsyncOP.Error;
			}
			return (long)writePeriodTerminatedAsyncOP.BytesWritten;
		}

		// Token: 0x06000B9C RID: 2972 RVA: 0x0004752C File Offset: 0x0004652C
		public bool WritePeriodTerminatedAsync(SmartStream.WritePeriodTerminatedAsyncOP op)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
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

		// Token: 0x06000B9D RID: 2973 RVA: 0x00047594 File Offset: 0x00046594
		public void WriteHeader(Stream stream)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			SmartStream smartStream = new SmartStream(stream, false);
			smartStream.ReadHeader(this, 0, SizeExceededAction.ThrowException);
		}

		// Token: 0x06000B9E RID: 2974 RVA: 0x000475E4 File Offset: 0x000465E4
		public override void Flush()
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("SmartStream");
			}
			this.m_pStream.Flush();
		}

		// Token: 0x06000B9F RID: 2975 RVA: 0x00047614 File Offset: 0x00046614
		public override long Seek(long offset, SeekOrigin origin)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("SmartStream");
			}
			return this.m_pStream.Seek(offset, origin);
		}

		// Token: 0x06000BA0 RID: 2976 RVA: 0x0004764C File Offset: 0x0004664C
		public override void SetLength(long value)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("SmartStream");
			}
			this.m_pStream.SetLength(value);
			this.m_ReadBufferOffset = 0;
			this.m_ReadBufferCount = 0;
		}

		// Token: 0x06000BA1 RID: 2977 RVA: 0x0004768C File Offset: 0x0004668C
		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag2 = offset < 0;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("offset", "Argument 'offset' value must be >= 0.");
			}
			bool flag3 = offset > buffer.Length;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("offset", "Argument 'offset' value must be < buffer.Length.");
			}
			bool flag4 = count < 0;
			if (flag4)
			{
				throw new ArgumentOutOfRangeException("count", "Argument 'count' value must be >= 0.");
			}
			bool flag5 = offset + count > buffer.Length;
			if (flag5)
			{
				throw new ArgumentOutOfRangeException("count", "Argument 'count' is bigger than than argument 'buffer' can store.");
			}
			return new SmartStream.ReadAsyncOperation(this, buffer, offset, count, callback, state);
		}

		// Token: 0x06000BA2 RID: 2978 RVA: 0x0004774C File Offset: 0x0004674C
		public override int EndRead(IAsyncResult asyncResult)
		{
			bool flag = asyncResult == null;
			if (flag)
			{
				throw new ArgumentNullException("asyncResult");
			}
			bool flag2 = !(asyncResult is SmartStream.ReadAsyncOperation);
			if (flag2)
			{
				throw new ArgumentException("Argument 'asyncResult' was not returned by a call to the BeginRead method.");
			}
			SmartStream.ReadAsyncOperation readAsyncOperation = (SmartStream.ReadAsyncOperation)asyncResult;
			bool isEndCalled = readAsyncOperation.IsEndCalled;
			if (isEndCalled)
			{
				throw new InvalidOperationException("EndRead is already called for specified 'asyncResult'.");
			}
			readAsyncOperation.AsyncWaitHandle.WaitOne();
			readAsyncOperation.AsyncWaitHandle.Close();
			readAsyncOperation.IsEndCalled = true;
			return readAsyncOperation.BytesStored;
		}

		// Token: 0x06000BA3 RID: 2979 RVA: 0x000477D4 File Offset: 0x000467D4
		public override int Read(byte[] buffer, int offset, int count)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("SmartStream");
			}
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag2 = offset < 0;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("offset", "Argument 'offset' value must be >= 0.");
			}
			bool flag3 = count < 0;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("count", "Argument 'count' value must be >= 0.");
			}
			bool flag4 = offset + count > buffer.Length;
			if (flag4)
			{
				throw new ArgumentOutOfRangeException("count", "Argument 'count' is bigger than than argument 'buffer' can store.");
			}
			bool flag5 = this.BytesInReadBuffer == 0;
			if (flag5)
			{
				this.BufferRead(false, null);
			}
			bool flag6 = this.BytesInReadBuffer == 0;
			int result;
			if (flag6)
			{
				result = 0;
			}
			else
			{
				int num = Math.Min(count, this.BytesInReadBuffer);
				Array.Copy(this.m_pReadBuffer, this.m_ReadBufferOffset, buffer, offset, num);
				this.m_ReadBufferOffset += num;
				result = num;
			}
			return result;
		}

		// Token: 0x06000BA4 RID: 2980 RVA: 0x000478C8 File Offset: 0x000468C8
		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("SmartStream");
			}
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag2 = offset < 0;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("offset", "Argument 'offset' value must be >= 0.");
			}
			bool flag3 = count < 0;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("count", "Argument 'count' value must be >= 0.");
			}
			bool flag4 = offset + count > buffer.Length;
			if (flag4)
			{
				throw new ArgumentOutOfRangeException("count", "Argument 'count' is bigger than than argument 'buffer' can store.");
			}
			this.m_LastActivity = DateTime.Now;
			this.m_BytesWritten += (long)count;
			return this.m_pStream.BeginWrite(buffer, offset, count, callback, state);
		}

		// Token: 0x06000BA5 RID: 2981 RVA: 0x00047984 File Offset: 0x00046984
		public override void EndWrite(IAsyncResult asyncResult)
		{
			bool flag = asyncResult == null;
			if (flag)
			{
				throw new ArgumentNullException("asyncResult");
			}
			this.m_pStream.EndWrite(asyncResult);
		}

		// Token: 0x06000BA6 RID: 2982 RVA: 0x000479B4 File Offset: 0x000469B4
		public override void Write(byte[] buffer, int offset, int count)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("SmartStream");
			}
			this.m_pStream.Write(buffer, offset, count);
			this.m_LastActivity = DateTime.Now;
			this.m_BytesWritten += (long)count;
		}

		// Token: 0x06000BA7 RID: 2983 RVA: 0x00047A04 File Offset: 0x00046A04
		private bool BufferRead(bool async, SmartStream.BufferCallback asyncCallback)
		{
			bool flag = this.BytesInReadBuffer != 0;
			if (flag)
			{
				throw new InvalidOperationException("There is already data in read buffer.");
			}
			this.m_ReadBufferOffset = 0;
			this.m_ReadBufferCount = 0;
			bool result;
			if (async)
			{
				this.m_pReadBufferOP.ReleaseEvents();
				this.m_pReadBufferOP.CompletedAsync += delegate(object s, EventArgs<SmartStream.BufferReadAsyncOP> e)
				{
					try
					{
						bool flag4 = e.Value.Error != null;
						if (flag4)
						{
							bool flag5 = asyncCallback != null;
							if (flag5)
							{
								asyncCallback(e.Value.Error);
							}
						}
						else
						{
							this.m_ReadBufferOffset = 0;
							this.m_ReadBufferCount = e.Value.BytesInBuffer;
							this.m_BytesReaded += (long)e.Value.BytesInBuffer;
							this.m_LastActivity = DateTime.Now;
							bool flag6 = asyncCallback != null;
							if (flag6)
							{
								asyncCallback(null);
							}
						}
					}
					catch (Exception x)
					{
						bool flag7 = asyncCallback != null;
						if (flag7)
						{
							asyncCallback(x);
						}
					}
				};
				bool flag2 = this.m_pReadBufferOP.Start(async, this.m_pReadBuffer, this.m_pReadBuffer.Length);
				if (flag2)
				{
					result = true;
				}
				else
				{
					bool flag3 = this.m_pReadBufferOP.Error != null;
					if (flag3)
					{
						throw this.m_pReadBufferOP.Error;
					}
					this.m_ReadBufferOffset = 0;
					this.m_ReadBufferCount = this.m_pReadBufferOP.BytesInBuffer;
					this.m_BytesReaded += (long)this.m_pReadBufferOP.BytesInBuffer;
					this.m_LastActivity = DateTime.Now;
					result = false;
				}
			}
			else
			{
				int num = this.m_pStream.Read(this.m_pReadBuffer, 0, this.m_pReadBuffer.Length);
				this.m_ReadBufferCount = num;
				this.m_BytesReaded += (long)num;
				this.m_LastActivity = DateTime.Now;
				result = false;
			}
			return result;
		}

		// Token: 0x170003C2 RID: 962
		// (get) Token: 0x06000BA8 RID: 2984 RVA: 0x00047B50 File Offset: 0x00046B50
		public bool IsDisposed
		{
			get
			{
				return this.m_IsDisposed;
			}
		}

		// Token: 0x170003C3 RID: 963
		// (get) Token: 0x06000BA9 RID: 2985 RVA: 0x00047B68 File Offset: 0x00046B68
		public int LineBufferSize
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				return this.m_BufferSize;
			}
		}

		// Token: 0x170003C4 RID: 964
		// (get) Token: 0x06000BAA RID: 2986 RVA: 0x00047B98 File Offset: 0x00046B98
		public Stream SourceStream
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				return this.m_pStream;
			}
		}

		// Token: 0x170003C5 RID: 965
		// (get) Token: 0x06000BAB RID: 2987 RVA: 0x00047BC8 File Offset: 0x00046BC8
		// (set) Token: 0x06000BAC RID: 2988 RVA: 0x00047BF8 File Offset: 0x00046BF8
		public bool IsOwner
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				return this.m_IsOwner;
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				this.m_IsOwner = value;
			}
		}

		// Token: 0x170003C6 RID: 966
		// (get) Token: 0x06000BAD RID: 2989 RVA: 0x00047C24 File Offset: 0x00046C24
		public DateTime LastActivity
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				return this.m_LastActivity;
			}
		}

		// Token: 0x170003C7 RID: 967
		// (get) Token: 0x06000BAE RID: 2990 RVA: 0x00047C54 File Offset: 0x00046C54
		public long BytesReaded
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				return this.m_BytesReaded;
			}
		}

		// Token: 0x170003C8 RID: 968
		// (get) Token: 0x06000BAF RID: 2991 RVA: 0x00047C84 File Offset: 0x00046C84
		public long BytesWritten
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				return this.m_BytesWritten;
			}
		}

		// Token: 0x170003C9 RID: 969
		// (get) Token: 0x06000BB0 RID: 2992 RVA: 0x00047CB4 File Offset: 0x00046CB4
		public int BytesInReadBuffer
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				return this.m_ReadBufferCount - this.m_ReadBufferOffset;
			}
		}

		// Token: 0x170003CA RID: 970
		// (get) Token: 0x06000BB1 RID: 2993 RVA: 0x00047CEC File Offset: 0x00046CEC
		// (set) Token: 0x06000BB2 RID: 2994 RVA: 0x00047D1C File Offset: 0x00046D1C
		public Encoding Encoding
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				return this.m_pEncoding;
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException();
				}
				this.m_pEncoding = value;
			}
		}

		// Token: 0x170003CB RID: 971
		// (get) Token: 0x06000BB3 RID: 2995 RVA: 0x00047D58 File Offset: 0x00046D58
		// (set) Token: 0x06000BB4 RID: 2996 RVA: 0x00047D70 File Offset: 0x00046D70
		public bool CRLFLines
		{
			get
			{
				return this.m_CRLFLines;
			}
			set
			{
				this.m_CRLFLines = value;
			}
		}

		// Token: 0x170003CC RID: 972
		// (get) Token: 0x06000BB5 RID: 2997 RVA: 0x00047D7C File Offset: 0x00046D7C
		public override bool CanRead
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				return this.m_pStream.CanRead;
			}
		}

		// Token: 0x170003CD RID: 973
		// (get) Token: 0x06000BB6 RID: 2998 RVA: 0x00047DB0 File Offset: 0x00046DB0
		public override bool CanSeek
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				return this.m_pStream.CanSeek;
			}
		}

		// Token: 0x170003CE RID: 974
		// (get) Token: 0x06000BB7 RID: 2999 RVA: 0x00047DE4 File Offset: 0x00046DE4
		public override bool CanWrite
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				return this.m_pStream.CanWrite;
			}
		}

		// Token: 0x170003CF RID: 975
		// (get) Token: 0x06000BB8 RID: 3000 RVA: 0x00047E18 File Offset: 0x00046E18
		public override long Length
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				return this.m_pStream.Length;
			}
		}

		// Token: 0x170003D0 RID: 976
		// (get) Token: 0x06000BB9 RID: 3001 RVA: 0x00047E4C File Offset: 0x00046E4C
		// (set) Token: 0x06000BBA RID: 3002 RVA: 0x00047E80 File Offset: 0x00046E80
		public override long Position
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				return this.m_pStream.Position;
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				this.m_pStream.Position = value;
				this.m_ReadBufferOffset = 0;
				this.m_ReadBufferCount = 0;
			}
		}

		// Token: 0x06000BBB RID: 3003 RVA: 0x00047EC0 File Offset: 0x00046EC0
		[Obsolete("Use method 'ReadLine' instead.")]
		public IAsyncResult BeginReadLine(byte[] buffer, int offset, int maxCount, SizeExceededAction exceededAction, AsyncCallback callback, object state)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag2 = offset < 0;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("offset", "Argument 'offset' value must be >= 0.");
			}
			bool flag3 = offset > buffer.Length;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("offset", "Argument 'offset' value must be < buffer.Length.");
			}
			bool flag4 = maxCount < 0;
			if (flag4)
			{
				throw new ArgumentOutOfRangeException("maxCount", "Argument 'maxCount' value must be >= 0.");
			}
			bool flag5 = offset + maxCount > buffer.Length;
			if (flag5)
			{
				throw new ArgumentOutOfRangeException("maxCount", "Argument 'maxCount' is bigger than than argument 'buffer' can store.");
			}
			return new SmartStream.ReadLineAsyncOperation(this, buffer, offset, maxCount, exceededAction, callback, state);
		}

		// Token: 0x06000BBC RID: 3004 RVA: 0x00047F84 File Offset: 0x00046F84
		[Obsolete("Use method 'ReadLine' instead.")]
		public int EndReadLine(IAsyncResult asyncResult)
		{
			bool flag = asyncResult == null;
			if (flag)
			{
				throw new ArgumentNullException("asyncResult");
			}
			bool flag2 = !(asyncResult is SmartStream.ReadLineAsyncOperation);
			if (flag2)
			{
				throw new ArgumentException("Argument 'asyncResult' was not returned by a call to the BeginReadLine method.");
			}
			SmartStream.ReadLineAsyncOperation readLineAsyncOperation = (SmartStream.ReadLineAsyncOperation)asyncResult;
			bool isEndCalled = readLineAsyncOperation.IsEndCalled;
			if (isEndCalled)
			{
				throw new InvalidOperationException("EndReadLine is already called for specified 'asyncResult'.");
			}
			readLineAsyncOperation.AsyncWaitHandle.WaitOne();
			readLineAsyncOperation.AsyncWaitHandle.Close();
			readLineAsyncOperation.IsEndCalled = true;
			bool flag3 = readLineAsyncOperation.BytesReaded == 0;
			int result;
			if (flag3)
			{
				result = -1;
			}
			else
			{
				result = readLineAsyncOperation.BytesStored;
			}
			return result;
		}

		// Token: 0x040004BE RID: 1214
		private bool m_IsDisposed = false;

		// Token: 0x040004BF RID: 1215
		private Stream m_pStream = null;

		// Token: 0x040004C0 RID: 1216
		private bool m_IsOwner = false;

		// Token: 0x040004C1 RID: 1217
		private DateTime m_LastActivity;

		// Token: 0x040004C2 RID: 1218
		private long m_BytesReaded = 0L;

		// Token: 0x040004C3 RID: 1219
		private long m_BytesWritten = 0L;

		// Token: 0x040004C4 RID: 1220
		private int m_BufferSize = 84000;

		// Token: 0x040004C5 RID: 1221
		private byte[] m_pReadBuffer = null;

		// Token: 0x040004C6 RID: 1222
		private int m_ReadBufferOffset = 0;

		// Token: 0x040004C7 RID: 1223
		private int m_ReadBufferCount = 0;

		// Token: 0x040004C8 RID: 1224
		private SmartStream.BufferReadAsyncOP m_pReadBufferOP = null;

		// Token: 0x040004C9 RID: 1225
		private Encoding m_pEncoding = Encoding.Default;

		// Token: 0x040004CA RID: 1226
		private bool m_CRLFLines = true;

		// Token: 0x020002C8 RID: 712
		// (Invoke) Token: 0x06001860 RID: 6240
		private delegate void BufferCallback(Exception x);

		// Token: 0x020002C9 RID: 713
		private class ReadAsyncOperation : IAsyncResult
		{
			// Token: 0x06001863 RID: 6243 RVA: 0x00096A80 File Offset: 0x00095A80
			public ReadAsyncOperation(SmartStream owner, byte[] buffer, int offset, int maxSize, AsyncCallback callback, object asyncState)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				bool flag2 = buffer == null;
				if (flag2)
				{
					throw new ArgumentNullException("buffer");
				}
				bool flag3 = offset < 0;
				if (flag3)
				{
					throw new ArgumentOutOfRangeException("offset", "Argument 'offset' value must be >= 0.");
				}
				bool flag4 = offset > buffer.Length;
				if (flag4)
				{
					throw new ArgumentOutOfRangeException("offset", "Argument 'offset' value must be < buffer.Length.");
				}
				bool flag5 = maxSize < 0;
				if (flag5)
				{
					throw new ArgumentOutOfRangeException("maxSize", "Argument 'maxSize' value must be >= 0.");
				}
				bool flag6 = offset + maxSize > buffer.Length;
				if (flag6)
				{
					throw new ArgumentOutOfRangeException("maxSize", "Argument 'maxSize' is bigger than than argument 'buffer' can store.");
				}
				this.m_pOwner = owner;
				this.m_pBuffer = buffer;
				this.m_OffsetInBuffer = offset;
				this.m_MaxSize = maxSize;
				this.m_pAsyncCallback = callback;
				this.m_pAsyncState = asyncState;
				this.m_pAsyncWaitHandle = new AutoResetEvent(false);
				this.DoRead();
			}

			// Token: 0x06001864 RID: 6244 RVA: 0x00096BC4 File Offset: 0x00095BC4
			private void Buffering_Completed(Exception x)
			{
				bool flag = x != null;
				if (flag)
				{
					this.m_pException = x;
					this.Completed();
				}
				else
				{
					bool flag2 = this.m_pOwner.BytesInReadBuffer == 0;
					if (flag2)
					{
						this.Completed();
					}
					else
					{
						this.DoRead();
					}
				}
			}

			// Token: 0x06001865 RID: 6245 RVA: 0x00096C14 File Offset: 0x00095C14
			private void DoRead()
			{
				try
				{
					bool flag = this.m_pOwner.BytesInReadBuffer == 0;
					if (flag)
					{
						bool flag2 = this.m_pOwner.BufferRead(true, new SmartStream.BufferCallback(this.Buffering_Completed));
						if (flag2)
						{
							return;
						}
						bool flag3 = this.m_pOwner.BytesInReadBuffer == 0;
						if (flag3)
						{
							this.Completed();
							return;
						}
					}
					int num = Math.Min(this.m_MaxSize, this.m_pOwner.BytesInReadBuffer);
					Array.Copy(this.m_pOwner.m_pReadBuffer, this.m_pOwner.m_ReadBufferOffset, this.m_pBuffer, this.m_OffsetInBuffer, num);
					this.m_pOwner.m_ReadBufferOffset += num;
					this.m_pOwner.m_LastActivity = DateTime.Now;
					this.m_BytesStored += num;
					this.Completed();
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.Completed();
				}
			}

			// Token: 0x06001866 RID: 6246 RVA: 0x00096D1C File Offset: 0x00095D1C
			private void Completed()
			{
				this.m_IsCompleted = true;
				this.m_pAsyncWaitHandle.Set();
				bool flag = this.m_pAsyncCallback != null;
				if (flag)
				{
					this.m_pAsyncCallback(this);
				}
			}

			// Token: 0x170007EA RID: 2026
			// (get) Token: 0x06001867 RID: 6247 RVA: 0x00096D5C File Offset: 0x00095D5C
			public object AsyncState
			{
				get
				{
					return this.m_pAsyncState;
				}
			}

			// Token: 0x170007EB RID: 2027
			// (get) Token: 0x06001868 RID: 6248 RVA: 0x00096D74 File Offset: 0x00095D74
			public WaitHandle AsyncWaitHandle
			{
				get
				{
					return this.m_pAsyncWaitHandle;
				}
			}

			// Token: 0x170007EC RID: 2028
			// (get) Token: 0x06001869 RID: 6249 RVA: 0x00096D8C File Offset: 0x00095D8C
			public bool CompletedSynchronously
			{
				get
				{
					return this.m_CompletedSynchronously;
				}
			}

			// Token: 0x170007ED RID: 2029
			// (get) Token: 0x0600186A RID: 6250 RVA: 0x00096DA4 File Offset: 0x00095DA4
			public bool IsCompleted
			{
				get
				{
					return this.m_IsCompleted;
				}
			}

			// Token: 0x170007EE RID: 2030
			// (get) Token: 0x0600186B RID: 6251 RVA: 0x00096DBC File Offset: 0x00095DBC
			// (set) Token: 0x0600186C RID: 6252 RVA: 0x00096DD4 File Offset: 0x00095DD4
			internal bool IsEndCalled
			{
				get
				{
					return this.m_IsEndCalled;
				}
				set
				{
					this.m_IsEndCalled = value;
				}
			}

			// Token: 0x170007EF RID: 2031
			// (get) Token: 0x0600186D RID: 6253 RVA: 0x00096DE0 File Offset: 0x00095DE0
			internal byte[] Buffer
			{
				get
				{
					return this.m_pBuffer;
				}
			}

			// Token: 0x170007F0 RID: 2032
			// (get) Token: 0x0600186E RID: 6254 RVA: 0x00096DF8 File Offset: 0x00095DF8
			internal int BytesStored
			{
				get
				{
					return this.m_BytesStored;
				}
			}

			// Token: 0x04000A5F RID: 2655
			private SmartStream m_pOwner = null;

			// Token: 0x04000A60 RID: 2656
			private byte[] m_pBuffer = null;

			// Token: 0x04000A61 RID: 2657
			private int m_OffsetInBuffer = 0;

			// Token: 0x04000A62 RID: 2658
			private int m_MaxSize = 0;

			// Token: 0x04000A63 RID: 2659
			private AsyncCallback m_pAsyncCallback = null;

			// Token: 0x04000A64 RID: 2660
			private object m_pAsyncState = null;

			// Token: 0x04000A65 RID: 2661
			private AutoResetEvent m_pAsyncWaitHandle = null;

			// Token: 0x04000A66 RID: 2662
			private bool m_CompletedSynchronously = false;

			// Token: 0x04000A67 RID: 2663
			private bool m_IsCompleted = false;

			// Token: 0x04000A68 RID: 2664
			private bool m_IsEndCalled = false;

			// Token: 0x04000A69 RID: 2665
			private int m_BytesStored = 0;

			// Token: 0x04000A6A RID: 2666
			private Exception m_pException = null;
		}

		// Token: 0x020002CA RID: 714
		public class ReadLineAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x0600186F RID: 6255 RVA: 0x00096E10 File Offset: 0x00095E10
			public ReadLineAsyncOP(byte[] buffer, SizeExceededAction exceededAction)
			{
				bool flag = buffer == null;
				if (flag)
				{
					throw new ArgumentNullException("buffer");
				}
				this.m_pBuffer = buffer;
				this.m_ExceededAction = exceededAction;
			}

			// Token: 0x06001870 RID: 6256 RVA: 0x00096E98 File Offset: 0x00095E98
			~ReadLineAsyncOP()
			{
				this.Dispose();
			}

			// Token: 0x06001871 RID: 6257 RVA: 0x00096EC8 File Offset: 0x00095EC8
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.m_State = AsyncOP_State.Disposed;
					this.m_pOwner = null;
					this.m_pBuffer = null;
					this.m_pException = null;
					this.CompletedAsync = null;
					this.Completed = null;
				}
			}

			// Token: 0x06001872 RID: 6258 RVA: 0x00096F10 File Offset: 0x00095F10
			internal bool Start(bool async, SmartStream stream)
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag2 = this.m_State == AsyncOP_State.Active;
				if (flag2)
				{
					throw new InvalidOperationException("There is existing active operation. There may be only one active operation at same time.");
				}
				bool flag3 = stream == null;
				if (flag3)
				{
					throw new ArgumentNullException("stream");
				}
				this.m_pOwner = stream;
				this.m_State = AsyncOP_State.Active;
				this.m_RiseCompleted = false;
				this.m_pException = null;
				this.m_BytesInBuffer = 0;
				this.m_LastByte = -1;
				bool flag4 = this.DoLineReading(async);
				if (flag4)
				{
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

			// Token: 0x06001873 RID: 6259 RVA: 0x00096FF4 File Offset: 0x00095FF4
			private void Buffering_Completed(Exception x)
			{
				bool flag = false;
				try
				{
					bool flag2 = x != null;
					if (flag2)
					{
						this.m_pException = x;
						flag = true;
					}
					else
					{
						bool flag3 = this.m_pOwner.BytesInReadBuffer == 0;
						if (flag3)
						{
							flag = true;
						}
						else
						{
							bool flag4 = this.DoLineReading(true);
							if (flag4)
							{
								flag = true;
							}
						}
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					flag = true;
				}
				bool flag5 = flag;
				if (flag5)
				{
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x06001874 RID: 6260 RVA: 0x00097078 File Offset: 0x00096078
			private bool DoLineReading(bool async)
			{
				try
				{
					for (;;)
					{
						bool flag = this.m_pOwner.BytesInReadBuffer == 0;
						if (flag)
						{
							bool flag2 = this.m_pOwner.BufferRead(async, new SmartStream.BufferCallback(this.Buffering_Completed));
							if (flag2)
							{
								break;
							}
							bool flag3 = this.m_pOwner.BytesInReadBuffer == 0;
							if (flag3)
							{
								goto Block_4;
							}
						}
						byte[] pReadBuffer = this.m_pOwner.m_pReadBuffer;
						SmartStream pOwner = this.m_pOwner;
						int num = pOwner.m_ReadBufferOffset;
						pOwner.m_ReadBufferOffset = num + 1;
						byte b = pReadBuffer[num];
						bool flag4 = this.m_BytesInBuffer >= this.m_pBuffer.Length;
						if (flag4)
						{
							bool flag5 = this.m_pException == null;
							if (flag5)
							{
								this.m_pException = new LineSizeExceededException();
							}
							bool flag6 = this.m_ExceededAction == SizeExceededAction.ThrowException;
							if (flag6)
							{
								goto Block_7;
							}
						}
						else
						{
							byte[] pBuffer = this.m_pBuffer;
							num = this.m_BytesInBuffer;
							this.m_BytesInBuffer = num + 1;
							pBuffer[num] = b;
						}
						bool flag7 = b == 10;
						if (flag7)
						{
							bool flag8 = !this.m_pOwner.CRLFLines || (this.m_pOwner.CRLFLines && this.m_LastByte == 13);
							if (flag8)
							{
								goto Block_11;
							}
						}
						this.m_LastByte = (int)b;
					}
					return false;
					Block_4:
					return true;
					Block_7:
					return true;
					Block_11:
					return true;
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
				}
				return true;
			}

			// Token: 0x06001875 RID: 6261 RVA: 0x000971F8 File Offset: 0x000961F8
			private void SetState(AsyncOP_State state)
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					bool riseCompleted = this.m_RiseCompleted;
					object pLock = this.m_pLock;
					lock (pLock)
					{
						this.m_State = state;
						riseCompleted = this.m_RiseCompleted;
					}
					bool flag3 = this.m_State == AsyncOP_State.Completed && riseCompleted;
					if (flag3)
					{
						this.OnCompletedAsync();
					}
				}
			}

			// Token: 0x170007F1 RID: 2033
			// (get) Token: 0x06001876 RID: 6262 RVA: 0x00097278 File Offset: 0x00096278
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x170007F2 RID: 2034
			// (get) Token: 0x06001877 RID: 6263 RVA: 0x00097290 File Offset: 0x00096290
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
						throw new InvalidOperationException("This property is only valid in AsyncOP_State.Completed state.");
					}
					return this.m_pException;
				}
			}

			// Token: 0x170007F3 RID: 2035
			// (get) Token: 0x06001878 RID: 6264 RVA: 0x000972E4 File Offset: 0x000962E4
			public SizeExceededAction SizeExceededAction
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
						throw new InvalidOperationException("This property is only valid in AsyncOP_State.Completed state.");
					}
					return this.m_ExceededAction;
				}
			}

			// Token: 0x170007F4 RID: 2036
			// (get) Token: 0x06001879 RID: 6265 RVA: 0x00097338 File Offset: 0x00096338
			public byte[] Buffer
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
						throw new InvalidOperationException("This property is only valid in AsyncOP_State.Completed state.");
					}
					return this.m_pBuffer;
				}
			}

			// Token: 0x170007F5 RID: 2037
			// (get) Token: 0x0600187A RID: 6266 RVA: 0x0009738C File Offset: 0x0009638C
			public int BytesInBuffer
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
						throw new InvalidOperationException("This property is only valid in AsyncOP_State.Completed state.");
					}
					return this.m_BytesInBuffer;
				}
			}

			// Token: 0x170007F6 RID: 2038
			// (get) Token: 0x0600187B RID: 6267 RVA: 0x000973E0 File Offset: 0x000963E0
			public int LineBytesInBuffer
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
						throw new InvalidOperationException("This property is only valid in AsyncOP_State.Completed state.");
					}
					int num = this.m_BytesInBuffer;
					bool flag3 = this.m_BytesInBuffer > 1;
					if (flag3)
					{
						bool flag4 = this.m_pBuffer[this.m_BytesInBuffer - 1] == 10;
						if (flag4)
						{
							num--;
							bool flag5 = this.m_pBuffer[this.m_BytesInBuffer - 2] == 13;
							if (flag5)
							{
								num--;
							}
						}
					}
					else
					{
						bool flag6 = this.m_BytesInBuffer > 0;
						if (flag6)
						{
							bool flag7 = this.m_pBuffer[this.m_BytesInBuffer - 1] == 10;
							if (flag7)
							{
								num--;
							}
						}
					}
					return num;
				}
			}

			// Token: 0x170007F7 RID: 2039
			// (get) Token: 0x0600187C RID: 6268 RVA: 0x000974B8 File Offset: 0x000964B8
			public string LineAscii
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
						throw new InvalidOperationException("This property is only valid in AsyncOP_State.Completed state.");
					}
					bool flag3 = this.BytesInBuffer == 0;
					string result;
					if (flag3)
					{
						result = null;
					}
					else
					{
						result = Encoding.ASCII.GetString(this.m_pBuffer, 0, this.LineBytesInBuffer);
					}
					return result;
				}
			}

			// Token: 0x170007F8 RID: 2040
			// (get) Token: 0x0600187D RID: 6269 RVA: 0x00097530 File Offset: 0x00096530
			public string LineUtf8
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
						throw new InvalidOperationException("This property is only valid in AsyncOP_State.Completed state.");
					}
					bool flag3 = this.BytesInBuffer == 0;
					string result;
					if (flag3)
					{
						result = null;
					}
					else
					{
						result = Encoding.UTF8.GetString(this.m_pBuffer, 0, this.LineBytesInBuffer);
					}
					return result;
				}
			}

			// Token: 0x170007F9 RID: 2041
			// (get) Token: 0x0600187E RID: 6270 RVA: 0x000975A8 File Offset: 0x000965A8
			public string LineUtf32
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
						throw new InvalidOperationException("This property is only valid in AsyncOP_State.Completed state.");
					}
					bool flag3 = this.BytesInBuffer == 0;
					string result;
					if (flag3)
					{
						result = null;
					}
					else
					{
						result = Encoding.UTF32.GetString(this.m_pBuffer, 0, this.LineBytesInBuffer);
					}
					return result;
				}
			}

			// Token: 0x140000AF RID: 175
			// (add) Token: 0x0600187F RID: 6271 RVA: 0x00097620 File Offset: 0x00096620
			// (remove) Token: 0x06001880 RID: 6272 RVA: 0x00097658 File Offset: 0x00096658
			
			public event EventHandler<EventArgs<SmartStream.ReadLineAsyncOP>> CompletedAsync = null;

			// Token: 0x06001881 RID: 6273 RVA: 0x00097690 File Offset: 0x00096690
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<SmartStream.ReadLineAsyncOP>(this));
				}
				bool flag2 = this.Completed != null;
				if (flag2)
				{
					this.Completed(this, new EventArgs<SmartStream.ReadLineAsyncOP>(this));
				}
			}

			// Token: 0x140000B0 RID: 176
			// (add) Token: 0x06001882 RID: 6274 RVA: 0x000976E4 File Offset: 0x000966E4
			// (remove) Token: 0x06001883 RID: 6275 RVA: 0x0009771C File Offset: 0x0009671C
			[Obsolete("Use CompletedAsync event istead.")]
			
			public event EventHandler<EventArgs<SmartStream.ReadLineAsyncOP>> Completed = null;

			// Token: 0x04000A6B RID: 2667
			private object m_pLock = new object();

			// Token: 0x04000A6C RID: 2668
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000A6D RID: 2669
			private Exception m_pException = null;

			// Token: 0x04000A6E RID: 2670
			private bool m_RiseCompleted = false;

			// Token: 0x04000A6F RID: 2671
			private SmartStream m_pOwner = null;

			// Token: 0x04000A70 RID: 2672
			private byte[] m_pBuffer = null;

			// Token: 0x04000A71 RID: 2673
			private SizeExceededAction m_ExceededAction = SizeExceededAction.JunkAndThrowException;

			// Token: 0x04000A72 RID: 2674
			private int m_BytesInBuffer = 0;

			// Token: 0x04000A73 RID: 2675
			private int m_LastByte = -1;
		}

		// Token: 0x020002CB RID: 715
		public class ReadPeriodTerminatedAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x06001884 RID: 6276 RVA: 0x00097754 File Offset: 0x00096754
			public ReadPeriodTerminatedAsyncOP(Stream stream, long maxCount, SizeExceededAction exceededAction)
			{
				bool flag = stream == null;
				if (flag)
				{
					throw new ArgumentNullException("stream");
				}
				bool flag2 = maxCount < 0L;
				if (flag2)
				{
					throw new ArgumentException("Argument 'maxCount' must be >= 0.", "maxCount");
				}
				this.m_pStream = stream;
				this.m_MaxCount = maxCount;
				this.m_ExceededAction = exceededAction;
				this.m_pReadLineOP = new SmartStream.ReadLineAsyncOP(new byte[32000], exceededAction);
				this.m_pReadLineOP.CompletedAsync += this.m_pReadLineOP_CompletedAsync;
			}

			// Token: 0x06001885 RID: 6277 RVA: 0x0009783C File Offset: 0x0009683C
			~ReadPeriodTerminatedAsyncOP()
			{
				this.Dispose();
			}

			// Token: 0x06001886 RID: 6278 RVA: 0x0009786C File Offset: 0x0009686C
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.m_State = AsyncOP_State.Disposed;
					this.m_pOwner = null;
					this.m_pStream = null;
					this.m_pReadLineOP.Dispose();
					this.m_pReadLineOP = null;
					this.m_pException = null;
					this.CompletedAsync = null;
					this.Completed = null;
				}
			}

			// Token: 0x06001887 RID: 6279 RVA: 0x000978C8 File Offset: 0x000968C8
			internal bool Start(bool async, SmartStream stream)
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag2 = this.m_State == AsyncOP_State.Active;
				if (flag2)
				{
					throw new InvalidOperationException("There is existing active operation. There may be only one active operation at same time.");
				}
				bool flag3 = stream == null;
				if (flag3)
				{
					throw new ArgumentNullException("stream");
				}
				this.m_pOwner = stream;
				this.m_State = AsyncOP_State.Active;
				this.m_RiseCompleted = false;
				this.m_pException = null;
				this.m_BytesStored = 0L;
				this.m_LinesStored = 0;
				bool flag4 = this.DoRead(async);
				if (flag4)
				{
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

			// Token: 0x06001888 RID: 6280 RVA: 0x000979AC File Offset: 0x000969AC
			private void m_pReadLineOP_CompletedAsync(object sender, EventArgs<SmartStream.ReadLineAsyncOP> e)
			{
				bool flag = false;
				try
				{
					bool flag2 = this.ProcessReadedLine();
					if (flag2)
					{
						flag = true;
					}
					else
					{
						bool flag3 = this.DoRead(true);
						if (flag3)
						{
							flag = true;
						}
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					flag = true;
				}
				bool flag4 = flag;
				if (flag4)
				{
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x06001889 RID: 6281 RVA: 0x00097A10 File Offset: 0x00096A10
			private bool DoRead(bool async)
			{
				try
				{
					while (this.m_pOwner.ReadLine(this.m_pReadLineOP, async))
					{
						bool flag = this.ProcessReadedLine();
						if (flag)
						{
							return true;
						}
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
				}
				return false;
			}

			// Token: 0x0600188A RID: 6282 RVA: 0x00097A6C File Offset: 0x00096A6C
			private bool ProcessReadedLine()
			{
				bool flag = this.m_pReadLineOP.Error != null;
				bool result;
				if (flag)
				{
					this.m_pException = this.m_pReadLineOP.Error;
					result = true;
				}
				else
				{
					bool flag2 = this.m_pReadLineOP.BytesInBuffer == 0;
					if (flag2)
					{
						this.m_pException = new IncompleteDataException("Data is not period-terminated.");
						result = true;
					}
					else
					{
						bool flag3 = this.m_pReadLineOP.LineBytesInBuffer == 1 && this.m_pReadLineOP.Buffer[0] == 46;
						if (flag3)
						{
							result = true;
						}
						else
						{
							bool flag4 = this.m_MaxCount < 1L || this.m_BytesStored + (long)this.m_pReadLineOP.BytesInBuffer < this.m_MaxCount;
							if (flag4)
							{
								bool flag5 = this.m_pReadLineOP.Buffer[0] == 46;
								if (flag5)
								{
									this.m_pStream.Write(this.m_pReadLineOP.Buffer, 1, this.m_pReadLineOP.BytesInBuffer - 1);
									this.m_BytesStored += (long)(this.m_pReadLineOP.BytesInBuffer - 1);
									this.m_LinesStored++;
								}
								else
								{
									this.m_pStream.Write(this.m_pReadLineOP.Buffer, 0, this.m_pReadLineOP.BytesInBuffer);
									this.m_BytesStored += (long)this.m_pReadLineOP.BytesInBuffer;
									this.m_LinesStored++;
								}
							}
							else
							{
								bool flag6 = this.m_ExceededAction == SizeExceededAction.ThrowException;
								if (flag6)
								{
									this.m_pException = new DataSizeExceededException();
									return true;
								}
								bool flag7 = this.m_pException == null;
								if (flag7)
								{
									this.m_pException = new DataSizeExceededException();
								}
							}
							result = false;
						}
					}
				}
				return result;
			}

			// Token: 0x0600188B RID: 6283 RVA: 0x00097C2C File Offset: 0x00096C2C
			private void SetState(AsyncOP_State state)
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					bool riseCompleted = this.m_RiseCompleted;
					object pLock = this.m_pLock;
					lock (pLock)
					{
						this.m_State = state;
						riseCompleted = this.m_RiseCompleted;
					}
					bool flag3 = this.m_State == AsyncOP_State.Completed && riseCompleted;
					if (flag3)
					{
						this.OnCompletedAsync();
					}
				}
			}

			// Token: 0x170007FA RID: 2042
			// (get) Token: 0x0600188C RID: 6284 RVA: 0x00097CAC File Offset: 0x00096CAC
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x170007FB RID: 2043
			// (get) Token: 0x0600188D RID: 6285 RVA: 0x00097CC4 File Offset: 0x00096CC4
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
						throw new InvalidOperationException("This property is only valid in AsyncOP_State.Completed state.");
					}
					return this.m_pException;
				}
			}

			// Token: 0x170007FC RID: 2044
			// (get) Token: 0x0600188E RID: 6286 RVA: 0x00097D18 File Offset: 0x00096D18
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
						throw new InvalidOperationException("This property is only valid in AsyncOP_State.Completed state.");
					}
					return this.m_pStream;
				}
			}

			// Token: 0x170007FD RID: 2045
			// (get) Token: 0x0600188F RID: 6287 RVA: 0x00097D6C File Offset: 0x00096D6C
			public long BytesStored
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
						throw new InvalidOperationException("This property is only valid in AsyncOP_State.Completed state.");
					}
					return this.m_BytesStored;
				}
			}

			// Token: 0x170007FE RID: 2046
			// (get) Token: 0x06001890 RID: 6288 RVA: 0x00097DC0 File Offset: 0x00096DC0
			public int LinesStored
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
						throw new InvalidOperationException("This property is only valid in AsyncOP_State.Completed state.");
					}
					return this.m_LinesStored;
				}
			}

			// Token: 0x140000B1 RID: 177
			// (add) Token: 0x06001891 RID: 6289 RVA: 0x00097E14 File Offset: 0x00096E14
			// (remove) Token: 0x06001892 RID: 6290 RVA: 0x00097E4C File Offset: 0x00096E4C
			
			public event EventHandler<EventArgs<SmartStream.ReadPeriodTerminatedAsyncOP>> CompletedAsync = null;

			// Token: 0x06001893 RID: 6291 RVA: 0x00097E84 File Offset: 0x00096E84
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<SmartStream.ReadPeriodTerminatedAsyncOP>(this));
				}
				bool flag2 = this.Completed != null;
				if (flag2)
				{
					this.Completed(this, new EventArgs<SmartStream.ReadPeriodTerminatedAsyncOP>(this));
				}
			}

			// Token: 0x140000B2 RID: 178
			// (add) Token: 0x06001894 RID: 6292 RVA: 0x00097ED8 File Offset: 0x00096ED8
			// (remove) Token: 0x06001895 RID: 6293 RVA: 0x00097F10 File Offset: 0x00096F10
			[Obsolete("Use CompletedAsync event istead.")]
			
			public event EventHandler<EventArgs<SmartStream.ReadPeriodTerminatedAsyncOP>> Completed = null;

			// Token: 0x04000A76 RID: 2678
			private object m_pLock = new object();

			// Token: 0x04000A77 RID: 2679
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000A78 RID: 2680
			private Exception m_pException = null;

			// Token: 0x04000A79 RID: 2681
			private bool m_RiseCompleted = false;

			// Token: 0x04000A7A RID: 2682
			private SmartStream m_pOwner = null;

			// Token: 0x04000A7B RID: 2683
			private Stream m_pStream = null;

			// Token: 0x04000A7C RID: 2684
			private long m_MaxCount = 0L;

			// Token: 0x04000A7D RID: 2685
			private SizeExceededAction m_ExceededAction = SizeExceededAction.JunkAndThrowException;

			// Token: 0x04000A7E RID: 2686
			private SmartStream.ReadLineAsyncOP m_pReadLineOP = null;

			// Token: 0x04000A7F RID: 2687
			private long m_BytesStored = 0L;

			// Token: 0x04000A80 RID: 2688
			private int m_LinesStored = 0;
		}

		// Token: 0x020002CC RID: 716
		private class BufferReadAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x06001896 RID: 6294 RVA: 0x00097F48 File Offset: 0x00096F48
			public BufferReadAsyncOP(SmartStream owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pOwner = owner;
			}

			// Token: 0x06001897 RID: 6295 RVA: 0x00097FC4 File Offset: 0x00096FC4
			~BufferReadAsyncOP()
			{
				this.Dispose();
			}

			// Token: 0x06001898 RID: 6296 RVA: 0x00097FF4 File Offset: 0x00096FF4
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					try
					{
						bool flag2 = this.m_State == AsyncOP_State.Active;
						if (flag2)
						{
							this.m_pException = new ObjectDisposedException("SmartStream");
							this.m_State = AsyncOP_State.Completed;
							this.OnCompletedAsync();
						}
					}
					catch
					{
					}
					this.m_State = AsyncOP_State.Disposed;
					this.m_pOwner = null;
					this.m_pBuffer = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x06001899 RID: 6297 RVA: 0x00098078 File Offset: 0x00097078
			internal bool Start(bool async, byte[] buffer, int count)
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag2 = this.m_State == AsyncOP_State.Active;
				if (flag2)
				{
					throw new InvalidOperationException("There is existing active operation. There may be only one active operation at same time.");
				}
				bool flag3 = buffer == null;
				if (flag3)
				{
					throw new ArgumentNullException("buffer");
				}
				bool flag4 = count < 0;
				if (flag4)
				{
					throw new ArgumentException("Argument 'count' value must be >= 0.");
				}
				bool flag5 = count > buffer.Length;
				if (flag5)
				{
					throw new ArgumentException("Argument 'count' value must be <= buffer.Length.");
				}
				this.m_State = AsyncOP_State.Active;
				this.m_RiseCompleted = false;
				this.m_pException = null;
				this.m_pBuffer = buffer;
				this.m_MaxCount = count;
				this.m_BytesInBuffer = 0;
				this.m_IsCallbackCalled = false;
				if (async)
				{
					try
					{
						this.m_pOwner.m_pStream.BeginRead(buffer, 0, count, delegate(IAsyncResult r)
						{
							try
							{
								this.m_BytesInBuffer = this.m_pOwner.m_pStream.EndRead(r);
							}
							catch (Exception pException3)
							{
								this.m_pException = pException3;
							}
							this.SetState(AsyncOP_State.Completed);
						}, null);
					}
					catch (Exception pException)
					{
						this.m_pException = pException;
						this.SetState(AsyncOP_State.Completed);
					}
				}
				else
				{
					try
					{
						this.m_BytesInBuffer = this.m_pOwner.m_pStream.Read(buffer, 0, count);
					}
					catch (Exception pException2)
					{
						this.m_pException = pException2;
					}
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

			// Token: 0x0600189A RID: 6298 RVA: 0x0009820C File Offset: 0x0009720C
			internal void ReleaseEvents()
			{
				this.CompletedAsync = null;
			}

			// Token: 0x0600189B RID: 6299 RVA: 0x00098218 File Offset: 0x00097218
			private void SetState(AsyncOP_State state)
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					bool riseCompleted = this.m_RiseCompleted;
					object pLock = this.m_pLock;
					lock (pLock)
					{
						this.m_State = state;
						riseCompleted = this.m_RiseCompleted;
					}
					bool flag3 = this.m_State == AsyncOP_State.Completed && riseCompleted;
					if (flag3)
					{
						this.OnCompletedAsync();
					}
				}
			}

			// Token: 0x170007FF RID: 2047
			// (get) Token: 0x0600189C RID: 6300 RVA: 0x00098298 File Offset: 0x00097298
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x17000800 RID: 2048
			// (get) Token: 0x0600189D RID: 6301 RVA: 0x000982B0 File Offset: 0x000972B0
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
						throw new InvalidOperationException("This property is only valid in AsyncOP_State.Completed state.");
					}
					return this.m_pException;
				}
			}

			// Token: 0x17000801 RID: 2049
			// (get) Token: 0x0600189E RID: 6302 RVA: 0x00098304 File Offset: 0x00097304
			public byte[] Buffer
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
						throw new InvalidOperationException("This property is only valid in AsyncOP_State.Completed state.");
					}
					return this.m_pBuffer;
				}
			}

			// Token: 0x17000802 RID: 2050
			// (get) Token: 0x0600189F RID: 6303 RVA: 0x00098358 File Offset: 0x00097358
			public int BytesInBuffer
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
						throw new InvalidOperationException("This property is only valid in AsyncOP_State.Completed state.");
					}
					return this.m_BytesInBuffer;
				}
			}

			// Token: 0x140000B3 RID: 179
			// (add) Token: 0x060018A0 RID: 6304 RVA: 0x000983AC File Offset: 0x000973AC
			// (remove) Token: 0x060018A1 RID: 6305 RVA: 0x000983E4 File Offset: 0x000973E4
			
			public event EventHandler<EventArgs<SmartStream.BufferReadAsyncOP>> CompletedAsync = null;

			// Token: 0x060018A2 RID: 6306 RVA: 0x0009841C File Offset: 0x0009741C
			private void OnCompletedAsync()
			{
				bool flag = !this.m_IsCallbackCalled && this.CompletedAsync != null;
				if (flag)
				{
					this.m_IsCallbackCalled = true;
					this.CompletedAsync(this, new EventArgs<SmartStream.BufferReadAsyncOP>(this));
				}
			}

			// Token: 0x04000A83 RID: 2691
			private object m_pLock = new object();

			// Token: 0x04000A84 RID: 2692
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000A85 RID: 2693
			private Exception m_pException = null;

			// Token: 0x04000A86 RID: 2694
			private bool m_RiseCompleted = false;

			// Token: 0x04000A87 RID: 2695
			private SmartStream m_pOwner = null;

			// Token: 0x04000A88 RID: 2696
			private byte[] m_pBuffer = null;

			// Token: 0x04000A89 RID: 2697
			private int m_MaxCount = 0;

			// Token: 0x04000A8A RID: 2698
			private int m_BytesInBuffer = 0;

			// Token: 0x04000A8B RID: 2699
			private bool m_IsCallbackCalled = false;
		}

		// Token: 0x020002CD RID: 717
		public class WriteStreamAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x060018A4 RID: 6308 RVA: 0x000984B0 File Offset: 0x000974B0
			public WriteStreamAsyncOP(Stream stream, long count)
			{
				bool flag = stream == null;
				if (flag)
				{
					throw new ArgumentNullException("stream");
				}
				this.m_pStream = stream;
				this.m_Count = count;
				this.m_pBuffer = new byte[32000];
			}

			// Token: 0x060018A5 RID: 6309 RVA: 0x00098544 File Offset: 0x00097544
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pStream = null;
					this.m_pOwner = null;
					this.m_pBuffer = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x060018A6 RID: 6310 RVA: 0x00098590 File Offset: 0x00097590
			internal bool Start(SmartStream owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pOwner = owner;
				this.SetState(AsyncOP_State.Active);
				this.BeginReadData();
				object pLock = this.m_pLock;
				bool result;
				lock (pLock)
				{
					this.m_RiseCompleted = true;
					result = (this.m_State == AsyncOP_State.Active);
				}
				return result;
			}

			// Token: 0x060018A7 RID: 6311 RVA: 0x0009860C File Offset: 0x0009760C
			private void SetState(AsyncOP_State state)
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					bool riseCompleted = this.m_RiseCompleted;
					object pLock = this.m_pLock;
					lock (pLock)
					{
						this.m_State = state;
						riseCompleted = this.m_RiseCompleted;
					}
					bool flag3 = this.m_State == AsyncOP_State.Completed && riseCompleted;
					if (flag3)
					{
						this.OnCompletedAsync();
					}
				}
			}

			// Token: 0x060018A8 RID: 6312 RVA: 0x0009868C File Offset: 0x0009768C
			private void BeginReadData()
			{
				try
				{
					bool flag3;
					do
					{
						bool isBeginReadCompleted = false;
						bool isCompletedSync = false;
						int count = (this.m_Count == -1L) ? this.m_pBuffer.Length : ((int)Math.Min((long)this.m_pBuffer.Length, this.m_Count - this.m_BytesWritten));
						IAsyncResult readResult = this.m_pStream.BeginRead(this.m_pBuffer, 0, count, delegate(IAsyncResult r)
						{
							object pLock2 = this.m_pLock;
							lock (pLock2)
							{
								bool flag5 = !isBeginReadCompleted;
								if (flag5)
								{
									isCompletedSync = true;
									return;
								}
							}
							this.ProcessReadDataResult(r);
						}, null);
						object pLock = this.m_pLock;
						lock (pLock)
						{
							isBeginReadCompleted = true;
						}
						bool isCompletedSync2 = isCompletedSync;
						if (!isCompletedSync2)
						{
							break;
						}
						bool flag2 = this.ProcessReadDataResult(readResult);
						if (flag2)
						{
							break;
						}
						flag3 = (this.State != AsyncOP_State.Active);
					}
					while (!flag3);
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x060018A9 RID: 6313 RVA: 0x000987B0 File Offset: 0x000977B0
			private bool ProcessReadDataResult(IAsyncResult readResult)
			{
				try
				{
					int countReaded = this.m_pStream.EndRead(readResult);
					bool flag = countReaded == 0;
					if (flag)
					{
						bool flag2 = this.m_Count == -1L;
						if (flag2)
						{
							this.SetState(AsyncOP_State.Completed);
						}
						else
						{
							this.m_pException = new ArgumentException("Argument 'stream' has less data than specified in 'count'.", "stream");
							this.SetState(AsyncOP_State.Completed);
						}
					}
					else
					{
						bool isBeginWriteCompleted = false;
						bool isCompletedSync = false;
						IAsyncResult asyncResult = this.m_pOwner.BeginWrite(this.m_pBuffer, 0, countReaded, delegate(IAsyncResult r)
						{
							object pLock2 = this.m_pLock;
							lock (pLock2)
							{
								bool flag6 = !isBeginWriteCompleted;
								if (flag6)
								{
									isCompletedSync = true;
									return;
								}
							}
							try
							{
								this.m_pOwner.EndWrite(r);
								this.m_BytesWritten += (long)countReaded;
								bool flag7 = this.m_Count == this.m_BytesWritten;
								if (flag7)
								{
									this.SetState(AsyncOP_State.Completed);
								}
								else
								{
									this.BeginReadData();
								}
							}
							catch (Exception pException2)
							{
								this.m_pException = pException2;
								this.SetState(AsyncOP_State.Completed);
							}
						}, null);
						object pLock = this.m_pLock;
						lock (pLock)
						{
							isBeginWriteCompleted = true;
						}
						bool isCompletedSync2 = isCompletedSync;
						if (!isCompletedSync2)
						{
							return true;
						}
						this.m_pOwner.EndWrite(asyncResult);
						this.m_BytesWritten += (long)countReaded;
						bool flag4 = this.m_Count == this.m_BytesWritten;
						if (flag4)
						{
							this.SetState(AsyncOP_State.Completed);
						}
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.SetState(AsyncOP_State.Completed);
				}
				return false;
			}

			// Token: 0x17000803 RID: 2051
			// (get) Token: 0x060018AA RID: 6314 RVA: 0x00098950 File Offset: 0x00097950
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x17000804 RID: 2052
			// (get) Token: 0x060018AB RID: 6315 RVA: 0x00098968 File Offset: 0x00097968
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

			// Token: 0x17000805 RID: 2053
			// (get) Token: 0x060018AC RID: 6316 RVA: 0x000989BC File Offset: 0x000979BC
			public long BytesWritten
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
						throw new InvalidOperationException("Property 'Socket' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					bool flag3 = this.m_pException != null;
					if (flag3)
					{
						throw this.m_pException;
					}
					return this.m_BytesWritten;
				}
			}

			// Token: 0x140000B4 RID: 180
			// (add) Token: 0x060018AD RID: 6317 RVA: 0x00098A24 File Offset: 0x00097A24
			// (remove) Token: 0x060018AE RID: 6318 RVA: 0x00098A5C File Offset: 0x00097A5C
			
			public event EventHandler<EventArgs<SmartStream.WriteStreamAsyncOP>> CompletedAsync = null;

			// Token: 0x060018AF RID: 6319 RVA: 0x00098A94 File Offset: 0x00097A94
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<SmartStream.WriteStreamAsyncOP>(this));
				}
			}

			// Token: 0x04000A8D RID: 2701
			private object m_pLock = new object();

			// Token: 0x04000A8E RID: 2702
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000A8F RID: 2703
			private Exception m_pException = null;

			// Token: 0x04000A90 RID: 2704
			private bool m_RiseCompleted = false;

			// Token: 0x04000A91 RID: 2705
			private SmartStream m_pOwner = null;

			// Token: 0x04000A92 RID: 2706
			private Stream m_pStream = null;

			// Token: 0x04000A93 RID: 2707
			private long m_Count = 0L;

			// Token: 0x04000A94 RID: 2708
			private byte[] m_pBuffer = null;

			// Token: 0x04000A95 RID: 2709
			private long m_BytesWritten = 0L;
		}

		// Token: 0x020002CE RID: 718
		public class WritePeriodTerminatedAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x060018B0 RID: 6320 RVA: 0x00098AC4 File Offset: 0x00097AC4
			public WritePeriodTerminatedAsyncOP(Stream stream)
			{
				bool flag = stream == null;
				if (flag)
				{
					throw new ArgumentNullException("stream");
				}
				this.m_pStream = new SmartStream(stream, false);
			}

			// Token: 0x060018B1 RID: 6321 RVA: 0x00098B44 File Offset: 0x00097B44
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pStream = null;
					this.m_pOwner = null;
					this.m_pReadLineOP = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x060018B2 RID: 6322 RVA: 0x00098B90 File Offset: 0x00097B90
			internal bool Start(SmartStream owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pOwner = owner;
				this.SetState(AsyncOP_State.Active);
				try
				{
					this.m_pReadLineOP = new SmartStream.ReadLineAsyncOP(new byte[32000], SizeExceededAction.ThrowException);
					this.m_pReadLineOP.CompletedAsync += delegate(object s, EventArgs<SmartStream.ReadLineAsyncOP> e)
					{
						this.ReadLineCompleted(this.m_pReadLineOP);
					};
					bool flag2 = this.m_pStream.ReadLine(this.m_pReadLineOP, true);
					if (flag2)
					{
						this.ReadLineCompleted(this.m_pReadLineOP);
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.SetState(AsyncOP_State.Completed);
					this.m_pReadLineOP.Dispose();
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

			// Token: 0x060018B3 RID: 6323 RVA: 0x00098C8C File Offset: 0x00097C8C
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

			// Token: 0x060018B4 RID: 6324 RVA: 0x00098D04 File Offset: 0x00097D04
			private void ReadLineCompleted(SmartStream.ReadLineAsyncOP op)
			{
				try
				{
					bool flag = op.Error != null;
					if (flag)
					{
						this.m_pException = op.Error;
						this.SetState(AsyncOP_State.Completed);
					}
					else
					{
						bool flag2 = op.BytesInBuffer == 0;
						if (flag2)
						{
							bool endsCRLF = this.m_EndsCRLF;
							if (endsCRLF)
							{
								this.m_BytesWritten += 3;
								this.m_pOwner.BeginWrite(new byte[]
								{
									46,
									13,
									10
								}, 0, 3, new AsyncCallback(this.SendTerminatorCompleted), null);
							}
							else
							{
								this.m_BytesWritten += 5;
								this.m_pOwner.BeginWrite(new byte[]
								{
									13,
									10,
									46,
									13,
									10
								}, 0, 5, new AsyncCallback(this.SendTerminatorCompleted), null);
							}
							op.Dispose();
						}
						else
						{
							this.m_BytesWritten += op.BytesInBuffer;
							bool flag3 = op.BytesInBuffer >= 2 && op.Buffer[op.BytesInBuffer - 2] == 13 && op.Buffer[op.BytesInBuffer - 1] == 10;
							if (flag3)
							{
								this.m_EndsCRLF = true;
							}
							else
							{
								this.m_EndsCRLF = false;
							}
							bool flag4 = op.Buffer[0] == 46;
							if (flag4)
							{
								byte[] array = new byte[op.BytesInBuffer + 1];
								array[0] = 46;
								Array.Copy(op.Buffer, 0, array, 1, op.BytesInBuffer);
								this.m_pOwner.BeginWrite(array, 0, array.Length, new AsyncCallback(this.SendLineCompleted), null);
							}
							else
							{
								this.m_pOwner.BeginWrite(op.Buffer, 0, op.BytesInBuffer, new AsyncCallback(this.SendLineCompleted), null);
							}
						}
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.SetState(AsyncOP_State.Completed);
					op.Dispose();
				}
			}

			// Token: 0x060018B5 RID: 6325 RVA: 0x00098F04 File Offset: 0x00097F04
			private void SendLineCompleted(IAsyncResult ar)
			{
				try
				{
					this.m_pOwner.EndWrite(ar);
					bool flag = this.m_pStream.ReadLine(this.m_pReadLineOP, true);
					if (flag)
					{
						this.ReadLineCompleted(this.m_pReadLineOP);
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x060018B6 RID: 6326 RVA: 0x00098F6C File Offset: 0x00097F6C
			private void SendTerminatorCompleted(IAsyncResult ar)
			{
				try
				{
					this.m_pOwner.EndWrite(ar);
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
				}
				this.SetState(AsyncOP_State.Completed);
			}

			// Token: 0x17000806 RID: 2054
			// (get) Token: 0x060018B7 RID: 6327 RVA: 0x00098FB0 File Offset: 0x00097FB0
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x17000807 RID: 2055
			// (get) Token: 0x060018B8 RID: 6328 RVA: 0x00098FC8 File Offset: 0x00097FC8
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

			// Token: 0x17000808 RID: 2056
			// (get) Token: 0x060018B9 RID: 6329 RVA: 0x0009901C File Offset: 0x0009801C
			public int BytesWritten
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
						throw new InvalidOperationException("Property 'Socket' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					bool flag3 = this.m_pException != null;
					if (flag3)
					{
						throw this.m_pException;
					}
					return this.m_BytesWritten;
				}
			}

			// Token: 0x140000B5 RID: 181
			// (add) Token: 0x060018BA RID: 6330 RVA: 0x00099084 File Offset: 0x00098084
			// (remove) Token: 0x060018BB RID: 6331 RVA: 0x000990BC File Offset: 0x000980BC
			
			public event EventHandler<EventArgs<SmartStream.WritePeriodTerminatedAsyncOP>> CompletedAsync = null;

			// Token: 0x060018BC RID: 6332 RVA: 0x000990F4 File Offset: 0x000980F4
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<SmartStream.WritePeriodTerminatedAsyncOP>(this));
				}
			}

			// Token: 0x04000A97 RID: 2711
			private object m_pLock = new object();

			// Token: 0x04000A98 RID: 2712
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000A99 RID: 2713
			private Exception m_pException = null;

			// Token: 0x04000A9A RID: 2714
			private SmartStream m_pStream = null;

			// Token: 0x04000A9B RID: 2715
			private SmartStream m_pOwner = null;

			// Token: 0x04000A9C RID: 2716
			private SmartStream.ReadLineAsyncOP m_pReadLineOP = null;

			// Token: 0x04000A9D RID: 2717
			private int m_BytesWritten = 0;

			// Token: 0x04000A9E RID: 2718
			private bool m_EndsCRLF = false;

			// Token: 0x04000A9F RID: 2719
			private bool m_RiseCompleted = false;
		}

		// Token: 0x020002CF RID: 719
		private class ReadLineAsyncOperation : IAsyncResult
		{
			// Token: 0x060018BE RID: 6334 RVA: 0x00099134 File Offset: 0x00098134
			public ReadLineAsyncOperation(SmartStream owner, byte[] buffer, int offset, int maxCount, SizeExceededAction exceededAction, AsyncCallback callback, object asyncState)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				bool flag2 = buffer == null;
				if (flag2)
				{
					throw new ArgumentNullException("buffer");
				}
				bool flag3 = offset < 0;
				if (flag3)
				{
					throw new ArgumentOutOfRangeException("offset", "Argument 'offset' value must be >= 0.");
				}
				bool flag4 = offset > buffer.Length;
				if (flag4)
				{
					throw new ArgumentOutOfRangeException("offset", "Argument 'offset' value must be < buffer.Length.");
				}
				bool flag5 = maxCount < 0;
				if (flag5)
				{
					throw new ArgumentOutOfRangeException("maxCount", "Argument 'maxCount' value must be >= 0.");
				}
				bool flag6 = offset + maxCount > buffer.Length;
				if (flag6)
				{
					throw new ArgumentOutOfRangeException("maxCount", "Argument 'maxCount' is bigger than than argument 'buffer' can store.");
				}
				this.m_pOwner = owner;
				this.m_pBuffer = buffer;
				this.m_OffsetInBuffer = offset;
				this.m_MaxCount = maxCount;
				this.m_SizeExceededAction = exceededAction;
				this.m_pAsyncCallback = callback;
				this.m_pAsyncState = asyncState;
				this.m_pAsyncWaitHandle = new AutoResetEvent(false);
				this.DoLineReading();
			}

			// Token: 0x060018BF RID: 6335 RVA: 0x0009928C File Offset: 0x0009828C
			private void Buffering_Completed(Exception x)
			{
				bool flag = x != null;
				if (flag)
				{
					this.m_pException = x;
					this.Completed();
				}
				else
				{
					bool flag2 = this.m_pOwner.BytesInReadBuffer == 0;
					if (flag2)
					{
						this.Completed();
					}
					else
					{
						this.DoLineReading();
					}
				}
			}

			// Token: 0x060018C0 RID: 6336 RVA: 0x000992DC File Offset: 0x000982DC
			private void DoLineReading()
			{
				try
				{
					for (;;)
					{
						bool flag = this.m_pOwner.BytesInReadBuffer == 0;
						if (flag)
						{
							bool flag2 = this.m_pOwner.BufferRead(true, new SmartStream.BufferCallback(this.Buffering_Completed));
							if (flag2)
							{
								break;
							}
							bool flag3 = this.m_pOwner.BytesInReadBuffer == 0;
							if (flag3)
							{
								goto Block_4;
							}
						}
						byte[] pReadBuffer = this.m_pOwner.m_pReadBuffer;
						SmartStream pOwner = this.m_pOwner;
						int num = pOwner.m_ReadBufferOffset;
						pOwner.m_ReadBufferOffset = num + 1;
						byte b = pReadBuffer[num];
						this.m_BytesReaded++;
						bool flag4 = b == 10;
						if (flag4)
						{
							goto Block_5;
						}
						bool flag5 = b == 13 && this.m_pOwner.Peek() == 10;
						if (flag5)
						{
							goto Block_7;
						}
						bool flag6 = b == 13;
						if (flag6)
						{
							goto Block_8;
						}
						bool flag7 = this.m_BytesStored >= this.m_MaxCount;
						if (flag7)
						{
							bool flag8 = this.m_SizeExceededAction == SizeExceededAction.ThrowException;
							if (flag8)
							{
								goto Block_10;
							}
						}
						else
						{
							byte[] pBuffer = this.m_pBuffer;
							num = this.m_OffsetInBuffer;
							this.m_OffsetInBuffer = num + 1;
							pBuffer[num] = b;
							this.m_BytesStored++;
						}
					}
					return;
					Block_4:
					this.Completed();
					return;
					Block_5:
					goto IL_156;
					Block_7:
					this.m_pOwner.ReadByte();
					this.m_BytesReaded++;
					Block_8:
					goto IL_156;
					Block_10:
					throw new LineSizeExceededException();
					IL_156:;
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
				}
				this.Completed();
			}

			// Token: 0x060018C1 RID: 6337 RVA: 0x00099474 File Offset: 0x00098474
			private void Completed()
			{
				this.m_IsCompleted = true;
				this.m_pAsyncWaitHandle.Set();
				bool flag = this.m_pAsyncCallback != null;
				if (flag)
				{
					this.m_pAsyncCallback(this);
				}
			}

			// Token: 0x17000809 RID: 2057
			// (get) Token: 0x060018C2 RID: 6338 RVA: 0x000994B4 File Offset: 0x000984B4
			public object AsyncState
			{
				get
				{
					return this.m_pAsyncState;
				}
			}

			// Token: 0x1700080A RID: 2058
			// (get) Token: 0x060018C3 RID: 6339 RVA: 0x000994CC File Offset: 0x000984CC
			public WaitHandle AsyncWaitHandle
			{
				get
				{
					return this.m_pAsyncWaitHandle;
				}
			}

			// Token: 0x1700080B RID: 2059
			// (get) Token: 0x060018C4 RID: 6340 RVA: 0x000994E4 File Offset: 0x000984E4
			public bool CompletedSynchronously
			{
				get
				{
					return this.m_CompletedSynchronously;
				}
			}

			// Token: 0x1700080C RID: 2060
			// (get) Token: 0x060018C5 RID: 6341 RVA: 0x000994FC File Offset: 0x000984FC
			public bool IsCompleted
			{
				get
				{
					return this.m_IsCompleted;
				}
			}

			// Token: 0x1700080D RID: 2061
			// (get) Token: 0x060018C6 RID: 6342 RVA: 0x00099514 File Offset: 0x00098514
			// (set) Token: 0x060018C7 RID: 6343 RVA: 0x0009952C File Offset: 0x0009852C
			internal bool IsEndCalled
			{
				get
				{
					return this.m_IsEndCalled;
				}
				set
				{
					this.m_IsEndCalled = value;
				}
			}

			// Token: 0x1700080E RID: 2062
			// (get) Token: 0x060018C8 RID: 6344 RVA: 0x00099538 File Offset: 0x00098538
			internal byte[] Buffer
			{
				get
				{
					return this.m_pBuffer;
				}
			}

			// Token: 0x1700080F RID: 2063
			// (get) Token: 0x060018C9 RID: 6345 RVA: 0x00099550 File Offset: 0x00098550
			internal int BytesReaded
			{
				get
				{
					return this.m_BytesReaded;
				}
			}

			// Token: 0x17000810 RID: 2064
			// (get) Token: 0x060018CA RID: 6346 RVA: 0x00099568 File Offset: 0x00098568
			internal int BytesStored
			{
				get
				{
					return this.m_BytesStored;
				}
			}

			// Token: 0x04000AA1 RID: 2721
			private SmartStream m_pOwner = null;

			// Token: 0x04000AA2 RID: 2722
			private byte[] m_pBuffer = null;

			// Token: 0x04000AA3 RID: 2723
			private int m_OffsetInBuffer = 0;

			// Token: 0x04000AA4 RID: 2724
			private int m_MaxCount = 0;

			// Token: 0x04000AA5 RID: 2725
			private SizeExceededAction m_SizeExceededAction = SizeExceededAction.JunkAndThrowException;

			// Token: 0x04000AA6 RID: 2726
			private AsyncCallback m_pAsyncCallback = null;

			// Token: 0x04000AA7 RID: 2727
			private object m_pAsyncState = null;

			// Token: 0x04000AA8 RID: 2728
			private AutoResetEvent m_pAsyncWaitHandle = null;

			// Token: 0x04000AA9 RID: 2729
			private bool m_CompletedSynchronously = false;

			// Token: 0x04000AAA RID: 2730
			private bool m_IsCompleted = false;

			// Token: 0x04000AAB RID: 2731
			private bool m_IsEndCalled = false;

			// Token: 0x04000AAC RID: 2732
			private int m_BytesReaded = 0;

			// Token: 0x04000AAD RID: 2733
			private int m_BytesStored = 0;

			// Token: 0x04000AAE RID: 2734
			private Exception m_pException = null;
		}

		// Token: 0x020002D0 RID: 720
		private class ReadToTerminatorAsyncOperation : IAsyncResult
		{
			// Token: 0x060018CB RID: 6347 RVA: 0x00099580 File Offset: 0x00098580
			public ReadToTerminatorAsyncOperation(SmartStream owner, string terminator, Stream storeStream, long maxCount, SizeExceededAction exceededAction, AsyncCallback callback, object asyncState)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				bool flag2 = terminator == null;
				if (flag2)
				{
					throw new ArgumentNullException("terminator");
				}
				bool flag3 = storeStream == null;
				if (flag3)
				{
					throw new ArgumentNullException("storeStream");
				}
				bool flag4 = maxCount < 0L;
				if (flag4)
				{
					throw new ArgumentException("Argument 'maxCount' must be >= 0.");
				}
				this.m_pOwner = owner;
				this.m_Terminator = terminator;
				this.m_pTerminatorBytes = Encoding.ASCII.GetBytes(terminator);
				this.m_pStoreStream = storeStream;
				this.m_MaxCount = maxCount;
				this.m_SizeExceededAction = exceededAction;
				this.m_pAsyncCallback = callback;
				this.m_pAsyncState = asyncState;
				this.m_pAsyncWaitHandle = new AutoResetEvent(false);
				this.m_pLineBuffer = new byte[32000];
				this.m_pOwner.BeginReadLine(this.m_pLineBuffer, 0, this.m_pLineBuffer.Length - 2, this.m_SizeExceededAction, new AsyncCallback(this.ReadLine_Completed), null);
			}

			// Token: 0x060018CC RID: 6348 RVA: 0x000996E8 File Offset: 0x000986E8
			private void ReadLine_Completed(IAsyncResult asyncResult)
			{
				try
				{
					int num = 0;
					try
					{
						num = this.m_pOwner.EndReadLine(asyncResult);
					}
					catch (LineSizeExceededException ex)
					{
						bool flag = this.m_SizeExceededAction == SizeExceededAction.ThrowException;
						if (flag)
						{
							throw ex;
						}
						this.m_pException = new LineSizeExceededException();
						num = 31998;
					}
					bool flag2 = num == -1;
					if (flag2)
					{
						throw new IncompleteDataException();
					}
					bool flag3 = Net_Utils.CompareArray(this.m_pTerminatorBytes, this.m_pLineBuffer, num);
					if (flag3)
					{
						this.Completed();
					}
					else
					{
						bool flag4 = this.m_MaxCount > 0L && this.m_BytesStored + (long)num + 2L > this.m_MaxCount;
						if (flag4)
						{
							bool flag5 = this.m_SizeExceededAction == SizeExceededAction.ThrowException;
							if (flag5)
							{
								throw new DataSizeExceededException();
							}
							this.m_pException = new DataSizeExceededException();
						}
						else
						{
							this.m_pLineBuffer[num++] = 13;
							this.m_pLineBuffer[num++] = 10;
							this.m_pStoreStream.Write(this.m_pLineBuffer, 0, num);
							this.m_BytesStored += (long)num;
						}
						this.m_pOwner.BeginReadLine(this.m_pLineBuffer, 0, this.m_pLineBuffer.Length - 2, this.m_SizeExceededAction, new AsyncCallback(this.ReadLine_Completed), null);
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.Completed();
				}
			}

			// Token: 0x060018CD RID: 6349 RVA: 0x00099874 File Offset: 0x00098874
			private void Completed()
			{
				this.m_IsCompleted = true;
				this.m_pAsyncWaitHandle.Set();
				bool flag = this.m_pAsyncCallback != null;
				if (flag)
				{
					this.m_pAsyncCallback(this);
				}
			}

			// Token: 0x17000811 RID: 2065
			// (get) Token: 0x060018CE RID: 6350 RVA: 0x000998B4 File Offset: 0x000988B4
			public string Terminator
			{
				get
				{
					return this.m_Terminator;
				}
			}

			// Token: 0x17000812 RID: 2066
			// (get) Token: 0x060018CF RID: 6351 RVA: 0x000998CC File Offset: 0x000988CC
			public object AsyncState
			{
				get
				{
					return this.m_pAsyncState;
				}
			}

			// Token: 0x17000813 RID: 2067
			// (get) Token: 0x060018D0 RID: 6352 RVA: 0x000998E4 File Offset: 0x000988E4
			public WaitHandle AsyncWaitHandle
			{
				get
				{
					return this.m_pAsyncWaitHandle;
				}
			}

			// Token: 0x17000814 RID: 2068
			// (get) Token: 0x060018D1 RID: 6353 RVA: 0x000998FC File Offset: 0x000988FC
			public bool CompletedSynchronously
			{
				get
				{
					return this.m_CompletedSynchronously;
				}
			}

			// Token: 0x17000815 RID: 2069
			// (get) Token: 0x060018D2 RID: 6354 RVA: 0x00099914 File Offset: 0x00098914
			public bool IsCompleted
			{
				get
				{
					return this.m_IsCompleted;
				}
			}

			// Token: 0x17000816 RID: 2070
			// (get) Token: 0x060018D3 RID: 6355 RVA: 0x0009992C File Offset: 0x0009892C
			// (set) Token: 0x060018D4 RID: 6356 RVA: 0x00099944 File Offset: 0x00098944
			internal bool IsEndCalled
			{
				get
				{
					return this.m_IsEndCalled;
				}
				set
				{
					this.m_IsEndCalled = value;
				}
			}

			// Token: 0x17000817 RID: 2071
			// (get) Token: 0x060018D5 RID: 6357 RVA: 0x00099950 File Offset: 0x00098950
			internal long BytesStored
			{
				get
				{
					return this.m_BytesStored;
				}
			}

			// Token: 0x17000818 RID: 2072
			// (get) Token: 0x060018D6 RID: 6358 RVA: 0x00099968 File Offset: 0x00098968
			internal Exception Exception
			{
				get
				{
					return this.m_pException;
				}
			}

			// Token: 0x04000AAF RID: 2735
			private SmartStream m_pOwner = null;

			// Token: 0x04000AB0 RID: 2736
			private string m_Terminator = "";

			// Token: 0x04000AB1 RID: 2737
			private byte[] m_pTerminatorBytes = null;

			// Token: 0x04000AB2 RID: 2738
			private Stream m_pStoreStream = null;

			// Token: 0x04000AB3 RID: 2739
			private long m_MaxCount = 0L;

			// Token: 0x04000AB4 RID: 2740
			private SizeExceededAction m_SizeExceededAction = SizeExceededAction.JunkAndThrowException;

			// Token: 0x04000AB5 RID: 2741
			private AsyncCallback m_pAsyncCallback = null;

			// Token: 0x04000AB6 RID: 2742
			private object m_pAsyncState = null;

			// Token: 0x04000AB7 RID: 2743
			private AutoResetEvent m_pAsyncWaitHandle = null;

			// Token: 0x04000AB8 RID: 2744
			private bool m_CompletedSynchronously = false;

			// Token: 0x04000AB9 RID: 2745
			private bool m_IsCompleted = false;

			// Token: 0x04000ABA RID: 2746
			private bool m_IsEndCalled = false;

			// Token: 0x04000ABB RID: 2747
			private byte[] m_pLineBuffer = null;

			// Token: 0x04000ABC RID: 2748
			private long m_BytesStored = 0L;

			// Token: 0x04000ABD RID: 2749
			private Exception m_pException = null;
		}

		// Token: 0x020002D1 RID: 721
		private class ReadToStreamAsyncOperation : IAsyncResult
		{
			// Token: 0x060018D7 RID: 6359 RVA: 0x00099980 File Offset: 0x00098980
			public ReadToStreamAsyncOperation(SmartStream owner, Stream storeStream, long count, AsyncCallback callback, object asyncState)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				bool flag2 = storeStream == null;
				if (flag2)
				{
					throw new ArgumentNullException("storeStream");
				}
				bool flag3 = count < 0L;
				if (flag3)
				{
					throw new ArgumentException("Argument 'count' must be >= 0.");
				}
				this.m_pOwner = owner;
				this.m_pStoreStream = storeStream;
				this.m_Count = count;
				this.m_pAsyncCallback = callback;
				this.m_pAsyncState = asyncState;
				this.m_pAsyncWaitHandle = new AutoResetEvent(false);
				bool flag4 = this.m_Count == 0L;
				if (flag4)
				{
					this.Completed();
				}
				else
				{
					this.DoDataReading();
				}
			}

			// Token: 0x060018D8 RID: 6360 RVA: 0x00099A74 File Offset: 0x00098A74
			private void Buffering_Completed(Exception x)
			{
				bool flag = x != null;
				if (flag)
				{
					this.m_pException = x;
					this.Completed();
				}
				else
				{
					bool flag2 = this.m_pOwner.BytesInReadBuffer == 0;
					if (flag2)
					{
						this.m_pException = new IncompleteDataException();
						this.Completed();
					}
					else
					{
						this.DoDataReading();
					}
				}
			}

			// Token: 0x060018D9 RID: 6361 RVA: 0x00099AD0 File Offset: 0x00098AD0
			private void DoDataReading()
			{
				try
				{
					for (;;)
					{
						bool flag = this.m_pOwner.BytesInReadBuffer == 0;
						if (flag)
						{
							bool flag2 = this.m_pOwner.BufferRead(true, new SmartStream.BufferCallback(this.Buffering_Completed));
							if (flag2)
							{
								break;
							}
							bool flag3 = this.m_pOwner.BytesInReadBuffer == 0;
							if (flag3)
							{
								goto Block_4;
							}
						}
						int num = (int)Math.Min(this.m_Count - this.m_BytesStored, (long)this.m_pOwner.BytesInReadBuffer);
						this.m_pStoreStream.Write(this.m_pOwner.m_pReadBuffer, this.m_pOwner.m_ReadBufferOffset, num);
						this.m_BytesStored += (long)num;
						this.m_pOwner.m_ReadBufferOffset += num;
						bool flag4 = this.m_Count == this.m_BytesStored;
						if (flag4)
						{
							goto Block_5;
						}
					}
					return;
					Block_4:
					throw new IncompleteDataException();
					Block_5:
					this.Completed();
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.Completed();
				}
			}

			// Token: 0x060018DA RID: 6362 RVA: 0x00099BE8 File Offset: 0x00098BE8
			private void Completed()
			{
				this.m_IsCompleted = true;
				this.m_pAsyncWaitHandle.Set();
				bool flag = this.m_pAsyncCallback != null;
				if (flag)
				{
					this.m_pAsyncCallback(this);
				}
			}

			// Token: 0x17000819 RID: 2073
			// (get) Token: 0x060018DB RID: 6363 RVA: 0x00099C28 File Offset: 0x00098C28
			public object AsyncState
			{
				get
				{
					return this.m_pAsyncState;
				}
			}

			// Token: 0x1700081A RID: 2074
			// (get) Token: 0x060018DC RID: 6364 RVA: 0x00099C40 File Offset: 0x00098C40
			public WaitHandle AsyncWaitHandle
			{
				get
				{
					return this.m_pAsyncWaitHandle;
				}
			}

			// Token: 0x1700081B RID: 2075
			// (get) Token: 0x060018DD RID: 6365 RVA: 0x00099C58 File Offset: 0x00098C58
			public bool CompletedSynchronously
			{
				get
				{
					return this.m_CompletedSynchronously;
				}
			}

			// Token: 0x1700081C RID: 2076
			// (get) Token: 0x060018DE RID: 6366 RVA: 0x00099C70 File Offset: 0x00098C70
			public bool IsCompleted
			{
				get
				{
					return this.m_IsCompleted;
				}
			}

			// Token: 0x1700081D RID: 2077
			// (get) Token: 0x060018DF RID: 6367 RVA: 0x00099C88 File Offset: 0x00098C88
			// (set) Token: 0x060018E0 RID: 6368 RVA: 0x00099CA0 File Offset: 0x00098CA0
			internal bool IsEndCalled
			{
				get
				{
					return this.m_IsEndCalled;
				}
				set
				{
					this.m_IsEndCalled = value;
				}
			}

			// Token: 0x1700081E RID: 2078
			// (get) Token: 0x060018E1 RID: 6369 RVA: 0x00099CAC File Offset: 0x00098CAC
			internal long BytesStored
			{
				get
				{
					return this.m_BytesStored;
				}
			}

			// Token: 0x1700081F RID: 2079
			// (get) Token: 0x060018E2 RID: 6370 RVA: 0x00099CC4 File Offset: 0x00098CC4
			internal Exception Exception
			{
				get
				{
					return this.m_pException;
				}
			}

			// Token: 0x04000ABE RID: 2750
			private SmartStream m_pOwner = null;

			// Token: 0x04000ABF RID: 2751
			private Stream m_pStoreStream = null;

			// Token: 0x04000AC0 RID: 2752
			private long m_Count = 0L;

			// Token: 0x04000AC1 RID: 2753
			private AsyncCallback m_pAsyncCallback = null;

			// Token: 0x04000AC2 RID: 2754
			private object m_pAsyncState = null;

			// Token: 0x04000AC3 RID: 2755
			private AutoResetEvent m_pAsyncWaitHandle = null;

			// Token: 0x04000AC4 RID: 2756
			private bool m_CompletedSynchronously = false;

			// Token: 0x04000AC5 RID: 2757
			private bool m_IsCompleted = false;

			// Token: 0x04000AC6 RID: 2758
			private bool m_IsEndCalled = false;

			// Token: 0x04000AC7 RID: 2759
			private long m_BytesStored = 0L;

			// Token: 0x04000AC8 RID: 2760
			private Exception m_pException = null;
		}
	}
}
