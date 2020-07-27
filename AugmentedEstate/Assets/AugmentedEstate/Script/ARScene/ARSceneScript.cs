using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ARSceneScript : MonoBehaviour
{



    public static ItemChild ItemChildAR;
    private List<GameObject> GameObjectList;
    public static GameObject asset;
    public Camera ARCamera;
         

    // Physic variables
    private int speed = 5;
    

    // Start is called before the first frame update
    void Start()
    {

        ItemChildAR = Overview.child;
        GameObjectList = new List<GameObject>();

        StartCoroutine(LoadAssets(ItemChildAR));


    }

    private IEnumerator LoadAssets(ItemChild itemChild)
    {
        var loading = LoadAssetFromRemote.GetAsset(itemChild.label, GameObjectList);
        
        yield return new WaitUntil(() => loading.IsCompleted);

        asset = GameObjectList[0];
        
        
        
    }


    public static GameObject getGameObject()
    {
        return asset;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CaptureScreen()
    {

        Texture2D screencapture = ScreenCapture.CaptureScreenshotAsTexture();
        
        CodelabUtils._ShowAndroidToastMessage(screencapture.ToString());
        
        

    }

    public void Previous()
    {
        SceneManager.LoadScene(PathString.Preview);
    }

    public void Forward()
    {
        ARCamera.transform.Translate(speed * Time.deltaTime * Vector3.forward);
    }

    public void Right()
    {
    
    ARCamera.transform.Translate(speed * Time.deltaTime * Vector3.right);
       
    }

    public void Left()
    {
        ARCamera.transform.Translate(speed * Time.deltaTime * Vector3.left);
    }

    public void Back()
    {
        ARCamera.transform.Translate(speed * Time.deltaTime * Vector3.back);
    }

}
