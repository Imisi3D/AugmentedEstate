using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;

[Serializable]
public class User
{
    public string Name;
    public string Email;
    public string Company;
    public string UserID;
    
    public User(string userid ,string name, string email, string comapany)
    {
        UserID = userid;
        Name = name;
        Email = email;
        Company = comapany;
    }

    public string UserId
    {
        get => UserID;
        set => UserID = value;
    }

    public string Name1
    {
        get => Name;
        set => Name = value;
    }

    public string Email1
    {
        get => Email;
        set => Email = value;
    }

    public string Company1
    {
        get => Company;
        set => Company = value;
    }
}
