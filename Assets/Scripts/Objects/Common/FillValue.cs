using UnityEngine;

public delegate void FillValueChangeEvent();

[System.Serializable]
public struct FillValue
{
    [SerializeField] int _current;
    [SerializeField] int _max;
    int _min;

    public event FillValueChangeEvent OnChanged;

    public int Current
    {
        readonly get => _current;
        set 
        {
            _current = Mathf.Clamp(value, Min, Max);
            OnChanged?.Invoke();
        }
    }
    public int Min => _min;
    public int Max => _max;
    public float Percent => (float)Current / Max;

    public bool IsEmpyt => Current <= Min;
    public bool IsMax => Current >= Max;
    public bool IsUnderZero => Current <= 0;

    public FillValue(int current, int max, int min = 0)
    {
        _current = current;
        _min = min;
        _max = max;
        OnChanged = null;
    }

    public int IncreaseCurrent(int value) => Current += value;
    public int DecreaseCurrent(int value) => Current -= value;
    public int SetCurrent(int value) => Current = value;
    public int SetFull() => Current = Max;
    public int SetEmpty() => Current = Min;
    public float SetPercent(float value) => Mathf.CeilToInt(Mathf.Lerp(Min, Max, Mathf.Clamp(value, 0.0f, 1.0f)));
        
    public void SetMax(int value) { _max = value; Current = Current; }
    public void SetMin(int value) { _min = value; Current = Current; }
}
