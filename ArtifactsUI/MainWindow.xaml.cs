using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArtifactsUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public static readonly DependencyProperty HtmlSourceProperty = DependencyProperty.RegisterAttached("HtmlSource", typeof(string), typeof(WebView2));

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LancerMCU(CoreWebView2NavigationCompletedEventArgs e)
        {
            /*if (WebView != null)
            {
                string content = (string)WebView.GetValue(HtmlSourceProperty);
            }*/
        }

        private async void LancerMCU2(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            //CoreWebView2 webview = (CoreWebView2)sender;
            /*string content = await WebView.ExecuteScriptAsync("document.documentElement.outerHTML");
            content = HttpUtility.HtmlDecode(content);
            Console.WriteLine(content);

            if (content.Contains("forest_bank1.png"))
            {
                Console.WriteLine("lancé");
            }*/
        }
    }
}