using Microsoft.Maui.Controls;
using System.Windows.Input;

namespace WorkItems.Model;

public sealed class MainModel(AppModel appModel) : PropertyNotifier
{
    public AppModel AppModel { get; } = appModel;

    private string gptKey = string.Empty;
    public string GptKey
    {
        get => this.gptKey;
        set => this.SetProperty(ref this.gptKey, value);
    }

    private string searchText = string.Empty;
    public string SearchText
    {
        get => this.searchText;
        set => this.SetProperty(ref this.searchText, value);
    }

    private Command searchCommand;
    public ICommand SearchCommand => this.searchCommand ??= new Command(this.OnSearch);

    private void OnSearch()
    {
    }
}
