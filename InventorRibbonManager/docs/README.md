
# About 
Inventor Ribbon Manager is a helper library to create **Ribbon Tabs**, **Panels**, and **Buttons** for an Inventor Addin.

Set the Property 'AdskVersion' to the Inventor Version you are using, e.g. "2025" for Inventor 2025.
Since Inventor 2025 the library use .NET8, for older versions .NET framework 4.8 is used.

---

## 🔧 Prerequisites

- Reference to `InventorRibbonManager.dll` (or included as a project)
- Your AddIn implements `ApplicationAddInServer`

---

## 🧱 Basic Structure

### 1. `InventorRibbonBuilder`

Main entry class to create tabs, panels, and controls.

```csharp
var ribbonBuilder = new InventorRibbonBuilder(inventorApp, myAddInGuid);
```

---

### 2. Create your own commands

Create your own classes that implement `IPanelButton`.  
Example:

```csharp
public class MyButton : IPanelButton
{
    public string DisplayName => "Hello";
    public string InternalName => "Hello.Internal";
    public string PanelName => "Example";
    public string PanelInternalName => "Example.Internal";
    public RibbonType Environment => RibbonType.Part;

    public ButtonDefinition InventorButtonDefinition { get; }

    public MyButton(Inventor.Application app)
    {
        InventorButtonDefinition = app.CommandManager.ControlDefinitions.AddButtonDefinition(
            DisplayName, InternalName, CommandTypesEnum.kShapeEditCmdType,
            StandardAddInServer._guid, "Tooltip", DisplayName, null, null);

        InventorButtonDefinition.OnExecute += (ctx) =>
            System.Windows.Forms.MessageBox.Show("Button clicked!");
    }

    public void AddToPanel(IRibbonPanel panel, RibbonType env)
    {
        panel.AddButton(this, true, env);
    }

    public void Execute() { }
    public object Icon => null;
    public bool Pressed { get; set; }
}
```

---

### 3. Register your buttons

Create button instances:

```csharp
var buttons = new List<IPanelButton>
{
    new MyButton(inventorApp),
    new MyPopup(inventorApp),
    new MyComboBox(inventorApp)
};
```

Then **automatically generate all panels and controls**:

```csharp
InventorRibbonBuilder.CreatePanelsAndButtonsForTab(
    ribbonBuilder,
    "My Tab",               // Display name
    "MyTab.InternalName",   // Internal name
    buttons
);
```

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

💡 All variants also exist as `ToSlideout(...)`.

---

## 🛠 Using Slideouts

Inside your `AddToPanel(...)` method, you can use slideout placement:

```csharp
panel.AddButtonToSlideout(this, env);
panel.AddPopupToSlideout(...);
panel.AddComboBoxToSlideout(...);
```

---

## 🔌 AddIn Integration

In your `StandardAddInServer` class:

```csharp
public void Activate(ApplicationAddInSite addInSiteObject, bool firstTime)
{
    _guid = ((GuidAttribute)Attribute.GetCustomAttribute(GetType(), typeof(GuidAttribute))).Value;
    _inventorApp = addInSiteObject.Application;

    var ribbonBuilder = new InventorRibbonBuilder(_inventorApp, _guid);

    var buttons = new List<IPanelButton>
    {
        new SampleButton(_inventorApp),
        new ComboBoxCommand(_inventorApp),
        // more...
    };

    InventorRibbonBuilder.CreatePanelsAndButtonsForTab(
        ribbonBuilder,
        "DemoTab",
        "DemoTab.Internal",
        buttons
    );
}
```

---


## ✅ Summary

With `InventorRibbonManager` you can:

- Define Inventor Ribbons cleanly and in structured C#/WPF-style
- Easily create buttons, combo boxes, split buttons, galleries, and popups
- Work fully typed and reusable across your AddIn architecture

