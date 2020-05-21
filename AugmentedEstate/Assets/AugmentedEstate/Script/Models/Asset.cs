using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Asset
{
    public string name;
    public string label;
    public string description;
    public List<String> images;
    public string id;

    public Asset(string Name, string Label, string Description, List<string> Images)
    {

        name = Name;
        label = Label;
        description = Description;
        images = Images;
    }

    public Asset(string Name, string Label, string ID, string Description,List<string> Images)
    {

        name = Name;
        label = Label;
        description = Description;
        id = ID;
        images = Images;

    }

    public string Id
    {
        get => id;
        set => id = value;
    }


    public string Name
    {
        get => name;
        set => name = value;
    }

    public string Label
    {
        get => label;
        set => label = value;
    }

    public string Description
    {
        get => description;
        set => description = value;
    }

    public List<string> Images
    {
        get => images;
        set => images = value;
    }
}
