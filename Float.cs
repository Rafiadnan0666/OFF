using UnityEngine;

public class Float : MonoBehaviour
{
    [SerializeField]
    private Vector3 currentPos;
    private Rigidbody rb;
    private bool isFloating = true;
    //public Animation ani;

    void Start()
    {
       
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; 
        }
       
    }

    void Update()
    {
        if (isFloating)
        {
            currentPos = transform.position;
            float newY = Mathf.Sin(Time.time * 3) * 0.01f + currentPos.y;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
        else
        {
            if (rb != null)
            {
                //ani.isActive = false;
                rb.isKinematic = false; 
            }
        }
    }

    public void SetFloating(bool enable)
    {
        isFloating = enable;
    }
}
