// The MIT License (MIT)
// 
// Copyright (c) 2015-2018 Rasmus Mikkelsen
// Copyright (c) 2015-2018 eBay Software Foundation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 

#r "System.IO.Compression.FileSystem"
#r "System.Xml"

#tool "nuget:?package=NUnit.ConsoleRunner"
#tool "nuget:?package=OpenCover"

using System.IO.Compression;
using System.Net;
using System.Xml;

var VERSION = GetArgumentVersion();
var PROJECT_DIR = Context.Environment.WorkingDirectory.FullPath;
var CONFIGURATION = "Release";

// IMPORTANT DIRECTORIES
var DIR_OUTPUT_PACKAGES = System.IO.Path.Combine(PROJECT_DIR, "Build", "Packages");
var DIR_OUTPUT_REPORTS = System.IO.Path.Combine(PROJECT_DIR, "Build", "Reports");

var FILE_SOLUTION = System.IO.Path.Combine(PROJECT_DIR, "TrackInfoReader.sln");
var RELEASE_NOTES = ParseReleaseNotes(System.IO.Path.Combine(PROJECT_DIR, "RELEASE_NOTES.md"));

// =====================================================================================================
Task("Default")
    .IsDependentOn("Package");

// =====================================================================================================
Task("Clean")
    .Does(() =>
        {
            CleanDirectories(new []
                {
                    DIR_OUTPUT_PACKAGES,
                    DIR_OUTPUT_REPORTS,
                });
				
			CleanDirectories(GetDirectories("**/bin"));
			CleanDirectories(GetDirectories("**/obj"));
        });
	
// =====================================================================================================
Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
        {
			DotNetCoreRestore(
				".", 
				new DotNetCoreRestoreSettings()
				{
					ArgumentCustomization = aggs => aggs.Append(GetDotNetCoreArgsVersions())
				});
        });
		
// =====================================================================================================
Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
        {
            DotNetCoreBuild(
				".", 
				new DotNetCoreBuildSettings()
				{
					Configuration = CONFIGURATION,
					ArgumentCustomization = aggs => aggs
                        .Append(GetDotNetCoreArgsVersions())
                        .Append("/p:ci=true")
                        .Append("/p:SourceLinkEnabled=true")
                        .AppendSwitch("/p:DebugType","=","Full")
				});
        });

// =====================================================================================================
Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
        {
		    var projectFiles = GetFiles("./tests/**/*.csproj");
			foreach(var file in projectFiles)
			{
				DotNetCoreTest(file.FullPath);
			}

        })
	.Finally(() =>
        {
        });

// =====================================================================================================
Task("Package")
    .IsDependentOn("Test")
    .Does(() =>
        {
            Information("Version: {0}", RELEASE_NOTES.Version);
            Information(string.Join(Environment.NewLine, RELEASE_NOTES.Notes));

			foreach (var project in GetFiles("./**/*.csproj"))
			{
				var name = project.GetDirectory().FullPath;
				var version = VERSION.ToString();
				
				if (name.Contains("Tests") || name.Contains("examples") || name.Contains("/tools/"))
				{
					continue;
				}

                SetReleaseNotes(project.ToString());
							
				DotNetCorePack(
					name,
					new DotNetCorePackSettings
					{
						Configuration = CONFIGURATION,
						OutputDirectory = DIR_OUTPUT_PACKAGES,
						//NoBuild = true,
						ArgumentCustomization = aggs => aggs.Append(GetDotNetCoreArgsVersions())
					});
			}
        });

// =====================================================================================================
Task("All")
    .IsDependentOn("Package")
    .Does(() =>
        {

        });

// =====================================================================================================

Version GetArgumentVersion()
{
    return Version.Parse(EnvironmentVariable("APPVEYOR_BUILD_VERSION") ?? "0.0.1");
}

string GetDotNetCoreArgsVersions()
{
	var version = GetArgumentVersion().ToString();
	
	return string.Format(
		@"/p:Version={0} /p:AssemblyVersion={0} /p:FileVersion={0} /p:ProductVersion={0}",
		version);
}

void SetReleaseNotes(string filePath)
{
    var releaseNotes = string.Join(Environment.NewLine, RELEASE_NOTES.Notes);

    var xmlDocument = new XmlDocument();
    xmlDocument.Load(filePath);

    var node = xmlDocument.SelectSingleNode("Project/PropertyGroup/PackageReleaseNotes") as XmlElement;
    if (node == null)
    {
        throw new Exception(string.Format(
            "Project {0} does not have a `<PackageReleaseNotes>UPDATED BY BUILD</PackageReleaseNotes>` property",
            filePath));
    }

    if (!AppVeyor.IsRunningOnAppVeyor)
    {
        Information("Skipping update of release notes");
        return;
    } 
    else
    {
        Information(string.Format("Setting release notes in '{0}'", filePath));
        
        node.InnerText = releaseNotes;

        xmlDocument.Save(filePath);
    }
}

void UploadArtifact(string filePath)
{
    if (!FileExists(filePath))
    {
        Information("Skipping uploading of artifact, does not exist: {0}", filePath);
        return;
    }

    if (AppVeyor.IsRunningOnAppVeyor)
    {
        Information("Uploading artifact: {0}", filePath);

        AppVeyor.UploadArtifact(filePath);
    }
    else
    {
        Information("Not on AppVeyor, skipping artifact upload of: {0}", filePath);
    }
}

void UploadTestResults(string filePath)
{
    if (!FileExists(filePath))
    {
        Information("Skipping uploading of test results, does not exist: {0}", filePath);
        return;
    }

    if (AppVeyor.IsRunningOnAppVeyor)
    {
        Information("Uploading test results: {0}", filePath);

        try
        {
            using (var webClient = new WebClient())
            {
                webClient.UploadFile(
                    string.Format(
                        "https://ci.appveyor.com/api/testresults/nunit3/{0}",
                        Environment.GetEnvironmentVariable("APPVEYOR_JOB_ID")),
                    filePath);
            }
        }
        catch (Exception e)
        {
            Error(
                "Failed to upload '{0}' due to {1} - {2}: {3}",
                filePath,
                e.Message,
                e.GetType().Name,
                e.ToString());
        }
        
        /*
        // This should work, but doesn't seem to
        AppVeyor.UploadTestResults(
            filePath,
            AppVeyorTestResultsType.NUnit3);
        */
    }    
    else
    {
        Information("Not on AppVeyor, skipping test result upload of: {0}", filePath);
    }
}

string ExecuteCommand(string exePath, string arguments = null, string workingDirectory = null)
{
    Information("Executing '{0}' {1}", exePath, arguments ?? string.Empty);

    using (var process = new System.Diagnostics.Process())
    {
        process.StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                FileName = exePath,
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
            };
        process.Start();

        var output = process.StandardOutput.ReadToEnd();

        if (!process.WaitForExit(30000))
        {
            throw new Exception("Failed to stop process!");
        }

        Debug(output);

        if (process.ExitCode != 0)
        {
            throw new Exception(string.Format("Error code {0} was returned", process.ExitCode));
        }

        return output;
    }
}

RunTarget(Argument<string>("target", "Package"));
