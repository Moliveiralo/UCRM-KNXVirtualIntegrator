using System.Windows;
using KNX_Virtual_Integrator.ViewModel;

namespace KNX_Virtual_Integrator.View.Windows;

public partial class TESTANALYSE : Window
{
    private readonly MainViewModel _viewModel;
    
    public TESTANALYSE(MainViewModel viewModel)
    {
        InitializeComponent();
        
        _viewModel = viewModel;
        DataContext = _viewModel;
    }
}