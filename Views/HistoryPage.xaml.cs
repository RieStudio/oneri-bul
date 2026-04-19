namespace OneriBul.Views;

public partial class HistoryPage : ContentPage
{
	public HistoryPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ViewModels.HistoryViewModel viewModel)
        {
            viewModel.RefreshCommand.Execute(null);
        }
    }
}
