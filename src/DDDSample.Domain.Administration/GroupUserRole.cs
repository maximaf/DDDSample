using System;
using System.ComponentModel.DataAnnotations;
using DDDSample.Domain.Annotations;

namespace DDDSample.Domain.Administration
{
    public class GroupUserRole
    {
        public static GroupUserRole CreateUserRole(Guid userId, Guid assignedBy, bool isOwner = false)
        {
            return new GroupUserRole { UserId = userId, AssignedBy = assignedBy, IsOwner = isOwner };
        }

        internal static GroupUserRole CreateFounderRole(User groupFounder)
        {
            return new GroupUserRole { UserId = groupFounder.Id, AssignedBy = groupFounder.Id, IsOwner = true };
        }

        [Required]
        [ReferencedEntityType(typeof(User))]
        public Guid UserId { get; private set; }

        [Required]
        [ReferencedEntityType(typeof(User))]
        public Guid AssignedBy { get; private set; }

        public bool IsOwner { get; private set; }
    }
}
