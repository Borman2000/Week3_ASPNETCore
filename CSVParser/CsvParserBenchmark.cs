namespace CSVParser;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

[MemoryDiagnoser]
[HardwareCounters(HardwareCounter.CacheMisses, HardwareCounter.BranchMispredictions)]
[SimpleJob(RuntimeMoniker.Net80)]
[MarkdownExporter]
public class CsvParserBenchmark
{
	private string _csvFake = "title;isbn;firstName;lastName;birthDate;bio;price;categories;year;email\r\n";

	[Params(100,1000,10000)]
	public int Iterations;

	[GlobalSetup]
	public void Setup()
	{
		for (int i = 0; i < Iterations; i++)
		{
			_csvFake += $"title{i};9780061120084;firstName{i};lastName{i};{DateOnly.FromDayNumber(i)};bio{i};{i};categories{i};{DateOnly.FromDayNumber(i).Year};email{i}\r\n";
		}
	}

	[Benchmark]
	public void ParseWithSplit()
	{
		ICsvParser parserSimple = new ParserSimple();
		parserSimple.ParseBooks(_csvFake);
	}

	[Benchmark]
	public void ParseWithSpan()
	{
		ICsvParser parserMem = new ParserMemory();
		parserMem.ParseBooks(_csvFake);
	}
}