using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using ImageResizer.ExtensionMethods;
using Microsoft.WindowsAzure.Storage;
using ImageResizer;
using Microsoft.WindowsAzure.Storage.Blob;

namespace WebJobImageResizer
{
    public class Functions
    {
        public static void ResizeImagesTask(
            [BlobTrigger("input/{name}.{ext}")] Stream inputBlob,
            string name,
            string ext,
            IBinder binder)
        {
            int[] sizes = { 800, 500, 250 };
            var inputBytes = inputBlob.CopyToBytes();
            foreach (var width in sizes)
            {
                var input = new MemoryStream(inputBytes);
                var output = binder.Bind<Stream>(new BlobAttribute($"output/{name}-w{width}.{ext}", FileAccess.Write));

                ResizeImage(input, output, width);
            }
        }

        private static void ResizeImage(Stream input, Stream output, int width)
        {
            var instructions = new Instructions
            {
                Width = width,
                Mode = FitMode.Carve,
                Scale = ScaleMode.Both
            };
            ImageBuilder.Current.Build(new ImageJob(input, output, instructions));
        }
    }
}
