using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace WorkItems.Model;

[DebuggerDisplay("{Name,nq} {Id}")]
public sealed class AdoAccount : PropertyNotifier, IComparable, IComparable<AdoAccount>, IEquatable<AdoAccount>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Uri Uri { get; set; }

    public ObservableCollection<AdoProject> Projects { get; } = new();

    private AdoProject currentProject;
    [JsonIgnore]
    public AdoProject CurrentProject
    {
        get => this.currentProject;
        set
        {
            if (this.SetProperty(ref this.currentProject, value))
            {
                this.previousProjectName = this.CurrentProjectName;
                this.OnPropertyChanged(nameof(this.CurrentProjectName));
            }
        }
    }

    private string previousProjectName;
    [JsonProperty(Order = 1)]
    public string CurrentProjectName
    {
        get => this.CurrentProject?.Name ?? this.previousProjectName;
        set => this.CurrentProject = this.Projects.FirstOrDefault(p => p.Name == value);
    }

    public override string ToString()
    {
        return this.Name;
    }

    public override bool Equals(object obj)
    {
        return obj is AdoAccount other && this.Equals(other);
    }

    public bool Equals(AdoAccount other)
    {
        return string.Equals(this.Name, other.Name);
    }

    public override int GetHashCode()
    {
        return this.Name?.GetHashCode() ?? 0;
    }

    public int CompareTo(AdoAccount other)
    {
        return this.Name.CompareTo(other.Name);
    }

    public int CompareTo(object obj)
    {
        if (obj is not AdoAccount other)
        {
            throw new InvalidOperationException();
        }

        return this.CompareTo(other);
    }
}
