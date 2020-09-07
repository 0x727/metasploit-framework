using System;
using System.Collections;
using System.Collections.Generic;

namespace LumiSoft.Net
{
	// Token: 0x0200000B RID: 11
	public class KeyValueCollection<K, V> : IEnumerable
	{
		// Token: 0x0600002A RID: 42 RVA: 0x00002C8A File Offset: 0x00001C8A
		public KeyValueCollection()
		{
			this.m_pDictionary = new Dictionary<K, V>();
			this.m_pList = new List<V>();
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002CB8 File Offset: 0x00001CB8
		public void Add(K key, V value)
		{
			this.m_pDictionary.Add(key, value);
			this.m_pList.Add(value);
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002CD8 File Offset: 0x00001CD8
		public bool Remove(K key)
		{
			V item = default(V);
			bool flag = this.m_pDictionary.TryGetValue(key, out item);
			bool result;
			if (flag)
			{
				this.m_pDictionary.Remove(key);
				this.m_pList.Remove(item);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002D25 File Offset: 0x00001D25
		public void Clear()
		{
			this.m_pDictionary.Clear();
			this.m_pList.Clear();
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002D40 File Offset: 0x00001D40
		public bool ContainsKey(K key)
		{
			return this.m_pDictionary.ContainsKey(key);
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002D60 File Offset: 0x00001D60
		public bool TryGetValue(K key, out V value)
		{
			return this.m_pDictionary.TryGetValue(key, out value);
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002D80 File Offset: 0x00001D80
		public bool TryGetValueAt(int index, out V value)
		{
			value = default(V);
			bool flag = this.m_pList.Count > 0 && index >= 0 && index < this.m_pList.Count;
			bool result;
			if (flag)
			{
				value = this.m_pList[index];
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002DD8 File Offset: 0x00001DD8
		public V[] ToArray()
		{
			List<V> pList = this.m_pList;
			V[] result;
			lock (pList)
			{
				result = this.m_pList.ToArray();
			}
			return result;
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002E24 File Offset: 0x00001E24
		public IEnumerator GetEnumerator()
		{
			return this.m_pList.GetEnumerator();
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000033 RID: 51 RVA: 0x00002E48 File Offset: 0x00001E48
		public int Count
		{
			get
			{
				return this.m_pList.Count;
			}
		}

		// Token: 0x1700000C RID: 12
		public V this[K key]
		{
			get
			{
				return this.m_pDictionary[key];
			}
		}

		// Token: 0x04000026 RID: 38
		private Dictionary<K, V> m_pDictionary = null;

		// Token: 0x04000027 RID: 39
		private List<V> m_pList = null;
	}
}
