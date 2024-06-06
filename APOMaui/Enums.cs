using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace APOMaui
{
    public enum ImgType
    {
        RGB,
        Gray,
    }
    public enum BuiltInFilters
    {
        Canny,
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
    public enum MorphOpExtend
    {
        SKELETONIZE,
        WATERSHED,
    }

    public enum ThreshType
    {
        MANUAL,
        ADAPTIVE,
        OTSU
    }
    public enum PyramidType
    {
        UP,
        DOWN
    }
}
