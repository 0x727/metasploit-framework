using System;
using System.IO;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x0200005E RID: 94
	public class Properties
	{
		// Token: 0x0600027D RID: 637 RVA: 0x0000AC38 File Offset: 0x00008E38
		public Properties()
		{
			this._properties = new Hashtable();
		}

		// Token: 0x0600027E RID: 638 RVA: 0x0000AC4D File Offset: 0x00008E4D
		public Properties(Properties defaultProp) : this()
		{
			this.PutAll(defaultProp._properties);
		}

		// Token: 0x0600027F RID: 639 RVA: 0x0000AC64 File Offset: 0x00008E64
		public void PutAll(Hashtable properties)
		{
			foreach (object key in properties.Keys)
			{
				this._properties.Put(key, properties[key]);
			}
		}

		// Token: 0x06000280 RID: 640 RVA: 0x0000ACCC File Offset: 0x00008ECC
		public void SetProperty(object key, object value)
		{
			this._properties.Put(key, value);
		}

		// Token: 0x06000281 RID: 641 RVA: 0x0000ACE0 File Offset: 0x00008EE0
		public object GetProperty(object key)
		{
			return this._properties.Keys.Contains(key) ? this._properties[key] : null;
		}

		// Token: 0x06000282 RID: 642 RVA: 0x0000AD14 File Offset: 0x00008F14
		public object GetProperty(object key, object def)
		{
			object obj = this._properties.Get(key);
			return obj ?? def;
		}

		// Token: 0x06000283 RID: 643 RVA: 0x0000AD3C File Offset: 0x00008F3C
		public void Load(InputStream input)
		{
			StreamReader streamReader = new StreamReader(input);
			while (!streamReader.EndOfStream)
			{
				string text = streamReader.ReadLine();
				bool flag = !string.IsNullOrEmpty(text);
				if (flag)
				{
					string[] array = text.Split(new char[]
					{
						'='
					});
					this._properties.Put(array[0], array[1]);
				}
			}
		}

		// Token: 0x06000284 RID: 644 RVA: 0x0000ADA4 File Offset: 0x00008FA4
		public void Store(OutputStream output)
		{
			StreamWriter streamWriter = new StreamWriter(output);
			foreach (object obj in this._properties.Keys)
			{
				string value = string.Format("{0}={1}", obj, this._properties[obj]);
				streamWriter.WriteLine(value);
			}
		}

		// Token: 0x06000285 RID: 645 RVA: 0x0000AE28 File Offset: 0x00009028
		public void Store(TextWriter output)
		{
			foreach (object obj in this._properties.Keys)
			{
				string value = string.Format("{0}={1}", obj, this._properties[obj]);
				output.WriteLine(value);
			}
		}

		// Token: 0x0400007B RID: 123
		protected Hashtable _properties;
	}
}
