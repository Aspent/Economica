using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Economica
{
    public partial class Form1 : Form
    {
        readonly Operation _frez = new Operation(28, 1.4f);
        readonly Operation _sverl = new Operation(22, 2.9f);
        readonly Operation _rast = new Operation(35, 1.0f);
        readonly Operation _shlif = new Operation(35, 1.5f);
        readonly Operation _tok = new Operation(22, 2.9f);
        Material _steel = new Material(35, 7800, new Size(0.035, 0.040, 0.020));
        private double _usageCoeff = 0.6;



        private int _repairCost = 180000;
        private int _gasCost = 450000;
        private int _addMaterial = 90000;
        private int _lightCost = 98000;
        private int _adminSalary = 580000;
        private int _safeCost = 36000;
        private readonly int _workDaysCount = 250;
        private readonly int _shifts = 2;
        private readonly int _shiftDuration = 8;
        private readonly double _machineDowntimePercent = 6.0;
        private double _workDowntimePercent = 15;
        private readonly double _normCompletePercent = 103;
        private int _deliveryInterval = 35;
        private int _maxDeliveryInterval = 40;
        private int _cycleDuration = 4;
        private int _shipmentFreq = 2;
        private double _wastePrice = 1500;
        private double _specificWeight = 45;
        private readonly double _squarePrice = 9400;
        private double _addSalaryPercent = 15;
        private double _profitPercent = 30;
        private readonly double _machineDepreciation = 20;
        private readonly double _shopDepreciation = 2.5;           
        readonly Machine _frezMachine = new Machine(239800, 3.7f, 8);
        readonly Machine _sverlMachine = new Machine(186500, 1.0f, 4);
        readonly Machine _rastMachine = new Machine(166950, 16.3f, 34);
        readonly Machine _shlifMachine = new Machine(151000, 4.8f, 10);
        readonly Machine _tokMachine = new Machine(181500, 7.6f, 16);    
        private readonly int _release = 160000;
        private readonly Dictionary<Machine, int> _machinesCount = new Dictionary<Machine, int>();
        private readonly List<Machine> _machines = new List<Machine>();
        private readonly List<Operation> _operations = new List<Operation>();
        readonly Dictionary<Operation, double> _trudoemkost = new Dictionary<Operation, double>();
        readonly Dictionary<Machine, double> _machinesCost = new Dictionary<Machine, double>();
        readonly Dictionary<Machine, double> _squaresDictionary = new Dictionary<Machine, double>();
        private double _totalMachinesCost;
        private double _totalBuildingCost;
        private readonly Dictionary<Machine, double> _depreciationSum = new Dictionary<Machine, double>();
        private double _buildingDepreciation;
        private readonly Dictionary<Machine, double> _powers = new Dictionary<Machine, double>();
        private double _effectiveTimeFond;
        private readonly Dictionary<Machine, double> _loadCoeffs = new Dictionary<Machine, double>();
        private double _effectiveTimeFondWorker;
        private Dictionary<Operation, int> _workersCount = new Dictionary<Operation, int>();
        private Dictionary<Operation, double> _salaryFonds = new Dictionary<Operation, double>();
        private double _totalSalaryFond;
        private double _materialCost;
        private double _wasteCost;
        private double _baseSalary;
        private double _ecspCost;
        private double _shopCost;
        private double _currentStock;
        private double _strStock;
        private double _normStock;
        private double _normUnfinished;
        private double _normFinished;
        private double _totalBaseFondCost;
        private double _primeCost;
    



        public void GetMachinesCount()
        {
            _effectiveTimeFond = _workDaysCount * _shifts * _shiftDuration 
                * (1 - _machineDowntimePercent / 100);
            foreach (var t in _operations)
            {
                var count = Math.Ceiling(_release/_effectiveTimeFond*t.RateTime/60);
                _machinesCount[_machines[_operations.IndexOf(t)]] = (int)count;
                var trud = 1.0*_release*t.RateTime/60;
                _trudoemkost[t] = Math.Ceiling(trud);
            }
        }

        void GetMachinesCost()
        {
            foreach (var t in _machines)
            {
                _machinesCost[t] = t.Price*_machinesCount[t];
            }
        }

        void GetTotalSquare()
        {
            foreach (var t in _machines)
            {
                _squaresDictionary[t] = _machinesCount[t]*(t.BaseSquare + t.AddSquare);
            }
        }

        void GetTotalCost()
        {
            _totalMachinesCost = _machinesCost.Sum(x => x.Value);
            _totalBuildingCost = _squaresDictionary.Sum(x => x.Value)*_squarePrice;
            _totalBaseFondCost = _totalBuildingCost + _totalMachinesCost * (1.18);
        }

        void GetDepreciationSum()
        {
            foreach (var t in _machines)
            {
                _depreciationSum[t] = _machinesCost[t]*1.0*_machineDepreciation/100;
            }
            _buildingDepreciation = _totalBuildingCost*_shopDepreciation/100;
        }

        void GetPowers()
        {
            foreach (var t in _machines)
            {
                _powers[t] = _effectiveTimeFond*_machinesCount[t]*_normCompletePercent/100*60
                             /_operations[_machines.IndexOf(t)].RateTime;
                _loadCoeffs[t] = _release/_powers[t];
            }
        }

        void GetWorkersCount()
        {
            _effectiveTimeFondWorker = _workDaysCount*_shiftDuration*(1 - _workDowntimePercent/100);
            foreach (var t in _operations)
            {
                _workersCount[t] = (int)Math.Ceiling(t.RateTime/60*_release/_effectiveTimeFondWorker/
                    (_normCompletePercent/100));
            }
        }

        void GetSalaryFond()
        {
            foreach (var t in _operations)
            {
                _salaryFonds[t] = t.RateTime/60*t.WageRate*_release;
            }
            _totalSalaryFond = _salaryFonds.Sum(x => x.Value);
        }

        void GetPrimeCost()
        {
            _materialCost = _steel.Size.Volume*_steel.Density/_usageCoeff*_steel.Price;
            _wasteCost = _steel.Size.Volume*_steel.Density/_usageCoeff*(1 - _usageCoeff)
                *(_wastePrice/1000);
            _baseSalary = _operations.Sum(x => x.RateTime/60*x.WageRate);
            _ecspCost = (_totalMachinesCost*_machineDepreciation/100 + _repairCost +
                         _gasCost + _addMaterial)/_totalSalaryFond*_baseSalary;
            _shopCost = (_totalBuildingCost*_shopDepreciation/100 + _lightCost + 1.3*(_adminSalary*1.15) + _safeCost)/
                        _totalSalaryFond*_baseSalary;

            _primeCost = _materialCost * (1.08) + _wasteCost + 1.3 * (1.15 * _baseSalary) + _ecspCost + _shopCost;




        }

        void GetStock()
        {
            var dayUsage = _steel.Size.Volume*_steel.Density*_release/360;
            
            _currentStock = dayUsage*_deliveryInterval*_steel.Price/2;
            

            _strStock = dayUsage*(_maxDeliveryInterval - _deliveryInterval)*_steel.Price;
            

            _normStock = _currentStock + _strStock;

            var coeff = (_specificWeight/100) + (1 - (_specificWeight/100))/2;
            var cost = _steel.Size.Volume*_steel.Density*_steel.Price;
            var costDay = cost*_release/(_specificWeight/100)/360;
            _normUnfinished = costDay*coeff*_cycleDuration;
          //  MessageBox.Show(costDay.ToString());

            var selfCost = cost/(_specificWeight/100);
            _normFinished = selfCost*_release/_workDaysCount*_shipmentFreq;


        }

        public Form1()
        {
            InitializeComponent();

            _machines.Add(_frezMachine);
            _machines.Add(_sverlMachine);
            _machines.Add(_rastMachine);
            _machines.Add(_shlifMachine);
            _machines.Add(_tokMachine);

            _operations.Add(_frez);
            _operations.Add(_sverl);
            _operations.Add(_rast);
            _operations.Add(_shlif);
            _operations.Add(_tok);

            GetMachinesCount();
            GetMachinesCost();
            GetTotalSquare();
            GetTotalCost();
            GetDepreciationSum();
            GetPowers();
            GetWorkersCount();
            GetSalaryFond();
            GetPrimeCost();
            GetStock();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            dataGridView1.Columns.Add("1", "№№ пп");
            dataGridView1.Columns.Add("2", "Наименование операций");
            dataGridView1.Columns.Add("3", "Норма времени на операцию");
            dataGridView1.Columns.Add("4", "Годовой объем производства");
            dataGridView1.Columns.Add("5", "Трудоемкость годового объема производства");
            dataGridView1.Columns.Add("6", "Количество единиц для оборудования");
            int index = 0;
            foreach (var t in _operations)
            {
                dataGridView1.Rows.Add();
                dataGridView1[0, index].Value = index + 1;
                dataGridView1[1, index].Value = "name";
                dataGridView1[2, index].Value = t.RateTime;
                dataGridView1[3, index].Value = _release;
                dataGridView1[4, index].Value = _trudoemkost[t];
                dataGridView1[5, index].Value = _machinesCount[_machines[index]];
                index++;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.Columns.Add("1", "№№ пп");
            dataGridView1.Columns.Add("2", "Виды оборудования");
            dataGridView1.Columns.Add("3", "Цена за единицу оборудования данного вида");
            dataGridView1.Columns.Add("4", "Количество единиц оборудования");
            dataGridView1.Columns.Add("5", "Общая стоимость оборудования");
            int index = 0;
            foreach (var t in _machines)
            {
                dataGridView1.Rows.Add();
                dataGridView1[0, index].Value = index + 1;
                dataGridView1[1, index].Value = "name";
                dataGridView1[2, index].Value = t.Price;
                dataGridView1[3, index].Value = _machinesCount[t];
                dataGridView1[4, index].Value = _machinesCost[t];
                index++;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dataGridView1.Columns.Add("1", "№№ пп");
            dataGridView1.Columns.Add("2", "Виды оборудования");
            dataGridView1.Columns.Add("3", "Количество единиц оборудования");
            dataGridView1.Columns.Add("4", "Основная площадь");
            dataGridView1.Columns.Add("5", "Дополнительная площадь");
            dataGridView1.Columns.Add("6", "Общая площадь");
            int index = 0;
            foreach (var t in _machines)
            {
                dataGridView1.Rows.Add();
                dataGridView1[0, index].Value = index + 1;
                dataGridView1[1, index].Value = "name";
                dataGridView1[2, index].Value = _machinesCount[t];
                dataGridView1[3, index].Value = t.BaseSquare;
                dataGridView1[4, index].Value = t.AddSquare;
                dataGridView1[5, index].Value = _squaresDictionary[t];
                index++;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dataGridView1.Columns.Add("1", "№№ пп");
            dataGridView1.Columns.Add("2", "Классификационные группы");
            dataGridView1.Columns.Add("3", "");
            dataGridView1.Columns.Add("4", "Стоимость, руб.");

            dataGridView1.Rows.Add();
            dataGridView1[0, 0].Value = 1;
            dataGridView1[1, 0].Value = "Здания";
            dataGridView1[3, 0].Value = String.Format("{0:0.00}", _totalBuildingCost);

            dataGridView1.Rows.Add();
            dataGridView1[0, 1].Value = 2;
            dataGridView1[1, 1].Value = "Рабочие машины";
            dataGridView1[3, 1].Value = _totalMachinesCost;

            var coeff = 0.1;
            dataGridView1.Rows.Add();
            dataGridView1[0, 2].Value = 3;
            dataGridView1[1, 2].Value = "Транспортные средства";
            dataGridView1[2, 2].Value = coeff*100 + "%";
            dataGridView1[3, 2].Value = _totalMachinesCost * coeff;

            coeff = 0.04;
            dataGridView1.Rows.Add();
            dataGridView1[0, 3].Value = 4;
            dataGridView1[1, 3].Value = "Производственный инвентарь";
            dataGridView1[2, 3].Value = coeff * 100 + "%";
            dataGridView1[3, 3].Value = _totalMachinesCost * coeff;

            coeff = 0.04;
            dataGridView1.Rows.Add();
            dataGridView1[0, 4].Value = 5;
            dataGridView1[1, 4].Value = "Инструменты";
            dataGridView1[2, 4].Value = coeff * 100 + "%";
            dataGridView1[3, 4].Value = _totalMachinesCost * coeff;

            //_totalBaseFondCost = _totalBuildingCost + _totalMachinesCost*(1.18);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dataGridView1.Columns.Add("1", "№№ пп");
            dataGridView1.Columns.Add("2", "Виды основных фондов");
            dataGridView1.Columns.Add("3", "Количество, шт");
            dataGridView1.Columns.Add("4", "Стоимость");
            dataGridView1.Columns.Add("5", "Норма амортизации");
            dataGridView1.Columns.Add("6", "Начисленная амортизация");
            var index = 0;
            foreach (var t in _machines)
            {
                dataGridView1.Rows.Add();
                dataGridView1[0, index].Value = index + 1;
                dataGridView1[1, index].Value = "name";
                dataGridView1[2, index].Value = _machinesCount[t];
                dataGridView1[3, index].Value = _machinesCost[t];
                dataGridView1[4, index].Value = _machineDepreciation;
                dataGridView1[5, index].Value = _depreciationSum[t];
                index++;
            }
            dataGridView1.Rows.Add();
            dataGridView1[0, index].Value = index + 1;
            dataGridView1[1, index].Value = "Здания";
            dataGridView1[3, index].Value = _totalBuildingCost;
            dataGridView1[4, index].Value = _shopDepreciation;
            dataGridView1[5, index].Value = _buildingDepreciation;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            dataGridView1.Columns.Add("1", "№№ пп");
            dataGridView1.Columns.Add("2", "Виды оборудования");
            dataGridView1.Columns.Add("3", "Количество единиц оборудования, шт.");
            dataGridView1.Columns.Add("4", "Мощность оборудования, шт.");
            dataGridView1.Columns.Add("5", "Коэффициент загрузки");
            var index = 0;
            foreach (var t in _machines)
            {
                dataGridView1.Rows.Add();
                dataGridView1[0, index].Value = index + 1;
                dataGridView1[1, index].Value = "name";
                dataGridView1[2, index].Value = _machinesCount[t];
                dataGridView1[3, index].Value = _powers[t];
                dataGridView1[4, index].Value = _loadCoeffs[t];
                index++;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            dataGridView1.Columns.Add("1", "№№ пп");
            dataGridView1.Columns.Add("2", "Наименование операции");
            dataGridView1.Columns.Add("3", "Разряд работы");
            dataGridView1.Columns.Add("4", "Норма времени на операцию");
            dataGridView1.Columns.Add("5", "Трудоемкость годового объема производства");
            dataGridView1.Columns.Add("6", "Численность рабочих сдельщиков");
            var index = 0;
            foreach (var t in _operations)
            {
                dataGridView1.Rows.Add();
                dataGridView1[0, index].Value = index + 1;
                dataGridView1[1, index].Value = "name";
                dataGridView1[2, index].Value = "razeyad";
                dataGridView1[3, index].Value = t.RateTime;
                dataGridView1[4, index].Value = _trudoemkost[t];
                dataGridView1[5, index].Value = _workersCount[t];
                index++;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            dataGridView1.Columns.Add("1", "№№ пп");
            dataGridView1.Columns.Add("2", "Наименование операции");
            dataGridView1.Columns.Add("3", "Норма времени на операцию");
            dataGridView1.Columns.Add("4", "Трудоемкость годового объема производства");
            dataGridView1.Columns.Add("5", "Часовая рабочая ставка");
            dataGridView1.Columns.Add("6", "Фонд основной зароботной платы");
            var index = 0;
            foreach (var t in _operations)
            {
                dataGridView1.Rows.Add();
                dataGridView1[0, index].Value = index + 1;
                dataGridView1[1, index].Value = "name";
                dataGridView1[2, index].Value = t.RateTime;
                dataGridView1[3, index].Value = _trudoemkost[t];
                dataGridView1[4, index].Value = t.WageRate;
                dataGridView1[5, index].Value = _salaryFonds[t];
                index++;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            dataGridView1.Columns.Add("1", "№№ пп");
            dataGridView1.Columns.Add("2", "Наименование статей затрат");
            dataGridView1.Columns.Add("3", "Сумма, руб.");

            dataGridView1.Rows.Add();
            dataGridView1[0, 0].Value = 1;
            dataGridView1[1, 0].Value = "Основные материалы";
            dataGridView1[2, 0].Value = _materialCost;

            dataGridView1.Rows.Add();
            dataGridView1[0, 1].Value = 2;
            dataGridView1[1, 1].Value = "Покупные полуфабоикаты и комплектующие изделия";
            dataGridView1[2, 1].Value = 0;

            dataGridView1.Rows.Add();
            dataGridView1[0, 2].Value = 3;
            dataGridView1[1, 2].Value = "Транспортные расходы";
            dataGridView1[2, 2].Value = _materialCost*0.08;

            dataGridView1.Rows.Add();
            dataGridView1[0, 3].Value = 4;
            dataGridView1[1, 3].Value = "Полуфабрикаты собственного производства";
            dataGridView1[2, 3].Value = 0;

            dataGridView1.Rows.Add();
            dataGridView1[0, 4].Value = 5;
            dataGridView1[1, 4].Value = "Отходы возвратные";
            dataGridView1[2, 4].Value = _wasteCost;

            dataGridView1.Rows.Add();
            dataGridView1[0, 5].Value = 6;
            dataGridView1[1, 5].Value = "Основная заработная плата производственных рабочих";
            dataGridView1[2, 5].Value = _baseSalary;

            var addSalary = _baseSalary*_addSalaryPercent/100;
            dataGridView1.Rows.Add();
            dataGridView1[0, 6].Value = 7;
            dataGridView1[1, 6].Value = "Дополнительная заработная плата производственных рабочих";
            dataGridView1[2, 6].Value = addSalary;

            dataGridView1.Rows.Add();
            dataGridView1[0, 7].Value = 8;
            dataGridView1[1, 7].Value = "Начисления на заработную плату";
            dataGridView1[2, 7].Value = (_baseSalary + addSalary)*_profitPercent/100;

            dataGridView1.Rows.Add();
            dataGridView1[0, 8].Value = 9;
            dataGridView1[1, 8].Value = "Расходы по содержанию и эксплуатации оборудования";
            dataGridView1[2, 8].Value = _ecspCost;

            dataGridView1.Rows.Add();
            dataGridView1[0, 9].Value = 10;
            dataGridView1[1, 9].Value = "Цеховые накладные расходы";
            dataGridView1[2, 9].Value = _shopCost;

            _primeCost = _materialCost*(1.08) + _wasteCost + 1.3*(1.15*_baseSalary) + _ecspCost + _shopCost;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            dataGridView1.Columns.Add("1", "№№ пп");
            dataGridView1.Columns.Add("2", "Показатели");
            dataGridView1.Columns.Add("3", "Ед. изм.");
            dataGridView1.Columns.Add("4", "Значение");

            dataGridView1.Rows.Add();
            dataGridView1[0, 0].Value = 1;
            dataGridView1[1, 0].Value = "Количество единиц оборудования цеха";
            dataGridView1[2, 0].Value = "шт.";
            dataGridView1[3, 0].Value = _machinesCount.Sum(x => x.Value);

            dataGridView1.Rows.Add();
            dataGridView1[0, 1].Value = 2;
            dataGridView1[1, 1].Value = "Общая стоимость оборудования цеха";
            dataGridView1[2, 1].Value = "руб.";
            dataGridView1[3, 1].Value = _totalMachinesCost;

            dataGridView1.Rows.Add();
            dataGridView1[0, 2].Value = 3;
            dataGridView1[1, 2].Value = "Стоимость производственного помещения цеха";
            dataGridView1[2, 2].Value = "руб.";
            dataGridView1[3, 2].Value = _totalBuildingCost;

            dataGridView1.Rows.Add();
            dataGridView1[0, 3].Value = 4;
            dataGridView1[1, 3].Value = "Общая стоимость основных производственных фондов";
            dataGridView1[2, 3].Value = "руб.";
            dataGridView1[3, 3].Value = _totalBaseFondCost;

            dataGridView1.Rows.Add();
            dataGridView1[0, 4].Value = 5;
            dataGridView1[1, 4].Value = "Производственная мощность цеха";
            dataGridView1[2, 4].Value = "шт.";
            dataGridView1[3, 4].Value = _powers.Max(x => x.Value);

            dataGridView1.Rows.Add();
            dataGridView1[0, 5].Value = 6;
            dataGridView1[1, 5].Value = "Узкое место";
            dataGridView1[3, 5].Value = "Отсутствует";

            dataGridView1.Rows.Add();
            dataGridView1[0, 6].Value = 7;
            dataGridView1[1, 6].Value = "Норматив производственного запаса";
            dataGridView1[2, 6].Value = "руб.";
            dataGridView1[3, 6].Value = _normStock;

            dataGridView1.Rows.Add();
            dataGridView1[0, 7].Value = 8;
            dataGridView1[1, 7].Value = "Норматив незавершенного производства";
            dataGridView1[2, 7].Value = "руб.";
            dataGridView1[3, 7].Value = _normUnfinished;

            dataGridView1.Rows.Add();
            dataGridView1[0, 8].Value = 9;
            dataGridView1[1, 8].Value = "Норматив готовой продукции";
            dataGridView1[2, 8].Value = "руб.";
            dataGridView1[3, 8].Value = _normFinished;

            dataGridView1.Rows.Add();
            dataGridView1[0, 9].Value = 10;
            dataGridView1[1, 9].Value = "Общая стоимость оборотных средств цеха";
            dataGridView1[2, 9].Value = "руб.";
            dataGridView1[3, 9].Value = _normStock + _normFinished + _normUnfinished;

            dataGridView1.Rows.Add();
            dataGridView1[0, 10].Value = 11;
            dataGridView1[1, 10].Value = "Численность рабочих-сдельщиков";
            dataGridView1[2, 10].Value = "чел.";
            dataGridView1[3, 10].Value = _workersCount.Sum(x => x.Value);

            dataGridView1.Rows.Add();
            dataGridView1[0, 11].Value = 12;
            dataGridView1[1, 11].Value = "Фонд заработной платы";
            dataGridView1[2, 11].Value = "руб.";
            dataGridView1[3, 11].Value = _salaryFonds.Sum(x => x.Value);

            dataGridView1.Rows.Add();
            dataGridView1[0, 12].Value = 13;
            dataGridView1[1, 12].Value = "Цеховая себестоимость детали";
            dataGridView1[2, 12].Value = "руб.";
            dataGridView1[3, 12].Value = _primeCost;
        }
    }
    
}
