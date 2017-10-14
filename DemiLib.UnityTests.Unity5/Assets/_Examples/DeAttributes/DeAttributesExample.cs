using System.Collections.Generic;
using DG.DeInspektor.Attributes;
using DG.DemiLib;
using UnityEngine;

[DeScriptExecutionOrder(-456)] // Sets the script execution order of this MonoBehaviour to -456
[DeComponentDescription("This is a components description made with <b>DeComponentDescription</b>")]
public class DeAttributesExample : MonoBehaviour
{
    const string _MainHeaderTextColor = "ffe682";
    const string _MainHeaderBgColor = "1e78ea";
    const FontStyle _MainHeaderFontStyle = FontStyle.Bold;
    const int _MainHeaderFontSize = 15;

    [DeHeader("██ DeInspektor", "ffffff", "144f9c", FontStyle.Normal, _MainHeaderFontSize + 10, mode = DeHeaderAttribute.Mode.TopDivider)]
    [DeComment("These features are automatically implemented by DeInspektor for some types (you can disable all extra features via Unity's Preferences).", "ffffff", "144f9c", fontSize = 12)]

    // DeList examples
    [DeHeader("█ Array/List", _MainHeaderTextColor, _MainHeaderBgColor, _MainHeaderFontStyle, _MainHeaderFontSize, mode = DeHeaderAttribute.Mode.TopDivider)]
    [DeComment("Shows the array/list with drag/add/delete options.", _MainHeaderTextColor, _MainHeaderBgColor)]
    //
    public int[] aList0 = new []{ 1, 2, 3 };

    [DeHeader("██ Attributes", "ffffff", "144f9c", FontStyle.Normal, _MainHeaderFontSize + 10, mode = DeHeaderAttribute.Mode.TopDivider)]
    [DeComment("Attributes that you can add to your class/fields/methods.", "ffffff", "144f9c", fontSize = 12)]

    // DeHeader examples
    [DeHeader("█ DeHeader", _MainHeaderTextColor, _MainHeaderBgColor, _MainHeaderFontStyle, _MainHeaderFontSize, mode = DeHeaderAttribute.Mode.TopDivider)]
    [DeComment("Header decorator with various options for colors etc.", _MainHeaderTextColor, _MainHeaderBgColor)]
    //
    [DeHeader("A default header")]
    public float aRandomVariable = -1;
    [DeHeader("A colored header", "ffd860", "a154df")]
    [DeHeader("A colored header with custom font style, size, and dividers", "ffffff", "ff5e31", FontStyle.Normal, 9, mode = DeHeaderAttribute.Mode.Dividers)]
    [DeHeader("A default header with a bottom divider", mode = DeHeaderAttribute.Mode.BottomDivider)]
    [DeHeader("More custom font style and size", "380b0b", "ffc331", FontStyle.Italic, 16)]
    [DeHeader("A right aligned colored header", TextAnchor.MiddleRight, "ffffff", "ff5458")]
    [DeHeader("Another colored header then an array", "ffffff", "000000")]
    public int[] anArray = new []{ 1, 2, 3 };

    // DeLabel examples
    [DeHeader("█ DeLabel", _MainHeaderTextColor, _MainHeaderBgColor, _MainHeaderFontStyle, _MainHeaderFontSize, mode = DeHeaderAttribute.Mode.TopDivider)]
    [DeComment("Writes custom text instead of the regular property label.", _MainHeaderTextColor, _MainHeaderBgColor)]
    //
    [DeLabel("1234")]
    public float aLabel0 = 27;
    [DeLabel("_%&()")][Tooltip("Can be combined with tooltips")]
    public float aLabel1 = 49;

    // DeColoredLabel examples
    [DeHeader("█ DeColoredLabel", _MainHeaderTextColor, _MainHeaderBgColor, _MainHeaderFontStyle, _MainHeaderFontSize, mode = DeHeaderAttribute.Mode.TopDivider)]
    [DeComment("Colors the label of a property in the Inspector.", _MainHeaderTextColor, _MainHeaderBgColor)]
    //
    [DeColoredLabel("ffd860", "a154df")]
    public string aString0 = "Hellow";
    [DeColoredLabel("ffffff", "ff5e31", "Custom Label")]
    public string aString1 = "Hellow again";
    [DeColoredLabel("222222", "ffc331")]
    public string aString2 = "Hellow enough";

