using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Economica
{
    class Operation
    {
        private float _wageRate;
        private float _rateTime;
        private string _title;

        public Operation(float wageRate, float rateTime, string title)
        {
            _wageRate = wageRate;
            _rateTime = rateTime;
            _title = title;
        }

        public float WageRate
        {
            get { return _wageRate; }
        }

        public float RateTime
        {
            get { return _rateTime; }
        }

        public string Title
        {
            get { return _title; }
        }
    }
}
