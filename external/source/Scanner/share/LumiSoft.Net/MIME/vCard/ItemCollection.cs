using System;
using System.Collections;
using System.Collections.Generic;

namespace LumiSoft.Net.Mime.vCard
{
	// Token: 0x02000172 RID: 370
	public class ItemCollection : IEnumerable
	{
		// Token: 0x06000F02 RID: 3842 RVA: 0x0005D8AC File Offset: 0x0005C8AC
		internal ItemCollection(vCard card)
		{
			this.m_pCard = card;
			this.m_pItems = new List<Item>();
		}

		// Token: 0x06000F03 RID: 3843 RVA: 0x0005D8D8 File Offset: 0x0005C8D8
		public Item Add(string name, string parametes, string value)
		{
			Item item = new Item(this.m_pCard, name, parametes, value);
			this.m_pItems.Add(item);
			return item;
		}

		// Token: 0x06000F04 RID: 3844 RVA: 0x0005D908 File Offset: 0x0005C908
		public void Remove(string name)
		{
			for (int i = 0; i < this.m_pItems.Count; i++)
			{
				bool flag = this.m_pItems[i].Name.ToLower() == name.ToLower();
				if (flag)
				{
					this.m_pItems.RemoveAt(i);
					i--;
				}
			}
		}

		// Token: 0x06000F05 RID: 3845 RVA: 0x0005D96A File Offset: 0x0005C96A
		public void Remove(Item item)
		{
			this.m_pItems.Remove(item);
		}

		// Token: 0x06000F06 RID: 3846 RVA: 0x0005D97A File Offset: 0x0005C97A
		public void Clear()
		{
			this.m_pItems.Clear();
		}

		// Token: 0x06000F07 RID: 3847 RVA: 0x0005D98C File Offset: 0x0005C98C
		public Item GetFirst(string name)
		{
			foreach (Item item in this.m_pItems)
			{
				bool flag = item.Name.ToLower() == name.ToLower();
				if (flag)
				{
					return item;
				}
			}
			return null;
		}

		// Token: 0x06000F08 RID: 3848 RVA: 0x0005DA04 File Offset: 0x0005CA04
		public Item[] Get(string name)
		{
			List<Item> list = new List<Item>();
			foreach (Item item in this.m_pItems)
			{
				bool flag = item.Name.ToLower() == name.ToLower();
				if (flag)
				{
					list.Add(item);
				}
			}
			return list.ToArray();
		}

		// Token: 0x06000F09 RID: 3849 RVA: 0x0005DA8C File Offset: 0x0005CA8C
		public void SetDecodedValue(string name, string value)
		{
			bool flag = value == null;
			if (flag)
			{
				this.Remove(name);
			}
			else
			{
				Item item = this.GetFirst(name);
				bool flag2 = item != null;
				if (flag2)
				{
					item.SetDecodedValue(value);
				}
				else
				{
					item = new Item(this.m_pCard, name, "", "");
					this.m_pItems.Add(item);
					item.SetDecodedValue(value);
				}
			}
		}

		// Token: 0x06000F0A RID: 3850 RVA: 0x0005DAF7 File Offset: 0x0005CAF7
		public void SetValue(string name, string value)
		{
			this.SetValue(name, "", value);
		}

		// Token: 0x06000F0B RID: 3851 RVA: 0x0005DB08 File Offset: 0x0005CB08
		public void SetValue(string name, string parametes, string value)
		{
			bool flag = value == null;
			if (flag)
			{
				this.Remove(name);
			}
			else
			{
				Item first = this.GetFirst(name);
				bool flag2 = first != null;
				if (flag2)
				{
					first.Value = value;
				}
				else
				{
					this.m_pItems.Add(new Item(this.m_pCard, name, parametes, value));
				}
			}
		}

		// Token: 0x06000F0C RID: 3852 RVA: 0x0005DB64 File Offset: 0x0005CB64
		public IEnumerator GetEnumerator()
		{
			return this.m_pItems.GetEnumerator();
		}

		// Token: 0x170004FC RID: 1276
		// (get) Token: 0x06000F0D RID: 3853 RVA: 0x0005DB88 File Offset: 0x0005CB88
		public int Count
		{
			get
			{
				return this.m_pItems.Count;
			}
		}

		// Token: 0x0400063C RID: 1596
		private vCard m_pCard = null;

		// Token: 0x0400063D RID: 1597
		private List<Item> m_pItems = null;
	}
}
