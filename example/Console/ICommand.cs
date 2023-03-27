namespace XboxAuthNetConsole
{
    public interface ICommand
    {
        Task Execute(CancellationToken cancellationToken);
    }
}