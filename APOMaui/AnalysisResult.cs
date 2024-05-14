using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APOMaui
{
    public class AnalysisResult
    {
        public int no {  get; set; }
        public int moments { get; set; }
        public double area { get; set; }
        public double perimeter { get; set; }
        public double aspectRatio { get; set; }
        public double extent { get; set; }
        public double solidity { get; set; }
        public double equivalentDiameter { get; set; }
        public AnalysisResult(int no, int moments, double area, double perimeter, double aspectRatio, double extent, double solidity, double equivalentDiameter)
        {
            this.no = no;
            this.moments = moments;
            this.area = area;
            this.perimeter = perimeter;
            this.aspectRatio = aspectRatio;
            this.extent = extent;
            this.solidity = solidity;
            this.equivalentDiameter = equivalentDiameter;
        }
    }
}
