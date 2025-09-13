using UnityEngine;


public class cameraController : MonoBehaviour
{
    [SerializeField] int sens;
    [SerializeField] int lockVertMin;
    [SerializeField] int lockVertMax;
    [SerializeField] bool invertY;

    [SerializeField] float zoomSpeed;
    [SerializeField] float minZoom;
    [SerializeField] float maxZoom;


    float rotX;
    Camera cam;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        cam = GetComponent<Camera>();
        if(cam == null)
        {
            cam = GetComponentInChildren<Camera>();
        }
    }


    // Update is called once per frame
    void Update()
    {
        float mouseX = (Input.GetAxisRaw("Mouse X") * sens * Time.deltaTime);
        float mouseY = (Input.GetAxisRaw("Mouse Y") * sens * Time.deltaTime);


        if (invertY)
        {
            rotX += mouseY;
        }
        else
        {
            rotX -= mouseY;
        }


        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);


        transform.localRotation = Quaternion.Euler(rotX, 0, 0);


        transform.parent.Rotate(Vector3.up * mouseX);

    }

    void Zoom()
    {
        if (cam == null)
        {
            return;
        }

        float Scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(Scroll) > 0.01f)
        {
            cam.fieldOfView -= Scroll * zoomSpeed;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minZoom, maxZoom);
        }
    }
}
