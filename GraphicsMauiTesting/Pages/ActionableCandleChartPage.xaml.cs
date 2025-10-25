using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Silmoon.Graphics.Financial;
using SkiaSharp;
using System.Threading.Tasks;

namespace GraphicsMauiTesting.Pages;

public partial class ActionableCandleChartPage : ContentPage
{
    ActionableCandleChartPageViewModel _viewModel;
    public ActionableCandleChartPage()
    {
        BindingContext = _viewModel = new ActionableCandleChartPageViewModel(this);
        InitializeComponent();
    }
}
public partial class ActionableCandleChartPageViewModel : ObservableObject
{
    ActionableCandleChartPage page;
    private ActionableCandleChart _chart;

    [ObservableProperty]
    public partial ImageSource ImageSource { get; set; }
    [ObservableProperty]
    public partial Stream ImageStream { get; set; }

    public ActionableCandleChartPageViewModel(ActionableCandleChartPage actionableCandleChartPage)
    {
        page = actionableCandleChartPage;
        _chart = new ActionableCandleChart(1, 800, 600);
        _chart.FrameRefreshed(frame =>
        {
            var stream = new MemoryStream();
            using var image = SKImage.FromBitmap(frame);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            data.SaveTo(stream);
            stream.Position = 0;
            ImageStream = stream;
        });
    }

    [RelayCommand]
    public void RefreshImage()
    {
        var bitmap = _chart.RefreshFrame();
    }
}