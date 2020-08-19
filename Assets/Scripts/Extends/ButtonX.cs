using UnityEngine.Events;
using UnityEngine.UI;
using XLua;

[LuaCallCSharp]
public static class ButtonX {
    
    public static void click(this Button button, UnityAction action)
    {
        var onclick = button.onClick;
        if (action != null)
        {
            onclick.RemoveAllListeners();
            onclick.AddListener(action);
        }
        else
        {
            onclick.RemoveAllListeners();
        }
    }
    
}
