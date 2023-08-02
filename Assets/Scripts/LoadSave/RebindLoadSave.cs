using UnityEngine;
using UnityEngine.InputSystem;

public class RebindLoadSave : MonoBehaviour
{
    public void OnDisable()
    {
        SaveKeybind();
    }
    public static void LoadKeybind()
    {
        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
            NewOptions.instance.inputActions.LoadBindingOverridesFromJson(rebinds);
    }
    public static void SaveKeybind()
    {
        var rebinds = NewOptions.instance.inputActions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }
}
