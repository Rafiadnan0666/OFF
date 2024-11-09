using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    public Camera mainCamera;
    public LineRenderer lineRenderer;
    public LayerMask interactableLayer;
    public Inventory inventory;
    public Text textE;
    public Text textF;
    public float maxInteractionDistance = 3f;

    private bool isHolding;
    private GameObject currentInteractable;
    private Rigidbody currentRigidbody;
    private Vector3 offset;
    private float distanceToCamera;
    private Door currentDoor;

    void Start()
    {
        mainCamera ??= Camera.main;

        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
        lineRenderer.enabled = false;

        textE?.gameObject.SetActive(false);
        textF?.gameObject.SetActive(false);

        
    }

    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxInteractionDistance, interactableLayer))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.CompareTag("Interactable"))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    isHolding = true;
                    currentInteractable = hitObject;
                    currentRigidbody = currentInteractable.GetComponent<Rigidbody>();

                    if (currentRigidbody != null)
                    {
                        currentRigidbody.useGravity = false;
                    }
                    distanceToCamera = Vector3.Distance(mainCamera.transform.position, hit.point);
                    offset = currentInteractable.transform.position - hit.point;
                }

                if (Input.GetKeyDown(KeyCode.F))
                {
                    InteractWithObject(hitObject);
                }
            }
            else if (IsPickable(hitObject))
            {
                ShowTextE(true);
                ShowTextF(false);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    PickUpItem(hitObject);
                }
            }
            else if (hitObject.CompareTag("Door"))
            {
                ShowTextE(false);
                ShowTextF(true);

                if (Input.GetKeyDown(KeyCode.F))
                {
                    Door door = hitObject.GetComponent<Door>();
                    if (door != null)
                    {
                        door.ToggleDoor();
                    }
                }
            }

            else
            {
                ShowTextE(false);
                ShowTextF(false);
                currentDoor = null;
            }
        }
        else
        {
            ShowTextE(false);
            ShowTextF(false);
            currentDoor = null;
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopHolding();
        }

        if (isHolding && Input.GetMouseButton(1) && currentInteractable != null)
        {
            RotateObject();
        }
       

        if (isHolding)
        {
            UpdateLine(hit.point);
            MoveObject();
        }


        if (isHolding && Input.GetMouseButton(1) && currentInteractable != null)
        {
            RotateObject(); 
        }



        if (Input.GetKeyDown(KeyCode.V))
        {
            ToggleFlashlight();
        }
    }

    bool IsPickable(GameObject item)
    {
        return item.CompareTag("Pickable") || item.CompareTag("Gun") || item.CompareTag("Ammo") || item.CompareTag("Flashlight");
    }

    private void StartHolding(GameObject item, Vector3 hitPoint)
    {
        isHolding = true;
        currentInteractable = item;
        currentRigidbody = currentInteractable.GetComponent<Rigidbody>();
        if (currentRigidbody != null)
        {
            currentRigidbody.useGravity = false;
        }
        distanceToCamera = Vector3.Distance(mainCamera.transform.position, hitPoint);
        offset = currentInteractable.transform.position - hitPoint;
    }

    private void StopHolding()
    {
        if (isHolding && currentRigidbody != null)
        {
            isHolding = false;
            currentRigidbody.useGravity = true;
            currentRigidbody = null;
            currentInteractable = null;
            lineRenderer.enabled = false;
        }
    }

    void UpdateLine(Vector3 hitPoint)
    {
        Vector3 screenPoint = new Vector3(Screen.width / 2, Screen.height / 2, distanceToCamera);
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(screenPoint);

        lineRenderer.SetPosition(0, worldPoint);
        lineRenderer.SetPosition(1, currentInteractable.transform.position);
        lineRenderer.enabled = true;
    }

    void MoveObject()
    {
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceToCamera);
        Vector3 targetPosition = mainCamera.ScreenToWorldPoint(mousePosition) + offset;
        currentRigidbody.velocity = (targetPosition - currentInteractable.transform.position) * 10f;
    }

    void RotateObject()
    {
        float rotationSpeed = 100f;
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        currentInteractable.transform.Rotate(Vector3.up, -mouseX, Space.World);
        currentInteractable.transform.Rotate(Vector3.right, mouseY, Space.World);
    }

    void PickUpItem(GameObject item)
    {
        inventory.StoreItem(item);
    }

    void ToggleFlashlight()
    {
        if (inventory != null)
        {
            inventory.ToggleFlashlight(); // Assuming ToggleFlashlight() is defined in your Inventory script
        }
    }

    void InteractWithObject(GameObject interactable)
    {
        // Implement interaction logic here (e.g., opening doors, pressing buttons)
    }

    bool RequiresLeftClick(GameObject item)
    {
        return item.GetComponent<Gun>() != null;
    }

    void ShowTextE(bool show)
    {
        if (textE != null)
        {
            textE.gameObject.SetActive(show);
        }
    }

    void ShowTextF(bool show)
    {
        if (textF != null)
        {
            textF.gameObject.SetActive(show);
        }
    }
}
