namespace Application.Authors;

public static class AuthorErrorCodes
{
    public static class CreateAuthor
    {
        public const string MissingFirstName = nameof(MissingFirstName);
        public const string MissingLastName = nameof(MissingLastName);
        public const string InvalidBirthDate = nameof(InvalidBirthDate);
        public const string FutureDate = "Birth date cannot be in the future.";
        public const string InvalidEmail = nameof(InvalidEmail);
    }

    public static class UpdateAuthor
    {
        public const string EmptyValues = nameof(EmptyValues);
    }
}
