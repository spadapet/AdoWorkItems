using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace WorkItems.Model;

internal sealed class MainModel(AppModel appModel) : PropertyNotifier
{
    public AppModel AppModel { get; } = appModel;
    public ObservableCollection<AdoWorkItem> WorkItems { get; } = new();

    private string apiKey = string.Empty;
    public string ApiKey
    {
        get => this.apiKey;
        set => this.SetProperty(ref this.apiKey, value, otherNames: [nameof(this.FinalApiKey), nameof(this.KeySourcePlaceholder)]);
    }

    public string EnvOpenAiKey => Environment.GetEnvironmentVariable(Strings.OpenAiKeyEnvironmentVariable) ?? string.Empty;
    public string EnvAzureKey => Environment.GetEnvironmentVariable(Strings.AzureKeyEnvironmentVariable) ?? string.Empty;
    public string EnvAzureEndpoint => Environment.GetEnvironmentVariable(Strings.AzureUrlEnvironmentVariable) ?? string.Empty;
    public string AzureDeployment => Environment.GetEnvironmentVariable(Strings.AzureDeploymentEnvironmentVariable) ?? Strings.AzureDeploymentDefault;

    public string FinalApiKey
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(this.ApiKey))
            {
                return this.ApiKey;
            }

            if (!string.IsNullOrEmpty(this.EnvAzureKey) && !string.IsNullOrEmpty(this.EnvAzureEndpoint))
            {
                return this.EnvAzureKey;
            }

            if (!string.IsNullOrEmpty(this.EnvOpenAiKey))
            {
                return this.EnvOpenAiKey;
            }

            return string.Empty;
        }
    }

    public string KeySourcePlaceholder
    {
        get
        {
            if (!string.IsNullOrEmpty(this.EnvAzureKey) && !string.IsNullOrEmpty(this.EnvAzureEndpoint))
            {
                return Strings.AzureKeyKnownPlaceholder;
            }

            if (!string.IsNullOrEmpty(this.EnvOpenAiKey))
            {
                return Strings.OpenAiKeyKnownPlaceholder;
            }

            return Strings.ApiKeyPlaceholder;
        }
    }

    private string searchText = string.Empty;
    public string SearchText
    {
        get => this.searchText;
        set => this.SetProperty(ref this.searchText, value);
    }

    private Command searchCommand;
    public ICommand SearchCommand => this.searchCommand ??= new Command(() => this.Search?.Invoke(this, EventArgs.Empty));
    public EventHandler Search;

    private string query = string.Empty;
    public string Query
    {
        get => this.query;
        set => this.SetProperty(ref this.query, value, otherNames: [nameof(this.HasQuery)]);
    }

    public bool HasQuery => !string.IsNullOrWhiteSpace(this.Query);

    private Command runQueryCommand;
    public ICommand RunQueryCommand => this.runQueryCommand ??= new Command(() => this.RunQuery?.Invoke(this, EventArgs.Empty));
    public EventHandler RunQuery;

    private Command workItemClickedCommand;
    public ICommand WorkItemClickedCommand => this.workItemClickedCommand ??= new Command(() => this.WorkItemClicked?.Invoke(this, EventArgs.Empty));
    public EventHandler WorkItemClicked;
}
