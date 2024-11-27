using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public static class SUtilities
{
    /// <summary>
    /// Give the percentage of chance you want to get using UnityEngine.Random.Range().
    ///    [ 100 : always true ]
    ///    [ 0 : always false ]
    ///    [ 50 : 50% chance to be true ]
    ///    [ n : n% chance to be true ]
    /// </summary>
    /// <param name="percentage"> A value between 0 and 100 </param>
    /// <returns> boolean of weather it have the chance </returns>
    public static bool Chance(float percentage, Action OnChance = null, Action OffChance = null)
    {
        bool haveChance = percentage > Random.Range(0f, 100f);
        if (haveChance)
            OnChance?.Invoke();
        else
            OffChance?.Invoke();
        return haveChance;
    }

    public static GameObject CreateSphereAtLocation(
        Vector3 positionArg,
        string nameArg = "Unnamed",
        Color? color = null,
        float? scale = null,
        float? autoDestroyTime = null,
        bool spawnTextAlso = false
    )
    {
        if (GameManager.Instance && !GameManager.Instance.CanUseDebugingFunctions)
            return new GameObject();
        GameObject newObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        newObject.transform.localScale = Vector3.one * (scale ?? 0.2f);
        newObject.transform.position = positionArg;
        newObject.name = "[loc] " + nameArg;
        newObject.GetComponent<Collider>().enabled = false;
        if (autoDestroyTime != null)
            newObject.AddComponent<AutoDestroyableInGivenTime>().SetTime(autoDestroyTime.Value);

        if (color != null)
            newObject.GetComponent<MeshRenderer>().material.color = (Color)color;
        Log($"Created Sphere GameObject at {positionArg} with the name of {nameArg}");
        if (spawnTextAlso && nameArg != "Unnamed")
            CreateTextAtLocation(positionArg, nameArg, autoDestroyTime: autoDestroyTime);
        return newObject;
    }

    public static void DelayedExecution(
        MonoBehaviour monoBehaviour,
        float delay,
        Action callback
    ) => monoBehaviour.StartCoroutine(Execute(delay, callback));

    private static IEnumerator Execute(float delay, Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }

    public static void Log(
        string message,
        Object context = null,
        string color = "yellow",
        string[] tags = null
    )
    {
        string tag = "";

        if (tags != null && tags.Length > 0)
        {
            foreach (var VARIABLE in tags)
                tag += $"[{VARIABLE}] ";
        }
        else
        {
            tag = "[Log]";
        }

        if (context != null)
            Debug.Log(
                $"<size=8>[SAB]{tag} {context.GetType().Name}</size>=><color={color}><b> {message}</b></color>",
                context
            );
        else
            Debug.Log($"<size=8>[SAB]{tag}</size><color={color}><b> {message}</b></color>");
    }

    public static TextMeshPro CreateTextAtLocation(
        Vector3 location,
        string text,
        float fontSize = 1,
        float? autoDestroyTime = null
    )
    {
        if (!GameManager.Instance.CanUseDebugingFunctions)
            return null;
        GameObject obj = new GameObject("ShowTextAtLocation {text}");
        TextMeshPro textMesh = obj.AddComponent<TextMeshPro>();
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.alignment = TextAlignmentOptions.Center;
        obj.transform.position = location;
        obj.transform.LookAt(Camera.main.transform.position);
        if (autoDestroyTime != null)
            GameObject.Destroy(obj, autoDestroyTime.Value);
        return textMesh;
    }

    public static void Log(
        string message,
        string tag,
        Object context = null,
        string color = "yellow"
    )
    {
        Log(message, context, color, (tag != null ? new string[] { tag } : null));
    }

    public static void Log(
        string message,
        bool useInGameLogAlso,
        Object context = null,
        string color = "yellow",
        string[] tags = null
    )
    {
        if (useInGameLogAlso)
            InGameConsole.Log(message);

        Log(message, context, color, tags);
    }

    public static void LogGameFlow(string message, Object context = null)
    {
        try
        {
            Log(message, context, color: "green", tags: new string[] { "IMP", "GameFlow" });
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LogGameFlow() error:{e}");
        }
    }
}
