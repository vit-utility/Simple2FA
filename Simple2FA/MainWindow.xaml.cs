using OtpNet;
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
using static System.Collections.Specialized.BitVector32;

namespace Simple2FA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DateTime startTime;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Paste_Click(object sender, RoutedEventArgs e)
        {
            this.tbKey.Text = Clipboard.GetText(TextDataFormat.Text);

            this.Button_Generate_Click(sender, e);
        }

        private void Button_Generate_Click(object sender, RoutedEventArgs e)
        {
            tbError.Visibility = Visibility.Collapsed;

            var secret = new String(this.tbKey.Text.Where(c => Char.IsLetterOrDigit(c)).ToArray());
            var totp = new Totp(Base32Encoding.ToBytes(secret));

            this.tbResult.Text = totp.ComputeTotp();
            this.StartTimer(totp.RemainingSeconds());
        }

        private void Button_Copy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(this.tbResult.Text, TextDataFormat.UnicodeText);
            }
            catch
            {
                tbError.Visibility = Visibility.Visible;
            }
        }

        private void StartTimer(int timeout)
        {
            this.startTime = DateTime.Now;
            var endTime = this.startTime.AddSeconds(timeout);
            var remainingTime = endTime - DateTime.Now;

            this.rValidTime.Text = remainingTime.ToString(@"mm\:ss");

            Task.Factory.StartNew(() =>
            {
                while (remainingTime > TimeSpan.Zero)
                {
                    Application.Current.Dispatcher.BeginInvoke(() => { this.rValidTime.Text = remainingTime.ToString(@"mm\:ss"); });

                    System.Threading.Thread.Sleep(500);
                    remainingTime = endTime - DateTime.Now;
                }
            },
                TaskCreationOptions.LongRunning);
        }
    }
}
