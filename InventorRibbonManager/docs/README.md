
# About 
Inventor Ribbon Manager is a helper library for creating ribbon tabs, panels,
and controls for Autodesk Inventor add-ins.

The library provides wrapper abstractions for common Inventor ribbon elements,
including tabs, panels, buttons, popups, split buttons, toggle popups, combo
boxes, galleries, and slideout controls.


## Target Frameworks and Inventor Version

The package currently provides builds for:

- `net48`
- `net8.0-windows`
- `net10.0-windows`

For Inventor 2025.5 and later, the recommended target framework is
`net10.0-windows`.

For Inventor 2025 through 2025.4.2, the recommended target framework is
`net8.0-windows`.

For older Inventor add-ins, `net48` is supported.

The Inventor version is configured in the consuming add-in project through the
MSBuild property `AdskVersion`.

For example:

```xml
<PropertyGroup>
  <TargetFramework>net10.0-windows</TargetFramework>
  <AdskVersion>2025</AdskVersion>
</PropertyGroup>
```

The package uses this value to locate the Inventor interop assembly:

```text
C:\Program Files\Autodesk\Inventor 2025\Bin\Autodesk.Inventor.Interop.dll
```

If Inventor is installed in a different location, override the installation
directory:

```xml
<PropertyGroup>
  <InventorInstallDir>D:\Autodesk\Inventor 2025</InventorInstallDir>
</PropertyGroup>
```

`AdskVersion` does not rebuild the NuGet assembly and does not select a
different compiled Ribbon Manager implementation. It is used by the package's
MSBuild targets to resolve the Inventor interop reference during the build.


---

## 🔧 Prerequisites

- A reference to the `wh7r4bb17.InventorRibbonManager` NuGet package
- Autodesk Inventor installed on the development machine.
- Your add-in implements `ApplicationAddInServer`
- `AdskVersion` set in the consuming add-in project.

---

## 🎨 Button Icons

The package provides the public `InventorIconConverter` class for converting
.NET `Icon` and `Bitmap` instances or image files into icon objects that can be
used by Autodesk Inventor ribbon buttons.

The internal OLE and COM conversion details are hidden inside the library.

### Convert a single icon

```csharp
public object? StandardIcon =>
    InventorIconConverter.FromFile(@"C:\Icons\hello16.png");

public object? LargeIcon =>
    InventorIconConverter.FromFile(@"C:\Icons\hello32.png");
```

The converter returns `null` if the path is `null`, empty, whitespace, invalid,
or if the image cannot be loaded.

### Convert two image files

Use `FromFiles(...)` when both the standard and large icons are loaded from files:

```csharp
private static readonly InventorButtonIcons Icons =
    InventorIconConverter.FromFiles(
        @"C:\Icons\hello16.png",
        @"C:\Icons\hello32.png");

public object? StandardIcon => Icons.StandardIcon;
public object? LargeIcon => Icons.LargeIcon;
```

The returned `InventorButtonIcons` value contains both converted icon objects.

### Convert bitmap instances

```csharp
using System.Drawing;

using var standardBitmap = new Bitmap(@"C:\Icons\hello16.png");
using var largeBitmap = new Bitmap(@"C:\Icons\hello32.png");

InventorButtonIcons icons =
    InventorIconConverter.FromBitmaps(
        standardBitmap,
        largeBitmap);

public object? StandardIcon => icons.StandardIcon;
public object? LargeIcon => icons.LargeIcon;
```

### Convert `Icon` instances

```csharp
using System.Drawing;

using var standardIcon = new Icon(@"C:\Icons\hello16.ico");
using var largeIcon = new Icon(@"C:\Icons\hello32.ico");

InventorButtonIcons icons =
    InventorIconConverter.FromIcons(
        standardIcon,
        largeIcon);

public object? StandardIcon => icons.StandardIcon;
public object? LargeIcon => icons.LargeIcon;
```

### Available conversion methods

| Method | Description |
|---|---|
| `FromFile` | Converts one image file into an Inventor-compatible icon object. |
| `FromFiles` | Converts standard and large image files and returns both icons. |
| `FromBitmap` | Converts one `Bitmap` into an Inventor-compatible icon object. |
| `FromBitmaps` | Converts standard and large `Bitmap` instances and returns both icons. |
| `FromIcon` | Converts one .NET `Icon` into an Inventor-compatible icon object. |
| `FromIcons` | Converts standard and large `Icon` instances and returns both icons. |




---

## 🧱 Basic Structure

### 1. `InventorRibbonBuilder`

Main entry class to create tabs, panels, and controls.

```csharp
var ribbonBuilder = new InventorRibbonBuilder(inventorApp, myAddInGuid);
```

---

### 2. Create your own commands

Create a class that implements `IPanelButton`.  
  
