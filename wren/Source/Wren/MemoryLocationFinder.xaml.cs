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
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Wren
{
    /// <summary>
    /// Interaction logic for MemoryLocationFinder.xaml
    /// </summary>
    public partial class MemoryFilter : Window
    {

        public MemoryFilter(MemoryFilterViewModel viewModel)
        {
            this.DataContext = viewModel;
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;

            base.OnClosing(e);

            
        }
    }
}
