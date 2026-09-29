using UnityEngine;

[CreateAssetMenu(fileName = "ArcherDivineBow", menuName = "Scriptable Objects/ArcherDivineBow")]
public class ArcherDivineBow : Skill
{
    [System.Serializable]
    public class Level
    {
        public basicModule timing;
        public float damage;
    }

    public Level[] levels = new Level[5];
    public float range = 30f;
    public float speed = 8f;
    public float width = 0.75f;
    public StraightProjectile projectilePrefab;
    public Material chargeMaterial;

    private GameObject chargePlate;
    private DivineBowChargeBar chargeBar;

    public override basicModule basic => levels[skillLevel - 1].timing;
    public override AttackWallPolicy WallPolicy => AttackWallPolicy.BlockHighWalls;
    public override void init() { }

    public override void OnWindupStart(Transform caster)
    {
        OnWindupEnd(caster);
        chargePlate = GameObject.CreatePrimitive(PrimitiveType.Cube);
        chargePlate.name = "Divine Bow Charge";
        chargePlate.layer = LayerMask.NameToLayer("Ignore Raycast");
        chargePlate.transform.SetParent(caster, false);
        chargePlate.transform.localPosition = new Vector3(0f, -0.72f, 0f);
        chargePlate.transform.localScale = new Vector3(1.4f, 0.04f, 1f);

        Collider plateCollider = chargePlate.GetComponent<Collider>();
        if (plateCollider != null)
        {
            plateCollider.enabled = false;
            Destroy(plateCollider);
        }
        MeshRenderer plateRenderer = chargePlate.GetComponent<MeshRenderer>();
        plateRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        plateRenderer.receiveShadows = false;
        if (chargeMaterial != null)
            plateRenderer.sharedMaterial = chargeMaterial;

        chargeBar = DivineBowChargeBar.Show(caster, basic.delayFront);
    }

    public override void OnWindupEnd(Transform caster)
    {
        if (chargeBar != null)
        {
            chargeBar.Hide();
            chargeBar = null;
        }
        if (chargePlate == null)
            return;
        chargePlate.SetActive(false);
        Destroy(chargePlate);
        chargePlate = null;
    }

    private void OnDisable()
    {
        OnWindupEnd(null);
    }

    public override void execute(Transform caster, SkillExecutor executor)
    {
        CharacterStatistics stats = caster.GetComponent<CharacterStatistics>();
        StraightProjectile.Launch(projectilePrefab, caster, caster.forward, range,
            stats != null ? stats.ProjectileSpeed(speed) : speed, width,
            levels[skillLevel - 1].damage, targetMask, WallPolicy);
    }
}
