using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WorkItems.Model;

internal interface IProgressBar
{
    IDisposable Begin(WorkData work);
    void Cancel();
}

internal sealed class ProgressBar : PropertyNotifier, IProgressBar
{
    private readonly List<TaskDataWrapper> tasks;

    private sealed class TaskDataWrapper : IDisposable
    {
        public ProgressBar Owner { get; set; }
        public WorkData Work { get; set; }

        public void Dispose()
        {
            if (this.Owner is ProgressBar owner)
            {
                this.Owner = null;
                owner.RemoveTask(this, cancel: false);
            }
        }

        public void Cancel()
        {
            if (this.Owner is ProgressBar owner)
            {
                this.Owner = null;
                owner.RemoveTask(this, cancel: true);
            }
        }
    }

    public ProgressBar()
    {
        this.tasks = new List<TaskDataWrapper>();
    }

    public bool IsLoading => this.tasks.Count > 0;
    public string LoadingText => this.IsLoading ? this.tasks[^1].Work.Text : string.Empty;
    public double Progress => this.IsLoading ? this.tasks[^1].Work.Progress : 0.0;
    public bool IsIndeterminate => this.IsLoading && this.Progress == 0.0;

    public IDisposable Begin(WorkData taskData)
    {
        TaskDataWrapper info = new()
        {
            Work = taskData
        };

        this.PushTask(info);
        return info;
    }

    private void OnTaskPropertyChanged(object sender, PropertyChangedEventArgs args)
    {
        bool any = string.IsNullOrEmpty(args.PropertyName);

        if (any || args.PropertyName == nameof(WorkData.Text))
        {
            this.OnPropertyChanged(nameof(this.LoadingText));
        }

        if (any || args.PropertyName == nameof(WorkData.Progress))
        {
            this.OnPropertyChanged(nameof(this.Progress));
            this.OnPropertyChanged(nameof(this.IsIndeterminate));
        }
    }

    private void PushTask(TaskDataWrapper info)
    {
        info.Owner = this;
        info.Work.PropertyChanged += this.OnTaskPropertyChanged;
        this.tasks.Add(info);

        this.OnPropertyChanged(nameof(this.LoadingText));
        this.OnPropertyChanged(nameof(this.Progress));
        this.OnPropertyChanged(nameof(this.IsIndeterminate));

        if (this.tasks.Count == 1)
        {
            this.OnPropertyChanged(nameof(this.IsLoading));
        }
    }

    private void RemoveTask(TaskDataWrapper info, bool cancel)
    {
        int index = this.tasks.IndexOf(info);
        if (index >= 0)
        {
            this.tasks.RemoveAt(index);
            info.Work.PropertyChanged -= this.OnTaskPropertyChanged;

            if (index == this.tasks.Count)
            {
                this.OnPropertyChanged(nameof(this.LoadingText));
                this.OnPropertyChanged(nameof(this.Progress));
                this.OnPropertyChanged(nameof(this.IsIndeterminate));
            }

            if (this.tasks.Count == 0)
            {
                this.OnPropertyChanged(nameof(this.IsLoading));
            }

            if (cancel)
            {
                info.Work.Cancel();
            }
        }
    }

    public void Cancel()
    {
        foreach (TaskDataWrapper info in this.tasks.ToArray())
        {
            info.Cancel();
        }
    }
}
