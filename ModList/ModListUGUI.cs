using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModCore.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModList;

public class ModListUGUI : MonoBehaviour
{
    public static void Create(MainMenu menu)
    {
        if (!menu) return;

        menu.StartCoroutine(MakeUI(menu));
    }

    private static IEnumerator MakeUI(MainMenu menu)
    {
        var listPrefab = UIManager.GetPrefab<ModListUGUI>("ModList_UI");
        if (!listPrefab)
        {
            listPrefab = MakeList(menu);
            UIManager.RegisterPrefab("ModList_UI", listPrefab);
        }

        var mainBtnPrefab = UIManager.GetPrefabAsGameObject("ModList_MainButton");
        if (!mainBtnPrefab)
        {
            mainBtnPrefab = MakeMainButton(menu);
            UIManager.RegisterPrefab("ModList_MainButton", mainBtnPrefab);
        }

        yield return null;

        var list = Instantiate(listPrefab, menu.transform);
        list.name = "ModList";
        list.transform.SetSiblingIndex(8);

        var mainBtn = Instantiate(mainBtnPrefab, menu.transform.Find("Main/Splash"));
        mainBtn.name = "ModListButton";

        var helper = menu.MenuNavigation;
        var target = helper.Groups[3].TargetObjects.ToArray();
        target[0] = list.gameObject;

        var group = new MenuGroup
        {
            GroupName = "ModList",
            TargetObjects = target
        };

        var index = helper.Groups.Length;
        helper.Groups = helper.Groups.Append(group).ToArray();

        list.GetComponent<Button>("BackButton/ButtonObject").onClick.AddListener(() => helper.SetGroupActive(0));
        mainBtn.GetComponent<Button>("ButtonObject").onClick.AddListener(() => helper.SetGroupActive(index));
    }

