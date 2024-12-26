using System.Collections.Generic;
using UnityEngine;

public static class WatcherWraper
{
    public static Dictionary<string, string> watchList_string /*{ get; private set; }*/ = new();
    public static Dictionary<string, Object> watchList_object /*{ get; private set; }*/ = new();
    public static Dictionary<string, bool> watchList_boolean /*{ get; private set; }*/ = new();

    public static void WatchString(string name, string value)
    {
        if (watchList_string == null) watchList_string = new();
        watchList_string[name] = value;
    }

    public static void WatchObject(string name, Object value)
    {
        if (watchList_object == null) watchList_object = new();
        watchList_object[name] = value;
    }

    public static void WatchBoolean(string name, bool value)
    {
        if (watchList_boolean == null) watchList_boolean = new();
        watchList_boolean[name] = value;
    }

    public static bool IsAnyWatcherAvailable()
    {
        if (watchList_string == null) watchList_string = new();
        if (watchList_object == null) watchList_object = new();
        if (watchList_boolean == null) watchList_boolean = new();
        return watchList_object.Keys.Count > 0 || watchList_string.Keys.Count > 0 || watchList_boolean.Count > 0;
    }

    public static void ClearAll()
    {
        watchList_string = new();
        watchList_object = new();
        watchList_boolean = new();
    }
}