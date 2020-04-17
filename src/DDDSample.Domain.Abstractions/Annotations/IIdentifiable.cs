namespace DDDSample.Domain.Annotations
{
    public interface IIdentifiable<T>
    {
        T Id { get; }
    }
}