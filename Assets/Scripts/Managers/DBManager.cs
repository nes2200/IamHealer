using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DBManager : ManagerBase
{
    FirebaseAuth authentication;
    private FirebaseUser user;
    private DatabaseReference rootDB;

    protected override IEnumerator Onconnected(GameManager newManager)
    {
        //              의존성 검사         비동기 
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(InitializeFirebase);
        yield return null;   
    }

    protected override void OnDisconnected()
    {
       
    }

    void InitializeFirebase(Task<DependencyStatus> task)
    {
        if(task.Result == DependencyStatus.Available)
        {
            //인증용 인스턴스 가져오기
            authentication = FirebaseAuth.DefaultInstance;
            //인증을 하기 위해 필요한 "유저" 가져오기
            user = authentication.CurrentUser;
            //데이터 베이스에 가려면 데이터 베이스가 어디에 있는지 찾아갈 수 있어야 한다
            //데이터 베이스 참조(Reference)
            rootDB = FirebaseDatabase.DefaultInstance.RootReference;

            GuestLogin();
         
            Debug.Log("Firebase Initialize");
        }
        else
        {
            Debug.LogError($"Fail to Initialize Firebase : {task.Exception}");
        }
    }

    public TMPro.TMP_InputField nickNameInput;

    public void MakeUserData()
    {
        WriteData(MakeNewUserData(nickNameInput.text), "users", "userData", user.UserId);
    }

    public void GuestLogin()
    {
        //인증기가 없다면?
        if (authentication is null) return;
        //이미 로그인 되어있는지 체크하기
        if(user is not null)
        {
            Debug.LogError($"Login Failed : Already Has Login Data ({user.IsValid()}, {user.UserId})");
            WriteData(MakeNewUserData("용뼈"), "users", "userData", user.UserId);
            return;
        }
        //익명으로 로그인하기
        authentication.SignInAnonymouslyAsync().ContinueWithOnMainThread(OnLoginResult);
    }

    private void OnLoginResult(Task<AuthResult> task)
    {
        if(task.IsCanceled || task.IsFaulted)
        {
            Debug.LogError($"Failed to Sign in : {task.Exception}");
            return;
        }

        user = task.Result.User;
        Debug.Log($"Sign in Succeed : {user.UserId}");
    }

    [Serializable]
    public class UserData
    {
        public string nickname;
        public DateTime assignDate;
        public int userLeve;
        public int money;
        public int attendtime;
    }

    public UserData MakeNewUserData(string wantNickname) => new()
    {
        nickname    = wantNickname,
        assignDate  = DateTime.Now,
        userLeve    = 1,
        money       = 3000,
        attendtime  = 0
    };

    public void WriteData(object wantData, params string[] directory)
    {
        if (rootDB is null || wantData is null) return;

        //NoSQL은 JSON으로 저장한다
        string jsonData = JsonUtility.ToJson(wantData);
        //뿌리에서부터 시작
        DatabaseReference currentReference = rootDB;
        //디렉토리 하나하나 타고 내려가기
        foreach(string currentChild in directory)
        {
            currentReference = currentReference.Child(currentChild);
        }
        //최종 도착한 위치에 저장하기
        currentReference.SetRawJsonValueAsync(jsonData).ContinueWithOnMainThread(OnTaskResult);

        Dictionary<string, object> item = new()
        {
            { "name", "돌"},
            { "weight", .3 },
            { "price", 1}
        };


        //폴더를 따라 내려가는 것
        //제일 처음에 만든 reference가 바로 root폴더 => c드라이브다
        rootDB.Child("Items").Child("Misc").Child("Nature").Child("Stone").UpdateChildrenAsync(item).ContinueWithOnMainThread(OnTaskResult);
    }

    private void OnTaskResult(Task task)
    {
        if(task.IsCanceled || task.IsFaulted)
        {
            Debug.LogError(task.Exception);
        }
    }
}
