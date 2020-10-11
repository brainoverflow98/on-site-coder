using Common.DataBase.Entities;
using Common.Environment;
using ExecutionService.Exceptions;
using ExecutionService.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ExecutionService.Services
{
    public class WindowsExecutionService
    {
        private const string 
            _root = @"C:\Projects\Graduation Project\ExecutionEnvironment\",            
            _mainFileName = "Main",
            _environmentSettings = "EnvironmentSettings.txt",
            _testCases = "TestCases.txt",
            _spliter = "!+%";
        private double compileTimeLimit = 5.0, executeTimeLimit = 5.0;

        private readonly char _ds = Path.DirectorySeparatorChar;

        //C:\Projects\Graduation Project\ExecutionEnvironment\userId  %cd%  $(pwd)

        // docker run -i -t -d --name 5e1b621b1d508b4d1ca8913f -v "C:\Projects\Graduation Project\ExecutionEnvironment\5e1b621b1d508b4d1ca8913f:/home" -w /home mono; docker exec executer csc Main.cs; docker exec executer mono Main.exe test1.txt > hello.txt; docker rm -f executer;

        // powershell -NoExit -Command "docker run -i -t -d --name executer -v \"$(pwd):/home\" -w /home mono;"

        public void CreateEnvironment(string userId, ProgramingLanguage programingL, IEnumerable<SolutionFile> inputFiles, IEnumerable<TestCase> testCases)
        {            
            Directory.CreateDirectory(_root + _ds + userId);

            foreach (var file in inputFiles)
            {
                using (StreamWriter writetext = new StreamWriter(_root + userId + _ds + file.Name))
                {
                    writetext.Write(file.Content);
                }
            }

            using (StreamWriter writetext = new StreamWriter(_root + userId + _ds + _testCases))
            {
                foreach (var testCase in testCases)
                {
                    writetext.Write(testCase.No + _spliter + testCase.Arguments + _spliter + testCase.ExpectedOutput + _spliter + testCase.IsHidden + _spliter);
                }
            }

            using (StreamWriter writetext = new StreamWriter(_root + userId + _ds + _environmentSettings))
            {
                writetext.Write(programingL.ToString());
            }            
        }
        public void SaveFile(string userId, string fileName, string fileContent)
        {
            var filePath = _root + userId + _ds + fileName;
            if (File.Exists(filePath)) 
                using (StreamWriter writetext = new StreamWriter(filePath))
                {
                    writetext.Write(fileContent);
                }
            else
                throw new ExecutionServiceException("File with the given name does not exist.");
        }
        public void DeleteEnvironment(string userId)
        {
            if (Directory.Exists(_root + userId)) 
                Directory.Delete(_root + userId, true);
            else
                throw new ExecutionServiceException("Environment could not be found");
        }
        public void DeleteFile(string userId, string fileName)
        {
            if (File.Exists(_root + userId + _ds + fileName)) 
                File.Delete(_root + userId + _ds + fileName);
            else
                throw new ExecutionServiceException("File with the given name does not exist.");
        }

        public void CreateFile(string userId, string fileName)
        {
            if (!File.Exists(_root + _ds + userId + _ds + fileName)) 
                File.Create(_root + _ds + userId + _ds + fileName).Close();
            else
                throw new ExecutionServiceException("File with the same name already exists.");            
        }

        public ExecutionResult CompileAndExecute(string userId)
        {
            string environmentPath = _root + userId + _ds;           
            
            var progLang = GetProgramingLanguage(environmentPath);

            string languageCompiler = null, sourceFileExtension = null, languageRuntime, runableFileExtension;
            bool interpretedLanguage = false;
            if (progLang == ProgramingLanguage.CSharp)
            {
                languageCompiler = "csc";                
                sourceFileExtension = ".cs";
                languageRuntime = "";
                runableFileExtension = ".exe";
            }
            else if (progLang == ProgramingLanguage.Java)
            {
                languageCompiler = "javac";                
                sourceFileExtension = ".java";
                languageRuntime = "java";
                runableFileExtension = "";
            }
            else if (progLang == ProgramingLanguage.Python)
            {
                interpretedLanguage = true;
                languageRuntime = "python";
                runableFileExtension = ".py";
            }
            else
            {
                throw new ExecutionServiceException("Unsupported programing language.");
            }

            if(!interpretedLanguage)
                CompileCode(environmentPath, languageCompiler, sourceFileExtension);

            var testCases = GetTestCases(environmentPath);

            foreach (var testCase in testCases)
            {
                ExecuteTestCase(testCase, environmentPath, languageRuntime, runableFileExtension);
            }

            var result = new ExecutionResult
            {
                FailedTestCount = testCases.Count(t => !t.IsSuccessful),
                SuccessfulTestCount = testCases.Count(t => t.IsSuccessful),
                TestCases = testCases.Where(t => !t.IsHidden)
                .Select(t => new ExecutionTestResult { No = t.No, IsSuccessful = t.IsSuccessful, UserOutput = t.UserOutput})
            };

            Console.WriteLine("********************************************************");
            Console.WriteLine("ID : " + userId + " / Score : " + result.SuccessfulTestCount);

            foreach (var testCase in testCases)
            {
                Console.WriteLine("TestCase No : " + testCase.No + " / Argument : " + testCase.Arguments + " / Expected : " + testCase.ExpectedOutput + " / UserOutput : " + testCase.UserOutput + " / isHidden : " + testCase.IsHidden);

            }
            Console.WriteLine("********************************************************");

            return result;

            #region LOCAL FUNCTION DECLERATIONS           
            
            ProgramingLanguage GetProgramingLanguage(string environmentPath)
            {
                if (!File.Exists(environmentPath + _ds + _environmentSettings))
                {
                    throw new ExecutionServiceException("EnvironmentSettings file does not exist.");
                }
                string progLang = File.ReadAllText(environmentPath + _environmentSettings, Encoding.UTF8);

                ProgramingLanguage result;

                if(!Enum.TryParse(progLang, out result))
                    throw new ExecutionServiceException("Undefined programing language.");

                return result;
            }

            void CompileCode(string environmentPath, string languageCompiler, string sourceFileExtension)
            {
                var processInfo = new ProcessStartInfo("cmd", "/c cd " + environmentPath + " & " + languageCompiler + " " + _mainFileName + sourceFileExtension)
                {
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var process = new Process
                {
                    StartInfo = processInfo
                };

                process.Start();

                DateTime timer = DateTime.Now.AddSeconds(compileTimeLimit);    // 5 saniye çalışma süresi atanıyor.

                while (!process.WaitForExit(200))
                {
                    // calismasi için ayrilan sureyi kontrol ediyor.
                    if (timer < DateTime.Now)
                    {
                        process.Kill();
                        throw new ExecutionServiceException("Code took too long to compile.");
                    }
                }

                string error = process.StandardError.ReadToEnd();                
                if (!string.IsNullOrEmpty(error))
                {
                    throw new ExecutionServiceException("Code didn't compile.");
                }

                string output = process.StandardOutput.ReadToEnd();
                if (output.Contains("error"))
                {
                    throw new ExecutionServiceException(output);
                }
            }

            IEnumerable<ExecutionTestCase> GetTestCases(string environmentPath)
            {
                var testCases = new List<ExecutionTestCase>();

                string[] ExecutionTestCases = null;
                if (File.Exists(environmentPath + _ds + _testCases))
                {
                    ExecutionTestCases = File.ReadAllText(environmentPath + _testCases, Encoding.UTF8).Split(new string[] { _spliter }, StringSplitOptions.None);
                }

                for (int i = 0; i < ExecutionTestCases.Length - 1; i += 4)
                {
                    testCases.Add(new ExecutionTestCase
                    {
                        No = int.Parse(ExecutionTestCases[i]),
                        Arguments = ExecutionTestCases[i + 1],
                        ExpectedOutput = ExecutionTestCases[i + 2],
                        IsHidden = bool.Parse(ExecutionTestCases[i + 3]),
                        UserOutput = null
                    });
                }

                return testCases;
            }

            void ExecuteTestCase(ExecutionTestCase testCase, string environmentPath, string languageRuntime, string runableFileExtension)
            {
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                var runCommand = languageRuntime + " " + _mainFileName + runableFileExtension;
                var processInfo = new ProcessStartInfo("cmd", "/c " + "cd " + environmentPath + " & " + runCommand + " " + testCase.Arguments);

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                processInfo.RedirectStandardOutput = true;
                processInfo.RedirectStandardError = true;
                processInfo.UseShellExecute = false;
                // Do not create the black window.
                processInfo.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                Process proc = new Process();
                proc.StartInfo = processInfo;
                proc.Start();

                DateTime timer = DateTime.Now.AddSeconds(executeTimeLimit);    // 5 saniye çalışma süresi atanıyor.

                while (!proc.WaitForExit(250))
                {
                    // calismasi için ayrilan sureyi kontrol ediyor.
                    if (timer < DateTime.Now)
                    {
                        proc.Kill();
                        throw new ExecutionServiceException("Code took too long to execute.");
                        //return kod 5 saniyeden uzun sürdü. *****
                    }
                }

                // Get the output into a string
                string consoleOutput = proc.StandardOutput.ReadToEnd();
                //Console.WriteLine(consoleOutput);
                string error = proc.StandardError.ReadToEnd();

                consoleOutput = consoleOutput.Replace("\r", "");

                if (!string.IsNullOrEmpty(error))
                {
                    testCase.UserOutput = error;
                }
                else
                {
                    testCase.UserOutput = consoleOutput;
                    if (testCase.ExpectedOutput.Equals(consoleOutput))
                        testCase.IsSuccessful = true;
                }

            }

            #endregion
        }
    }
}