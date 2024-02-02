namespace WorkItems.Model;

internal interface ICopyFrom<T>
{
    void CopyFrom(T other);
}
