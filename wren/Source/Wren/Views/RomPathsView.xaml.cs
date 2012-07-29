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
using Microsoft.Win32;
using Wren.Core.Directory;

namespace Wren.Views
{
    /// <summary>
    /// Interaction logic for RomPathsView.xaml
    /// </summary>
    public partial class RomPathsView : UserControl
    {
        RomPathsViewModel _viewModel;

        public RomPathsView(RomPathsViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            this.DataContext = _viewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddPath();

            grdPaths.SelectedIndex = grdPaths.Items.Count - 1;
            grdPaths.BeginEdit();
        }

        private void GetFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            var result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                ((PathSettings.PathSetting)grdPaths.SelectedItem).Path = dialog.SelectedPath;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _viewModel.DeletePath(grdPaths.SelectedIndex);
        }
    }
}
