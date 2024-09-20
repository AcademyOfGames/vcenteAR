using System;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    Transform objTransform;

    Action inputCheck;

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
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;


            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.GetComponent<Draggable>() == null) return;
                objTransform = hit.transform;
                // Start dragging
                isDragging = true;

                // Calculate the offset between the GameObject and the touch point
                offset = objTransform.position - mousePos;
            }
        }
        if (Input.GetMouseButton(0))
        {
            if (isDragging)
            {
                // Update the GameObject's position while dragging
                Vector3 newPosition = mousePos + offset;
                newPosition.z = objTransform.position.z;  // Maintain the original Z position
                objTransform.position = newPosition;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    private void MobileInput()
    {
        print("mobile input");

        // Check for touch input (on mobile) or mouse input (for debugging in the editor)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));

            // Handle different phases of the touch
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Raycast to detect if the touch is on the GameObject
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit) && hit.collider.GetComponent<Draggable>() != null)
                    {
                        objTransform = hit.transform;
                        // Start dragging
                        isDragging = true;

                        // Calculate the offset between the GameObject and the touch point
                        offset = objTransform.position - touchPosition;
                    }
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        // Update the GameObject's position while dragging
                        Vector3 newPosition = touchPosition + offset;
                        newPosition.z = objTransform.position.z;  // Maintain the original Z position
                        objTransform.position = newPosition;
                    }
                    break;

                case TouchPhase.Ended:
                    // End dragging
                    isDragging = false;
                    break;
            }
        }
    }
}
