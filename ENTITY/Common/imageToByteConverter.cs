using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;


namespace Entity.Common
{
    public class imageToByteConverter
    {
        public static byte[] image2byte(string docfilepath)
        {
            Image img = Image.FromFile(docfilepath);
            ImageConverter converter = new ImageConverter();
            byte[] imgByte = (byte[])converter.ConvertTo(img, typeof(byte[]));
            img.Dispose();
            return imgByte;
        }

        public static string Byte2Photo(byte[] byteArray, string docfilepath)
        {
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                bool isExists = Directory.Exists(docfilepath);
                if (!isExists)
                    Directory.CreateDirectory(docfilepath);

                Bitmap img = (Bitmap)Image.FromStream(ms);
                string imageName = "temp." + ImageFormat.Jpeg;
                img.Save(docfilepath + "\\" + imageName);
                ms.Close();
                return imageName;
            }
        }

        public static string Byte2Photo(byte[] byteArray, string docfilepath, string fileName)
        {
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                bool isExists = Directory.Exists(docfilepath);
                if (!isExists)
                    Directory.CreateDirectory(docfilepath);

                Bitmap img = (Bitmap)Image.FromStream(ms);
                string imageName = fileName + "." + ImageFormat.Jpeg;
                img.Save(docfilepath + "\\" + imageName);
                ms.Close();
                return imageName;
            }
        }

        public static byte[] dataURL2byte(string dataurl)
        {
            string[] dataURL = dataurl.Split(new char[] { ',' }, 2);

            byte[] urlByte = Convert.FromBase64String(dataURL[1]);
            return urlByte;
        }

        public static string byte2dataURL(byte[] urlByte)
        {
            string dataURL = Convert.ToBase64String(urlByte);
            return dataURL;
        }

        public static string Byte2PDF(byte[] byteArray, string docfilepath, string fileName)
        {
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                bool isExists = Directory.Exists(docfilepath);
                if (!isExists)
                    Directory.CreateDirectory(docfilepath);

                Bitmap img = (Bitmap)Image.FromStream(ms);
                string imageName = fileName + "." + ImageFormat.Jpeg;
                img.Save(docfilepath + "\\" + imageName);
                ms.Close();
                return imageName;
            }
        }
        //public byte[] imageToByteArray(System.Drawing.Image imageIn)
        //{
        //    MemoryStream ms = new MemoryStream();
        //    imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
        //    return ms.ToArray();
        //}
    }
}
