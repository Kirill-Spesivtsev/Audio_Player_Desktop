using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AudioPlayer
{
    /// <summary>
    /// Логика взаимодействия для Visualizer1.xaml
    /// </summary>
    public partial class Visualizer1 : UserControl
    {
        public Visualizer1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Установка значений шкал визуализатора
        /// </summary>
        /// <param name="data"></param>
        internal void Set(List<byte> data) 
        {
            if (data.Count < 16) return;
            Bar1.Value = data[0];
            Bar2.Value = data[1];
            Bar3.Value = data[2];
            Bar4.Value = data[3];
            Bar5.Value = data[4];
            Bar6.Value = data[5];
            Bar7.Value = data[6];
            Bar8.Value = data[7];
            Bar9.Value = data[8];
            Bar10.Value = data[9];
            Bar11.Value = data[10];
            Bar12.Value = data[11];
            Bar13.Value = data[12];
            Bar14.Value = data[13];
            Bar15.Value = data[14];
            Bar16.Value = data[15];
        }

        /// <summary>
        /// Очистка визуализатора
        /// </summary>
        public void Clear()
        {
            List<byte> temp = new List<byte> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
            Set(temp);
        }
    }
}
