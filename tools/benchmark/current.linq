<Query Kind="Program">
  <Reference Relative="..\..\MAB.DotIgnore\bin\Release\netstandard1.3\MAB.DotIgnore.dll">C:\Src\MAB.DotIgnore\MAB.DotIgnore\bin\Release\netstandard1.3\MAB.DotIgnore.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.IO.FileSystem.dll</Reference>
  <Namespace>MAB.DotIgnore</Namespace>
</Query>

#LINQPad optimize+

void Main()
{
    Util.NewProcess = true;
    
    var workingDirectory = Path.GetDirectoryName(Util.CurrentQueryPath);
    
    // This gives us an array of ~1500 file paths
    var fileList = File.ReadAllLines($@"{workingDirectory}\filelist.txt")
        .Select(l => l.Trim('"').Replace(@"C:\", "").Replace(@"\", "/"))
        .Where(l => !string.IsNullOrWhiteSpace(l))
        .ToList();

    var ignoreList = new IgnoreList($@"{workingDirectory}\.ignores");
    
    ignoreList.IsIgnored("TEST", false);

    Action action = () => fileList.ForEach(f => ignoreList.IsIgnored(f, pathIsDirectory: false));

    Benchmark.Perform(action, 1);

    var result = Benchmark.Perform(action, 500);
    
    $"Completed in {result.total}ms (average {result.average})".Dump("Result");
}

public class Benchmark
{
	public static (double average, long total) Perform(Action action, int iterations = 1)
	{
		var timings = new List<long>();
        
		for (int i = 0; i < iterations; i++)
		{
            var stopwatch = Stopwatch.StartNew();
			
            action();
            
            stopwatch.Stop();
            
            timings.Add(stopwatch.ElapsedMilliseconds);
		}
        
		return (timings.Average(), timings.Sum());
	}
}