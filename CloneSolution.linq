<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.dll</Reference>
  <Namespace>System.Net</Namespace>
</Query>

// the base of the template folder
//string fromFolder = @"C:\Dev\Projects\Home\lazycowprojects";
string fromFolder = @"C:\Dev\Projects\Home\lazycowprojects\Templates\";
// the template name
string solutionName = @"";

// the base of the output folder
string toFolder = @"C:\Dev\Projects\Home\";
// the new project name
string newSolutionName = @"";
Tuple<string,Func<string>>[] regexReplace;
void Main()
{
	
	while (string.IsNullOrEmpty(solutionName)) {
		try {
			var dirs = Directory.GetDirectories(fromFolder);
			string.Format("Select one:").Dump();
			for (int i = 0; i < dirs.Length; i++)	
			{
				string.Format("{0} - {1}",(i+1),Path.GetFileName(dirs[i])).Dump();
			}
			var line = int.Parse(Util.ReadLine("Select the template"));
			solutionName = Path.GetFileName(dirs[line-1]);
		}
		catch(Exception e) {
			e.Message.Dump();
		}
	
		
	}
	
	if (string.IsNullOrEmpty(newSolutionName)) {
		newSolutionName = Util.ReadLine("New solution name");
	}
	
	regexReplace = new Tuple<string,Func<string>>[] {
		new Tuple<string,Func<string>>("__NAME__",() => newSolutionName),	
		new Tuple<string,Func<string>>(solutionName,() => newSolutionName),	
		new Tuple<string,Func<string>>("assembly\\: Guid\\(\\\"[a-z0-9\\-]+\\\"\\)",() => string.Format("assembly: Guid(\"{0}\")",Guid.NewGuid())),
		new Tuple<string,Func<string>>("ProductID=\\\"\\{[a-z0-9\\-]+\\}\\\"",() => string.Format("ProductID=\"{{{0}}}\" ",Guid.NewGuid())),
		new Tuple<string,Func<string>>("AppId = \\\"[a-z0-9\\-]+\\\"",() => string.Format("AppId = \"{0}\"",Guid.NewGuid())),
		new Tuple<string,Func<string>>(@"\<ProjectGuid\>\{[A-z0-9\\-]+\}\<\/ProjectGuid\>",() => string.Format(@"<ProjectGuid>{{{0}}}</ProjectGuid>",Guid.NewGuid())),
	};
	
	
	var mainPath = new DirectoryInfo(Path.Combine(fromFolder,solutionName));
	//if (Directory.Exists(Path.Combine(toFolder,newSolutionName))) Directory.Delete(Path.Combine(toFolder,newSolutionName),true);
	CopyFolder(mainPath);
	Util.Cmd(@"c:\windows\explorer.exe",ReplaceFileInfo(mainPath.ToString()).ToString(),true);
	
}
void CopyFolder(DirectoryInfo fromDir) {
	foreach (var p in fromDir.GetDirectories()) {
		if (IsIgnored(p.FullName)) continue;
		var toDir = new DirectoryInfo(ReplaceFileInfo(p.FullName));
		if (!toDir.Exists) toDir.Create();
		CopyFolder(p);
	}
	foreach (var fromFile in fromDir.GetFiles()) {
		if (IsIgnored(fromFile.FullName)) continue;
		var toFile = new FileInfo(ReplaceFileInfo(fromFile.FullName));
		CopyFile(fromFile,toFile);
	}
} 

void CopyFile(FileInfo fromFile,FileInfo toFile ) {
	
	//straight bin copy
	if (!IsContentReplaceAble(fromFile.Name)) {
		if (toFile.Exists && toFile.Length == fromFile.Length) 
		{
			string.Format("ski {0}",fromFile.FullName).Dump();
		}
		else {
			if (toFile.Exists) toFile.Delete();
			string.Format("bin {0}",toFile.FullName).Dump();
			using (var newFile = toFile.Create()) {
				using (var stream = fromFile.OpenRead()) {
					stream.CopyTo(newFile);
					newFile.Flush();
				}
			}
		}
	}
	else {
		string.Format("txt {0}",toFile.FullName).Dump();
		using (var newFile = toFile.Create()) {
			using (var stream = fromFile.OpenRead()) {
				var sr = new StreamReader(stream);
				var sw = new StreamWriter(newFile);
				var inputText = sr.ReadToEnd();
				inputText = ReplaceFileContents(inputText);
				sw.Write(inputText);
				sw.Flush();
			}
		}
	}
}

string ReplaceFileInfo(string oldLocation) {
	var newLocation = oldLocation.Replace(Path.Combine(fromFolder,solutionName),Path.Combine(toFolder,newSolutionName));
	foreach (var replace in regexReplace) {
		newLocation = Regex.Replace(newLocation,replace.Item1,replace.Item2());
	}	
	return newLocation;
}

string ReplaceFileContents(string inputText) {
	
	foreach (var replace in regexReplace) {
		inputText = Regex.Replace(inputText,replace.Item1,replace.Item2());
	}	
	return inputText;
}

bool IsIgnored(string path) {
	return ignore.Where(x=> path.Contains(x)).Any();
}

bool IsContentReplaceAble(string path) {
	return replaceContent.Where(x=> path.EndsWith(x,StringComparison.InvariantCultureIgnoreCase)).Any();
}

string[] ignore = new string[] {
	@"\build",
	@"\bin",
	@"\Bin",
	@"\obj",
	@"\_ReSharper.",
	@"\_UpgradeReport_Files",
	@"\Backup",
	@".cache",
	@".user",
	@".suo",
	@"UpgradeLog.XML",
	@".hg",
};

string[] replaceContent = new string[] {
	@".bat",
	@".build",
	@".sln",
	@".cs",
	@".config",
	@".xml",
	@".xaml",
	@".aspx",
	@".csproj",
	@".asax",
	@"Site.Master",
	
};


// Define other methods and classes here