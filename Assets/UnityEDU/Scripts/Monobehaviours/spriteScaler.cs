using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spriteScaler : MonoBehaviour {

    //Camera variable to store the Main camera 
    //(so we aren't calling Camera.main 
    //in every frame which does FindCOmponent every time
    
    Camera cam;

    //sprite transform to scale when we have the aspect ratio

    Transform sprite;
    
    // Use this for initialization
   
    float cachedOrtho;
	
    void Start () {

        cam = Camera.main;
        sprite = gameObject.GetComponent<Transform>();
        cachedOrtho = cam.orthographicSize;
        ScaleToScreenSize(gameObject);

    }
	
    public void ScaleToScreenSize(GameObject s)
    {

            var sr = s.GetComponent<SpriteRenderer>() ;
            if (sr == null) return;

            transform.localScale = Vector3.one;

            var width = sr.sprite.bounds.size.x;
            var height = sr.sprite.bounds.size.y;

            // vertical size in world units is simply twice the orthographicSize
            float worldScreenHeight = cam.orthographicSize * 2.0f;
            // horizontal size is scaled by the pixel aspect ratio
            float worldScreenWidth  = worldScreenHeight / Screen.height * Screen.width;
            
            // apply new scale
            transform.localScale = new Vector3(worldScreenWidth / width, worldScreenHeight / height, 1);
    }
}
