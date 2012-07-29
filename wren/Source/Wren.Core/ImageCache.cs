using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace Wren.Core
{
    public static class ImageCache
    {
        public static Object _lockObject = new Object();

        public static Stream GetImage(String imageName)
        {
            try
            {
                lock (_lockObject)
                {
                    String imageFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Wren", "Images");
                    System.IO.Directory.CreateDirectory(imageFolder);
                    String filePath = Path.Combine(imageFolder, imageName);
                    Byte[] bytes;

                    if (!File.Exists(filePath))
                    {
                        WebClient client = new WebClient();
                        String imageUrl = String.Format("http://{0}/content/images/{1}", WrenCore.ServerAddress, imageName);

                        client.DownloadFile(imageUrl, filePath);
                    }

                    bytes = File.ReadAllBytes(filePath);
                    return new MemoryStream(bytes);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
