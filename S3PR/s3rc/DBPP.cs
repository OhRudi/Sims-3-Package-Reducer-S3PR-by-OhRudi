using System;
using System.IO;
using System.Text;

namespace s3rc
{
    class DBPP
    {
        const uint COMPTAIL_0 = 0x00010000;
        const uint COMPTAIL_1 = 0x0001FFFF;

        public static void Fix(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new Exception("File does not exist");
            }

            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite))
            using (var br = new BinaryReader(fs))
            using (var bw = new BinaryWriter(fs))
            {
                long length = fs.Length;
                if (length < 96)
                {
                    throw new Exception("Corrupted: File header size is smaller than '96' bytes");
                }

                fs.Seek(0, SeekOrigin.Begin);
                byte[] magicBytes = br.ReadBytes(4);
                string magic = Encoding.ASCII.GetString(magicBytes);

                if (magic == "DBPF")
                {
                    return;
                }
                else if (magic == "DBPP")
                {
                    DBPFHeader fixedHeader = new DBPFHeader();

                    if (FixHeader(fs, ref fixedHeader, 0, length))
                    {
                        fs.Seek(0, SeekOrigin.Begin);
                        bw.Write(Encoding.ASCII.GetBytes("DBPF"));
                        bw.Write(fixedHeader.MajVer);
                        bw.Write(fixedHeader.MinVer);
                        bw.Write(new byte[24]);
                        bw.Write(fixedHeader.IdxCount);
                        bw.Write((uint)0);
                        bw.Write(fixedHeader.IdxSize);
                        bw.Write(new byte[12]);
                        bw.Write(fixedHeader.IdxVer);
                        bw.Write(fixedHeader.IdxOffs);
                        bw.Write(new byte[28]);
                    }
                    else
                    {
                        throw new Exception("Corrupted: Could not fix protected file");
                    }
                }
                else
                {
                    throw new Exception("Corrupted: File magic does not match DBPF or DBPP");
                }
            }
        }

        static bool FixHeader(FileStream fs, ref DBPFHeader hdr, long start, long end)
        {
            var br = new BinaryReader(fs);

            if (end - start < 96) return false;

            fs.Seek(end - 4, SeekOrigin.Begin);
            uint d32 = br.ReadUInt32();

            if (d32 != COMPTAIL_0 && d32 != COMPTAIL_1)
            {
                return false;
            }

            int fc = 0;
            uint entrySize = 0;
            uint idxCount = 1;
            uint hSize = 0;
            long off = 0;

            do
            {
                fs.Seek(end - 20, SeekOrigin.Begin);
                off = fs.Position;
                entrySize = 0;
                idxCount = 1;
                hSize = 0;
                int f = 0;

                while (off - start >= 96)
                {
                    d32 = br.ReadUInt32();
                    if (d32 == COMPTAIL_0 || d32 == COMPTAIL_1)
                    {
                        f++;
                        if (f > fc)
                        {
                            off = fs.Position;
                            entrySize = (uint)(end - off);
                            break;
                        }
                    }
                    fs.Seek(-8, SeekOrigin.Current);
                    off = fs.Position;
                }

                if (entrySize == 0)
                {
                    fc++;
                    continue;
                }

                fs.Seek(end - 4, SeekOrigin.Begin);
                off = fs.Position;

                while (true)
                {
                    fs.Seek(off - entrySize, SeekOrigin.Begin);
                    off = fs.Position;
                    d32 = br.ReadUInt32();
                    if (d32 == COMPTAIL_0 || d32 == COMPTAIL_1)
                    {
                        idxCount++;
                    }
                    else
                    {
                        break;
                    }
                }

                switch (entrySize - 16)
                {
                    case 16: hSize = 4; break;
                    case 12: hSize = 8; break;
                    case 8: hSize = 12; break;
                    case 4: hSize = 16; break;
                    default: hSize = 0xFFFFFFFF; break;
                }

                if (hSize == 0xFFFFFFFF)
                {
                    d32 = 0xFFFFFFFF;
                }
                else
                {
                    fs.Seek(end - idxCount * entrySize - hSize, SeekOrigin.Begin);
                    off = fs.Position;
                    d32 = br.ReadUInt32();
                }

                fc++;

                if ((d32 & ~0x0007) == 0)
                {
                    fs.Seek(off + hSize, SeekOrigin.Begin);
                    int i;
                    for (i = 0; i < idxCount; i++)
                    {
                        fs.Seek(entrySize - 12, SeekOrigin.Current);

                        uint cs = br.ReadUInt32();
                        uint ucs = br.ReadUInt32();
                        ushort cf = br.ReadUInt16();
                        ushort fl = br.ReadUInt16();

                        if ((cs & 0x80000000) != 0)
                        {
                            cs &= ~0x80000000;
                        }
                        else
                        {
                            break;
                        }

                        if (fl != 0x0001) break;

                        if (cf == 0x0000)
                        {
                            if (cs != ucs) break;
                        }
                        else
                        {
                            if (cs > ucs) break;
                        }
                    }

                    if (i < idxCount)
                    {
                        d32 = 0xFFFFFFFF;
                    }
                    else
                    {
                        break;
                    }
                }

            } while ((d32 & ~0x0007) != 0 && fc < 32);

            if ((d32 & ~0x0007) != 0)
            {
                return false;
            }

            hdr.MajVer = 2;
            hdr.MinVer = 0;
            hdr.IdxCount = idxCount;
            hdr.IdxSize = (uint)(end - off);
            hdr.IdxVer = 3;
            hdr.IdxOffs = (uint)(off - start);

            return true;
        }
    }

    struct DBPFHeader
    {
        public uint MajVer;
        public uint MinVer;
        public uint IdxCount;
        public uint IdxSize;
        public uint IdxVer;
        public uint IdxOffs;
    }
}