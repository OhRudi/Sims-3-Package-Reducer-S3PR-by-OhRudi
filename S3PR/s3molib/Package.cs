using s3pi.Package;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace s3molib
{
    public class Package
    {
        public string FilePath { get; private set; }

        public void ChangePath(string path)
        {
            this.FilePath = path;
        }

        public uint IndexCount { get; private set; }

        public uint IndexLength { get; private set; }

        public uint IndexPosition { get; private set; }

        public void SetIndexPosition(uint position)
        {
            this.IndexPosition = position;
        }

        public List<ResourceEntry> ResourceEntries
        {
            get
            {
                return new List<ResourceEntry>(this._resourceEntries);
            }
        }

        public static Package New(string path, bool writeExtra = false)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
            if (Path.GetFileName(path) == "")
            {
                throw new Exception("Need filename to create file");
            }
            if (Path.GetExtension(path) != ".package")
            {
                Path.ChangeExtension(path, ".package");
            }
            FileStream stream = new FileStream(path, FileMode.Create);
            BinaryWriter w = new BinaryWriter(stream);
            w.Write(1179664964U);
            w.Write(2U);
            w.Write(0U);
            stream.Position = 60L;
            w.Write(3U);
            w.Write(96);
            w.Write(new byte[28]);
            Package package;
            if (writeExtra)
            {
                w.Write(7);
                w.Write(new byte[12]);
                stream.Position = 44L;
                w.Write(16);
                package = new Package(path, 0U, 16U, 96U);
            }
            else
            {
                package = new Package(path, 0U, 0U, 96U);
            }
            w.Flush();
            stream.Close();
            return package;
        }

        public static Package Load(string path)
        {
            if (!File.Exists(path))
            {
                throw new Exception("File does not exist");
            }
            if (new FileInfo(path).Length < 96L)
            {
                throw new Exception("Corrupted: File header size is smaller than '96' bytes");
            }
            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader r = new BinaryReader(stream);
            List<byte> list = new List<byte>();
            uint magic = r.ReadUInt32();
            uint major = r.ReadUInt32();
            uint minor = r.ReadUInt32();
            list.AddRange(r.ReadBytes(24));
            uint indexCount = r.ReadUInt32();
            list.AddRange(r.ReadBytes(4));
            uint indexLength = r.ReadUInt32();
            list.AddRange(r.ReadBytes(12));
            uint indexVersion = r.ReadUInt32();
            uint indexPosition = r.ReadUInt32();
            list.AddRange(r.ReadBytes(28));
            if (magic != 1179664964U)
            {
                throw new Exception("File magic does not match DBPF");
            }
            if (major != 2U)
            {
                throw new Exception(string.Format("File major '{0}' does not match '{1}'", major, 2U));
            }
            if (minor != 0U)
            {
                throw new Exception(string.Format("File minor '{0}' does not match '{1}'", minor, 0U));
            }
            if (indexVersion != 3U)
            {
                throw new Exception(string.Format("File index version '{0}' does not match '{1}'", indexVersion, 3U));
            }
            if (indexPosition == 0U)
            {
                throw new Exception("File does not have index position");
            }
            stream.Position = (long)((ulong)indexPosition);
            uint bitFlag = r.ReadUInt32();
            int commonCount = 0;
            if ((bitFlag & 1U) != 0U)
            {
                commonCount++;
            }
            if ((bitFlag & 2U) != 0U)
            {
                commonCount++;
            }
            if ((bitFlag & 4U) != 0U)
            {
                commonCount++;
            }
            if ((bitFlag & 8U) != 0U)
            {
                commonCount++;
            }
            if ((ulong)indexLength != (ulong)((long)(4 + commonCount * 4) + (long)(32 - commonCount * 4) * (long)((ulong)indexCount)))
            {
                throw new Exception(string.Format("Corrupted: File index length '{0}' does not match expected value of '{1}'", indexLength, (long)(4 + commonCount * 4) + (long)(32 - commonCount * 4) * (long)((ulong)indexCount)));
            }
            uint[] common = new uint[commonCount];
            for (int i = 0; i < commonCount; i++)
            {
                common[i] = r.ReadUInt32();
            }
            int c = 0;
            Package package = new Package(path, indexCount, indexLength, indexPosition);
            int j = 0;
            while ((long)j < (long)((ulong)indexCount))
            {
                uint type = ((bitFlag & 1U) != 0U) ? common[c++] : r.ReadUInt32();
                uint group = ((bitFlag & 2U) != 0U) ? common[c++] : r.ReadUInt32();
                uint id = ((bitFlag & 4U) != 0U) ? common[c++] : r.ReadUInt32();
                uint id2 = r.ReadUInt32();
                uint chunkOffset = r.ReadUInt32();
                uint fileSize = r.ReadUInt32();
                uint memSize = r.ReadUInt32();
                ushort compressed = r.ReadUInt16();
                ushort unknown2 = r.ReadUInt16();
                c = 0;
                package._resourceEntries.Add(new ResourceEntry(package, type, group, id, id2, chunkOffset, fileSize, memSize, compressed, unknown2));
                j++;
            }
            r.Close();
            stream.Close();
            return package;
        }


        public static void RemoveThumAndIconResourcesFromPackage(IEnumerable<Package> source, bool removeThumbnails=true, bool removeIcons=true)
        {
            for (int i = 0; i < source.Count<Package>(); i++)
            {
                Package importPackage = source.ElementAt(i);
                String originalPath = importPackage.FilePath;
                String tempPath = Path.Combine(Path.GetDirectoryName(originalPath), Path.GetFileNameWithoutExtension(originalPath) + ".temp" + Path.GetExtension(originalPath));
                Package package = Package.New(tempPath);
                FileStream stream = new FileStream(tempPath, FileMode.Open, FileAccess.Write, FileShare.Read);
                BinaryWriter w = new BinaryWriter(stream);
                stream.Position = (long)((ulong)package.IndexPosition);
                foreach (ResourceEntry re in importPackage.ResourceEntries)
                {
                    if (!(Helper.THUMResources.Contains(re.Type) && removeThumbnails == true) && !(Helper.ICONResources.Contains(re.Type) && removeIcons == true))
                    {
                        byte[] data = importPackage.GetRawData(re);
                        if (data == null)
                        {
                            throw new Exception("Unable to obtain data from " + Path.GetFileName(importPackage.FilePath));
                        }
                        package._resourceEntries.Add(new ResourceEntry(importPackage, re.Type, re.Group, re.ID1, re.ID2, (uint)stream.Position, re.FileSize, re.MemSize, re.Compressed, re.Unknown2));
                        w.Write(data, 0, (int)re.FileSize);
                   }
                }
                package.IndexPosition = (uint)stream.Position;
                package.WriteIndex(w);
                package.WriteHeader(stream, w);
                w.Close();
                stream.Close();
                File.Delete(originalPath);
                File.Move(tempPath, originalPath);
            }
        }

        public static bool Merge(string directoryPath, string mergeName, IEnumerable<Package> source, uint fileSizeLimit = 1000000000U, Action<int> progress = null)
        {
            if (File.Exists(directoryPath))
            {
                directoryPath = Path.GetDirectoryName(directoryPath);
            }
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            int mergedCount = 1;
            Package package = Package.New(Path.Combine(directoryPath, string.Format(mergeName + ".package", mergedCount++)), false);
            FileStream stream = new FileStream(package.FilePath, FileMode.Open, FileAccess.Write, FileShare.Read);
            BinaryWriter w = new BinaryWriter(stream);
            stream.Position = (long)((ulong)package.IndexPosition);
            for (int i = 0; i < source.Count<Package>(); i++)
            {
                Package importPackage = source.ElementAt(i);
                if (progress != null)
                {
                    progress(i);
                }
                foreach (ResourceEntry re in importPackage.ResourceEntries)
                {
                    // ignore all THUM and ICON resources
                    if (re.Type != 0x73E93EEB || re.Type != 0x0166038C || re.Type != 0x7672F0C5 || re.Type != 0x0580A2B4 || re.Type != 0x0580A2B5 || re.Type != 0x0580A2B6 || re.Type != 0x0589DC44 || re.Type != 0x0589DC45 || re.Type != 0x0589DC46 || re.Type != 0x0589DC47 || re.Type != 0x05B17698 || re.Type != 0x05B17699 || re.Type != 0x05B1769A || re.Type != 0x05B1B524 || re.Type != 0x05B1B525 || re.Type != 0x05B1B526 || re.Type != 0x2653E3C8 || re.Type != 0x2653E3C9 || re.Type != 0x2653E3CA || re.Type != 0x2D4284F0 || re.Type != 0x2D4284F1 || re.Type != 0x2D4284F2 || re.Type != 0x5DE9DBA0 || re.Type != 0x5DE9DBA1 || re.Type != 0x5DE9DBA2 || re.Type != 0x626F60CC || re.Type != 0x626F60CD || re.Type != 0x626F60CE || re.Type != 0xAD366F95 || re.Type != 0xAD366F96 || re.Type != 0xFCEAB65B || re.Type != 0x2E75C764 || re.Type != 0x2E75C765 || re.Type != 0x2E75C766 || re.Type != 0x2E75C767 || re.Type != 0xD84E7FC5 || re.Type != 0xD84E7FC6 || re.Type != 0xD84E7FC7)
                    {
                        if (stream.Position >= (long)((ulong)fileSizeLimit))
                        {
                            package.IndexPosition = (uint)stream.Position;
                            package.WriteIndex(w);
                            package.WriteHeader(stream, w);
                            w.Flush();
                            stream.Close();
                            package = Package.New(Path.Combine(directoryPath, string.Format(mergeName + "_{0}.package", mergedCount++)), false);
                            stream = new FileStream(package.FilePath, FileMode.Open, FileAccess.Write, FileShare.None);
                            w = new BinaryWriter(stream);
                            stream.Position = (long)((ulong)package.IndexPosition);
                        }
                        byte[] data = importPackage.GetRawData(re);
                        if (data == null)
                        {
                            throw new Exception("Unable to obtain data from " + Path.GetFileName(importPackage.FilePath));
                        }
                        package._resourceEntries.Add(new ResourceEntry(importPackage, re.Type, re.Group, re.ID1, re.ID2, (uint)stream.Position, re.FileSize, re.MemSize, re.Compressed, re.Unknown2));
                        w.Write(data, 0, (int)re.FileSize);
                    }
                }
            }
            package.IndexPosition = (uint)stream.Position;
            package.WriteIndex(w);
            package.WriteHeader(stream, w);
            w.Close();
            stream.Close();
            return true;
        }

        public void WriteIndex(BinaryWriter writer)
        {
            writer.Write(0);
            foreach (ResourceEntry r in this._resourceEntries)
            {
                writer.Write(r.Type);
                writer.Write(r.Group);
                writer.Write(r.ID1);
                writer.Write(r.ID2);
                writer.Write(r.ChunkOffset);
                writer.Write(r.FileSize | 2147483648U);
                writer.Write(r.MemSize);
                writer.Write(r.Compressed);
                writer.Write(r.Unknown2);
            }
        }

        public void WriteHeader(FileStream stream, BinaryWriter writer)
        {
            this.IndexCount = (uint)this._resourceEntries.Count;
            this.IndexLength = 4U + this.IndexCount * 32U;
            stream.Position = 36L;
            writer.Write(this.IndexCount);
            stream.Position = 44L;
            writer.Write(this.IndexLength);
            stream.Position = 64L;
            writer.Write(this.IndexPosition);
        }

        public byte[] GetRawData(ResourceEntry resourceEntry)
        {
            if (!File.Exists(this.FilePath))
            {
                throw new Exception("File does not exist anymore since the last time it was opened");
            }
            if (!this._resourceEntries.Contains(resourceEntry))
            {
                throw new Exception(string.Format("File does not contain resource entry the last time it was opened, try call Package.Open() again'{0}'", resourceEntry));
            }
            int offset = (int)resourceEntry.ChunkOffset;
            byte[] data = new byte[resourceEntry.FileSize];
            byte[] result;
            using (FileStream stream = new FileStream(this.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                stream.Position = (long)offset;
                stream.Read(data, 0, data.Length);
                result = data;
                stream.Close();
            }
            return result;
        }

        public byte[] GetUncompressedData(ResourceEntry resourceEntry)
        {
            byte[] data = this.GetRawData(resourceEntry);
            if (resourceEntry.FileSize != resourceEntry.MemSize)
            {
                MemoryStream memoryStream = new MemoryStream(data);
                data = Compression.UncompressStream(memoryStream, (int)resourceEntry.FileSize, (int)resourceEntry.MemSize);
                memoryStream.Close();
            }
            return data;
        }

        private Package(string path, uint indexCount, uint indexLength, uint indexPosition)
        {
            this.IndexCount = indexCount;
            this.IndexLength = indexLength;
            this.IndexPosition = indexPosition;
            this.FilePath = path;
        }

        public bool HasConflictingResourceEntry(ResourceEntry resourceEntry, out ResourceEntry ownResourceEntry)
        {
            ResourceEntry resourceEntry2;
            ownResourceEntry = (resourceEntry2 = this._resourceEntries.Find((ResourceEntry r) => r.Type.Equals(resourceEntry.Type) && r.Group.Equals(resourceEntry.Group) && r.Instance.Equals(resourceEntry.Instance)));
            return resourceEntry2 != null;
        }

        public bool AddResourceEntry(ResourceEntry resourceEntry, bool replaceConflicting = true)
        {
            ResourceEntry ownResourceEntry;
            if (this.HasConflictingResourceEntry(resourceEntry, out ownResourceEntry))
            {
                if (!replaceConflicting)
                {
                    return false;
                }
                this._resourceEntries.Remove(ownResourceEntry);
            }
            this._resourceEntries.Add(resourceEntry);
            return true;
        }

        public bool RemoveResourceEntry(ResourceEntry resourceEntry)
        {
            if (this._resourceEntries.Remove(resourceEntry))
            {
                return true;
            }
            return false;
        }

        public bool IsConflictingWith(Package package, out List<ResourceEntry> conflictedResourceEntries, out List<ResourceEntry> conflictedOtherResourceEntries, bool fastCheck = false)
        {
            conflictedResourceEntries = new List<ResourceEntry>();
            conflictedOtherResourceEntries = new List<ResourceEntry>();
            foreach (ResourceEntry otherR in package.ResourceEntries)
            {
                ResourceEntry ownR;
                if (this.HasConflictingResourceEntry(otherR, out ownR) && ownR.Type != 23462796U && ownR.Type != 1944665835U)
                {
                    if (fastCheck)
                    {
                        return true;
                    }
                    conflictedResourceEntries.Add(ownR);
                    conflictedOtherResourceEntries.Add(otherR);
                }
            }
            return conflictedOtherResourceEntries.Count > 0;
        }

        private const uint Magic = 1179664964U;

        private const uint Major = 2U;

        private const uint Minor = 0U;

        private const uint IndexVersion = 3U;

        private List<ResourceEntry> _resourceEntries = new List<ResourceEntry>();
    }
}
