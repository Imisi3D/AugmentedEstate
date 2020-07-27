using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreviewScript : MonoBehaviour
{


    public Button button;
    public RawImage preview;

    private Overview Overview;

    private PreviewItem previewItem;
    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(HandleChildClick);

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetUp(PreviewItem item , Overview overviews)
    {

        preview.texture = item.preview;
        Overview = overviews;
        previewItem = item;



    }
    
    public void HandleChildClick()
    {
        Overview.PreviewItemClick(previewItem);
    }
}
