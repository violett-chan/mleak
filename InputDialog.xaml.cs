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
using System.Windows.Shapes;

namespace mleak
{
    /// <summary>
    /// Логика взаимодействия для InputDialog.xaml
    /// </summary>
    public partial class InputDialog : Window
    {
        public string ResponseText = "";
        public InputDialog()
        {
            InitializeComponent();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (inputTextBox.Content != null) ResponseText = inputTextBox.Content.ToString()!;
            if (e.Key == Key.Enter)
            {
                this.DialogResult = true;
            }
            else if (e.Key == Key.Back)
            {
                if (ResponseText.Length != 0) inputTextBox.Content = ResponseText.Substring(0, ResponseText.Length - 1);
            }
            else
            {
                if (ResponseText.Length != 4) inputTextBox.Content += GetDigitFromKey(e).ToString();
            }
        }

        private int? GetDigitFromKey(KeyEventArgs e)
        {
            // Проверяем диапазон основных цифровых клавиш
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                // Не на NumPad
                return e.Key - Key.D0;
            }
            // Проверяем диапазон цифровых клавиш на NumPad
            else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                return e.Key - Key.NumPad0;
            }

            return null; // Возвращаем null, если нажата не цифровая клавиша
        }
    }
}
