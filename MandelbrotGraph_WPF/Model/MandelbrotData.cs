using System.Drawing;

namespace MandelbrotGraph_WPF.Model
{
    class MandelbrotData
    {
        public double Precision { get; set; }
        public double Iterations { get; set; }
        public MandelCoOrds CoOrds { get; set; }
        public Size OutputSize { get; set; }

        public MandelbrotData()
        {
            // Set some basic values
            Precision = 4.0;
            Iterations = 256;
            CoOrds = new MandelCoOrds(2, -2, 2, -2);
            OutputSize = new Size(500, 500);
        }
    }
}
