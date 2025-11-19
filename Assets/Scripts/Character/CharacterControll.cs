using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControll : MonoBehaviour
{
    public SkillExecutor skillExecutor;
    public SkillsetBase skillSet;
    public CharacterStatistics chstats;

    public bool isDashing = false;

    private GameObject javelin;

    public readonly Dictionary<string, float> coolEnd = new();

    private void Awake()
    {
        if(skillExecutor == null)
            skillExecutor = GetComponent<SkillExecutor>();
        if(chstats == null)
            chstats = GetComponent<CharacterStatistics>();
        javelin = transform.Find("Player/Javelin").gameObject;
    }
    // Update is called once per frame
    void Update()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(hor, 0, ver).normalized;
        if(!isDashing)
            chstats.Move(move);
        
        if(Input.GetMouseButtonDown(0)&&isReady(skillSet.LeftClick,"LClick"))
        {
            StartCoroutine(CastRoutine(skillSet.LeftClick,"LClick"));
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)&&isReady(skillSet.LShift,"LShift"))
        {
            
            javelin.transform.localPosition = new Vector3(0.6f, 0.5f, 0);
            javelin.transform.localEulerAngles = new Vector3(90, 90, 90);
            StartCoroutine(CastRoutine(skillSet.LShift,"LShift"));

        }

        if (Input.GetMouseButtonDown(1) && isReady(skillSet.RightClick,"RClick"))
        {
            StartCoroutine(CastRoutine(skillSet.RightClick,"RClick"));
        }

        if(Input.GetKeyDown(KeyCode.Space)&&isReady(skillSet.Space,"Space"))
        {
            StartCoroutine (CastRoutine(skillSet.Space,"Space"));
        }

        if(Input.GetKeyDown(KeyCode.Q)&&isReady(skillSet.QSkill,"Q"))
        {
            StartCoroutine(CastRoutine(skillSet.QSkill,"Q"));
        }

        if(Input.GetKeyDown(KeyCode.E)&&isReady(skillSet.ESkill,"E"))
        {
            StartCoroutine(CastRoutine(skillSet.ESkill,"E"));
        }

        if(Input.GetKeyDown(KeyCode.LeftControl)&&isReady(skillSet.LCtrl,"LCtrl"))
        {
            StartCoroutine(CastRoutine(skillSet.LCtrl, "LCtrl"));
        }

#if DEBUG
        if(Input.GetMouseButtonDown(2))
        {
            DebugingSkill_SelfDamage debugSkill = ScriptableObject.CreateInstance<DebugingSkill_SelfDamage>();

            // 배열이 null일 수도 있으므로 방어적으로 초기화
            if (debugSkill.skillStructures == null || debugSkill.skillStructures.Length == 0)
                debugSkill.skillStructures = new DebugingSkill_SelfDamage.SkillStructure[1];

            // 0번째 요소 생성
            if (debugSkill.skillStructures[0] == null)
                debugSkill.skillStructures[0] = new DebugingSkill_SelfDamage.SkillStructure();

            // damageModule도 생성
            if (debugSkill.skillStructures[0].damageMd == null)
                debugSkill.skillStructures[0].damageMd = new damageModule();

            if (debugSkill.skillStructures[0].basicMd == null)
                debugSkill.skillStructures[0].basicMd = new basicModule();


            debugSkill.skillStructures[0].damageMd.damage = 5;
            debugSkill.skillID = "debug, selfAttack";
            StartCoroutine(CastRoutine(debugSkill, "Debug"));
        }
    }
#endif

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject==javelin)
        {
            if(coolEnd.ContainsKey("LShift"))
            {
                coolEnd["LShift"] -= 5;
                Debug.Log("Javelin Cooled down for 5 s");
            }
        }
    }

    bool isReady(Skill s, string skillSlot)
    {
        if (!coolEnd.TryGetValue(skillSlot, out float t))
            return true;
        return Time.time >= t;
    }

    IEnumerator CastRoutine(Skill s, string skillSlot)
    {
        Debug.Log("Skill used: "+s.skillID);
        Debug.Log("Skill Level: "+s.skillLevel);
        if(s.basic.delayFront>0f)
            yield return new WaitForSeconds(s.basic.delayFront);
        s.execute(transform,skillExecutor);
        coolEnd[skillSlot] = Time.time + s.basic.cooldown;
    }
}
