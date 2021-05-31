using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerDeath : NetworkBehaviour {

    public Cinemachine.CinemachineVirtualCamera deathVCam;
    public Cinemachine.CinemachineVirtualCamera playerVCam;

    NetworkAnimator networkAnimator;

    public SkinnedMeshRenderer playerModel;

    //ragdoll effect
    public List<Behaviour> ragdollComponents;
    public Collider playerBaseCollider;
    public List<Collider> ragdollColliders;
    public Rigidbody playerRB;

    private void Start() {
        networkAnimator = GetComponent<NetworkAnimator>();
    }

    public void StartAnimation() {
        // only run this for the player that is dead.
        // the animation will trigger for everyone because it uses
        // networkAnimator.SetTrigger() which syncs
        if (!isLocalPlayer) return;

        deathVCam.Priority = 250;
        playerVCam.Priority = 0;

        playerModel.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

        LeanTween.delayedCall(4f, () => {
            deathVCam.Priority = 0;
        });

        PlayerParkourMovement playerController = GetComponent<PlayerParkourMovement>();

        playerController.enabled = false;
        playerController.GetComponent<Rigidbody>().velocity = Vector3.zero;

        ToggleRagdoll(true);

        //networkAnimator.SetTrigger("death");
    }

    public void ToggleRagdoll(bool toggle = true)
    {
        foreach (Behaviour component in ragdollComponents)
        {
            component.enabled = !toggle;
        }

        foreach (Collider collider in ragdollColliders)
        {
            collider.enabled = toggle;
        }

        playerBaseCollider.enabled = !toggle;
        playerRB.isKinematic = !toggle;

    }
}
