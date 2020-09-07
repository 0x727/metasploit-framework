using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using LumiSoft.Net.IO;

namespace LumiSoft.Net.MIME
{
	// Token: 0x02000117 RID: 279
	public class MIME_Message : MIME_Entity
	{
		// Token: 0x06000AED RID: 2797 RVA: 0x00042360 File Offset: 0x00041360
		public static MIME_Message ParseFromFile(string file)
		{
			bool flag = file == null;
			if (flag)
			{
				throw new ArgumentNullException("file");
			}
			bool flag2 = file == "";
			if (flag2)
			{
				throw new ArgumentException("Argument 'file' value must be specified.");
			}
			MIME_Message result;
			using (FileStream fileStream = File.OpenRead(file))
			{
				result = MIME_Message.ParseFromStream(fileStream, Encoding.UTF8);
			}
			return result;
		}

		// Token: 0x06000AEE RID: 2798 RVA: 0x000423D0 File Offset: 0x000413D0
		public static MIME_Message ParseFromFile(string file, Encoding headerEncoding)
		{
			bool flag = file == null;
			if (flag)
			{
				throw new ArgumentNullException("file");
			}
			bool flag2 = file == "";
			if (flag2)
			{
				throw new ArgumentException("Argument 'file' value must be specified.");
			}
			bool flag3 = headerEncoding == null;
			if (flag3)
			{
				throw new ArgumentNullException("headerEncoding");
			}
			MIME_Message result;
			using (FileStream fileStream = File.OpenRead(file))
			{
				result = MIME_Message.ParseFromStream(fileStream, headerEncoding);
			}
			return result;
		}

		// Token: 0x06000AEF RID: 2799 RVA: 0x00042454 File Offset: 0x00041454
		public static MIME_Message ParseFromStream(Stream stream)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			return MIME_Message.ParseFromStream(stream, Encoding.UTF8);
		}

