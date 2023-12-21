#if UNITY_EDITOR

using UnityEngine.UIElements;



public  class MBSImageVisualElement : Image
{
    public  new class UxmlFactory : UxmlFactory<MBSImageVisualElement, UxmlTraits>
    {
    }
}
#endif