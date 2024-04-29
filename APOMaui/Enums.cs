﻿using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace APOMaui
{
    public enum ImgType
    {
        RGB,
        HSV,
        Gray,
        Lab,
    }
    public enum BuiltInFilters
    {
        SobelX,
        SobelY,
        LaplacianEdge,
        Canny,
        Blur,
        GaussianBlur,
        Median
    }
    public enum TwoArgsOps
    {
        ADD,
        SUBTRACT,
        BLEND,
        AND,
        OR,
        NOT,
        XOR
    }
}