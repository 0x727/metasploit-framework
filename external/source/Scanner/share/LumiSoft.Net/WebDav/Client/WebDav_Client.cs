using System;
using System.IO;
using System.Net;
using System.Text;

namespace LumiSoft.Net.WebDav.Client
{
	// Token: 0x02000041 RID: 65
	public class WebDav_Client
	{
		// Token: 0x0600022B RID: 555 RVA: 0x0000D574 File Offset: 0x0000C574
		public WebDav_MultiStatus PropFind(string requestUri, string[] propertyNames, int depth)
		{
			bool flag = requestUri == null;
			if (flag)
			{
				throw new ArgumentNullException("requestUri");
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n");
			stringBuilder.Append("<propfind xmlns=\"DAV:\">\r\n");
			stringBuilder.Append("<prop>\r\n");
			bool flag2 = propertyNames == null || propertyNames.Length == 0;
			if (flag2)
			{
				stringBuilder.Append("   <propname/>\r\n");
			}
			else
			{
				foreach (string str in propertyNames)
				{
					stringBuilder.Append("<" + str + "/>");
				}
			}
			stringBuilder.Append("</prop>\r\n");
			stringBuilder.Append("</propfind>\r\n");
			byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
			httpWebRequest.Method = "PROPFIND";
			httpWebRequest.ContentType = "application/xml";
			httpWebRequest.ContentLength = (long)bytes.Length;
			httpWebRequest.Credentials = this.m_pCredentials;
			bool flag3 = depth > -1;
			if (flag3)
			{
				httpWebRequest.Headers.Add("Depth: " + depth);
			}
			httpWebRequest.GetRequestStream().Write(bytes, 0, bytes.Length);
			return WebDav_MultiStatus.Parse(httpWebRequest.GetResponse().GetResponseStream());
		}

		// Token: 0x0600022C RID: 556 RVA: 0x0000D6D0 File Offset: 0x0000C6D0
		public void MkCol(string uri)
		{
			bool flag = uri == null;
			if (flag)
			{
				throw new ArgumentNullException("uri");
			}
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
			httpWebRequest.Method = "MKCOL";
			httpWebRequest.Credentials = this.m_pCredentials;
			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
		}

		// Token: 0x0600022D RID: 557 RVA: 0x0000D724 File Offset: 0x0000C724
		public Stream Get(string uri, out long contentSize)
		{
			bool flag = uri == null;
			if (flag)
			{
				throw new ArgumentNullException("uri");
			}
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
			httpWebRequest.Method = "GET";
			httpWebRequest.Credentials = this.m_pCredentials;
			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			contentSize = httpWebResponse.ContentLength;
			return httpWebResponse.GetResponseStream();
		}

		// Token: 0x0600022E RID: 558 RVA: 0x0000D78C File Offset: 0x0000C78C
		public void Delete(string uri)
		{
			bool flag = uri == null;
			if (flag)
			{
				throw new ArgumentNullException("uri");
			}
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
			httpWebRequest.Method = "DELETE";
			httpWebRequest.Credentials = this.m_pCredentials;
			httpWebRequest.GetResponse();
		}

		// Token: 0x0600022F RID: 559 RVA: 0x0000D7DC File Offset: 0x0000C7DC
		public void Put(string targetUri, Stream stream)
		{
			bool flag = targetUri == null;
			if (flag)
			{
				throw new ArgumentNullException("targetUri");
			}
			bool flag2 = stream == null;
			if (flag2)
			{
				throw new ArgumentNullException("stream");
			}
			try
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(targetUri);
				httpWebRequest.Credentials = this.m_pCredentials;
				httpWebRequest.PreAuthenticate = true;
				httpWebRequest.Method = "HEAD";
				((HttpWebResponse)httpWebRequest.GetResponse()).Close();
			}
			catch
			{
			}
			HttpWebRequest httpWebRequest2 = (HttpWebRequest)WebRequest.Create(targetUri);
			httpWebRequest2.Method = "PUT";
			httpWebRequest2.ContentType = "application/octet-stream";
			httpWebRequest2.Credentials = this.m_pCredentials;
			httpWebRequest2.PreAuthenticate = true;
			httpWebRequest2.AllowWriteStreamBuffering = false;
			httpWebRequest2.Timeout = -1;
			bool canSeek = stream.CanSeek;
			if (canSeek)
			{
				httpWebRequest2.ContentLength = stream.Length - stream.Position;
			}
			using (Stream requestStream = httpWebRequest2.GetRequestStream())
			{
				Net_Utils.StreamCopy(stream, requestStream, 32000);
			}
			httpWebRequest2.GetResponse();
		}

		// Token: 0x06000230 RID: 560 RVA: 0x0000D910 File Offset: 0x0000C910
		public void Copy(string sourceUri, string targetUri, int depth, bool overwrite)
		{
			bool flag = sourceUri == null;
			if (flag)
			{
				throw new ArgumentNullException(sourceUri);
			}
			bool flag2 = targetUri == null;
			if (flag2)
			{
				throw new ArgumentNullException(targetUri);
			}
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(sourceUri);
			httpWebRequest.Method = "COPY";
			httpWebRequest.Headers.Add("Destination: " + targetUri);
			httpWebRequest.Headers.Add("Overwrite: " + (overwrite ? "T" : "F"));
			bool flag3 = depth > -1;
			if (flag3)
			{
				httpWebRequest.Headers.Add("Depth: " + depth);
			}
			httpWebRequest.Credentials = this.m_pCredentials;
			httpWebRequest.GetResponse();
		}

		// Token: 0x06000231 RID: 561 RVA: 0x0000D9D0 File Offset: 0x0000C9D0
		public void Move(string sourceUri, string targetUri, int depth, bool overwrite)
		{
			bool flag = sourceUri == null;
			if (flag)
			{
				throw new ArgumentNullException(sourceUri);
			}
			bool flag2 = targetUri == null;
			if (flag2)
			{
				throw new ArgumentNullException(targetUri);
			}
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(sourceUri);
			httpWebRequest.Method = "MOVE";
			httpWebRequest.Headers.Add("Destination: " + targetUri);
			httpWebRequest.Headers.Add("Overwrite: " + (overwrite ? "T" : "F"));
			bool flag3 = depth > -1;
			if (flag3)
			{
				httpWebRequest.Headers.Add("Depth: " + depth);
			}
			httpWebRequest.Credentials = this.m_pCredentials;
			httpWebRequest.GetResponse();
		}

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06000232 RID: 562 RVA: 0x0000DA90 File Offset: 0x0000CA90
		// (set) Token: 0x06000233 RID: 563 RVA: 0x0000DAA8 File Offset: 0x0000CAA8
		public NetworkCredential Credentials
		{
			get
			{
				return this.m_pCredentials;
			}
			set
			{
				this.m_pCredentials = value;
			}
		}

		// Token: 0x040000F1 RID: 241
		private NetworkCredential m_pCredentials = null;
	}
}
