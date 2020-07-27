using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[Serializable]
public class ItemChild
{
    public string id;
    public Texture2D texture;
    public string imageuri;
    public string name;
    public string label;
    public string description;
    public Asset asset;


    public ItemChild(string ID,Texture2D texture2D, string Name, string Label, string Description)
    {
        texture = texture2D;
        name = Name;
        label = Label;
        description = Description;
        id = ID;
    }

    public ItemChild(Texture2D texture2D, string Name, string Label, string Description, string ImageUri, string ID)
    {
        texture = texture2D;
        name = Name;
        label = Label;
        description = Description;
        imageuri = ImageUri;
        id = ID;
    }

    public ItemChild(Texture2D texture2D, string Name, string Label, string Description, string ImageUri, string ID,Asset assets)
    {
        texture = texture2D;
        name = Name;
        label = Label;
        description = Description;
        imageuri = ImageUri;
        id = ID;
        asset = assets;
    }

}


public class ContentScrollist : MonoBehaviour
{


    // this will serve as the itemchild class that will be used in other scenes once a child is clicked;
    public static ItemChild ItemChildtransfer;

    private ObjectPooler _pooler;
    
    // List to check if a child is added to the scroll view
    private List<string> scrollcheck;
    
    // list of Asset from the Database
    private List<Asset> assets;
    private List<ItemChild> itemChildren;
    
    
    // Tranform of the content GameObject
    public Transform ContentGameObject;
    public Transform Sizeitem;
    
