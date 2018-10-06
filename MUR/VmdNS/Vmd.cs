using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MUR.VmdNS
{
    public class Vmd
    {
        public Vmd()
        {
            Frames = new List<Frame>();
        }

        public string Header { get; set; }
        public string ModelName { get; set; }
        public int FramesTotal { get; set; }
        public List<Frame> Frames { get; set; }
    }
}
