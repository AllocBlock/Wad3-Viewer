using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;

namespace Wad3Parser
{

    class Header
    {
        public char[] magic;       // 1 Offset to the character in data
        public uint number;        // 4 number of texture
        public uint offset;        // 4 offset of the start to first lump info

        public void Read(ref BinaryReader f)
        {
            magic = f.ReadChars(4);
            number = f.ReadUInt32();
            offset = f.ReadUInt32();
        }
    };

    public class LumpInfo
    {
        public uint offset;        // 4 offset of the start to lump
        public uint cSize;         // 4 the compressed size of lump
        public uint fullSize;      // 4 the really size of lump
        public byte type;          // 1 the type of lump, 0x40. 0x42, 0x43 or 0x46 in half-life
        public byte cType;         // 1 compression type
        public byte[] padding;     // 2 used for alignment and it's ignored
        public char[] name;        // 16 the name of texture, end with '\0'

        public void Read(ref BinaryReader f)
        {
            offset = f.ReadUInt32();
            cSize = f.ReadUInt32();
            fullSize = f.ReadUInt32();
            type = f.ReadByte();
            cType = f.ReadByte();
            padding = f.ReadBytes(2);
            name = f.ReadChars(16);
        }

        public static List<LumpInfo> ReadAll(ref BinaryReader f, uint number, uint offset)
        {
            f.BaseStream.Seek(offset, SeekOrigin.Begin);
            List<LumpInfo> res = new List<LumpInfo>();
            for (int i = 0; i < number; i++)
            {
                LumpInfo t = new LumpInfo();
                t.Read(ref f);
                res.Add(t);
            }
            return res;
        }
    };

    struct CharInfo
    {
        public ushort offset;      // 2 the start of character from left to right
        public ushort width;       // 2 the width of character
    };

    public abstract class WadLump
    {
        public uint width, height;          // 4 the size of font picture. width is always 256 pixels
        public byte[] data;                // width*height pixel data, each char save an index to palette
        public ushort colorsUsed;           // 2 the number of colors that are used(always 0x00 0x01?)
        public ushort padding;              // 2 use for padding

        public LumpInfo lumpInfo;

        public abstract void Read(ref BinaryReader f, LumpInfo lumpInfo);
    }
    class Lump40 : Lump43 { }
    class Lump42 : WadLump
    {
        public List<Color> palette;             // 256 * 3 palette data in RGB form
        public override void Read(ref BinaryReader f, LumpInfo lumpInfo)
        {
            this.lumpInfo = lumpInfo;
            f.BaseStream.Seek(lumpInfo.offset, SeekOrigin.Begin);

            width = f.ReadUInt32();
            height = f.ReadUInt32();

            int size = (int)(height * width);

            data = new byte[size];
            data = f.ReadBytes(size);

            colorsUsed = f.ReadUInt16();

            palette = new List<Color>(256);
            for (int i = 0; i < 256; i++)
            {
                Color t = new Color();
                t.R = f.ReadByte();
                t.G = f.ReadByte();
                t.B = f.ReadByte();
                t.A = 0xff;
                palette.Add(t);
            }
        }
    };
    class Lump43 : WadLump
    {
        public char[] textureName;              // 4 the number of rows
        public uint offset;                     // 4 offset of origin image
        public uint offsetMipmap1;              // 4 offset of mipmap1
        public uint offsetMipmap2;              // 4 offset of mipmap2
        public uint offsetMipmap3;              // 4 offset of mipmap3
        public byte[] dataMipmap1;                // width*height/4 data of mipmap1
        public byte[] dataMipmap2;                // width*height/16 data of mipmap2
        public byte[] dataMipmap3;                // width*height/64 data of mipmap3
        public List<Color> palette;             // 256 * 3 palette data in RGB form
        public override void Read(ref BinaryReader f, LumpInfo lumpInfo)
        {
            this.lumpInfo = lumpInfo;
            f.BaseStream.Seek(lumpInfo.offset, SeekOrigin.Begin);

            textureName = f.ReadChars(16);
            width = f.ReadUInt32();
            height = f.ReadUInt32();
            offset = f.ReadUInt32();
            offsetMipmap1 = f.ReadUInt32();
            offsetMipmap2 = f.ReadUInt32();
            offsetMipmap3 = f.ReadUInt32();

            //f.BaseStream.Seek(offset, SeekOrigin.Begin);

            int size = (int)(height * width);

            data = new byte[size];
            data = f.ReadBytes(size);

            //f.BaseStream.Seek(offsetMipmap1, SeekOrigin.Begin);
            dataMipmap1 = new byte[size / 4];
            dataMipmap1 = f.ReadBytes(size / 4);


            //f.BaseStream.Seek(offsetMipmap2, SeekOrigin.Begin);
            dataMipmap2 = new byte[size / 16];
            dataMipmap2 = f.ReadBytes(size / 16);


            //f.BaseStream.Seek(offsetMipmap3, SeekOrigin.Begin);
            dataMipmap3 = new byte[size / 64];
            dataMipmap3 = f.ReadBytes(size / 64);

            colorsUsed = f.ReadUInt16();

            palette = new List<Color>(256);
            for (int i = 0; i < 256; i++)
            {
                Color t = new Color();
                t.R = f.ReadByte();
                t.G = f.ReadByte();
                t.B = f.ReadByte();
                t.A = 0xff;
                palette.Add(t);
            }
        }
    };

