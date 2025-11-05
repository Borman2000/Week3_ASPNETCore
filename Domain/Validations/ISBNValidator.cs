namespace Domain.Validations;

public class ISBNValidator
{
	public static bool IsValid(string isbn) {
		if (string.IsNullOrWhiteSpace(isbn))
			return false;

		// Remove hyphens and spaces
		isbn = isbn.Replace("-", "").Replace(" ", "");

		// Validate 10-digit ISBN
		if (isbn.Length == 10)
		{
			int sum = 0;
			for (int i = 0; i < 9; i++)
			{
				if (!int.TryParse(isbn[i].ToString(), out int num)) return false;
				sum += num * (10 - i);
			}

			char lastChar = isbn[9];
			int lastDigit = (lastChar == 'X' || lastChar == 'x') ? 10 : (int.TryParse(lastChar.ToString(), out int digit) ? digit : -1);
			if (lastDigit == -1) return false;

			sum += lastDigit;
			return (sum % 11 == 0);
		}
		// Validate 13-digit ISBN
		else if (isbn.Length == 13)
		{
			int sum = 0;
			for (int i = 0; i < 13; i++)
			{
				if (!int.TryParse(isbn[i].ToString(), out int digit)) return false;
				sum += (i % 2 == 0) ? digit * 1 : digit * 3;
			}
			return (sum % 10 == 0);
		}

		return false; // Invalid length
	}
}