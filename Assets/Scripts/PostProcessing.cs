using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessing : MonoBehaviour {

    public Volume volume;

    ChromaticAberration ca = null;
    DepthOfField dof = null;

    public static PostProcessing instance;

    private void Awake() {
        instance = this;

        volume.profile.TryGet(out ca);
        volume.profile.TryGet(out dof);
    }

    public void SetChromaticAberration(float amount) {
        ca.intensity.value = amount;
    }

    public void SetDepthOfField(float distance) {
        dof.focusDistance.value = distance;
    }
}
