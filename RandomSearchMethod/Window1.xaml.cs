using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RandomSearchMethod
{
	public partial class Window1 : Window
	{
		const int count = 4;
		const double accuracy = 0.01; // точность
		double r = 1; // шаг
		List<double[]> listPoint;
		List<int> listConnection;
		List<double[]> listUnsuccessfulPoint;
		List<double> listObjectiveFunction;
		double[] coefficients = new double[count];
		double[] startingPoint;
		int n;
		double max;
		double min;
		DrawingGroup Chart = new DrawingGroup();
		
		public Window1()
		{
			InitializeComponent();
			WindowStartupLocation = WindowStartupLocation.CenterScreen;
		}
		
		void Button_Click(object sender, RoutedEventArgs e)
		{
			if (double.TryParse(functionX12.Text, out coefficients[0]) && double.TryParse(functionX1.Text, out coefficients[1]) &&
			    double.TryParse(functionX22.Text, out coefficients[2]) && double.TryParse(functionX2.Text, out coefficients[3]) &&
			    StartingPoint.Text != string.Empty && double.TryParse(step.Text, out r))
			{
				listPoint = new List<double[]>();
				listConnection =  new List<int>();
				listUnsuccessfulPoint = new List<double[]>();
				listObjectiveFunction = new List<double>();
				string[] start = StartingPoint.Text.Split();
				startingPoint = new double[2];
				if (start.Length == 2 && double.TryParse(start[0], out startingPoint[0]) && double.TryParse(start[1], out startingPoint[1]))
				{
					if ((int)startingPoint[1] > 10)
						n = 10;
					else
						n = (int)startingPoint[1];
					double W; // Целевая функция
					result.Text = "";
					listPoint.Add(startingPoint); // первая точка в список
					result.Text += "Минимум целевой функции методом случайного поиска:\r\n";
					result.Text += "Итерация - 1\r\n";
					result.Text += "Задана начальная точка x0 = ("+ startingPoint[0] +";"+ startingPoint[1] +"); шаг: r = " + r + "\r\n";
					W = CalculateObjectiveFunction(startingPoint[0], startingPoint[1]);
					listObjectiveFunction.Add(W);
					CalculateRandomSearchMethod();
					DotChart.Source = new DrawingImage(ConstructGraph());
				}
				else
					MessageBox.Show("Неправильно введена начальная точка", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			else
				MessageBox.Show("Неверно введены данные", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
		}
		
		void CalculateRandomSearchMethod()
		{
			int i = 2; // Номер итерации
			bool end = true;
			do
			{
				double W = listObjectiveFunction[listObjectiveFunction.Count - 1];
				double Wnew;
				int k = 0;
				result.Text += "\nИтерация - "+ i +":\r\n";
				do
				{
					double[] PointNew = RandomOnUnitSphere(listPoint[listPoint.Count - 1][0], listPoint[listPoint.Count - 1][1]);
					result.Text += "Расчитывает единичный вектор направления:\r\n";
					result.Text += "x" + i + " = ("+ Math.Round(PointNew[0], 3) +";"+ Math.Round(PointNew[1], 3) +");\r\n";
					Wnew = CalculateObjectiveFunction(PointNew[0], PointNew[1]);
					if (W > Wnew)
					{
						result.Text += "Взята верная точка, переходим на следующую итерацию\r\n";
						i++;
						listPoint.Add(PointNew);
						listObjectiveFunction.Add(Wnew);
					}
					else
					{
						k++;
						if (k > 10)
						{
							end = false;
						}
						result.Text += "Взята неверная точка, перещитывает единичный вектор направления\r\n";
						listConnection.Add(i - 2);
						listUnsuccessfulPoint.Add(PointNew);
					}
				}
				while (W < Wnew && end);
			}
			while (CheckEndSearch(i) && end);
			result.Text += "\nОтвет: Точка("+ Math.Round(listPoint[listPoint.Count - 1][0], 3) +" ; "+ Math.Round(listPoint[listPoint.Count - 1][1], 3) +"), W = "+ Math.Round(CalculateObjectiveFunction(listPoint[listPoint.Count - 1][0], listPoint[listPoint.Count - 1][1]), 3);
		}
		
		Random rng = new Random();
		double[] RandomOnUnitSphere(double X, double Y)
		{
			double D = rng.Next(360);
			double[] Point = new double[2];
			Point[0] = r * Math.Cos(D) + X;
			Point[1] = r * Math.Sin(D) + Y;
			return Point;
		}
		
		bool CheckEndSearch(int i)
		{
			result.Text += "Проверка на окончание поиска:\r\n";
			if (listObjectiveFunction[listObjectiveFunction.Count - 2] - listObjectiveFunction[listObjectiveFunction.Count -1] > accuracy && i < 50)
			{
				return true;
			}
			return false;
		}
		
		double CalculateObjectiveFunction(double X1, double X2)
		{
			double res = coefficients[0] * (X1 * X1) + coefficients[1] * X1 + coefficients[2] * (X2 * X2) + coefficients[3] * X2;
			result.Text += "W(" + Math.Round(X1, 3) + " ; " + Math.Round(X2, 3) + ") = "+ Math.Round(res, 3) + "\r\n";
			return res;
		}
		
		void MaxMin()
		{
			double maxX = double.MinValue;
			double minX = double.MaxValue;
			double maxY = double.MinValue;
			double minY = double.MaxValue;
			
			for (int i = 0; i < listPoint.Count; i++)
			{
				if (maxX < listPoint[i][0]) maxX = listPoint[i][0];
				if (minX > listPoint[i][0]) minX = listPoint[i][0];
				if (maxY < listPoint[i][1]) maxY = listPoint[i][1];
				if (minY > listPoint[i][1]) minY = listPoint[i][1];
			}
			if (minX > 0) minX = 0;
			if (minY > 0) minY = 0;
			max = Math.Max(maxX, maxY);
			min = Math.Min(Math.Floor(minX), Math.Floor(minY));
			max += Math.Abs(min);
		}
		
		public DrawingGroup ConstructGraph()
		{
			Chart.Children.Clear();
			MaxMin();
			n = (int)max;
			double step = max / n;
			
			GeometryDrawing FrameDrw = new GeometryDrawing();
			GeometryGroup FrameGG = new GeometryGroup();
			FrameDrw.Brush = Brushes.Transparent;
			FrameDrw.Pen = new Pen(Brushes.Black, 2);
			RectangleGeometry FrameRG = new RectangleGeometry();
			FrameRG.Rect = new Rect(-60, -20, 490, 480);
			FrameGG.Children.Add(FrameRG);
			FrameDrw.Geometry = FrameGG;
			Chart.Children.Add(FrameDrw);

			GeometryDrawing drw1 = new GeometryDrawing();
			GeometryGroup gg1 = new GeometryGroup();
			drw1.Brush = Brushes.LightGoldenrodYellow;
			drw1.Pen = new Pen(Brushes.Gray, 0.5);
			for (int i = 1; i < n; i++)
			{
				LineGeometry myRectGeometry1 = new LineGeometry(new Point(400, i * (400 / n)), new Point(-10, i * (400 / n)));
				gg1.Children.Add(myRectGeometry1);
			}
			drw1.Geometry = gg1;
			Chart.Children.Add(drw1);
			
			GeometryDrawing drw2 = new GeometryDrawing();
			GeometryGroup gg2 = new GeometryGroup();
			drw2.Brush = Brushes.LightGoldenrodYellow;
			drw2.Pen = new Pen(Brushes.Gray, 0.5);
			for (int i = 1; i < n; i++)
			{
				LineGeometry myRectGeometry2 = new LineGeometry(new Point(i * (400 / n), 410), new Point(i * (400 / n), 0));
				gg2.Children.Add(myRectGeometry2);
			}
			drw2.Geometry = gg2;
			Chart.Children.Add(drw2);
			
			GeometryDrawing drw4 = new GeometryDrawing();
			GeometryGroup gg4 = new GeometryGroup();
			drw4.Brush = Brushes.Transparent;
			drw4.Pen = new Pen(Brushes.LightGray, 2);
			RectangleGeometry myRectGeometry4 = new RectangleGeometry();
			myRectGeometry4.Rect = new Rect(0, 0, 400, 400);
			gg4.Children.Add(myRectGeometry4);
			drw4.Geometry = gg4;
			Chart.Children.Add(drw4);
			
			int q = 0;
			for (int i = 0; i < n + 1; i++) // y
			{
				double y = Math.Round((max - i * step) + min);
				if (y == 0)
				{
					q = i;
					break;
				}
			}
			GeometryDrawing drw = new GeometryDrawing();
			GeometryGroup gg = new GeometryGroup();
			drw.Brush = Brushes.Black;
			drw.Pen = new Pen(Brushes.Black, 1);
			LineGeometry myRectGeometry = new LineGeometry(new Point(400, q * (400 / n)), new Point(-10, q * (400 / n)));
			gg.Children.Add(myRectGeometry);
			drw.Geometry = gg;
			Chart.Children.Add(drw);
			
			for (int i = 0; i < n + 1; i++) // x
			{
				double x = Math.Round((i * step) + min);
				if (x == 0)
				{
					q = i;
					break;
				}
			}
			GeometryDrawing drw0 = new GeometryDrawing();
			GeometryGroup gg0 = new GeometryGroup();
			drw0.Brush = Brushes.Black;
			drw0.Pen = new Pen(Brushes.Black, 1);
			LineGeometry myRectGeometry0 = new LineGeometry(new Point(q * (400 / n), 410), new Point(q * (400 / n), 0));
			gg0.Children.Add(myRectGeometry0);
			drw0.Geometry = gg0;
			Chart.Children.Add(drw0);
			
			GeometryDrawing drw5 = new GeometryDrawing();
			GeometryGroup gg5 = new GeometryGroup();
			drw5.Brush = Brushes.LightGray;
			drw5.Pen = new Pen(Brushes.Black, 0.3);
			for (int i = 0; i < n + 1; i++) // y
			{
				string y = Math.Round((max - i * step) + min).ToString();
				FormattedText formattedText = new FormattedText(y, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 10, Brushes.Black);
				Geometry geometry = formattedText.BuildGeometry(new Point(-20, i * (400 / n)- 7));
				gg5.Children.Add(geometry);
			}
			drw5.Geometry = gg5;
			Chart.Children.Add(drw5);
			
			GeometryDrawing drw6 = new GeometryDrawing();
			GeometryGroup gg6 = new GeometryGroup();
			drw6.Brush = Brushes.LightGray;
			drw6.Pen = new Pen(Brushes.Black, 0.3);
			for (int i = 0; i < n + 1; i++) // x
			{
				string x = Math.Round((i * step) + min).ToString();
				FormattedText formattedText = new FormattedText(x, CultureInfo.GetCultureInfo("en-us"), FlowDirection.RightToLeft, new Typeface("Verdana"), 10, Brushes.Black);
				Geometry geometry = formattedText.BuildGeometry(new Point(i * (400 / n) + 5, 410));
				gg6.Children.Add(geometry);
			}
			drw6.Geometry = gg6;
			Chart.Children.Add(drw6);
			
			Chart.Children.Add(DrawLine(max, min, listPoint, 1, Brushes.Green));
			Chart.Children.Add(DrawUnsuccessfulLine(max, min, listUnsuccessfulPoint, 0.5, Brushes.Red));
			
			return (Chart);
		}
		
		public GeometryDrawing DrawLine(double max, double min, List<double[]> list, double thickness, Brush ColorWay)
		{
			GeometryDrawing geometry_Chart = new GeometryDrawing();
			GeometryGroup geometry = new GeometryGroup();
			for (int i = 0; i < list.Count - 1; i++)
			{
				double x = list[i][0];
				double x2 = list[i + 1][0];
				double y = list[i][1];
				double y2 = list[i + 1][1];
				x +=  Math.Abs(min);
				x2 += Math.Abs(min);
				y +=  Math.Abs(min);
				y2 += Math.Abs(min);
				LineGeometry line = new LineGeometry();
				line.StartPoint = new Point( ((double)400 / max) * x, 400 - y * ((double)400 / max));
				line.EndPoint = new Point( ((double)400 / max) * x2, 400 - y2 * ((double)400 / max));
				geometry.Children.Add(line);
			}
			geometry_Chart.Geometry = geometry;
			geometry_Chart.Pen = new Pen(ColorWay, thickness);
			return (geometry_Chart);
		}
		
		public GeometryDrawing DrawUnsuccessfulLine(double max, double min, List<double[]> list, double thickness, Brush ColorWay)
		{
			GeometryDrawing geometry_Chart = new GeometryDrawing();
			GeometryGroup geometry = new GeometryGroup();
			for (int i = 0; i < list.Count; i++)
			{
				double x = listPoint[listConnection[i]][0];
				double x2 = list[i][0];
				double y = listPoint[listConnection[i]][1];
				double y2 = list[i][1];
				x +=  Math.Abs(min);
				x2 += Math.Abs(min);
				y +=  Math.Abs(min);
				y2 += Math.Abs(min);
				LineGeometry line = new LineGeometry();
				line.StartPoint = new Point( ((double)400 / max) * x, 400 - y * ((double)400 / max));
				line.EndPoint = new Point( ((double)400 / max) * x2, 400 - y2 * ((double)400 / max));
				geometry.Children.Add(line);
			}
			geometry_Chart.Geometry = geometry;
			geometry_Chart.Pen = new Pen(ColorWay, thickness);
			return (geometry_Chart);
		}
		
		void MenuItemSpravka_Click(object sender, RoutedEventArgs e)
		{
			Window2 win2 = new Window2();
			win2.Show();
		}
		
		void MenuItemAbout_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("Разработал: студент группы БПИ-211\n\nСергей Галиев\n\nНОВОСИБИРСК","О разработчике");
		}
		
		void MenuItemQuit_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult vehot = MessageBox.Show("Вы уверены?","Выход", MessageBoxButton.OKCancel, MessageBoxImage.Question);
			if (vehot == MessageBoxResult.OK)
				this.Close();
		}
	}
}