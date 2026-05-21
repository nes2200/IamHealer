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

    private StageState _currentState;
    public StageState CurrentState => _currentState;

    [SerializeField] TeamEliminateNotifier teamA;
    [SerializeField] TeamEliminateNotifier teamB;

    //스테이지 상태 변경
    public void ChangeState(StageState newState)
    {
        if (CurrentState == newState) return;

        //배틀이면 배틀로, 아니면 배틀 끝내기
        if(newState == StageState.Battle)
        {
            GameManager.StartBattle();
        }
        else
        {
            GameManager.EndBattle();
        }

        //바뀌었으니까 바뀐 상태로 바꿔주고 이벤트 발동
        StageState oldState = CurrentState;
        _currentState = newState;
        OnStageStateChange?.Invoke(oldState, newState);
    }

    public void StartBattle() => ChangeState(StageState.Battle);
    public void EndBattle() => ChangeState(StageState.Result);

    //Ready는 따로 안만듬?
    //Ready -> Battle -> Result는 일방향임. 
    //되돌아 간다는 것은 다시하기 등을 통해 씬을 새로 로드헀거나, 완전히 새로고침 했다는 것
    //그렇기에 따로 Ready를 만들지 않고, 나중에 씬 분리 과정에서 만드는게 더 좋을 것 같음
}