# Updating to Nautilus

In this article, we will be talking about the necessary changes you must apply to update your mod to Nautilus from SMLHelper 2.0. 

## Namespace
The root namespace for Nautilus is not the same as SMLHelper 2.0.
<pre class="lang-diff">
<span class="lang-diff-rem">- &lt;RootNamespace&gt;SMLHelper.V2&lt;/RootNamespace&gt;</span>
<span class="lang-diff-add">+ &lt;RootNamespace&gt;Nautilus&lt;/RootNamespace&gt;</span>
</pre>


## Handlers

Handlers no longer implement an interface matching their name. Additionally, they're now `public static`.  
This means they also no longer have a public static `Main` property anymore, so you will have to drop it from anywhere
mentioned in your code.  

### Handler.cs
Following the handler interfaces change, the overly under-used `Handler` class will leave us in Nautilus
<pre class="lang-diff">
// Handler.cs
<span class="lang-diff-rem">
- namespace SMLHelper.V2
- {
-     using Interfaces;
- 
-     /// &lt;summary&gt;
-     /// A simple location where every SMLHelper handler class can be accessed.
-     /// &lt;/summary&gt;
-     public static class Handler
-     {
-         public static IBioReactorHandler BioReactorHandler => Handlers.BioReactorHandler.Main;
- 
-         public static ICraftDataHandler CraftDataHandler => Handlers.CraftDataHandler.Main;
- 
-         public static ICraftTreeHandler CraftTreeHandler => Handlers.CraftTreeHandler.Main;
- 
-         public static IIngameMenuHandler IngameMenuHandler => Handlers.IngameMenuHandler.Main;
-         ...
-     }
</span>
</pre>  

### BioReactorHandler
The `BioReactorHandler` class is removed in Nautilus because it only had one very simple method to patch, and was forcing patch-time. That means if you tried to modify a bio charge _after_
SML's entry point, it didn't get applied.  
  
The following example demonstrates how you can implement the same functionality the `BioReactorHandler` class offered.
<pre class="lang-diff">
<span class="lang-diff-rem">- BioReactorHandler.SetBioreactorCharge(TechType.Peeper, 69f);</span>
<span class="lang-diff-add">+ BaseBioReactor.charge[TechType.Peeper] = 69f;</span>
</pre>

### FishHandler
Ever since this class has been added, it never received any further updates due to unpopularity among modders, and unfamiliarity with how creatures worked in general and thus, has been broken for a long time.  
The `FishHandler` has been removed in Nautilus. At the time being, we have not added a system to replace it, so stay tuned for that.  

### PDAEncyclopediaHandler And PDALogHandler  
Beginning with Nautilus, both of these handler methods were moved to `PDAHandler` as they only had one method each.
<pre class="lang-diff">
PDAEncyclopedia.EntryData entry = new PDAEncyclopedia.EntryData()
{
  key = "SomeEncy",
  path = "Tech/Tools",
  nodes = new[] { "Tech", "Tools" }
};

<span class="lang-diff-rem">- PDAEncyclopediaHandler.AddCustomEntry(entry);</span>
<span class="lang-diff-add">+ PDAHandler.AddEncyclopediaEntry(entry);</span>

<span class="lang-diff-rem">- PDALogHandler.AddCustomEntry("SomeLog", "SomeLanguageKey");</span>
<span class="lang-diff-add">+ PDAHandler.AddLogEntry("SomeLog", "SomeLanguageKey");</span>
</pre>

### InGameMenuHandler
The methods `InGameMenuHandler` class had have been moved to the `Nautilus.Utility` namespace and the class has been renamed to `SaveUtils`.
<pre class="lang-diff">
<span class="lang-diff-rem">- InGameMenuHandler.RegisterOnSaveEvent(() => ErrorMessage.AddMessage("We do be saving!"));</span>
<span class="lang-diff-add">+ SaveUtils.RegisterOnSaveEvent(() => ErrorMessage.AddMessage("We do be saving!"));</span>
</pre>

## Enum Handlers
Beginning with Nautilus, enum handling will be made generic. Instead of working with individual handlers associated with the enum type (E.G: For `TechType` we had `TechTypeHandler`),
Now you can add a new enum value to any enum type by using `EnumHandler.AddEntry<TEnum>(string enumName)`.  

This means the following handlers are removed:
* `TechTypeHandler`
* `BackgroundTypeHandler`
* `EquipmentHandler`
* `PingTypeHandler`
* `TechCategoryHandler`
* `TechGroupHandler`
* `CraftTreeTypeHandler` - Only the methods below are removed:
  1. `CreateCustomCraftTreeAndType`
  2. `ModdedCraftTreeTypeExists`  
  
  
The [EnumHandler](xref:Nautilus.Handlers.EnumHandler) class contains the following methods, all of them can be used for any enum type:

| Signature                                                                  | Summary                                                                                   |
|----------------------------------------------------------------------------|-------------------------------------------------------------------------------------------|
| `EnumBuilder<TEnum> AddEntry<TEnum>(string name)`                          | Adds a new enum value instance of TEnum type.                                             |
| `bool ModdedEnumExists<TEnum>(string name)`                                | Safely looks for a custom enum object from another mod.                                   |
| `bool TryGetValue<TEnum>(string name, out TEnum enumValue)`                | Safely looks for a custom enum object from another mod and outputs the instance if found. |


> [!NOTE]
> The [EnumHandler](xref:Nautilus.Handlers.EnumHandler) class only takes care of registering a new enum object for an enum type. Further configuration is now handled via extension methods for the [EnumBuilder<TEnum>](xref:Nautilus.Handlers.EnumBuilder`1) type.

Below we will talk about the necessary changes you will need to make your custom enum values work for each of the aforementioned handlers.  


### Configuring Custom TechType Objects
<pre class="lang-diff">
<span class="lang-diff-rem">- TechType customTech = TechTypeHandler.AddTechType("CustomTech", "Custom Tech", "Custom Tech that makes me go yes.", SpriteManager.Get(TechType.Titanium), unlockedAtStart: false);</span>
<span class="lang-diff-add">+ TechType customTech = EnumHandler.AddEntry&lt;TechType&gt;("CustomTech")
+             .WithPdaInfo("Custom Tech", "Custom Tech that makes me go yes.", unlockedAtStart: false)
+             .WithIcon(SpriteManager.Get(TechType.Titanium));</span>
</pre>

### Configuring Custom CraftData.BackgroundType Objects
<pre class="lang-diff">
<span class="lang-diff-rem">- CraftData.BackgroundType customBG = BackgroundTypeHandler.AddBackgroundType("CustomBackground", SpriteManager.GetBackground(TechType.Battery));</span>
<span class="lang-diff-add">+ CraftData.BackgroundType customBG = EnumHandler.AddEntry&lt;CraftData.BackgroundType&gt;("CustomBackground")
+             .WithBackground(SpriteManager.GetBackground(TechType.Battery));</span>
</pre>

### Configuring Custom EquipmentType Objects
<pre class="lang-diff">
<span class="lang-diff-rem">- EquipmentType customEquipment = EquipmentHandler.AddEquipmentType("CustomEquipment");</span>
<span class="lang-diff-add">+ EquipmentType customEquipment = EnumHandler.AddEntry&lt;EquipmentType&gt;("CustomEquipment");</span>
</pre>

### Configuring Custom PingType Objects
<pre class="lang-diff">
<span class="lang-diff-rem">- PingType customPing = PingHandler.RegisterNewPingType("CustomPing", SpriteManager.Get(SpriteManager.Group.Pings, PingType.Signal.ToString()));</span>
<span class="lang-diff-add">+ PingType customPing = EnumHandler.AddEntry&lt;PingType&gt;("CustomPing")
+             .WithIcon(SpriteManager.Get(SpriteManager.Group.Pings, PingType.Signal.ToString()));</span>
</pre>

### Configuring Custom TechCategory and TechGroup Objects
<pre class="lang-diff">
<span class="lang-diff-rem">- TechGroup customGroup = TechGroupHandler.AddTechCategory("CustomGroup", "Custom Group");</span>
<span class="lang-diff-add">+ TechGroup customGroup = EnumHandler.AddEntry&lt;TechCategory&gt;("CustomGroup").WithPdaInfo("Custom Group");</span>

<span class="lang-diff-rem">- TechCategory customCategory = TechCategoryHandler.AddTechCategory("CustomCategory", "Custom Category");
- TechCategoryHandler.TryRegisterTechCategoryToTechGroup(customGroup, customCategory);</span>
<span class="lang-diff-add">+ TechCategory customCategory = EnumHandler.AddEntry&lt;TechCategory&gt;("CustomCategory").WithPdaInfo("Custom Group")
+             .RegisterToTechGroup(customGroup);</span>
</pre>

### Configuring Custom CraftTree.Type Objects
<pre class="lang-diff">
<span class="lang-diff-rem">- ModCraftTreeRoot root = CraftTreeHandler.CreateCustomCraftTreeAndType(CustomTree, out CraftTree.Type customTree);</span>
<span class="lang-diff-add">+ CraftTree.Type customTree = EnumHandler.AddEntry&lt;CraftTree.Type&gt;("CustomTree")
+             .CreateCraftTreeRoot(out ModCraftTreeRoot root);</span>

root.AddTabNode("SomeTab");
</pre>

___

## Options
The Options system backend was largely changed in Nautilus. This rewrite mostly effects the more in-depth options systems leaving the simplest usage(s) more or less untouched. Mods which made use of the `ConfigFile` attribute system should not require major changes.

Mods which extended `ModOptions` to create their config system will require changes:
  - All individual `AddXYZOption` methods have been replaced by a single unified generic `AddItem` method.
    - This method takes a `ModOption` which can be created using `ModXYZOption.Create(...)`.
  - All individual `Options_XXYZChanged` methods have been replaced by a single unified `OnChanged` method.
  - Option specific OnChanged events can be added to each option instead of being forced to use the global OnChange.
  - `ModChoiceOption` has been made into a generic type `ModChoiceOption<T>` which can support an array of almost any type and enums.

In addition to these Nautilus specific methods there have also been extensions provided to directly create Nautilus `OptionItem` instances from BepInEx `ConfigEntry` instances:

```csharp
var bepInExToggle = cfg.Bind<bool>(
    section: "Testing boolean",
    key: "A boolean",
    defaultValue: true
);
AddItem(bepInExToggle.ToModToggleOption());
```

### SML 2.0
```csharp
public class ModOptionsV2 : ModOptions
{
    public ModOptionsV2() : base("My Mod Options")
    {
        OptionsPanelHandler.RegisterModOptions(this);

        SliderChanged += Options_SliderChanged;
        ChoiceChanged += Options_ChoiceChagned;
    }

    public override void BuildModOptions()
    {
        AddSliderOption(id: "Foo", label: "Bar", minValue: 0, maxValue: 100, value: 50);
        AddChoiceOption(id: "Baz", label: "Qux", options: new[] { "ABC", "DEF", "XYZ" }, index: 0);
    }

    private void Options_SliderChanged(object sender, SliderChangedEventArgs e)
    {
        switch (e.Id)
        {
            case "Foo":
                // Do stuff here
                break;
        }
    }

    private void Options_ChoiceChagned(object sender, ChoiceChangedEventArgs e)
    {
        switch (e.Id)
        {
            case "Baz":
                // Do stuff here
                break;
        }
    }
}
```
### Nautilus
```csharp
public class ModOptionsV3 : ModOptions
{
    public ModOptionsV3() : base("My Mod Options")
    {
        OptionsPanelHandler.RegisterModOptions(this);

        OnChanged += GlobalOptions_Changed;

        var sliderWithChange = ModSliderOption.Create(id: "Fancy", label: "Slider", minValue: 0, maxValue: 100, value: 50);
        sliderWithChange.OnChanged += specific_OnChanged;
        AddItem(sliderWithChange);

        AddItem(ModSliderOption.Create(id: "Foo", label: "Bar", minValue: 0, maxValue: 100, value: 50));
        AddItem(ModChoiceOption<string>.Create(id: "Baz", label: "Qux", options: new[] { "ABC", "DEF", "XYZ" }, index: 0));
    }

    private void specific_OnChanged(object sender, SliderChangedEventArgs e)
    {
        // Do onChange here
    }

    private void GlobalOptions_Changed(object sender, OptionEventArgs e)
    {
        switch (e)
        {
            case SliderChangedEventArgs sliderArgs:
                switch (sliderArgs.Id)
                {
                    case "Foo":
                        // Do stuff here
                        break;
                }
                break;
            case ChoiceChangedEventArgs<string> choiceArgs:
                switch (choiceArgs.Id)
                {
                    case "Baz":
                        // Do stuff here
                        break;
                }
                break;
        }
    }
}
```
___

## Assets 
The Assets system received a complete rewrite in Nautilus, making it the biggest change of this version. 

With this rewrite, asset classes are no longer an inherited chain mess, meaning `Buildable`, `Craftable`, `CustomFabricator`, `Equipable`, `FishPrefab`, `PdaItem`, `Spawnable` and `ModPrefab` classes have been removed.  

As of Nautilus, the asset system will have three main parts: Custom Prefabs, Gadgets, and Prefab Templates.

The following table represents all the previous asset classes and what they have been replaced with in Nautilus.

| SML 2.0 (old)          | Nautilus (new)                                                      |
|------------------------|--------------------------------------------------------------------|
| `ModPrefab`            | [CustomPrefab](xref:Nautilus.Assets.CustomPrefab)                 |
| `Buildable`, `PdaItem` | [ScanningGadget](xref:Nautilus.Assets.Gadgets.ScanningGadget)     |
| `Equipable`            | [EquipmentGadget](xref:Nautilus.Assets.Gadgets.EquipmentGadget)   |
| `CustomFabricator`     | [FabricatorGadget](xref:Nautilus.Assets.Gadgets.FabricatorGadget) |
| `Craftable`            | [CraftingGadget](xref:Nautilus.Assets.Gadgets.GadgetExtensions#)  |
| `Spawnable`            | `ICustomPrefab.SetSpawns`                                          |


### Custom Prefabs
`CustomPrefab` is a class that takes care of registering gadgets and also the game object into the game.  
This class is essentially equivalent to the previous `ModPrefab` class. It is what you will use to actually make a custom prefab.  

### Gadgets
To put it simply, Gadgets are classes that take certain data and register them to the game for our custom prefab item.  
They are pretty much equivalent to the different asset classes and their properties we had before.  
Gadgets will be our primary way of interacting with game systems and to add functionality to a tech type and/or class ID.  

> [!NOTE]
> Gadgets only interact with tech types and/or class IDs. They don't have any business with a prefab's game object.

### Prefab Templates
Previously on SML 2.0, asset classes optionally also provided game objects (E.G: `CustomFabricator`). To allow for diversity in the game object template you choose from
and also to make it easier to manage such functionality and modularize game objects, we have moved game object templates to their own system: Prefab templates.  

Prefab templates will be our main way of providing a base game object for custom prefabs. There are a couple of options you can choose from that suit your needs, however, it is not enforced to choose one;
you can still build up a game object from scratch.  

A couple of prefab templates that will be available in Nautilus are the following:
  - CloneTemplate
  - EnergySourceTemplate
  - FabricatorTemplate


## Custom Prefab Examples
In this example, we will demonstrate how you can change an SML 2.0 custom prefab to the Nautilus system.

# [Equipable](#tab/equippable)

### SML 2.0
```csharp
public class SeamothBrineResistanceModule : Equipable
{
    public static TechType TechTypeID { get; protected set; }
    public SeamothBrineResistanceModule()
        : base("SeamothBrineResistModule",
              "Seamoth brine resistant coating",
              "Makes the Seamoth resistant to corrosive brine pools, by means of a protective coating.")
    {
        OnFinishedPatching += () =>
        {
            TechTypeID = this.TechType;
        };
    }
    public override EquipmentType EquipmentType => EquipmentType.SeamothModule;
    public override TechType RequiredForUnlock => TechType.BaseUpgradeConsole;
    public override TechGroup GroupForPDA => TechGroup.VehicleUpgrades;
    public override TechCategory CategoryForPDA => TechCategory.VehicleUpgrades;
    public override CraftTree.Type FabricatorType => CraftTree.Type.SeamothUpgrades;
    public override string[] StepsToFabricatorTab => new string[] { "SeamothModules" };
    public override QuickSlotType QuickSlotType => QuickSlotType.Passive;
    public override GameObject GetGameObject()
    {
        var prefab = CraftData.GetPrefabForTechType(TechType.SeamothElectricalDefense);
        var obj = GameObject.Instantiate(prefab);
        return obj;
    }
    protected override TechData GetBlueprintRecipe()
    {
        return new TechData()
        {
            craftAmount = 1,
            Ingredients =
            {
                new Ingredient(TechType.Polyaniline, 1),
                new Ingredient(TechType.CopperWire, 2),
                new Ingredient(TechType.AluminumOxide, 2),
                new Ingredient(TechType.Nickel, 1),
            },
        };
    }
}
```

### Nautilus
```csharp
// Create a custom prefab instance and set the class ID, friendly name, and description respectively
var seamothBrineResistanceModule = new CustomPrefab(
            "SeamothBrineResistModule",
            "Seamoth brine resistant coating",
            "Makes the Seamoth resistant to corrosive brine pools, by means of a protective coating.");
        
// Set our prefab to a clone of the Seamoth electrical defense module
seamothBrineResistanceModule.SetGameObject(new CloneTemplate(seamothBrineResistanceModule.Info, TechType.SeamothElectricalDefense));

// Make our item compatible with the seamoth module slot
seamothBrineResistanceModule.SetEquipment(EquipmentType.SeamothModule)
    .WithQuickSlotType(QuickSlotType.Passive);

// Make the Vehicle upgrade console a requirement for our item's blueprint
ScanningGadget scanning = seamothBrineResistanceModule.SetUnlock(TechType.BaseUpgradeConsole);

// Add our item to the Vehicle upgrades category
scanning.WithPdaGroupCategory(TechGroup.VehicleUpgrades, TechCategory.VehicleUpgrades);

var recipe = new RecipeData()
{
    craftAmount = 1,
    Ingredients =
    {
        new CraftData.Ingredient(TechType.Polyaniline, 1),
        new CraftData.Ingredient(TechType.CopperWire, 2),
        new CraftData.Ingredient(TechType.AluminumOxide, 2),
        new CraftData.Ingredient(TechType.Nickel, 1),
    },
};

// Add a recipe for our item, as well as add it to the Moonpool fabricator and Seamoth modules tab
seamothBrineResistanceModule.SetRecipe(recipe)
    .WithFabricatorType(CraftTree.Type.SeamothUpgrades)
    .WithStepsToFabricatorTab("SeamothModules");

// Register our item to the game
seamothBrineResistanceModule.Register();
```

This example is based off of a real mod. You can get access to the full source code [here](https://github.com/Metious/MetiousSubnauticaMods/tree/master/SeamothBrineResist).

# [CustomFabricator](#tab/custom-fabricator)

### SML 2.0
```csharp
public class AbyssFabricator : CustomFabricator
{
    private static Texture2D texture;
    public override Models Model { get; } = Models.Fabricator;
    public AbyssFabricator()
        : base("AbyssFabricator",
              "Abyss Fabricator",
              "Abyss Batteries Fabricator")
    {
        OnStartedPatching += () =>
        {
            texture = ImageUtils.LoadTextureFromFile(Path.Combine(Main.AssetsFolder, "AbyssFabricatorskin.png"));
        };
    }
    public override GameObject GetGameObject()
    {
        GameObject prefab = base.GetGameObject();
        if (texture != null)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = prefab.GetComponentInChildren<SkinnedMeshRenderer>();
            skinnedMeshRenderer.material.mainTexture = texture;
        }
        return prefab;
    }
    protected override TechData GetBlueprintRecipe()
    {
        return new TechData
        {
            craftAmount = 1,
            Ingredients = new List<Ingredient>
            {
                new Ingredient(TechType.Titanium, 2),
                new Ingredient(TechType.Quartz, 2),
                new Ingredient(TechType.JeweledDiskPiece, 1),
            }
        };
    }
    protected override Atlas.Sprite GetItemSprite()
    {
        return SpriteManager.Get(TechType.Fabricator);
    }
    public override TechCategory CategoryForPDA { get; } = TechCategory.InteriorModule;
    public override TechGroup GroupForPDA { get; } = TechGroup.InteriorModules;
}
```

### Nautilus
```csharp
// Create a custom prefab instance and set the class ID, friendly name, description and icon respectively
var abyssFabricator = new CustomPrefab(
    "AbyssFabricator",
    "Abyss Fabricator",
    "Abyss Batteries Fabricator",
    SpriteManager.Get(TechType.Fabricator));

// Create a custom crafting tree for this tech type. This method returns a FabricatorGadget, which we can use to customize our crafting tree. For example, to add a new tab or crafting node.
var abyssFabCraftTree = abyssFabricator.CreateFabricator(out CraftTree.Type abyssFabType);

// Load up the custom main (diffuse) texture from disk.
var mainTexture = ImageUtils.LoadTextureFromFile(Path.Combine(AssetsFolder, "AbyssFabricatorskin.png"));

// Create our fabricator game object. The fabricator game object will use the vanilla Fabricator model, then set the main texture to the texture we loaded earlier.
var abyssFabricatorModel = new FabricatorTemplate(abyssFabricator.Info, abyssFabType)
{
    FabricatorModel = FabricatorTemplate.Model.Fabricator,
    ModifyPrefab = obj => obj.GetComponentInChildren<SkinnedMeshRenderer>().material.mainTexture = mainTexture
};

// Sets this prefab's game object to the model we created earlier.
abyssFabricator.SetGameObject(abyssFabricatorModel);

// Sets the recipe for the fabricator.
abyssFabricator.SetRecipe(new RecipeData(new Ingredient(TechType.Titanium, 2), new Ingredient(TechType.Quartz, 2), new Ingredient(TechType.JeweledDiskPiece, 1)));

// Adds the fabricator item to the Interior Modules group. This also makes our object buildable.
abyssFabricator.SetPdaGroupCategory(TechGroup.InteriorModules, TechCategory.InteriorModule);

// Register our item to the game.
abyssFabricator.Register();
```

This example is based off of a real mod. You can get access to the full source code [here](https://github.com/Metious/MetiousSubnauticaMods/tree/master/AbyssBatteries).

---

## Audio and FMOD
In the last few versions of SML 2, we made a lot of changes to the audio system SML offered, this was because of FMOD.  
FMOD is the sound engine Subnautica uses. It is more advanced and flexible compared to the built-in Unity audio system.  

Since we discovered the best practices and better ways to deal with custom sounds, we have deleted a bunch of previously-obsolete methods from
`CustomSoundHandler` and `AudioUtils` classes, as well as the `SoundChannel` enumeration in Nautilus.  

Beginning with Nautilus, all custom sounds will require a bus instead of a SoundChannel to determine the effects (E.G: reverb, muffling, low-pass, etc..) and the volume slider.  
Additionally, the `PlaySound` signature was also modified and renamed to `TryPlaySound`.

<pre class="lang-diff">
<span class="lang-diff-rem">- Channel channel = AudioUtils.PlaySound(soundPath, SoundChannel.Music);</span>
<span class="lang-diff-add">+ if (AudioUtils.TryPlaySound(soundPath, AudioUtils.BusPaths.Music, out Channel channel))
+ {
+   // do something with channel
+ }
</span>

<span class="lang-diff-rem">- Channel channel = AudioUtils.PlaySound(soundPath, SoundChannel.Voice);</span>
<span class="lang-diff-add">+ if (AudioUtils.TryPlaySound(soundPath, AudioUtils.BusPaths.PDAVoice, out Channel channel))
+ {
+   // do something with channel
+ }
</span>

<span class="lang-diff-rem">- Channel channel = AudioUtils.PlaySound(soundPath, SoundChannel.Ambient);</span>
<span class="lang-diff-add">+ if (AudioUtils.TryPlaySound(soundPath, AudioUtils.BusPaths.UnderwaterAmbient, out Channel channel))
+ {
+   // do something with channel
+ }
</span>

<span class="lang-diff-rem">- Channel channel = AudioUtils.PlaySound(soundPath, SoundChannel.Master);</span>
<span class="lang-diff-add">+ if (AudioUtils.TryPlaySound(soundPath, "bus:/", out Channel channel))
+ {
+   // do something with channel
+ }
</span>
</pre>

> [!WARNING]
> Creating or playing a custom sound on the master bus is il-advised as it is dangerous and has the possibility of breaking the audio for a game session.  
> Try to set an appropriate bus for your sound instead of the master one.

---
