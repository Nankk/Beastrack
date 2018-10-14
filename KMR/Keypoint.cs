using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMR
{
    public class Keypoint
    {
        public Keypoint()
        {

        }
        public Keypoint(double x, double y, double reliability)
        {
            X = x;
            Y = y;
            Reliability = reliability;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Reliability { get; set; }
    }
}
