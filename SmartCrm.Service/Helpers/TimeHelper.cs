namespace SmartCrm.Service.Helpers;

public class TimeHelper
{
    public static DateTime GetDateTime()
    {
        var time = DateTime.UtcNow;
        //time = time.AddHours(5);

        return time;
    }
    public static DateTime ToUzbekistanTime(DateTime utcTime)
    {
        if (utcTime.Kind != DateTimeKind.Utc)
        {
            utcTime = DateTime.SpecifyKind(utcTime, DateTimeKind.Utc);
        }
        return utcTime.AddHours(5);
    }
}
