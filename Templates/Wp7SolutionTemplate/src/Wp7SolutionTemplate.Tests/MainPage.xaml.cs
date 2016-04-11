using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Silverlight.Testing;

namespace Wp7SolutionTemplate.Tests
{
    public partial class MainPage
    {
        private IMobileTestPage _mobileTestPage;
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            _mobileTestPage = UnitTestSystem.CreateTestPage() as IMobileTestPage;
            Loaded += LoadTests;
        }

        private void LoadTests(object sender, RoutedEventArgs e)
        {
            Loaded -= LoadTests;
            SystemTray.IsVisible = false;
            
            BackKeyPress += (x, xe) => xe.Cancel = _mobileTestPage.NavigateBack();
            ((PhoneApplicationFrame) Application.Current.RootVisual).Content = _mobileTestPage;     
        }
    }
}