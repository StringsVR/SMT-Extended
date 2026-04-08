using System;
using System.Collections.Generic;
using System.Text;

namespace SMT.Core.Utilities
{
    public static class NetworkUtils
    {
        public static async Task DownloadFileAsync(string url, string outputPath)
        {
            using HttpClient client = new HttpClient();

            byte[] data = await client.GetByteArrayAsync(url);

            await File.WriteAllBytesAsync(outputPath, data);
        }
    }
}
