using System;

public class UserAlreadyInMatchException : Exception
{
    private UserAlreadyInMatchException() : base("User already in match") { }
    public static UserAlreadyInMatchException Instance { get; } = new();
}