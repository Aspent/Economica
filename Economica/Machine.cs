namespace Economica
{
    class Machine
    {
        private int _price;
        private float _baseSquare;
        private float _addSquare;
        private string _title;

        public Machine(int price, float baseSquare, float addSquare, string title)
        {
            _price = price;
            _baseSquare = baseSquare;
            _addSquare = addSquare;
            _title = title;
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

        public string Title
        {
            get { return _title; }
        }
    }
}
