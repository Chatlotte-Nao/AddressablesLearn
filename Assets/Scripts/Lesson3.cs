using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Lesson3 : MonoBehaviour
{

    public AssetReference arf;
    public AssetReferenceAtlasedSprite atlas;
    public AssetReferenceTexture texture;
    
    // Start is called before the first frame update
    void Start()
    {
        Addressables.LoadAssetAsync<GameObject>("SD").Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Instantiate(handle.Result);
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
