using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WorkItems.Model;

/// <summary>
/// Base class for any view model
/// </summary>
public abstract class PropertyNotifier : INotifyPropertyChanging, INotifyPropertyChanged
{
    private event PropertyChangingEventHandler propertyChanging;
    private event PropertyChangedEventHandler propertyChanged;

    protected void OnPropertiesChanging()
    {
        this.OnPropertyChanging(null);
    }

    protected void OnPropertiesChanged()
    {
        this.OnPropertyChanged(null);
    }

    protected void OnPropertyChanging(string name)
    {
        this.propertyChanging?.Invoke(this, new PropertyChangingEventArgs(name));
    }

    protected void OnPropertyChanged(string name)
    {
        this.propertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    protected bool SetProperty<T>(ref T property, T value, [CallerMemberName] string name = null)
    {
        if (EqualityComparer<T>.Default.Equals(property, value))
        {
            return false;
        }

        if (name != null)
        {
            this.OnPropertyChanging(name);
        }

        property = value;

        if (name != null)
        {
            this.OnPropertyChanged(name);
        }

        return true;
    }

    public event PropertyChangingEventHandler PropertyChanging
    {
        add => this.propertyChanging += value;
        remove => this.propertyChanging -= value;
    }

    public event PropertyChangedEventHandler PropertyChanged
    {
        add => this.propertyChanged += value;
        remove => this.propertyChanged -= value;
    }
}
