using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace IntegrationsTester.GeneralFunctions
{
    public class DataManipulation
    {
        public static BitmapImage DecodeBase64Image(string base64string)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(base64string);

                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = new MemoryStream(bytes);
                bi.EndInit();

                return bi;
            }
            catch (Exception ex)
            {
                //Implement Logging
                return null;
            }
        }
    }
}
