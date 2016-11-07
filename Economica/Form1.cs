using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Economica
{
    public partial class Form1 : Form
    {
        private int _workDaysCount = 260;
        private int _shifts = 2;
        private int _shiftDuration = 8;
        private double _machineDowntimePercent = 6.0;
        private double _workDowntimePercent = 15;
        private double _normCompletePercent = 103;
        private int _deliveryInterval = 35;
        private int _cycleDuration = 4;
        private int _shipmentFreq = 2;
        private double _wastePrice = 1500;
        private double _specificWeight = 45;
        private double _squarePrice = 9400;
        private double _addSalaryPercent = 15;
        private double _profitPercent = 30;
        private double _machineDepreciation = 20;
        private double _shopDepreciation = 2.5;
        private double _usageCoeff = 0.9;
        Material _steel = new Material(35, 7800, new Size(35, 40, 20));
        Machine _frezMachine = new Machine(239800, 3.7f, 8);
        Machine _sverlMachine = new Machine(186500, 1.0f, 4);
        Machine _rastMachine = new Machine(166950, 16.3f, 34);
        Machine _shlifMachine = new Machine(151000, 4.8f, 10);
        Machine _tokMachine = new Machine(181500, 7.6f, 16);
        Operation _frez = new Operation(28, 1.5f);
        Operation _sverl = new Operation(22, 1.7f);
        Operation _rast = new Operation(35, 2.6f);
        Operation _shlif = new Operation(35, 1.7f);
        Operation _tok = new Operation(22, 1.9f);
        private int _release = 160000;
        private Dictionary<Machine, int> _machinesCount = new Dictionary<Machine, int>();
        private List<Machine> _machines = new List<Machine>();
        private List<Operation> _operations = new List<Operation>();
        Dictionary<Operation, double> _trudoemkost = new Dictionary<Operation, double>(); 
        Dictionary<Machine, double> _machinesCost = new Dictionary<Machine, double>(); 
        Dictionary<Machine, double> _squaresDictionary = new Dictionary<Machine, double>();
        private double _totalMachinesCost;
        private double _totalBuildingCost;
        private Dictionary<Machine, double> _depreciationSum = new Dictionary<Machine, double>();
        private double _buildingDepreciation;
         

        public void GetMachinesCount()
        {
            var feff = _workDaysCount*_shifts*_shiftDuration*(1 - _machineDowntimePercent/100);
            foreach (var t in _operations)
            {
                var count = Math.Ceiling(_release/feff*t.RateTime/60);
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
        }

        void GetDepreciationSum()
        {
            foreach (var t in _machines)
            {
                _depreciationSum[t] = _machinesCost[t]*1.0*_machineDepreciation/100;
            }
            _buildingDepreciation = _totalBuildingCost*_shopDepreciation/100;
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
            //dataGridView1[2, index].Value = _machinesCount[t];
            dataGridView1[3, index].Value = _totalBuildingCost;
            dataGridView1[4, index].Value = _shopDepreciation;
            dataGridView1[5, index].Value = _buildingDepreciation;
        }
    }
}
