using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace s3molib
{
	public class ResourceEntry
	{
		public Package Package { get; private set; }

		public uint Type { get; private set; }

		public uint Group { get; private set; }

		public uint ID1 { get; private set; }

		public uint ID2 { get; private set; }

		public ulong Instance { get; private set; }

		public uint ChunkOffset { get; set; }

		public uint FileSize { get; private set; }

		public uint MemSize { get; private set; }

		public ushort Compressed { get; private set; }

		public ushort Unknown2 { get; private set; }

		public byte[] Data
		{
			get
			{
				if (this._data == null)
				{
					return this._data = this.Package.GetUncompressedData(this);
				}
				return this._data;
			}
		}

		public ResourceEntry(Package package, uint type, uint group, uint id1, uint id2, uint chunkOffset, uint fileSize, uint memSize, ushort compressed, ushort unknown2)
		{
			this.Package = package;
			this.Type = type;
			this.Group = group;
			this.ID1 = id1;
			this.ID2 = id2;
			this.Instance = ((ulong)id1 << 32 | (ulong)id2);
			this.ChunkOffset = chunkOffset;
			this.FileSize = (fileSize & 2147483647U);
			this.MemSize = memSize;
			this.Compressed = compressed;
			this.Unknown2 = unknown2;
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				Helper.UInt32ToHexString(this.Type),
				"-",
				Helper.UInt32ToHexString(this.Group),
				"-",
				Helper.UInt64ToHexString(this.Instance)
			});
		}

		public string Decode()
		{
			string decoded = string.Empty;
			if (ResourceEntry.TextTypes.Contains(this.Type))
			{
				decoded = Encoding.Default.GetString(this.Data);
			}
			else if (this.Type == 23462796U)
			{
				decoded = string.Join(Environment.NewLine, (from kvp in ResourceEntry.DecodeNameMap(this)
				select Helper.UInt64ToHexString(kvp.Key) + ": " + kvp.Value).ToArray<string>());
			}
			return decoded;
		}

		public static Dictionary<ulong, string> DecodeNameMap(ResourceEntry resourceEntry)
		{
			if (resourceEntry.Type != 23462796U)
			{
				throw new Exception("Trying to decode non-NMAP resources.");
			}
			Dictionary<ulong, string> nameMapLookup = new Dictionary<ulong, string>();
			BinaryReader r = new BinaryReader(new MemoryStream(resourceEntry.Data));
			r.ReadUInt32();
			uint count = r.ReadUInt32();
			int i = 0;
			while ((long)i < (long)((ulong)count))
			{
				ulong instance = r.ReadUInt64();
				uint charCount = r.ReadUInt32();
				string resourceName = Encoding.Default.GetString(r.ReadBytes((int)charCount));
				if (nameMapLookup.ContainsKey(instance))
				{
					nameMapLookup[instance] = resourceName;
				}
				else
				{
					nameMapLookup.Add(instance, resourceName);
				}
				i++;
			}
			return nameMapLookup;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static ResourceEntry()
		{
		}

		public static List<uint> TextTypes = new List<uint>
		{
			38407762U,
			39620774U,
			39622070U,
			43922235U,
			46788594U,
			38407762U,
			49987766U,
			53690476U,
			62078431U,
			100969434U,
			177793776U,
			529034925U,
			728542047U,
			1735860060U,
			1944665835U,
			2832567269U,
			3571055589U,
			3711050663U,
			3714200534U,
			3843051622U,
			3843051623U,
			3843051624U,
			4043265432U
		};

		private byte[] _data;
	}
}
