using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Analytics;
using Firebase.Auth;
using Firebase.Database;

#if PLATFORM_ANDROID

using UnityEngine.Android;

#endif

using UnityEngine.SceneManagement;
using UnityEngine.UI;




public class LoginManager : MonoBehaviour
{

    

    // Variable needed
    private FirebaseAuth _auth;
    public Text email;
    public Text password;
    private string _emailtext;
    private string _passwordtext;
    private FirebaseUser currentUser;
  


    // Start is called before the first frame update
    void Start()
    {
        
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        });
        
        /*#if PLATFORM_ANDROID

        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead) || !Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            
        }
        
        #endif*/
        
        _auth = FirebaseAuth.DefaultInstance;
        
        FirebaseAuth.DefaultInstance.StateChanged += HandleAuthStateChanged;
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public void signIn()
    {

        CodelabUtils._ShowAndroidToastMessage("SignIn clicked");

        _emailtext = email.text;
        _passwordtext = password.text;
        
        if (string.IsNullOrEmpty(_emailtext) && string.IsNullOrEmpty(_passwordtext))
        {
            CodelabUtils._ShowAndroidToastMessage("Please fill the Email and Password entry");
        }
        else
        {
            CodelabUtils._ShowAndroidToastMessage("This are the entered details: "+ _emailtext+" "+_passwordtext);
            StartCoroutine(Login(_emailtext, _passwordtext));

            
        }

        
    }

    private IEnumerator Login(string email, string password)
    {
        var auth = FirebaseAuth.DefaultInstance;
        var registerTask = auth.SignInWithEmailAndPasswordAsync(email, password);
        
        yield return new WaitUntil(() => registerTask.IsCompleted);

        if (registerTask.Exception != null)
        {
            Debug.LogWarning($"Failed to Login user with {registerTask.Exception}");
            CodelabUtils._ShowAndroidToastMessage($"Failed to Login user with {registerTask.Exception}");
        }
        else
        {
            Debug.LogWarning($"Successfully logged in user {registerTask.Result.Email}");
            CodelabUtils._ShowAndroidToastMessage($"Successfully logged in User {registerTask.Result.Email}");
            SceneManager.LoadScene("Main");
        }
        



    }

    public void singUp()
    {
        SceneManager.LoadScene("Register");
    }

    public void forgotPassword()
    {
        Debug.Log("Forgot Password is yet to be implemented");
    }
    
    
    private void HandleAuthStateChanged(object sender, EventArgs e)
    {
        CheckUser();
    }

    public void CheckUser()
    {
        
        var auth = FirebaseAuth.DefaultInstance;
        
        if (auth.CurrentUser != null)
        {
            SceneManager.LoadScene("Main");
        }
        
    }


    
}
