namespace Banks.Interfaces
{
    public interface IObserver
    {
        IObserverId? Id { get; }
        void HaveBeenNotified(ISubject subject);
    }
}
