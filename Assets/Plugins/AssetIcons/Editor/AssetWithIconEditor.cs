using UnityEditor;

[CustomEditor(typeof(AssetWithIcon))]
public class AssetWithIconEditor : AssetWithIconEditorBase
{
    public override string Base64IconString
    {
        get
        {
            return "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAD80lEQVR4Ae2ZL3PcMBDFk0tIYenB0MKjhfkKpYWlhf0MoYWlhaGFpYWlhaWFpYHd3809z1Zj2ZIlZeKRdmYjnf7ue7ta2c716XS66lkOPYMH+yBgREDnDIwj0HkAjCQ4jsA4Ap0z0P0RuH2hAXBndr2+2Pbbyr+t7GxFAABQDEcBkSq8nX0wpWTu40WbkNCCAIC/u6gVV99NAfGTHyuC1+9Nma8IEJGsUV1aEqD3bAAgqaHMPIFnHvNpg8jqUXCwRWsLxnsA1PEonk2RueMCCX7NlHWSxrQgAAAhCHlxDQQe5qiEnl6blwR2blArAubClTCGiDWZIxACmpDQggB5MYyCVADMDyNgjbTN/S0IwBjCOMz6qaBEoB8Pmf43e1SRm+PxWGWhYJEn+/3KlJCn/GPKNcbRoG9J1E/EMBciv5r+MlWfVetIi2tQlikPQMKcVzVurgT0g+mWuXPrRduux2fxKDd9dLRKgrthbxCwG1c1MrT7CGh5DTby2X/L8qzAVYlw1YZPn+eOpT97JgDg/i2TZ4cvplkk7JmAewPLlyNFACXgISFZDskjX9ZAQj98u4QANPWl64xorwQAFAK8kAOkvn2xvmcCIMGL3jd822q9NAcQbgq5bPZXrZsfoPDXvhpFEsxKgEwsIQAPKAsDnrc/NNsIm5MjsfCHAOzIkq0EwD5Z+JOpPMGZpE4WzjbE5qQK+4TnH9I3EVCSA/CEwGO8IiI0jr5awh4Q7/dlbcCj2bKVAJ330NOA1391so1JmAD4kGBs2OR99ttKAHN15ql7wUj/gOL7SuryPqWXzd5nkZJvgnzng/03pv7Dor4FWvM5IYZRQnuuEPLvL8r6Etb+bPrDdNP3whICMAID2BgS/LnESNoQElQpCW9tjY+mofcBzgfTzTdPKQGA1+YY14IEzjzgOVpeIBXvf/ONufVSAtgPEjgO8roPUbVBDGMYi6aKkirhHwqeR4uiqwYBGIYRigRCPySB6NAxoY/xS0QwXsl0DjwJmOeNTVefzZvkdqqVVyBAr6LcAv44UAcQHmWcMjdEhB4EPOMYTz0U5lYBz8I1CWC9JRLohwjAoXPgrfk8xpNHm0TrEwEhcRqTVdY6An5TDMNQSoD4K9J+TsJRoD9Uf3ymwVZhzQfTR9Mq4G2doucA5scEA/lfnp4VBDI2fqmdtbjuAE/Grwbe1mpGAGuT5CBBKsPxcMzL1jWJSMTjXHeE/VLinCbmVGrngLm9CV0UAEpw5IClqNAcnyxtSn15DgJkNR4VoCXwjFe0qNQa1cvnJMAbD7Dm4PyGsfoh1tFL+yCgF0/HcI4IiDHTS/uIgF48HcM5IiDGTC/t3UfAP7uJyQI+rb+RAAAAAElFTkSuQmCC";
        }
    }

    public override int IconHeight
    {
        get
        {
            return 32;
        }
    }

    public override int IconWidth
    {
        get
        {
            return 32;
        }
    }
}
