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
using System.Windows.Interop;
using Wren.Core;
using Wren.Emulation.Nes;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using ShaderEffectLibrary;
using System.Windows.Media.Effects;
using AHD.MyNes.Nes;
using Wren.Core.Input;
using Wren.Module.Core.UI.Windows;
using Wren.Core.Settings;
using System.Threading;
using Wren.Core.Replay;

namespace Wren
{
    /// <summary>
    /// Interaction logic for EmulatorView.xaml
    /// </summary>
    public partial class EmulatorView : UserControl
    {
        IEmulator _emulator;
        Boolean _hasQuit = false;
        RenderingSource _source;

        public EmulatorView()
        {
            InitializeComponent();
            Keyboard.AddKeyDownHandler(this, KeyHandler);
        }

        private void KeyHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                _hasQuit = true;
            }
        }
    }
}
