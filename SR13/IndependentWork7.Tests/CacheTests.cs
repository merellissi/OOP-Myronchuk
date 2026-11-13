using IndependentWork7;
using Xunit;

namespace IndependentWork7.Tests;

public class CacheTests
{
    

    [Fact]
    public void Add_NewItem_ItemIsStored()
    {
        var cache = new Cache<string, int>(3);

        cache.Add("a", 1);

        Assert.True(cache.ContainsKey("a"));
        Assert.Equal(1, cache.Get("a"));
    }

    [Fact]
    public void Add_ExistingKey_UpdatesValueWithoutIncreasingCount()
    {
        var cache = new Cache<string, int>(3);

        cache.Add("a", 1);
        cache.Add("a", 100);

        Assert.Equal(100, cache.Get("a"));
        Assert.Equal(1, cache.Count);
    }

   

    [Fact]
    public void Get_ExistingKey_ReturnsValue()
    {
        var cache = new Cache<int, string>(2);
        cache.Add(1, "one");
        cache.Add(2, "two");

        Assert.Equal("one", cache.Get(1));
        Assert.Equal("two", cache.Get(2));
    }

    [Fact]
    public void Get_MissingKey_ThrowsKeyNotFoundException()
    {
        var cache = new Cache<string, int>(2);

        Assert.Throws<KeyNotFoundException>(() => cache.Get("nope"));
    }

    

    [Fact]
    public void Add_WhenFull_RemovesOldestItem()
    {
        var cache = new Cache<string, int>(2);
        cache.Add("a", 1);
        cache.Add("b", 2);

        cache.Add("c", 3); 

        Assert.False(cache.ContainsKey("a"));
        Assert.True(cache.ContainsKey("b"));
        Assert.True(cache.ContainsKey("c"));
        Assert.Equal(2, cache.Count);
    }

    [Fact]
    public void Add_SeveralOverflows_RemovesItemsInInsertionOrder()
    {
        var cache = new Cache<int, int>(3);
        for (int i = 1; i <= 5; i++)
            cache.Add(i, i * 10);

      
        Assert.False(cache.ContainsKey(1));
        Assert.False(cache.ContainsKey(2));
        Assert.Equal(30, cache.Get(3));
        Assert.Equal(40, cache.Get(4));
        Assert.Equal(50, cache.Get(5));
    }

    [Fact]
    public void Get_RemovedByOverflow_ThrowsKeyNotFoundException()
    {
        var cache = new Cache<string, int>(1);
        cache.Add("a", 1);
        cache.Add("b", 2);

        Assert.Throws<KeyNotFoundException>(() => cache.Get("a"));
    }

    

    [Fact]
    public void ContainsKey_ExistingKey_ReturnsTrue()
    {
        var cache = new Cache<string, int>(2);
        cache.Add("a", 1);

        Assert.True(cache.ContainsKey("a"));
    }

    [Fact]
    public void ContainsKey_MissingKey_ReturnsFalse()
    {
        var cache = new Cache<string, int>(2);

        Assert.False(cache.ContainsKey("a"));
    }

    
    [Fact]
    public void Count_NewCache_IsZero()
    {
        var cache = new Cache<string, int>(5);

        Assert.Equal(0, cache.Count);
    }

    [Fact]
    public void Count_AfterAdding_IncreasesUntilCapacity()
    {
        var cache = new Cache<int, int>(3);

        cache.Add(1, 1);
        Assert.Equal(1, cache.Count);

        cache.Add(2, 2);
        Assert.Equal(2, cache.Count);

        cache.Add(3, 3);
        cache.Add(4, 4); 
        Assert.Equal(3, cache.Count);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_InvalidCapacity_Throws(int capacity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Cache<string, int>(capacity));
    }
}