    class Lump46 : WadLump
    {
        public uint rowCount;                   // 4 the number of rows
        public uint rowHeight;                  // 4 the height of a row
        public List<CharInfo> charInfo;         // 256 * 4 info about each character
        public List<Color> palette;             // 256 * 3 palette data in RGB form
        public override void Read(ref BinaryReader f, LumpInfo lumpInfo)
        {
            this.lumpInfo = lumpInfo;

            f.BaseStream.Seek(lumpInfo.offset, SeekOrigin.Begin);
            width = f.ReadUInt32();
            width = 256; // force
            height = f.ReadUInt32();
            rowCount = f.ReadUInt32();
            rowHeight = f.ReadUInt32();

            charInfo = new List<CharInfo>();
            for (int i = 0; i < 256; i++)
            {
                CharInfo t = new CharInfo();
                t.offset = f.ReadUInt16();
                t.width = f.ReadUInt16();
                charInfo.Add(t);
            }

            int size = (int)(height * width);
            data = new byte[size];
            data= f.ReadBytes(size);

            colorsUsed = f.ReadUInt16();

            palette = new List<Color>(256);
            for (int i = 0; i < 256; i++)
            {
                Color t =  new Color();
                t.R = f.ReadByte();
                t.G = f.ReadByte();
                t.B = f.ReadByte();
                t.A = 0xff;
                palette.Add(t);
            }
        }

    };

    class Parser
    {
        public List<WadLump> Parse(string fileName)
        {
            List<WadLump> res = new List<WadLump>();
            BinaryReader f = null;
            try
            {
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                f = new BinaryReader(fs);
            }
            catch
            {
                //MessageBox.Show("文件打开失败！", "错误", 0, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
            

            Header header = new Header();
            header.Read(ref f);

            if (new string(header.magic) != "WAD3")
            {
                //MessageBox.Show("非WAD3文件或文件损坏！", "错误", 0, MessageBoxIcon.Error);
                return null;
            }

            List<LumpInfo> lumpInfos = LumpInfo.ReadAll(ref f, header.number, header.offset);
            foreach (LumpInfo lumpInfo in lumpInfos)
            {
                WadLump lump = null;
                if (lumpInfo.type == 0x40)
                {
                    lump = new Lump43();
                }
                else if (lumpInfo.type == 0x42)
                {
                    lump = new Lump42();
                }
                else if (lumpInfo.type == 0x43)
                {
                    lump = new Lump43();
                }
                else if (lumpInfo.type == 0x46)
                {
                    lump = new Lump46();

                }
                else
                {
                    MessageBox.Show("纹理类型(0x"+ Convert.ToString(lumpInfo.type, 16) + ")不被支持", "错误", 0, MessageBoxIcon.Error);
                    continue;
                }
                lump.Read(ref f, lumpInfo);
                res.Add(lump);
            }

            f.Close();
            return res;
        }
    }

}
