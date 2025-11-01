using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControll : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    public SkillExecutor skillExecutor;
    public Skill basicAttack;
    public Skill javelinThrow;

    private GameObject javelin;

    private readonly Dictionary<string, float> coolEnd = new();

    private void Awake()
    {
        if(skillExecutor == null)
            skillExecutor = GetComponent<SkillExecutor>();
        javelin = transform.Find("Javelin").gameObject;
    }
    // Update is called once per frame
    void Update()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(hor, 0, ver).normalized;
        transform.position += move*moveSpeed*Time.deltaTime;
        
        if(Input.GetMouseButtonDown(0)&&isReady(basicAttack))
        {
            StartCoroutine(CastRoutine(basicAttack));
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)&&isReady(javelinThrow))
        {
            
            javelin.transform.localPosition = new Vector3(0.6f, 0.5f, 0);
            javelin.transform.localEulerAngles = new Vector3(90, 90, 90);
            StartCoroutine(CastRoutine(javelinThrow));

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject==javelin)
        {
            if(coolEnd.TryGetValue("Javelin", out float t))
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
