using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControll : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    public SkillExecutor skillExecutor;
    public Skill basicAttack;
    public Skill javelinThrow;
    public Skill rushSlash;
    public Skill doubleSlash;
    public Skill recovery;

    public bool isDashing = false;

    private GameObject javelin;

    public readonly Dictionary<string, float> coolEnd = new();

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
        if(!isDashing)
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

        if (Input.GetMouseButtonDown(1) && isReady(doubleSlash))
        {
            StartCoroutine(CastRoutine(doubleSlash));
        }

        if(Input.GetKeyDown(KeyCode.Space)&&isReady(rushSlash))
        {
            StartCoroutine (CastRoutine(rushSlash));
        }

        if(Input.GetKeyDown(KeyCode.Q)&&isReady(recovery))
        {
            StartCoroutine(CastRoutine(recovery));
        }
    }

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
