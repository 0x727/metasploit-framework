using System;
using SharpCifs.Dcerpc.Ndr;
using SharpCifs.Util;

namespace SharpCifs.Dcerpc
{
	// Token: 0x020000DD RID: 221
	public class DcerpcBind : DcerpcMessage
	{
		// Token: 0x06000760 RID: 1888 RVA: 0x000288B4 File Offset: 0x00026AB4
		internal static string GetResultMessage(int result)
		{
			return (result < 4) ? DcerpcBind.ResultMessage[result] : ("0x" + Hexdump.ToHexString(result, 4));
		}

		// Token: 0x06000761 RID: 1889 RVA: 0x000288E4 File Offset: 0x00026AE4
		public override DcerpcException GetResult()
		{
			bool flag = this.Result != 0;
			DcerpcException result;
			if (flag)
			{
				result = new DcerpcException(DcerpcBind.GetResultMessage(this.Result));
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000762 RID: 1890 RVA: 0x00028918 File Offset: 0x00026B18
		public DcerpcBind()
		{
		}

		// Token: 0x06000763 RID: 1891 RVA: 0x00028924 File Offset: 0x00026B24
		internal DcerpcBind(DcerpcBinding binding, DcerpcHandle handle)
		{
			this.Binding = binding;
			this.MaxXmit = handle.MaxXmit;
			this.MaxRecv = handle.MaxRecv;
			this.Ptype = 11;
			this.Flags = (DcerpcConstants.DcerpcFirstFrag | DcerpcConstants.DcerpcLastFrag);
		}

		// Token: 0x06000764 RID: 1892 RVA: 0x00028974 File Offset: 0x00026B74
		public override int GetOpnum()
		{
			return 0;
		}

		// Token: 0x06000765 RID: 1893 RVA: 0x00028988 File Offset: 0x00026B88
		public override void Encode_in(NdrBuffer dst)
		{
			dst.Enc_ndr_short(this.MaxXmit);
			dst.Enc_ndr_short(this.MaxRecv);
			dst.Enc_ndr_long(0);
			dst.Enc_ndr_small(1);
			dst.Enc_ndr_small(0);
			dst.Enc_ndr_short(0);
			dst.Enc_ndr_short(0);
			dst.Enc_ndr_small(1);
			dst.Enc_ndr_small(0);
			this.Binding.Uuid.Encode(dst);
			dst.Enc_ndr_short(this.Binding.Major);
			dst.Enc_ndr_short(this.Binding.Minor);
			DcerpcConstants.DcerpcUuidSyntaxNdr.Encode(dst);
			dst.Enc_ndr_long(2);
		}

		// Token: 0x06000766 RID: 1894 RVA: 0x00028A34 File Offset: 0x00026C34
		public override void Decode_out(NdrBuffer src)
		{
			src.Dec_ndr_short();
			src.Dec_ndr_short();
			src.Dec_ndr_long();
			int n = src.Dec_ndr_short();
			src.Advance(n);
			src.Align(4);
			src.Dec_ndr_small();
			src.Align(4);
			this.Result = src.Dec_ndr_short();
			src.Dec_ndr_short();
			src.Advance(20);
		}

		// Token: 0x040004BB RID: 1211
		internal static readonly string[] ResultMessage = new string[]
		{
			"0",
			"DCERPC_BIND_ERR_ABSTRACT_SYNTAX_NOT_SUPPORTED",
			"DCERPC_BIND_ERR_PROPOSED_TRANSFER_SYNTAXES_NOT_SUPPORTED",
			"DCERPC_BIND_ERR_LOCAL_LIMIT_EXCEEDED"
		};

		// Token: 0x040004BC RID: 1212
		internal DcerpcBinding Binding;

		// Token: 0x040004BD RID: 1213
		internal int MaxXmit;

		// Token: 0x040004BE RID: 1214
		internal int MaxRecv;
	}
}
