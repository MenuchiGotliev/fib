//fib bundle --output D:/ folder / bundlefile.txt;
using System.CommandLine;
using System.CommandLine.Invocation;
var bundleCommand = new Command("bundle", "Bundle code files to a single file");
var bundleOutputOption = new Option<FileInfo>("--output", "File path name");
var bundleLanguageOption = new Option<string>("--language", "language option") { IsRequired = true };
var bundleNoteOption = new Option<bool>("--note", "note option");
var bundleSortOption = new Option<bool>("--sort", "sort option");
var bundleRemoveOption = new Option<bool>("--remove-empty-lines", "remove-empty-lines option");
var bundleAuthorOption = new Option<string>("--author", "author option");

bundleOutputOption.AddAlias("-o");
bundleLanguageOption.AddAlias("-l");
bundleNoteOption.AddAlias("-n");
bundleSortOption.AddAlias("-s");
bundleRemoveOption.AddAlias("-r");
bundleAuthorOption.AddAlias("-a");


bundleCommand.AddOption(bundleLanguageOption);
bundleCommand.AddOption(bundleLanguageOption);
bundleCommand.AddOption(bundleNoteOption);
bundleCommand.AddOption(bundleSortOption);
bundleCommand.AddOption(bundleRemoveOption);
bundleCommand.AddOption(bundleAuthorOption);

static string[] AllEndings(string language, string[] allLanguages, string[] endings)
{
    if (language.Equals("all"))
        return endings;
    string[] languages = language.Split(' ');
    for (int i = 0; i < languages.Length; i++)
    {
        for (int j = 0; j < allLanguages.Length; j++)
        {
            if (languages[i].Equals(allLanguages[j]))
            {
                languages[i] = endings[j];
                break;
            }
        }
    }
    return languages;
}
string[] arrLanguage = { "c#", "c++", "c", "java", "python", "javaScript", "html", "css" };
string[] arrEndings = { ".cs", ".cpp", ".c", ".java", ".py", ".js", ".html", ".css" };



//bundleCommand.SetHandler((output, language, note, sort, remove, author) =>
//{
//    Console.WriteLine("h");
//    string[] endings = AllEndings(language, arrLanguage, arrEndings);
//    string path = Directory.GetCurrentDirectory();
//    List<string> Folders = Directory.GetFiles(path, "", SearchOption.AllDirectories).Where(file => !file.Contains("bin") && !file.Contains("Debug")).ToList();
//    string[] files = Folders.Where(f => endings.Contains(Path.GetExtension(f))).ToArray();
//    try
//    {

//        //// Get all files in the current directory that match the specified language
//        //var supportedLanguages = new List<string> { "cs", "java", "py", "cpp" }; // הוסף את השפות שתרצה
//        //var files = Directory.GetFiles(currentDirectory, $"*.*", SearchOption.TopDirectoryOnly)
//        //    .Select(file => new FileInfo(file))
//        //    .Where(file => !IsInBinOrDebugFolder(file) && (language.ToLower() == "all" || supportedLanguages.Contains(file.Extension.ToLower().TrimStart('.'))))
//        //    .ToArray();

//        // Create or overwrite the output file
//        using (var outputStream = new StreamWriter(output.FullName))
//        {
//            if (files.Length == 0)
//            {
//                Console.WriteLine($"No {language} files found in the directory.");
//                return;
//            }

//            // Sort files if required
//            if (sort)
//            {
//                files = files.OrderBy(file => file).ToArray();
//            }
//            // Write note if required
//            if (note)
//            {
//                foreach (var file in files)
//                {
//                    outputStream.WriteLine($"// Source: {file}");
//                }
//            }

//            // Write author if provided
//            if (!string.IsNullOrEmpty(author))
//            {
//                outputStream.WriteLine($"// Author: {author}");
//            }

//            // Write code content
//            foreach (var file in files)
//            {
//                var lines = File.ReadAllLines(file);

