namespace IntegrationTests.Helpers;

public class TestValues
{
	public static readonly Guid AUTHOR_ID_EXISTS = Guid.Parse("AB29FC40-CA47-1067-B31D-00DD010662D1");
	public static readonly Guid AUTHOR_ID_NOT_EXISTS = Guid.Parse("AAAAAAAA-BBBB-CCCC-DDDD-000000000000");

	public static readonly string ISBN_VALID = "978-0-7432-4722-1";
	public static readonly string ISBN_INVALID = "123-4-4568-9123-4";

	public static readonly string TITLE = "Test Book Title";

	public static readonly Guid BOOK_ID1_EXISTS = Guid.Parse("BE61D971-5EBC-4F02-A3A9-6C82895E5C01");
	public static readonly Guid BOOK_ID2_EXISTS = Guid.Parse("BE61D971-5EBC-4F02-A3A9-6C82895E5C05");
	public static readonly Guid BOOK_ID_NOT_EXISTS = Guid.Parse("AAAAAAAA-BBBB-CCCC-DDDD-000000000000");
}