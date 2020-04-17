using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DDDSample.Domain.Annotations;

namespace DDDSample.Domain.Administration
{
    public class Group : IIdentifiable<Guid>
    {
        public static Group CreateGroup(User groupFounder, string groupName)
        {
            var ownerRole = GroupUserRole.CreateFounderRole(groupFounder);
            return new Group
            {
                Id = Guid.NewGuid(),
                Name = groupName,
                UserRoles = new[] { ownerRole }
            };
        }

        [Required]
        public Guid Id { get; private set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public GroupUserRole[] UserRoles { get; private set; }

        public bool IsOwner(Guid userId)
        {
            return UserRoles.Any(role => role.UserId == userId && role.IsOwner);
        }

        public bool HasAnotherOwner(Guid userId)
        {
            return UserRoles.Any(role => role.UserId != userId && role.IsOwner);
        }

        private void CheckUserOwnership(Guid userId)
        {
            if (!IsOwner(userId))
            {
                throw new ArgumentException("User isn't a group owner.", nameof(userId));
            }
        }

        private void CheckAnotherOwnership(Guid userId)
        {
            if (!HasAnotherOwner(userId))
            {
                throw new ArgumentException("User is the single group owner.", nameof(userId));
            }
        }

        private void SetUserRole(Guid userId, GroupUserRole userRole)
        {
            var userDict = UserRoles.ToDictionary(r => r.UserId);
            userDict[userId] = userRole;
            UserRoles = userDict.Values.ToArray();
        }

        private void UnsetUserRole(Guid userId)
        {
            var userRoles = UserRoles.Where(ur => ur.UserId != userId);
            UserRoles = userRoles.ToArray();
        }

        public void AssignGroupOwnership(User groupOwner, User affectedMember)
        {
            CheckUserOwnership(groupOwner.Id);

            var ownerRole = GroupUserRole.CreateUserRole(affectedMember.Id, groupOwner.Id, true);
            SetUserRole(affectedMember.Id, ownerRole);
        }

        public void UnassignGroupOwnership(User groupOwner, User affectedMember)
        {
            CheckUserOwnership(groupOwner.Id);
            CheckAnotherOwnership(affectedMember.Id);

            var memberRole = GroupUserRole.CreateUserRole(affectedMember.Id, groupOwner.Id);
            SetUserRole(affectedMember.Id, memberRole);
        }

        public void UnassignOwnGroupOwnership(User affectedOwner)
        {
            UnassignGroupOwnership(affectedOwner, affectedOwner);
        }

        public void AssignGroupMembership(User groupOwner, User affectedMember)
        {
            CheckUserOwnership(groupOwner.Id);

            var memberRole = GroupUserRole.CreateUserRole(affectedMember.Id, groupOwner.Id);
            SetUserRole(affectedMember.Id, memberRole);
        }

        public void UnassignGroupMembership(User groupOwner, User affectedMember)
        {
            CheckUserOwnership(groupOwner.Id);
            CheckAnotherOwnership(affectedMember.Id);

            UnsetUserRole(affectedMember.Id);
        }

        public void UnassignOwnGroupMembership(User affectedMember)
        {
            CheckAnotherOwnership(affectedMember.Id);

            UnsetUserRole(affectedMember.Id);
        }
    }
}
