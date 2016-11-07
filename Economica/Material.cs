using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Economica
{
    class Material
    {
        private double _price;
        private double _density;
        private Size _size;

        public Material(double price, double density, Size size)
        {
            _price = price;
            _density = density;
            _size = size;
        }

        public double Price
        {
            get { return _price; }
        }

        public double Density
        {
            get { return _density; }
        }

        public Size Size
        {
            get { return _size; }
        }
    }
}
