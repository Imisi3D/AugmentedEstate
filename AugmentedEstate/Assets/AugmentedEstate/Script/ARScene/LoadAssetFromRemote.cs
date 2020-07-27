using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LoadAssetFromRemote : MonoBehaviour
{
   
    
    // Start is called before the first frame update
    
    public static async void Get(string label, List<GameObject> createdList)
    {

        var locations = await Addressables.LoadResourceLocationsAsync(label).Task;
        
        foreach(var location in locations)
        {
            createdList.Add(await Addressables.InstantiateAsync(location).Task as GameObject);
            
        }



    }

    public static async Task InitAsset<T>(string label, List<T> gameObjectList) where T : Object
    {
        var locations = await Addressables.LoadResourceLocationsAsync(label).Task;

        foreach (var location in locations)
        {
            gameObjectList.Add(await Addressables.InstantiateAsync(location).Task as T);
        }


    }


    public static async Task GetAsset(string label, List<GameObject> assetlist)
    {
        var locations = await Addressables.LoadResourceLocationsAsync(label).Task;
        
        foreach(var location in locations)
        {
            assetlist.Add(await Addressables.InstantiateAsync(location).Task as GameObject);
            
        }

    }

}
