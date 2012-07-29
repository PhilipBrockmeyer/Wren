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
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        SettingsViewModel _viewModel;

        public SettingsView(SettingsViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            this.DataContext = _viewModel;
            romPaths.Content = new RomPathsView(viewModel.RomPathsViewModel);
            smsInput.Content = new SmsInputView(viewModel.SmsInputViewModel, viewModel.InputManager);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Save();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _viewModel.Cancel();
        }
    }
}
