using System;
using Firebase;

// maps Firebase error codes to readable messages
public static class FirebaseErrorMapper
{
    public static string GetMessage(AggregateException ex)
    {
        if (ex?.InnerException is FirebaseException fe)
        {
            switch (fe.ErrorCode)
            {
                case 17007: return "This email is already registered.";
                case 17009: return "Wrong password. Try again.";
                case 17011: return "No account found with this email.";
                case 17008: return "Invalid email address. Check for typos.";
                default:    return fe.Message;
            }
        }
        return "Something went wrong. Please try again.";
    }
}
