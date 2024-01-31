using Microsoft.Maui.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;
using WorkItems.Model;
using WorkItems.Utility;

namespace WorkItems.UI;

internal partial class LoginPage : ContentPage, IUpdatable
{
    public LoginModel Model { get; }
    private CancellationTokenSource cancellationTokenSource;

    public LoginPage()
    {
        this.Model = new LoginModel(App.Current.Model);
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
            this.Model.SetError(null);

            try
            {
                await this.UpdateAsync(this.cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                this.Model.SetError(ex);
                throw;
            }
            finally
            {
                this.cancellationTokenSource = null;
            }
        });
    }

    private async Task UpdateAsync(CancellationToken cancellationToken)
    {
        AdoModel ado = this.Model.AppModel.AdoModel;
        ado.Connection = await AdoConnectionUtility.GetConnectionAsync(cancellationToken);

        this.Model.AppModel.State = AppState.Shell;
    }

    private void OnCancelClicked(object sender, EventArgs args)
    {
        this.cancellationTokenSource?.Cancel();
    }

    private void OnRetryClicked(object sender, EventArgs args)
    {
        this.StartUpdate();
    }
}
