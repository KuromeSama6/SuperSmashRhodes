using System;
using UnityEngine;

namespace SuperSmashRhodes.Util {
public class CUID : IEquatable<CUID> {
    public static readonly string vowels = "aeiou";
    public static readonly string chrSet = "abcdefghijklmnopqrstuvwxz";
    public static readonly string consonants = "bcdfghjklmnpqrstvwxz";
    public static readonly string digits = "0123456789";
    public static readonly int uidLength = 5;
    public static readonly long sizeLimit = (long)Mathf.Pow(chrSet.Length, 2) * (long)Mathf.Pow(consonants.Length, 3) * vowels.Length * 1000;

    
    public string fix = "*****"; // Five readables
    public int idNum = -1;
    public string sector = "*"; // Last sector

    public string text {
        get {
            return $"{fix}{id}{sector}";
        }
    }

    public string id {
        get {
            return idNum.ToString("D3");
        }
    }

    public static CUID Null {
        get {
            return new CUID {
                fix = "XXXXX",
                idNum = 0,
                sector = "X"
            };
        }
    }

    public bool Equals(CUID other) {
        return other.text == text;
    }

    public static CUID Random() {
        var ret = new CUID();
        ret.fix = GeneratePronounceableWord();
        ret.idNum = (int)(UnityEngine.Random.Range(0f, 1f) * 1000);
        ret.sector = RandomSample(chrSet);
        return ret;
    }

    public override string ToString() {
        return text.ToUpper();
    }

    public static CUID FromDenary(long num) {
        if (num >= sizeLimit) throw new CUIDOverflowException($"CUID Overflowed: {num} >= {sizeLimit}");

        int alphabetLength = chrSet.Length;
        int vowelLength = vowels.Length;
        int consonantLength = consonants.Length;

        // the last 4 digits - there are 10^3 * 26 possible combinations
        var ret = new CUID();
        ret.sector = ToChr(num % alphabetLength, (string)null);

        ret.idNum = Mathf.FloorToInt(num / alphabetLength) % 1000;

        string[] fix = new[] {
            "*", "*", "*", "*", "*"
        };
        fix[4] = ToChr(Mathf.Floor(num / 25000), consonants);
        fix[3] = ToChr(Mathf.Floor(num / (25000 * consonantLength)), vowels);

        // the hard part
        // we will let fix[2] take priority first, and then recalculate if fix[1] is the same as fix[2]
        string fix2 = ToChr(Mathf.Floor(num / (25000 * consonantLength * vowelLength)), consonants);
        string fix1 = ToChr(Mathf.Floor(num / (25000 * consonantLength * vowelLength * consonantLength)), Concat(consonants, vowels));
        string fix0 = ToChr(num / (25000 * consonantLength * vowelLength * consonantLength * Concat(consonants, vowels).Length), vowels.Contains(fix1) ? consonants : vowels);

        if (fix1 == fix2) fix2 = "y";

        fix[2] = fix2;
        fix[1] = fix1;
        fix[0] = fix0;

        ret.fix = string.Join(",", fix);

        return ret;
    }

    public static string GeneratePronounceableWord() {
        // Generate the first two characters
        string firstCharSet = UnityEngine.Random.Range(0f, 1f) < 0.5 ? vowels : consonants;
        string secondCharSet = firstCharSet == vowels ? consonants : vowels;
        string secondChar = RandomSample(secondCharSet);
        string firstTwoChars = $"{RandomSample(firstCharSet)}{secondChar}";

        // Generate the last three characters
        string lastThreeChars = "";
        for (int i = 0; i < uidLength - 2; i++) {
            string charSet = "";
            switch (i) {
                case 0:
                    charSet = secondCharSet == consonants ? consonants.Replace(secondChar, "") : consonants;
                    break;
                case 1:
                    charSet = vowels;
                    break;
                case 2:
                    charSet = consonants;
                    break;
            }
            lastThreeChars += RandomSample(charSet);
        }

        return $"{firstTwoChars}{lastThreeChars}";
    }

    private static string RandomSample(string str) {
        return str[Mathf.FloorToInt(UnityEngine.Random.Range(0f, 1f) * str.Length)].ToString();
    }

    private static string ToChr(float str, string sampleSet = null) { return ToChr(str, sampleSet.Split("")); }

    private static string ToChr(float str, string[] sampleSet = null) {
        float num = Mathf.Round(str);
        string[] chrSet = sampleSet ?? "abcdefghijklmnopqrstuvwxz".Split("");
        return chrSet[(int)(num % chrSet.Length)];
    }

    private static string Concat(params string[] strs) {
        string ret = "";
        foreach (string str in strs) ret = ret + str;
        return ret;
    }
}

public class CUIDOverflowException : Exception {
    public CUIDOverflowException() { }
    public CUIDOverflowException(string msg) : base(msg) { }
}
}
