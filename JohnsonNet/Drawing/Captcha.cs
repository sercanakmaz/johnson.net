using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace JohnsonNet.Drawing
{

    public class Captcha
    {
        public static void DrawCaptcha(Stream stream, string foreColor = "#ff0000", string backColor = "#000000", int minCharCount = 6, int maxCharCount = 9
                , string Characters = "abcçdefgğhıijklmnoöprsştuüvyxwzABCÇDEFGĞHIİJKLMNOÖPRSŞTUÜVYZ0123456789")
        {
            var charCount = new Random().Next(maxCharCount);
            if (charCount < minCharCount)
                charCount = minCharCount;

            var captchaString = new string(Characters.ToCharArray()
                                                   .OrderBy(c => Guid.NewGuid())
                                                   .Take(charCount)
                                                   .ToArray());
          
            using (SHA1 sha1 = new SHA1CryptoServiceProvider())
            {
                HttpContext.Current.Session["CaptchaString"] =
                            BitConverter.ToString(sha1.ComputeHash(Encoding.UTF8.GetBytes(captchaString)));

            }

            using (IDisposable b = new SolidBrush(System.Drawing.ColorTranslator.FromHtml(foreColor)),
                              f = new Font("Thoma", 24f),
                              i = new Bitmap(1, 1),
                              g = Graphics.FromImage(i as Image))
            {
                var size = (g as Graphics).MeasureString(captchaString, f as Font);
                int prevAngle = 0, chw = (int)(size.Width / captchaString.Length);
                using (var img = new Bitmap((int)size.Width, (int)size.Height))
                {
                    using (var graphics = Graphics.FromImage(img))
                    {
                        graphics.Clear(System.Drawing.ColorTranslator.FromHtml(backColor));
                        for (var c = 0; c < captchaString.Length; c++)
                        {
                            prevAngle = DrawCharacter(captchaString[c].ToString(), graphics, b as Brush, f as Font, c * chw, chw, (int)size.Width, (int)size.Height, prevAngle);
                        }
                        //graphics.DrawString(captchaString, f as Font, b as Brush, 0, 0);
                        graphics.Flush();
                    }

                    img.Save(stream, ImageFormat.Gif);
                }

            }
        }
        static int DrawCharacter(string txt, Graphics gr, Brush b,
            Font the_font, int X, int ch_wid, int wid, int hgt, int prevAngle = 0)
        {
            // Center the text.
            StringFormat string_format = new StringFormat();
            string_format.Alignment = StringAlignment.Center;
            string_format.LineAlignment = StringAlignment.Center;
            RectangleF rectf = new RectangleF(X, 0, ch_wid, hgt);

            // Convert the text into a path.
            using (GraphicsPath graphics_path = new GraphicsPath())
            {
                graphics_path.AddString(txt, the_font.FontFamily,
                    (int)(the_font.Style), the_font.Size, rectf, string_format);

                // Make random warping parameters.
                Random rnd = new Random();
                float x1 = (float)(X + rnd.Next(ch_wid) / 2);
                float y1 = (float)(rnd.Next(hgt) / 2);
                float x2 = (float)(X + ch_wid / 2 + rnd.Next(ch_wid) / 2);
                float y2 = (float)(hgt / 2 + rnd.Next(hgt) / 2);
                PointF[] pts = {
                    new PointF((float)(X + rnd.Next(ch_wid) / 4), (float)(rnd.Next(hgt) / 4)),
                    new PointF((float)(X + ch_wid - rnd.Next(ch_wid) / 4), (float)(rnd.Next(hgt) / 4)),
                    new PointF((float)(X + rnd.Next(ch_wid) / 4), (float)(hgt - rnd.Next(hgt) / 4)),
                    new PointF((float)(X + ch_wid - rnd.Next(ch_wid) / 4), (float)(hgt - rnd.Next(hgt) / 4))
                };
                Matrix mat = new Matrix();
                graphics_path.Warp(pts, rectf, mat, WarpMode.Perspective, 0);

                // Rotate a bit randomly.
                float dx = (float)(X + ch_wid / 2);
                float dy = (float)(hgt / 2);
                gr.TranslateTransform(-dx, -dy, MatrixOrder.Append);
                int angle = prevAngle;
                do
                {
                    angle = rnd.Next(-30, 30);
                } while (Math.Abs(angle - prevAngle) < 20);
                prevAngle = angle;
                gr.RotateTransform(angle, MatrixOrder.Append);
                gr.TranslateTransform(dx, dy, MatrixOrder.Append);

                // Draw the text.
                gr.FillPath(b, graphics_path);
                gr.ResetTransform();
            }

            return prevAngle;
        }

        public static bool CheckCaptcha(string captchaString)
        {
            if (string.IsNullOrEmpty(captchaString)) return false;

            using (SHA1 sha1 = new SHA1CryptoServiceProvider())
                return (HttpContext.Current.Session["CaptchaString"] as string).Equals(BitConverter.ToString(sha1.ComputeHash(Encoding.UTF8.GetBytes(captchaString))));
        }

    }
}
