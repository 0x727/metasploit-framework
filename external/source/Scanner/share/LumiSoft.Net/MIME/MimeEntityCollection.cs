using System;
using System.Collections;
using System.Collections.Generic;

namespace LumiSoft.Net.Mime
{
	// Token: 0x02000167 RID: 359
	[Obsolete("See LumiSoft.Net.MIME or LumiSoft.Net.Mail namepaces for replacement.")]
	public class MimeEntityCollection : IEnumerable
	{
		// Token: 0x06000EA0 RID: 3744 RVA: 0x0005AC2E File Offset: 0x00059C2E
		internal MimeEntityCollection(MimeEntity ownerEntity)
		{
			this.m_pOwnerEntity = ownerEntity;
			this.m_pEntities = new List<MimeEntity>();
		}

		// Token: 0x06000EA1 RID: 3745 RVA: 0x0005AC58 File Offset: 0x00059C58
		public MimeEntity Add()
		{
			MimeEntity mimeEntity = new MimeEntity();
			this.Add(mimeEntity);
			return mimeEntity;
		}

		// Token: 0x06000EA2 RID: 3746 RVA: 0x0005AC7C File Offset: 0x00059C7C
		public void Add(MimeEntity entity)
		{
			bool flag = (this.m_pOwnerEntity.ContentType & MediaType_enum.Multipart) == (MediaType_enum)0;
			if (flag)
			{
				throw new Exception("You don't have Content-Type: multipart/xxx. Only Content-Type: multipart/xxx can have nested mime entities !");
			}
			bool flag2 = this.m_pOwnerEntity.ContentType_Boundary == null || this.m_pOwnerEntity.ContentType_Boundary.Length == 0;
			if (flag2)
			{
				throw new Exception("Please specify Boundary property first !");
			}
			this.m_pEntities.Add(entity);
		}

		// Token: 0x06000EA3 RID: 3747 RVA: 0x0005ACF0 File Offset: 0x00059CF0
		public void Insert(int index, MimeEntity entity)
		{
			bool flag = (this.m_pOwnerEntity.ContentType & MediaType_enum.Multipart) == (MediaType_enum)0;
			if (flag)
			{
				throw new Exception("You don't have Content-Type: multipart/xxx. Only Content-Type: multipart/xxx can have nested mime entities !");
			}
			bool flag2 = this.m_pOwnerEntity.ContentType_Boundary == null || this.m_pOwnerEntity.ContentType_Boundary.Length == 0;
			if (flag2)
			{
				throw new Exception("Please specify Boundary property first !");
			}
			this.m_pEntities.Insert(index, entity);
		}

		// Token: 0x06000EA4 RID: 3748 RVA: 0x0005AD63 File Offset: 0x00059D63
		public void Remove(int index)
		{
			this.m_pEntities.RemoveAt(index);
		}

		// Token: 0x06000EA5 RID: 3749 RVA: 0x0005AD73 File Offset: 0x00059D73
		public void Remove(MimeEntity entity)
		{
			this.m_pEntities.Remove(entity);
		}

		// Token: 0x06000EA6 RID: 3750 RVA: 0x0005AD83 File Offset: 0x00059D83
		public void Clear()
		{
			this.m_pEntities.Clear();
		}

		// Token: 0x06000EA7 RID: 3751 RVA: 0x0005AD94 File Offset: 0x00059D94
		public bool Contains(MimeEntity entity)
		{
			return this.m_pEntities.Contains(entity);
		}

		// Token: 0x06000EA8 RID: 3752 RVA: 0x0005ADB4 File Offset: 0x00059DB4
		public IEnumerator GetEnumerator()
		{
			return this.m_pEntities.GetEnumerator();
		}

		// Token: 0x170004E1 RID: 1249
		public MimeEntity this[int index]
		{
			get
			{
				return this.m_pEntities[index];
			}
		}

		// Token: 0x170004E2 RID: 1250
		// (get) Token: 0x06000EAA RID: 3754 RVA: 0x0005ADF8 File Offset: 0x00059DF8
		public int Count
		{
			get
			{
				return this.m_pEntities.Count;
			}
		}

		// Token: 0x04000615 RID: 1557
		private MimeEntity m_pOwnerEntity = null;

		// Token: 0x04000616 RID: 1558
		private List<MimeEntity> m_pEntities = null;
	}
}
