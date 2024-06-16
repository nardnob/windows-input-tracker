using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nardnob.InputTracker.WinForms
{
    internal class State
    {
        public Dictionary<int, Dictionary<int, int>> ClickedPoints = new Dictionary<int, Dictionary<int, int>>();

        public UInt64 ClickCount { get; set; }
        public UInt64 KeyCount { get; set; }

        public bool IsLoading { get; set; }
        public bool IsHidden { get; set; }
    }
}
