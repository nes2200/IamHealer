using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : ManagerBase
{
    protected override IEnumerator Onconnected(GameManager newManager)
    {
        yield return null;
    }

    protected override void OnDisconnected()
    {

    }

    public void LoadSceneAndSetup(string sceneName)
    {
        StartCoroutine(CoReloadSceneAndSetup(sceneName));
    }

    private IEnumerator CoReloadSceneAndSetup(string sceneName)
    {
        UIManager.ClaimOpenScreen(UIType.Stage, ScreenChangeType.SlideChanger);

        //해당 씬 로드 확인
        Scene targetScene = SceneManager.GetSceneByName(sceneName);
        if (targetScene.isLoaded)
        {
            //기존 씬 언로드 작업
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(sceneName);

            //언로드 완료까지 대기
            while(unloadOperation is null && !unloadOperation.isDone)
            {
                yield return null;
            }
        }

        //새 씬을 비동기로 불러오기
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!loadOperation.isDone)
        {
            yield return null;
        }

        //로드하려느 씬은 게임 오브젝트들이 올라가있는 씬이기에 액티브 씬으로 지정해주기
        Scene newScene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(newScene);

        //카메라 위치 초기화
        GameManager.Camera.SetCameraDefaultPosition();

        yield return null;
    }

}
