using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Wad3Parser;

namespace Wad3Convertor
{
    public class Texture
    {
        public string textureName;
        public byte textureType;
        public int width, height;
        public BitmapSource image;
        public BitmapSource fullImage;
        public bool hasPalatte;
        public List<Color> palatte;

        public Texture(WadLump lump)
        {
            this.textureName = new string(lump.lumpInfo.name);
            this.textureType = lump.lumpInfo.type;
            this.width = (int)(lump.width);
            this.height = (int)(lump.height);
            this.image = WadLumpToBitmapSource(lump, 0);
            this.fullImage = WadLumpToBitmapSource(lump, 1);

            switch (lump.lumpInfo.type)
            {
                case 0x40:
                    {
                        hasPalatte = true;
                        palatte = (lump as Lump40).palette;
                        break;
                    }
                case 0x43:
                    {
                        hasPalatte = true;
                        palatte = (lump as Lump43).palette;
                        break;
                    }
                case 0x42:
                case 0x46:
                    {
                        hasPalatte = false;
                        break;
                    }
                default:
                    {
                        throw new Exception();
                    }
            }
        }

        private BitmapSource WadLumpToBitmapSource(WadLump lump, int mode)
        {
            string textureName = new string(lump.lumpInfo.name);
            byte textureType = lump.lumpInfo.type;
            switch (lump.lumpInfo.type)
            {
                case 0x42:
                    {
                        Lump42 tLump = (Lump42)lump;
                        return BitmapSource.Create((int)tLump.width, (int)tLump.height, 96d, 96d, PixelFormats.Indexed8, new BitmapPalette(tLump.palette), tLump.data, (int)tLump.width);
                    }
                case 0x40:
                    {
                        Lump40 tLump = (Lump40)lump;

                        if (mode == 0)
                        {
                            return BitmapSource.Create((int)tLump.width, (int)tLump.height, 96d, 96d, PixelFormats.Indexed8, new BitmapPalette(tLump.palette), tLump.data, (int)tLump.width);
                        }
                        else
                        {

                            byte[] bytes = MipMapFormat(tLump.width, tLump.height, tLump.data, tLump.dataMipmap1, tLump.dataMipmap2, tLump.dataMipmap3);
                            return BitmapSource.Create((int)tLump.width, (int)tLump.height * 3 / 2, 96d, 96d, PixelFormats.Indexed8, new BitmapPalette(tLump.palette), bytes, (int)tLump.width);
                        }

                    }
                case 0x43:
                    {
                        Lump43 tLump = (Lump43)lump;

                        if (mode == 0)
                        {
                            return BitmapSource.Create((int)tLump.width, (int)tLump.height, 96d, 96d, PixelFormats.Indexed8, new BitmapPalette(tLump.palette), tLump.data, (int)tLump.width);
                        }
                        else
                        {

                            byte[] bytes = MipMapFormat(tLump.width, tLump.height, tLump.data, tLump.dataMipmap1, tLump.dataMipmap2, tLump.dataMipmap3);
                            return BitmapSource.Create((int)tLump.width, (int)tLump.height * 3 / 2, 96d, 96d, PixelFormats.Indexed8, new BitmapPalette(tLump.palette), bytes, (int)tLump.width);
                        }

                    }
                case 0x46:
                    {
                        Lump46 tLump = (Lump46)lump;
                        return BitmapSource.Create((int)tLump.width, (int)tLump.height, 96d, 96d, PixelFormats.Indexed8, new BitmapPalette(tLump.palette), tLump.data, (int)tLump.width);
                    }
                default:
                    {
                        throw new Exception();
                    }
            }

        }

        private byte[] MipMapFormat(uint width, uint height, byte[] data, byte[] dataMipmap1, byte[] dataMipmap2, byte[] dataMipmap3)
        {
            int size = (int)(width * height);
            byte[] bytes = new byte[size * 3 / 2];

            // 全部置255
            for (int i = 0; i < size * 3 / 2; i++)
            {
                bytes[i] = 255;
            }

            for (int i = 0; i < size; i++)
            {
                bytes[i] = data[i];
            }

            for (int i = 0; i < height / 2; i++)
            {
                for (int j = 0; j < width / 2; j++)
                {
                    bytes[size + i * width + j] = dataMipmap1[i * width / 2 + j];
                }
            }

            for (int i = 0; i < height / 4; i++)
            {
                for (int j = 0; j < width / 4; j++)
                {
                    bytes[size + width / 2 + i * width + j] = dataMipmap2[i * width / 4 + j];
                }
            }

            for (int i = 0; i < height / 8; i++)
            {
                for (int j = 0; j < width / 8; j++)
                {
                    bytes[size + width / 2 + width / 4 + i * width + j] = dataMipmap3[i * width / 8 + j];
                }
            }

            return bytes;
        }

    }

}
