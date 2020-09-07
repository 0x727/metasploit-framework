using System;
using System.IO;
using SharpCifs.Dcerpc.Ndr;
using SharpCifs.Smb;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Dcerpc
{
	// Token: 0x020000E2 RID: 226
	public abstract class DcerpcHandle
	{
		// Token: 0x06000777 RID: 1911 RVA: 0x00028F54 File Offset: 0x00027154
		protected internal static DcerpcBinding ParseBinding(string str)
		{
			char[] array = str.ToCharArray();
			string proto = null;
			string text = null;
			DcerpcBinding dcerpcBinding = null;
			int num2;
			int num;
			int index = num = (num2 = 0);
			for (;;)
			{
				char c = array[num2];
				switch (num)
				{
				case 0:
				{
					bool flag = c == ':';
					if (flag)
					{
						proto = Runtime.Substring(str, index, num2);
						index = num2 + 1;
						num = 1;
					}
					break;
				}
				case 1:
				{
					bool flag2 = c == '\\';
					if (!flag2)
					{
						num = 2;
						goto IL_83;
					}
					index = num2 + 1;
					break;
				}
				case 2:
					goto IL_83;
				case 3:
				case 4:
					goto IL_144;
				case 5:
				{
					bool flag3 = c == '=';
					if (flag3)
					{
						text = Runtime.Substring(str, index, num2).Trim();
						index = num2 + 1;
					}
					else
					{
						bool flag4 = c == ',' || c == ']';
						if (flag4)
						{
							string val = Runtime.Substring(str, index, num2).Trim();
							bool flag5 = text == null;
							if (flag5)
							{
								text = "endpoint";
							}
							dcerpcBinding.SetOption(text, val);
							text = null;
						}
					}
					break;
				}
				default:
					goto IL_144;
				}
				IL_14B:
				num2++;
				if (num2 >= array.Length)
				{
					break;
				}
				continue;
				IL_83:
				bool flag6 = c == '[';
				if (flag6)
				{
					string text2 = Runtime.Substring(str, index, num2).Trim();
					bool flag7 = text2.Length == 0;
					if (flag7)
					{
					}
					dcerpcBinding = new DcerpcBinding(proto, Runtime.Substring(str, index, num2));
					index = num2 + 1;
					num = 5;
				}
				goto IL_14B;
				IL_144:
				num2 = array.Length;
				goto IL_14B;
			}
			bool flag8 = dcerpcBinding == null || dcerpcBinding.Endpoint == null;
			if (flag8)
			{
				throw new DcerpcException("Invalid binding URL: " + str);
			}
			return dcerpcBinding;
		}

		// Token: 0x06000778 RID: 1912 RVA: 0x000290F4 File Offset: 0x000272F4
		public static DcerpcHandle GetHandle(string url, NtlmPasswordAuthentication auth)
		{
			bool flag = url.StartsWith("ncacn_np:");
			if (flag)
			{
				return new DcerpcPipeHandle(url, auth);
			}
			throw new DcerpcException("DCERPC transport not supported: " + url);
		}

		// Token: 0x06000779 RID: 1913 RVA: 0x00029130 File Offset: 0x00027330
		public virtual void Bind()
		{
			lock (this)
			{
				try
				{
					this.State = 1;
					DcerpcMessage msg = new DcerpcBind(this.Binding, this);
					this.Sendrecv(msg);
				}
				catch (IOException ex)
				{
					this.State = 0;
					throw;
				}
			}
		}

		// Token: 0x0600077A RID: 1914 RVA: 0x000291A4 File Offset: 0x000273A4
		public virtual void Sendrecv(DcerpcMessage msg)
		{
			bool flag = this.State == 0;
			if (flag)
			{
				this.Bind();
			}
			bool isDirect = true;
			byte[] array = BufferCache.GetBuffer();
			try
			{
				NdrBuffer ndrBuffer = new NdrBuffer(array, 0);
				msg.Flags = (DcerpcConstants.DcerpcFirstFrag | DcerpcConstants.DcerpcLastFrag);
				msg.CallId = DcerpcHandle._callId++;
				msg.Encode(ndrBuffer);
				bool flag2 = this.SecurityProvider != null;
				if (flag2)
				{
					ndrBuffer.SetIndex(0);
					this.SecurityProvider.Wrap(ndrBuffer);
				}
				int num = ndrBuffer.GetLength() - 24;
				int i;
				int num2;
				for (i = 0; i < num; i += num2)
				{
					num2 = num - i;
					bool flag3 = 24 + num2 > this.MaxXmit;
					if (flag3)
					{
						msg.Flags &= ~DcerpcConstants.DcerpcLastFrag;
						num2 = this.MaxXmit - 24;
					}
					else
					{
						msg.Flags |= DcerpcConstants.DcerpcLastFrag;
						isDirect = false;
						msg.AllocHint = num2;
					}
					msg.Length = 24 + num2;
					bool flag4 = i > 0;
					if (flag4)
					{
						msg.Flags &= ~DcerpcConstants.DcerpcFirstFrag;
					}
					bool flag5 = (msg.Flags & (DcerpcConstants.DcerpcFirstFrag | DcerpcConstants.DcerpcLastFrag)) != (DcerpcConstants.DcerpcFirstFrag | DcerpcConstants.DcerpcLastFrag);
					if (flag5)
					{
						ndrBuffer.Start = i;
						ndrBuffer.Reset();
						msg.Encode_header(ndrBuffer);
						ndrBuffer.Enc_ndr_long(msg.AllocHint);
						ndrBuffer.Enc_ndr_short(0);
						ndrBuffer.Enc_ndr_short(msg.GetOpnum());
					}
					this.DoSendFragment(array, i, msg.Length, isDirect);
				}
				this.DoReceiveFragment(array, isDirect);
				ndrBuffer.Reset();
				ndrBuffer.SetIndex(8);
				ndrBuffer.SetLength(ndrBuffer.Dec_ndr_short());
				bool flag6 = this.SecurityProvider != null;
				if (flag6)
				{
					this.SecurityProvider.Unwrap(ndrBuffer);
				}
				ndrBuffer.SetIndex(0);
				msg.Decode_header(ndrBuffer);
				i = 24;
				bool flag7 = msg.Ptype == 2 && !msg.IsFlagSet(DcerpcConstants.DcerpcLastFrag);
				if (flag7)
				{
					i = msg.Length;
				}
				byte[] array2 = null;
				NdrBuffer ndrBuffer2 = null;
				while (!msg.IsFlagSet(DcerpcConstants.DcerpcLastFrag))
				{
					bool flag8 = array2 == null;
					if (flag8)
					{
						array2 = new byte[this.MaxRecv];
						ndrBuffer2 = new NdrBuffer(array2, 0);
					}
					this.DoReceiveFragment(array2, isDirect);
					ndrBuffer2.Reset();
					ndrBuffer2.SetIndex(8);
					ndrBuffer2.SetLength(ndrBuffer2.Dec_ndr_short());
					bool flag9 = this.SecurityProvider != null;
					if (flag9)
					{
						this.SecurityProvider.Unwrap(ndrBuffer2);
					}
					ndrBuffer2.Reset();
					msg.Decode_header(ndrBuffer2);
					int num3 = msg.Length - 24;
					bool flag10 = i + num3 > array.Length;
					if (flag10)
					{
						byte[] array3 = new byte[i + num3];
						Array.Copy(array, 0, array3, 0, i);
						array = array3;
					}
					Array.Copy(array2, 24, array, i, num3);
					i += num3;
				}
				ndrBuffer = new NdrBuffer(array, 0);
				msg.Decode(ndrBuffer);
			}
			finally
			{
				BufferCache.ReleaseBuffer(array);
			}
			DcerpcException result;
			bool flag11 = (result = msg.GetResult()) != null;
			if (flag11)
			{
				throw result;
			}
		}

		// Token: 0x0600077B RID: 1915 RVA: 0x00029508 File Offset: 0x00027708
		public virtual void SetDcerpcSecurityProvider(IDcerpcSecurityProvider securityProvider)
		{
			this.SecurityProvider = securityProvider;
		}

		// Token: 0x0600077C RID: 1916 RVA: 0x00029514 File Offset: 0x00027714
		public virtual string GetServer()
		{
			bool flag = this is DcerpcPipeHandle;
			string result;
			if (flag)
			{
				result = ((DcerpcPipeHandle)this).Pipe.GetServer();
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600077D RID: 1917 RVA: 0x00029548 File Offset: 0x00027748
		public virtual Principal GetPrincipal()
		{
			bool flag = this is DcerpcPipeHandle;
			Principal result;
			if (flag)
			{
				result = ((DcerpcPipeHandle)this).Pipe.GetPrincipal();
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600077E RID: 1918 RVA: 0x0002957C File Offset: 0x0002777C
		public override string ToString()
		{
			return this.Binding.ToString();
		}

		// Token: 0x0600077F RID: 1919
		protected internal abstract void DoSendFragment(byte[] buf, int off, int length, bool isDirect);

		// Token: 0x06000780 RID: 1920
		protected internal abstract void DoReceiveFragment(byte[] buf, bool isDirect);

		// Token: 0x06000781 RID: 1921
		public abstract void Close();

		// Token: 0x06000782 RID: 1922 RVA: 0x00029599 File Offset: 0x00027799
		public DcerpcHandle()
		{
			this.MaxRecv = this.MaxXmit;
		}

		// Token: 0x040004DD RID: 1245
		protected internal DcerpcBinding Binding;

		// Token: 0x040004DE RID: 1246
		protected internal int MaxXmit = 4280;

		// Token: 0x040004DF RID: 1247
		protected internal int MaxRecv;

		// Token: 0x040004E0 RID: 1248
		protected internal int State;

		// Token: 0x040004E1 RID: 1249
		protected internal IDcerpcSecurityProvider SecurityProvider;

		// Token: 0x040004E2 RID: 1250
		private static int _callId = 1;
	}
}
