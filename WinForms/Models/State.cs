namespace nardnob.InputTracker.WinForms.Models
{
    internal class State
    {
        public Dictionary<Point, int> ClickedPoints = [];

        public ulong ClickCount { get; set; }
        public ulong KeyCount { get; set; }

        public bool IsLoading { get; set; }
        public bool IsHidden { get; set; }
    }
}
