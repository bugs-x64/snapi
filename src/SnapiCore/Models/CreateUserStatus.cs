namespace SnapiCore.Services
{
    public enum CreateUserStatus
    {
        Created,
        AlreadyExists,
        TooShortName,
        TooLongName
    }
}