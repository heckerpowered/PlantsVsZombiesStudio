using System.Collections.Generic;
using System.IO;

namespace PlantsVsZombiesStudio.Shell
{
    public class CommandBuilder
    {
        public List<CommandBase> Commands { get; init; }
        public CommandBuilder()
        {
            Commands = new();
        }

        public void WriteTo(Stream stream)
        {
            using var streamWriter = new StreamWriter(stream);

            foreach (var command in Commands)
                streamWriter.WriteLine(command.Process());
        }
    }
}
