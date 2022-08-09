namespace BlazorCalendar.Styles;

public sealed class Colors
{
    public static string GetHatching(FillStyleEnum FillStyle, string HexaColor)
    {

        string css;
        switch (FillStyle)
        {
            case FillStyleEnum.BackwardDiagonal:
                css = $"background: repeating-linear-gradient(-45deg,{HexaColor}78 0px,{HexaColor}78 5px,{HexaColor} 5px,{HexaColor} 10px);";
                break;

            case FillStyleEnum.CrossDots:
                css = $"background-image: radial-gradient({HexaColor} 3px, {HexaColor}78 1px),radial-gradient({HexaColor} 3px, transparent 1px);" +
                       $"background-size: calc(14 * 1px) calc(14 * 1px);" +
                       $"background-position: 0 0,calc(7 * 1px) calc(7 * 1px);";
                break;

            case FillStyleEnum.ZigZag:
                css = $"background-color: {HexaColor}78 !important;" +
                      $"background: linear-gradient(135deg, {HexaColor} 25%, transparent 25%) -8px 0,linear-gradient(225deg, {HexaColor} 25%, transparent 25%) -8px 0,linear-gradient(315deg, {HexaColor} 25%, transparent 25%),linear-gradient(45deg, {HexaColor} 25%, transparent 25%);" +
                      "background-size: calc(2 * 8px) calc(2 * 8px);";
                break;

            case FillStyleEnum.Triangles:
                css = $"background-image: linear-gradient(45deg, {HexaColor} 50%, {HexaColor}78 50%);" +
                      $"background-size: 10px 10px;";
                break;

            default: // FillStyleEnum.Solid
                css = $"background-color:{HexaColor};";

                break;
        }

        return css;
    }
}
