namespace IndependentWork1;

public class Rectangle
{
    private double _width;
    private double _height;

    public double Width
    {
        get { return _width; }
        set { _width = value; }
    }

    public double Height
    {
        get { return _height; }
        set { _height = value; }
    }

    // read-only, обчислюється з полів
    public double Area
    {
        get { return _width * _height; }
    }

    public Rectangle(double width, double height)
    {
        _width = width;
        _height = height;
    }

    public double GetPerimeter()
    {
        return 2 * (_width + _height);
    }

    public bool IsSquare()
    {
        return Math.Abs(_width - _height) < 0.000001;
    }

    public void Scale(double factor)
    {
        _width *= factor;
        _height *= factor;
    }
}