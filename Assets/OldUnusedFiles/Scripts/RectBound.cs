public class RectBound
{
    public float xMin;
    public float xMax;
    public float yMin;
    public float yMax;
    public RectBound(float xMin, float xMax, float yMin, float yMax)
    {
        this.xMin = xMin;
        this.xMax = xMax;
        this.yMin = yMin;
        this.yMax = yMax;
    }
    public bool Contains(float x, float y)
    {
        if (x > xMin && x < xMax && y > yMin && y < yMax)
            return true;
        return false;
    }
}