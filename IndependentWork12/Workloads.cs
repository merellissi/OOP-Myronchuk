namespace IndependentWork12;


public static class Workloads
{
   
    public static bool IsPrime(int n)
    {
        if (n < 2) return false;
        if (n % 2 == 0) return n == 2;

        int limit = (int)Math.Sqrt(n);
        for (int divisor = 3; divisor <= limit; divisor += 2)
        {
            if (n % divisor == 0)
                return false;
        }
        return true;
    }



    public static long SumOfPrimesLinq(List<int> numbers) =>
        numbers
            .Where(IsPrime)
            .Select(n => (long)n)
            .Sum();

    public static long SumOfPrimesPlinq(List<int> numbers) =>
        numbers
            .AsParallel()
            .Where(IsPrime)
            .Select(n => (long)n)
            .Sum();

    

    public static long SumOfEvenDoubledLinq(List<int> numbers) =>
        numbers
            .Where(n => n % 2 == 0)
            .Select(n => (long)n * 2)
            .Sum();

    public static long SumOfEvenDoubledPlinq(List<int> numbers) =>
        numbers
            .AsParallel()
            .Where(n => n % 2 == 0)
            .Select(n => (long)n * 2)
            .Sum();
}