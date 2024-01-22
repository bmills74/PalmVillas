namespace Palm.Static
{
    public static class DateFunctions
    {
        public static(DateTime, DateTime, bool) ConvertRangeString(string date)
        {
            var firstIndex = date.IndexOf("to") - 1;
            var startString = date.Substring(0, firstIndex);
            var endString = date.Substring(firstIndex + 4);

            var success = DateTime.TryParse(startString, out var startDate);
            success = DateTime.TryParse(endString, out var endDate);

            return (startDate,endDate,success);

        }
    }
}
