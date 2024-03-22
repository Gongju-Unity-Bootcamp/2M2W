public enum ViewEvent
{
    BeginDrag,
    Drag,
    EndDrag,
    Click,
    DoubleClick,
#if UNITY_ANDROID
    Pinch
#endif
}