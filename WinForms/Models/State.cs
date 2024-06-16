namespace nardnob.InputTracker.WinForms.Models
{
    internal class State
    {
        public Dictionary<int, Dictionary<int, int>> ClickedPoints = new Dictionary<int, Dictionary<int, int>>();

        public ulong ClickCount { get; set; }
        public ulong KeyCount { get; set; }

        public bool IsLoading { get; set; }
        public bool IsHidden { get; set; }
    }
}
