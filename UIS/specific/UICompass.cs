using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICompass : UI, UIAction
{
    public GameObject colliderer;
    public Transform compass;
    private Quaternion selfRot = Quaternion.identity;
    public static Quaternion rotTo = Quaternion.identity;
    new public void Start() {
        self = this.gameObject;
        selfCollider = colliderer.GetComponent<Collider>();
        selfSprite = GetComponent<SpriteRenderer>();
        GManager.addUI(selfCollider.name, gameObject, is3D);
    }

    void UIAction.doAction(MParams par) {
        Vector3 localPos = self.transform.InverseTransformPoint(par.hit.point);
        localPos.z = 0f;
        rotTo.eulerAngles = 
            Vector3.forward *(
                Vector3.SignedAngle(GManager.notZero, localPos, Vector3.forward)-45f);
    }

    void Update() {
        selfRot = Quaternion.Lerp(selfRot, rotTo, .25f);
        compass.localRotation = selfRot;
        CameraController.rotation = selfRot.eulerAngles.z;
    }
}
