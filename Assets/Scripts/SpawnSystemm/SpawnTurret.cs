using UnityEngine;
using UnityEngine.InputSystem;

namespace LP.SpawnObjectsNewInput
{

}
public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] GameObject basicPrefab = null;
    private Camera cam = null;
    public bool SpawnActive = false;
    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (SpawnActive)
        SpawnAtMousePos();
    }
    private void SpawnAtMousePos()
    {
        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit) )
            {
                Instantiate(basicPrefab, new Vector3(hit.point.x, 0, hit.point.z), Quaternion.identity);
            }
            deactivateSpawn();
        }
    }
    public void activateSpawn()
    {
        SpawnActive = true;
    }
    public void deactivateSpawn()
    {  
        SpawnActive = false;
    }
}
