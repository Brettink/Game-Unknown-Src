using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Controller
{
    private float healthV = 100f;
    public AnimationClip deathClip;
    private bool dead = false, doDead = false;
    public float health {
        get { return healthV; }
        set {
            healthV = value;
            UpdateBar();
        }
    }
    public Vector3 spawnPoint;
    public float distFromSpawn;
    public float maxHealth = 100f;
    private SpriteRenderer bar;
    public GameObject hitGlow;
    public TextMesh healthText;
    public Transform healthBarParent, healthBar;
    private bool foundPlayer = false;
    private Vector2 sizeRef;
    public Transform conePos;
    Vector3 playerLastPos = Vector3.zero;

    new void Start() {
        base.Start();
        bar = healthBar.GetComponent<SpriteRenderer>();
        sizeRef = bar.size;
        UpdateBar();
    }

    void OnTriggerEnter(Collider col) {
        if (col.tag == "Player") {
            Debug.DrawLine(conePos.position, 
                GManager.self.playerHead.position, Color.red, .2f);
            if (Physics.Linecast(conePos.position, 
                GManager.self.playerHead.position, out RaycastHit hit)) {
                if (hit.transform.tag == "Player") {
                    foundPlayer = true;
                }
            }
        }
    }

    public void Hit() {
        if (!dead) {
            HAS.stats.TryGetValue(STAT.STR, out float curVal);
            float dmgRecieved = curVal * ((HAS.lvlTot + 1f) / 10f);
            health -= dmgRecieved;
        }
    }

    void OnTriggerStay(Collider col) {
        if (col.tag == "Player") {
            if (foundPlayer) {
                playerLastPos = GManager.self.playerHead.position;
                Debug.DrawLine(conePos.position, playerLastPos, Color.red, .2f);
                playerLastPos.y = 0f;
            }
        }
    }

    void OnTriggerExit(Collider col) {
        if (col.tag == "Player") {
            foundPlayer = false;
            contMove = false;
        }
    }

    public void UpdateBar() {
        float perC = health / maxHealth;
        Vector2 upDateVal = Vector2.up * sizeRef;
        upDateVal.x = (sizeRef.x * perC);
        if (perC < 0.25f) {
            upDateVal.y -= .85f-(perC*4);
        }
        healthText.text = (int)health + "/" + (int)maxHealth;
        bar.size = upDateVal;
    }

    // Update is called once per frame
    new void Update()
    {
        //health -= .1f;
        if (health <= 0f) {
            health = 0f;
            dead = true;
        }
        healthBarParent.LookAt(GManager.myCam.transform.position);
        if (!doDead) {
            if (foundPlayer) {
                Vector3 pos = transform.position;
                pos.y = 0f;
                pos = playerLastPos - pos;
                pos.Normalize();
                moveVect = pos / 60f;
                if (!contMove) {
                    contMove = true;
                }
            }
            base.Update();
        }
        selfAnimator.SetBool("die", dead);
        if (CheckPercent("death", deathClip, 0, 9)){
            doDead = true;
            selfAnimator.SetBool("die", false);
        }
    }
}
