using UnityEngine;
using UnityEngine.InputSystem;

public class RebindLoadSave : MonoBehaviour
{
    public InputActionAsset inputActions;
    // Start is called before the first frame update
    public void OnEnable()
    {
        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
            inputActions.LoadBindingOverridesFromJson(rebinds);
    }
    public void OnDisable()
    {
        var rebinds = inputActions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }
}
