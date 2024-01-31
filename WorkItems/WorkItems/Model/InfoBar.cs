using System;
using System.Linq;
using WorkItems.Utility;

namespace WorkItems.Model;

public enum InfoLevel
{
    Message,
    Warning,
    Error,
}

public interface IInfoBar
{
    void SetError(Exception exception, string text = null);
    void SetInfo(InfoLevel level, string text, string details = null);
    void Clear();
}

public sealed class InfoBar : PropertyNotifier, IInfoBar
{
    public bool HasText => !string.IsNullOrEmpty(this.text);

    private InfoLevel errorLevel;
    public InfoLevel ErrorLevel
    {
        get => this.errorLevel;
        set => this.SetProperty(ref this.errorLevel, value);
    }

    private string text = string.Empty;
    public string Text
    {
        get => this.text;
        set
        {
            if (this.SetProperty(ref this.text, value ?? string.Empty))
            {
                this.OnPropertyChanged(nameof(this.HasText));
                this.OnPropertyChanged(nameof(this.Details));
            }
        }
    }

    private string details = string.Empty;
    public string Details
    {
        get => !string.IsNullOrEmpty(this.details) ? this.details : this.text;
        set
        {
            if (!string.Equals(this.details, value ?? string.Empty, StringComparison.Ordinal))
            {
                this.details = value ?? string.Empty;
                this.OnPropertyChanged(nameof(this.Details));
            }
        }
    }

    public void SetError(Exception exception, string text = null)
    {
        if (string.IsNullOrEmpty(text))
        {
            // Combine the messages of all aggregate and inner exceptions into a single string.
            text = exception.FlattenMessages();
        }

        exception = exception.Flatten().FirstOrDefault();

        // If the user canceled a task, they don't need to know about it
        if (exception is not OperationCanceledException)
        {
            this.SetInfo(InfoLevel.Error, text);
        }
    }

    public void SetInfo(InfoLevel level, string text, string details = null)
    {
        this.ErrorLevel = level;
        this.Text = text ?? string.Empty;
        this.Details = details ?? string.Empty;
    }

    public void Clear()
    {
        this.SetError(null);
    }
}
