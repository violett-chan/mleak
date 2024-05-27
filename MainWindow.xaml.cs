using mleak.UIElements;
using System.Printing;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace mleak
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Ellipse firstEllipse = null; // Первый выбранный кружок
        private int[,] graph = new int[16, 16];
        private int[,] distances = new int[16, 16];
        public MainWindow()
        {
            InitializeComponent();
            LoadingWindow loadingWindow = new LoadingWindow();
            loadingWindow.ShowDialog();
        }
       
        private void OnCalculateClick(object sender, RoutedEventArgs e)
        {
            /*int[,] graph = ParseInput(InputTextBox.Text);
            int[,] distances = FloydWarshall(graph);
            OutputTextBlock.Text = FormatOutput(distances);*/
        }
        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            Point relativeLocation = border.TransformToAncestor(globalGrid)
                                           .Transform(new Point(-44, -44));
            hintBorder.Margin = new Thickness(relativeLocation.X, relativeLocation.Y, 0, 0);
            int row = Grid.GetRow(border);
            int col = Grid.GetColumn(border);
            int dist = 0;
            if ((border.Parent as Grid) == arrMain) dist = graph[row, col];
            else if ((border.Parent as Grid) == arrAns) dist = distances[row, col];
            hintFirst.Content = ((char)(65 + row)).ToString();
            hintSecond.Content = ((char)(65 + col)).ToString();
            if (dist >= 1000) hintDist.Content = "∞";
            else hintDist.Content = dist;
            hintBorder.BeginAnimation(Border.OpacityProperty, null);
            AnimateHintOpacity(1);
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            hintBorder.BeginAnimation(Border.OpacityProperty, null);
            AnimateHintOpacity(0);
        }

        private void AnimateHintOpacity(double targetOpacity)
        {
            DoubleAnimation opacityAnimation = new DoubleAnimation
            {
                From = hintBorder.Opacity,
                To = targetOpacity,
                Duration = TimeSpan.FromSeconds(0.5)
            };

            hintBorder.BeginAnimation(Border.OpacityProperty, opacityAnimation);
        }
        public SolidColorBrush GetColorFromValue(int value)
        {
            byte red = 0;
            byte green = 0;
            byte blue = 0;

            if (value >= -1000 && value < 0)
            {
                // Переход от белого к максимально синему
                value *= -1;
                red = 255;
                green = (byte)(255 - (value * 255 / 500));
                blue = (byte)(255 - (value * 255 / 500));
            }
            else if (value >= 0 && value <= 500)
            {
                // Переход от белого к максимально синему
                red = (byte)(255 - (value * 255 / 500));
                green = (byte)(255 - (value * 255 / 500));
                blue = 255;
            }
            else if (value >= 501 && value <= 1000)
            {
                // Переход от максимально синего к черному
                int adjustedValue = value - 500;  // Нормализуем значение от 0 до 500
                blue = (byte)(255 - (adjustedValue * 255 / 500));
            }

            return new SolidColorBrush(Color.FromRgb(red, green, blue));
        }
        public void UpdateGridWithLabels(Grid grid, int[,] graph)
        {
            for (int row = 0; row < grid.RowDefinitions.Count; row++)
            {
                for (int col = 0; col < grid.ColumnDefinitions.Count; col++)
                {
                    int c = graph[row, col];
                    // Проверяем, существует ли Label в данной ячейке
                    Border border = grid.Children.Cast<UIElement>().FirstOrDefault(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col) as Border;
                    SolidColorBrush color = GetColorFromValue(c);
                    if (border == null)
                    {
                        // Если Label не существует, создаем новый
                        border = new Border()
                        {
                            Width = 12,
                            Height = 12,
                            CornerRadius = new CornerRadius(6),
                        };
                        border.MouseEnter += (s, e) => Border_MouseEnter(s, e);
                        border.MouseLeave += Border_MouseLeave;
                        Grid.SetRow(border, row);
                        Grid.SetColumn(border, col);
                        grid.Children.Add(border);
                    }
                    border.Background = color;
                }
            }
        }
        private int[,] ParseInput(string input)
        {
            string[] lines = input.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            int n = lines.Length;
            int[,] graph = new int[n, n];

            for (int i = 0; i < n; i++)
            {
                string[] parts = lines[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < n; j++)
                {
                    graph[i, j] = int.Parse(parts[j]);
                }
            }
            return graph;
        }

        private int[,] FloydWarshall(int[,] graph)
        {
            int n = graph.GetLength(0);
            int[,] distances = (int[,])graph.Clone();

            for (int k = 0; k < n; k++)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (distances[i, k] + distances[k, j] < distances[i, j])
                        {
                            distances[i, j] = distances[i, k] + distances[k, j];
                        }
                    }
                }
            }
            return distances;
        }

        private string FormatOutput(int[,] distances)
        {
            int n = distances.GetLength(0);
            string result = "";
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    result += distances[i, j].ToString() + " ";
                }
                result += "\n";
            }
            return result;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int counter = 0; // Счетчик для номерации кругов
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    // Создаем Grid для хранения Ellipse и TextBlock
                    Grid cellGrid = new Grid();
                    Ellipse ellipse = new Ellipse();
                    TextBlock textBlock = new TextBlock();

                    ellipse.Style = (Style)FindResource("GlowingCircleStyle");
                    ellipse.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;

                    // Настройка TextBlock
                    textBlock.Text = ((char)(65 + counter)).ToString();
                    textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                    textBlock.VerticalAlignment = VerticalAlignment.Center;
                    textBlock.FontSize = 20;
                    textBlock.IsHitTestVisible = false;
                    textBlock.FontFamily = new FontFamily("Candara Light");
                    textBlock.Foreground = new SolidColorBrush(Colors.Gray);

                    // Добавление Ellipse и TextBlock во внутренний Grid
                    cellGrid.Children.Add(ellipse);
                    cellGrid.Children.Add(textBlock);

                    // Устанавливаем Grid в ячейку основного Grid
                    Grid.SetRow(cellGrid, i);
                    Grid.SetColumn(cellGrid, j);
                    mainGrid.Children.Add(cellGrid);

                    counter++; // Увеличиваем счетчик для следующего номера круга
                }
            }

            DrawGrid(arrMainCanvas);
            DrawGrid(arrAnsCanvas);

            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    if (i == j) graph[i, j] = 0;
                    else graph[i, j] = 1000;
                }
            }
            updateAll();
        }
        private void updateAll()
        {
            UpdateGridWithLabels(arrMain, graph);
            distances = FloydWarshall(graph);
            UpdateGridWithLabels(arrAns, distances);
        }
        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Ellipse clickedEllipse = sender as Ellipse;
            clickedEllipse.Stroke = new SolidColorBrush(Colors.White);
            if (firstEllipse == null)
            {
                firstEllipse = clickedEllipse;
            }
            else
            {
                if (firstEllipse == clickedEllipse)
                {
                    clickedEllipse.Stroke = new SolidColorBrush(Colors.Black);
                    firstEllipse = null;
                    return; 
                }
                DrawArrowBetweenEllipses(firstEllipse, clickedEllipse, mainCanvas);

                int firstNum = Grid.GetColumn(firstEllipse.Parent as Grid) 
                    + Grid.GetRow(firstEllipse.Parent as Grid) * 4;
                int secondNum = Grid.GetColumn(clickedEllipse.Parent as Grid) 
                    + Grid.GetRow(clickedEllipse.Parent as Grid) * 4;

                int result = 0;
                StartBlur(globalBorder);
                InputDialog inputDialog = new InputDialog();
                inputDialog.Owner = this;
                if (inputDialog.ShowDialog() == true)
                {
                    string response = inputDialog.ResponseText;
                    result = int.Parse(response);
                    StopBlur(globalBorder);
                    graph[firstNum, secondNum] = result;
                    updateAll();
                }
                string dist = result.ToString();
                if (result >= 1000) dist = "∞";
                eventUI ui = new eventUI(((char)(65 + firstNum)).ToString(), 
                    ((char)(65 + secondNum)).ToString(), dist);

                ansStack.Children.Add(ui);

                firstEllipse = null; // Сброс первого кружка после рисования стрелки
            }
        }
        private Point GetCenterPoint(Ellipse ellipse)
        {
            var transform = ellipse.TransformToAncestor(mainCanvas);
            var topLeft = transform.Transform(new Point(0, 0)); // Получаем верхний левый угол кружка
            var center = new Point(topLeft.X + ellipse.ActualWidth / 2, topLeft.Y + ellipse.ActualHeight / 2);
            return center;
        }
        private void DrawArrowBetweenEllipses(Ellipse ellipse1, Ellipse ellipse2, Canvas mainCanvas)
        {
            if (ellipse1 == ellipse2) return;
            // Получаем центры обоих кругов
            Point center1 = GetCenterPoint(ellipse1);
            Point center2 = GetCenterPoint(ellipse2);

            // Рассчитываем вектор направления между центрами
            Vector direction = center2 - center1;
            direction.Normalize(); // Нормализуем вектор для получения единичного вектора

            // Рассчитываем точки старта и окончания линии с небольшим отступом от края круга
            Point start = center1 + direction * (ellipse1.ActualWidth / 2); // Отступаем на радиус + 10 пикселей
            Point end = center2 - direction * (ellipse2.ActualWidth / 2 + 10); // Отступаем на радиус + 10 пикселей

            // Рисуем линию
            Line line = new Line
            {
                X1 = start.X,
                Y1 = start.Y,
                X2 = end.X,
                Y2 = end.Y,
                Effect = new DropShadowEffect()
                {
                    Color = Colors.White,
                    ShadowDepth = 0,
                },
                Stroke = Brushes.White,
                StrokeThickness = 5
            };
            mainCanvas.Children.Add(line);
            end = center2 - direction * (ellipse2.ActualWidth / 2 - 5);
            // Рисуем стрелку на конце линии
            double arrowAngle = Math.Atan2(line.Y2 - line.Y1, line.X2 - line.X1) + Math.PI;
            double arrowLength = 20;
            Polygon arrowHead = new Polygon
            {
                Effect = new DropShadowEffect()
                {
                    Color = Colors.White,
                    ShadowDepth = 0,
                },
                Fill = Brushes.White,
                Points = new PointCollection
        {
            new Point(end.X, end.Y),
            new Point(end.X + arrowLength * Math.Cos(arrowAngle - 0.3), end.Y + arrowLength * Math.Sin(arrowAngle - 0.3)),
            new Point(end.X + arrowLength * Math.Cos(arrowAngle + 0.3), end.Y + arrowLength * Math.Sin(arrowAngle + 0.3))
        }
            };
            mainCanvas.Children.Add(arrowHead);
        }

        private void DrawGrid(Canvas arrMainCanvas)
        {
            double margin = 5;  // Отступ от краев
            double horizontalStep = (arrMainCanvas.ActualWidth - 2 * margin) / 16;  // Шаг между линиями по горизонтали
            double verticalStep = (arrMainCanvas.ActualHeight - 2 * margin) / 16;  // Шаг между линиями по вертикали

            // Рисуем горизонтальные линии
            for (int i = 1; i <= 15; i++)
            {
                Line horizontalLine = new Line
                {
                    X1 = margin,
                    Y1 = margin + i * verticalStep,
                    X2 = arrMainCanvas.ActualWidth - margin,
                    Y2 = margin + i * verticalStep,
                    Stroke = Brushes.White,
                    Opacity = 0.1,
                    StrokeThickness = 0.5
                };
                arrMainCanvas.Children.Add(horizontalLine);
            }

            // Рисуем вертикальные линии
            for (int j = 1; j <= 15; j++)
            {
                Line verticalLine = new Line
                {
                    X1 = margin + j * horizontalStep,
                    Y1 = margin,
                    X2 = margin + j * horizontalStep,
                    Y2 = arrMainCanvas.ActualHeight - margin,
                    Stroke = Brushes.White,
                    Opacity = 0.1,
                    StrokeThickness = 0.5
                };
                arrMainCanvas.Children.Add(verticalLine);
            }
        }
        public void StartBlur(Border border)
        {
            Storyboard blurStoryboard = (Storyboard)this.Resources["BlurAnimation"];
            Storyboard.SetTarget(blurStoryboard, border); // Привязываем анимацию к Border
            blurStoryboard.Begin();
        }

        public void StopBlur(Border border)
        {
            Storyboard unblurStoryboard = (Storyboard)this.Resources["UnblurAnimation"];
            Storyboard.SetTarget(unblurStoryboard, border); // Привязываем анимацию к Border
            unblurStoryboard.Begin();
        }

    }
}