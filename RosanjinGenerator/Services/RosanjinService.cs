using System.Reflection;
using ContentGenerator;
using NMeCab.Specialized;

namespace RosanjinGenerator.Services;

public class RosanjinService
{
    readonly List<string> sentences = new();

    public RosanjinService()
    {
        var files = new[]
        {
            @"Data\\ryori_issekibanashi.txt",
            @"Data\\dashino_torikata.txt"
        };

        foreach (var file in files)
        {
            var lines = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!, file)).Where(x => !string.IsNullOrEmpty(x));
            sentences.AddRange(lines);
        }

        using var tagger = MeCabIpaDicTagger.Create();
    }

    public string[] Generate(int paragraphCount)
    {
        using var tagger = MeCabIpaDicTagger.Create();
        var markovDic = new MarkovDictionary();
        var paragraphs = new List<string>();

        foreach (var sentence in sentences.Where(x => !string.IsNullOrEmpty(x)))
        {
            var nodes = tagger.Parse(sentence);
            markovDic.AddSentence(nodes.Select(x => x.Surface).ToArray());
        }

        for (var i = 0; i < paragraphCount; i++)
        {
            paragraphs.Add(string.Join("", markovDic.BuildSentence()));
        }

        return paragraphs.ToArray();
    }
}