    private static ModListUGUI MakeList(MainMenu menu)
    {
        var origin = menu.transform.Find("SelectCharacter")?.gameObject;
        if (!origin) return null;

        var prefab = Instantiate(origin);
        var transform = prefab.transform;
        var mb = prefab.AddComponent<ModListUGUI>();
        prefab.name = "ModList";
        mb.ModPerkPrefab = menu.PerkPreviewPrefab;

        var spriteWhite = transform.GetComponent<Image>("BackButton/ButtonFill").sprite;

        string[] delGoList =
        [
            "EasyModePopup",
            "SunsAndMoons",
            "Details/CreateCharacterScreen",
            "Details/StartGameScreen/PurchaseGroup",
            "Details/StartGameScreen/StartGroup/StartGame",
            "Details/StartGameScreen/StartGroup/EditButton",
            "Details/StartGameScreen/StartGroup/TrashButton",
            "Details/StartGameScreen/StartGroup/CannotPlayInDemo",
            "Details/StartGameScreen/StartGroup/PackageOptions/EasyPackageToggle",
            "List/PopupBG/Title/Text",
            "List/PopupBG/Title/Tabs/CustomCharacters",
            "List/PopupBG/VerticalScrollView/Viewport/CharacterSelectList/SelectCharacterCreation",
            "List/PopupBG/VerticalScrollView/Viewport/CharacterSelectList/SelectCharacterCreation/Image",
            "List/PopupBG/VerticalScrollView/Viewport/CharacterSelectList/SelectCharacterCreation/LockedIcon",
            "List/PopupBG/VerticalScrollView/Viewport/CharacterSelectList/SelectCharacterCreation/PulsingOutline",
            "List/PopupBG/VerticalScrollView/Viewport/CharacterSelectList/SelectCharacterCreation/Frame/LockedIcon_InFrame",
            "List/PopupBG/VerticalScrollView/Viewport/CharacterSelectList/SelectCharacterCreation/Frame/Mask/CharacterPortrait/Plus",
        ];

        DestroyGameObjects(transform, delGoList);

        var titleText = transform.Find("TitleText");
        titleText.localPosition = new Vector3(80, -40);

        var filterTabAll = transform.Find("List/PopupBG/Title/Tabs/OfficialCharacters");
        filterTabAll.name = "FilterTab";
        filterTabAll.Find("Icon").gameObject.SetActive(false);
        var filterTabAllText = filterTabAll.Find("ButtonText").gameObject;
        filterTabAllText.SetActive(true);

        var filterTabDll = Instantiate(filterTabAll, filterTabAll.parent);
        var filterTabMe = Instantiate(filterTabAll, filterTabAll.parent);
        var filterTabDllMe = Instantiate(filterTabAll, filterTabAll.parent);

        mb.FilterButtons =
        [
            filterTabAll.GetComponent<IndexButton>(), filterTabDll.GetComponent<IndexButton>(),
            filterTabMe.GetComponent<IndexButton>(), filterTabDllMe.GetComponent<IndexButton>()
        ];

        var modSelectList = transform.Find("List/PopupBG/VerticalScrollView/Viewport/CharacterSelectList");
        modSelectList.name = "ModSelectList";
        mb.ModButtonParent = modSelectList;

        var charBtn = Instantiate(menu.CharacterSelectButtonPrefab);
        var modBtn = charBtn.gameObject.AddComponent<ModButton>();
        modBtn.name = "ModSelectButton";
        modBtn.Init(charBtn);
        DestroyImmediate(charBtn);
        DestroyGameObjects(modBtn.transform, [
            "Image",
            "LockedIcon",
            "PulsingOutline",
            "Frame/LockedIcon_InFrame",
            "Frame/Mask/CharacterPortrait/Plus",
        ]);
        modBtn.Text = "模组名称";
        modBtn.Icon.sprite = spriteWhite;
        modBtn.GetComponent<Image>("Frame/BG").sprite = null;
        mb.ModButtonPrefab = modBtn;
        UIManager.RegisterPrefab("ModList_ModButton", modBtn);

        var modInfoScreen = transform.Find("Details/StartGameScreen");
        modInfoScreen.name = "ModInfoScreen";

        var modName = modInfoScreen.GetComponent<TextMeshProUGUI>("CharacterName");
        modName.name = "ModName";
        modName.text = "模组名称";
        mb.ModNameText = modName;

        var modVersion = modName.GetComponent<TextMeshProUGUI>("CharacterDifficulty");
        modVersion.name = "ModVersion";
        modVersion.text = "版本号";
        mb.ModVersionText = modVersion;

        var modType = Instantiate(modVersion, modName.transform);
        modType.name = "ModType";
        modType.text = "类型";
        modType.transform.localPosition = new Vector3(-1630, 0);
        modType.alignment = TextAlignmentOptions.Left;
        mb.ModTypeText = modType;

        var ctrlGroup = modInfoScreen.Find("StartGroup");
        ctrlGroup.name = "ControlGroup";
        ctrlGroup.localPosition = new Vector3(0, 45);

        var enableToggleTp = ctrlGroup.Find("PackageOptions/SafeModeToggle").GetComponent<LocalizedStaticTooltip>();
        enableToggleTp.TooltipTitle = new LocalizedString();
        enableToggleTp.TooltipContent = new LocalizedString
        {
            DefaultText = "这只是一个饼",
            LocalizationKey = $"{Localization.KeyPrefix}EnableToggleTooltipContent"
        };

        var enableToggle = enableToggleTp.GetComponent<Toggle>();
        enableToggle.interactable = false;
        mb.ModEnableToggle = enableToggle;

        var modItems = modInfoScreen.Find("StartingItems");
        modItems.name = "ModItems";
        modItems.localPosition = new Vector3(0, -650);

        var modPerkTitle = modItems.Find("ItemsFrame/StartingConditionsText");
        modPerkTitle.name = "ModPerkTitle";

        var modPerks = modItems.Find("ItemsFrame/HorizontalScrollView/Viewport/CharacterStartingCards");
        modPerks.name = "ModPerks";
        modPerks.gameObject.AddComponent<NonDrawingGraphic>();
        mb.ModPerkParent = modPerks;

        var imageAndDesc = modInfoScreen.Find("ImageAndDesc");
        ((RectTransform)imageAndDesc.transform).sizeDelta = new Vector2(-40, 410);

        var imageFrame = imageAndDesc.Find("ImageFrame");
        imageFrame.localPosition = new Vector3(0, -55);

        imageFrame.GetComponent<Image>("BG").sprite = null;

        var modImg = imageFrame.GetComponent<Image>("Mask/CharacterPortrait");
        modImg.name = "ModImage";
        modImg.sprite = spriteWhite;
        mb.ModImage = modImg;

        var modDesc = imageAndDesc.GetComponent<TextMeshProUGUI>("CharacterDesc");
        modDesc.name = "ModDesc";
        modDesc.text = "模组简介";
        mb.ModDesc = modDesc;

        SetLocalizedText(transform.Find("TitleText").gameObject, "模组列表", "TitleText");
        SetLocalizedText(filterTabAllText, "全部", "FilterTabAll");
        SetLocalizedText(filterTabDll.Find("ButtonText").gameObject, "DLL", "FilterTabDLL");
        SetLocalizedText(filterTabMe.Find("ButtonText").gameObject, "ME", "FilterTabME");
        SetLocalizedText(filterTabDllMe.Find("ButtonText").gameObject, "DLL ME", "FilterTabDLLME");
        SetLocalizedText(enableToggleTp.transform.Find("ButtonText").gameObject, "启用模组", "EnableToggleText");
        SetLocalizedText(modPerkTitle.gameObject, "模组特质", "ModPerkTitle");

        return mb;
    }

