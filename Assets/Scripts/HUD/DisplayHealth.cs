using UnityEngine;
using TMPro;
public class DisplayHealth : MonoBehaviour
{
    [SerializeField] TMP_Text hpDisplay;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        hpDisplay.text = "HP:" + (int)GameManager.instance.GetCurrentPlayerHealth();
    }
}
