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
using Microsoft.VisualStudio.PlatformUI;

namespace MobTimer.UI
{
    /// <summary>
    /// Interaction logic for MobTimerPopupDialog.xaml
    /// </summary>
    public partial class MobTimerPopupDialog : DialogWindow
    {
        public MobTimerPopupDialog(string nextUp)
        {
            InitializeComponent();

            nextUpNameLabel.Content = nextUp;
        }
        
        private void goButton_Click(object sender, RoutedEventArgs e)
        {
            CloseWithOk();

        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            CloseWithOk(false);
        }

        private void CloseWithOk(bool isOk = true)
        {
            base.DialogResult = isOk;
            base.Close();
        }
    }
}