    // DeImage examples
    [DeHeader("█ DeImage", _MainHeaderTextColor, _MainHeaderBgColor, _MainHeaderFontStyle, _MainHeaderFontSize, mode = DeHeaderAttribute.Mode.TopDivider)]
    [DeComment("Draws the given image with options for max width/height.", _MainHeaderTextColor, _MainHeaderBgColor)]
    //
    [DeImage("_Examples/_Images/demilib.png", 150)]
    public string imgString0 = "A kinda logo!";
    [DeImage("_Examples/_Images/ncus.png", -1, 200)]
    [DeImage("_Examples/_Images/goscurry.png", -1, 200)]
    public string imgString1 = "Images above me!";

    // DeImagePreview examples
    [DeHeader("█ DeImagePreview", _MainHeaderTextColor, _MainHeaderBgColor, _MainHeaderFontStyle, _MainHeaderFontSize, mode = DeHeaderAttribute.Mode.TopDivider)]
    [DeComment("Shows a texture/sprite with a bigger preview.", _MainHeaderTextColor, _MainHeaderBgColor)]
    [DeImagePreview(emptyAlert = true)]
    public Sprite sprite0;
    [DeImagePreview(emptyAlert = true)]
    public Texture texture0;
    [DeImagePreview(emptyAlert = true)]
    public Texture2D texture2D0;

    // DeConditional + DeBegin/EndDisabled examples
    [DeHeader("█ DeConditional + DeBegin/EndDisabled", _MainHeaderTextColor, _MainHeaderBgColor, _MainHeaderFontStyle, _MainHeaderFontSize, mode = DeHeaderAttribute.Mode.TopDivider)]
    [DeComment("You can choose to disable or completely hide a property based on given conditions. Also, you can disable a group of fields with <b>DeBegin/EndDisabled</b>.", _MainHeaderTextColor, _MainHeaderBgColor)]
    //
    [DeToggleButton("Group Toggle", DePosition.HExtended)]
    public bool groupBool = true;
    [DeBeginDisabled("groupBool")]
    [DeBeginGroup][DeColoredLabel("ffffff", "222222")]
    public float conditioningFloat = 10;
    [DeConditional("conditioningFloat", 10f, Condition.Is, ConditionalBehaviour.Hide)]
    public string conditionalS00 = "Visible if conditioningFloat == 10";
    [DeConditional("conditioningFloat", 10f, Condition.LessThan)]
    public string conditionalS01 = "Enabled if conditioningFloat < 10";
    [DeEndGroup][DeConditional("conditioningFloat", 10f, Condition.GreaterOrEqual)]
    public string conditionalS02 = "Enabled if conditioningFloat >= 10";
    [DeBeginGroup][DeColoredLabel("ffffff", "222222")]
    public int conditioningInt = 69;
    [DeEndGroup][DeConditional("conditioningInt", 45)]
    public string conditionalS10 = "Enabled if conditioningInt == 45";
    [DeBeginGroup][DeColoredLabel("ffffff", "222222")]
    public string conditioningString = "Gino";
    [DeConditional("conditioningString", "Gino")]
    public string conditionalS20 = "Enabled if conditioningString == \"Gino\"";
    [DeConditional("conditioningString", Condition.IsNotNullOrEmpty)]
    public string conditionalS21 = "Enabled if conditioningString is not empty";
    [DeEndGroup][DeConditional("conditioningString", "Pino", Condition.IsNot)]
    public string conditionalS22 = "Enabled if conditioningString != \"Pino\"";
    [DeBeginGroup][DeColoredLabel("ffffff", "222222")]
    public GameObject conditioningObj0;
    [DeEndGroup][DeConditional("conditioningObj0")]
    public string conditionalS30 = "Enabled if conditioningObj is not NULL";
    [DeBeginGroup][DeColoredLabel("ffffff", "222222")]
    public bool conditioningBool = true;
    [DeDisabled("conditioningBool")]
    public string conditionalS40 = "Enabled if conditioningBool is TRUE";
    [DeEndGroup][DeEndDisabled][DeConditional("conditioningBool", false)]
    public string conditionalS41 = "Enabled if conditioningBool is FALSE";

