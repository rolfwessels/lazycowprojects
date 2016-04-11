using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Windows;
using System.Windows.Input;

namespace ArdumotoBot.Remote
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowDataModel _mainWindowDataModel = new MainWindowDataModel(new WpfDispatcher(Application.Current.Dispatcher));

        public MainWindow()
        {
            InitializeComponent();

            DataContext = _mainWindowDataModel;
        }


        private void ClosingHandler(object sender, CancelEventArgs cancelEventArgs)
        {
            _mainWindowDataModel.DisconnectCommand.Execute(null);   
        }
    }
}