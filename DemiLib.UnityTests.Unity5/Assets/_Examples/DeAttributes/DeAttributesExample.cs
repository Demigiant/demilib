using DG.DemiLib.Attributes;
using UnityEngine;

[DeScriptExecutionOrder(-456)] // Sets the script execution order of this MonoBehaviour to -456
public class DeAttributesExample : MonoBehaviour
{
    const string _MainHeaderTextColor = "ffeb42";
    const string _MainHeaderBgColor = "198519";
    const FontStyle _MainHeaderFontStyle = FontStyle.Bold;
    const int _MainHeaderFontSize = 18;

    // DeHeader examples
    [DeDivider(2, _MainHeaderBgColor, 3, -3)]
    [DeHeader("█ DeHeader", _MainHeaderTextColor, _MainHeaderBgColor, _MainHeaderFontStyle, _MainHeaderFontSize)]
    [DeComment("Header decorator with various options for colors etc.", _MainHeaderTextColor, _MainHeaderBgColor)]
    //
    [DeHeader("A default header")]
    public float aRandomVariable = -1;
    [DeHeader("A colored header", "ffd860", "a154df")]
    [DeHeader("A colored header with custom font style and size", "ffffff", "ff5e31", FontStyle.Normal, 9)]
    [DeHeader("More custom font style and size", "380b0b", "ffc331", FontStyle.Italic, 16)]
    [DeHeader("A right aligned colored header", TextAnchor.MiddleRight, "ffffff", "327dcc")]
    [DeHeader("Another colored header then an array", "ffffff", "000000")]
    public int[] anArray = new []{ 1, 2, 3 };

    // DeColoredLabel examples
    [DeDivider(2, _MainHeaderBgColor, 3, -3)]
    [DeHeader("█ DeColoredLabel", _MainHeaderTextColor, _MainHeaderBgColor, _MainHeaderFontStyle, _MainHeaderFontSize)]
    [DeComment("Colors the label of a property in the Inspector", _MainHeaderTextColor, _MainHeaderBgColor)]
    //
    [DeColoredLabel("ffd860", "a154df")]
    public string aString0 = "Hellow";
    [DeColoredLabel("ffffff", "ff5e31")]
    public string aString1 = "Hellow again";
    [DeColoredLabel("222222", "ffc331")]
    public string aString2 = "Hellow enough";

    // DeConditional examples
    [DeDivider(2, _MainHeaderBgColor, 3, -3)]
    [DeHeader("█ DeConditional", _MainHeaderTextColor, _MainHeaderBgColor, _MainHeaderFontStyle, _MainHeaderFontSize)]
    [DeComment("You can choose to disable or completely hide a property based on given conditions.", _MainHeaderTextColor, _MainHeaderBgColor)]
    //
    [DeColoredLabel("ffffff", "222222")]
    public float conditioningFloat = 10;
    [DeConditional("conditioningFloat", 10f, Condition.Is, ConditionalBehaviour.Hide)]
    public string conditionalS00 = "Visible if conditioningFloat == 10";
    [DeConditional("conditioningFloat", 10f, Condition.LessThan)]
    public string conditionalS01 = "Enabled if conditioningFloat < 10";
    [DeConditional("conditioningFloat", 10f, Condition.GreaterOrEqual)]
    public string conditionalS02 = "Enabled if conditioningFloat >= 10";
    [DeColoredLabel("ffffff", "222222")]
    public int conditioningInt = 69;
    [DeConditional("conditioningInt", 45)]
    public string conditionalS10 = "Enabled if conditioningInt == 45";
    [DeColoredLabel("ffffff", "222222")]
    public string conditioningString = "Gino";
    [DeConditional("conditioningString", "Gino")]
    public string conditionalS20 = "Enabled if conditioningString == \"Gino\"";
    [DeConditional("conditioningString", "", Condition.IsNot)]
    public string conditionalS21 = "Enabled if conditioningString != \"\"";
    [DeColoredLabel("ffffff", "222222")]
    public bool conditioningBool = true;
    [DeConditional("conditioningBool", true)]
    public string conditionalS30 = "Enabled if conditioningBool is TRUE";
    [DeConditional("conditioningBool", false)]
    public string conditionalS31 = "Enabled if conditioningBool is FALSE";

    // DeComment examples
    [DeDivider(2, _MainHeaderBgColor, 3, -3)]
    [DeHeader("█ DeComment", _MainHeaderTextColor, _MainHeaderBgColor, _MainHeaderFontStyle, _MainHeaderFontSize)]
    [DeComment("Shows a box with the desired text, with options for colors etc.", _MainHeaderTextColor, _MainHeaderBgColor)]
    //
    [DeComment("This is how a default DeComment looks like when optional parameters are not used.")]
    [DeComment("This one is colored, gosh!\nIt almost looks like a colored DeHeader, if not for the wordWrap.", "ffd860", "a154df")]

    // DeDivider examples
    [DeDivider(2, _MainHeaderBgColor, 3, -3)]
    [DeHeader("█ DeDivider", _MainHeaderTextColor, _MainHeaderBgColor, _MainHeaderFontStyle, _MainHeaderFontSize)]
    [DeComment("Dividers, with options for color, size and margins", _MainHeaderTextColor, _MainHeaderBgColor)]
    //
    [DeDivider]
    [DeDivider(4, "ff5e31")]
    [DeDivider(1, "a154df")]

    // DeButton examples
    [DeDivider(2, _MainHeaderBgColor, 3, -3)]
    [DeHeader("█ DeButton", _MainHeaderTextColor, _MainHeaderBgColor, _MainHeaderFontStyle, _MainHeaderFontSize)]
    [DeComment("You can place as many DeButton as you want (as long as they're above a field), and use them to call any method of any class.", _MainHeaderTextColor, _MainHeaderBgColor)]
    //
    [DeButton(typeof(DeAttributesExample), "SamplePublic")]
    [DeButton("ffcf40", "ff0000", typeof(DeAttributesExample), "SamplePrivate")]
    [DeButton("Method with params and custom label", null, "ffcf40", typeof(DeAttributesExample), "SamplePrivateWithParams", 45, "hellow")]
    [DeButton(DePosition.HThirdLeft, typeof(DeAttributesExample), "SamplePublic")]
    [DeButton(DePosition.HThirdMiddle, null, "00ff00", typeof(DeAttributesExample), "SamplePublic")]
    [DeButton(DePosition.HThirdRight, "d39fff", "ac6be3", typeof(DeAttributesExample), "SamplePublic")]
    [DeButton(DePosition.HHalfLeft, typeof(DeAttributesExample), "SamplePublic")]
    [DeButton(DePosition.HHalfRight, typeof(DeAttributesExample), "SamplePublic")]
    [DeButton(DePosition.HDefault, typeof(DeAttributesExample), "SamplePublic")]
    public string run0 = "Below me there's DeMethodButtons instead";

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

    [DeMethodButton("Custom text no params")]
    [DeMethodButton("Custom text and params", 1, "CUSTOM!")]
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