    // DeEmptyAlert examples
    [DeHeader("█ DeEmptyAlert", _MainHeaderTextColor, _MainHeaderBgColor, _MainHeaderFontStyle, _MainHeaderFontSize, mode = DeHeaderAttribute.Mode.TopDivider)]
    [DeComment("Only for object references and string: shows the field in red if empty or null.", _MainHeaderTextColor, _MainHeaderBgColor)]
    //
    [DeEmptyAlert] public GameObject sampleGO;
    [DeEmptyAlert] public string sampleString;

    // DeComment examples
    [DeHeader("█ DeComment", _MainHeaderTextColor, _MainHeaderBgColor, _MainHeaderFontStyle, _MainHeaderFontSize, mode = DeHeaderAttribute.Mode.TopDivider)]
    [DeComment("Shows a box with the desired text, with options for colors etc.", _MainHeaderTextColor, _MainHeaderBgColor)]
    //
    [DeComment("This is how a default DeComment looks like when optional parameters are not used. <b>You can also use <color=#ffd860>rich-text</color></b>.")]
    [DeComment("This one is colored, gosh!\nIt almost looks like a colored DeHeader, if not for the wordWrap.", "ffd860", "a154df")]
    [DeComment("This has some bigger text just because it's cool. <i>Or maybe not</i>.\n<size=18>Is it cool?</size> Is it? Uh? Uh? Ehhhh.", "ffffff", "ff4f5e", fontSize = 11)]

    // DeDivider examples
    [DeHeader("█ DeDivider", _MainHeaderTextColor, _MainHeaderBgColor, _MainHeaderFontStyle, _MainHeaderFontSize, mode = DeHeaderAttribute.Mode.TopDivider)]
    [DeComment("Dividers, with options for color, size and margins.", _MainHeaderTextColor, _MainHeaderBgColor)]
    //
    [DeDivider]
    [DeDivider(4, "ff5e31")]
    [DeDivider(1, "a154df")]

    // DeToggle examples
    [DeHeader("█ DeToggle", _MainHeaderTextColor, _MainHeaderBgColor, _MainHeaderFontStyle, _MainHeaderFontSize, mode = DeHeaderAttribute.Mode.TopDivider)]
    [DeComment("Shows a toggle button instead of the usual checkbox.", _MainHeaderTextColor, _MainHeaderBgColor)]
    //
    [DeToggleButton()]
    public bool aToggle0 = true;
    [DeToggleButton("This one has different colors", false, "000000", "fb262d", "", "ffcf8d")]
    public bool aToggle1 = true;
    [DeToggleButton("0", DePosition.HThirdLeft)]
    public bool bToggle0;
    [DeToggleButton("1", DePosition.HThirdMiddle)]
    public bool bToggle1 = true;
    [DeToggleButton("2", DePosition.HThirdRight)]
    public bool bToggle2;
    [DeToggleButton("A", DePosition.HHalfLeft)]
    public bool cToggle0;
    [DeToggleButton("B", DePosition.HHalfRight)]
    public bool cToggle1;
    [DeToggleButton("This one has a label", true, customLabel = "A Custom Label")]
    public bool anotherToggle0;
    [DeToggleButton("I'm ON", DePosition.HDefault, offText = "I'm OFF")]
    public bool anotherToggle1 = true;

