using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using UnityEngine.UI;


public class ProfileScript : MonoBehaviour
{
    
    // UI Element Variable
    [SerializeField]private Text name;
    [SerializeField] private Text company;
    
    
    // Firebase Variables
    private FirebaseUser currentUser;
    private DatabaseReference userreference;
    private Coroutine _coroutine;
    private User user;
    

    // Start is called before the first frame update
    void Start()
    {
        
        //RegisterManager.FireInitializer();
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://augmentedestate.firebaseio.com/"); 
        
        var auth = FirebaseAuth.DefaultInstance;
        
        if (auth.CurrentUser != null)
        {
            currentUser = auth.CurrentUser;
            
            CodelabUtils._ShowAndroidToastMessage(currentUser.UserId);
        }
        

        CodelabUtils._ShowAndroidToastMessage(PathString.Users);
        userreference = FirebaseDatabase.DefaultInstance.RootReference.Child(PathString.Users).Child(currentUser.UserId);

        StartCoroutine(LoadUser(userreference));

        
        
        






    }

    private IEnumerator LoadUser(DatabaseReference reference)
    {
        

        var userasync = Load(reference);
        yield return new WaitUntil(() => userasync.IsCompleted);
        
        user = userasync.Result;
        
        CodelabUtils._ShowAndroidToastMessage($" normal {user.Name} or prop {user.Name1} and normal {user.Company} or prop {user.Company1}");

        name.text = user.Name;
        company.text = user.Company;

        //_coroutine = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async Task<User> Load(DatabaseReference reference)
    {
        var dataSnapshot = await reference.GetValueAsync();
        
        if (!dataSnapshot.Exists)
        {
            return null;
        }

        return JsonUtility.FromJson<User>(dataSnapshot.GetRawJsonValue());

    }

    public void SignOut()
    {
        var auth = FirebaseAuth.DefaultInstance;
        auth.SignOut();
    }
}
