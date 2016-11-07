namespace Economica
{
    class Machine
    {
        private int _price;
        private float _baseSquare;
        private float _addSquare;

        public Machine(int price, float baseSquare, float addSquare)
        {
            _price = price;
            _baseSquare = baseSquare;
            _addSquare = addSquare;
        }

        public int Price
        {
            get { return _price; }
        }

        public float BaseSquare
        {
            get { return _baseSquare; }
        }

        public float AddSquare
        {
            get { return _addSquare; }
        }
    }
}
