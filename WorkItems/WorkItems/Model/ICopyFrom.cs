namespace WorkItems.Model;

public interface ICopyFrom<T>
{
    void CopyFrom(T other);
}
