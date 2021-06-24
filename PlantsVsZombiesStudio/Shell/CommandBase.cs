namespace PlantsVsZombiesStudio.Shell
{
    public abstract class CommandBase : ICommand
    {
        public abstract string OperationCode { get; }

        public abstract string Process();
    }
}