		// Token: 0x06000AF0 RID: 2800 RVA: 0x00042488 File Offset: 0x00041488
		public static MIME_Message ParseFromStream(Stream stream, Encoding headerEncoding)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			bool flag2 = headerEncoding == null;
			if (flag2)
			{
				throw new ArgumentNullException("headerEncoding");
			}
			MIME_Message mime_Message = new MIME_Message();
			mime_Message.Parse(new SmartStream(stream, false), headerEncoding, new MIME_h_ContentType("text/plain"));
			return mime_Message;
		}

		// Token: 0x06000AF1 RID: 2801 RVA: 0x000424E2 File Offset: 0x000414E2
		public void ToFile(string file)
		{
			base.ToFile(file, new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B, Encoding.UTF8), Encoding.UTF8);
		}

		// Token: 0x06000AF2 RID: 2802 RVA: 0x000424FD File Offset: 0x000414FD
		public void ToStream(Stream stream)
		{
			base.ToStream(stream, new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B, Encoding.UTF8), Encoding.UTF8);
		}

		// Token: 0x06000AF3 RID: 2803 RVA: 0x00042518 File Offset: 0x00041518
		public override string ToString()
		{
			return base.ToString(new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B, Encoding.UTF8), Encoding.UTF8);
		}

		// Token: 0x06000AF4 RID: 2804 RVA: 0x00042540 File Offset: 0x00041540
		public byte[] ToByte()
		{
			return base.ToByte(new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B, Encoding.UTF8), Encoding.UTF8);
		}

		// Token: 0x06000AF5 RID: 2805 RVA: 0x00042568 File Offset: 0x00041568
		public MIME_Entity[] GetAllEntities(bool includeEmbbedMessage)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			List<MIME_Entity> list = new List<MIME_Entity>();
			List<MIME_Entity> list2 = new List<MIME_Entity>();
			list2.Add(this);
			while (list2.Count > 0)
			{
				MIME_Entity mime_Entity = list2[0];
				list2.RemoveAt(0);
				list.Add(mime_Entity);
				bool flag = base.Body != null && mime_Entity.Body.GetType().IsSubclassOf(typeof(MIME_b_Multipart));
				if (flag)
				{
					MIME_EntityCollection bodyParts = ((MIME_b_Multipart)mime_Entity.Body).BodyParts;
					for (int i = 0; i < bodyParts.Count; i++)
					{
						list2.Insert(i, bodyParts[i]);
					}
				}
				else
				{
					bool flag2 = includeEmbbedMessage && base.Body != null && mime_Entity.Body is MIME_b_MessageRfc822;
					if (flag2)
					{
						list2.Add(((MIME_b_MessageRfc822)mime_Entity.Body).Message);
					}
				}
			}
			return list.ToArray();
		}

		// Token: 0x06000AF6 RID: 2806 RVA: 0x00042690 File Offset: 0x00041690
		public MIME_Entity GetEntityByCID(string cid)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = cid == null;
			if (flag)
			{
				throw new ArgumentNullException("cid");
			}
			bool flag2 = cid == "";
			if (flag2)
			{
				throw new ArgumentException("Argument 'cid' value must be specified.", "cid");
			}
			foreach (MIME_Entity mime_Entity in this.AllEntities)
			{
				bool flag3 = mime_Entity.ContentID == cid;
				if (flag3)
				{
					return mime_Entity;
				}
			}
			return null;
		}

		// Token: 0x06000AF7 RID: 2807 RVA: 0x00042734 File Offset: 0x00041734
		public void ConvertToMultipartSigned(X509Certificate2 signerCert)
		{
			bool flag = signerCert == null;
			if (flag)
			{
				throw new ArgumentNullException("signerCert");
			}
			bool isSigned = this.IsSigned;
			if (isSigned)
			{
				throw new InvalidOperationException("Message is already signed.");
			}
			MIME_Entity mime_Entity = new MIME_Entity();
			mime_Entity.Body = base.Body;
			mime_Entity.ContentDisposition = base.ContentDisposition;
			mime_Entity.ContentTransferEncoding = base.ContentTransferEncoding;
			base.ContentTransferEncoding = null;
			MIME_b_MultipartSigned mime_b_MultipartSigned = new MIME_b_MultipartSigned();
			base.Body = mime_b_MultipartSigned;
			mime_b_MultipartSigned.SetCertificate(signerCert);
			mime_b_MultipartSigned.BodyParts.Add(mime_Entity);
		}

		// Token: 0x06000AF8 RID: 2808 RVA: 0x000427C4 File Offset: 0x000417C4
		public bool VerifySignatures()
		{
			foreach (MIME_Entity mime_Entity in this.AllEntities)
			{
				bool flag = string.Equals(mime_Entity.ContentType.TypeWithSubtype, MIME_MediaTypes.Application.pkcs7_mime, StringComparison.InvariantCultureIgnoreCase);
				if (flag)
				{
					bool flag2 = !((MIME_b_ApplicationPkcs7Mime)mime_Entity.Body).VerifySignature();
					if (flag2)
					{
						return false;
					}
				}
				else
				{
					bool flag3 = string.Equals(mime_Entity.ContentType.TypeWithSubtype, MIME_MediaTypes.Multipart.signed, StringComparison.InvariantCultureIgnoreCase);
					if (flag3)
					{
						bool flag4 = !((MIME_b_MultipartSigned)mime_Entity.Body).VerifySignature();
						if (flag4)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		// Token: 0x1700039B RID: 923
		// (get) Token: 0x06000AF9 RID: 2809 RVA: 0x00042878 File Offset: 0x00041878
		public bool IsSigned
		{
			get
			{
				MIME_Entity[] allEntities = this.AllEntities;
				int i = 0;
				while (i < allEntities.Length)
				{
					MIME_Entity mime_Entity = allEntities[i];
					bool flag = string.Equals(mime_Entity.ContentType.TypeWithSubtype, MIME_MediaTypes.Application.pkcs7_mime, StringComparison.InvariantCultureIgnoreCase);
					bool result;
					if (flag)
					{
						result = true;
					}
					else
					{
						bool flag2 = string.Equals(mime_Entity.ContentType.TypeWithSubtype, MIME_MediaTypes.Multipart.signed, StringComparison.InvariantCultureIgnoreCase);
						if (!flag2)
						{
							i++;
							continue;
						}
						result = true;
					}
					return result;
				}
				return false;
			}
		}

		// Token: 0x1700039C RID: 924
		// (get) Token: 0x06000AFA RID: 2810 RVA: 0x000428EC File Offset: 0x000418EC
		public MIME_Entity[] AllEntities
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				List<MIME_Entity> list = new List<MIME_Entity>();
				List<MIME_Entity> list2 = new List<MIME_Entity>();
				list2.Add(this);
				while (list2.Count > 0)
				{
					MIME_Entity mime_Entity = list2[0];
					list2.RemoveAt(0);
					list.Add(mime_Entity);
					bool flag = base.Body != null && mime_Entity.Body.GetType().IsSubclassOf(typeof(MIME_b_Multipart));
					if (flag)
					{
						MIME_EntityCollection bodyParts = ((MIME_b_Multipart)mime_Entity.Body).BodyParts;
						for (int i = 0; i < bodyParts.Count; i++)
						{
							list2.Insert(i, bodyParts[i]);
						}
					}
					else
					{
						bool flag2 = base.Body != null && mime_Entity.Body is MIME_b_MessageRfc822;
						if (flag2)
						{
							list2.Add(((MIME_b_MessageRfc822)mime_Entity.Body).Message);
						}
					}
				}
				return list.ToArray();
			}
		}

		// Token: 0x06000AFB RID: 2811 RVA: 0x00042A10 File Offset: 0x00041A10
		[Obsolete("Use MIME_Entity.CreateEntity_Attachment instead.")]
		public static MIME_Entity CreateAttachment(string file)
		{
			bool flag = file == null;
			if (flag)
			{
				throw new ArgumentNullException("file");
			}
			MIME_Entity mime_Entity = new MIME_Entity();
			MIME_b_Application mime_b_Application = new MIME_b_Application(MIME_MediaTypes.Application.octet_stream);
			mime_Entity.Body = mime_b_Application;
			mime_b_Application.SetDataFromFile(file, MIME_TransferEncodings.Base64);
			mime_Entity.ContentType.Param_Name = Path.GetFileName(file);
			FileInfo fileInfo = new FileInfo(file);
			mime_Entity.ContentDisposition = new MIME_h_ContentDisposition(MIME_DispositionTypes.Attachment)
			{
				Param_FileName = Path.GetFileName(file),
				Param_Size = fileInfo.Length,
				Param_CreationDate = fileInfo.CreationTime,
				Param_ModificationDate = fileInfo.LastWriteTime,
				Param_ReadDate = fileInfo.LastAccessTime
			};
			return mime_Entity;
		}

		// Token: 0x06000AFC RID: 2812 RVA: 0x00042AD0 File Offset: 0x00041AD0
		[Obsolete("Use MIME_Entity.CreateEntity_Attachment instead.")]
		public static MIME_Entity CreateAttachment(Stream stream, string fileName)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			bool flag2 = fileName == null;
			if (flag2)
			{
				throw new ArgumentNullException("fileName");
			}
			long param_Size = stream.CanSeek ? (stream.Length - stream.Position) : -1L;
			MIME_Entity mime_Entity = new MIME_Entity();
			MIME_b_Application mime_b_Application = new MIME_b_Application(MIME_MediaTypes.Application.octet_stream);
			mime_Entity.Body = mime_b_Application;
			mime_b_Application.SetData(stream, MIME_TransferEncodings.Base64);
			mime_Entity.ContentType.Param_Name = Path.GetFileName(fileName);
			mime_Entity.ContentDisposition = new MIME_h_ContentDisposition(MIME_DispositionTypes.Attachment)
			{
				Param_FileName = Path.GetFileName(fileName),
				Param_Size = param_Size
			};
			return mime_Entity;
		}

		// Token: 0x04000487 RID: 1159
		private bool m_IsDisposed = false;
	}
}
