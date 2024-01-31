using System;
using WorkItems.Utility;

namespace WorkItems.Model;

public sealed class LoginModel : PropertyNotifier
{
    public AppModel AppModel { get; }
    public bool HasError => !string.IsNullOrEmpty(this.errorMessage);

    public LoginModel(AppModel appModel)
    {
        this.AppModel = appModel;
    }

    public void SetError(Exception ex)
    {
        this.ErrorMessage = ex.FlattenMessages();
    }

    private string errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => this.errorMessage;
        set
        {
            if (this.SetProperty(ref this.errorMessage, value ?? string.Empty))
            {
                this.OnPropertyChanged(nameof(this.HasError));
            }
        }
    }
}
