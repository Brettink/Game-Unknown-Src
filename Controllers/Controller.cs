using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Controller : MonoBehaviour {

	public Animator selfAnimator;
    public bool newModel = true;
	public AnimationClip walkingAnim, sheathAnimation, jumpAnim, moveJumpAnim;
	public NPCType type;
	public int seqId = 0;
	private AudioSource footStep;
	private Transform coll;
	public Rigidbody body;
	private bool isJump = false;
	private bool isMoving = false;
	private bool contMove = false;
	public static bool hasWeapon = false;
	public float timeJump = 0f, yAcel = 0f, prevYVel = 0f;
	private Vector3 moveVect = new Vector3 (0.00001f, 0f, 0.00001f);
    private Vector3 colLastPos = Vector3.zero;
	private float timeToIdleAnim;
	private List<GameObject> groundCols = new List<GameObject> ();
	//private Vector3 minPos = Vector3.one * (-GManager.MAX_SIZE / 2);
	//private Vector3 maxPos = Vector3.one * ( GManager.MAX_SIZE / 2);

	private bool sheathedV = false;
	public bool sheath {
		get {
			return sheathedV;
		}
		set {
			sheathedV = value;
			OnSheath ();
		}
	}

	// Use this for initialization
	public void Start () {
		footStep = GetComponent<AudioSource> ();
        if (newModel) {
            selfAnimator = transform.GetChild(1).GetComponent<Animator>();
        } else {
            selfAnimator = GetComponent<Animator>();
        }
		coll = transform. GetChild(1);
		body = GetComponent<Rigidbody> ();
		timeToIdleAnim = Time.fixedTime + 10f;
        if (this.tag == "Enemy") {
            moveVect = Vector3.forward*.005f;
            contMove = true;
        }
    }

	public void Sheath(){
		sheath = !sheath;
	}

	public void OnSheath(){
		GManager.self.UIAction.BroadcastMessage("OnSheath", sheath);
		selfAnimator.SetTrigger ("sheath");
	}

	public void endMove(){
		selfAnimator.SetBool ("walking", false);
		isMoving = false;
		contMove = false;
	}


	public void contToMove(bool doCont){
		contMove = doCont;
	}

	public void move (Vector3 moveBy){
        //moveBy = Quaternion.Euler(new Vector3(0f, -90f, 0f)) * moveBy;
        if (selfAnimator.GetFloat ("walkSpeed") > 0.001f){
			isMoving = true;
			moveVect = moveBy;
			float angle = Vector2.SignedAngle (new Vector2 (0.00001f, 0.00001f), new Vector2 (moveBy.x, moveBy.z));
			this.transform.localRotation = Quaternion.AngleAxis (-angle + 45f, Vector3.up);
		}
		if (!selfAnimator.GetBool ("walking")) {
			selfAnimator.SetBool ("walking", true);
		}
		selfAnimator.SetFloat ("walkSpeed", (float)(Math.Abs (moveBy.magnitude * 20f) + 0.001f));
	}

	public void Jump(){
		if (groundCols.Count () > 0) {
            //isJump = true;
			StartCoroutine ("DoJump");
			//Instantiate (weapon, rightHand.transform);
		}
		return;
	}

	IEnumerator DoJump(){
		selfAnimator.SetTrigger ("jump");
		if (!selfAnimator.GetBool ("walking")) {
			yield return new WaitForSeconds (.7f);
		}
		timeJump = Time.fixedTime + .1f;
		isJump = true;
		yield return null;
	}

	void OnCollisionEnter(Collision col){
		if (col.collider.tag == "ground") {
			if (selfAnimator.GetCurrentAnimatorStateInfo (0).IsTag ("falling")){
				if (!isJump /*&& selfAnimator.GetFloat ("ySpeed") < -4f*/) {
					selfAnimator.SetTrigger ("landed");
					Vector3 vel = body.velocity;
					vel.y = 0f;
					body.velocity = vel;
                    yAcel = 0f;
				}
			}
			if (!groundCols.Contains (col.gameObject)){
				groundCols.Add (col.gameObject);
			}
		}
	}

	void OnCollisionStay(Collision col){
		if (col.collider.tag == "ground"){
            if (!selfAnimator.IsInTransition(0) && selfAnimator.GetCurrentAnimatorStateInfo(0).IsTag("falling")) {
                if (!isJump /*&& selfAnimator.GetFloat ("ySpeed") < -4f*/) {
                    selfAnimator.SetTrigger("landed");
                    Vector3 vel = body.velocity;
                    vel.y = 0f;
                    body.velocity = vel;
                    yAcel = 0f;
                }
            }
            if (!groundCols.Contains (col.gameObject)){
				groundCols.Add (col.gameObject);
			}
		}
	}

	void OnCollisionExit(Collision col){
		if (col.collider.tag == "ground") {
			if (groundCols.Contains (col.gameObject)) {
				groundCols.Remove (col.gameObject);
			}
		}
	}

	// Update is called once per frame
	public void Update () {
		/*if (body.velocity.y < -0.5f){
			selfAnimator.SetBool ("jump", false);
			selfAnimator.SetBool ("falling", true);
		}
		if (selfAnimator.GetBool ("landed")){
			selfAnimator.SetBool ("falling", false);
			selfAnimator.SetBool ("landed", false);
		}*/

		if (Input.GetKeyUp (KeyCode.A)){
			sheath = !sheath;
		}

		body.velocity = GManager.clampVector3 (body.velocity, new Vector3 (-1.5f, -5f, -1.5f), new Vector3 (1.5f, 5f, 1.5f));
		if (selfAnimator.GetCurrentAnimatorStateInfo (0).IsTag ("idle")){
			if (Time.fixedTime >= timeToIdleAnim){
				selfAnimator.SetTrigger ("doIdle");
			}
		} else {
			timeToIdleAnim = Time.fixedTime + 10f;
		}
		if (selfAnimator.GetCurrentAnimatorStateInfo (0).IsTag ("idleGo")) {
			selfAnimator.ResetTrigger ("doIdle");
		}
	}

	void LateUpdate(){

        if (this.tag == "Enemy") {
            if (groundCols.Count() >= 1 && !isJump) {
                //Jump();
            }
        }

        //if (!canJump) {

        //float centerY = (transform.GetChild (2).position.y - transform.position.y) + 0.475f;
        //coll.localPosition = centerY * Vector3.up;
        //}
        if (transform.position.y < 0) {
            RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.up);
            foreach (RaycastHit hit in hits) {
                if (hit.collider.tag == "Tile") {
                    Vector3 pos = transform.position;
                    float y = hit.collider.GetComponent<adjustment>().GetYTo();
                    pos.y = y;
                    transform.position = pos;
                }
            }
          //  Debug.Log(colLastPos);
        }
	}

    public bool CheckPercent(String tag, AnimationClip clip, int layer, int percent) {
        return CheckPercent(tag, clip, layer, percent, 0);
    }

    public bool CheckPercent(String tag, AnimationClip clip, int layer, int percent, int dir){
		if (selfAnimator.GetCurrentAnimatorStateInfo (layer).IsTag (tag)){
			AnimatorStateInfo aniClip = selfAnimator.GetCurrentAnimatorStateInfo (layer);
			float currentFrame = (aniClip.normalizedTime * clip.averageDuration) % clip.averageDuration;
			float perC = currentFrame / clip.averageDuration;
			perC = ((int)(perC * 10));
            switch (dir) {
                case 0: return (perC == percent);
                case -1: return (perC < percent);
                case 1: return (perC > percent);
            }
		}
		return false;
	}

	public void FaceTo(Transform other){
		Vector3 newRot = other.position;
		newRot.y = transform.position.y;
		transform.LookAt (newRot);
	}

	public void DoMove(Vector3 moveBy){
		float angle = Vector2.SignedAngle (new Vector2 (0.00001f, 0.00001f), new Vector2 (moveBy.x, moveBy.z));
		this.transform.rotation = Quaternion.AngleAxis (-angle + 45f, Vector3.up);
		body.AddForce (moveBy*75f);
		//transform.position = GManager.clampVector3 (transform.position, minPos, maxPos);
		moveVect = moveBy;
	}

	public void FixedUpdate () {
        /*if (selfAnimator.GetCurrentAnimatorStateInfo (0).IsTag ("walk")){
			AnimatorStateInfo animationClip = selfAnimator.GetCurrentAnimatorStateInfo(0);
			float currentFrame = (animationClip.normalizedTime * walkingAnim.averageDuration) % walkingAnim.averageDuration;
			float percent = currentFrame / walkingAnim.averageDuration;
			percent = ((int)(percent * 10));
			if (percent % 5 == 0f){
				GManager.soundStep.Play ();
			}
		}*/
        if (CheckPercent("jump", (isMoving)?moveJumpAnim:jumpAnim, 0, 2, 1)) {
            yAcel = (body.position.y - prevYVel);
            prevYVel = body.position.y;
        } else {
            yAcel = 0f;
        }
		selfAnimator.SetFloat ("ySpeed", yAcel);
		if (CheckPercent ("walk", walkingAnim, 0, 5)){
			footStep.Play ();
		}

		if (isJump){
			if (Time.fixedTime >= timeJump) {
				isJump = false;
			} else {
				body.AddForce (Vector3.up * 10f);
			}
		}
		if (contMove || isMoving){
			if (!selfAnimator.GetBool ("walking")) {
				selfAnimator.SetBool ("walking", true);
			}
			DoMove (moveVect);
		}

		//transform.position = new Vector3 (0f, pos.y, pos.z);
	}
}
