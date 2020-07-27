using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Storage;
using Firebase.Unity.Editor;
using UnityEngine.UI;


[System.Serializable]
public class Item
{
    public Texture2D image;
    public string id;

    public Item(Texture2D texture, string ID)
    {
        image = texture;
        id = ID;
        
    }
}

public class ContentUIScript : MonoBehaviour
{
    public List<Item> items;
    public Transform contenttranform;
    public GameObject prefab;
    private ObjectPooler Pooler;
    private List<string> checkstring;
    private List<string> imageURLs;
    
    // the UI elements
    public Text name;
    public Text label;
    public Text description;


    // firebase variables
    private StorageReference storage;
    private DatabaseReference database;


    // Start is called before the first frame update
    void Start()
    {
        





        // Set up the Editor before calling into the realtime database
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://augmentedestate.firebaseio.com/");
        
        checkstring = new List<string>();
        imageURLs = new List<string>();

        Pooler = ObjectPooler.instance;
        
        
        FirebaseStorage firebaseStorage = FirebaseStorage.DefaultInstance;
        storage = firebaseStorage.GetReferenceFromUrl("gs://augmentedestate.appspot.com");


        database = FirebaseDatabase.DefaultInstance.RootReference.Child(PathString.Assets);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RetrieveImage()
    {

        if (NativeGallery.CheckPermission() != NativeGallery.Permission.Granted)
        {
            NativeGallery.Permission permission =  NativeGallery.RequestPermission();
            
            CodelabUtils._ShowAndroidToastMessage("Please Grant the permission before you continue");
            
        }
        //else
        //{
            NativeGallery.Permission permissionr =
                NativeGallery.GetImageFromGallery((path) =>
                {
                    if (path != null)
                    {
                        CodelabUtils._ShowAndroidToastMessage($"Image path {path}");
                        // Create Texture from selected image
                        Texture2D texture = NativeGallery.LoadImageAtPath(path, 512);

                        if (texture == null)
                        {
                            CodelabUtils._ShowAndroidToastMessage($"Could not load texturee from {path}");
                            CodelabUtils._ShowAndroidToastMessage($"Could not load texturee from {path}");
                            return;
                        }
                        
                        // add to the list of texture item
                        AddItem(new Item(texture,path));
                        
                        refreshList();

                        /*StartCoroutine(StoreImages(new Item(texture, path),
                            path.Substring(path.Length - 10, path.Length)));*/




                    }


                }, "Select an Image", "image/*");
        //}



    }

    public void refreshList()
    {
        //TODO: Refresh the Horizontal scroll view to reflect the added Image
        //RemoveImage();
        AddImage();
    }


    public GameObject SpawnImageitem()
    {

        GameObject imageobj = (GameObject)Instantiate(prefab,contenttranform);
        return imageobj;
    }

    public void AddItem(Item item)
    {
        items.Add(item);

    }

    public void AddImage()
    {

        for (int i = 0; i < items.Count; i++)
        {

            Item item = items[i];

            if (!checkstring.Contains(item.id))
            {
                //GameObject imageobj = SpawnImageitem();
                GameObject imageobj = Pooler.SpawnObject(PathString.AddAsset);
            
                // set the transform of the child to parent content
                imageobj.transform.SetParent(contenttranform,false);

                ImagelistScript imageitem = imageobj.GetComponent<ImagelistScript>();
                imageitem.Setup(item, this);
                
                checkstring.Add(item.id);
                
            }


           



        }
        
    }


    public void RemoveItem(Item itemToRemove)
    {

        for (int i = items.Count -1; i > 0; i--)
        {
            if (items[i] == itemToRemove)
            {
                items.RemoveAt(i);
            }
            
        }
    }

    public void RemoveImage()
    {
        while (contenttranform.childCount > 0)
        {
            GameObject toremove = contenttranform.GetChild(0).gameObject;
            Pooler.RemoveObject(PathString.AddAsset,toremove);
            
            
        }
        
    } 
    
    public void RemoveImages()
    {
        while (contenttranform.childCount > 0)
        {
            GameObject toremove = contenttranform.GetChild(0).gameObject;
            Destroy(toremove);
            
            
        }
        
    }

    public void Post()
    {
        

        /*string namestring = name.text;
        string labelstring = label.text;
        string describestring = description.text;*/

        /*Item item = items[0];

        StartCoroutine(StoreImages(item, "test"));*/


        StartCoroutine(StoreImages(items, name.text));


        /*if (items.Count > 0)
        {

            foreach (Item item in items)
            {
                StartCoroutine(StoreImages(storage, item, namestring));
            }
        }*/


        /*Asset asset = new Asset(namestring,labelstring,describestring,imageURLs);
        string json = JsonUtility.ToJson(asset);
        database.Child(namestring).SetRawJsonValueAsync(json);
        
        CodelabUtils._ShowAndroidToastMessage("Successfully uploaded the Asset");

        
        // clear all the data collected
        name.text = " ";
        label.text = " ";
        description.text = " ";
        
        items.Clear();
        //RemoveImages();
        
        CodelabUtils._ShowAndroidToastMessage("Cleared all data");*/



    }
    


    private IEnumerator StoreImages(StorageReference storageReference,Item item, string name)
    {

        String path = item.id;
        string id = path.Substring(path.LastIndexOf("/", StringComparison.Ordinal), path.Length);
        StorageReference reference = storageReference.Child(PathString.Images).Child(name).Child(id);

        var storagetask = reference.PutFileAsync(path);
        
        yield return new WaitUntil(() => storagetask.IsCompleted);

        CodelabUtils._ShowAndroidToastMessage("Upload completed Successfully");
        
        
        var uritask = storagetask.Result.Reference.GetDownloadUrlAsync();
                
        yield return new WaitUntil(() => uritask.IsCompleted); 
        
        String uri = uritask.Result.ToString();
        imageURLs.Add(uri); 
        CodelabUtils._ShowAndroidToastMessage($"Successfully stored Uri to list : {uri}");
        
        //StartCoroutine(GetFileUri(storagetask.Result));



    }
    
    private IEnumerator StoreImages(Item item, string namee)
    {
        
        String path = item.id;
        string id = path; //path.Substring(path.LastIndexOf("/", StringComparison.Ordinal), path.Length);
        CodelabUtils._ShowAndroidToastMessage($"The ID : {id}");
        
        var storage = FirebaseStorage.DefaultInstance;
        var referenced = storage.GetReferenceFromUrl("gs://augmentedestate.appspot.com");//storage.GetReference($"/{PathString.Assets}/{namee}/{id}");
        var reference = referenced.Child(PathString.Assets).Child(namee).Child(id);
        
        CodelabUtils._ShowAndroidToastMessage("Storage reference got");
        
        //var bytes = item.image.EncodeToPNG();
        CodelabUtils._ShowAndroidToastMessage("byte encoded");

        //var uploadTask = reference.PutBytesAsync(bytes);
        
        Stream stream = new FileStream(path,FileMode.Open);
        
        //var uploadTask = reference.PutFileAsync(path);

        var uploadTask = reference.PutStreamAsync(stream);
        
        yield return new WaitUntil(() => uploadTask.IsCompleted);
        
        CodelabUtils._ShowAndroidToastMessage("Image uploaded successfully");
        
        
        // check errors

        if (uploadTask.Exception != null)
        {
            CodelabUtils._ShowAndroidToastMessage($"Failed to upload image {uploadTask.Exception}");
            yield break;
        }
        
        
        // get the download uri

        var getUriTask = reference.GetDownloadUrlAsync();
        
        yield return new WaitUntil(() => getUriTask.IsCompleted);
        
        if (getUriTask.Exception != null)
        {
            CodelabUtils._ShowAndroidToastMessage($"Failed to upload image {getUriTask.Exception}");
            yield break;
        }
        
        String uri = getUriTask.Result.ToString();
        imageURLs.Add(uri); 
        CodelabUtils._ShowAndroidToastMessage($"Successfully stored Uri to list : {uri}");
        
        string namestring = name.text;
        string labelstring = label.text;
        string describestring = description.text;
        
        
         Asset asset = new Asset(namestring,labelstring,describestring,imageURLs);
            string json = JsonUtility.ToJson(asset);
            database.Child(namestring).SetRawJsonValueAsync(json);
            
            CodelabUtils._ShowAndroidToastMessage("Successfully uploaded the Asset");
    
            
            // clear all the data collected
            name.text = " ";
            label.text = " ";
            description.text = " ";
            
            items.Clear();
            //RemoveImages();
            
            CodelabUtils._ShowAndroidToastMessage("Cleared all data");

        




    }



    private IEnumerator StoreImages(List<Item> items, string namee)
    {

        foreach (Item item in items)
        {
            
        
            String path = item.id;
            //string id = path; //path.Substring(path.LastIndexOf("/"), path.Length);
            CodelabUtils._ShowAndroidToastMessage($"The ID : {path}");
            
            string id = $"{namee}as{DateTime.Now.Millisecond}";
            CodelabUtils._ShowAndroidToastMessage(id);
            
            var storage = FirebaseStorage.DefaultInstance;
            var referenced = storage.GetReferenceFromUrl("gs://augmentedestate.appspot.com");
            var reference = referenced.Child(PathString.Assets).Child(namee).Child(id);
            
            CodelabUtils._ShowAndroidToastMessage("Storage reference got");
            CodelabUtils._ShowAndroidToastMessage("byte encoded");
            
            Stream stream = new FileStream(path,FileMode.Open);
            var uploadTask = reference.PutStreamAsync(stream);
        
            yield return new WaitUntil(() => uploadTask.IsCompleted);
        
            CodelabUtils._ShowAndroidToastMessage("Image uploaded successfully");
            
            
            // check errors

            if (uploadTask.Exception != null)
            {
                CodelabUtils._ShowAndroidToastMessage($"Failed to upload image {uploadTask.Exception}");
                yield break;
            }
        
        
            // get the download uri

            var getUriTask = reference.GetDownloadUrlAsync();
        
            yield return new WaitUntil(() => getUriTask.IsCompleted);
        
            if (getUriTask.Exception != null)
            {
                CodelabUtils._ShowAndroidToastMessage($"Failed to upload image {getUriTask.Exception}");
                yield break;
            }
        
            String uri = getUriTask.Result.ToString();
            imageURLs.Add(uri); 
            CodelabUtils._ShowAndroidToastMessage($"Successfully stored Uri to list : {uri}");

        }
        
        
        string namestring = name.text;
        string labelstring = label.text;
        string describestring = description.text;
        
        
       
       
        DatabaseReference referec = database.Push();

        string key = referec.Key;
        
        Asset asset = new Asset(namestring,labelstring,key,describestring,imageURLs);
        string json = JsonUtility.ToJson(asset);

        referec.SetRawJsonValueAsync(json);
            
        CodelabUtils._ShowAndroidToastMessage("Successfully uploaded the Asset");
    
            
        // clear all the data collected
        name.text.Remove(0);
        label.text.Remove(0);
        description.text.Remove(0);
            
        items.Clear();
        //RemoveImages();
            
        CodelabUtils._ShowAndroidToastMessage("Cleared all data");


    }

    private IEnumerator GetFileUri(StorageMetadata storageMetadata)
    {

        var uritask = storageMetadata.Reference.GetDownloadUrlAsync();
        
        yield return new WaitUntil(() => uritask.IsCompleted);

        String uri = uritask.Result.ToString();
        imageURLs.Add(uri);
        
        CodelabUtils._ShowAndroidToastMessage($"Successfully stored Uri to list : {uri}");

        

    }

    
}
