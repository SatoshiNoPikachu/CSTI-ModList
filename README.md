# 模组列表（Mod List）- CSTI MOD



## 简介

模组列表（Mod List）是一个功能类Mod，为游戏添加了模组列表UI界面，帮助玩家浏览已安装模组的信息。



当前版本：1.1.0

By.サトシの皮卡丘



## 安装说明

请将Mod压缩包解压于 BepInEx\plugins 文件夹下。



## 使用说明

> Tips：该内容供Mod作者阅读。
>

> Tips：“Mod” 的表述指您所制作的 Mod。
>



### 创建模组元数据文件

模组元数据文件（ModMeta.json）是JSON格式文件，描述了Mod的一些信息，模组列表将优先使用这些信息在模组列表中展示Mod。

对于DLL模组（即BepInEx插件），在DLL文件所在目录下创建元数据文件。注意，模组列表仅会读取“plugins”目录的子目录下的元数据文件。

对于ME模组，或是DLL与ME组合模组，在Mod根目录（即“ModInfo.json”所在目录）下创建元数据文件。注意，模组列表不支持从ZIP压缩包加载方式中读取元数据文件！



#### 模组元数据文件模板

```json
{
  "Name": {},
  "Description": {},
  "Author": "",
  "Version": "",
  "Icon": "",
  "PluginGuid": []
}
```

> Tips：字段多为可选字段，如有不需要使用的字段，可以将其移除。



### Name（本地化模组名称）

`Name`字段用于提供模组的本地化名称。可以设置多种语言的名称，以便在不同语言环境中显示对应的名称。

该字段需以键值对的形式编写，其中键为与语言对应的游戏本地化文件名称，值为模组名称的字符串。

若不提供某语言的本地化名称，则无需编写，将会优先显示首条本地化名称。

> Tips：仅支持游戏当前的可选语言。

该字段为可选字段。当该字段为空时，将会根据模组类型显示名称：

- DLL模组显示插件名称（BepInPlugin特性的第二个参数）。
- ME模组显示“ModInfo.json”文件中的“Name”值。
- DLL与ME组合模组优先显示ME名称。

示例：

```json
"Name": {
  "SimpCn": "模组列表",
  "En": "Mod List"
}
```



#### 本地化名称与语言名称对应参考表

| 本地化名称 | 语言名称            |
| ---------- | ------------------- |
| En         | English（英语）     |
| SimpCn     | 简体中文            |
| Es         | Español（西班牙语） |
| Kr         | 한국어（韩语）      |
| De         | Deutsch（德语）     |
| Tr         | Türkçe（土耳其语）  |
| Jp         | 日本語（日语）      |
| Hun        | Magyar（匈牙利语）  |
| Fr         | Français（法语）    |



### Description（本地化描述）

`Description`字段用于提供模组的本地化描述，同样支持多语言设置。可以用此字段来向玩家简要介绍模组的功能和内容。格式与`Name`字段相同。

该字段为可选字段。

示例：

```json
"Description": {
  "SimpCn": "为游戏添加了模组列表UI界面，帮助玩家浏览已安装模组的信息。",
  "En": "Mod List is a functional mod that adds a mod list UI to the game, helping players browse information about installed mods."
}
```



### Author（作者）

`Author`字段为模组的作者信息，该字段值类型为字符串。

该字段为可选字段。当该字段为空时：

- DLL模组不显示作者信息。
- ME模组显示“ModInfo.json”文件中的`Author`值。
- DLL与ME组合模组显示ME作者信息。

示例：

```json
"Author": "サトシの皮卡丘"
```



### Version（版本）

`Version`字段表示模组的版本号，以便玩家和开发者了解当前的版本进度和更新情况。建议使用"主版本.次版本.修订版"的格式，例如`"1.1.0"`。

该字段为可选字段。当该字段为空时：

- DLL模组显示插件版本（BepInPlugin特性的第三个参数）。
- ME模组显示“ModInfo.json”文件中的`Version`值。
- DLL与ME组合模组优先显示ME版本信息。

示例：

```json
"Version": "1.1.0"
```



### Icon（图标）

`Icon`字段用于设置模组的图标，值为相对路径字符串。支持文件格式`png`和`jpg`。

该字段为可选字段。

示例：

```json
"Icon": "ModIcon.png"
```



### PluginGuid（插件GUID）

`PluginGuid`是BepInEx插件的GUID（BepInPlugin特性的第一个参数）列表。

该字段为可选字段，ME模组和单插件（仅有一个GUID）模组无需编写该字段。

示例：

```json
"PluginGuid": [
  "Pikachu.CSTI.ModA",
  "Pikachu.CSTI.ModB"
]
```



## 更新日志

### Version 1.1.0

1. 优化了名称和描述的本地化文本显示逻辑。
2. 将BepInEx作为DLL模组加入模组列表。
3. 加入了模组排序规则。
4. 图标的背景图像改为了白色。

