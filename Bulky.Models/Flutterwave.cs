public class FlutterwaveInitResponse
{
    public string status { get; set; }
    public string message { get; set; }
    public FlutterwaveInitData data { get; set; }
}

public class FlutterwaveInitData
{
    public string link { get; set; }
}