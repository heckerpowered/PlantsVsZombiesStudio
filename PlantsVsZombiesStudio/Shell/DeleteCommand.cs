namespace PlantsVsZombiesStudio.Shell
{
    public class DeleteCommand : CommandBase
    {
        public DeleteCommand(string fileName)
        {
            FileName = fileName;
        }
        public override string OperationCode => "del";

        public string FileName { get; }

        public override string Process()
        {
            return $"del {FileName}";
        }
    }
}
