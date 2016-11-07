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

        public Operation(float wageRate, float rateTime)
        {
            _wageRate = wageRate;
            _rateTime = rateTime;
        }

        public float WageRate
        {
            get { return _wageRate; }
        }

        public float RateTime
        {
            get { return _rateTime; }
        }
    }
}
