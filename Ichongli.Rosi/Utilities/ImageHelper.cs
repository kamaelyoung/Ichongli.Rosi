using System.IO;

namespace Ichongli.Rosi
{
    public enum ImageType
    {
        Unknown, Jepg, Png, Webp
    }
    public class ImageInfo
    {
        public ImageType format;
        public int width, height, bits, channels;
    }
    public class ImageHelper
    {
        private const int M_SOF0 = 0xC0;
        private const int M_SOF1 = 0xC1;
        private const int M_SOF2 = 0xC2;
        private const int M_SOF3 = 0xC3;
        private const int M_SOF5 = 0xC5;
        private const int M_SOF6 = 0xC6;
        private const int M_SOF7 = 0xC7;
        private const int M_SOF9 = 0xC9;
        private const int M_SOF10 = 0xCA;
        private const int M_SOF11 = 0xCB;
        private const int M_SOF13 = 0xCD;
        private const int M_SOF14 = 0xCE;
        private const int M_SOF15 = 0xCF;
        private const int M_SOI = 0xD8;
        private const int M_EOI = 0xD9;
        private const int M_SOS = 0xDA;
        private const int M_APP0 = 0xe0;
        private const int M_APP1 = 0xe1;
        private const int M_APP2 = 0xe2;
        private const int M_APP3 = 0xe3;
        private const int M_APP4 = 0xe4;
        private const int M_APP5 = 0xe5;
        private const int M_APP6 = 0xe6;
        private const int M_APP7 = 0xe7;
        private const int M_APP8 = 0xe8;
        private const int M_APP9 = 0xe9;
        private const int M_APP10 = 0xea;
        private const int M_APP11 = 0xeb;
        private const int M_APP12 = 0xec;
        private const int M_APP13 = 0xed;
        private const int M_APP14 = 0xee;
        private const int M_APP15 = 0xef;
        private const int M_COM = 0xFE;
        private const int M_PSEUDO = 0xFFD8;

        private static int nextMarker(Stream stream, int last_marker, int comment_correction, int ff_read)
        {
            int a = 0, marker;
            if (last_marker == M_COM && comment_correction != 0)
            {
                comment_correction = 2;
            }
            else
            {
                last_marker = 0;
                comment_correction = 0;
            }
            if (ff_read != 0)
            {
                a = 1;
            }
            do
            {
                if ((marker = stream.ReadByte()) == -1)
                {
                    return M_EOI;
                }
                if (last_marker == M_COM && comment_correction > 0)
                {
                    if (marker != 0xFF)
                    {
                        marker = 0xff;
                        comment_correction--;
                    }
                    else
                    {
                        last_marker = M_PSEUDO;
                    }
                }
                a++;
            } while (marker == 0xff);
            if (a < 2)
            {
                return M_EOI;
            }
            if (last_marker == M_COM && comment_correction != 0)
            {
                return M_EOI;
            }
            return marker;
        }

        private static int read2(Stream stream)
        {
            byte[] a = new byte[2];
            if ((stream.Read(a, 0, 2)) <= 0)
                return 0;

            return ((a[0] & 0x0ff) << 8) + (a[1] & 0x0ff);
        }

        private static int skipVariable(Stream stream)
        {
            int length = read2(stream);

            if (length < 2)
            {
                return 0;
            }
            length = length - 2;
            stream.Seek(length, SeekOrigin.Current);
            return 1;
        }

        public static ImageInfo handle(string file)
        {
            using (var fis = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                return handle(fis);
            }
        }

        public static ImageInfo handle(Stream stream)
        {
            byte[] buf = new byte[16];
            if (stream.Read(buf, 0, 3) != 3)
            {
                return null;
            }
            if ((buf[0] & 0x0ff) == 0xff && (buf[1] & 0x0ff) == M_SOI && (buf[2] & 0x0ff) == 0xff)
            {
                return handleJpeg(stream);
            }
            else if ((buf[0] & 0x0ff) == 0x89 && (buf[1] & 0x0ff) == 0x50 && (buf[2] & 0x0ff) == 0x4e)
            {
                if (stream.Read(buf, 3, 5) != 5)
                {
                    return null;
                }
                if ((buf[3] & 0x0ff) == 0x47 && (buf[4] & 0x0ff) == 0x0d && (buf[5] & 0x0ff) == 0x0a && (buf[6] & 0x0ff) == 0x1a && (buf[7] & 0x0ff) == 0x0a)
                {
                    return handlePng(stream);
                }
            }
            else if ((buf[0] & 0x0ff) == 0x52 && (buf[1] & 0x0ff) == 0x49 && (buf[2] & 0x0ff) == 0x46)
            {
                if (stream.Read(buf, 3, 13) != 13)
                {
                    return null;
                }
                if ((buf[3] & 0x0ff) == 0x46 && (buf[8] & 0x0ff) == 'W' && (buf[9] & 0x0ff) == 'E' && (buf[10] & 0x0ff) == 'B' && (buf[11] & 0x0ff) == 'P'
                        && (buf[12] & 0x0ff) == 'V' && (buf[13] & 0x0ff) == 'P' && (buf[14] & 0x0ff) == '8')
                {
                    if ((buf[15] & 0x0ff) == 'X')
                    {
                        return handleWebPX(stream);
                    }
                    else
                    {
                        return handleWebP(stream);
                    }
                }
            }
            return null;
        }

