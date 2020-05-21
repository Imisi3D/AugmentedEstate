using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChilditemScript : MonoBehaviour
{
    public RawImage image;
    public Text name;
    public Text label;
    public Text describe;
    public Button clickable;

    private ContentScrollist contentscript;
    private ItemChild childitem;
    
    // Start is called before the first frame update
    void Start()
    {
        clickable.onClick.AddListener(HandleChildClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup(ItemChild child, ContentScrollist content)
    {
        contentscript = content;
        childitem = child;

        image.texture = child.texture;
        name.text = child.name;
        label.text = child.label;
        describe.text = child.description;
        
        

    }

    public void HandleChildClick()
    {
        contentscript.ChildrenClickEvent(childitem);
    }

}
