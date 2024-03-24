using UniRx.Triggers;
using UniRx;
using System;
using Object = UnityEngine.Object;
using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections.Generic;

public class Utilities : MonoBehaviour
{
    public static T FindChild<T>(GameObject gameObject, string name = null, bool recursive = false) where T : Object
    {
        if (gameObject == null)
        {
            throw new InvalidOperationException();
        }

        if (false == recursive)
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                Transform transform = gameObject.transform.GetChild(i);

                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();

                    if (component != null)
                    {
                        return component;
                    }
                }
            }
        }
        else
        {
            foreach (T component in gameObject.GetComponentsInChildren<T>())
            {
                if (false == string.IsNullOrEmpty(name) && component.name != name)
                {
                    continue;
                }

                return component;
            }
        }

        throw new InvalidOperationException();
    }

    public static GameObject FindChild(GameObject gameObject, string name = null, bool recursive = false)
        => FindChild<Transform>(gameObject, name, recursive).gameObject;

    public static void BindViewEvent(UIBehaviour view, Action<PointerEventData> action, ViewEvent type, Component component)
    {
        switch (type)
        {
#if UNITY_EDITOR
            case ViewEvent.BeginDrag:
                view.OnBeginDragAsObservable().Subscribe(action).AddTo(component);
                break;
            case ViewEvent.Drag:
                view.OnDragAsObservable().Subscribe(action).AddTo(component);
                break;
            case ViewEvent.EndDrag:
                view.OnEndDragAsObservable().Subscribe(action).AddTo(component);
                break;
            case ViewEvent.Click:
                view.OnPointerClickAsObservable().Subscribe(action).AddTo(component);
                break;
            case ViewEvent.DoubleClick:
                IObservable<PointerEventData> e = view.OnPointerClickAsObservable();
                e.Buffer(e.Throttle(TimeSpan.FromMilliseconds(200)))
                 .Where(buffer => buffer.Count >= 2).Subscribe(buffer => action.Invoke(buffer[0])).AddTo(component);
                break;
#elif UNITY_ANDROID
            case ViewEvent.BeginDrag:
                view.OnBeginDragAsObservable()
                .Where(_ => Input.touchCount <= 1)
                .Subscribe(action).AddTo(component);
                break;
            case ViewEvent.Drag:
                view.OnDragAsObservable()
                .Where(_ => Input.touchCount <= 1)
                .Subscribe(action).AddTo(component);
                break;
            case ViewEvent.EndDrag:
                view.OnEndDragAsObservable()
                .Where(_ => Input.touchCount <= 1)
                .Subscribe(action).AddTo(component);
                break;
            case ViewEvent.Click:
                view.OnPointerClickAsObservable()
                .Where(_ => Input.touchCount <= 1)
                .Subscribe(action).AddTo(component);
                break;
            case ViewEvent.DoubleClick:
                IObservable<PointerEventData> e = view.OnPointerClickAsObservable();
                e.Where(_ => Input.touchCount <= 1)
                 .Buffer(e.Throttle(TimeSpan.FromMilliseconds(200)))
                 .Where(buffer => buffer.Count >= 2).Subscribe(buffer => action.Invoke(buffer[0])).AddTo(component);
                break;
            case ViewEvent.Pinch:
                view.OnDragAsObservable()
                .Where(_ => Input.touchCount <= 2 && Managers.App.isPinch)
                .Subscribe(action).AddTo(component);
                break;
#endif
        };
    }

    public static void BindModelEvent<T>(ReactiveProperty<T> model, Action<T> action, Component component)
       => model.Subscribe(action).AddTo(component);

    public static void SwapValue<T>(ref T origin, ref T last)
    {
        T temp = origin;
        origin = last;
        last = temp;
    }

    public static string ConvertToSecondsToTime(int seconds)
    {
        if (seconds == 0)
        {
            return "0초";
        }

        int hours = seconds / 3600;
        int remainingMinutes = (seconds % 3600);
        int minutes = remainingMinutes / 60;
        int remainingSecondsOutput = remainingMinutes % 60;

        string timeString = "";

        if (hours > 0)
        {
            timeString += $"{hours:D2}시간 ";
        }

        if (minutes > 0 || hours > 0)
        {
            timeString += $"{minutes:D2}분 ";
        }

        if (remainingSecondsOutput > 0 || minutes > 0 || hours > 0)
        {
            timeString += $"{remainingSecondsOutput:D2}초";
        }

        return timeString.Trim();
    }

    public static string ConvertToKorean(string mode)
    {
        Dictionary<string, string> modeMap = new Dictionary<string, string>
        {
            { "Driving", "운전" },
            { "Walking", "도보" },
            { "Bicycling", "자전거" },
            { "Transit", "대중교통" }
        };

        if (modeMap.ContainsKey(mode))
        {
            return modeMap[mode];
        }
        else
        {
            return "Unknown travel mode";
        }
    }

    public static string ConvertToDistanceString(double kilometers)
    {
        if (kilometers == 0)
        {
            return "0m";
        }

        string[] units = { "m", "km", "Mm", "Gm" };
        double distance = kilometers * 1000;
        int unitIndex = 0;

        while (distance >= 1000 && unitIndex < units.Length - 1)
        {
            distance /= 1000;
            unitIndex++;
        }

        return $"{distance:N2}{units[unitIndex]}";
    }
}
