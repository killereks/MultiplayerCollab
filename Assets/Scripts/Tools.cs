using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class Tools {
    public static float Lerp(float a, float b, float t) => a + ( b - a ) * t;
    public static float LerpQuad(float a, float b, float t) => Lerp(a, b, t * t);
    public static float LerpCubic(float a, float b, float t) => Lerp(a, b, t * t * t);
    public static float LerpSqrt(float a, float b, float t) => Lerp(a, b, Mathf.Sqrt(t));

    public static string FormatTime(float seconds) {
        // values corresponding to: years, weeks, days, hours, minutes, seconds
        int[] values = new int[]{52*24*3600*7, 24*3600*7,24*3600,3600,60,1 };
        char[] prefixes = new char[]{'y','w','d','h','m','s'};

        float[] outcome = new float[values.Length];

        StringBuilder str = new StringBuilder();

        for (int i = 0; i < values.Length; i++) {
            if (seconds > values[i]) {
                outcome[i] = Mathf.Floor(seconds / values[i]);
                seconds -= outcome[i] * values[i];
                str.Append(outcome[i]).Append(prefixes[i]).Append(' ');
            }
        }

        float ms = Decimals(seconds) * 1000f;
        ms = Mathf.Round(ms);
        seconds = Mathf.Floor(seconds);

        if (ms > 0f) str.Append(ms).Append("ms ");

        return str.ToString();
    }

    public static float Decimals(float number) => number - Mathf.Floor(number);

    public static bool InsideRange(float num, float min, float max) => min <= num && num <= max;
    public static bool InsideRange(int num, int min, int max) => min <= num && num <= max;

    public static T PickRandom<T>(T[] list) {
        return list[UnityEngine.Random.Range(0, list.Length)];
    }
    public static Transform FindDeepChild(this Transform aParent, string aName) {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(aParent);
        while (queue.Count > 0) {
            var c = queue.Dequeue();
            if (c.name == aName)
                return c;
            foreach (Transform t in c)
                queue.Enqueue(t);
        }
        return null;
    }
    public static Transform FindDeepChild(this GameObject aParent, string aName) => FindDeepChild(aParent.transform, aName);
    public static T FindDeepChild<T>(GameObject aParent, string aName) => FindDeepChild(aParent, aName).GetComponent<T>();

    public static int ChildCountDeep(this Transform transform) {
        int childCount = transform.childCount;
        foreach (Transform child in transform) {
            childCount += ChildCountDeep(child);
        }
        return childCount;
    }

    public static float RandomNormal(float min, float max, int iterations = 10) {
        float sum = 0f;
        for (int i = 0; i < iterations; i++) {
            sum += UnityEngine.Random.Range(min, max);
        }
        return sum / iterations;
    }

    public static int RandomNormal(int min, int max, int iterations = 10) {
        int sum = 0;
        for (int i = 0; i < iterations; i++) {
            sum += UnityEngine.Random.Range(min, max);
        }
        return sum / iterations;
    }
}