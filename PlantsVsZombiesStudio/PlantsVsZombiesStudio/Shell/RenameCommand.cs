namespace PlantsVsZombiesStudio.Shell
{
    public class RenameCommand : CommandBase
    {
        public RenameCommand(string file, string newName)
        {
            File = file;
            NewName = newName;
        }
        public override string OperationCode => "rename";

        public string File { get; }
        public string NewName { get; }

        public override string Process()
        {
            return $"rename {File} {NewName}";
        }
    }
}
