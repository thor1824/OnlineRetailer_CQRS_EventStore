namespace OnlineRetailer.Domain.Common
{
    public interface IProjector<TProjection>
    {
        BaseStream Stream { get; }
        TProjection Projection { get; }

        void ApplyEvent(IEvent evnt);
    }
}