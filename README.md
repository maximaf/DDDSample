# DDDSample
Sample of rich domain model using Domain Driven Design technique

## Principles

Domain behavior is completely placed into domain objects. Accesibility of properties is strictly handled by public/private modifiers.
Instances are created via factory methods. Domain objects can be also serialized into any persistent storage via any available serializer/deserializer and deserialized using implicit contructor and field setters invoked by reflection.

## Domain Model

- User
  - aggregate root
  - factories
    - User CreateUser(string userEmail, string userName)
  - commands
    - void DeleteOwnAccount(User user)

- Group
  - aggregate root
  - aggregates
    - GroupUserRole collection
  - factories
    - Group CreateGroup(User groupFounder, string groupName)
  - commands
    - void DeleteOwnAccount(User user)
    - void AssignGroupOwnership(User groupOwner, User affectedMember)
    - void UnassignGroupOwnership(User groupOwner, User affectedMember)
    - void UnassignOwnGroupOwnership(User affectedOwner)
    - void AssignGroupMembership(User groupOwner, User affectedMember)
    - void UnassignGroupMembership(User groupOwner, User affectedMember)
    - void UnassignOwnGroupMembership(User affectedMember)
  - queries
    - bool IsOwner(Guid userId)
    - bool HasAnotherOwner(Guid userId)

- GroupUserRole
  - nested entity
  - factories
    - GroupUserRole CreateUserRole(Guid userId, Guid assignedBy, bool isOwner = false)
    - GroupUserRole CreateFounderRole(User groupFounder)
