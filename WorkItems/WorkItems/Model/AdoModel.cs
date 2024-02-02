using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace WorkItems.Model;

[DebuggerDisplay("Account={CurrentAccount}, Connection={Connection}")]
internal sealed class AdoModel : PropertyNotifier, IDisposable
{
    public ObservableCollection<AdoAccount> Accounts { get; } = [];

    public void Dispose()
    {
        if (this.connection != null)
        {
            this.connection.Dispose();
            this.connection = null;
        }
    }

#pragma warning disable CA1822 // Mark members as static
    public void EnsureValid()
#pragma warning restore CA1822 // Mark members as static
    {
        // Nothing to ensure yet
    }

    private AdoConnection connection;
    [JsonIgnore]
    public AdoConnection Connection
    {
        get => this.connection;
        set => this.SetProperty(ref this.connection, value);
    }

    private AdoAccount currentAccount;
    [JsonIgnore]
    public AdoAccount CurrentAccount
    {
        get => this.currentAccount;
        set
        {
            if (this.SetProperty(ref this.currentAccount, value))
            {
                this.previousAccountName = this.CurrentAccountName;
                this.OnPropertyChanged(nameof(this.CurrentAccountName));
            }
        }
    }

    private string previousAccountName;
    [JsonProperty(Order = 1)]
    public string CurrentAccountName
    {
        get => this.CurrentAccount?.Name ?? this.previousAccountName;
        set => this.CurrentAccount = this.Accounts.FirstOrDefault(a => a.Name == value);
    }
}
