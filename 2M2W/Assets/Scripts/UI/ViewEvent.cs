public enum ViewEvent
{
    BeginDrag,
    Drag,
    EndDrag,
    Click,
    DoubleClick,
#if UNITY_ANDROID
    Enable,
    Pinch
#endif
}