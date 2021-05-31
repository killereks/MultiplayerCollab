using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    //Wobble
    public Renderer rend;
    Vector3 lastPos;
    Vector3 velocity;
    Vector3 lastRot;
    Vector3 angularVelocity;
    public float MaxWobble = 0.03f;
    public float WobbleSpeed = 1f;
    public float Recovery = 1f;
    float wobbleAmountX;
    float wobbleAmountZ;
    float wobbleAmountToAddX;
    float wobbleAmountToAddZ;
    float pulse;
    float time = 0.5f;

    //Timer
    public float maxTime = 10f;
    float currentTime = 0f;
    float bombProgress;

    //Colors
    [ColorUsage(false, true)]
    public Color baseSafe, baseDanger;
    [ColorUsage(false, true)]
    public Color surfaceSafe, surfaceDanger;

    private void Update()
    {
        Timer();
        WobbleEffect();
    }

    void WobbleEffect()
    {
        time += Time.deltaTime;
        // decrease wobble over time
        wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, Time.deltaTime * (Recovery));
        wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, Time.deltaTime * (Recovery));

        // make a sine wave of the decreasing wobble
        pulse = 2 * Mathf.PI * WobbleSpeed;
        wobbleAmountX = wobbleAmountToAddX * Mathf.Sin(pulse * time);
        wobbleAmountZ = wobbleAmountToAddZ * Mathf.Sin(pulse * time);

        // send it to the shader
        rend.material.SetFloat("_WobbleX", wobbleAmountX);
        rend.material.SetFloat("_WobbleZ", wobbleAmountZ);

        // velocity
        velocity = (lastPos - transform.position) / Time.deltaTime;
        angularVelocity = transform.rotation.eulerAngles - lastRot;


        // add clamped velocity to wobble
        wobbleAmountToAddX += Mathf.Clamp((velocity.x + (angularVelocity.z * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
        wobbleAmountToAddZ += Mathf.Clamp((velocity.z + (angularVelocity.x * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);

        // keep last position
        lastPos = transform.position;
        lastRot = transform.rotation.eulerAngles;
    }

    void Timer()
    {

        currentTime += Time.deltaTime;

        if(currentTime > maxTime)
        {
            currentTime = 0;
        }

        bombProgress = currentTime / maxTime;

        rend.material.SetFloat("_Fill", bombProgress);

        rend.material.SetColor("_LiquidColor", Color.Lerp(baseSafe, baseDanger, bombProgress));
        rend.material.SetColor("_SurfaceColor", Color.Lerp(surfaceSafe, surfaceDanger, bombProgress));
    }


}