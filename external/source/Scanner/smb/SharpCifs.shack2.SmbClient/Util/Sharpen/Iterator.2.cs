using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x0200004F RID: 79
	public abstract class Iterator<T> : IEnumerator<T>, IDisposable, IEnumerator, ITerator
	{
		// Token: 0x060001E8 RID: 488 RVA: 0x00009164 File Offset: 0x00007364
		object ITerator.Next()
		{
			return this.Next();
		}

		// Token: 0x060001E9 RID: 489
		public abstract bool HasNext();

		// Token: 0x060001EA RID: 490
		public abstract T Next();

		// Token: 0x060001EB RID: 491
		public abstract void Remove();

		// Token: 0x060001EC RID: 492 RVA: 0x00009184 File Offset: 0x00007384
		bool IEnumerator.MoveNext()
		{
			bool flag = this.HasNext();
			bool result;
			if (flag)
			{
				this._lastValue = this.Next();
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x060001ED RID: 493 RVA: 0x00002380 File Offset: 0x00000580
		void IEnumerator.Reset()
		{
			throw new NotImplementedException();
		}

		// Token: 0x060001EE RID: 494 RVA: 0x00008663 File Offset: 0x00006863
		void IDisposable.Dispose()
		{
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x060001EF RID: 495 RVA: 0x000091B4 File Offset: 0x000073B4
		T IEnumerator<T>.Current
		{
			get
			{
				return this._lastValue;
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060001F0 RID: 496 RVA: 0x000091CC File Offset: 0x000073CC
		object IEnumerator.Current
		{
			get
			{
				return this._lastValue;
			}
		}

		// Token: 0x04000061 RID: 97
		private T _lastValue;
	}
}
