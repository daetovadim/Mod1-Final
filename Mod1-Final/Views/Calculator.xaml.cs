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

namespace Mod1_Final
{
    /// <summary>
    /// Interaction logic for Calculator.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Uri uri = new Uri("Views/Light.xaml", UriKind.Relative);

        public MainWindow()
        {
            InitializeComponent();
            
            ResourceDictionary resource = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resource);
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ImageSourceValueSerializer imageSourceValue = new ImageSourceValueSerializer();
            ImageSource darkImageSource = (ImageSource)imageSourceValue.ConvertFromString("pack://application:,,,/Views/icons/dark.png", null);
            ImageSource lightImageSource = (ImageSource)imageSourceValue.ConvertFromString("pack://application:,,,/Views/icons/light.png", null);

            if (themeIcon.Source.ToString() == lightImageSource.ToString())
            {
                themeIcon.Source = darkImageSource;
                uri = new Uri("Views/Dark.xaml", UriKind.Relative);
            }
            else
            {
                themeIcon.Source = lightImageSource;
                uri = new Uri("Views/Light.xaml", UriKind.Relative);
            }

            ResourceDictionary resource = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resource);
        }
    }
}
