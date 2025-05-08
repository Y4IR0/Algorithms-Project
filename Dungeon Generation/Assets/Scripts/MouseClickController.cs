using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class MouseClickController : MonoBehaviour
{
    public UnityEvent<Vector3> OnClick;
    //public Vector3 clickPosition;
    Vector3 validClickPosition;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        { 
            Ray mouseRay = Camera.main.ScreenPointToRay( Input.mousePosition ); 
            
            if (Physics.Raycast( mouseRay, out RaycastHit hitInfo ))
            { 
                Vector3 clickWorldPosition = hitInfo.point; 
                validClickPosition = clickWorldPosition;
                OnClick.Invoke(validClickPosition);
            } 
        }
    }

    void OnDrawGizmos()
    {
        Debug.DrawRay( Camera.main.transform.position, validClickPosition - Camera.main.transform.position, Color.yellow);
        DebugExtension.DrawPoint(validClickPosition, Color.blue, 1);
    }
}
