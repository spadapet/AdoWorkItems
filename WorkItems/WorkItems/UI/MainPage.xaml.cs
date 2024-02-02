using Azure;
using Azure.AI.OpenAI;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorkItems.Model;
using WorkItems.Utility;

namespace WorkItems.UI;

internal partial class MainPage : ContentPage, IUpdatable
{
    public MainModel Model { get; }
    private Wrapper<CancellationTokenSource> ctsWrapper = new(null);

    public MainPage()
    {
        this.Model = new MainModel(App.Current.Model);
        this.Model.Search += this.OnSearch;
        this.Model.RunQuery += this.OnRunQuery;
        this.Model.WorkItemClicked += this.OnWorkItemClicked;
        this.InitializeComponent();
    }

    private void OnLoaded(object sender, EventArgs args)
    {
        this.StartUpdate();
    }

    public void StartUpdate()
    {
        TaskUtility.FileOnceAndForget(this.ctsWrapper, this.Model.AppModel.ProgressBar, this.Model.AppModel.InfoBar, async (work, cts) =>
        {
            await this.UpdateAsync(cts.Token);
        });
    }

    private async Task UpdateAsync(CancellationToken cancellationToken)
    {
        AdoModel ado = this.Model.AppModel.AdoModel;

        if (await AdoUtility.UpdateAccountsAsync(ado, Strings.DefaultAccountName, cancellationToken) is AdoAccount account)
        {
            _ = await AdoUtility.UpdateProjectsAsync(ado.Connection, account, Strings.DefaultProjectName, cancellationToken);
        }
    }

    private void OnSearch(object sender, EventArgs args)
    {
        TaskUtility.FileOnceAndForget(this.ctsWrapper, this.Model.AppModel.ProgressBar, this.Model.AppModel.InfoBar, async (work, cts) =>
        {
            await this.SearchAzureAsync(this.Model.SearchText, cts.Token);
        });
    }

    private async Task SearchAzureAsync(string searchText, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(searchText) ||
            string.IsNullOrWhiteSpace(this.Model.AzureDeployment) ||
            string.IsNullOrWhiteSpace(this.Model.EnvAzureEndpoint) ||
            string.IsNullOrWhiteSpace(this.Model.EnvAzureKey))
        {
            return;
        }

        ChatRequestMessage[] messages =
        [
            new ChatRequestSystemMessage(Strings.SystemContent),
            new ChatRequestUserMessage(string.Format(CultureInfo.CurrentCulture, Strings.UserContent, searchText)),
        ];

        ChatCompletionsOptions options = new(this.Model.AzureDeployment, messages);
        Uri endpoint = new(this.Model.EnvAzureEndpoint);
        AzureKeyCredential key = new(this.Model.EnvAzureKey);
        OpenAIClient client = new(endpoint, key);
        Response<ChatCompletions> response = await client.GetChatCompletionsAsync(options, cancellationToken);
        ChatCompletions completions = response?.Value;

        if (completions?.Choices?.FirstOrDefault()?.Message?.Content is string query && !string.IsNullOrWhiteSpace(query))
        {
            this.Model.Query = query;
            await this.RunQueryAsync(query, cancellationToken);
        }
    }

    private async Task RunQueryAsync(string query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return;
        }

        const string titleColumnName = "System.Title";
        const string editLinkName = "html";

        this.Model.WorkItems.Clear();

        AdoModel ado = this.Model.AppModel.AdoModel;
        WorkItemTrackingHttpClient client = await ado.Connection.Connect(ado.CurrentAccount).GetClientAsync<WorkItemTrackingHttpClient>(cancellationToken);
        Wiql wiql = new() { Query = query };
        WorkItemQueryResult result = await client.QueryByWiqlAsync(wiql, cancellationToken: cancellationToken);

        if (!result.WorkItems.Any())
        {
            return;
        }

        List<WorkItem> workItems = await client.GetWorkItemsAsync(result.WorkItems.Select(i => i.Id), [titleColumnName], expand: WorkItemExpand.Links, cancellationToken: cancellationToken);

        foreach (WorkItem workItem in workItems
            .Where(i => i.Id.HasValue && i.Fields.ContainsKey(titleColumnName) && i.Fields[titleColumnName] is string)
            .OrderBy(i => i.Id.Value))
        {
            AdoWorkItem model = new()
            {
                Id = workItem.Id.Value,
                Title = (string)workItem.Fields[titleColumnName],
                EditUrl = (workItem.Links.Links.ContainsKey(editLinkName) && workItem.Links.Links["html"] is ReferenceLink link) ? link.Href : string.Empty,
            };

            this.Model.WorkItems.Add(model);
        }
    }

    private void OnRunQuery(object sender, EventArgs args)
    {
        TaskUtility.FileOnceAndForget(this.ctsWrapper, this.Model.AppModel.ProgressBar, this.Model.AppModel.InfoBar, async (work, cts) =>
        {
            await this.RunQueryAsync(this.Model.Query, cts.Token);
        });
    }

    private void OnClickedCancel(object sender, EventArgs args)
    {
        this.ctsWrapper.Value?.Cancel();
    }

    private void OnWorkItemClicked(object sender, EventArgs args)
    {
        if (this.workItemsView.SelectedItem is AdoWorkItem item && !string.IsNullOrWhiteSpace(item.EditUrl))
        {
            TaskUtility.FileAndForget(() => Launcher.OpenAsync(item.EditUrl));
        }
    }
}
