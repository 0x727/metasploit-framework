using System;
using System.Text;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001C1 RID: 449
	public class IMAP_t_MsgFlags
	{
		// Token: 0x06001102 RID: 4354 RVA: 0x00068E1C File Offset: 0x00067E1C
		public IMAP_t_MsgFlags(params string[] flags)
		{
			this.m_pFlags = new KeyValueCollection<string, string>();
			bool flag = flags != null;
			if (flag)
			{
				foreach (string text in flags)
				{
					bool flag2 = !string.IsNullOrEmpty(text);
					if (flag2)
					{
						this.m_pFlags.Add(text.ToLower(), text);
					}
				}
			}
		}

		// Token: 0x06001103 RID: 4355 RVA: 0x00068E88 File Offset: 0x00067E88
		public static IMAP_t_MsgFlags Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			value = value.Trim();
			bool flag2 = value.StartsWith("(") && value.EndsWith(")");
			if (flag2)
			{
				value = value.Substring(1, value.Length - 2);
			}
			string[] flags = new string[0];
			bool flag3 = !string.IsNullOrEmpty(value);
			if (flag3)
			{
				flags = value.Split(new char[]
				{
					' '
				});
			}
			return new IMAP_t_MsgFlags(flags);
		}

		// Token: 0x06001104 RID: 4356 RVA: 0x00068F18 File Offset: 0x00067F18
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			string[] array = this.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				bool flag = i > 0;
				if (flag)
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(array[i]);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06001105 RID: 4357 RVA: 0x00068F78 File Offset: 0x00067F78
		public bool Contains(string flag)
		{
			bool flag2 = flag == null;
			if (flag2)
			{
				throw new ArgumentNullException("flag");
			}
			return this.m_pFlags.ContainsKey(flag.ToLower());
		}

		// Token: 0x06001106 RID: 4358 RVA: 0x00068FB0 File Offset: 0x00067FB0
		public string[] ToArray()
		{
			return this.m_pFlags.ToArray();
		}

		// Token: 0x170005C5 RID: 1477
		// (get) Token: 0x06001107 RID: 4359 RVA: 0x00068FD0 File Offset: 0x00067FD0
		public int Count
		{
			get
			{
				return this.m_pFlags.Count;
			}
		}

		// Token: 0x040006C9 RID: 1737
		public static readonly string Seen = "\\Seen";

		// Token: 0x040006CA RID: 1738
		public static readonly string Answered = "\\Answered";

		// Token: 0x040006CB RID: 1739
		public static readonly string Flagged = "\\Flagged";

		// Token: 0x040006CC RID: 1740
		public static readonly string Deleted = "\\Deleted";

		// Token: 0x040006CD RID: 1741
		public static readonly string Draft = "\\Draft";

		// Token: 0x040006CE RID: 1742
		public static readonly string Recent = "\\Recent";

		// Token: 0x040006CF RID: 1743
		private KeyValueCollection<string, string> m_pFlags = null;
	}
}
