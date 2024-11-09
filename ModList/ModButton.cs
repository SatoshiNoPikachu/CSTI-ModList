using UnityEngine.UI;

namespace ModList;

public class ModButton : IndexButton
{
    public Mod Mod { get; private set; }

    public bool Active
    {
        get => gameObject.activeSelf;
        set => gameObject.SetActive(value);
    }

    public void Init(IndexButton source)
    {
        Icon = source.Icon;
        OnSelected = source.OnSelected;
        ButtonText = source.ButtonText;
        Group = source.Group;

        Icon.name = "ModImage";
    }

    private void Awake()
    {
        var modBtnObj = this.GetComponent<Button>("ButtonObject");
        modBtnObj.onClick = new Button.ButtonClickedEvent();
        modBtnObj.onClick.AddListener(Click);
    }

    public void Setup(int index, Mod mod)
    {
        Mod = mod;

        var modName = mod.Name;
        if (string.IsNullOrWhiteSpace(modName)) modName = Localization.NoName;
        var enableText = mod.Enabled ? Localization.Enabled : Localization.NotEnabled;

        Selected = false;

        if (mod.Icon) Sprite = mod.Icon;

        base.Setup(index, $"{modName}\n<size=65%>{enableText}", "", false);
    }
}