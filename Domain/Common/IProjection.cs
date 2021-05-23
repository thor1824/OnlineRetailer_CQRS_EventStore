namespace OnlineRetailer.Domain.Common
{
    public interface IProjection<TProjection>
    {
        BaseStream Stream { get; }
        TProjection Aggregate { get; }

        void ApplyEvent(IEvent evnt);
    }
}