        private static ImageInfo handleWebPX(Stream stream)
        {
            stream.Seek(8, SeekOrigin.Current);
            byte[] buf = new byte[4];
            if (stream.Read(buf, 0, 6) != 6)
            {
                return null;
            }
            ImageInfo info = new ImageInfo();
            info.format = ImageType.Webp;
            info.width = 1 + ((buf[2] & 0x0ff) << 16) + ((buf[1] & 0x0ff) << 8) + (buf[0] & 0x0ff);
            info.height = 1 + ((buf[5] & 0x0ff) << 16) + ((buf[4] & 0x0ff) << 8) + (buf[3] & 0x0ff);
            return info;
        }

        private static ImageInfo handleWebP(Stream stream)
        {
            stream.Seek(10, SeekOrigin.Current);
            byte[] buf = new byte[4];
            if (stream.Read(buf, 0, 4) != 4)
            {
                return null;
            }
            ImageInfo info = new ImageInfo();
            info.format = ImageType.Webp;
            info.width = ((buf[1] & 0x0ff) << 8) + (buf[0] & 0x0ff);
            info.height = ((buf[3] & 0x0ff) << 8) + (buf[2] & 0x0ff);
            return info;
        }

        private static ImageInfo handlePng(Stream stream)
        {
            stream.Seek(8, SeekOrigin.Current);
            byte[] buf = new byte[9];
            if (stream.Read(buf, 0, 9) != 9)
            {
                return null;
            }
            ImageInfo info = new ImageInfo();
            info.format = ImageType.Png;
            info.width = ((buf[0] & 0x0ff) << 24) + ((buf[1] & 0x0ff) << 16) + ((buf[2] & 0x0ff) << 8) + (buf[3] & 0x0ff);
            info.height = ((buf[4] & 0x0ff) << 24) + ((buf[5] & 0x0ff) << 16) + ((buf[6] & 0x0ff) << 8) + (buf[7] & 0x0ff);
            info.bits = (buf[8] & 0x0ff);
            return info;
        }

        private static ImageInfo handleJpeg(Stream stream)
        {
            ImageInfo info = null;
            int marker = M_PSEUDO;
            short ff_read = 1;

            for (; ; )
            {
                marker = nextMarker(stream, marker, 1, ff_read);
                ff_read = 0;
                switch (marker)
                {
                    case M_SOF0:
                    case M_SOF1:
                    case M_SOF2:
                    case M_SOF3:
                    case M_SOF5:
                    case M_SOF6:
                    case M_SOF7:
                    case M_SOF9:
                    case M_SOF10:
                    case M_SOF11:
                    case M_SOF13:
                    case M_SOF14:
                    case M_SOF15:
                        {
                            if (info == null)
                            {
                                info = new ImageInfo();
                                info.format = ImageType.Jepg;
                                int length = read2(stream);
                                info.bits = stream.ReadByte();
                                info.height = read2(stream);
                                info.width = read2(stream);
                                info.channels = stream.ReadByte();
                                if (length < 8)
                                {
                                    return info;
                                }
                                if (stream.Seek(length - 8, SeekOrigin.Current) != 0)
                                {
                                    return info;
                                }
                            }
                            else
                            {
                                if (skipVariable(stream) == 0)
                                {
                                    return info;
                                }
                            }
                            break;
                        }
                    case M_APP0:
                    case M_APP1:
                    case M_APP2:
                    case M_APP3:
                    case M_APP4:
                    case M_APP5:
                    case M_APP6:
                    case M_APP7:
                    case M_APP8:
                    case M_APP9:
                    case M_APP10:
                    case M_APP11:
                    case M_APP12:
                    case M_APP13:
                    case M_APP14:
                    case M_APP15:
                        if (skipVariable(stream) == 0)
                        {
                            return info;
                        }
                        break;
                    case M_SOS:
                    case M_EOI:
                        return info;
                    default:
                        if (skipVariable(stream) == 0)
                        {
                            return info;
                        }
                        break;
                }
            }
        }
    }
}