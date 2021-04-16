using System;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using ImageMagick;

namespace GraphicRenamer
{
    public static class HeicToJpeg
    {

        public static void Convert(string filename, string savepath)
        {
            try
            {
                // Read image from file
                using (var inputImage = new MagickImage(filename))
                {
                    inputImage.Write(savepath);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
