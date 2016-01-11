public struct MandelCoOrds
{
    private readonly double xMax;
    private readonly double xMin;
    private readonly double yMax;
    private readonly double yMin;

    public double XMax { get { return xMax; } }
    public double XMin { get { return xMin; } }
    public double YMax { get { return yMax; } }
    public double YMin { get { return yMin; } }

    public MandelCoOrds(double xMax, double xMin, double yMax, double yMin)
    {
        this.xMax = xMax;
        this.xMin = xMin;
        this.yMax = yMax;
        this.yMin = yMin;
    }
}