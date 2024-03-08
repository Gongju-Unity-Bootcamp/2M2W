using UniRx;
using System;
using Object = UnityEngine.Object;
using UnityEngine.EventSystems;
using UnityEngine;
using System.Runtime.InteropServices.WindowsRuntime;

public static class Extensions
{
    public static T FindChild<T>(GameObject gameObject, string name = null, bool recursive = false) where T : Object
        => Utilities.FindChild<T>(gameObject, name, recursive);

    public static GameObject FindChild(GameObject gameObject, string name = null, bool recursive = false)
        => Utilities.FindChild<Transform>(gameObject, name, recursive).gameObject;

    public static void BindViewEvent(this UIBehaviour view, Action<PointerEventData> action, ViewEvent type, Component component)
        => Utilities.BindViewEvent(view, action, type, component);

    public static void BindModelEvent<T>(this ReactiveProperty<T> model, Action<T> action, Component component)
        => Utilities.BindModelEvent(model, action, component);

    public static SoundType ConvertToSoundType(this SoundID id)
    {
        if (id > SoundID.BGM)
        {
            return SoundType.VFX;
        }

        return SoundType.BGM;
    }
}
