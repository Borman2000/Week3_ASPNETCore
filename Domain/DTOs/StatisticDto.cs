namespace Domain.DTOs;

public class StatisticDto
{
	public Dictionary<string, int> NumBooksByCategory { get; set; }
	public Dictionary<string, decimal> AvgPriceByAuthor { get; set; }
	public Dictionary<string, decimal> TopPrices { get; set; }
}