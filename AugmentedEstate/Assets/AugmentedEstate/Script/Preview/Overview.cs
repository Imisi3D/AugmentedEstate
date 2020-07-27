using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


[Serializable]
public class PreviewItem
{
    public Texture2D preview;
    public string id;

    public PreviewItem(Texture2D previewitem, string ID)
    {
        preview = previewitem;
        id = ID;

    }


}

public class Overview : MonoBehaviour
{


   
    
    // UI element
    public Text name;
    public Text label;
    public Text description;
    public RawImage overpreview;
    public Image imagepreview;

    public static ItemChild child;
    
    
    
    
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        
        isObjectContainList = new List<string>();
        previewItems = new List<PreviewItem>();
        isPreviewInList = new List<string>();
        objectPooler = ObjectPooler.instance;
        
        
        // get the Itemchild from the content controller of Assetlist
        child = ContentScrollist.ItemChildtransfer;

        // get the list of images and spawn them
        images = child.asset.images;
        AddImagePreviews(images);

        
        

        // fill the text ui element
        name.text = child.name;
        label.text = child.label;
        description.text = child.description;
        overpreview.texture = child.texture;
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region CLick listeners

    

    

    public void Back()
    {
        SceneManager.LoadScene(PathString.Main);
    }

    public void ARView()
    {
        SceneManager.LoadScene(PathString.ARScene);
    }

    public void PreviewItemClick(PreviewItem previewItem)
    {

        CodelabUtils._ShowAndroidToastMessage("Preview Image is clicked");
        
        overpreview.texture = previewItem.preview;

        Texture2D texture = previewItem.preview;
        
        Sprite image = Sprite.Create(texture,new Rect(0f,0f,512f,512f),new Vector2(0,0) );
        imagepreview.sprite = image;


    }
    
    #endregion
    
    #region HorizontalScroll Variable region

    private ObjectPooler objectPooler;
    public Transform ContentTransform;
    private List<string> isObjectContainList;
    private List<string> isPreviewInList;
    private List<PreviewItem> previewItems;
    private List<string> images;

    
    
    




    private void AddImagePreviews(List<string> images)
    {

        foreach (var image in images)
        {

            StartCoroutine(downloadTexture(image));

        }
        
        
        
    }
    
    

    
    private IEnumerator downloadTexture(string imagestring)
    {
        
        
        Texture2D texture = new Texture2D(512,512);
        
        
       

        //var handler = UnityWebRequestTexture.GetTexture(asset.images[0]).downloadHandler;

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imagestring);
        
        // yield return new WaitUntil(() => handler.isDone);

        yield return www.SendWebRequest();
        
        
        if (www.isNetworkError || www.isHttpError)
        {
         
            CodelabUtils._ShowAndroidToastMessage($"Something went wrong : {www.error}");
        }
        else
        {
            
            texture = ((DownloadHandlerTexture) www.downloadHandler).texture;
            // ---------------------- or -------------------------
            // texture = DownloadHandlerTexture.GetContent(www);
           
            CodelabUtils._ShowAndroidToastMessage($"Texture is got");
        }

        PreviewItem previewItem = new PreviewItem(texture,imagestring);
        
        CodelabUtils._ShowAndroidToastMessage($"downloadtexture : Adding Item {previewItem.id}");
        
        
        // TODO: check if the itemchild is contain in the Itemchildren. use a method to loop through the itemchildren and check



        // Add to the preview list and spawn the image object
        if (!isPreviewInList.Contains(previewItem.id))
        {
            previewItems.Add(previewItem);
            SpawnPreview(previewItems);
            
            isPreviewInList.Add(previewItem.id);
            
        }


    }
    
    
    private void SpawnPreview(List<PreviewItem> images)
    {
        foreach (var imagestring in images)
        {

            if (!isObjectContainList.Contains(imagestring.id))
            {
                
                GameObject content = objectPooler.SpawnObject(PathString.Preview);
                
                
                content.transform.SetParent(ContentTransform,false);

                PreviewScript previewScript = content.GetComponent<PreviewScript>();
                previewScript.SetUp(imagestring,this);
                
                
                
                isObjectContainList.Add(imagestring.id);
            }
            
        }
    }
    

    

    #endregion


}
