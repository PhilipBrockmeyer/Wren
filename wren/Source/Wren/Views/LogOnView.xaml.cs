using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wren.ViewModels;

namespace Wren.Views
{
    /// <summary>
    /// Interaction logic for LogOnView.xaml
    /// </summary>
    public partial class LogOnView : UserControl
    {
        Boolean _isRegistering;
        LogOnViewModel _viewModel;

        public LogOnView(LogOnViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            this.DataContext = _viewModel;

            txtPassword.Password = _viewModel.Password;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {           
            if (_isRegistering)
            {
                _isRegistering = false;

                grdContentRow.BeginAnimation(RowDefinition.HeightProperty,
                    new GridLengthAnimation() { To = new GridLength(160.0), From = new GridLength(195.0), Duration = TimeSpan.FromMilliseconds(200) });

                rowEmail.BeginAnimation(RowDefinition.HeightProperty,
                    new GridLengthAnimation() { To = new GridLength(0.0), From = new GridLength(35.0), Duration = TimeSpan.FromMilliseconds(200) });
            }
            else
            {
                _viewModel.LogOn(txtPassword.Password);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!_isRegistering)
            {
                _isRegistering = true;

                grdContentRow.BeginAnimation(RowDefinition.HeightProperty,
                    new GridLengthAnimation() { To = new GridLength(195.0), From = new GridLength(160.0), Duration = TimeSpan.FromMilliseconds(200) });

                rowEmail.BeginAnimation(RowDefinition.HeightProperty,
                    new GridLengthAnimation() { To = new GridLength(35.0), From = new GridLength(0.0), Duration = TimeSpan.FromMilliseconds(200) });
            }
            else
            {
                _viewModel.RegisterUser(txtPassword.Password);
            }
        }
    }
}
