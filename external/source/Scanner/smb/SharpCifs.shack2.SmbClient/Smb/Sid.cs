using System;
using System.Collections.Generic;
using System.IO;
using SharpCifs.Dcerpc;
using SharpCifs.Dcerpc.Msrpc;
using SharpCifs.Util;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x02000085 RID: 133
	public class Sid : Rpc.SidT
	{
		// Token: 0x060003DA RID: 986 RVA: 0x00011700 File Offset: 0x0000F900
		static Sid()
		{
			try
			{
				Sid.Everyone = new Sid("S-1-1-0");
				Sid.CreatorOwner = new Sid("S-1-3-0");
				Sid.SYSTEM = new Sid("S-1-5-18");
			}
			catch (SmbException)
			{
			}
		}

		// Token: 0x060003DB RID: 987 RVA: 0x000117B4 File Offset: 0x0000F9B4
		internal static void ResolveSids(DcerpcHandle handle, LsaPolicyHandle policyHandle, Sid[] sids)
		{
			MsrpcLookupSids msrpcLookupSids = new MsrpcLookupSids(policyHandle, sids);
			handle.Sendrecv(msrpcLookupSids);
			int retval = msrpcLookupSids.Retval;
			if (retval != -1073741709 && retval != 0 && retval != 263)
			{
				throw new SmbException(msrpcLookupSids.Retval, false);
			}
			for (int i = 0; i < sids.Length; i++)
			{
				sids[i].Type = (int)msrpcLookupSids.Names.Names[i].SidType;
				sids[i].DomainName = null;
				switch (sids[i].Type)
				{
				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
				{
					int sidIndex = msrpcLookupSids.Names.Names[i].SidIndex;
					Rpc.Unicode_string name = msrpcLookupSids.Domains.Domains[sidIndex].Name;
					sids[i].DomainName = new UnicodeString(name, false).ToString();
					break;
				}
				}
				sids[i].AcctName = new UnicodeString(msrpcLookupSids.Names.Names[i].Name, false).ToString();
				sids[i].OriginServer = null;
				sids[i].OriginAuth = null;
			}
		}

		// Token: 0x060003DC RID: 988 RVA: 0x000118E4 File Offset: 0x0000FAE4
		internal static void ResolveSids0(string authorityServerName, NtlmPasswordAuthentication auth, Sid[] sids)
		{
			DcerpcHandle dcerpcHandle = null;
			LsaPolicyHandle lsaPolicyHandle = null;
			Hashtable sidCache = Sid.SidCache;
			lock (sidCache)
			{
				try
				{
					dcerpcHandle = DcerpcHandle.GetHandle("ncacn_np:" + authorityServerName + "[\\PIPE\\lsarpc]", auth);
					string text = authorityServerName;
					int num = text.IndexOf('.');
					bool flag2 = num > 0 && !char.IsDigit(text[0]);
					if (flag2)
					{
						text = Runtime.Substring(text, 0, num);
					}
					lsaPolicyHandle = new LsaPolicyHandle(dcerpcHandle, "\\\\" + text, 2048);
					Sid.ResolveSids(dcerpcHandle, lsaPolicyHandle, sids);
				}
				finally
				{
					bool flag3 = dcerpcHandle != null;
					if (flag3)
					{
						bool flag4 = lsaPolicyHandle != null;
						if (flag4)
						{
							lsaPolicyHandle.Close();
						}
						dcerpcHandle.Close();
					}
				}
			}
		}

		// Token: 0x060003DD RID: 989 RVA: 0x000119D4 File Offset: 0x0000FBD4
		public static void ResolveSids(string authorityServerName, NtlmPasswordAuthentication auth, Sid[] sids, int offset, int length)
		{
			List<object> list = new List<object>();
			Hashtable sidCache = Sid.SidCache;
			lock (sidCache)
			{
				for (int i = 0; i < length; i++)
				{
					Sid sid = (Sid)Sid.SidCache.Get(sids[offset + i]);
					bool flag2 = sid != null;
					if (flag2)
					{
						sids[offset + i].Type = sid.Type;
						sids[offset + i].DomainName = sid.DomainName;
						sids[offset + i].AcctName = sid.AcctName;
					}
					else
					{
						list.Add(sids[offset + i]);
					}
				}
				bool flag3 = list.Count > 0;
				if (flag3)
				{
					sids = (Sid[])list.ToArray();
					Sid.ResolveSids0(authorityServerName, auth, sids);
					for (int i = 0; i < sids.Length; i++)
					{
						Sid.SidCache.Put(sids[i], sids[i]);
					}
				}
			}
		}

		// Token: 0x060003DE RID: 990 RVA: 0x00011AE4 File Offset: 0x0000FCE4
		public static void ResolveSids(string authorityServerName, NtlmPasswordAuthentication auth, Sid[] sids)
		{
			List<object> list = new List<object>();
			Hashtable sidCache = Sid.SidCache;
			lock (sidCache)
			{
				for (int i = 0; i < sids.Length; i++)
				{
					Sid sid = (Sid)Sid.SidCache.Get(sids[i]);
					bool flag2 = sid != null;
					if (flag2)
					{
						sids[i].Type = sid.Type;
						sids[i].DomainName = sid.DomainName;
						sids[i].AcctName = sid.AcctName;
					}
					else
					{
						list.Add(sids[i]);
					}
				}
				bool flag3 = list.Count > 0;
				if (flag3)
				{
					sids = (Sid[])list.ToArray();
					Sid.ResolveSids0(authorityServerName, auth, sids);
					for (int i = 0; i < sids.Length; i++)
					{
						Sid.SidCache.Put(sids[i], sids[i]);
					}
				}
			}
		}

		// Token: 0x060003DF RID: 991 RVA: 0x00011BE8 File Offset: 0x0000FDE8
		public static Sid GetServerSid(string server, NtlmPasswordAuthentication auth)
		{
			DcerpcHandle dcerpcHandle = null;
			LsaPolicyHandle lsaPolicyHandle = null;
			Lsarpc.LsarDomainInfo lsarDomainInfo = new Lsarpc.LsarDomainInfo();
			Hashtable sidCache = Sid.SidCache;
			Sid result;
			lock (sidCache)
			{
				try
				{
					dcerpcHandle = DcerpcHandle.GetHandle("ncacn_np:" + server + "[\\PIPE\\lsarpc]", auth);
					lsaPolicyHandle = new LsaPolicyHandle(dcerpcHandle, null, 1);
					MsrpcQueryInformationPolicy msrpcQueryInformationPolicy = new MsrpcQueryInformationPolicy(lsaPolicyHandle, 5, lsarDomainInfo);
					dcerpcHandle.Sendrecv(msrpcQueryInformationPolicy);
					bool flag2 = msrpcQueryInformationPolicy.Retval != 0;
					if (flag2)
					{
						throw new SmbException(msrpcQueryInformationPolicy.Retval, false);
					}
					result = new Sid(lsarDomainInfo.Sid, 3, new UnicodeString(lsarDomainInfo.Name, false).ToString(), null, false);
				}
				finally
				{
					bool flag3 = dcerpcHandle != null;
					if (flag3)
					{
						bool flag4 = lsaPolicyHandle != null;
						if (flag4)
						{
							lsaPolicyHandle.Close();
						}
						dcerpcHandle.Close();
					}
				}
			}
			return result;
		}

		// Token: 0x060003E0 RID: 992 RVA: 0x00011CDC File Offset: 0x0000FEDC
		public static byte[] ToByteArray(Rpc.SidT sid)
		{
			byte[] array = new byte[(int)(8 + sid.SubAuthorityCount * 4)];
			int num = 0;
			array[num++] = sid.Revision;
			array[num++] = sid.SubAuthorityCount;
			Array.Copy(sid.IdentifierAuthority, 0, array, num, 6);
			num += 6;
			for (int i = 0; i < (int)sid.SubAuthorityCount; i++)
			{
				Encdec.Enc_uint32le(sid.SubAuthority[i], array, num);
				num += 4;
			}
			return array;
		}

		// Token: 0x060003E1 RID: 993 RVA: 0x00011D5C File Offset: 0x0000FF5C
		public Sid(byte[] src, int si)
		{
			this.Revision = src[si++];
			this.SubAuthorityCount = src[si++];
			this.IdentifierAuthority = new byte[6];
			Array.Copy(src, si, this.IdentifierAuthority, 0, 6);
			si += 6;
			bool flag = this.SubAuthorityCount > 100;
			if (flag)
			{
				throw new RuntimeException("Invalid SID sub_authority_count");
			}
			this.SubAuthority = new int[(int)this.SubAuthorityCount];
			for (int i = 0; i < (int)this.SubAuthorityCount; i++)
			{
				this.SubAuthority[i] = ServerMessageBlock.ReadInt4(src, si);
				si += 4;
			}
		}

		// Token: 0x060003E2 RID: 994 RVA: 0x00011E04 File Offset: 0x00010004
		public Sid(string textual)
		{
			StringTokenizer stringTokenizer = new StringTokenizer(textual, "-");
			bool flag = stringTokenizer.CountTokens() < 3 || !stringTokenizer.NextToken().Equals("S");
			if (flag)
			{
				throw new SmbException("Bad textual SID format: " + textual);
			}
			this.Revision = byte.Parse(stringTokenizer.NextToken());
			string text = stringTokenizer.NextToken();
			bool flag2 = text.StartsWith("0x");
			long num;
			if (flag2)
			{
				num = long.Parse(Runtime.Substring(text, 2));
			}
			else
			{
				num = long.Parse(text);
			}
			this.IdentifierAuthority = new byte[6];
			int num2 = 5;
			while (num > 0L)
			{
				this.IdentifierAuthority[num2] = (byte)(num % 256L);
				num >>= 8;
				num2--;
			}
			this.SubAuthorityCount = (byte)stringTokenizer.CountTokens();
			bool flag3 = this.SubAuthorityCount > 0;
			if (flag3)
			{
				this.SubAuthority = new int[(int)this.SubAuthorityCount];
				for (int i = 0; i < (int)this.SubAuthorityCount; i++)
				{
					this.SubAuthority[i] = (int)(long.Parse(stringTokenizer.NextToken()) & (long)(-1));
				}
			}
		}

		// Token: 0x060003E3 RID: 995 RVA: 0x00011F40 File Offset: 0x00010140
		public Sid(Sid domsid, int rid)
		{
			this.Revision = domsid.Revision;
			this.IdentifierAuthority = domsid.IdentifierAuthority;
			this.SubAuthorityCount = (byte)(domsid.SubAuthorityCount + 1);
			this.SubAuthority = new int[(int)this.SubAuthorityCount];
			int i;
			for (i = 0; i < (int)domsid.SubAuthorityCount; i++)
			{
				this.SubAuthority[i] = domsid.SubAuthority[i];
			}
			this.SubAuthority[i] = rid;
		}

		// Token: 0x060003E4 RID: 996 RVA: 0x00011FC0 File Offset: 0x000101C0
		public Sid(Rpc.SidT sid, int type, string domainName, string acctName, bool decrementAuthority)
		{
			this.Revision = sid.Revision;
			this.SubAuthorityCount = sid.SubAuthorityCount;
			this.IdentifierAuthority = sid.IdentifierAuthority;
			this.SubAuthority = sid.SubAuthority;
			this.Type = type;
			this.DomainName = domainName;
			this.AcctName = acctName;
			if (decrementAuthority)
			{
				this.SubAuthorityCount -= 1;
				this.SubAuthority = new int[(int)this.SubAuthorityCount];
				for (int i = 0; i < (int)this.SubAuthorityCount; i++)
				{
					this.SubAuthority[i] = sid.SubAuthority[i];
				}
			}
		}

		// Token: 0x060003E5 RID: 997 RVA: 0x0001206C File Offset: 0x0001026C
		public virtual Sid GetDomainSid()
		{
			return new Sid(this, 3, this.DomainName, null, this.GetType() != 3);
		}

		// Token: 0x060003E6 RID: 998 RVA: 0x00012098 File Offset: 0x00010298
		public virtual int GetRid()
		{
			bool flag = this.GetType() == 3;
			if (flag)
			{
				throw new ArgumentException("This SID is a domain sid");
			}
			return this.SubAuthority[(int)(this.SubAuthorityCount - 1)];
		}

		// Token: 0x060003E7 RID: 999 RVA: 0x000120D4 File Offset: 0x000102D4
		public new virtual int GetType()
		{
			bool flag = this.OriginServer != null;
			if (flag)
			{
				this.ResolveWeak();
			}
			return this.Type;
		}

		// Token: 0x060003E8 RID: 1000 RVA: 0x00012104 File Offset: 0x00010304
		public virtual string GetTypeText()
		{
			bool flag = this.OriginServer != null;
			if (flag)
			{
				this.ResolveWeak();
			}
			return Sid.SidTypeNames[this.Type];
		}

		// Token: 0x060003E9 RID: 1001 RVA: 0x00012138 File Offset: 0x00010338
		public virtual string GetDomainName()
		{
			bool flag = this.OriginServer != null;
			if (flag)
			{
				this.ResolveWeak();
			}
			bool flag2 = this.Type == 8;
			string result;
			if (flag2)
			{
				string text = this.ToString();
				result = Runtime.Substring(text, 0, text.Length - this.GetAccountName().Length - 1);
			}
			else
			{
				result = this.DomainName;
			}
			return result;
		}

		// Token: 0x060003EA RID: 1002 RVA: 0x0001219C File Offset: 0x0001039C
		public virtual string GetAccountName()
		{
			bool flag = this.OriginServer != null;
			if (flag)
			{
				this.ResolveWeak();
			}
			bool flag2 = this.Type == 8;
			string result;
			if (flag2)
			{
				result = string.Empty + this.SubAuthority[(int)(this.SubAuthorityCount - 1)];
			}
			else
			{
				bool flag3 = this.Type == 3;
				if (flag3)
				{
					result = string.Empty;
				}
				else
				{
					result = this.AcctName;
				}
			}
			return result;
		}

		// Token: 0x060003EB RID: 1003 RVA: 0x00012210 File Offset: 0x00010410
		public override int GetHashCode()
		{
			int num = (int)this.IdentifierAuthority[5];
			for (int i = 0; i < (int)this.SubAuthorityCount; i++)
			{
				num += 65599 * this.SubAuthority[i];
			}
			return num;
		}

		// Token: 0x060003EC RID: 1004 RVA: 0x00012254 File Offset: 0x00010454
		public override bool Equals(object obj)
		{
			bool flag = obj is Sid;
			if (flag)
			{
				Sid sid = (Sid)obj;
				bool flag2 = sid == this;
				if (flag2)
				{
					return true;
				}
				bool flag3 = sid.SubAuthorityCount == this.SubAuthorityCount;
				if (flag3)
				{
					int i = (int)this.SubAuthorityCount;
					while (i-- > 0)
					{
						bool flag4 = sid.SubAuthority[i] != this.SubAuthority[i];
						if (flag4)
						{
							return false;
						}
					}
					for (i = 0; i < 6; i++)
					{
						bool flag5 = sid.IdentifierAuthority[i] != this.IdentifierAuthority[i];
						if (flag5)
						{
							return false;
						}
					}
					return sid.Revision == this.Revision;
				}
			}
			return false;
		}

		// Token: 0x060003ED RID: 1005 RVA: 0x00012334 File Offset: 0x00010534
		public override string ToString()
		{
			string text = "S-" + (int)(this.Revision & byte.MaxValue) + "-";
			bool flag = this.IdentifierAuthority[0] != 0 || this.IdentifierAuthority[1] > 0;
			if (flag)
			{
				text += "0x";
				text += Hexdump.ToHexString(this.IdentifierAuthority, 0, 6);
			}
			else
			{
				int num = 0;
				long num2 = 0L;
				for (int i = 5; i > 1; i--)
				{
					num2 += (long)((long)((ulong)this.IdentifierAuthority[i] & 255UL) << num);
					num += 8;
				}
				text += num2;
			}
			for (int j = 0; j < (int)this.SubAuthorityCount; j++)
			{
				text = text + "-" + ((long)this.SubAuthority[j] & (long)(-1));
			}
			return text;
		}

		// Token: 0x060003EE RID: 1006 RVA: 0x00012430 File Offset: 0x00010630
		public virtual string ToDisplayString()
		{
			bool flag = this.OriginServer != null;
			if (flag)
			{
				this.ResolveWeak();
			}
			bool flag2 = this.DomainName != null;
			string result;
			if (flag2)
			{
				bool flag3 = this.Type == 3;
				string text;
				if (flag3)
				{
					text = this.DomainName;
				}
				else
				{
					bool flag4 = this.Type == 5 || this.DomainName.Equals("BUILTIN");
					if (flag4)
					{
						bool flag5 = this.Type == 8;
						if (flag5)
						{
							text = this.ToString();
						}
						else
						{
							text = this.AcctName;
						}
					}
					else
					{
						text = this.DomainName + "\\" + this.AcctName;
					}
				}
				result = text;
			}
			else
			{
				result = this.ToString();
			}
			return result;
		}

		// Token: 0x060003EF RID: 1007 RVA: 0x000124F4 File Offset: 0x000106F4
		public virtual void Resolve(string authorityServerName, NtlmPasswordAuthentication auth)
		{
			Sid.ResolveSids(authorityServerName, auth, new Sid[]
			{
				this
			});
		}

		// Token: 0x060003F0 RID: 1008 RVA: 0x00012518 File Offset: 0x00010718
		internal virtual void ResolveWeak()
		{
			bool flag = this.OriginServer != null;
			if (flag)
			{
				try
				{
					this.Resolve(this.OriginServer, this.OriginAuth);
				}
				catch (IOException)
				{
				}
				finally
				{
					this.OriginServer = null;
					this.OriginAuth = null;
				}
			}
		}

		// Token: 0x060003F1 RID: 1009 RVA: 0x00012580 File Offset: 0x00010780
		internal static Sid[] GetGroupMemberSids0(DcerpcHandle handle, SamrDomainHandle domainHandle, Sid domsid, int rid, int flags)
		{
			SamrAliasHandle samrAliasHandle = null;
			Lsarpc.LsarSidArray sids = new Lsarpc.LsarSidArray();
			Sid[] result;
			try
			{
				samrAliasHandle = new SamrAliasHandle(handle, domainHandle, 131084, rid);
				MsrpcGetMembersInAlias msrpcGetMembersInAlias = new MsrpcGetMembersInAlias(samrAliasHandle, sids);
				handle.Sendrecv(msrpcGetMembersInAlias);
				bool flag = msrpcGetMembersInAlias.Retval != 0;
				if (flag)
				{
					throw new SmbException(msrpcGetMembersInAlias.Retval, false);
				}
				Sid[] array = new Sid[msrpcGetMembersInAlias.Sids.NumSids];
				string server = handle.GetServer();
				NtlmPasswordAuthentication ntlmPasswordAuthentication = (NtlmPasswordAuthentication)handle.GetPrincipal();
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = new Sid(msrpcGetMembersInAlias.Sids.Sids[i].Sid, 0, null, null, false);
					array[i].OriginServer = server;
					array[i].OriginAuth = ntlmPasswordAuthentication;
				}
				bool flag2 = array.Length != 0 && (flags & 1) != 0;
				if (flag2)
				{
					Sid.ResolveSids(server, ntlmPasswordAuthentication, array);
				}
				result = array;
			}
			finally
			{
				bool flag3 = samrAliasHandle != null;
				if (flag3)
				{
					samrAliasHandle.Close();
				}
			}
			return result;
		}

		// Token: 0x060003F2 RID: 1010 RVA: 0x00012698 File Offset: 0x00010898
		public virtual Sid[] GetGroupMemberSids(string authorityServerName, NtlmPasswordAuthentication auth, int flags)
		{
			bool flag = this.Type != 2 && this.Type != 4;
			Sid[] result;
			if (flag)
			{
				result = new Sid[0];
			}
			else
			{
				DcerpcHandle dcerpcHandle = null;
				SamrPolicyHandle samrPolicyHandle = null;
				SamrDomainHandle samrDomainHandle = null;
				Sid domainSid = this.GetDomainSid();
				Hashtable sidCache = Sid.SidCache;
				lock (sidCache)
				{
					try
					{
						dcerpcHandle = DcerpcHandle.GetHandle("ncacn_np:" + authorityServerName + "[\\PIPE\\samr]", auth);
						samrPolicyHandle = new SamrPolicyHandle(dcerpcHandle, authorityServerName, 48);
						samrDomainHandle = new SamrDomainHandle(dcerpcHandle, samrPolicyHandle, 512, domainSid);
						result = Sid.GetGroupMemberSids0(dcerpcHandle, samrDomainHandle, domainSid, this.GetRid(), flags);
					}
					finally
					{
						bool flag3 = dcerpcHandle != null;
						if (flag3)
						{
							bool flag4 = samrPolicyHandle != null;
							if (flag4)
							{
								bool flag5 = samrDomainHandle != null;
								if (flag5)
								{
									samrDomainHandle.Close();
								}
								samrPolicyHandle.Close();
							}
							dcerpcHandle.Close();
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060003F3 RID: 1011 RVA: 0x000127A0 File Offset: 0x000109A0
		internal static Hashtable GetLocalGroupsMap(string authorityServerName, NtlmPasswordAuthentication auth, int flags)
		{
			Sid serverSid = Sid.GetServerSid(authorityServerName, auth);
			DcerpcHandle dcerpcHandle = null;
			SamrPolicyHandle samrPolicyHandle = null;
			SamrDomainHandle samrDomainHandle = null;
			Samr.SamrSamArray sam = new Samr.SamrSamArray();
			Hashtable sidCache = Sid.SidCache;
			Hashtable result;
			lock (sidCache)
			{
				try
				{
					dcerpcHandle = DcerpcHandle.GetHandle("ncacn_np:" + authorityServerName + "[\\PIPE\\samr]", auth);
					samrPolicyHandle = new SamrPolicyHandle(dcerpcHandle, authorityServerName, 33554432);
					samrDomainHandle = new SamrDomainHandle(dcerpcHandle, samrPolicyHandle, 33554432, serverSid);
					MsrpcEnumerateAliasesInDomain msrpcEnumerateAliasesInDomain = new MsrpcEnumerateAliasesInDomain(samrDomainHandle, 65535, sam);
					dcerpcHandle.Sendrecv(msrpcEnumerateAliasesInDomain);
					bool flag2 = msrpcEnumerateAliasesInDomain.Retval != 0;
					if (flag2)
					{
						throw new SmbException(msrpcEnumerateAliasesInDomain.Retval, false);
					}
					Hashtable hashtable = new Hashtable();
					for (int i = 0; i < msrpcEnumerateAliasesInDomain.Sam.Count; i++)
					{
						Samr.SamrSamEntry samrSamEntry = msrpcEnumerateAliasesInDomain.Sam.Entries[i];
						Sid[] groupMemberSids = Sid.GetGroupMemberSids0(dcerpcHandle, samrDomainHandle, serverSid, samrSamEntry.Idx, flags);
						Sid sid = new Sid(serverSid, samrSamEntry.Idx);
						sid.Type = 4;
						sid.DomainName = serverSid.GetDomainName();
						sid.AcctName = new UnicodeString(samrSamEntry.Name, false).ToString();
						for (int j = 0; j < groupMemberSids.Length; j++)
						{
							List<object> list = (List<object>)hashtable.Get(groupMemberSids[j]);
							bool flag3 = list == null;
							if (flag3)
							{
								list = new List<object>();
								hashtable.Put(groupMemberSids[j], list);
							}
							bool flag4 = !list.Contains(sid);
							if (flag4)
							{
								list.Add(sid);
							}
						}
					}
					result = hashtable;
				}
				finally
				{
					bool flag5 = dcerpcHandle != null;
					if (flag5)
					{
						bool flag6 = samrPolicyHandle != null;
						if (flag6)
						{
							bool flag7 = samrDomainHandle != null;
							if (flag7)
							{
								samrDomainHandle.Close();
							}
							samrPolicyHandle.Close();
						}
						dcerpcHandle.Close();
					}
				}
			}
			return result;
		}

		// Token: 0x0400017C RID: 380
		public const int SidTypeUseNone = 0;

		// Token: 0x0400017D RID: 381
		public const int SidTypeUser = 1;

		// Token: 0x0400017E RID: 382
		public const int SidTypeDomGrp = 2;

		// Token: 0x0400017F RID: 383
		public const int SidTypeDomain = 3;

		// Token: 0x04000180 RID: 384
		public const int SidTypeAlias = 4;

		// Token: 0x04000181 RID: 385
		public const int SidTypeWknGrp = 5;

		// Token: 0x04000182 RID: 386
		public const int SidTypeDeleted = 6;

		// Token: 0x04000183 RID: 387
		public const int SidTypeInvalid = 7;

		// Token: 0x04000184 RID: 388
		public const int SidTypeUnknown = 8;

		// Token: 0x04000185 RID: 389
		internal static readonly string[] SidTypeNames = new string[]
		{
			"0",
			"User",
			"Domain group",
			"Domain",
			"Local group",
			"Builtin group",
			"Deleted",
			"Invalid",
			"Unknown"
		};

		// Token: 0x04000186 RID: 390
		public const int SidFlagResolveSids = 1;

		// Token: 0x04000187 RID: 391
		public static Sid Everyone;

		// Token: 0x04000188 RID: 392
		public static Sid CreatorOwner;

		// Token: 0x04000189 RID: 393
		public static Sid SYSTEM;

		// Token: 0x0400018A RID: 394
		internal static Hashtable SidCache = new Hashtable();

		// Token: 0x0400018B RID: 395
		internal int Type;

		// Token: 0x0400018C RID: 396
		internal string DomainName;

		// Token: 0x0400018D RID: 397
		internal string AcctName;

		// Token: 0x0400018E RID: 398
		internal string OriginServer;

		// Token: 0x0400018F RID: 399
		internal NtlmPasswordAuthentication OriginAuth;
	}
}
