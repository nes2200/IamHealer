using UnityEngine;

//[createassetmenu(filename = "infocontainer", menuname = "scriptable objects/infocontainer")]
public abstract class InfoContainer : ScriptableObject
{
    public Sprite icon;
    public string displayName;
    public string explain;
}
