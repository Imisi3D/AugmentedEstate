using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RegisterManager : MonoBehaviour
{

    public Text name,email,password,company;
    private FirebaseAuth _auth;
    private DatabaseReference _reference;
    private FirebaseUser currentUser;


    public static void FireInitializer()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://augmentedestate.firebaseio.com/"); 
    }

    // Start is called before the first frame update
    void Start()
    {
       // Set up the Editor before calling into the realtime database
       FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://augmentedestate.firebaseio.com/");
       _reference = FirebaseDatabase.DefaultInstance.RootReference.Child(PathString.Users);

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void signIn()
    {
        SceneManager.LoadScene("Login");
    }

    public void signUp()
    {
        CodelabUtils._ShowAndroidToastMessage("SignUp pressed");
        
        CodelabUtils._ShowAndroidToastMessage("This are the entrys : "+name.text + " "+email.text+" "+password.text+" "+company.text);


        if (string.IsNullOrEmpty(name.text) && string.IsNullOrEmpty(email.text) && string.IsNullOrEmpty(password.text) && string.IsNullOrEmpty(company.text))
        {
            CodelabUtils._ShowAndroidToastMessage("Please fill in the missing entries");
        }
        else
        {
            StartCoroutine(Register(email.text, password.text));
        }
        
    }


    private IEnumerator Register(string email, string password)
    {
        var auth = FirebaseAuth.DefaultInstance;
        var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
        
        yield return new WaitUntil(() => registerTask.IsCompleted);

        if (registerTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register user with {registerTask.Exception}");
            CodelabUtils._ShowAndroidToastMessage($"Failed to register user with {registerTask.Exception}");
        }
        else
        {
            Debug.LogWarning($"Successfully Registered user {registerTask.Result.Email}");
            CodelabUtils._ShowAndroidToastMessage($"Successfully Registered user {registerTask.Result.Email}");
            
            currentUser = registerTask.Result;
            
            User user = new User(currentUser.UserId,name.text,email,company.text);
            string json = JsonUtility.ToJson(user);
            _reference.Child(currentUser.UserId).SetRawJsonValueAsync(json);
            CodelabUtils._ShowAndroidToastMessage("User saved to Database");

            StartCoroutine(SceneWaitLoader(3f));


        }

    }

    private IEnumerator SceneWaitLoader(float sec)
    {
        yield return new WaitForSeconds(sec);
        
        SceneManager.LoadScene("Login");
    }






}
