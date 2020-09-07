using System;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Netbios
{
	// Token: 0x020000D9 RID: 217
	internal class NodeStatusResponse : NameServicePacket
	{
		// Token: 0x06000744 RID: 1860 RVA: 0x0002811F File Offset: 0x0002631F
		internal NodeStatusResponse(NbtAddress queryAddress)
		{
			this._queryAddress = queryAddress;
			this.RecordName = new Name();
			this._macAddress = new byte[6];
		}

		// Token: 0x06000745 RID: 1861 RVA: 0x00028148 File Offset: 0x00026348
		internal override int WriteBodyWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000746 RID: 1862 RVA: 0x0002815C File Offset: 0x0002635C
		internal override int ReadBodyWireFormat(byte[] src, int srcIndex)
		{
			return this.ReadResourceRecordWireFormat(src, srcIndex);
		}

		// Token: 0x06000747 RID: 1863 RVA: 0x00028178 File Offset: 0x00026378
		internal override int WriteRDataWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000748 RID: 1864 RVA: 0x0002818C File Offset: 0x0002638C
		internal override int ReadRDataWireFormat(byte[] src, int srcIndex)
		{
			int num = srcIndex;
			this._numberOfNames = (int)(src[srcIndex] & byte.MaxValue);
			int num2 = this._numberOfNames * 18;
			int num3 = this.RDataLength - num2 - 1;
			this._numberOfNames = (int)(src[srcIndex++] & byte.MaxValue);
			Array.Copy(src, srcIndex + num2, this._macAddress, 0, 6);
			srcIndex += this.ReadNodeNameArray(src, srcIndex);
			this._stats = new byte[num3];
			Array.Copy(src, srcIndex, this._stats, 0, num3);
			srcIndex += num3;
			return srcIndex - num;
		}

		// Token: 0x06000749 RID: 1865 RVA: 0x0002821C File Offset: 0x0002641C
		private int ReadNodeNameArray(byte[] src, int srcIndex)
		{
			int num = srcIndex;
			this.AddressArray = new NbtAddress[this._numberOfNames];
			string scope = this._queryAddress.HostName.Scope;
			bool flag = false;
			try
			{
				for (int i = 0; i < this._numberOfNames; i++)
				{
					int num2 = srcIndex + 14;
					while (src[num2] == 32)
					{
						num2--;
					}
					string stringForBytes = Runtime.GetStringForBytes(src, srcIndex, num2 - srcIndex + 1, Name.OemEncoding);
					int num3 = (int)(src[srcIndex + 15] & byte.MaxValue);
					bool groupName = (src[srcIndex + 16] & 128) == 128;
					int nodeType = (src[srcIndex + 16] & 96) >> 5;
					bool isBeingDeleted = (src[srcIndex + 16] & 16) == 16;
					bool isInConflict = (src[srcIndex + 16] & 8) == 8;
					bool isActive = (src[srcIndex + 16] & 4) == 4;
					bool isPermanent = (src[srcIndex + 16] & 2) == 2;
					bool flag2 = !flag && this._queryAddress.HostName.HexCode == num3 && (this._queryAddress.HostName == NbtAddress.UnknownName || this._queryAddress.HostName.name.Equals(stringForBytes));
					if (flag2)
					{
						bool flag3 = this._queryAddress.HostName == NbtAddress.UnknownName;
						if (flag3)
						{
							this._queryAddress.HostName = new Name(stringForBytes, num3, scope);
						}
						this._queryAddress.GroupName = groupName;
						this._queryAddress.NodeType = nodeType;
						this._queryAddress.isBeingDeleted = isBeingDeleted;
						this._queryAddress.isInConflict = isInConflict;
						this._queryAddress.isActive = isActive;
						this._queryAddress.isPermanent = isPermanent;
						this._queryAddress.MacAddress = this._macAddress;
						this._queryAddress.IsDataFromNodeStatus = true;
						flag = true;
						this.AddressArray[i] = this._queryAddress;
					}
					else
					{
						this.AddressArray[i] = new NbtAddress(new Name(stringForBytes, num3, scope), this._queryAddress.Address, groupName, nodeType, isBeingDeleted, isInConflict, isActive, isPermanent, this._macAddress);
					}
					srcIndex += 18;
				}
			}
			catch (UnsupportedEncodingException)
			{
			}
			return srcIndex - num;
		}

		// Token: 0x0600074A RID: 1866 RVA: 0x00028484 File Offset: 0x00026684
		public override string ToString()
		{
			return "NodeStatusResponse[" + base.ToString() + "]";
		}

		// Token: 0x040004A8 RID: 1192
		private NbtAddress _queryAddress;

		// Token: 0x040004A9 RID: 1193
		private int _numberOfNames;

		// Token: 0x040004AA RID: 1194
		private byte[] _macAddress;

		// Token: 0x040004AB RID: 1195
		private byte[] _stats;

		// Token: 0x040004AC RID: 1196
		internal NbtAddress[] AddressArray;
	}
}
