namespace Sync.Net.TestHelpers
{
    public class DirectoryHelper
    {
        public const string Contents = "This is file content";
        public const string FileName = "file.txt";
        public const string FileName2 = "file2.txt";
        public const string SubDirectoryName = "dir";
        public const string SubFileName = "subFile.txt";
        public const string SubFileName2 = "subfile2.txt";

        public static MemoryDirectoryObject CreateFullDirectory()
        {
            var memoryDirectoryObject = CreateDirectoryWithFiles();

            memoryDirectoryObject.AddDirectory(
                new MemoryDirectoryObject(DirectoryHelper.SubDirectoryName, memoryDirectoryObject.FullName)
                    .AddFile(DirectoryHelper.SubFileName, DirectoryHelper.Contents)
                    .AddFile(DirectoryHelper.SubFileName2, DirectoryHelper.Contents));
            return memoryDirectoryObject;
        }

        public static MemoryDirectoryObject CreateDirectoryWithFiles()
        {
            var memoryDirectoryObject = CreateDirectoryWithFile();

            memoryDirectoryObject.AddFile(DirectoryHelper.FileName2, DirectoryHelper.Contents);
            return memoryDirectoryObject;
        }

        public static MemoryDirectoryObject CreateDirectoryWithFile()
        {
            var memoryDirectoryObject = CreateEmpty()
                .AddFile(DirectoryHelper.FileName, DirectoryHelper.Contents);
            return memoryDirectoryObject;
        }

        public static MemoryDirectoryObject CreateEmpty()
        {
            return new MemoryDirectoryObject("directory");
        }

        public static MemoryDirectoryObject CreateEmptyDirectoryWithSub()
        {
            var memoryDirectoryObject = CreateEmpty()
                .AddDirectory(SubDirectoryName);
            return memoryDirectoryObject;
        }
    }
}