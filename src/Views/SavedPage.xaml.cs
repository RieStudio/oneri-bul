namespace OneriBul.Views;

public partial class SavedPage : ContentPage
{
	public SavedPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ViewModels.SavedViewModel viewModel)
        {
            viewModel.Refresh();
        }
    }
}
