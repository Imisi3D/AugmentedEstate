using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using UnityEngine;

public class Controller : MonoBehaviour
{

    // GameObject reference of the various screens
    
    [SerializeField]private GameObject _assetList;
    [SerializeField]private GameObject _addasset;
    [SerializeField]private GameObject _userprofile;

    public static FirebaseUser currentUser;
    
    // Start is called before the first frame update
    void Start()
    {
        FirebaseAuth.DefaultInstance.StateChanged += HandleAuthStateChanged;
        CheckUser();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssetListClick()
    {
        _assetList.SetActive(true);
        _addasset.SetActive(false);
        _userprofile.SetActive(false);
    }
    
    public void AddAssetClick()
    {
        _addasset.SetActive(true);
        _assetList.SetActive(false);
        _userprofile.SetActive(false);
    }
    
    public void UserProfileClick()
    {
        _userprofile.SetActive(true);
        _assetList.SetActive(false);
        _addasset.SetActive(false);
    }





    private void HandleAuthStateChanged(object sender, EventArgs e)
    {
        CheckUser();
    }

    public static FirebaseUser CheckUser()
    {
        
        var auth = FirebaseAuth.DefaultInstance;
        
        if (auth.CurrentUser != null)
        {
            currentUser = auth.CurrentUser;
        }

        return currentUser;
    }
}
