using CSharpFunctionalExtensions;
using System.Text.RegularExpressions;

namespace Poshta.Core.Models
{
    public class User
    {
        public const string NamePattern = @"^[A-Za-zÀ-ÿ'-]{1,50}$";
        public const string PhoneNumberPattern = @"^\+?[0-9\s\-()]{7,20}$";

        private User(
            Guid id,
            string lastName,
            string firstName,
            string passwordHash,
            string phoneNumber,
            string? middlename = null)
        {
            Id = id;
            LastName = lastName;
            FirstName = firstName;
            PasswordHash = passwordHash;
            PhoneNumber = phoneNumber;
            MiddleName = middlename;
        }

        public Guid Id { get; }

        public string LastName { get; } = string.Empty;

        public string FirstName { get; } = string.Empty;

        public string PasswordHash { get; } = string.Empty;

        public string PhoneNumber { get; } = string.Empty;

        public string? Email { get; set; }

        public string? MiddleName { get; }

        public static Result<User> Create(
            Guid id,
            string lastName,
            string firstName,
            string passwordHash,
            string phoneNumber,
            string? middlename = null)
        {
            if (!Regex.IsMatch(lastName, NamePattern))
                return Result.Failure<User>("Invalid last name");

            if (!Regex.IsMatch(firstName, NamePattern))
                return Result.Failure<User>("Invalid first name");

            if (!string.IsNullOrEmpty(middlename) && !Regex.IsMatch(middlename, NamePattern))
                return Result.Failure<User>("Invalid middle name");

            if (!Regex.IsMatch(phoneNumber, PhoneNumberPattern))
                return Result.Failure<User>("Invalid phone");

            var cleanedPhoneNumber = Regex.Replace(phoneNumber, @"[^\d+]", "");

            return new User(id, lastName, firstName, passwordHash, cleanedPhoneNumber, middlename);
        }
    }
}
