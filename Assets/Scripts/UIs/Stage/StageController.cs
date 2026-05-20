using UnityEngine;

//스테이지의 현재 상태
public enum StageState
{
    Ready,
    Battle,
    Result
}

public delegate void StageStateChangeEvent(StageState oldState, StageState newState);

public class StageController : MonoBehaviour
{
    public static event StageStateChangeEvent OnStageStateChange;


}
