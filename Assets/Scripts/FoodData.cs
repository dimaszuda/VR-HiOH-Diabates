using UnityEngine;
using UnityEngine.UI;


public class FoodData : MonoBehaviour
{
    public string foodName;
    [TextArea] public string karboInfo;
    [TextArea] public string GIInfo;
    [TextArea] public string GLInfo;
    [TextArea] public string category;

    public float glucoseRise;

    public Sprite foodIcon;
}
