namespace PlantsVsZombiesStudio.Shell
{
    public interface ICommand
    {
        public abstract string OperationCode { get; }
        public string Process();
    }
}
