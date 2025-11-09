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
        javelin = transform.Find("Javelin").gameObject;
    }
    // Update is called once per frame
    void Update()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(hor, 0, ver).normalized;
        if(!isDashing)
            chstats.Move(move);
        
        if(Input.GetMouseButtonDown(0)&&isReady(skillSet.LeftClick))
        {
            StartCoroutine(CastRoutine(skillSet.LeftClick));
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)&&isReady(skillSet.LShift))
        {
            
            javelin.transform.localPosition = new Vector3(0.6f, 0.5f, 0);
            javelin.transform.localEulerAngles = new Vector3(90, 90, 90);
            StartCoroutine(CastRoutine(skillSet.LShift));

        }

        if (Input.GetMouseButtonDown(1) && isReady(skillSet.RightClick))
        {
            StartCoroutine(CastRoutine(skillSet.RightClick));
        }

        if(Input.GetKeyDown(KeyCode.Space)&&isReady(skillSet.Space))
        {
            StartCoroutine (CastRoutine(skillSet.Space));
        }

        if(Input.GetKeyDown(KeyCode.Q)&&isReady(skillSet.QSkill))
        {
            StartCoroutine(CastRoutine(skillSet.QSkill));
        }

        if(Input.GetKeyDown(KeyCode.E)&&isReady(skillSet.ESkill))
        {
            StartCoroutine(CastRoutine(skillSet.ESkill));
        }

        if(Input.GetKeyDown(KeyCode.LeftControl)&&isReady(skillSet.LCtrl))
        {
            StartCoroutine(CastRoutine(skillSet.LCtrl));
        }

#if DEBUG
        if(Input.GetMouseButtonDown(2))
        {
            Skill debugSkill = ScriptableObject.CreateInstance<DebugingSkill_SelfDamage>();
            debugSkill.damage = 5;
            debugSkill.skillID = "debug, selfAttack";
            StartCoroutine(CastRoutine(debugSkill));
        }
    }
#endif

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject==javelin)
        {
            if(coolEnd.ContainsKey("Javelin"))
            {
                coolEnd["Javelin"] -= 5;
                Debug.Log("Javelin Cooled down for 5 s");
            }
        }
    }

    bool isReady(Skill s)
    {
        if (!coolEnd.TryGetValue(s.skillID, out float t))
            return true;
        return Time.time >= t;
    }

    IEnumerator CastRoutine(Skill s)
    {
        Debug.Log("Skill used: "+s.skillID);
        if(s.delayFront>0f)
            yield return new WaitForSeconds(s.delayFront);
        s.execute(transform,skillExecutor);
        coolEnd[s.skillID] = Time.time + s.cooldown;
    }
}
