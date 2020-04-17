using System;

namespace DDDSample.Domain.Annotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ReferencedEntityTypeAttribute : Attribute
    {
        public Type Type { get; }

        public ReferencedEntityTypeAttribute(Type type)
        {
            Type = type;
        }
    }
}
