using System;
using System.Collections.Generic;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x0200001E RID: 30
	internal class EnumeratorWrapper<T> : Iterator<T>
	{
		// Token: 0x06000100 RID: 256 RVA: 0x000070AE File Offset: 0x000052AE
		public EnumeratorWrapper(object collection, IEnumerator<T> e)
		{
			this._e = e;
			this._collection = collection;
			this._more = e.MoveNext();
		}

		// Token: 0x06000101 RID: 257 RVA: 0x000070D4 File Offset: 0x000052D4
		public override bool HasNext()
		{
			return this._more;
		}

		// Token: 0x06000102 RID: 258 RVA: 0x000070EC File Offset: 0x000052EC
		public override T Next()
		{
			bool flag = !this._more;
			if (flag)
			{
				throw new NoSuchElementException();
			}
			this._lastVal = this._e.Current;
			this._more = this._e.MoveNext();
			return this._lastVal;
		}

		// Token: 0x06000103 RID: 259 RVA: 0x0000713C File Offset: 0x0000533C
		public override void Remove()
		{
			ICollection<T> collection = this._collection as ICollection<T>;
			bool flag = collection == null;
			if (flag)
			{
				throw new NotSupportedException();
			}
			bool flag2 = this._more && !this._copied;
			if (flag2)
			{
				List<T> list = new List<T>();
				do
				{
					list.Add(this._e.Current);
				}
				while (this._e.MoveNext());
				this._e = list.GetEnumerator();
				this._e.MoveNext();
				this._copied = true;
			}
			collection.Remove(this._lastVal);
		}

		// Token: 0x04000053 RID: 83
		private object _collection;

		// Token: 0x04000054 RID: 84
		private IEnumerator<T> _e;

		// Token: 0x04000055 RID: 85
		private T _lastVal;

		// Token: 0x04000056 RID: 86
		private bool _more;

		// Token: 0x04000057 RID: 87
		private bool _copied;
	}
}
