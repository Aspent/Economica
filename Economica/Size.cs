namespace Economica
{
    class Size
    {
        private double _length;
        private double _width;
        private double _height;

        public Size(double length, double width, double height)
        {
            _length = length;
            _width = width;
            _height = height;
        }

        public double Length
        {
            get { return _length; }
        }

        public double Width
        {
            get { return _width; }
        }

        public double Height
        {
            get { return _height; }
        }

        public double Volume
        {
            get { return _length*_width*_height; }
        }
    }
}
