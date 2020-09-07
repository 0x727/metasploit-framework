using System;
using System.IO;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using LumiSoft.Net.IO;

namespace LumiSoft.Net.MIME
{
	// Token: 0x02000102 RID: 258
	public class MIME_b_MultipartSigned : MIME_b_Multipart
	{
		// Token: 0x06000A0A RID: 2570 RVA: 0x0003D54C File Offset: 0x0003C54C
		public MIME_b_MultipartSigned()
		{
			MIME_h_ContentType mime_h_ContentType = new MIME_h_ContentType(MIME_MediaTypes.Multipart.signed);
			mime_h_ContentType.Parameters["protocol"] = "application/x-pkcs7-signature";
			mime_h_ContentType.Parameters["micalg"] = "sha1";
			mime_h_ContentType.Param_Boundary = Guid.NewGuid().ToString().Replace('-', '.');
			base.ContentType = mime_h_ContentType;
		}

		// Token: 0x06000A0B RID: 2571 RVA: 0x0003D5CC File Offset: 0x0003C5CC
		public MIME_b_MultipartSigned(MIME_h_ContentType contentType) : base(contentType)
		{
			bool flag = !string.Equals(contentType.TypeWithSubtype, "multipart/signed", StringComparison.CurrentCultureIgnoreCase);
			if (flag)
			{
				throw new ArgumentException("Argument 'contentType.TypeWithSubype' value must be 'multipart/signed'.");
			}
		}

		// Token: 0x06000A0C RID: 2572 RVA: 0x0003D610 File Offset: 0x0003C610
		protected new static MIME_b Parse(MIME_Entity owner, MIME_h_ContentType defaultContentType, SmartStream stream)
		{
			bool flag = owner == null;
			if (flag)
			{
				throw new ArgumentNullException("owner");
			}
			bool flag2 = defaultContentType == null;
			if (flag2)
			{
				throw new ArgumentNullException("defaultContentType");
			}
			bool flag3 = stream == null;
			if (flag3)
			{
				throw new ArgumentNullException("stream");
			}
			bool flag4 = owner.ContentType == null || owner.ContentType.Param_Boundary == null;
			if (flag4)
			{
				throw new ParseException("Multipart entity has not required 'boundary' paramter.");
			}
			MIME_b_MultipartSigned mime_b_MultipartSigned = new MIME_b_MultipartSigned(owner.ContentType);
			MIME_b_Multipart.ParseInternal(owner, owner.ContentType.TypeWithSubtype, stream, mime_b_MultipartSigned);
			return mime_b_MultipartSigned;
		}

		// Token: 0x06000A0D RID: 2573 RVA: 0x0003D6AC File Offset: 0x0003C6AC
		public void SetCertificate(X509Certificate2 signerCert)
		{
			bool flag = signerCert == null;
			if (flag)
			{
				throw new ArgumentNullException("signerCert");
			}
			this.m_pSignerCert = signerCert;
		}

		// Token: 0x06000A0E RID: 2574 RVA: 0x0003D6D8 File Offset: 0x0003C6D8
		public X509Certificate2Collection GetCertificates()
		{
			bool flag = base.BodyParts.Count != 2;
			X509Certificate2Collection result;
			if (flag)
			{
				result = null;
			}
			else
			{
				MIME_Entity mime_Entity = base.BodyParts[1];
				SignedCms signedCms = new SignedCms();
				signedCms.Decode(((MIME_b_SinglepartBase)mime_Entity.Body).Data);
				result = signedCms.Certificates;
			}
			return result;
		}

		// Token: 0x06000A0F RID: 2575 RVA: 0x0003D734 File Offset: 0x0003C734
		public bool VerifySignature()
		{
			bool flag = this.m_pSignerCert != null;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = base.BodyParts.Count != 2;
				if (flag2)
				{
					result = false;
				}
				else
				{
					MIME_Entity mime_Entity = base.BodyParts[1];
					MemoryStream memoryStream = new MemoryStream();
					base.BodyParts[0].ToStream(memoryStream, null, null, false);
					try
					{
						SignedCms signedCms = new SignedCms(new ContentInfo(memoryStream.ToArray()), true);
						signedCms.Decode(((MIME_b_SinglepartBase)mime_Entity.Body).Data);
						signedCms.CheckSignature(true);
						result = true;
					}
					catch
					{
						result = false;
					}
				}
			}
			return result;
		}

		// Token: 0x06000A10 RID: 2576 RVA: 0x0003D7EC File Offset: 0x0003C7EC
		protected internal override void ToStream(Stream stream, MIME_Encoding_EncodedWord headerWordEncoder, Encoding headerParmetersCharset, bool headerReencode)
		{
			bool flag = base.BodyParts.Count > 0 && this.m_pSignerCert != null;
			if (flag)
			{
				bool flag2 = base.BodyParts.Count > 1;
				if (flag2)
				{
					base.BodyParts.Remove(1);
				}
				MemoryStream memoryStream = new MemoryStream();
				base.BodyParts[0].ToStream(memoryStream, null, null, false);
				SignedCms signedCms = new SignedCms(new ContentInfo(memoryStream.ToArray()), true);
				signedCms.ComputeSignature(new CmsSigner(this.m_pSignerCert));
				byte[] buffer = signedCms.Encode();
				MIME_Entity mime_Entity = new MIME_Entity();
				MIME_b_Application mime_b_Application = new MIME_b_Application(MIME_MediaTypes.Application.x_pkcs7_signature);
				mime_Entity.Body = mime_b_Application;
				mime_b_Application.SetData(new MemoryStream(buffer), MIME_TransferEncodings.Base64);
				mime_Entity.ContentType.Param_Name = "smime.p7s";
				mime_Entity.ContentDescription = "S/MIME Cryptographic Signature";
				base.BodyParts.Add(mime_Entity);
				signedCms.Decode(mime_b_Application.Data);
				signedCms.CheckSignature(true);
			}
			base.ToStream(stream, headerWordEncoder, headerParmetersCharset, headerReencode);
		}

		// Token: 0x04000457 RID: 1111
		private X509Certificate2 m_pSignerCert = null;
	}
}
