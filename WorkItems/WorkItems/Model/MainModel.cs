namespace WorkItems.Model;

internal class MainModel : PropertyNotifier
{
    private string searchText = string.Empty;
    public string SearchText
    {
        get => this.searchText;
        set => this.SetProperty(ref this.searchText, value ?? string.Empty);
    }

    public string ResultsTitleText
    {
        get
        {
            return Strings.ResultsTitle;
        }
    }
}
