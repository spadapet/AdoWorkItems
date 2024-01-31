using System;

namespace WorkItems.Model;

public sealed class Settings : PropertyNotifier, ICopyFrom<Settings>, ICloneable
{
    object ICloneable.Clone()
    {
        return this.Clone();
    }

    public Settings Clone()
    {
        Settings clone = new();
        clone.CopyFrom(this);
        return clone;
    }

    public void CopyFrom(Settings source)
    {
        // Copy properties when they exist
    }
}
