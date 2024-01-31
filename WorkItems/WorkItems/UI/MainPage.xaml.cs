using Microsoft.Maui.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;
using WorkItems.Model;
using WorkItems.Utility;

namespace WorkItems.UI;

internal partial class MainPage : ContentPage, IUpdatable
{
    public MainModel Model { get; }
    private CancellationTokenSource cancellationTokenSource;

    public MainPage()
    {
        this.Model = new MainModel(App.Current.Model);
        this.InitializeComponent();
    }

    private void OnLoaded(object sender, EventArgs args)
    {
        this.StartUpdate();
    }

    public void StartUpdate()
    {
        if (this.cancellationTokenSource != null)
        {
            // Already updating
            return;
        }

        TaskUtility.FileAndForget(async () =>
        {
            this.cancellationTokenSource = new CancellationTokenSource();
            WorkData work = new("Loading builds...", this.cancellationTokenSource)
            {
                Progress = 1
            };

            using (this.Model.AppModel.ProgressBar.Begin(work))
            {
                try
                {
                    await this.UpdateAsync(this.cancellationTokenSource.Token);
                }
                catch (Exception ex)
                {
                    this.Model.AppModel.InfoBar.SetError(ex);
                    throw;
                }
                finally
                {
                    this.cancellationTokenSource = null;
                }
            }
        });
    }

    private async Task UpdateAsync(CancellationToken cancellationToken)
    {
        AdoModel ado = this.Model.AppModel.AdoModel;

        if (await AdoUtility.UpdateAccountsAsync(ado, Strings.DefaultAccountName, cancellationToken) is AdoAccount account &&
            await AdoUtility.UpdateProjectsAsync(ado.Connection, account, Strings.DefaultProjectName, cancellationToken) is AdoProject project)
        {
            // TODO
        }
    }

    private void OnClickCancel(object sender, EventArgs args)
    {
        this.cancellationTokenSource?.Cancel();
    }

    private void OnRestart(object sender, EventArgs args)
    {
        this.Model.AppModel.State = AppState.Loading;
    }
}
