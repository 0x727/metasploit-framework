using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000066 RID: 102
	internal class SynchronizedList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
	{
		// Token: 0x060002D2 RID: 722 RVA: 0x0000B8DE File Offset: 0x00009ADE
		public SynchronizedList(IList<T> list)
		{
			this._list = list;
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x0000B8F0 File Offset: 0x00009AF0
		public int IndexOf(T item)
		{
			IList<T> list = this._list;
			int result;
			lock (list)
			{
				result = this._list.IndexOf(item);
			}
			return result;
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x0000B93C File Offset: 0x00009B3C
		public void Insert(int index, T item)
		{
			IList<T> list = this._list;
			lock (list)
			{
				this._list.Insert(index, item);
			}
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x0000B98C File Offset: 0x00009B8C
		public void RemoveAt(int index)
		{
			IList<T> list = this._list;
			lock (list)
			{
				this._list.RemoveAt(index);
			}
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x0000B9D8 File Offset: 0x00009BD8
		void ICollection<T>.Add(T item)
		{
			IList<T> list = this._list;
			lock (list)
			{
				this._list.Add(item);
			}
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x0000BA24 File Offset: 0x00009C24
		void ICollection<T>.Clear()
		{
			IList<T> list = this._list;
			lock (list)
			{
				this._list.Clear();
			}
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x0000BA70 File Offset: 0x00009C70
		bool ICollection<T>.Contains(T item)
		{
			IList<T> list = this._list;
			bool result;
			lock (list)
			{
				result = this._list.Contains(item);
			}
			return result;
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x0000BABC File Offset: 0x00009CBC
		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			IList<T> list = this._list;
			lock (list)
			{
				this._list.CopyTo(array, arrayIndex);
			}
		}

		// Token: 0x060002DA RID: 730 RVA: 0x0000BB0C File Offset: 0x00009D0C
		bool ICollection<T>.Remove(T item)
		{
			IList<T> list = this._list;
			bool result;
			lock (list)
			{
				result = this._list.Remove(item);
			}
			return result;
		}

		// Token: 0x060002DB RID: 731 RVA: 0x0000BB58 File Offset: 0x00009D58
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return this._list.GetEnumerator();
		}

		// Token: 0x060002DC RID: 732 RVA: 0x0000BB78 File Offset: 0x00009D78
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this._list.GetEnumerator();
		}

		// Token: 0x17000026 RID: 38
		public T this[int index]
		{
			get
			{
				IList<T> list = this._list;
				T result;
				lock (list)
				{
					result = this._list[index];
				}
				return result;
			}
			set
			{
				IList<T> list = this._list;
				lock (list)
				{
					this._list[index] = value;
				}
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060002DF RID: 735 RVA: 0x0000BC34 File Offset: 0x00009E34
		int ICollection<T>.Count
		{
			get
			{
				IList<T> list = this._list;
				int count;
				lock (list)
				{
					count = this._list.Count;
				}
				return count;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060002E0 RID: 736 RVA: 0x0000BC80 File Offset: 0x00009E80
		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return this._list.IsReadOnly;
			}
		}

		// Token: 0x04000086 RID: 134
		private IList<T> _list;
	}
}
