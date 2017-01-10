using DG.DemiLib.Attributes;
using UnityEngine;

public class DeAttributesExample : MonoBehaviour
{
    const string _MainHeaderTextColor = "ffffff";
    const string _MainHeaderBgColor = "198519";
    const FontStyle _MainHeaderFontStyle = FontStyle.Bold;
    const int _MainHeaderFontSize = 18;

    [DeHeader("DeHeaders examples", _MainHeaderTextColor, _MainHeaderBgColor, _MainHeaderFontStyle, _MainHeaderFontSize)]
    public float aFloat = 2;
    [DeHeader("A default header")]
    public float aFloat0 = 2;
    [DeHeader("A colored header", "ffd860", "a154df")]
    public float aFloat1 = 2;
    [DeHeader("A colored header with custom font style and size", "ffffff", "ff5e31", FontStyle.Normal, 9)]
    public float aFloat2 = 2;
    [DeHeader("More custom font style and size", "380b0b", "ffc331", FontStyle.Italic, 16)]
    public float aFloat3 = 2;
    [DeHeader("A right aligned colored header", TextAnchor.MiddleRight, "ffffff", "327dcc")]
    public float aFloat4 = 2;
    [DeHeader("Another colored header then an array", "ffffff", "000000")]
    public int[] anArray = new []{ 1, 2, 3 };

    [DeHeader("DeColoredLabel examples", _MainHeaderTextColor, _MainHeaderBgColor, _MainHeaderFontStyle, _MainHeaderFontSize)]
    [DeColoredLabel("ffd860", "a154df")]
    public string aString0 = "Hellow";
    [DeColoredLabel("ffffff", "ff5e31")]
    public string aString1 = "Hellow again";
    [DeColoredLabel("222222", "ffc331")]
    public string aString2 = "Hellow enough";

    [DeHeader("DeConditional examples", _MainHeaderTextColor, _MainHeaderBgColor, _MainHeaderFontStyle, _MainHeaderFontSize)]
    [DeComment("You can choose to disable or completely hide a property based on given conditions.")]
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

    [DeHeader("DeComment examples", _MainHeaderTextColor, _MainHeaderBgColor, _MainHeaderFontStyle, _MainHeaderFontSize)]
    [DeComment("Some example of how DeComment works, either with default values or with colored comments.")]
    public bool comment0 = true;
    [DeComment("This one is colored, gosh!\nIt almost looks like a colored DeHeader, if not for the wordWrap.", "ffd860", "a154df")]
    public bool comment1 = true;

    [DeHeader("DeDivider examples", _MainHeaderTextColor, _MainHeaderBgColor, _MainHeaderFontStyle, _MainHeaderFontSize)]
    [DeDivider]
    public string divider0 = "default";
    [DeDivider(4)]
    public string divider1 = "custom height";
    [DeDivider(2, "a154df")]
    public string divider2 = "colored";
}