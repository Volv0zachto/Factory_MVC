using Xunit;

public class SampleTest
{
    [Fact]
    public void Test1()
    {
        int a = 5;
        int b = 10;
        int sum = a + b;

        Assert.Equal(15, sum);
    }
}