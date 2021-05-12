using UnityEngine;

public class FootstepManager : MonoBehaviour {
    public AudioSource audioSource;

    public AudioClip[] dirtyGround;
    public AudioClip[] grass;
    public AudioClip[] gravel;
    public AudioClip[] leaves;
    public AudioClip[] metal;
    public AudioClip[] mud;
    public AudioClip[] rock;
    public AudioClip[] sand;
    public AudioClip[] snow;
    public AudioClip[] tile;
    public AudioClip[] water;
    public AudioClip[] wood;

    AudioClip[] clipsToUse;

    public LayerMask layerMask;

    public enum Location {
        dirtyGround,
        grass,
        gravel,
        leaves,
        metal,
        mud,
        rock,
        sand,
        snow,
        tile,
        water,
        wood
    }

    private void Start() {
        ChangeLocation(Location.rock);
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
            case Location.dirtyGround:
                clipsToUse = dirtyGround;
                break;
            case Location.grass:
                clipsToUse = grass;
                break;
            case Location.gravel:
                clipsToUse = gravel;
                break;
            case Location.leaves:
                clipsToUse = leaves;
                break;
            case Location.metal:
                clipsToUse = metal;
                break;
            case Location.mud:
                clipsToUse = mud;
                break;
            case Location.rock:
                clipsToUse = rock;
                break;
            case Location.sand:
                clipsToUse = sand;
                break;
            case Location.snow:
                clipsToUse = snow;
                break;
            case Location.tile:
                clipsToUse = tile;
                break;
            case Location.water:
                clipsToUse = water;
                break;
            case Location.wood:
                clipsToUse = wood;
                break;
            default:
                throw new System.Exception("Invalid location.");
        }
    }

    
}
