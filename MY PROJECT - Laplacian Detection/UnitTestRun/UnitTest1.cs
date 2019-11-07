using LearningFoundation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.IO;
using Laplacianedgedetection;
using System;

namespace UnitTest
{

    [TestClass]
    public class LaplacianDetection    
    {
        /// <summary>
        /// This converts the input file from Bitmap to Array
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>

        public double[,,] BitmapTo3DArray(Bitmap bitmap)
        {
            int imageW = bitmap.Width;
            int imageH = bitmap.Height;


            double[,,] ResultArray = new double[imageW, imageH, 3];

            for (int w = 0; w < imageW; w++)
            {
                for (int h = 0; h < imageH; h++)
                {
                    Color color = bitmap.GetPixel(w, h);
                    ResultArray[w, h, 0] = color.R;
                    ResultArray[w, h, 1] = color.G;
                    ResultArray[w, h, 2] = color.B;
                }

            }

            return ResultArray;
        }


        /// <summary>
        /// This Coverts the file from Array to Bitmap
        /// </summary>
        /// <param name="doubleArray"></param>
        /// <returns></returns>


        public Bitmap ArrayToBitmap(double[,,] doubleArray)
        {
            Bitmap resbitmap = new Bitmap(doubleArray.GetLength(0), doubleArray.GetLength(1));

            for (int i = 0; i < doubleArray.GetLength(0); i++)
            {
                for (int j = 0; j < doubleArray.GetLength(1); j++)
                {
                    int Red = (int)doubleArray[i, j, 0];
                    int Green = (int)doubleArray[i, j, 1];
                    int Blue = (int)doubleArray[i, j, 2];
                    resbitmap.SetPixel(i, j, Color.FromArgb(255, Red, Green, Blue));
                }

            }
            return resbitmap;
        }


        /// <summary>
        /// this is a Load method to load the file in Bitmap. After loading it will be converted to double[,,]
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>


        public double[,,] Load(string filename)
        {
            Bitmap bitmap = new Bitmap(filename);

            return BitmapTo3DArray(bitmap);
        }


        /// <summary>
        /// "Save method" to save the image in Bitmap after the algorithm has been executed.
        /// </summary>
        /// <param name="ResultArray"></param>
        /// <param name="filename"></param>

        public void Save(double[,,] ResultArray, string filename)
        {
            Bitmap bitmap = ArrayToBitmap(ResultArray);

            bitmap.Save(filename);
        }


        /// <summary>
        /// this Executes the Laplacian Algorithm. Bitmap will be loaded and converted to double[,,]. After that the Laplacian algorithm will be executed. The edges of the images will be detected
        /// Then the Test Picture will be converted back to Bitmap and saved in OutputImages folder. The OutputImages folder will be made in bin folder. 
        /// </summary>

        [TestMethod]
        public void LaplacianUnitTest1()
        {
            Lap laplacian = new Lap();
            LearningApi a = new LearningApi();

            a.UseActionModule<double[,,], double[,,]>((input, ctx) =>
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string path = Path.Combine(baseDirectory, "TestImages\\img2.jpg");
                double[,,] data = Load(path);

                return data;
            });

            a.AddModule(laplacian);

            double[,,] output = a.Run() as double[,,];
            Assert.IsNotNull(output);

            string baseDirectory2 = AppDomain.CurrentDomain.BaseDirectory;
            string outputPath = Path.Combine(baseDirectory2, "OutputImages\\img_Test.jpg");
            Save(output, outputPath);


        }

    }   

}

