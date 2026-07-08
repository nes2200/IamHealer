#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json; 
using System.Collections.Generic;
using Unity.VisualScripting;

public class SceneScannerWindow : EditorWindow
{
    private string inputFileName = "FileName_Default";

    private string calculatedSubPath = "1.Datas/Origin/Stages/Globals";

    //저장할 데이터 전체를 담은 그릇
    [System.Serializable]
    public class SceneSaveData
    {
        public List<StageObject> probs = new List<StageObject>();
    }
    //Vectro3를 직렬화하여 저장할 컨테이너
    [System.Serializable]
    public struct SerializableVector3
    {
        public float x, y, z;

        //변환이 쉽게 하기 위한 생성자
        public SerializableVector3(Vector3 v3)
        {
            x = v3.x;
            y = v3.y;
            z = v3.z;
        }

        //형변환을 쉽게 하기 위한 연산자
        public static implicit operator SerializableVector3(Vector3 v3) => new SerializableVector3(v3);
        public static implicit operator Vector3(SerializableVector3 v3) => new Vector3(v3.x, v3.y, v3.z);
    }
    //저장할 데이터에 대한 정보
    [System.Serializable]
    public class StageObject
    {
        public string name;
        public string prefabName;
        public string parentName;
        public SerializableVector3 position;
        public SerializableVector3 rotation;
        public SerializableVector3 scale;
    }

    //유니티 상단 바에 메뉴 추가
    [MenuItem("Tools/Scene Scanner")]
    public static void ShowWindow()
    {
        GetWindow<SceneScannerWindow>("Scene Scanner");
    }

    //그리기
    private void OnGUI()
    {
        //헤더
        GUILayout.Label("Scene Object Scanner", EditorStyles.boldLabel);

        GUILayout.Space(10); //여백

        //파일 이름 적는 곳
        GUILayout.Label("File Name Here (No Extensions)");
        inputFileName = GUILayout.TextField(inputFileName);

        GUILayout.Space(10);

        //유니티 내장 폴더 선택 필드
        GUILayout.Label("Target Save Folder");

        //유니티 os 내장 폴더 탐색기를 띄울 버튼
        if(GUILayout.Button("Choose Folder..", GUILayout.Width(120)))
        {
            //프로젝트의 Assets 폴더 기준 절대 경로 추출
            string defaultPath = Path.Combine(Application.dataPath, calculatedSubPath);
            if (!Directory.Exists(defaultPath)) defaultPath = Application.dataPath;

            //폴더 선택창 팝업
            string selectedPath = EditorUtility.OpenFolderPanel("저장할 폴더 선택", defaultPath, "");

            if (!string.IsNullOrEmpty(selectedPath))
            {
                //선택한 경로가 현재 프로젝트 안의 Asset 폴더인지 검증
                if (selectedPath.StartsWith(Application.dataPath))
                {
                    if(selectedPath == Application.dataPath)
                    {
                        calculatedSubPath = "";
                    }
                    else
                    {
                        calculatedSubPath = selectedPath.Substring(Application.dataPath.Length + 1);
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("경고", "현재 프로젝트의 Asset 폴더 내부에서만 선택 가능", "확인");
                }
            }
        }

        //계산되어 적용될 경로를 화면에 가이드라인으로 표시
        EditorGUILayout.HelpBox($"저장 위치 : Asset/{calculatedSubPath}", MessageType.Info);

        GUILayout.Space(20); 

        //실행 버튼
        if (GUILayout.Button("Scan Scene and Save JSON", GUILayout.Height(40)))
        {
            //빈칸 있는지 방어 함수 추가
            if(string.IsNullOrEmpty(inputFileName))
            {
                EditorUtility.DisplayDialog("경고", "파일 이름과 경로가 불확실함", "확인");
                return;
            }

            ExecuteScan(inputFileName, calculatedSubPath);
        }
    }

    private void ExecuteScan(string fileName, string subPath)
    {
        GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        SceneSaveData saveData = new SceneSaveData();

        //필터링된 오브젝트만 담을 리스트
        List<GameObject> targetObjects = new List<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            //예외 처리
            if (obj.name == "Terrain" || obj.name == "CrossLine")
            {
                targetObjects.Add(obj);
                continue;
            }

            //부모 오브젝트가 있는 경우에만 검사
            if (obj.transform.parent != null)
            {
                string parentName = obj.transform.parent.name;

                if (parentName == "Probs" || parentName == "TeamB")
                {
                    targetObjects.Add(obj);
                    continue;
                }
            }
        }

        foreach(GameObject obj in targetObjects)
        {
            //부모가 있으면 부모 이름을, 없으면 none을
            string registeredParent = obj.transform.parent != null ? obj.transform.parent.name : "None";

            GameObject originalPrefab = PrefabUtility.GetCorrespondingObjectFromSource(obj);
            string sourcePrefabName = originalPrefab ? originalPrefab.name : obj.name;

            StageObject data = new StageObject
            {
                name = obj.name,
                prefabName = sourcePrefabName,
                parentName = registeredParent,
                position = obj.transform.localPosition,
                rotation = obj.transform.eulerAngles,
                scale = obj.transform.localScale
            };
            saveData.probs.Add(data);
        }

        string jsonResult = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        string directoryPath = Path.Combine(Application.dataPath, subPath);

        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
        if (!fileName.EndsWith(".json")) fileName += ".json";

        string finalPath = Path.Combine(directoryPath, fileName);
        File.WriteAllText(finalPath, jsonResult);

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("저장 완료", $"성공적으로 스테이지 파일 생성 \n경로 : Asset/{subPath}/{fileName}", "확인");
    }
}
#endif