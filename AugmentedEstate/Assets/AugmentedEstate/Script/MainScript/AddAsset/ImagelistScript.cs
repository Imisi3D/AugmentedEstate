using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImagelistScript : MonoBehaviour
{
    [SerializeField]private RawImage image;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Setup(Item item, ContentUIScript contentscript)
    {
        //TODO: Setup the Imageitem here with the approciate image

        image.texture = item.image;

    }
}
