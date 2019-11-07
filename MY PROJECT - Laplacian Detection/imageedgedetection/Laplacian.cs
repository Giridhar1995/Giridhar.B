using LearningFoundation;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

/// <summary>
/// The Laplacian Algorithm searches for zero crossings in the second derivative of the 
/// image to find edges.An edge has the one-dimensional shape of a ramp and calculating the derivative of the image can
/// highlight its location.
/// </summary>
/// 

 
namespace Laplacianedgedetection
{
    /// <summary>
    /// The Algorithm has been implemented in this class. This is the main class where convolution filter has been applied.
    /// </summary>
    public class Lap : IPipelineModule<double[,,],double[,,]>
    {
        /// <summary>
        /// In this Laplacian algortihm is 5x5 kernel is used for better approximation. 
        /// </summary>
        public static double[,] Laplacian
        {
            get
            {

                return new double[,]
               { //a 5X5 Filter
                  { -1, -1, -1, -1, -1, },
                  { -1, -1, -1, -1, -1, },
                  { -1, -1, 24, -1, -1, },
                  { -1, -1, -1, -1, -1, },
                  { -1, -1, -1, -1, -1  }, 
               };

                //a 3X3 Filter
                /* { -1, -1, -1},
                 * { -1,  8, -1},
                 * { -1, -1, -1},  
                 * };*/
                

            }
        }


        /// <summary>
        /// Implementation of Ipipeline module in double 3 dimension
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        

        public double[,,] Run(double[,,] data, IContext ctx)
        {
            return ConvolutionFilter(data, Laplacian);

        }


        /// <summary>
        /// Taking input as double data and applying necessary convolution 5x5 and returning back the double data
        /// </summary>
        /// <param name="sourceBitmap"></param>
        /// <param name="filterMatrix"></param>
        /// <param name="factor"></param>
        /// <param name="bias"></param>
        /// <param name="grayscale"></param>
        /// <returns></returns>
        private  double[,,] ConvolutionFilter(double[,,] sourceBitmap,
                                             double[,] filterMatrix,
                                                  double factor = 1,
                                                       int bias = 0,
                                             bool grayscale = false)
        {

            Bitmap bitmap = new Bitmap(sourceBitmap.GetLength(0), sourceBitmap.GetLength(1));

            for (int i = 0; i < sourceBitmap.GetLength(0); i++)
            {
                for (int j = 0; j < sourceBitmap.GetLength(1); j++)
                {
                    int Red = (int)sourceBitmap[i, j, 0];
                    int Green = (int)sourceBitmap[i, j, 1];
                    int Blue = (int)sourceBitmap[i, j, 2];
                    bitmap.SetPixel(i, j, Color.FromArgb(255, Red, Green, Blue));
                }
            }
            BitmapData sourceData = bitmap.LockBits(new Rectangle(0, 0,
                                     bitmap.Width, bitmap.Height),
                                                       ImageLockMode.ReadOnly,
                                                 PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            bitmap.UnlockBits(sourceData);
            // Converting an image into gray scale
            if (grayscale == true)
            {
                float rgb = 0;

                for (int p = 0; p < pixelBuffer.Length; p += 4)
                {
                    rgb = pixelBuffer[p] * 0.11f;
                    rgb += pixelBuffer[p + 1] * 0.59f;
                    rgb += pixelBuffer[p + 2] * 0.3f;


                    pixelBuffer[p] = (byte)rgb;
                    pixelBuffer[p + 1] = pixelBuffer[p];
                    pixelBuffer[p + 2] = pixelBuffer[p];
                    pixelBuffer[p + 3] = 255;
                }
            }

            // Initialization of variables RGB
            double bl = 0.0; //Color BLUE
            double gr = 0.0;// color GREEN
            double rd = 0.0;// color RED

            int filterWidth = filterMatrix.GetLength(1);
            int filterHeight = filterMatrix.GetLength(0);

            int filterOffset = (filterWidth - 1) / 2;
            int calcOffset = 0;

            int byteOffset = 0;

            for (int offsetY = filterOffset; offsetY <
                bitmap.Height - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX <
                    bitmap.Width - filterOffset; offsetX++)
                {
                    bl = 0; //blue
                    gr = 0;
                    rd = 0;

                    byteOffset = offsetY *
                                 sourceData.Stride +
                                 offsetX * 4;

                    for (int filterY = -filterOffset;
                        filterY <= filterOffset; filterY++)
                    {
                        for (int filterX = -filterOffset;
                            filterX <= filterOffset; filterX++)
                        {

                            calcOffset = byteOffset +
                                         (filterX * 4) +
                                         (filterY * sourceData.Stride);

                            bl += (double)(pixelBuffer[calcOffset]) *
                                    filterMatrix[filterY + filterOffset,
                                                        filterX + filterOffset];

                            gr += (double)(pixelBuffer[calcOffset + 1]) *
                                     filterMatrix[filterY + filterOffset,
                                                        filterX + filterOffset];

                            rd += (double)(pixelBuffer[calcOffset + 2]) *
                                   filterMatrix[filterY + filterOffset,
                                                      filterX + filterOffset];
                        }
                    }

                    bl = factor * bl + bias;
                    gr = factor * gr + bias;
                    rd = factor * rd + bias;

                    if (bl > 255)
                    { bl = 255; }
                    else if (bl < 0)
                    { bl = 0; }

                    if (gr> 255)
                    { gr = 255; }
                    else if (gr < 0)
                    { gr = 0; }

                    if (rd > 255)
                    { rd = 255; }
                    else if (rd < 0)
                    { rd = 0; }

                    resultBuffer[byteOffset] = (byte)(bl);
                    resultBuffer[byteOffset + 1] = (byte)(gr);
                    resultBuffer[byteOffset + 2] = (byte)(rd);
                    resultBuffer[byteOffset + 3] = 255;
                }
            }

            // creating a new bitmap assigning new width and height after the kernel is applied
            Bitmap resultBitmap = new Bitmap(bitmap.Width, bitmap.Height);

            // Lock the bitmap's bits
            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                                     resultBitmap.Width, resultBitmap.Height),
                                                      ImageLockMode.WriteOnly,
                                                 PixelFormat.Format32bppArgb);
            // Unlock the bits.
            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            // convert the resulted image into double 
            double[,,] resultImageDouble = new double[resultBitmap.Width, resultBitmap.Height, 3];
            for (int w = 0; w < resultBitmap.Width; w++)
            {
                for (int h = 0; h < resultBitmap.Height; h++)
                {
                    Color color = resultBitmap.GetPixel(w, h);
                    resultImageDouble[w, h, 0] = color.R;
                    resultImageDouble[w, h, 1] = color.G;
                    resultImageDouble[w, h, 2] = color.B;
                }

            }

            // return processed image
            return resultImageDouble;

        }
    }
}


        
        
        