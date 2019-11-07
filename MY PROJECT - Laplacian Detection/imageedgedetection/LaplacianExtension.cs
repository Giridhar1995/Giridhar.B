using Laplacianedgedetection;
using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAlgorithm
{
    public static class PipelineExtensions
    {
        public static LearningApi Useimageedge(this LearningApi a)
        {
            Lap alg = new Lap();
            a.AddModule(alg, "Useimageedge");
            return a;
        }
    }
}