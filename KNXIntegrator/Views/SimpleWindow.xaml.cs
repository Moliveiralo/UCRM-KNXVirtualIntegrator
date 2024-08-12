using System.Windows;

namespace KNXIntegrator
{
    public partial class SimpleWindow : Window
    {
        public Publisher publisher = new Publisher();
        public SimpleWindow()
        {
            InitializeComponent();
        }

        private void MyButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Button clicked!");
            publisher.notifySubscribers("Button clicked!");
        }
    }
}
