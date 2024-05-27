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

namespace mleak.UIElements
{
    /// <summary>
    /// Логика взаимодействия для eventUI.xaml
    /// </summary>
    public partial class eventUI : UserControl
    {
        public eventUI(string first, string second, string value)
        {
            InitializeComponent();
            firstNode.Content = first;
            secondNode.Content = second;
            weightValue.Content = value;
        }
    }
}
