using System;
using UnityEngine;

public class DragControls : MonoBehaviour
{
    private Vector3 offset;
    Transform objTransform;
    [SerializeField] private Transform targetTransform;
    Draggable dragObject;

    Action inputCheck;
    private float initialCamToTreeDist;
    Vector3 mousePos;

    private void Start()
    {
        inputCheck = Application.isMobilePlatform ? MobileInput : PcInput;
    }
    private void Update()
    {
        inputCheck();
    }
    private void PcInput()
    {

        if (Input.GetMouseButtonDown(0))
        {
            initialCamToTreeDist = (targetTransform.position - Camera.main.transform.position).magnitude;

            mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, initialCamToTreeDist));
            DragCheck();
        }

        if (Input.GetMouseButton(0))
        {
            UpdateDragPos();
        }

        if (Input.GetMouseButtonUp(0))
        {
            EndDrag();
        }
    }

    private void DragCheck()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        print("Drag check");
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            print("Hit " + hit.collider.name);  
            dragObject = hit.collider.GetComponent<Draggable>();
            if (dragObject == null) return;
            print("Hit has dragobject");

            StartDragging(hit);
        }
    }

    private void StartDragging(RaycastHit hit)
    {
        objTransform = hit.transform;

        // Start dragging
        dragObject.ActivateDragObject();

        // Calculate the offset between the GameObject and the touch point
        offset = objTransform.position - mousePos;
    }

    private Vector3 UpdateDragPos()
    {
        if (dragObject)
        {
            mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, initialCamToTreeDist));

            // Update the GameObject's position while dragging
            Vector3 newPosition = mousePos + offset;
            //newPosition.z = objTransform.position.z;  // Maintain the original Z position
            objTransform.position = newPosition;
        }

        return mousePos;
    }

    private void EndDrag()
    {
        if(!dragObject)     return;
        dragObject.DeactivateDragging();
        dragObject = null;
    }

    private void MobileInput()
    {

        // Check for touch input (on mobile) or mouse input (for debugging in the editor)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));

            // Handle different phases of the touch
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    initialCamToTreeDist = (targetTransform.position - Camera.main.transform.position).magnitude;

                    mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, initialCamToTreeDist));
                    DragCheck();
                    break;

                case TouchPhase.Moved:
                    UpdateDragPos();

                    break;

                case TouchPhase.Ended:
                    // End dragging
                    EndDrag();
                    break;
            }
        }
    }
}
