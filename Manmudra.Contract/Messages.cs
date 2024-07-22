namespace Manmudra.Contract
{
    public class Messages
    {
        public static class Success
        {
            public static string UserRegistered = "User registered successfully";
            public static string LoggedIn = "Logged in successfully";
            public static string LoggedOut = "Logged out successfully";
            public static string RefreshedToken = "Refreshed token successfully";
            public static string Fetched = "Data fetched successfully";
            public static string Deleted = "Record deleted successfully";
            public static string Added = "Record added successfully";
            public static string Updated = "Record updated successfully";
            public static string SetPassword = "Set password successfully";
            public static string ResetPassword = "Reset password successfully";
            public static string EmailSent = "Thanks for contacting us. We will get back to you soon.";
            public static string MessageSent = "Message sent successfully.";
        }

        public static class Failure
        {
            public static string InvalidCredentials = "Invalid pin";
            public static string InvalidRequest = "Something went wrong";
            public static string NoRecordFound = "No record found";
            public static string NoPassword = "No password";
            public static string AlreadyExists = "Record already exists";
            public static string AttemptsExpired = "Attempts expired";
            public static string OtpError = "The system encountered an error processing the request. Please try again.";
        }
    }
}
