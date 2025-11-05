namespace Application.Books;

public static class BookErrorCodes
{
    public static class CreateBook
    {
        public const string MissingTitle = nameof(MissingTitle);
        public const string InvalidPrice = nameof(InvalidPrice);
        public const string InvalidISBN = nameof(InvalidISBN);
    }

    public static class UpdateBook
    {
        public const string EmptyValues = nameof(EmptyValues);
    }
}
