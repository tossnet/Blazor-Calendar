namespace BlazorWebAssembly.Helpers;

public static class Colors
{
    public static bool IsLighter(ReadOnlySpan<char> color, byte level = 48)
    {
        var (isValid, rgb) = HexToRgb(color);
        if (!isValid)
            return false;

        // Calcul de la luminosité (formule HSL simplifiée)
        byte max = Math.Max(Math.Max(rgb.R, rgb.G), rgb.B);
        byte min = Math.Min(Math.Min(rgb.R, rgb.G), rgb.B);
        int lightness = (max + min) / 2 * 100 / 255;

        return lightness > level;
    }

    public static (bool IsValid, (byte R, byte G, byte B) Color) HexToRgb(ReadOnlySpan<char> hex)
    {
        if (hex.IsEmpty || hex.IsWhiteSpace())
            return (false, (0, 0, 0));

        // Supprime le # si présent
        if (hex[0] == '#')
            hex = hex[1..];

        if (hex.Length != 6)
            return (false, (0, 0, 0));

        if (byte.TryParse(hex[..2], System.Globalization.NumberStyles.HexNumber, null, out byte r) &&
            byte.TryParse(hex[2..4], System.Globalization.NumberStyles.HexNumber, null, out byte g) &&
            byte.TryParse(hex[4..6], System.Globalization.NumberStyles.HexNumber, null, out byte b))
        {
            return (true, (r, g, b));
        }

        return (false, (0, 0, 0));
    }
}
