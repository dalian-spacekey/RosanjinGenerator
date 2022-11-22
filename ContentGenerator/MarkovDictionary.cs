/*
 * Markov chain 
 * https://github.com/shibayan/MarkovDemo
 */
using System.Collections;

namespace ContentGenerator;

public class MarkovDictionary : IEnumerable<KeyValuePair<MarkovKey, List<string>>>
{
    const string Eos = "__END_OF_SENTENCE__";

    readonly Random random = new();
    readonly Dictionary<MarkovKey, List<string>> innerDictionary = new();

    static readonly MarkovKey StartKey = new();

    public void AddSentence(string[] words)
    {
        var key = StartKey;

        foreach (var word in words.Where(x => !string.IsNullOrEmpty(x)))
        {
            innerDictionary.AddOrGetExisting(key).Add(word);

            key = key.Push(word);
        }

        innerDictionary.AddOrGetExisting(key).Add(Eos);
    }

    public IList<string> BuildSentence()
    {
        var result = new List<string>();

        var key = StartKey;

        while (true)
        {
            var list = innerDictionary[key];

            var word = list[random.Next(list.Count)];

            if (word == Eos )
            {
                break;
            }

            result.Add(word);

            key = key.Push(word);
        }

        return result;
    }

    public IEnumerator<KeyValuePair<MarkovKey, List<string>>> GetEnumerator()
    {
        return innerDictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public sealed class MarkovKey : IEquatable<MarkovKey>
{
    public MarkovKey()
    {
        keys = new string[N];
    }

    MarkovKey(params string[] keys)
    {
        this.keys = keys;
    }

    const int N = 2;
    const string DefaultKeySeparator = ":";

    readonly string[] keys;

    public MarkovKey Push(string key)
    {
        var newKeys = new string[N];

        newKeys[0] = key;

        for (int i = 0; i < N - 1; i++)
        {
            newKeys[i + 1] = keys[i];
        }

        return new MarkovKey(newKeys);
    }

    public override int GetHashCode()
    {
        return keys.Aggregate(int.MaxValue, (current, t) => current ^ (t ?? "").GetHashCode());
    }

    public override string ToString()
    {
        return string.Join(DefaultKeySeparator, keys);
    }

    public override bool Equals(object? obj)
    {
        var key = obj as MarkovKey;

        return key != null && Equals(key);
    }

    public bool Equals(MarkovKey other)
    {
        if (keys.Length != other.keys.Length)
        {
            return false;
        }

        return !keys.Where((t, i) => t != other.keys[i]).Any();
    }
}

public static class DictionaryExtensions
{
    public static TValue AddOrGetExisting<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key) where TValue : new() where TKey : notnull
    {
        if (dictionary.TryGetValue(key, out var value)) return value;

        value = new TValue();
        dictionary.Add(key, value);

        return value;
    }
}