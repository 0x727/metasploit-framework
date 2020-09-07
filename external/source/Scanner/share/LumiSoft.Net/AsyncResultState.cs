using System;
using System.Threading;

namespace LumiSoft.Net
{
	// Token: 0x0200001C RID: 28
	internal class AsyncResultState : IAsyncResult
	{
		// Token: 0x06000090 RID: 144 RVA: 0x00005934 File Offset: 0x00004934
		public AsyncResultState(object asyncObject, Delegate asyncDelegate, AsyncCallback callback, object state)
		{
			this.m_pAsyncObject = asyncObject;
			this.m_pAsyncDelegate = asyncDelegate;
			this.m_pCallback = callback;
			this.m_pState = state;
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00005990 File Offset: 0x00004990
		public void SetAsyncResult(IAsyncResult asyncResult)
		{
			bool flag = asyncResult == null;
			if (flag)
			{
				throw new ArgumentNullException("asyncResult");
			}
			this.m_pAsyncResult = asyncResult;
		}

		// Token: 0x06000092 RID: 146 RVA: 0x000059BC File Offset: 0x000049BC
		public void CompletedCallback(IAsyncResult ar)
		{
			bool flag = this.m_pCallback != null;
			if (flag)
			{
				this.m_pCallback(this);
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000093 RID: 147 RVA: 0x000059E8 File Offset: 0x000049E8
		public object AsyncObject
		{
			get
			{
				return this.m_pAsyncObject;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000094 RID: 148 RVA: 0x00005A00 File Offset: 0x00004A00
		public Delegate AsyncDelegate
		{
			get
			{
				return this.m_pAsyncDelegate;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000095 RID: 149 RVA: 0x00005A18 File Offset: 0x00004A18
		public IAsyncResult AsyncResult
		{
			get
			{
				return this.m_pAsyncResult;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000096 RID: 150 RVA: 0x00005A30 File Offset: 0x00004A30
		// (set) Token: 0x06000097 RID: 151 RVA: 0x00005A48 File Offset: 0x00004A48
		public bool IsEndCalled
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

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000098 RID: 152 RVA: 0x00005A54 File Offset: 0x00004A54
		public object AsyncState
		{
			get
			{
				return this.m_pState;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000099 RID: 153 RVA: 0x00005A6C File Offset: 0x00004A6C
		public WaitHandle AsyncWaitHandle
		{
			get
			{
				return this.m_pAsyncResult.AsyncWaitHandle;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600009A RID: 154 RVA: 0x00005A8C File Offset: 0x00004A8C
		public bool CompletedSynchronously
		{
			get
			{
				return this.m_pAsyncResult.CompletedSynchronously;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x0600009B RID: 155 RVA: 0x00005AAC File Offset: 0x00004AAC
		public bool IsCompleted
		{
			get
			{
				return this.m_pAsyncResult.IsCompleted;
			}
		}

		// Token: 0x04000056 RID: 86
		private object m_pAsyncObject = null;

		// Token: 0x04000057 RID: 87
		private Delegate m_pAsyncDelegate = null;

		// Token: 0x04000058 RID: 88
		private AsyncCallback m_pCallback = null;

		// Token: 0x04000059 RID: 89
		private object m_pState = null;

		// Token: 0x0400005A RID: 90
		private IAsyncResult m_pAsyncResult = null;

		// Token: 0x0400005B RID: 91
		private bool m_IsEndCalled = false;
	}
}
