using OneriBul.ViewModels;

namespace OneriBul.Views;

public partial class DiscoverPage : ContentPage
{
	public DiscoverPage()
	{
		InitializeComponent();
	}

    private CancellationTokenSource? _marqueeCts;

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        if (BindingContext is DiscoverViewModel viewModel)
        {
            viewModel.PropertyChanged += async (s, e) =>
            {
                if (e.PropertyName == "IsPlatformVisible")
                {
                    if (viewModel.IsPlatformVisible)
                    {
                        PlatformLine.Scale = 0;
                        PlatformSelector.Opacity = 0;
                        PlatformSelector.TranslationY = -10;
                        
                        await PlatformLine.ScaleTo(1, 400, Easing.CubicOut);
                        await Task.WhenAll(
                            PlatformSelector.FadeTo(1, 400),
                            PlatformSelector.TranslateTo(0, 0, 400, Easing.CubicOut)
                        );
                    }
                    else
                    {
                        await Task.WhenAll(
                            PlatformSelector.FadeTo(0, 200),
                            PlatformLine.ScaleTo(0, 200)
                        );
                    }
                }
                
                if (e.PropertyName == nameof(viewModel.CurrentRecommendation))
                {
                    StartGenreMarquee();
                }
            };
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CheckAndStartPillMarquee(CatLabel0);
        CheckAndStartPillMarquee(CatLabel1);
        CheckAndStartPillMarquee(CatLabel2);
        CheckAndStartPillMarquee(CatLabel3);
    }

    private async void CheckAndStartPillMarquee(Label label)
    {
        try 
        {
            if (label == null) return;
            
            label.TranslationX = 0;
            await Task.Delay(1000); // Render süresini bekle
            
            string txt = label.Text ?? "";
            double labelWidth = label.Width;
            
            // 12 karakterden uzunsa veya sığmıyorsa kaydır (Pill genişliği yaklaşıktır)
            if (txt.Length > 12 || labelWidth > 60) 
            {
                double delta = labelWidth > 60 ? labelWidth - 60 + 10 : labelWidth * 0.5;
                while (true) // Sayfa kapanana kadar dönsün
                {
                    await Task.Delay(2000);
                    await label.TranslateTo(-delta, 0, (uint)(delta * 40), Easing.Linear);
                    await Task.Delay(1500);
                    await label.FadeTo(0, 200);
                    label.TranslationX = 0;
                    await label.FadeTo(1, 200);
                }
            }
        }
        catch { }
    }

    private async void StartGenreMarquee()
    {
        try 
        {
            _marqueeCts?.Cancel();
            _marqueeCts = new CancellationTokenSource();
            var token = _marqueeCts.Token;

            GenreLabel.TranslationX = 0;
            GenreLabel.HorizontalOptions = LayoutOptions.Center;
            
            // Give time for UI to render new text and calculate size
            await Task.Delay(1000);
            if (token.IsCancellationRequested) return;

            // KURAL: Eğer sadece 1 kelimeyse (boşluk yoksa) kaydırma yapma
            string txt = (GenreLabel.Text ?? "").Trim();
            if (!txt.Contains(" ")) return;

            double containerWidth = GenreBorder.Width - GenreBorder.Padding.HorizontalThickness;
            double labelWidth = GenreLabel.Width;

            // Daha hassas kontrol (Eğer yazı tabelanın boyutundan büyükse kaydır)
            if (labelWidth > containerWidth - 2)
            {
                // Sığmayan metin için sola dayanmalı ki kayma düzgün başlasın
                GenreLabel.HorizontalOptions = LayoutOptions.Start;
                await Task.Delay(100);

                double delta = labelWidth - containerWidth + 10;
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(2000, token); // Başlangıçta biraz bekle ki kullanıcı ilk kelimeyi okusun
                    
                    // Sola doğru kaydır
                    await GenreLabel.TranslateTo(-delta, 0, (uint)(delta * 35), Easing.Linear);
                    
                    // Beklemeden başa sar (Ufak bir fade efekti ile şık görünmesini sağla)
                    await GenreLabel.FadeTo(0, 150);
                    GenreLabel.TranslationX = 0;
                    await GenreLabel.FadeTo(1, 150);
                }
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception) { }
    }
}