    private static GameObject MakeMainButton(Component menu)
    {
        var origin = menu.transform.Find("Main/Splash/CreditsButton")?.gameObject;
        if (!origin) return null;

        var prefab = Instantiate(origin);
        prefab.name = "ModListButton";
        prefab.transform.localPosition = new Vector3(-853, -54);

        prefab.GetComponent<TextMeshProUGUI>("ButtonText").text = "M";
        prefab.GetComponent<Button>("ButtonObject").onClick = new Button.ButtonClickedEvent();

        return prefab;
    }

    private static void DestroyGameObjects(Transform transform, IEnumerable<string> paths)
    {
        foreach (var path in paths)
        {
            var go = transform.Find(path)?.gameObject;
            if (go) Destroy(go);
        }
    }

    private static void SetLocalizedText(GameObject go, string def, string key)
    {
        var textMeshProUGUI = go?.GetComponent<TextMeshProUGUI>();
        if (!textMeshProUGUI) return;

        textMeshProUGUI.text = def;

        var localizedStaticText = go.GetComponent<LocalizedStaticText>() ?? go.AddComponent<LocalizedStaticText>();
        localizedStaticText.LocalizedStringKey = $"{Localization.KeyPrefix}{key}";
    }

    [SerializeField] private IndexButton[] FilterButtons;

    [SerializeField] private Transform ModButtonParent;

    [SerializeField] private ModButton ModButtonPrefab;

    [SerializeField] private TextMeshProUGUI ModNameText;

    [SerializeField] private TextMeshProUGUI ModVersionText;

    [SerializeField] private TextMeshProUGUI ModTypeText;

    [SerializeField] private Image ModImage;

    [SerializeField] private TextMeshProUGUI ModDesc;

    [SerializeField] private Toggle ModEnableToggle;

    [SerializeField] private Transform ModPerkParent;

    [SerializeField] private MenuPerkPreview ModPerkPrefab;

    private Mod[] _mods;

    private ModButton[] _modButtons;

    private int _currentSelectMod = -1;

    private readonly List<MenuPerkPreview> _perkPreviews = [];

    private void Awake()
    {
        for (var i = 0; i < FilterButtons.Length; i++)
        {
            var btn = FilterButtons[i];
            btn.Index = i;
            btn.Selected = false;
            btn.OnClicked += SelectModTab;
        }

        Setup(ModManager.ModMetas);
        SelectModTab(0);
        SelectMod(0);
    }

    private void SelectModTab(int index)
    {
        foreach (var btn in FilterButtons)
        {
            btn.Selected = btn.Index == index;
        }

        foreach (var btn in _modButtons)
        {
            var mod = btn.Mod;
            btn.Active = index switch
            {
                1 => mod.IsDLL,
                2 => mod.IsMe,
                3 => mod.IsDLL && mod.IsMe,
                _ => true
            };
        }
    }

    private void Setup(IReadOnlyList<ModMeta> metas)
    {
        _mods = new Mod[metas.Count];
        _modButtons = new ModButton[metas.Count];

        for (var i = 0; i < metas.Count; i++)
        {
            var mod = new Mod(metas[i]);
            _mods[i] = mod;
            _modButtons[i] = AddModButton(i, mod);
        }
    }

    private ModButton AddModButton(int index, Mod mod)
    {
        var btn = Instantiate(ModButtonPrefab, ModButtonParent);
        btn.Setup(index, mod);
        btn.OnClicked += SelectMod;
        return btn;
    }

    private void SelectMod(int index)
    {
        if (index >= _mods.Length || index == _currentSelectMod) return;

        _currentSelectMod = index;

        foreach (var button in _modButtons)
        {
            button.Selected = button.Index == index;
        }

        var mod = _mods[index];
        ModNameText.text = mod.Name;
        ModVersionText.text = mod.Version;
        ModImage.overrideSprite = mod.Icon;
        ModEnableToggle.isOn = mod.Enabled;

        ModTypeText.text = mod.IsDLL switch
        {
            true when mod.IsMe => $"{Localization.ModTypeDLL}&{Localization.ModTypeMe}",
            true => Localization.ModTypeDLL,
            _ => mod.IsMe ? Localization.ModTypeMe : ""
        };

        var desc = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(mod.Author)) desc.Append($"{Localization.ModAuthor}: {mod.Author}\n");
        desc.Append(mod.Desc);
        ModDesc.text = desc.ToString();

        SetupModPerks(mod.Perks);
    }

    private void SetupModPerks(CharacterPerk[] perks)
    {
        foreach (var preview in _perkPreviews)
        {
            preview.gameObject.SetActive(false);
        }

        if (perks is null) return;

        for (var i = 0; i < perks.Length; i++)
        {
            if (i >= _perkPreviews.Count) _perkPreviews.Add(Instantiate(ModPerkPrefab, ModPerkParent));

            var preview = _perkPreviews[i];
            preview.Setup(perks[i]);
            preview.gameObject.SetActive(true);
        }
    }
}