# AnKuchen

Control UI Prefab from Script Library  
You won't have to drag and drop on Inspector when you create your UI.

<a href="https://www.buymeacoffee.com/kyubuns" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/default-orange.png" alt="Buy Me A Coffee" height="41" width="174"></a>

## Sample

Sample GameObject  
<img width="640" alt="Screen Shot 2020-09-03 at 20 49 27" src="https://user-images.githubusercontent.com/961165/92111226-f6806900-ee26-11ea-9718-beb328af5038.png">

### Get Button

You can get a component from a GameObject simply by specifying its name.  
If there is only one "HogeButton" under the UICache, the same code will work regardless of where the HogeButton is located.

```csharp
public class Sample : MonoBehaviour
{
    [SerializeField] private UICache root = default;

    public void Start()
    {
        var hogeButton = root.Get<Button>("HogeButton");
    }
}
```

### Get Text under the HogeButton

Look at the image above." There are four GameObjects named "Text".  
If you want to get the Text under the HogeButton, you can do this.  
As before, it works whether the HogeButton is directly under the Root or not.

```csharp
var hogeButtonText = root.Get<Text>("HogeButton/Text");
hogeButtonText.text = "Hoge!";
```

### Get Text directly under the root

```csharp
var text = root.Get<Text>("Text"); // This is an error." There are four names for "Text".
var text = root.Get<Text>("./Text"); // Root/Text
```

### Create children map

When you want to do the same thing with all three buttons in the UI,  
you can treat each of them as a map.

```csharp
public void Start()
{
    var hogeButton = root.GetMapper("HogeButton");
    var fugaButton = root.GetMapper("FugaButton");
    var piyoButton = root.GetMapper("PiyoButton");

    SetButtonText(hogeButton, "Hoge");
    SetButtonText(fugaButton, "Fuga");
    SetButtonText(piyoButton, "Piyo");
}

private void SetButtonText(IMapper button, string labelText)
{
    button.Get<Text>("Text").text = labelText;
}
```

### Duplicate

Want more buttons? You can.

```csharp
var newButton = root.GetMapper("HogeButton").Duplicate();
newButton.Get<Text>("Text").text = "New Button!";
```

### Duplicate and Layout

On top of more buttons, you can also line them up nicely.

```csharp
using (var editor = Layouter.TopToBottom(root.GetMapper("HogeButton")))
{
    var newButton1 = editor.Create();
    newButton1.Get<Text>("Text").text = "NewButton1";
    
    var newButton2 = editor.Create();
    newButton2.Get<Text>("Text").text = "NewButton2";
    
    var newButton3 = editor.Create();
    newButton3.Get<Text>("Text").text = "NewButton3";
}
```

### Code Template

Did you notice the "Copy Template" button in UICacheComponent?  
This way you won't have to worry about typo.

```csharp
public void Start()
{
    var ui = new UIElements(root);
    ui.HogeButtonText.text = "Hoge"; // I love having a type!
}

// ↓ The code from here on down is in your clipboard!
public class UIElements : IMappedObject
{
    public IMapper Mapper { get; private set; }
    public GameObject Root { get; private set; }
    public Text Text { get; private set; }
    public Button HogeButton { get; private set; }
    public Text HogeButtonText { get; private set; }
    public Button FugaButton { get; private set; }
    public Text FugaButtonText { get; private set; }
    public Button PiyoButton { get; private set; }
    public Text PiyoButtonText { get; private set; }

    public UIElements(IMapper mapper)
    {
        Initialize(mapper);
    }

    public void Initialize(IMapper mapper)
    {
        Mapper = mapper;
        Root = mapper.Get();
        Text = mapper.Get<Text>("./Text");
        HogeButton = mapper.Get<Button>("HogeButton");
        HogeButtonText = mapper.Get<Text>("HogeButton/Text");
        FugaButton = mapper.Get<Button>("FugaButton");
        FugaButtonText = mapper.Get<Text>("FugaButton/Text");
        PiyoButton = mapper.Get<Button>("PiyoButton");
        PiyoButtonText = mapper.Get<Text>("PiyoButton/Text");
    }
}
```

### Duplicate with type

You can still use Duplicate and Layouter, even if you have the type. Don't worry.

