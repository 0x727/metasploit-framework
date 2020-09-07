using System;
using System.Collections.Generic;

namespace LumiSoft.Net
{
	// Token: 0x0200001E RID: 30
	public class CircleCollection<T>
	{
		// Token: 0x060000AB RID: 171 RVA: 0x00005DDA File Offset: 0x00004DDA
		public CircleCollection()
		{
			this.m_pItems = new List<T>();
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00005E00 File Offset: 0x00004E00
		public void Add(T[] items)
		{
			bool flag = items == null;
			if (flag)
			{
				throw new ArgumentNullException("items");
			}
			foreach (T item in items)
			{
				this.Add(item);
			}
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00005E48 File Offset: 0x00004E48
		public void Add(T item)
		{
			bool flag = item == null;
			if (flag)
			{
				throw new ArgumentNullException("item");
			}
			this.m_pItems.Add(item);
			this.m_Index = 0;
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00005E84 File Offset: 0x00004E84
		public void Remove(T item)
		{
			bool flag = item == null;
			if (flag)
			{
				throw new ArgumentNullException("item");
			}
			this.m_pItems.Remove(item);
			this.m_Index = 0;
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00005EBF File Offset: 0x00004EBF
		public void Clear()
		{
			this.m_pItems.Clear();
			this.m_Index = 0;
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00005ED8 File Offset: 0x00004ED8
		public bool Contains(T item)
		{
			return this.m_pItems.Contains(item);
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00005EF8 File Offset: 0x00004EF8
		public T Next()
		{
			bool flag = this.m_pItems.Count == 0;
			if (flag)
			{
				throw new InvalidOperationException("There is no items in the collection.");
			}
			List<T> pItems = this.m_pItems;
			T result;
			lock (pItems)
			{
				T t = this.m_pItems[this.m_Index];
				this.m_Index++;
				bool flag3 = this.m_Index >= this.m_pItems.Count;
				if (flag3)
				{
					this.m_Index = 0;
				}
				result = t;
			}
			return result;
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00005FA0 File Offset: 0x00004FA0
		public T[] ToArray()
		{
			List<T> pItems = this.m_pItems;
			T[] result;
			lock (pItems)
			{
				result = this.m_pItems.ToArray();
			}
			return result;
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00005FEC File Offset: 0x00004FEC
		public T[] ToCurrentOrderArray()
		{
			List<T> pItems = this.m_pItems;
			T[] result;
			lock (pItems)
			{
				int num = this.m_Index;
				T[] array = new T[this.m_pItems.Count];
				for (int i = 0; i < this.m_pItems.Count; i++)
				{
					array[i] = this.m_pItems[num];
					num++;
					bool flag2 = num >= this.m_pItems.Count;
					if (flag2)
					{
						num = 0;
					}
				}
				result = array;
			}
			return result;
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000B4 RID: 180 RVA: 0x0000609C File Offset: 0x0000509C
		public int Count
		{
			get
			{
				return this.m_pItems.Count;
			}
		}

		// Token: 0x1700002A RID: 42
		public T this[int index]
		{
			get
			{
				return this.m_pItems[index];
			}
		}

		// Token: 0x04000062 RID: 98
		private List<T> m_pItems = null;

		// Token: 0x04000063 RID: 99
		private int m_Index = 0;
	}
}
