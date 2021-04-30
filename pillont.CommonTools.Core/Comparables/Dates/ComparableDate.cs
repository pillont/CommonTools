using System;

namespace pillont.CommonTools.Core.Comparables.Dates
{
    public class ComparableDate
    {
        public bool AllowNull { get; set; } = false;
        public ComparatorType ComparatorType { get; set; }

        public DateTime Date { get; set; }
        public DateType DateType { get; set; }
    }
}
