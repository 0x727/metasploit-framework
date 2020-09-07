using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x0200001C RID: 28
	internal class ConcurrentHashMap<T, TU> : AbstractMap<T, TU>, IConcurrentMap<T, TU>, IDictionary<T, TU>, ICollection<KeyValuePair<T, TU>>, IEnumerable<KeyValuePair<T, TU>>, IEnumerable
	{
		// Token: 0x060000EA RID: 234 RVA: 0x00006C5F File Offset: 0x00004E5F
		public ConcurrentHashMap()
		{
			this._table = new Dictionary<T, TU>();
		}

		// Token: 0x060000EB RID: 235 RVA: 0x00006C74 File Offset: 0x00004E74
		public ConcurrentHashMap(int initialCapacity, float loadFactor, int concurrencyLevel)
		{
			this._table = new Dictionary<T, TU>(initialCapacity);
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00006C8C File Offset: 0x00004E8C
		public override void Clear()
		{
			Dictionary<T, TU> table = this._table;
			lock (table)
			{
				this._table = new Dictionary<T, TU>();
			}
		}

		// Token: 0x060000ED RID: 237 RVA: 0x00006CD8 File Offset: 0x00004ED8
		public override bool ContainsKey(object name)
		{
			return this._table.ContainsKey((T)((object)name));
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00006CFC File Offset: 0x00004EFC
		public override ICollection<KeyValuePair<T, TU>> EntrySet()
		{
			return this;
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00006D10 File Offset: 0x00004F10
		public override TU Get(object key)
		{
			TU result;
			this._table.TryGetValue((T)((object)key), out result);
			return result;
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00006D38 File Offset: 0x00004F38
		protected override IEnumerator<KeyValuePair<T, TU>> InternalGetEnumerator()
		{
			return this._table.GetEnumerator();
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00006D5C File Offset: 0x00004F5C
		public override bool IsEmpty()
		{
			return this._table.Count == 0;
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00006D7C File Offset: 0x00004F7C
		public override TU Put(T key, TU value)
		{
			Dictionary<T, TU> table = this._table;
			TU result;
			lock (table)
			{
				TU tu = this.Get(key);
				Dictionary<T, TU> dictionary = new Dictionary<T, TU>(this._table);
				dictionary[key] = value;
				this._table = dictionary;
				result = tu;
			}
			return result;
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00006DE8 File Offset: 0x00004FE8
		public TU PutIfAbsent(T key, TU value)
		{
			Dictionary<T, TU> table = this._table;
			TU result;
			lock (table)
			{
				bool flag2 = !this.ContainsKey(key);
				if (flag2)
				{
					Dictionary<T, TU> dictionary = new Dictionary<T, TU>(this._table);
					dictionary[key] = value;
					this._table = dictionary;
					result = value;
				}
				else
				{
					result = this.Get(key);
				}
			}
			return result;
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00006E6C File Offset: 0x0000506C
		public override TU Remove(object key)
		{
			Dictionary<T, TU> table = this._table;
			TU result;
			lock (table)
			{
				TU tu = this.Get((T)((object)key));
				Dictionary<T, TU> dictionary = new Dictionary<T, TU>(this._table);
				dictionary.Remove((T)((object)key));
				this._table = dictionary;
				result = tu;
			}
			return result;
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x00006EE4 File Offset: 0x000050E4
		public bool Remove(object key, object value)
		{
			Dictionary<T, TU> table = this._table;
			bool result;
			lock (table)
			{
				bool flag2 = this.ContainsKey(key) && value.Equals(this.Get(key));
				if (flag2)
				{
					Dictionary<T, TU> dictionary = new Dictionary<T, TU>(this._table);
					dictionary.Remove((T)((object)key));
					this._table = dictionary;
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x00006F70 File Offset: 0x00005170
		public bool Replace(T key, TU oldValue, TU newValue)
		{
			Dictionary<T, TU> table = this._table;
			bool result;
			lock (table)
			{
				bool flag2 = this.ContainsKey(key) && oldValue.Equals(this.Get(key));
				if (flag2)
				{
					Dictionary<T, TU> dictionary = new Dictionary<T, TU>(this._table);
					dictionary[key] = newValue;
					this._table = dictionary;
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x060000F7 RID: 247 RVA: 0x00007008 File Offset: 0x00005208
		public override IEnumerable<T> Keys
		{
			get
			{
				return this._table.Keys;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x060000F8 RID: 248 RVA: 0x00007028 File Offset: 0x00005228
		public override IEnumerable<TU> Values
		{
			get
			{
				return this._table.Values;
			}
		}

		// Token: 0x04000050 RID: 80
		private Dictionary<T, TU> _table;
	}
}