//                // Remove empty lines if required
//                if (remove)
//                {
//                    lines = lines.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();
//                }

//                foreach (var line in lines)
//                {
//                    outputStream.WriteLine(line);
//                }
//            }
//        }

//        Console.WriteLine($"Bundle created successfully at: {output.FullName}");
//    }
//    catch (DirectoryNotFoundException ex)
//	{

//		Console.WriteLine("Error: file path is invalid");
//	}

//}, bundleOutputOption, bundleLanguageOption, bundleNoteOption, bundleSortOption, bundleRemoveOption, bundleAuthorOption);
bundleCommand.SetHandler((output, language, note, sort, remove, author) =>
{
    string[] endings = AllEndings(language, arrLanguage, arrEndings);
    string path = Directory.GetCurrentDirectory();
    List<string> Folders = Directory.GetFiles(path, "", SearchOption.AllDirectories).Where(file => !file.Contains("bin") && !file.Contains("Debug")).ToList();
    string[] files = Folders.Where(f => endings.Contains(Path.GetExtension(f))).ToArray();

    try
    {
        if (files.Any())
        {
            using (var bundleFile = new StreamWriter(output.FullName, false))
            {
                if (!string.IsNullOrEmpty(author))
                    bundleFile.WriteLine("#Author: " + author);
                if (note)            // רשומת הערה עם מקור הקוד
                    bundleFile.WriteLine($"# Source code from: {path}\n");
                foreach (string file in files)
                {
                    if (note)
                        bundleFile.WriteLine($"#this Source code from: {file}\n");

                    var sourceCode = File.ReadAllLines(file);

                    if (remove)
                        sourceCode = sourceCode.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();
                    if (sort)
                        Array.Sort(files, (a, b) => Path.GetExtension(a).CompareTo(Path.GetExtension(b)));

                    else
                        Array.Sort(files);

                    foreach (var line in sourceCode)
                        bundleFile.WriteLine(line);

                    bundleFile.WriteLine();

                }
                Console.WriteLine($"Bundle created successfully at: {Directory.GetCurrentDirectory()}");
            }
        }

    }
    catch (DirectoryNotFoundException d)
    {
        Console.WriteLine("Error: file path invalid");
    }

}, bundleOutputOption, bundleLanguageOption, bundleNoteOption, bundleSortOption, bundleRemoveOption, bundleAuthorOption);
var rspCommand = new Command("create-rsp", "Create response file for bundle command");
rspCommand.SetHandler(() =>
{
    Console.Write("Enter language (type 'all' to include all languages): ");
    string language = Console.ReadLine();

    Console.Write("Enter output file path (press Enter for default location): ");
    string outputFilePath = Console.ReadLine();

    Console.Write("Include note? (true/false): ");
    bool includeNote = bool.Parse(Console.ReadLine());

    Console.Write("Sort files? (true/false): ");
    bool sortFiles = bool.Parse(Console.ReadLine());

    Console.Write("Remove empty lines? (true/false): ");
    bool removeEmptyLines = bool.Parse(Console.ReadLine());

    Console.Write("Enter author name (press Enter if none): ");
    string author = Console.ReadLine();

    using (var rspFile = new StreamWriter("fileName.rsp"))
    {
        rspFile.WriteLine($"--language {language}");
        rspFile.WriteLine($"--output {outputFilePath}");
        rspFile.WriteLine($"--note {includeNote}");
        rspFile.WriteLine($"--sort {sortFiles}");
        rspFile.WriteLine($"--remove-empty-lines {removeEmptyLines}");
        rspFile.WriteLine($"--author {author}");
    }

    Console.WriteLine("Response file created successfully: fileName.rsp");
});



var rootCommand = new RootCommand("RootCommand command for file bundler CLI");
rootCommand.AddCommand(bundleCommand);
rootCommand.AddCommand(rspCommand);
rootCommand.InvokeAsync(args);