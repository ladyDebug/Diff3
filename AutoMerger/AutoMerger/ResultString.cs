using System.Collections.Generic;

namespace AutoMerger
{
    public class ResultString
    {
        public List<string> Line { get; set; }

        public bool IsConflicted { get; set; } = false;

        public List<string> BaseLine { get; set; }
        public List<string> LeftLine { get; set; }
        public List<string> RightLine { get; set; }

    }
}
