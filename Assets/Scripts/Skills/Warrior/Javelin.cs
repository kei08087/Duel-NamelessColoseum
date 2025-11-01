using UnityEngine;

public class Javelin : MonoBehaviour
{
    public bool launch = false;
    public bool activate = false;
    public bool landed = false;
    public float length;
    public float coolDown;
    public float damage;
    public float speed;
    public LayerMask layer;
    private float moved;
    private float returnTime;

    public GameObject parent;
    // Update is called once per frame
    void Update()
    {
        if (activate)
        {
            moved = 0;
            returnTime = Time.time + coolDown;
            launch = true;
        }
        if (launch)
        {
            activate = false;
            transform.localPosition += transform.up * speed * Time.deltaTime;
            moved += speed*Time.deltaTime;
            if(moved>=length)
            {
                fall();
            }
        }
        if(landed&&Time.time>returnTime)
        {
            landed = false;
            returnParent();

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (launch&&((1<<other.gameObject.layer)&layer.value)!=0)
        {
            var dmg = other.GetComponent<IDamageable>();

            if (dmg != null)
                dmg.TakeDamage(damage);
            fall();
        }
        else if(launch&&other.CompareTag("Map"))
        {
            fall();
        }
        if (landed && other.gameObject == parent.gameObject)
        {
            landed = false;
            returnParent();
        }
    }

    private void returnParent()
    {
        transform.SetParent(parent.transform,false);
        transform.localPosition = new Vector3(0, 0, -0.6f);
        transform.localEulerAngles = new Vector3(0, 0, -30);
    }

    private void fall()
    {
        transform.localEulerAngles = new Vector3(-45, 90, 90);
        transform.localPosition -= new Vector3(0, 1, 0);
        landed = true;
        launch = false;
    }
}
