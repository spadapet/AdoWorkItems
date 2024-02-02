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
    private Wrapper<CancellationTokenSource> ctsWrapper = new(null);

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
        TaskUtility.FileOnceAndForget(this.ctsWrapper, async cts =>
        {
            this.Model.SetError(null);

            try
            {
                await this.UpdateAsync(cts.Token);
            }
            catch (Exception ex)
            {
                this.Model.SetError(ex);
                throw;
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
        this.ctsWrapper.Value?.Cancel();
    }

    private void OnRetryClicked(object sender, EventArgs args)
    {
        this.StartUpdate();
    }
}
