using System.Linq;
using P02.Core;
using Xunit;

namespace P02.Tests;

public class MemoryBankTests
{
    [Fact]
    public void Add_StoresRecords_InOrder()
    {
        var stream = new MemoryBank();
        stream.Add("狼群袭击了我", day: 1, importance: 7);
        stream.Add("我捡到一把斧头", day: 2, importance: 6);

        Assert.Equal(2, stream.Count);
        Assert.Equal("狼群袭击了我", stream.All[0].Content);
        Assert.Equal(2, stream.All[1].Day);
    }

    [Fact]
    public void MemoryRecord_ClampsImportanceTo010()
    {
        var m = new MemoryRecord("x", 1, 99);

        Assert.Equal(10f, m.Importance);
    }
}

public class MemoryRetrievalTests
{
    private static MemoryBank SampleStream()
    {
        var stream = new MemoryBank();
        stream.Add("森林里的狼群很危险", day: 1, importance: 8);
        stream.Add("村子的居民很友善", day: 3, importance: 3);
        stream.Add("河边的鱼很多", day: 5, importance: 4);
        return stream;
    }

    [Fact]
    public void Retrieve_RelevantMemories_AreReturned()
    {
        var stream = SampleStream();
        var retrieval = new MemoryRetrieval(topK: 5);

        var result = retrieval.Retrieve(stream, "森林 危险", currentDay: 6);

        Assert.Contains(result, m => m.Content.Contains("森林"));
        Assert.DoesNotContain(result, m => m.Content.Contains("河边"));
    }

    [Fact]
    public void Retrieve_RecentWinsOverOld_WhenEqualOtherwise()
    {
        // 同样的词、同样重要性，更近的应该排前面
        var stream = new MemoryBank();
        stream.Add("狼群出现", day: 1, importance: 5);
        stream.Add("狼群出现", day: 5, importance: 5);
        var retrieval = new MemoryRetrieval(topK: 5);

        var result = retrieval.Retrieve(stream, "狼群", currentDay: 6);

        Assert.Equal(5, result[0].Day); // 更近的排第一
        Assert.Equal(1, result[1].Day);
    }

    [Fact]
    public void Retrieve_ImportantWinsOverTrivial_WhenSameRecency()
    {
        var stream = new MemoryBank();
        stream.Add("狼群袭击", day: 5, importance: 9);
        stream.Add("捡到石头", day: 5, importance: 2);
        var retrieval = new MemoryRetrieval(topK: 5);

        var result = retrieval.Retrieve(stream, "狼群 石头", currentDay: 6);

        Assert.Contains("狼群袭击", result[0].Content); // 重要性高的排第一
    }

    [Fact]
    public void Retrieve_RespectsTopK()
    {
        var stream = new MemoryBank();
        stream.Add("a 狼群", 1, 5);
        stream.Add("b 狼群", 2, 5);
        stream.Add("c 狼群", 3, 5);
        stream.Add("d 狼群", 4, 5);
        var retrieval = new MemoryRetrieval(topK: 2);

        var result = retrieval.Retrieve(stream, "狼群", currentDay: 10);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Retrieve_NoRelevant_ReturnsEmpty()
    {
        var stream = new MemoryBank();
        stream.Add("狼群危险", 1, 8);
        var retrieval = new MemoryRetrieval();

        var result = retrieval.Retrieve(stream, "龙", currentDay: 2);

        Assert.Empty(result);
    }
}

public class ReflectionTests
{
    [Fact]
    public void Reflect_NoMemories_ReturnsNone()
    {
        var result = Reflection.Reflect(System.Array.Empty<MemoryRecord>(), "狼群");

        Assert.Contains("暂无", result);
    }

    [Fact]
    public void Reflect_HasMemories_Summarizes()
    {
        var memories = new[]
        {
            new MemoryRecord("狼群袭击了我", 1, 8),
            new MemoryRecord("狼群又来了", 3, 7),
        };

        var result = Reflection.Reflect(memories, "狼群");

        Assert.Contains("2 件", result);
        Assert.Contains("狼群袭击了我", result);
    }

    [Fact]
    public void TopTopics_ReturnsFrequentWords()
    {
        var stream = new MemoryBank();
        stream.Add("狼群 狼群 危险", 1, 8);
        stream.Add("狼群 出现", 2, 7);

        var topics = Reflection.TopTopics(stream, topN: 2);

        Assert.Equal("狼群", topics.First());
    }
}
