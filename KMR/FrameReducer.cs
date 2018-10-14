using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMR
{
    class FrameReducer
    {
        public FrameReducer(double cutOffThreshold = 0.3)
        {
            _cutOffThreshold = cutOffThreshold;
        }

        public List<List<Keypoint>> Reduce(List<List<Keypoint>> denseFrames)
        {
            throw new NotImplementedException("FrameReducer.Reduce");
        }

        double _cutOffThreshold;
    }
}