```csharp
public void Start()
{
    var ui = new UIElements(root);
    using (var editor = Layouter.Edit(ui.HogeButton))
    {
        foreach (var a in new[] { "h1", "h2", "h3" })
        {
            var button = editor.Create();
            button.Text.text = a;
        }
    }
}

public class UIElements : IMappedObject
{
    public IMapper Mapper { get; private set; }
    public GameObject Root { get; private set; }
    public Text Text { get; private set; }
    public ButtonElements HogeButton { get; private set; }

    public UIElements(IMapper mapper)
    {
        Initialize(mapper);
    }

    public void Initialize(IMapper mapper)
    {
        Mapper = mapper;
        Root = mapper.Get();
        Text = mapper.Get<Text>("./Text");
        HogeButton = mapper.GetChild<ButtonElements>("HogeButton");
    }
}

public class ButtonElements : IMappedObject
{
    public IMapper Mapper { get; private set; }
    public GameObject Root { get; private set; }
    public Button Button { get; private set; }
    public Text Text { get; private set; }

    public void Initialize(IMapper mapper)
    {
        Mapper = mapper;
        Root = mapper.Get();
        Button = mapper.Get<Button>();
        Text = mapper.Get<Text>("Text");
    }
}
```

### Translation

Sometimes you want to put a message in all the Text all together.

```csharp
root.SetText(new Dictionary<string, string>
{
    { "./Text", "Title" },
    { "HogeButton/Text", "Hoge" },
    { "FugaButton/Text", "Fuga" },
    { "PiyoButton/Text", "Piyo" },
});
```

### Testing

Someone changed the Prefab after I made the type!  
Let's prevent such accidents.

```csharp
var test1Object = Resources.Load<GameObject>("Test1");
Assert.DoesNotThrow(() => UIElementTester.Test<UIElements>(test1Object));
```

### Scroll List

![output](https://user-images.githubusercontent.com/961165/102043233-3805b480-3e17-11eb-8f2a-c54b6121a64a.gif)

```csharp
public class Sample : MonoBehaviour
{
    [SerializeField] private UICache root = default;

    public void Start()
    {
        var ui = new UIElements(root);

        using (var editor = ui.List.Edit())
        {
            editor.Spacing = 10f;
            editor.Margin.TopBottom = 10f;

            for (var i = 0; i < 1000; ++i)
            {
                if (Random.Range(0, 2) == 0)
                {
                    var i1 = i;
                    editor.Contents.Add(new UIFactory<ListElements1, ListElements2>((ListElements1 x) =>
                    {
                        x.LineText.text = $"Test {i1}";
                    }));
                }
                else
                {
                    editor.Contents.Add(new UIFactory<ListElements1, ListElements2>((ListElements2 x) =>
                    {
                        x.Background.color = Random.ColorHSV();
                        x.Button.onClick.AddListener(() => Debug.Log("Click Button"));
                    }));
                }
            }
        }
    }
}

public class UIElements : IMappedObject
{
    public IMapper Mapper { get; private set; }
    public VerticalList<ListElements1, ListElements2> List { get; private set; }

    public UIElements(IMapper mapper)
    {
        Initialize(mapper);
    }

    public void Initialize(IMapper mapper)
    {
        Mapper = mapper;
        List = new VerticalList<ListElements1, ListElements2>(
            mapper.Get<ScrollRect>("List"),
            mapper.GetChild<ListElements1>("Element1"),
            mapper.GetChild<ListElements2>("Element2")
        );
    }
}

public class ListElements1 : IMappedObject
{
    public IMapper Mapper { get; private set; }
    public GameObject Root { get; private set; }
    public Text LineText { get; private set; }

    public void Initialize(IMapper mapper)
    {
        Mapper = mapper;
        Root = mapper.Get();
        LineText = mapper.Get<Text>("./Text");
    }
}

public class ListElements2 : IReusableMappedObject
{
    public IMapper Mapper { get; private set; }
    public GameObject Root { get; private set; }
    public Image Background { get; private set; }
    public Button Button { get; private set; }

    public void Initialize(IMapper mapper)
    {
        Mapper = mapper;
        Root = mapper.Get();
        Background = mapper.Get<Image>("./Image");
        Button = mapper.Get<Button>("./Button");
    }

    public void Activate()
    {
    }

    public void Deactivate()
    {
        Button.onClick.RemoveAllListeners();
    }
}
```

### With Baum2

- Baum2 is Photoshop(psd) to Unity(uGUI) Library.
- Please use [ankuchen branch in baum2](https://github.com/kyubuns/Baum2/tree/ankuchen).

## Setup

- Install "AnKuchen" by UnityPackageManager. `https://github.com/kyubuns/AnKuchen.git?path=Unity/Assets/AnKuchen`
- Add a UICacheComponent to the Root of the UI and press the Update button.
  - We recommend that you automate the process of pressing the Update button to fit your workflow.
<img width="640" alt="Screen Shot 2020-09-03 at 20 49 27" src="https://user-images.githubusercontent.com/961165/92111226-f6806900-ee26-11ea-9718-beb328af5038.png">
- To update, rewrite the Hash in Packages/packages-lock.json.

## Requirements

- Requires Unity2018.4 or later

## License

MIT License (see [LICENSE](LICENSE))
