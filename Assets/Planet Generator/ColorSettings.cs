using UnityEngine;


[CreateAssetMenu(fileName = "New Color Settings", menuName = "Planet Settings/Color")]
public class ColorSettings : ScriptableObject
{

    [SerializeField]
    private Color color = Color.white;
    public Color GetColor { get { return color; } }
	
}
