using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Diagnostics;

namespace WorkItems.Model;

[DebuggerDisplay("{UserName,nq}")]
public sealed class AdoConnection : IDisposable
{
    public string UserName { get; set; }
    public string AccessToken { get; set; }

    private string accountName;
    private VssConnection accountConnection;

    public void Dispose()
    {
        this.SetAccountConnection(null);
    }

    private void SetAccountConnection(VssConnection connection)
    {
        if (this.accountConnection != connection)
        {
            this.accountConnection?.Dispose();
            this.accountConnection = connection;
        }
    }

    internal VssConnection Connect(AdoAccount account)
    {
        if (this.accountConnection == null || this.accountName != account.Name)
        {
            this.accountName = account.Name;
            this.SetAccountConnection(this.Connect(account.Uri));
        }

        return this.accountConnection;
    }

    internal VssConnection Connect(Uri uri)
    {
        return new VssConnection(uri, new VssOAuthAccessTokenCredential(this.AccessToken));
    }
}
