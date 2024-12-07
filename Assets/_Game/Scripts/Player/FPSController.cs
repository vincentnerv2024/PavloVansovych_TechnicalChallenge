using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using MoreMountains.Feedbacks;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class FPSController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;               
    public float sprintMultiplier = 1.5f;       
    public float jumpHeight = 2f;              
    public float gravity = -9.81f;             

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 100f;      
    public Transform cameraTransform;          
    public Transform characterBody;           

    [Header("Interaction Settings")]
    public LayerMask interactableLayer;         
    public float interactRange = 2f;            

    [Header("UI Interaction Settings")]
    public float uiRaycastDistance = 5f;      

    [Header("Hands and Sockets")]
    public Transform leftHandSocket;           
    public Transform rightHandSocket;           
    public Transform blockPosition;             

    [Header("Animation Durations")]
    public float grabDuration = 0.3f;           
    public float blockDuration = 0.2f;          

    private CharacterController characterController; 
    private Vector3 velocity;                 
    private bool isGrounded;                   
    private float xRotation = 0f;               


    //Shield and Sphere interaction
    private GameObject shieldObj;              
    private GameObject sphereObj;             
    private bool isBlocking;                 

    [Header("Throw Settings")]
    public float throwForce = 20f;             

    public MMF_Player onInteractionFeedback;
    public MMF_Player offInteractionFeedback;

    public bool isContactWithInteractableObj;

    public Animator r_hand_anim;
    public Animator l_hand_anim;
   public List<AIBotController> botList;
    public MMF_Player winFeedback;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; // Make the cursor visible
        characterController = GetComponent<CharacterController>();

        AIBotController[] bots = FindObjectsByType<AIBotController>(FindObjectsSortMode.None);

        // Iterate through the bots array
        foreach (AIBotController bot in bots)
        {
            botList.Add(bot);
            Debug.Log(bot.name);
        }
    }
    public void LoadScene()
    {
        SceneManager.LoadScene(0);
    }

    void Update()
    {

        HandleMouseLook();     
        HandleMovement();        
        HandleInteraction();     
        HandleBlock();           
        HandleDropShield();      
        HandleUIInteraction();   
        UpdateHeldObjects();

        if (botList.Count <= 0)
        {
            DOVirtual.DelayedCall(2, winFeedback.PlayFeedbacks);

            DOVirtual.DelayedCall(5f, () => LoadScene());
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true; // Make the cursor visible
        }
     
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate the character body horizontally
        characterBody.Rotate(Vector3.up * mouseX);

        // Rotate the camera vertically and clamp rotation to avoid flipping
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void HandleMovement()
    {
        isGrounded = characterController.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Reset vertical velocity
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 move = transform.right * horizontal + transform.forward * vertical;

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;

        characterController.Move(move * currentSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }




    #region ObjectInteractions

    private void HandleInteraction()
    {
        if (Input.GetMouseButtonDown(1)) // Right mouse button for shield
        {
            TryGrabShield();
        }

        if (Input.GetMouseButtonDown(0)) // Left mouse button for sphere
        {
            if (sphereObj != null)
            {
                ThrowSphere();
            }
            else
            {
                TryGrabSphere();
            }
        }
    }

    private void TryGrabShield()
    {
        if (shieldObj == null)
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
            {
                if (hit.collider.CompareTag("Shield"))
                {
                    GrabShield(hit.collider.gameObject);
                }
            }
        }
    }

    private void GrabShield(GameObject shield)
    {
        shieldObj = shield;
        Rigidbody rb = shield.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        shield.transform.SetParent(leftHandSocket);
        shield.transform.DOLocalMove(Vector3.zero, grabDuration).SetEase(Ease.OutBack);
        shield.transform.DOLocalRotate(Vector3.zero, grabDuration).SetEase(Ease.OutBack);
    }

    private void TryGrabSphere()
    {
        if (sphereObj == null)
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
            {
                if (hit.collider.CompareTag("Sphere"))
                {
                    GrabSphere(hit.collider.gameObject);
                }
            }
        }
    }

    private void GrabSphere(GameObject sphere)
    {
        sphereObj = sphere;
        Rigidbody rb = sphere.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        sphere.transform.SetParent(rightHandSocket);
        sphere.transform.DOLocalMove(Vector3.zero, grabDuration).SetEase(Ease.OutBack);
        sphere.transform.DOLocalRotate(Vector3.zero, grabDuration).SetEase(Ease.OutBack);
        r_hand_anim.SetTrigger("Grab");
    }

    private void ThrowSphere()
    {
        if (sphereObj == null) return;

        Rigidbody rb = sphereObj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            sphereObj.transform.SetParent(null);
            rb.isKinematic = false;

            r_hand_anim.SetTrigger("Throw");
            // Apply throw force
            Vector3 throwDirection = cameraTransform.forward;
            DOVirtual.DelayedCall(0.15f, ()=> rb.AddForce(throwDirection * throwForce, ForceMode.Impulse));
        }

        sphereObj = null;
    }

    private void HandleBlock()
    {
        if (Input.GetMouseButton(1) && shieldObj != null)
        {
            isBlocking = true;
            l_hand_anim.SetBool("Block", true);
            shieldObj.transform.DOLocalMove(blockPosition.localPosition, blockDuration).SetEase(Ease.OutBack);
            shieldObj.transform.DOLocalRotate(blockPosition.localRotation.eulerAngles, blockDuration).SetEase(Ease.OutBack);
        }
        else if (isBlocking && shieldObj != null)
        {
            isBlocking = false;
            l_hand_anim.SetBool("Block", false);
            shieldObj.transform.DOLocalMove(Vector3.zero, grabDuration).SetEase(Ease.OutBack);
            shieldObj.transform.DOLocalRotate(Vector3.zero, grabDuration).SetEase(Ease.OutBack);
        }
    }

    private void UpdateHeldObjects()
    {
        if (shieldObj != null && !isBlocking)
        {
            shieldObj.transform.position = leftHandSocket.position;
            shieldObj.transform.rotation = leftHandSocket.rotation;
        }

        if (sphereObj != null)
        {
            sphereObj.transform.position = rightHandSocket.position;
            sphereObj.transform.rotation = rightHandSocket.rotation;
        }
    }






    private void HandleDropShield()
    {
        if (Input.GetKeyDown(KeyCode.T) && shieldObj != null)
        {
            // Drop the shield
            Rigidbody rb = shieldObj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }

            shieldObj.transform.SetParent(null);
            shieldObj = null;
        }
    }


    #endregion ObjectInteractions


    //UI interaction 
    private void HandleUIInteraction()
    {

            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, uiRaycastDistance))
            {
                if (hit.collider.GetComponentInParent<Canvas>() != null)
                {
                    onInteractionFeedback.PlayFeedbacks();
                    PointerEventData pointerData = new PointerEventData(EventSystem.current)
                    {
                        position = Input.mousePosition
                    };
                    if (Input.GetMouseButtonDown(0))
                    {
                        ExecuteUIInteraction(pointerData);
                    }
                }else
                {
                  offInteractionFeedback.PlayFeedbacks();
                }
           
            }
        
    }

    private void ExecuteUIInteraction(PointerEventData pointerData)
    {
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject.GetComponent<Button>() != null)
            {
                result.gameObject.GetComponent<Button>().onClick.Invoke();
                Debug.Log($"Clicked on button: {result.gameObject.name}");
                return;
            }
        }
    }

  
}
