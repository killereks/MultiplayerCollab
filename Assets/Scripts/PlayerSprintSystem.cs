using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;
using Mirror;

public class PlayerSprintSystem : NetworkBehaviour {

    [BoxGroup("Settings")]
    public float maxStamina; // maximum amount of stamina we can have
    [BoxGroup("Settings")]
    public float staminaRegenDelay; // how long it takes to regen after we stop running
    [BoxGroup("Settings")]
    public float staminaRegenSpeed; // base speed for regen
    [BoxGroup("Settings")]
    public float staminaRegenAcceleration; // how quickly we speed up the regen speed
    [BoxGroup("Settings")]
    public float staminaUsageRate; // how quickly we use up sprinting when running

    [BoxGroup("UI")]
    public Image sprintBar;

    float sprint;
    float sprintFillRate;
    float lastSprintTime;

    private void Start() {
        sprint = maxStamina;
    }

    private void Update() {
        if (!isLocalPlayer) return;

        if (Time.time - lastSprintTime > staminaRegenDelay) {
            sprint += sprintFillRate * Time.deltaTime;
            sprintFillRate += staminaRegenAcceleration * Time.deltaTime;
            sprint = Mathf.Clamp(sprint, 0f, maxStamina);
            sprintBar.fillAmount = sprint / maxStamina;
        }

        if (PostProcessing.instance != null) {
            PostProcessing.instance.SetChromaticAberration(1f - sprint / maxStamina);
        }
    }

    public bool CanSprint() {
        return sprint > 0f;
    }

    public void Sprinting() {
        lastSprintTime = Time.time;
        sprintFillRate = staminaRegenSpeed;
        sprint -= staminaUsageRate * Time.deltaTime;

        sprintBar.fillAmount = sprint / maxStamina;
    }
}
