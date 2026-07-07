#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Collections.Generic;

public class SceneScannerWindow : EditorWindow
{
    //저장할 데이터 전체를 담은 그릇
    [System.Serializable]
    public class SceneSaveData
    {
        public List<ProbData> probs = new List<ProbData>();
    }
    //저장할 데이터에 대한 정보
    [System.Serializable]
    public class ProbData
    {
        public string name;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
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
        GUILayout.Label("Scene Object Scanner", EditorStyles.boldLabel);
        if (GUILayout.Button("Scan Scene and Save JSON", GUILayout.Height(40)))
        {
            ExecuteScan();
        }
    }

    private void ExecuteScan()
    {
        GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        SceneSaveData saveData = new SceneSaveData();

        foreach(GameObject obj in allObjects)
        {
            if (obj.name == "Main Camera" || obj.name == "Directional Light") continue;

            ProbData data = new ProbData();


        } 

    }

}
#endif