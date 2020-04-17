using System;
using System.Linq;
using DDDSample.Domain.Administration;
using Shouldly;
using Xunit;

namespace DDDSample.Domain.Tests
{
    public class AdministrationTests
    {
        User TestUser => User.CreateUser("tester@tester.net", "Tester");
        User GroupOwner => User.CreateUser("owner@group.net", "Owner");

        [Fact]
        public void CreateNewGroup_Returns_NewGroupWithOwner()
        {
            // Arrange
            var user = TestUser;

            // Act
            var group = Group.CreateGroup(user, "Test group");

            // Assert
            group.Id.ShouldNotBe(Guid.Empty);
            group.Name.ShouldBe("Test group");
            group.IsOwner(user.Id).ShouldBeTrue();
            group.HasAnotherOwner(user.Id).ShouldBeFalse();
            
            var role = group.UserRoles.ShouldHaveSingleItem();
            role.UserId.ShouldBe(user.Id);
            role.AssignedBy.ShouldBe(user.Id);
            role.IsOwner.ShouldBeTrue();
        }

        [Fact]
        public void IsOwner_ForGroupOwner_Returns_True()
        {
            // Arrange
            var user = TestUser;
            var group = Group.CreateGroup(user, "Test group");

            // Act + Assert
            group.IsOwner(user.Id).ShouldBeTrue();
            group.HasAnotherOwner(user.Id).ShouldBeFalse();
            group.UserRoles.ShouldContain(ur => ur.UserId == user.Id);

            var role = group.UserRoles.Single(ur => ur.UserId == user.Id);
            role.IsOwner.ShouldBeTrue();
        }

        [Fact]
        public void HasAnotherOwner_ForMultipleOwners_Returns_True()
        {
            // Arrange
            var user = TestUser;
            var groupOwner = GroupOwner;
            var group = Group.CreateGroup(groupOwner, "Test group");
            group.AssignGroupOwnership(groupOwner, user);

            // Act + Assert
            group.HasAnotherOwner(user.Id).ShouldBeTrue();
            group.IsOwner(user.Id).ShouldBeTrue();
            group.IsOwner(groupOwner.Id).ShouldBeTrue();
        }

        [Fact]
        public void AssignGroupMembership_AssignsAdditionalMember()
        {
            // Arrange
            var user = TestUser;
            var groupOwner = GroupOwner;
            var group = Group.CreateGroup(groupOwner, "Test group");

            // Act
            group.AssignGroupMembership(groupOwner, user);

            // Assert
            group.IsOwner(user.Id).ShouldBeFalse();
            group.UserRoles.ShouldContain(ur => ur.UserId == user.Id);
            group.UserRoles.ShouldContain(ur => ur.UserId == groupOwner.Id);

            var role = group.UserRoles.Single(ur => ur.UserId == user.Id);
            role.UserId.ShouldBe(user.Id);
            role.AssignedBy.ShouldBe(groupOwner.Id);
            role.IsOwner.ShouldBeFalse();
        }

        [Fact]
        public void UnassignGroupMembership_UnassignsExistingMember()
        {
            // Arrange
            var user = TestUser;
            var groupOwner = GroupOwner;
            var group = Group.CreateGroup(groupOwner, "Test group");
            group.AssignGroupMembership(groupOwner, user);

            // Act
            group.UnassignGroupMembership(groupOwner, user);

            // Assert
            group.IsOwner(user.Id).ShouldBeFalse();
            group.UserRoles.ShouldNotContain(ur => ur.UserId == user.Id);
            group.UserRoles.ShouldContain(ur => ur.UserId == groupOwner.Id);
        }

        [Fact]
        public void UnassignOwnGroupMembership_ForMultipleOwners_UnassignsSelfFromGroup()
        {
            // Arrange
            var user = TestUser;
            var groupOwner = GroupOwner;
            var group = Group.CreateGroup(groupOwner, "Test group");
            group.AssignGroupMembership(groupOwner, user);

            // Act
            group.UnassignOwnGroupMembership(user);

            // Assert
            group.IsOwner(user.Id).ShouldBeFalse();
            group.UserRoles.ShouldNotContain(ur => ur.UserId == user.Id);
            group.UserRoles.ShouldContain(ur => ur.UserId == groupOwner.Id);
        }

        [Fact]
        public void AssignGroupOwnership_AssignsAdditionalOwner()
        {
            // Arrange
            var user = TestUser;
            var groupOwner = GroupOwner;
            var group = Group.CreateGroup(groupOwner, "Test group");

            // Act
            group.AssignGroupOwnership(groupOwner, user);

            // Assert
            group.IsOwner(user.Id).ShouldBeTrue();
            group.UserRoles.ShouldContain(ur => ur.UserId == user.Id);
            group.UserRoles.ShouldContain(ur => ur.UserId == groupOwner.Id);

            var role = group.UserRoles.Single(ur => ur.UserId == user.Id);
            role.UserId.ShouldBe(user.Id);
            role.AssignedBy.ShouldBe(groupOwner.Id);
            role.IsOwner.ShouldBeTrue();
        }

        [Fact]
        public void UnassignGroupOwnership_ForMultipleOwners_UnassignsAdditionalOwner()
        {
            // Arrange
            var user = TestUser;
            var groupOwner = GroupOwner;
            var group = Group.CreateGroup(groupOwner, "Test group");
            group.AssignGroupOwnership(groupOwner, user);

            // Act
            group.UnassignGroupOwnership(groupOwner, user);

            // Assert
            group.IsOwner(user.Id).ShouldBeFalse();
            group.UserRoles.ShouldContain(ur => ur.UserId == user.Id);
            group.UserRoles.ShouldContain(ur => ur.UserId == groupOwner.Id);

            var role = group.UserRoles.Single(ur => ur.UserId == user.Id);
            role.UserId.ShouldBe(user.Id);
            role.AssignedBy.ShouldBe(groupOwner.Id);
            role.IsOwner.ShouldBeFalse();
        }

        [Fact]
        public void UnassignOwnGroupOwnership_ForMultipleOwners_UnassignsSelfFromOwners()
        {
            // Arrange
            var user = TestUser;
            var groupOwner = GroupOwner;
            var group = Group.CreateGroup(groupOwner, "Test group");
            group.AssignGroupOwnership(groupOwner, user);

            // Act
            group.UnassignOwnGroupOwnership(user);

            // Assert
            group.IsOwner(user.Id).ShouldBeFalse();
            group.UserRoles.ShouldContain(ur => ur.UserId == user.Id);
            group.UserRoles.ShouldContain(ur => ur.UserId == groupOwner.Id);

            var role = group.UserRoles.Single(ur => ur.UserId == user.Id);
            role.UserId.ShouldBe(user.Id);
            role.AssignedBy.ShouldBe(user.Id);
            role.IsOwner.ShouldBeFalse();
        }
    }
}
