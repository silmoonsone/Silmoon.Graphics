using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Silmoon.Extension;
using Silmoon.Graphics.Financial;
using SkiaSharp;

namespace GraphicsMauiTesting.Pages;

public partial class ActionableCandleChartPage : ContentPage
{
    public ActionableCandleChart _chart;
    ActionableCandleChartPageViewModel _viewModel;
    public ActionableCandleChartPage()
    {
        _chart = new ActionableCandleChart(1, 800, 600);
        BindingContext = _viewModel = new ActionableCandleChartPageViewModel(this);
        InitializeComponent();
    }

    private void Image_SizeChanged(object sender, EventArgs e)
    {
        _chart.SetSize((int)nameImage.Width, (int)nameImage.Height);
    }
}
public partial class ActionableCandleChartPageViewModel : ObservableObject
{
    ActionableCandleChartPage page;

    [ObservableProperty]
    public partial ImageSource ImageSource { get; set; }
    [ObservableProperty]
    public partial Stream ImageStream { get; set; }

    public ActionableCandleChartPageViewModel(ActionableCandleChartPage actionableCandleChartPage)
    {
        page = actionableCandleChartPage;
        page._chart.FrameRefreshed(frame =>
        {
            var stream = new MemoryStream();
            using var data = frame.Encode(SKEncodedImageFormat.Png, 100);
            if (!data.IsNullOrDefault())
            {
                data.SaveTo(stream);
                stream.Position = 0;
                ImageStream = stream;
            }
        });
    }

    [RelayCommand]
    public void RefreshImage()
    {
        var bitmap = page._chart.RefreshFrame();
    }
}