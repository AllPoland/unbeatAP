namespace UNBEATAP.Traps;

public class ScrollSpeed
{
    private static bool zoom;
    private static bool crawl;

    public static bool GetZoom()
    {
        return zoom;
    }

    public static bool GetCrawl()
    {
        return crawl;
    }

    public static void SetZoom(bool zoomstate)
    {
        if(!crawl)
        {
            zoom = zoomstate;
        }
    }

    public static void SetCrawl(bool crawlstate)
    {
        if(!zoom)
        {
            crawl = crawlstate;
        }
    }
}