using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x02000204 RID: 516
	public class IMAP_r_u_Namespace : IMAP_r_u
	{
		// Token: 0x06001242 RID: 4674 RVA: 0x0006E184 File Offset: 0x0006D184
		public IMAP_r_u_Namespace(IMAP_Namespace_Entry[] personalNamespaces, IMAP_Namespace_Entry[] otherUsersNamespaces, IMAP_Namespace_Entry[] sharedNamespaces)
		{
			bool flag = personalNamespaces == null;
			if (flag)
			{
				throw new ArgumentNullException("personalNamespaces");
			}
			this.m_pPersonalNamespaces = personalNamespaces;
			this.m_pOtherUsersNamespaces = otherUsersNamespaces;
			this.m_pSharedNamespaces = sharedNamespaces;
		}

		// Token: 0x06001243 RID: 4675 RVA: 0x0006E1D8 File Offset: 0x0006D1D8
		public static IMAP_r_u_Namespace Parse(string response)
		{
			bool flag = response == null;
			if (flag)
			{
				throw new ArgumentNullException("response");
			}
			StringReader stringReader = new StringReader(response);
			stringReader.ReadWord();
			stringReader.ReadWord();
			stringReader.ReadToFirstChar();
			List<IMAP_Namespace_Entry> list = new List<IMAP_Namespace_Entry>();
			bool flag2 = stringReader.SourceString.StartsWith("(");
			if (flag2)
			{
				StringReader stringReader2 = new StringReader(stringReader.ReadParenthesized());
				while (stringReader2.Available > 0L)
				{
					string[] array = TextUtils.SplitQuotedString(stringReader2.ReadParenthesized(), ' ', true);
					list.Add(new IMAP_Namespace_Entry(array[0], array[1][0]));
				}
			}
			else
			{
				stringReader.ReadWord();
			}
			stringReader.ReadToFirstChar();
			List<IMAP_Namespace_Entry> list2 = new List<IMAP_Namespace_Entry>();
			bool flag3 = stringReader.SourceString.StartsWith("(");
			if (flag3)
			{
				StringReader stringReader3 = new StringReader(stringReader.ReadParenthesized());
				while (stringReader3.Available > 0L)
				{
					string[] array2 = TextUtils.SplitQuotedString(stringReader3.ReadParenthesized(), ' ', true);
					list2.Add(new IMAP_Namespace_Entry(array2[0], array2[1][0]));
				}
			}
			else
			{
				stringReader.ReadWord();
			}
			stringReader.ReadToFirstChar();
			List<IMAP_Namespace_Entry> list3 = new List<IMAP_Namespace_Entry>();
			bool flag4 = stringReader.SourceString.StartsWith("(");
			if (flag4)
			{
				StringReader stringReader4 = new StringReader(stringReader.ReadParenthesized());
				while (stringReader4.Available > 0L)
				{
					string[] array3 = TextUtils.SplitQuotedString(stringReader4.ReadParenthesized(), ' ', true);
					list3.Add(new IMAP_Namespace_Entry(array3[0], array3[1][0]));
				}
			}
			else
			{
				stringReader.ReadWord();
			}
			return new IMAP_r_u_Namespace(list.ToArray(), list2.ToArray(), list3.ToArray());
		}

		// Token: 0x06001244 RID: 4676 RVA: 0x0006E3A8 File Offset: 0x0006D3A8
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("* NAMESPACE ");
			bool flag = this.m_pPersonalNamespaces != null && this.m_pPersonalNamespaces.Length != 0;
			if (flag)
			{
				stringBuilder.Append("(");
				for (int i = 0; i < this.m_pPersonalNamespaces.Length; i++)
				{
					bool flag2 = i > 0;
					if (flag2)
					{
						stringBuilder.Append(" ");
					}
					stringBuilder.Append(string.Concat(new string[]
					{
						"(\"",
						this.m_pPersonalNamespaces[i].NamespaceName,
						"\" \"",
						this.m_pPersonalNamespaces[i].HierarchyDelimiter.ToString(),
						"\")"
					}));
				}
				stringBuilder.Append(")");
			}
			else
			{
				stringBuilder.Append("NIL");
			}
			stringBuilder.Append(" ");
			bool flag3 = this.m_pOtherUsersNamespaces != null && this.m_pOtherUsersNamespaces.Length != 0;
			if (flag3)
			{
				stringBuilder.Append("(");
				for (int j = 0; j < this.m_pOtherUsersNamespaces.Length; j++)
				{
					bool flag4 = j > 0;
					if (flag4)
					{
						stringBuilder.Append(" ");
					}
					stringBuilder.Append(string.Concat(new string[]
					{
						"(\"",
						this.m_pOtherUsersNamespaces[j].NamespaceName,
						"\" \"",
						this.m_pOtherUsersNamespaces[j].HierarchyDelimiter.ToString(),
						"\")"
					}));
				}
				stringBuilder.Append(")");
			}
			else
			{
				stringBuilder.Append("NIL");
			}
			stringBuilder.Append(" ");
			bool flag5 = this.m_pSharedNamespaces != null && this.m_pSharedNamespaces.Length != 0;
			if (flag5)
			{
				stringBuilder.Append("(");
				for (int k = 0; k < this.m_pSharedNamespaces.Length; k++)
				{
					bool flag6 = k > 0;
					if (flag6)
					{
						stringBuilder.Append(" ");
					}
					stringBuilder.Append(string.Concat(new string[]
					{
						"(\"",
						this.m_pSharedNamespaces[k].NamespaceName,
						"\" \"",
						this.m_pSharedNamespaces[k].HierarchyDelimiter.ToString(),
						"\")"
					}));
				}
				stringBuilder.Append(")");
			}
			else
			{
				stringBuilder.Append("NIL");
			}
			stringBuilder.Append("\r\n");
			return stringBuilder.ToString();
		}

		// Token: 0x17000615 RID: 1557
		// (get) Token: 0x06001245 RID: 4677 RVA: 0x0006E674 File Offset: 0x0006D674
		public IMAP_Namespace_Entry[] PersonalNamespaces
		{
			get
			{
				return this.m_pPersonalNamespaces;
			}
		}

		// Token: 0x17000616 RID: 1558
		// (get) Token: 0x06001246 RID: 4678 RVA: 0x0006E68C File Offset: 0x0006D68C
		public IMAP_Namespace_Entry[] OtherUsersNamespaces
		{
			get
			{
				return this.m_pOtherUsersNamespaces;
			}
		}

		// Token: 0x17000617 RID: 1559
		// (get) Token: 0x06001247 RID: 4679 RVA: 0x0006E6A4 File Offset: 0x0006D6A4
		public IMAP_Namespace_Entry[] SharedNamespaces
		{
			get
			{
				return this.m_pSharedNamespaces;
			}
		}

		// Token: 0x04000719 RID: 1817
		private IMAP_Namespace_Entry[] m_pPersonalNamespaces = null;

		// Token: 0x0400071A RID: 1818
		private IMAP_Namespace_Entry[] m_pOtherUsersNamespaces = null;

		// Token: 0x0400071B RID: 1819
		private IMAP_Namespace_Entry[] m_pSharedNamespaces = null;
	}
}
