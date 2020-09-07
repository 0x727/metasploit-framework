using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000013 RID: 19
	public abstract class AbstractMap<T, TU> : IDictionary<T, TU>, ICollection<KeyValuePair<T, TU>>, IEnumerable<KeyValuePair<T, TU>>, IEnumerable
	{
		// Token: 0x060000A4 RID: 164 RVA: 0x00006358 File Offset: 0x00004558
		public virtual void Clear()
		{
			this.EntrySet().Clear();
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00006368 File Offset: 0x00004568
		public virtual bool ContainsKey(object name)
		{
			return this.EntrySet().Any(delegate(KeyValuePair<T, TU> p)
			{
				T key = p.Key;
				return key.Equals((T)((object)name));
			});
		}

		// Token: 0x060000A6 RID: 166
		public abstract ICollection<KeyValuePair<T, TU>> EntrySet();

		// Token: 0x060000A7 RID: 167 RVA: 0x000063A0 File Offset: 0x000045A0
		public virtual TU Get(object key)
		{
			return (from p in this.EntrySet().Where(delegate(KeyValuePair<T, TU> p)
			{
				T key2 = p.Key;
				return key2.Equals(key);
			})
			select p.Value).FirstOrDefault<TU>();
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00006400 File Offset: 0x00004600
		protected virtual IEnumerator<KeyValuePair<T, TU>> InternalGetEnumerator()
		{
			return this.EntrySet().GetEnumerator();
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00006420 File Offset: 0x00004620
		public virtual bool IsEmpty()
		{
			return !this.EntrySet().Any<KeyValuePair<T, TU>>();
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00006440 File Offset: 0x00004640
		public virtual TU Put(T key, TU value)
		{
			throw new NotSupportedException();
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00006448 File Offset: 0x00004648
		public virtual TU Remove(object key)
		{
			Iterator<TU> iterator = this.EntrySet() as Iterator<TU>;
			bool flag = iterator == null;
			if (flag)
			{
				throw new NotSupportedException();
			}
			while (iterator.HasNext())
			{
				TU result = iterator.Next();
				bool flag2 = result.Equals((T)((object)key));
				if (flag2)
				{
					iterator.Remove();
					return result;
				}
			}
			return default(TU);
		}

		// Token: 0x060000AC RID: 172 RVA: 0x000064C1 File Offset: 0x000046C1
		void ICollection<KeyValuePair<T, TU>>.Add(KeyValuePair<T, TU> item)
		{
			this.Put(item.Key, item.Value);
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00002380 File Offset: 0x00000580
		bool ICollection<KeyValuePair<T, TU>>.Contains(KeyValuePair<T, TU> item)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060000AE RID: 174 RVA: 0x000064D9 File Offset: 0x000046D9
		void ICollection<KeyValuePair<T, TU>>.CopyTo(KeyValuePair<T, TU>[] array, int arrayIndex)
		{
			this.EntrySet().CopyTo(array, arrayIndex);
		}

		// Token: 0x060000AF RID: 175 RVA: 0x000064EC File Offset: 0x000046EC
		bool ICollection<KeyValuePair<T, TU>>.Remove(KeyValuePair<T, TU> item)
		{
			this.Remove(item.Key);
			return true;
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00006512 File Offset: 0x00004712
		void IDictionary<T, TU>.Add(T key, TU value)
		{
			this.Put(key, value);
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00006520 File Offset: 0x00004720
		bool IDictionary<T, TU>.ContainsKey(T key)
		{
			return this.ContainsKey(key);
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00006540 File Offset: 0x00004740
		bool IDictionary<T, TU>.Remove(T key)
		{
			bool flag = this.ContainsKey(key);
			bool result;
			if (flag)
			{
				this.Remove(key);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00006578 File Offset: 0x00004778
		bool IDictionary<T, TU>.TryGetValue(T key, out TU value)
		{
			bool flag = this.ContainsKey(key);
			bool result;
			if (flag)
			{
				value = this.Get(key);
				result = true;
			}
			else
			{
				value = default(TU);
				result = false;
			}
			return result;
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x000065BC File Offset: 0x000047BC
		IEnumerator<KeyValuePair<T, TU>> IEnumerable<KeyValuePair<T, TU>>.GetEnumerator()
		{
			return this.InternalGetEnumerator();
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x000065D4 File Offset: 0x000047D4
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.InternalGetEnumerator();
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x060000B6 RID: 182 RVA: 0x000065EC File Offset: 0x000047EC
		public virtual int Count
		{
			get
			{
				return this.EntrySet().Count;
			}
		}

		// Token: 0x17000007 RID: 7
		public TU this[T key]
		{
			get
			{
				return this.Get(key);
			}
			set
			{
				this.Put(key, value);
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x060000B9 RID: 185 RVA: 0x0000662C File Offset: 0x0000482C
		public virtual IEnumerable<T> Keys
		{
			get
			{
				return from p in this.EntrySet()
				select p.Key;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x060000BA RID: 186 RVA: 0x00006668 File Offset: 0x00004868
		int ICollection<KeyValuePair<T, TU>>.Count
		{
			get
			{
				return this.Count;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x060000BB RID: 187 RVA: 0x00006680 File Offset: 0x00004880
		bool ICollection<KeyValuePair<T, TU>>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x060000BC RID: 188 RVA: 0x00006694 File Offset: 0x00004894
		ICollection<T> IDictionary<T, TU>.Keys
		{
			get
			{
				return this.Keys.ToList<T>();
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x060000BD RID: 189 RVA: 0x000066B4 File Offset: 0x000048B4
		ICollection<TU> IDictionary<T, TU>.Values
		{
			get
			{
				return this.Values.ToList<TU>();
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x060000BE RID: 190 RVA: 0x000066D4 File Offset: 0x000048D4
		public virtual IEnumerable<TU> Values
		{
			get
			{
				return from p in this.EntrySet()
				select p.Value;
			}
		}
	}
}
