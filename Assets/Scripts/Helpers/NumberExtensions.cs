public static class NumberExtensions
{
    public static string ToShortString(this double value)
    {
        if (value < 1000)
            return value.ToString("0.##"); // just strip extra decimals

        string[] suffixes = { "", "k", "M", "B", "T", "Qa", "Qi" };
        int i = 0;
        double shortened = value;

        while (shortened >= 1000 && i < suffixes.Length - 1)
        {
            shortened /= 1000;
            i++;
        }

        return shortened.ToString("0.##") + suffixes[i]; // abbreviated with max 2 decimals
    }

    public static string ToShortString(this float value)
    {
        if (value < 1000f)
            return value.ToString("0.##"); // just strip extra decimals

        string[] suffixes = { "", "k", "M", "B", "T", "Qa", "Qi" };
        int i = 0;
        float shortened = value;

        while (shortened >= 1000f && i < suffixes.Length - 1)
        {
            shortened /= 1000f;
            i++;
        }

        return shortened.ToString("0.##") + suffixes[i]; // abbreviated with max 2 decimals
    }
}