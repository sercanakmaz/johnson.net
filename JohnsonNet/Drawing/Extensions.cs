using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace JohnsonNet.Drawing
{
    public static class Extensions
    {
        public static string GetMimeType(this Image i, string extension = null)
        {
            foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageDecoders())
            {
                if (codec.FormatID == i.RawFormat.Guid)
                    return codec.MimeType;
            }

            if (string.IsNullOrEmpty(extension)) return "image/unknown";
            else return string.Format("image/{0}", extension.ToLower().Substring(1).Replace("jpg", "jpeg"));
        }
        public static string ToExtension(this ImageFormat format)
        {
            var encoder = ImageCodecInfo.GetImageEncoders().FirstOrDefault(x => x.FormatID == format.Guid);
            return encoder.FilenameExtension
                    .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .First().Substring(1).ToLower();
        }
        public static ImageCodecInfo GetEncoderInfo(this Image obj, string extension = null)
        {
            string mimeType = obj.GetMimeType(extension);
            // Get image codecs for all image formats 
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec 
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];
            return null;
        } /// highest quality</param> 
        public static void SaveLowerQuality(this Image img, string path, int quality)
        {
            if (quality < 0 || quality > 100)
                throw new ArgumentOutOfRangeException("quality must be between 0 and 100.");

            // Encoder parameter for image quality 
            EncoderParameter qualityParam =
                new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            // Jpeg image codec 
            ImageCodecInfo jpegCodec = img.GetEncoderInfo(System.IO.Path.GetExtension(path));

            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;

            img.Save(path, jpegCodec, encoderParams);
        }
      
        public static Image ConvertToBase64Image(this string base64String)
        {
            // Convert Base64 String to byte[]

            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
            imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }

        public static Image ConvertToHexImage(this string hexString)
        {
            // Convert Base64 String to byte[]

            byte[] imageBytes = HexString2Bytes(hexString);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
            imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }

        private static byte[] HexString2Bytes(string hexString)
        {
            int bytesCount = (hexString.Length) / 2;
            byte[] bytes = new byte[bytesCount];
            for (int x = 0; x < bytesCount; ++x)
            {
                bytes[x] = Convert.ToByte(hexString.Substring(x * 2, 2), 16);
            }

            return bytes;
        }

    }
}
