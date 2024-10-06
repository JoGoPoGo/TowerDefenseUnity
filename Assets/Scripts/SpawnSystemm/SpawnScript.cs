using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{ 
    private Camera cam;
    private GameObject obj;
    void Start()
    {
        obj = Resources.Load<GameObject>("Basic");
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }
    void Update()
    {
      
            if (Input.GetKeyDown("mouse 0") == true)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit RayHit;
                if (Physics.Raycast(ray, out RayHit))
                {
                    GameObject targetHit = RayHit.transform.gameObject;
                    Vector3 hitPos = RayHit.point;
                    if (targetHit != null)
                    {
                        hitPos = hitPos + Vector3.up * obj.transform.localScale.y / 2;
                        Instantiate(obj, hitPos, Quaternion.identity);
                    obj.transform.position = new Vector3(transform.position.x, 2, transform.position.z);
                }
                }
               
            }
         }
}
