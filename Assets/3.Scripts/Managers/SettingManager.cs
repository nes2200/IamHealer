using System.Collections;
using UnityEngine;

public class SettingManager : ManagerBase
{
    protected override IEnumerator Onconnected(GameManager newManager)
    {
        //스마트폰의 방향은 가로 세로 역가로 역세로
        Screen.autorotateToLandscapeLeft      = true; //카메라가 왼쪽
        Screen.autorotateToLandscapeRight     = true; //카메라가 오른쪽
        Screen.autorotateToPortrait           = false; //카메라가 위
        Screen.autorotateToPortraitUpsideDown = false; //카메라가 아래

        //이건 화면 고정
        //Screen.orientation = ScreenOrientation.LandscapeLeft;

        //게임하다 컷씬이 나오는데, 갑자기 화면이 꺼지면 안된다
        //스크린이 얼마나 오랫동안 터치가 안되면 꺼질지 설정하기
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        yield return null;
    }

    protected override void OnDisconnected()
    {
    }
}
