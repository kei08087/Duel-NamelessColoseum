using UnityEngine;

public class Javelin : MonoBehaviour
{
    public bool launch;
    public bool activate;
    public bool landed;
    public float length;
    public float coolDown;
    public float damage;
    public float speed;
    public LayerMask layer;
    public AttackWallPolicy wallPolicy;
    public GameObject parent;

    private float moved;
    private float returnTime;
    private Vector3 originalScale;
    private float tempoScale;
    private Collider ownCollider;

    private void Awake()
    {
        originalScale = transform.localScale;
        ownCollider = GetComponent<Collider>();
        tempoScale = GameManager.Instance != null ? GameManager.Instance.TempoScale : 1f;
    }

    private void Update()
    {
        if (activate)
        {
            activate = false;
            launch = true;
            landed = false;
            moved = 0f;
            returnTime = Time.time + coolDown;
        }

        if (launch)
            Advance();

        if (landed && Time.time >= returnTime)
            ReturnToOwner();
    }

    private void Advance()
    {
        float remaining = Mathf.Max(0f, length * tempoScale - moved);
        float step = Mathf.Min(remaining, speed * Time.deltaTime * tempoScale);
        if (step <= 0f)
        {
            Fall();
            return;
        }

        Vector3 direction = transform.up.normalized;
        Physics.SyncTransforms();
        if (ProjectileCollision.Sweep(transform.position, direction, 0.1f, step, layer,
            wallPolicy, ownCollider, out RaycastHit nearest))
        {
            transform.position += direction * Mathf.Max(0f, nearest.distance);
            Hit(nearest.collider);
            return;
        }

        transform.position += direction * step;
        moved += step;
        if (moved >= length * tempoScale)
            Fall();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (launch && ProjectileCollision.ShouldImpact(other.gameObject.layer, layer, wallPolicy))
            Hit(other);

        if (landed && parent != null && other.gameObject == parent)
        {
            ReturnToOwner();
            CharacterControll controller = parent.GetComponent<CharacterControll>();
            if (controller != null)
                controller.coolDownEffect("LShift", 5f);
        }
    }

    private void Hit(Collider other)
    {
        if (!launch)
            return;
        if (((1 << other.gameObject.layer) & layer.value) != 0 &&
            other.TryGetComponent<IDamageable>(out var target))
            target.TakeDamage(damage);
        Fall();
    }

    private void Fall()
    {
        if (!launch)
            return;
        launch = false;
        landed = true;
        transform.localEulerAngles = new Vector3(-45f, 90f, 90f);
        transform.position -= Vector3.up;
    }

    public void ReturnToOwner()
    {
        if (parent == null)
        {
            Destroy(gameObject);
            return;
        }
        transform.SetParent(parent.transform, false);
        transform.localPosition = new Vector3(0f, 0f, -0.6f);
        transform.localEulerAngles = new Vector3(0f, 0f, -30f);
        transform.localScale = originalScale;
        launch = false;
        activate = false;
        landed = false;
    }
}
