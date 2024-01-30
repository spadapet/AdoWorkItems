using Microsoft.Maui.Controls;
using WorkItems.Model;

namespace WorkItems;

internal partial class MainPage : ContentPage
{
    public MainModel Model { get; } = new();

    public MainPage()
    {
        this.InitializeComponent();
    }
}
