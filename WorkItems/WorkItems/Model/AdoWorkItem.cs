using System;
using System.Diagnostics;

namespace WorkItems.Model;

[DebuggerDisplay("{Id}")]
internal sealed class AdoWorkItem : IComparable, IComparable<AdoWorkItem>, IEquatable<AdoWorkItem>
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string EditUrl { get; set; }

    public override string ToString()
    {
        return $"{this.Id} : {this.Title}";
    }

    public override bool Equals(object obj)
    {
        return obj is AdoWorkItem other && this.Equals(other);
    }

    public bool Equals(AdoWorkItem other)
    {
        return this.Id == other.Id;
    }

    public override int GetHashCode()
    {
        return this.Id;
    }

    public int CompareTo(AdoWorkItem other)
    {
        return this.Id.CompareTo(other.Id);
    }

    public int CompareTo(object obj)
    {
        if (obj is not AdoWorkItem other)
        {
            throw new InvalidOperationException();
        }

        return this.CompareTo(other);
    }
}
