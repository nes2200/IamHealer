using UnityEditor;
using UnityEngine;

public class UI_LoadingScreen : UI_ScreenBase, IProgress<int>, IStatus<string>
{
    public int Current { get; protected set; }
    public int Max { get; protected set; }
    public float Progress => (Max != 0) ? (float)Current / (float)Max : 0.0f;

    public int AddCurrent(int value) => Set(Current + value);

    public int AddMax(int value) => Set(Current, Max + value);

    public UnityEngine.UI.Slider progressBar;
    public TMPro.TextMeshProUGUI progressText;
    public TMPro.TextMeshProUGUI loadingText;

    public GameObject layoutOnComplete;
    public GameObject layoutOnLoading;

    // IStatus<T>
    public string SetCurrentStatus(string newText)
    {
        loadingText.SetText(newText);
        return newText;
    }

    public void SetComplete()
    {
        layoutOnComplete.SetActive(true);
        layoutOnLoading.SetActive(false);
    }

    public int Set(int newCurrent) 
    {
        Current = Mathf.Min(newCurrent, Max);
        progressBar.value = Progress;
        progressText.SetText($"{Progress * 100f : 0.00}%");
        return Current;
    }
    public int Set(int newCurrent, int newMax)
    {
        layoutOnComplete.SetActive(false);
        layoutOnLoading.SetActive(true);
        Max = newMax;
        return Set(newCurrent);
    }
}
