namespace UNBEATAP.Traps;

public class ScrollSpeed
{
    public static TrapTimer ZoomTimer = new TrapTimer();
    public static TrapTimer CrawlTimer = new TrapTimer();


    public static bool GetZoom()
    {
        return ZoomTimer.GetActive();
    }


    public static bool GetCrawl()
    {
        return CrawlTimer.GetActive();
    }


    public static void ActivateZoom()
    {
        if(CrawlTimer.GetActive())
        {
            CrawlTimer.Deactivate();
        }

        ZoomTimer.Activate();
    }


    public static void ActivateCrawl()
    {
        if(ZoomTimer.GetActive())
        {
            ZoomTimer.Deactivate();
        }

        CrawlTimer.Activate();
    }


    public static void Deactivate()
    {
        ZoomTimer.Deactivate();
        CrawlTimer.Deactivate();
    }


    public static void Update(float deltaTime)
    {
        ZoomTimer.Update(deltaTime);
        CrawlTimer.Update(deltaTime);
    }
}