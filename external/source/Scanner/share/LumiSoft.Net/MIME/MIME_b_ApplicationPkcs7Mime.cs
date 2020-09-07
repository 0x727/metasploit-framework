using System;
using System.IO;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using LumiSoft.Net.IO;

namespace LumiSoft.Net.MIME
{
	// Token: 0x020000F3 RID: 243
	public class MIME_b_ApplicationPkcs7Mime : MIME_b_Application
	{
		// Token: 0x060009D2 RID: 2514 RVA: 0x0003C024 File Offset: 0x0003B024
		public MIME_b_ApplicationPkcs7Mime() : base(MIME_MediaTypes.Application.pkcs7_mime)
		{
		}

		// Token: 0x060009D3 RID: 2515 RVA: 0x0003C034 File Offset: 0x0003B034
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
			MIME_b_ApplicationPkcs7Mime mime_b_ApplicationPkcs7Mime = new MIME_b_ApplicationPkcs7Mime();
			Net_Utils.StreamCopy(stream, mime_b_ApplicationPkcs7Mime.EncodedStream, stream.LineBufferSize);
			return mime_b_ApplicationPkcs7Mime;
		}

		// Token: 0x060009D4 RID: 2516 RVA: 0x0003C0A0 File Offset: 0x0003B0A0
		public X509Certificate2Collection GetCertificates()
		{
			bool flag = base.Data == null;
			X509Certificate2Collection result;
			if (flag)
			{
				result = null;
			}
			else
			{
				SignedCms signedCms = new SignedCms();
				signedCms.Decode(base.Data);
				result = signedCms.Certificates;
			}
			return result;
		}

		// Token: 0x060009D5 RID: 2517 RVA: 0x0003C0E0 File Offset: 0x0003B0E0
		public bool VerifySignature()
		{
			bool flag = !string.Equals(base.Entity.ContentType.Parameters["smime-type"], "signed-data", StringComparison.InvariantCultureIgnoreCase);
			if (flag)
			{
				throw new InvalidOperationException("The VerifySignature method is only valid if Content-Type parameter smime-type=signed-data.");
			}
			bool flag2 = base.Data == null;
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				try
				{
					SignedCms signedCms = new SignedCms();
					signedCms.Decode(base.Data);
					signedCms.CheckSignature(true);
					return true;
				}
				catch
				{
				}
				result = false;
			}
			return result;
		}

		// Token: 0x060009D6 RID: 2518 RVA: 0x0003C174 File Offset: 0x0003B174
		public MIME_Message GetSignedMime()
		{
			bool flag = !string.Equals(base.Entity.ContentType.Parameters["smime-type"], "signed-data", StringComparison.InvariantCultureIgnoreCase);
			if (flag)
			{
				throw new InvalidOperationException("The VerifySignature method is only valid if Content-Type parameter smime-type=signed-data.");
			}
			bool flag2 = base.Data != null;
			MIME_Message result;
			if (flag2)
			{
				SignedCms signedCms = new SignedCms();
				signedCms.Decode(base.Data);
				result = MIME_Message.ParseFromStream(new MemoryStream(signedCms.ContentInfo.Content));
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x060009D7 RID: 2519 RVA: 0x0003C1FC File Offset: 0x0003B1FC
		public MIME_Message GetEnvelopedMime(X509Certificate2 cert)
		{
			bool flag = cert == null;
			if (flag)
			{
				throw new ArgumentNullException("cert");
			}
			bool flag2 = !string.Equals(base.Entity.ContentType.Parameters["smime-type"], "enveloped-data", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new InvalidOperationException("The VerifySignature method is only valid if Content-Type parameter smime-type=enveloped-data.");
			}
			EnvelopedCms envelopedCms = new EnvelopedCms();
			envelopedCms.Decode(base.Data);
			X509Certificate2Collection extraStore = new X509Certificate2Collection(cert);
			envelopedCms.Decrypt(extraStore);
			return MIME_Message.ParseFromStream(new MemoryStream(envelopedCms.Encode()));
		}
	}
}
