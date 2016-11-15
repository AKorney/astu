using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagedCuda;
using ManagedCuda.NPP;

namespace TrafficSignRecognition
{
    internal static class CudaHelper
    {
        public static Bitmap FilterRed(Bitmap src)
        {
            //Bitmap result = new Bitmap(src.Width, src.Height, src.PixelFormat);
            //int _colorChannels = 0;
            //switch (src.PixelFormat)
            //{
            //    case PixelFormat.Format24bppRgb:
            //        _colorChannels = 3;
            //        break;
            //    case PixelFormat.Format32bppArgb:
            //        _colorChannels = 4;
            //        break;
            //    case PixelFormat.Format32bppRgb:
            //        _colorChannels = 4;
            //        break;
            //    case PixelFormat.Format8bppIndexed:
            //        _colorChannels = 1;
            //        break;
            //    default:
            //        _colorChannels = 0;
            //        break;
            //}
            //switch (_colorChannels)
            //{
            //    case 3:
            //        NPPImage_8uC3 bmp_d = new NPPImage_8uC3(src.Width, src.Height);
            //        NPPImage_8uC3 bmpResult_d = new NPPImage_8uC3(src.Width, src.Height);
            //        bmp_d.CopyToDevice(src);
            //        bmp_d.SwapChannels(new int[]{ 2, 1, 0});
            //        bmp_d.border

            //        NPPImage_8uC1 h_image, l_image, s_image;
            //        h_image = new NPPImage_8uC1(src.Width, src.Height);
            //        l_image = new NPPImage_8uC1(src.Width, src.Height);
            //        s_image = new NPPImage_8uC1(src.Width, src.Height);

            //        bmp_d.BGRToHLS(h_image, l_image, s_image);

            //        result.Save("result.jpg");
            //        break;
            //}
            //;

            int redThreshold = 50;
            Bitmap result = new Bitmap(src.Width, src.Height, src.PixelFormat);
            for (int i = 0; i < src.Width; i++)
            {
                for (int j = 0; j < src.Height; j++)
                {
                    Color rgb = src.GetPixel(i, j);
                    float hue = rgb.GetHue();


                    bool hueFlag = hue > 360 - redThreshold || hue < redThreshold;
                    float hueCorrection = hue > 360 - redThreshold
                        ? (hue - (360 - redThreshold)) / redThreshold
                        : (redThreshold - hue) / redThreshold;

                    float sat = rgb.GetSaturation();
                    int colorValue = Convert.ToInt32(255 * sat * hueCorrection);

                    result.SetPixel(i, j, hueFlag ? Color.FromArgb(colorValue, colorValue, colorValue) : Color.Black);

                }
            }
            result.Save("result1.jpg");
            return result;





        }

        public static Bitmap DetectBorders(Bitmap src)
        {
            Bitmap result = new Bitmap(src.Width, src.Height, src.PixelFormat);
            NPPImage_8uC3 bmp_d = new NPPImage_8uC3(src.Width, src.Height);
            NPPImage_8uC3 bmpResult_d = new NPPImage_8uC3(src.Width, src.Height);
            bmp_d.CopyToDevice(src);

           // bmp_d.FilterGauss(bmp_d, MaskSize.Size_5_X_5);

            
            //bmp_d.SobelHoriz(bmp_d);
            //bmp_d.FilterSobelVert(bmp_d);
           
            bmp_d.CopyToHost(result);
            result.Save("result2.jpg");
            return result;
        }
    }
}
