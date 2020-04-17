using System;
using System.ComponentModel.DataAnnotations;
using DDDSample.Domain.Annotations;

namespace DDDSample.Domain.Administration
{
    public class User : IIdentifiable<Guid>
    {
        public static User CreateUser(string userEmail, string userName)
        {
            return new User
            {
                Id = Guid.NewGuid(),
                Email = userEmail,
                Name = userName,
            };
        }

        [Required]
        public Guid Id { get; private set; }

        [Required]
        public string Email { get; private set; }

        [Required]
        public string Name { get; set; }

        // Note: supervisor is assignable through database update only
        public bool IsSupervisor { get; private set; }

        public bool IsDeleted { get; private set; }

        public void DeleteOwnAccount(User user)
        {
            if (user.IsSupervisor)
            {
                if (Id == user.Id)
                {
                    throw new ArgumentException("Supervisor can't delete own user account.", nameof(user));
                }
            }
            else
            {
                if (Id != user.Id)
                {
                    throw new ArgumentException("User can't delete other user account.", nameof(user));
                }
            }

            IsDeleted = true;
        }
    }
}
