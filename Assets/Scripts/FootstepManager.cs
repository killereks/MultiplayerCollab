using UnityEngine;

public class FootstepManager : MonoBehaviour {
    public AudioSource audioSource;

    public AudioClip[] carpetSounds;
    public AudioClip[] cementSounds;
    public AudioClip[] creakyWoodSounds;
    public AudioClip[] forestSounds;
    public AudioClip[] glassSounds;
    public AudioClip[] metalSounds;
    public AudioClip[] mudSounds;
    public AudioClip[] puddleSounds;
    public AudioClip[] sandGravelSounds;

    AudioClip[] clipsToUse;

    public LayerMask layerMask;

    public enum Location {
        Carpet,
        Cement,
        Wood,
        Forest,
        Glass,
        Metal,
        Mud,
        Puddle,
        SandGravel
    }

    private void Start() {
        ChangeLocation(Location.Cement);
    }
    private void Update(){
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 2f, layerMask)) {
            GroundIdentifier groundIdentifier = hit.collider.GetComponent<GroundIdentifier>();
            if (groundIdentifier != null) {
                ChangeLocation(groundIdentifier.location);
            }
        }
    }

    void PlayFootstepSound(AnimationEvent evt) {
        if (evt.animatorClipInfo.weight < 0.5f) return;
        AudioClip clip = clipsToUse[Random.Range(0, clipsToUse.Length)];
        audioSource.PlayOneShot(clip);
    }

    public void PlayFootstepSound() {
        AudioClip clip = clipsToUse[Random.Range(0, clipsToUse.Length)];
        audioSource.PlayOneShot(clip);
    }

    public void ChangeLocation(Location location) {
        switch (location) {
            case Location.Carpet:
                clipsToUse = carpetSounds;
                break;
            case Location.Cement:
                clipsToUse = cementSounds;
                break;
            case Location.Wood:
                clipsToUse = creakyWoodSounds;
                break;
            case Location.Forest:
                clipsToUse = forestSounds;
                break;
            case Location.Glass:
                clipsToUse = glassSounds;
                break;
            case Location.Metal:
                clipsToUse = metalSounds;
                break;
            case Location.Mud:
                clipsToUse = mudSounds;
                break;
            case Location.Puddle:
                clipsToUse = puddleSounds;
                break;
            case Location.SandGravel:
                clipsToUse = sandGravelSounds;
                break;
            default:
                throw new System.Exception("Invalid location.");
        }
    }

    
}
