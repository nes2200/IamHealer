using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class StageLoadManager : ManagerBase
{
    protected override IEnumerator Onconnected(GameManager newManager)
    {
        yield return null;
    }

    protected override void OnDisconnected()
    {

    }

    public void LoadStage(TextAsset stageData)
    {
        ExecuteLoad(stageData);
        //카메라 위치 초기화
        GameManager.Camera.SetCameraDefaultPosition();
    }

    private void ExecuteLoad(TextAsset stageData)
    {
        //제이슨 파일 읽고 역직렬화
        string jsonContent = stageData.text;
        SceneSaveData loadData = JsonConvert.DeserializeObject<SceneSaveData>(jsonContent);

        if (loadData is null || loadData.probs is null)
        {
            EditorUtility.DisplayDialog("에러", "로드 데이터가 올바르지 않거나 비어있음", "확인");
            return;
        }

        //씬에 있는 주요 부모/관리자를 미리 검색하여 등록
        Dictionary<string, GameObject> parentContainer = new Dictionary<string, GameObject>();
        string[] keyParent = { "Probs", "TeamB", "Terrain", "Floor" };
        foreach (string parentName in keyParent)
        {
            GameObject found = GameObject.Find(parentName);
            if (found is not null)
            {
                parentContainer.Add(parentName, found);
            }
        }

        //기존 자식 및 배치 오브젝트 청소
        //Probs, TeamB 아래 있는 기존 자식들을 일괄 제거
        foreach (var container in parentContainer)
        {
            //Terrain이나 Floor를 지우면 안되니까 자식만 특정하기
            if (container.Key == "Probs" || container.Key == "TeamB")
            {
                List<GameObject> childrenToDestroy = new List<GameObject>();
                foreach (Transform child in container.Value.transform)
                {
                    childrenToDestroy.Add(child.gameObject);
                }
            }
            else if (container.Key == "Terrain")
            {
                Transform crossLine = container.Value.transform.Find("CrossLine");
            }
        }

        //데이터를 기반으로 에셋 폴더 내 프리팹을 검색하여 스폰 및 위치 복구
        foreach (StageObject data in loadData.probs)
        {
            GameObject spawnObject = null;

            //프로젝트 내 프리팹 검색
            string[] guids = AssetDatabase.FindAssets($"{data.prefabName} t:Prefab");
            GameObject prefabAsset = null;
            if (guids.Length > 0)
            {
                //정확히 일치하는 프리팹 찾기
                foreach (string guid in guids)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    if (obj && obj.name == data.prefabName)
                    {
                        prefabAsset = obj;
                        break;
                    }
                }
            }

            //프리팹을 찾았다면 Prefab 연결을 유지하며 스폰
            if (prefabAsset)
            {
                spawnObject = (GameObject)PrefabUtility.InstantiatePrefab(prefabAsset);
            }
            else
            {
                //프리팹을 못찾은 경우 기본 빈 오브젝트로 임시 생성하여 유실을 방지
                spawnObject = new GameObject(data.name);
                Debug.LogWarning($"[SceneScanner] '{data.prefabName}' 프리팹을 찾을 수 없어 기본 오브젝트로 대체됩니다;");
            }

            if (spawnObject)
            {
                spawnObject.name = data.name;

                //부모 관계 복구
                if (data.parentName != "None" && parentContainer.ContainsKey(data.parentName))
                {
                    spawnObject.transform.SetParent(parentContainer[data.parentName].transform);
                }
                else if (data.parentName == "Terrain" && data.name == "CrossLine")
                {
                    // Terrain의 자식인 CrossLine 예외 부모 설정
                    if (parentContainer.ContainsKey("Terrain"))
                    {
                        spawnObject.transform.SetParent(parentContainer["Terrain"].transform);
                    }
                }

                // 트랜스폼 복구 (Local 값 적용)
                spawnObject.transform.localPosition = data.position;
                spawnObject.transform.localScale = data.scale;
                spawnObject.transform.localRotation = data.rotation;
            }
        }

        //Probs 폴더에서 자식들 세팅하기
        if (parentContainer.ContainsKey("Probs"))
        {
            foreach (Transform child in parentContainer["Probs"].transform)
            {
                GameObject probObj = child.gameObject;

                //자신과 자식들의 모든 상태 변경 
                SetChildsStaticAndLayer(probObj, true, 9);

                NavMeshObstacle navObs = probObj.GetComponent<NavMeshObstacle>();
                if (navObs == null)
                {
                    // Undo 지원용 컴포넌트 추가 기능 사용
                    navObs = Undo.AddComponent<NavMeshObstacle>(probObj);
                }
                if (navObs)
                {
                    navObs.carving = true;
                }
            }
        }

        // 2. Terrain의 NavMesh 베이크(Bake) 하기
        if (parentContainer.ContainsKey("Terrain"))
        {
            NavMeshSurface navSurface = parentContainer["Terrain"].GetComponent<NavMeshSurface>();
            if (navSurface != null)
            {
                // 실제 NavMesh 빌드(Bake) 실행!
                navSurface.BuildNavMesh();

                // 에디터 씬 뷰 갱신 유도
                SceneView.RepaintAll();
            }
            else
            {
                Debug.LogWarning("[SceneScanner] Terrain 오브젝트에서 NavMeshSurface 컴포넌트를 찾을 수 없습니다.");
            }
        }

        //유닛들의 내부 참조 해주기
        Transform teamA = GameObject.Find("TeamA").transform;
        foreach (Transform unit in parentContainer["TeamB"].transform)
        {
            TargetingModule targetModule = unit.GetComponent<TargetingModule>();
            if (targetModule)
            {
                targetModule.SetHostileGroupParents(teamA);
            }
        }
    }

    private void SetChildsStaticAndLayer(GameObject obj, bool isStatic, int layer)
    {
        if (obj == null) return;

        obj.isStatic = isStatic;
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetChildsStaticAndLayer(child.gameObject, isStatic, layer);
        }
    }
}
