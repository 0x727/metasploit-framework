using System;
using System.Collections;
using System.Collections.Generic;

namespace LumiSoft.Net.MIME
{
	// Token: 0x02000116 RID: 278
	public class MIME_EntityCollection : IEnumerable
	{
		// Token: 0x06000AE0 RID: 2784 RVA: 0x0004213F File Offset: 0x0004113F
		internal MIME_EntityCollection()
		{
			this.m_pCollection = new List<MIME_Entity>();
		}

		// Token: 0x06000AE1 RID: 2785 RVA: 0x00042164 File Offset: 0x00041164
		public void Add(MIME_Entity entity)
		{
			bool flag = entity == null;
			if (flag)
			{
				throw new ArgumentNullException("entity");
			}
			this.m_pCollection.Add(entity);
			this.m_IsModified = true;
		}

		// Token: 0x06000AE2 RID: 2786 RVA: 0x0004219C File Offset: 0x0004119C
		public void Insert(int index, MIME_Entity entity)
		{
			bool flag = entity == null;
			if (flag)
			{
				throw new ArgumentNullException("entity");
			}
			this.m_pCollection.Insert(index, entity);
			this.m_IsModified = true;
		}

		// Token: 0x06000AE3 RID: 2787 RVA: 0x000421D4 File Offset: 0x000411D4
		public void Remove(MIME_Entity entity)
		{
			bool flag = entity == null;
			if (flag)
			{
				throw new ArgumentNullException("field");
			}
			this.m_pCollection.Remove(entity);
			this.m_IsModified = true;
		}

		// Token: 0x06000AE4 RID: 2788 RVA: 0x0004220A File Offset: 0x0004120A
		public void Remove(int index)
		{
			this.m_pCollection.RemoveAt(index);
			this.m_IsModified = true;
		}

		// Token: 0x06000AE5 RID: 2789 RVA: 0x00042221 File Offset: 0x00041221
		public void Clear()
		{
			this.m_pCollection.Clear();
			this.m_IsModified = true;
		}

		// Token: 0x06000AE6 RID: 2790 RVA: 0x00042238 File Offset: 0x00041238
		public bool Contains(MIME_Entity entity)
		{
			bool flag = entity == null;
			if (flag)
			{
				throw new ArgumentNullException("entity");
			}
			return this.m_pCollection.Contains(entity);
		}

		// Token: 0x06000AE7 RID: 2791 RVA: 0x0004226A File Offset: 0x0004126A
		internal void SetModified(bool isModified)
		{
			this.m_IsModified = isModified;
		}

		// Token: 0x06000AE8 RID: 2792 RVA: 0x00042274 File Offset: 0x00041274
		public IEnumerator GetEnumerator()
		{
			return this.m_pCollection.GetEnumerator();
		}

		// Token: 0x17000398 RID: 920
		// (get) Token: 0x06000AE9 RID: 2793 RVA: 0x00042298 File Offset: 0x00041298
		public bool IsModified
		{
			get
			{
				bool isModified = this.m_IsModified;
				bool result;
				if (isModified)
				{
					result = true;
				}
				else
				{
					foreach (MIME_Entity mime_Entity in this.m_pCollection)
					{
						bool isModified2 = mime_Entity.IsModified;
						if (isModified2)
						{
							return true;
						}
					}
					result = false;
				}
				return result;
			}
		}

		// Token: 0x17000399 RID: 921
		// (get) Token: 0x06000AEA RID: 2794 RVA: 0x00042310 File Offset: 0x00041310
		public int Count
		{
			get
			{
				return this.m_pCollection.Count;
			}
		}

		// Token: 0x1700039A RID: 922
		public MIME_Entity this[int index]
		{
			get
			{
				return this.m_pCollection[index];
			}
		}

		// Token: 0x04000485 RID: 1157
		private bool m_IsModified = false;

		// Token: 0x04000486 RID: 1158
		private List<MIME_Entity> m_pCollection = null;
	}
}
