namespace Manmudra.Contract.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime? ConvertToUtc(this DateTime? date)
        {
            if (date == null) return null;
            return ConvertToUtc(date.Value);
        }

        public static DateTime ConvertToUtc(this DateTime date)
        {
            return date.Kind == DateTimeKind.Unspecified ?
                        DateTime.SpecifyKind(date, DateTimeKind.Utc) :
                        date;
        }
    }
}