    // Firebase Database reference
    private DatabaseReference assetReference;
    
    
    // Start is called before the first frame update
    void Start()
    {
        S("Start method first line");
        
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://augmentedestate.firebaseio.com/");
        
        
        assets = new List<Asset>();
        itemChildren = new List<ItemChild>();
        
        _pooler = ObjectPooler.instance;
        scrollcheck = new List<string>();

        assetReference = FirebaseDatabase.DefaultInstance.RootReference.Child(PathString.Assets);

        S("About to call RetrieveData StartCoroutine");
        
        // call the Retrieve method to fill the list with the asset
        StartCoroutine(RetrieveData(assetReference));

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChildrenClickEvent(ItemChild child)
    {

        ItemChildtransfer = child;

        SceneManager.LoadScene(PathString.Preview);
        

        //TODO: this will load a new scene and will transfer data to static Itemchild to be reference from the other scene
        //TODO: The EventHandler in ChildItem script will call this method
    }

    public void RefreshList()
    {
    
        //TODO: Call AddContnet to refresh the list;
        
        AddContent();
    }

    public void Additemchild(Asset asset)
    {
        //TODO: Add itemchild to a List of itemchild
        
        S($"Inside Additemchild: Adding assets {asset.name}");
        
        if (!CheckIfContain(assets,asset))
        {
            assets.Add(asset);
            
            S($"Added assets {asset.name}");


            StartCoroutine(downloadTexture(asset));



        }
        
        
    }


    private IEnumerator downloadTexture(Asset asset)
    {
        S($"Inside downloadTextture: Adding assets {asset.name}");
        
        Texture2D texture = new Texture2D(512,512);
        
        
        S($"Created Texture");

        //var handler = UnityWebRequestTexture.GetTexture(asset.images[0]).downloadHandler;

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(asset.images[0]);
        
       // yield return new WaitUntil(() => handler.isDone);

       yield return www.SendWebRequest();
        
        S($"Done with Handler in downloadTexture");

        if (www.isNetworkError || www.isHttpError)
        {
         
            S($"Something went wrong : {www.error}");
        }
        else
        {
            
            texture = ((DownloadHandlerTexture) www.downloadHandler).texture;
            // ---------------------- or -------------------------
           // texture = DownloadHandlerTexture.GetContent(www);
           
           S($"Texture is got");
        }
        
       

        //  byte[] image = handler.data;
        
       // S($"Got the byte image");

      //  texture.LoadRawTextureData(image);
        
        //S($"texture.LoadRawTextureData called");
        
        
        ItemChild itemChild = new ItemChild(texture,asset.name,asset.label,asset.description,asset.images[0],asset.id,asset);
        
        S($"downloadtexture : Adding Item {itemChild.name}");
        
        
        // TODO: check if the itemchild is contain in the Itemchildren. use a method to loop through the itemchildren and check


        if (!CheckIfItemContain(itemChildren,itemChild))
        {
            S($"Adding Item {itemChild.name}");
            itemChildren.Add(itemChild);
            S($"downloadtexture : Added Item {itemChild.name}");
            RefreshList();
        }
        

    }

    public void AddContent()
    {
        
        //TODO: Run a for load to Instantiate or retrive GameObject of content from Object pool and display to user
        S($"Inside AddContent");
        
        foreach (var child in itemChildren)
        {
            if (!scrollcheck.Contains(child.id))
            {

                GameObject content = _pooler.SpawnObject(PathString.AssetList);
                
                
                content.transform.SetParent(ContentGameObject,false);

                ChilditemScript childitemScript = content.GetComponent<ChilditemScript>();
                childitemScript.Setup(child,this);
                
                S($"Spawned object in AddContent");
                
                scrollcheck.Add(child.id);

            }
        }
        
        
        
        
    }
    
    
    public async Task<DataSnapshot> LoadData(DatabaseReference reference, int start , int end)
    {
        var dataSnapshot = await reference.GetValueAsync();
        
        if (!dataSnapshot.Exists)
        {
            return null;
        }

        return dataSnapshot;

    }


    public async Task<DataSnapshot> LoadDataAsset(DatabaseReference reference, Asset asset, int count)
    {
        string value = JsonUtility.ToJson(asset);
        
        var dataSnapshot = await reference.OrderByKey().StartAt(value, asset.id).LimitToFirst(count).GetValueAsync();
        
        if (!dataSnapshot.Exists)
        {
            return null;
        }

        return dataSnapshot;
    }

    private IEnumerator RetrieveData(DatabaseReference reference)
    {
        
        // TODO: This will be called in the start() method and again once the refresh button is clicked
        S("Inside RetrieveData");
        
        if (assets.Count > 0)
        {
            
            S("RD - asset count > 0");
            var datasnap = LoadDataAsset(reference, assets[assets.Count - 1], 10);
            
            yield return new WaitUntil(() => datasnap.IsCompleted);

            S("RD - ac>0 : Finised datasnapshot");
            
            DataSnapshot snapshot = datasnap.Result;

            S($"RD - ac>0 : got datasnapshot {snapshot.ToString()}");


            foreach (var datashot in snapshot.Children)
            {

                Asset asset = JsonUtility.FromJson<Asset>(datashot.GetRawJsonValue());

                Additemchild(asset);
                
                S($"RD - ac>0 : Adding assets {asset.name}");
            
            }
        }
        else
        {
            
            S("RD - asset count = 0");
            
            var datasnap = LoadData(reference,0,1);
            
            
            yield return new WaitUntil(() => datasnap.IsCompleted);

            S("RD - ac=0 : Finised Datasnapshot process");
            
            DataSnapshot snapshot = datasnap.Result;

            S($"RD - ac=0 : Got datasnapshot {snapshot.ToString()}");


            foreach (var datashot in snapshot.Children)
            {

                Asset asset = JsonUtility.FromJson<Asset>(datashot.GetRawJsonValue());

                Additemchild(asset);
                
                S($"RD - ac=0 : Adding assets {asset.name}");
            
            }

            
        }
        
        

        S($"Done with Retrieve Data");




    }


    private bool CheckIfContain(List<Asset> list, Asset asset )
    {
        bool check = false;


        foreach (var ast in list)
        {

            if (ast.id.Equals(asset.id))
            {
                check = true;
                break;
            }
            
            
        }

        return check;




    }
    
    private bool CheckIfItemContain(List<ItemChild> items , ItemChild child)
    {

        bool check = false;


        foreach (var item in items)
        {
            if (item.id.Equals(child.id))
            {
                check = true;
                break;
                
            }
        }


        return check;


    }

    void S(string msg)
    {
        CodelabUtils._ShowAndroidToastMessage(msg);
    }











}
