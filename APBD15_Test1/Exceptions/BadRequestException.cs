namespace apbd_testPractice1.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException(string msg) : base(msg) { }
}