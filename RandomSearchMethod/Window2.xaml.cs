using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RandomSearchMethod
{
	public partial class Window2 : Window
	{
		Image container = null;
		
		public Window2()
		{
			InitializeComponent();
			BitmapImage image = new BitmapImage();
			image.BeginInit();
			image.CacheOption = BitmapCacheOption.OnLoad;
			image.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"1.png");
			image.EndInit();
			container = new Image();
			container.Source = image;
			container.Stretch = Stretch.Fill;
			container.Margin = new Thickness(0.5);
			SP.Children.Add(container);
			
			BitmapImage image2 = new BitmapImage();
			image2.BeginInit();
			image2.CacheOption = BitmapCacheOption.OnLoad;
			image2.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"2.png");
			image2.EndInit();
			container = new Image();
			container.Source = image2;
			container.Stretch = Stretch.Fill;
			container.Margin = new Thickness(0.5);
			SP.Children.Add(container);
			
			BitmapImage image3 = new BitmapImage();
			image3.BeginInit();
			image3.CacheOption = BitmapCacheOption.OnLoad;
			image3.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"3.png");
			image3.EndInit();
			container = new Image();
			container.Source = image3;
			container.Stretch = Stretch.Fill;
			container.Margin = new Thickness(0.5);
			SP.Children.Add(container);
			
			BitmapImage image4 = new BitmapImage();
			image4.BeginInit();
			image4.CacheOption = BitmapCacheOption.OnLoad;
			image4.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"4.png");
			image4.EndInit();
			container = new Image();
			container.Source = image4;
			container.Stretch = Stretch.Fill;
			container.Margin = new Thickness(0.5);
			SP.Children.Add(container);
		}
	}
}