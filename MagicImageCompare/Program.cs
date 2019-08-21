using ImageMagick;
using System;
using System.IO;

namespace MagicImageCompare
{
    class Program
    {
        private static readonly string CurrentPath  = Directory
                .GetParent(Environment.CurrentDirectory)
                .Parent.FullName;
        private static readonly double ThreashHold = 0.01;

        static void Main(string[] args)
        {
            
            var expectedImage = new MagickImage(CurrentPath + "\\images\\expected-image.png")
            {
                ColorFuzz = new Percentage(15)
            };
            var generatedImage = new MagickImage(CurrentPath + "\\images\\generated-image.png");

            // correct image comparison. This should pass
            var deltaImagePath = $"{CurrentPath}\\{DateTime.Now.Ticks}.png";
            CompareImages(expectedImage, generatedImage, deltaImagePath);

            // Invalid image comparison. This should fail with a larger compare result
            var files = Directory.GetFiles(CurrentPath + "\\invalid-images\\");
            foreach (var file in files)
            {
                var invalidImage = new MagickImage(file);
                deltaImagePath = $"{CurrentPath}\\{DateTime.Now.Ticks}.png";
                CompareImages(expectedImage, invalidImage, deltaImagePath);
            }

            Console.WriteLine("press any key to exit");
            Console.ReadLine();
        }

        private static void CompareImages(MagickImage expectedImage, 
            MagickImage imageToCompare, 
            string deltaImagePath)
        {
            var fileName = imageToCompare.FileName.Replace(CurrentPath, "");
            using (var delta = new MagickImage())
            {
                var result = expectedImage.Compare(imageToCompare, ErrorMetric.Fuzz, delta);
                if (result > ThreashHold)
                {                    
                    //delta.Write(deltaImagePath);
                    Console.WriteLine($"Threshhold: {ThreashHold} compare result: {result} Does not match {fileName}.");
                }
                else
                {
                    Console.WriteLine($"Threshhold: {ThreashHold} compare result: {result} Matched {fileName}");
                }
            }
        }
    }
}
