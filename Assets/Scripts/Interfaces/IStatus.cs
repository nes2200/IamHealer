using UnityEngine;

public interface IStatus<T>
{
    public T SetCurrentStatus(T value);
}