Example:
```csharp
public sealed class MyButton : IPanelButton
{
    public string DisplayName => "Hello";
    public string InternalName => "Hello.Internal";
    public string PanelName => "Example";
    public string PanelInternalName => "Example.Internal";
    public RibbonType Environment => RibbonType.Part;

    public ButtonDefinition InventorButtonDefinition { get; }

    private static readonly InventorButtonIcons Icons =
        InventorIconConverter.FromFiles(
            @"C:\Icons\hello16.png",
            @"C:\Icons\hello32.png");

    public object? StandardIcon => Icons.StandardIcon;
    public object? LargeIcon => Icons.LargeIcon;
    public bool Pressed { get; set; }

    public MyButton(Inventor.Application app, string clientId)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        if (string.IsNullOrWhiteSpace(clientId))
            {throw new ArgumentException(
                "The client identifier must not be null, empty, or white-space.",
                nameof(clientId));}

        InventorButtonDefinition =
            app.CommandManager.ControlDefinitions.AddButtonDefinition(
                DisplayName,
                InternalName,
                CommandTypesEnum.kShapeEditCmdType,
                clientId,
                "Description Text",
                "Tooltip Text",
                StandardIcon,
                LargeIcon,
                ButtonDisplayEnum.kDisplayTextInLearningMode);

        InventorButtonDefinition.OnExecute += OnExecute;
    }

    public void AddToPanel(IRibbonPanel panel, RibbonType environment)
    {
        if (panel == null)
            {throw new ArgumentNullException(nameof(panel));}

        panel.AddButton(this, true, environment);
    }

    public void Execute()
    {
        System.Windows.Forms.MessageBox.Show("Button clicked!");
    }

    private void OnExecute(NameValueMap context)
    {
        Execute();
    }
}
```

---

### 3. Register your buttons

Create button instances:

```csharp
var buttons = new List<IPanelButton>
{
    new MyButton(inventorApp, myAddInGuid),
    new MyPopup(inventorApp, myAddInGuid),
    new MyComboBox(inventorApp, myAddInGuid)
};
```

Then create the required tabs, panels, and controls:

```csharp
InventorRibbonBuilder.CreatePanelsAndButtonsForTab(
    ribbonBuilder,
    "My Tab",
    "MyTab.InternalName",
    buttons);
```

Buttons are grouped by panel and ribbon environment. Existing tabs and panels are reused when their internal names match.

---


## 🔄 Supported Controls (`IRibbonPanel` Methods)

| Type               | Method                          | Example Class                   |
|--------------------|----------------------------------|----------------------------------|
| Standard Button    | `AddButton`                      | `SampleButton`                  |
| ButtonPopup        | `AddButtonPopup`                 | `ButtonPopupCommand`            |
| Popup              | `AddPopup`                       | `PopupCommand`                  |
| SplitButton        | `AddSplitButton`                 | `SplitButtonCommand`            |
| SplitButton MRU    | `AddSplitButtonMRU`              | `SplitButtonMRUCommand`         |
| TogglePopup        | `AddTogglePopup`                 | `TogglePopupCommand`            |
| ComboBox           | `AddComboBox`                    | `ComboBoxCommand`               |
| Gallery            | `AddGallery`                     | `GalleryCommand`                |

💡 The corresponding slideout methods use the `ToSlideout` suffix, for example:

```csharp
panel.AddButtonToSlideout(button, environment);
panel.AddPopupToSlideout(displayedButton, additionalButtons, true, environment);
panel.AddComboBoxToSlideout(comboBox, environment);
panel.AddGalleryToSlideout(buttons, true, 200, environment);
```

---

## 🛠 Using Slideouts

Inside `AddToPanel(...)`, controls can be added to the panel's slideout area:

```csharp
panel.AddButtonToSlideout(this, env);
panel.AddPopupToSlideout(...);
panel.AddComboBoxToSlideout(...);
```

---

## 🔌 Add-in Integration

In your `StandardAddInServer` class:

```csharp
public void Activate(ApplicationAddInSite addInSiteObject, bool firstTime)
{
    _guid = ((GuidAttribute)Attribute.GetCustomAttribute(GetType(), typeof(GuidAttribute))).Value;
    _inventorApp = addInSiteObject.Application;

       using (var ribbonBuilder = new InventorRibbonBuilder(_inventorApp, _guid))
    {
        var buttons = new List<IPanelButton>
        {
            new SampleButton(_inventorApp, _guid),
            new ComboBoxCommand(_inventorApp)
        };

        InventorRibbonBuilder.CreatePanelsAndButtonsForTab(
            ribbonBuilder,
            "DemoTab",
            "DemoTab.Internal",
            buttons);
    }
}
```

---



## ✅ Summary

With `InventorRibbonManager` you can:

- Create Inventor ribbon tabs, panels, and controls through typed abstractions.
- Reuse existing tabs and panels by their internal names.
- Add standard buttons, popups, split buttons, MRU split buttons, toggle popups,
  combo boxes, galleries, and slideout controls.
- Group controls by panel and ribbon environment.
- Keep Autodesk Inventor API access behind wrapper classes where appropriate.
- Use the same ribbon-building pattern across supported target frameworks.
