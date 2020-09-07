using System;
using System.Collections;

namespace LumiSoft.Net.Mime
{
	// Token: 0x02000161 RID: 353
	[Obsolete("See LumiSoft.Net.MIME or LumiSoft.Net.Mail namepaces for replacement.")]
	public class HeaderFieldParameterCollection : IEnumerable
	{
		// Token: 0x06000E2B RID: 3627 RVA: 0x000579B4 File Offset: 0x000569B4
		internal HeaderFieldParameterCollection(ParametizedHeaderField headerField)
		{
			this.m_pHeaderField = headerField;
		}

		// Token: 0x06000E2C RID: 3628 RVA: 0x000579CC File Offset: 0x000569CC
		public void Add(string parameterName, string parameterValue)
		{
			parameterName = parameterName.ToLower();
			Hashtable hashtable = this.m_pHeaderField.ParseParameters();
			bool flag = !hashtable.ContainsKey(parameterName);
			if (flag)
			{
				hashtable.Add(parameterName, parameterValue);
				this.m_pHeaderField.StoreParameters(this.m_pHeaderField.Value, hashtable);
				return;
			}
			throw new Exception(string.Concat(new string[]
			{
				"Header field '",
				this.m_pHeaderField.Name,
				"' parameter '",
				parameterName,
				"' already exists !"
			}));
		}

		// Token: 0x06000E2D RID: 3629 RVA: 0x00057A60 File Offset: 0x00056A60
		public void Remove(string parameterName)
		{
			parameterName = parameterName.ToLower();
			Hashtable hashtable = this.m_pHeaderField.ParseParameters();
			bool flag = !hashtable.ContainsKey(parameterName);
			if (flag)
			{
				hashtable.Remove(parameterName);
				this.m_pHeaderField.StoreParameters(this.m_pHeaderField.Value, hashtable);
			}
		}

		// Token: 0x06000E2E RID: 3630 RVA: 0x00057AB4 File Offset: 0x00056AB4
		public void Clear()
		{
			Hashtable hashtable = this.m_pHeaderField.ParseParameters();
			hashtable.Clear();
			this.m_pHeaderField.StoreParameters(this.m_pHeaderField.Value, hashtable);
		}

		// Token: 0x06000E2F RID: 3631 RVA: 0x00057AF0 File Offset: 0x00056AF0
		public bool Contains(string parameterName)
		{
			parameterName = parameterName.ToLower();
			Hashtable hashtable = this.m_pHeaderField.ParseParameters();
			return hashtable.ContainsKey(parameterName);
		}

		// Token: 0x06000E30 RID: 3632 RVA: 0x00057B20 File Offset: 0x00056B20
		public IEnumerator GetEnumerator()
		{
			Hashtable hashtable = this.m_pHeaderField.ParseParameters();
			HeaderFieldParameter[] array = new HeaderFieldParameter[hashtable.Count];
			int num = 0;
			foreach (object obj in hashtable)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				array[num] = new HeaderFieldParameter(dictionaryEntry.Key.ToString(), dictionaryEntry.Value.ToString());
				num++;
			}
			return array.GetEnumerator();
		}

		// Token: 0x170004B4 RID: 1204
		public string this[string parameterName]
		{
			get
			{
				parameterName = parameterName.ToLower();
				Hashtable hashtable = this.m_pHeaderField.ParseParameters();
				bool flag = !hashtable.ContainsKey(parameterName);
				if (flag)
				{
					throw new Exception("Specified parameter '" + parameterName + "' doesn't exist !");
				}
				return (string)hashtable[parameterName];
			}
			set
			{
				parameterName = parameterName.ToLower();
				Hashtable hashtable = this.m_pHeaderField.ParseParameters();
				bool flag = hashtable.ContainsKey(parameterName);
				if (flag)
				{
					hashtable[parameterName] = value;
					this.m_pHeaderField.StoreParameters(this.m_pHeaderField.Value, hashtable);
				}
			}
		}

		// Token: 0x170004B5 RID: 1205
		// (get) Token: 0x06000E33 RID: 3635 RVA: 0x00057C68 File Offset: 0x00056C68
		public int Count
		{
			get
			{
				return this.m_pHeaderField.ParseParameters().Count;
			}
		}

		// Token: 0x040005F2 RID: 1522
		private ParametizedHeaderField m_pHeaderField = null;
	}
}