    // DeBeginGroup examples
    [DeHeader("█ DeBegin/EndGroup", _MainHeaderTextColor, _MainHeaderBgColor, _MainHeaderFontStyle, _MainHeaderFontSize, mode = DeHeaderAttribute.Mode.TopDivider)]
    [DeComment("Wraps all the fields between <b>DeBeginGroup</b> and <b>DeEndGroup</b> inside a box-styled gui skin.", _MainHeaderTextColor, _MainHeaderBgColor)]
    //
    public bool ungroupedBool;
    [DeBeginGroup][DeHeader("Main Group", marginTop = 0)][Range(0, 50)]
    public float groupedSlider = 13;
    public bool groupedBool1;
    [DeBeginGroup][DeHeader("Sub Group", marginTop = 0)]
    public string subgroupedS0 = "This is inside a sub-group";
    [DeEndGroup]
    public string subgroupedS1 = "This too";
    [DeEndGroup]
    public string groupedS0 = "This is inside the main group";

    // Extra structs examples
    [DeHeader("█ Extra Structs Attributes", _MainHeaderTextColor, _MainHeaderBgColor, _MainHeaderFontStyle, _MainHeaderFontSize, mode = DeHeaderAttribute.Mode.TopDivider)]
    [DeComment("Attributes for DemiLib's core extra structs.", _MainHeaderTextColor, _MainHeaderBgColor)]
    //
    [DeRange(0, 100)]
    public Range aFloatRange = new Range(3, 10);
    [DeRange(0, 100)]
    public IntRange anIntRange = new IntRange(3, 10);

    // DeButton examples
    [DeHeader("█ DeButton + DeMethodButton", _MainHeaderTextColor, _MainHeaderBgColor, _MainHeaderFontStyle, _MainHeaderFontSize, mode = DeHeaderAttribute.Mode.TopDivider)]
    [DeComment("You can place as many DeButton as you want (as long as they're above a field), and use them to call any method of any class. Use <b>DeMethodButton</b> instead to draw a button and directly call the method over which it resides.", _MainHeaderTextColor, _MainHeaderBgColor)]
    //
    [DeButton(typeof(DeAttributesExample), "SamplePublic")]
    [DeButton("ffcf40", "ff0000", typeof(DeAttributesExample), "SamplePrivate")]
    [DeButton("A Method with params and custom label", null, "ffcf40", typeof(DeAttributesExample), "SamplePrivateWithParams", 45, "hellow")]
    [DeButton(DePosition.HThirdLeft, typeof(DeAttributesExample), "SamplePublic")]
    [DeButton(DePosition.HThirdMiddle, null, "00ff00", typeof(DeAttributesExample), "SamplePublic")]
    [DeButton(DePosition.HThirdRight, "d39fff", "ac6be3", typeof(DeAttributesExample), "SamplePublic")]
    [DeButton(DePosition.HHalfLeft, typeof(DeAttributesExample), "SamplePublic")]
    [DeButton(DePosition.HHalfRight, typeof(DeAttributesExample), "SamplePublic")]
    [DeButton(DePosition.HDefault, typeof(DeAttributesExample), "SamplePublic")]
    [TextArea]
    public string note = "Below me there's DeMethodButtons instead, which are attributes placed directly over a method instead of a field (like the ones above me).";

    public void SamplePublic()
    {
        Debug.Log("SamplePublic method was called, wohooo!");
    }
    void SamplePrivate()
    {
        Debug.Log("SamplePrivate method was called, wohooo!");
    }
    static void SampleStatic()
    {
        Debug.Log("SampleStatic method was called, ta-dah!");
    }
    void SamplePrivateWithParams(float f, string s)
    {
        Debug.Log(string.Format("SamplePrivateWithParams method was called with params: {0}, \"{1}\"", f, s));
    }

    [DeMethodButton("Custom text no params", layout = DeLayout.BeginHorizontal)]
    [DeMethodButton("Custom text and params", 0, "CUSTOM!", layout = DeLayout.EndHorizontal)]
    void SampleMethod00(string s = null)
    {
        Debug.Log(string.Format("SampleMethod0 called, dum dee dum! Custom param: \"{0}\"", s));
    }

    [DeMethodButton]
    void SampleMethod_01(string s = null)
    {
        Debug.Log(string.Format("SampleMethod1 called, la dee da! Custom param: \"{0}\"", s));
    }
}