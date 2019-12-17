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

        public Texture(WadLump lump)
        {
            this.textureName = new string(lump.lumpInfo.name);
            this.textureType = lump.lumpInfo.type;
            this.width = (int)(lump.width);
            this.height = (int)(lump.height);
            this.image = WadLumpToBitmapSource(lump, 0);
            this.fullImage = WadLumpToBitmapSource(lump, 1);
        }

        private BitmapSource WadLumpToBitmapSource(WadLump lump, int mode)
        {
            string textureName = new string(lump.lumpInfo.name);
            byte textureType = lump.lumpInfo.type;
            switch (lump.lumpInfo.type)
            {
                case 0x43:
                    {
                        Lump43 tLump = (Lump43)lump;

                        if (mode == 0)
                        {
                            return BitmapSource.Create((int)tLump.width, (int)tLump.height, 96d, 96d, PixelFormats.Indexed8, new BitmapPalette(tLump.palette), tLump.data, (int)tLump.width);
                        }
                        else
                        {
                            int size = (int)(tLump.width * tLump.height);
                            byte[] bytes = new byte[size * 3 / 2];

                            // 全部置255
                            for (int i = 0; i < size * 3 / 2; i++)
                            {
                                bytes[i] = 255;
                            }

                            for (int i = 0; i < size; i++)
                            {
                                bytes[i] = tLump.data[i];
                            }

                            for (int i = 0; i < tLump.height / 2; i++)
                            {
                                for (int j = 0; j < tLump.width / 2; j++)
                                {
                                    bytes[size + i * tLump.width + j] = tLump.dataMipmap1[i * tLump.width / 2 + j];
                                }
                            }

                            for (int i = 0; i < tLump.height / 4; i++)
                            {
                                for (int j = 0; j < tLump.width / 4; j++)
                                {
                                    bytes[size + tLump.width / 2 + i * tLump.width + j] = tLump.dataMipmap2[i * tLump.width / 4 + j];
                                }
                            }

                            for (int i = 0; i < tLump.height / 8; i++)
                            {
                                for (int j = 0; j < tLump.width / 8; j++)
                                {
                                    bytes[size + tLump.width / 2 + tLump.width / 4 + i * tLump.width + j] = tLump.dataMipmap3[i * tLump.width / 8 + j];
                                }
                            }

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

    }
    class Wad3Convertor
    {
       
    }

}
