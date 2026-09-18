using UnityEngine;

public class BackgroundControlller : MonoBehaviour
{
    
    private float startPos;
    public GameObject cam;
    public float parallaxEffect; // vitesse du bg qui vas bouger relativement a la camera.


    void Start()
    {
        startPos = transform.position.x;
    }

    void Update()
    {
        //calculer la distance du bg baser sur le mouvement de la cam
        float distance = cam.transform.position.x * parallaxEffect ; // 0 = mouvement de avec la cam 1 =  pas bouger 0.5 = moitie

        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);
    }
    
}