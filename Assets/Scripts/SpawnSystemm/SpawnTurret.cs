using UnityEngine;
using UnityEngine.InputSystem;

namespace LP.SpawnObjectsNewInput
{

}
public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] GameObject basicPrefab = null;
    private Camera cam = null;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
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
                Instantiate(basicPrefab, new Vector3(hit.point.x, hit.point.y + basicPrefab.transform.position.y, hit.point.z), Quaternion.identity);
            }
        }
    }
}
