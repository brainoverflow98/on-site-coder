using Common.DataBase.Entities;
using Common.Environment;
using Common.Helpers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace Common.DataBase
{
    public class DataSeeder : IDataSeeder
    {
        public void Seed(IDbContext db)
        {
            var users = new List<User>();

            users.Add( new User { 
                Email = "ayvaz@gmail.com",
                DisplayName = "Furkan Ayvaz",
                Role = Role.SuperAdmin,
                PasswordHash = PasswordHasher.HashPassword("12345678"),
                IsEmailConfirmed = true,
                CreationDate = DateTime.UtcNow,
            });

            users.Add(new User
            {
                Email = "b21627711@cs.hacettepe.edu.tr",
                DisplayName = "Halil Etka Tutkun",
                Role = Role.RegularUser,
                PasswordHash = PasswordHasher.HashPassword("12345678"),
                IsEmailConfirmed = true,
                CreationDate = DateTime.UtcNow,
            });

            db.Users.InsertMany(users);


            var challenges = new List<Challenge>();

            challenges.Add(new Challenge 
            { 
                OwnerId = users[0].Id,
                OwnerDisplayName = users[0].DisplayName,
                CreationDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(30),
                PrivacyLevel = ChallengePrivacyLevel.Public,
                Name = "Factorial",
                ProgramingLanguage = ProgramingLanguage.CSharp,
                ProblemDefinition = "Write a program that takes the factorial of the given input.",
                TestCaseCount = 3,
                Files = new List<ChallengeFile> { },
                TestCases = new List<TestCase> 
                { 
                    new TestCase
                    {
                        No = 1,
                        IsHidden = false,
                        Arguments = "4",
                        ExpectedOutput = "24"
                    },

                    new TestCase
                    {
                        No = 2,
                        IsHidden = true,
                        Arguments = "5",
                        ExpectedOutput = "120"
                    },

                    new TestCase
                    {
                        No = 3,
                        IsHidden = true,
                        Arguments = "6",
                        ExpectedOutput = "720"
                    }
                }                
            }); //C# factorial

            challenges.Add(new Challenge
            {
                OwnerId = users[0].Id,
                OwnerDisplayName = users[0].DisplayName,
                CreationDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(15),
                PrivacyLevel = ChallengePrivacyLevel.Public,
                Name = "Check Range",
                ProgramingLanguage = ProgramingLanguage.CSharp,
                ProblemDefinition = "Write a C# Sharp program to check two given integers whether either of them is in the range 100..200 inclusive.",
                TestCaseCount = 3,
                Files = new List<ChallengeFile> { },
                TestCases = new List<TestCase>
                {
                    new TestCase
                    {
                        No = 1,
                        IsHidden = false,
                        Arguments = "100 199",
                        ExpectedOutput = "True"
                    },

                    new TestCase
                    {
                        No = 2,
                        IsHidden = true,
                        Arguments = "250 300",
                        ExpectedOutput = "False"
                    },

                    new TestCase
                    {
                        No = 3,
                        IsHidden = true,
                        Arguments = "105 190",
                        ExpectedOutput = "True"
                    }
                }
            }); //C# check range - sleep 10 seconds

            challenges.Add(new Challenge
            {
                OwnerId = users[0].Id,
                OwnerDisplayName = users[0].DisplayName,
                CreationDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(10),
                PrivacyLevel = ChallengePrivacyLevel.Public,
                Name = "Count Lines in File",
                ProgramingLanguage = ProgramingLanguage.CSharp,
                ProblemDefinition = "Write a program in C# Sharp to count the number of lines in a file.",
                TestCaseCount = 3,
                TestCases = new List<TestCase>
                {
                    new TestCase
                    {
                        No = 1,
                        IsHidden = false,
                        Arguments = "test1.txt",
                        ExpectedOutput = "4"
                    },

                    new TestCase
                    {
                        No = 2,
                        IsHidden = true,
                        Arguments = "hidden_test1.txt",
                        ExpectedOutput = "5"
                    },

                    new TestCase
                    {
                        No = 3,
                        IsHidden = true,
                        Arguments = "hidden_test2.txt",
                        ExpectedOutput = "6"
                    }
                },
                Files = new List<ChallengeFile>
                { 
                    new ChallengeFile
                    { 
                        FileType = FileType.InputFile,
                        Name = "test1.txt",
                        Content = "line1\nline2\nline3\nline4"
                    },

                    new ChallengeFile
                    {
                        FileType = FileType.HiddenInputFile,
                        Name = "hidden_test1.txt",
                        Content = "line1\nline2\nline3\nline4\nline5"
                    },

                    new ChallengeFile
                    {
                        FileType = FileType.HiddenInputFile,
                        Name = "hidden_test2.txt",
                        Content = "line1\nline2\nline3\nline4\nline5\nline6"
                    }
                }
            }); //C# read file

            challenges.Add(new Challenge
            {
                OwnerId = users[0].Id,
                OwnerDisplayName = users[0].DisplayName,
                CreationDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(12),
                PrivacyLevel = ChallengePrivacyLevel.Public,
                Name = "Print Lines",
                ProgramingLanguage = ProgramingLanguage.Java,
                ProblemDefinition = "Write a java program that reads a file line by line and prints it to console.",
                TestCaseCount = 3,
                TestCases = new List<TestCase>
                {
                    new TestCase
                    {
                        No = 1,
                        IsHidden = false,
                        Arguments = "test1.txt",
                        ExpectedOutput = "line1\nline2\nline3\nline4\n"
                    },

                    new TestCase
                    {
                        No = 2,
                        IsHidden = true,
                        Arguments = "hidden_test1.txt",
                        ExpectedOutput = "line1\nline2\nline3\nline4\nline5\n"
                    },

                    new TestCase
                    {
                        No = 3,
                        IsHidden = true,
                        Arguments = "hidden_test2.txt",
                        ExpectedOutput = "line1\nline2\nline3\nline4\nline5\nline6\n"
                    }
                },
                Files = new List<ChallengeFile>
                {
                    new ChallengeFile
                    {
                        FileType = FileType.InputFile,
                        Name = "test1.txt",
                        Content = "line1\nline2\nline3\nline4"
                    },

                    new ChallengeFile
                    {
                        FileType = FileType.HiddenInputFile,
                        Name = "hidden_test1.txt",
                        Content = "line1\nline2\nline3\nline4\nline5"
                    },

                    new ChallengeFile
                    {
                        FileType = FileType.HiddenInputFile,
                        Name = "hidden_test2.txt",
                        Content = "line1\nline2\nline3\nline4\nline5\nline6"
                    }
                }
            }); //Java read file

            challenges.Add(new Challenge
            {
                OwnerId = users[0].Id,
                OwnerDisplayName = users[0].DisplayName,
                CreationDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(10),
                PrivacyLevel = ChallengePrivacyLevel.Public,
                Name = "N+NN+NNN",
                ProgramingLanguage = ProgramingLanguage.Python,
                ProblemDefinition = "Write a Python program that accepts an integer (n) and computes the value of n+nn+nnn.",
                TestCaseCount = 3,
                Files = new List<ChallengeFile> { },
                TestCases = new List<TestCase>
                {
                    new TestCase
                    {
                        No = 1,
                        IsHidden = false,
                        Arguments = "5",
                        ExpectedOutput = "615"
                    },

                    new TestCase
                    {
                        No = 2,
                        IsHidden = true,
                        Arguments = "6",
                        ExpectedOutput = "738"
                    },

                    new TestCase
                    {
                        No = 3,
                        IsHidden = true,
                        Arguments = "7",
                        ExpectedOutput = "861"
                    }
                }
            }); //Pyhton n+nn+nnn

            challenges.Add(new Challenge
            {
                OwnerId = users[0].Id,
                OwnerDisplayName = users[0].DisplayName,
                CreationDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(7),
                PrivacyLevel = ChallengePrivacyLevel.Public,
                Name = "Range Distance",
                ProgramingLanguage = ProgramingLanguage.Python,
                ProblemDefinition = "Write a Python program to test whether a number is within 100 of 1000 or 2000.",
                TestCaseCount = 3,
                Files = new List<ChallengeFile> { },
                TestCases = new List<TestCase>
                {
                    new TestCase
                    {
                        No = 1,
                        IsHidden = false,
                        Arguments = "900",
                        ExpectedOutput = "True"
                    },

                    new TestCase
                    {
                        No = 2,
                        IsHidden = true,
                        Arguments = "800",
                        ExpectedOutput = "False"
                    },

                    new TestCase
                    {
                        No = 3,
                        IsHidden = true,
                        Arguments = "2200",
                        ExpectedOutput = "False"
                    }
                }
            }); //Pyhton 100 of 1000 or 2000

            challenges.Add(new Challenge
            {
                OwnerId = users[0].Id,
                OwnerDisplayName = users[0].DisplayName,
                CreationDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(-1),
                PrivacyLevel = ChallengePrivacyLevel.Public,
                Name = "Calculator",
                ProgramingLanguage = ProgramingLanguage.Java,
                ProblemDefinition = "Write a Java program that takes the factorial of the given input.",
                TestCaseCount = 3,
                Files = new List<ChallengeFile> { },
                TestCases = new List<TestCase>
                {
                    new TestCase
                    {
                        No = 1,
                        IsHidden = false,
                        Arguments = "4",
                        ExpectedOutput = "24"
                    },

                    new TestCase
                    {
                        No = 2,
                        IsHidden = true,
                        Arguments = "5",
                        ExpectedOutput = "120"
                    },

                    new TestCase
                    {
                        No = 3,
                        IsHidden = true,
                        Arguments = "6",
                        ExpectedOutput = "720"
                    }
                }
            }); //Java calculator(factorial) - for due test

            challenges.Add(new Challenge
            {
                OwnerId = users[1].Id,
                OwnerDisplayName = users[1].DisplayName,
                CreationDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(14),
                PrivacyLevel = ChallengePrivacyLevel.ShareWithDomain,
                PrivacyDomain = "cs.hacettepe.edu.tr",
                Name = "Range 100-200",
                ProgramingLanguage = ProgramingLanguage.Java,
                ProblemDefinition = "Write a Java program to check two given integers whether either of them is in the range 100..200 inclusive.",
                TestCaseCount = 3,
                Files = new List<ChallengeFile> { },
                TestCases = new List<TestCase>
                {
                    new TestCase
                    {
                        No = 1,
                        IsHidden = false,
                        Arguments = "100 199",
                        ExpectedOutput = "True"
                    },

                    new TestCase
                    {
                        No = 2,
                        IsHidden = true,
                        Arguments = "250 300",
                        ExpectedOutput = "False"
                    },

                    new TestCase
                    {
                        No = 3,
                        IsHidden = true,
                        Arguments = "105 190",
                        ExpectedOutput = "True"
                    }
                }
            }); //Java check range - for privacy domain test

            db.Challenges.InsertMany(challenges);

            
            var solutions = new List<Solution>();

            solutions.Add(new Solution 
            { 
                OwnerId = users[1].Id,
                OwnerDisplayName = users[1].DisplayName,
                ChallengeId = challenges[0].Id,
                CreationDate = DateTime.UtcNow,
                ProgramingLanguage = challenges[0].ProgramingLanguage,
                TestCaseCount = challenges[0].TestCaseCount,
                Files = new List<SolutionFile>
                { 
                    new SolutionFile 
                    { 
                        Name = "Main.cs",
                        Content = "using System;\n\nnamespace WebApp\n{\n    public class Program\n    {\n        public static int Factorial(int n) {\n         int res = 1;\n         while (n != 1) {\n            res = res * n;\n            n = n - 1;\n         }\n         return res;\n      }\n        \n        public static void Main(string[] args)\n        {\n            var input = Int32.Parse(args[0]);\n            Console.Write(Factorial(input));\n        }\n    }\n}"
                    }
                }
            }); //C# factorial

            solutions.Add(new Solution
            {
                OwnerId = users[1].Id,
                OwnerDisplayName = users[1].DisplayName,
                ChallengeId = challenges[1].Id,
                CreationDate = DateTime.UtcNow,
                ProgramingLanguage = challenges[1].ProgramingLanguage,
                TestCaseCount = challenges[1].TestCaseCount,
                Files = new List<SolutionFile>
                {
                    new SolutionFile
                    {
                        Name = "Main.cs",
                        Content =
@"using System;
using System.Threading;

class Example
{
    static void Main()
    {
        for (int i = 0; i < 5; i++)
        {
            Thread.Sleep(10000);
        }
    }
}"
//@"
//using System;

//namespace exercises
//{
// class Program
//    {
//        static void Main(string[] args)
//        {
//            Console.Write( CheckRange(int.Parse(args[0]), int.Parse(args[1]) ) );
//        }
//        public static bool CheckRange(int x, int y)
//        {
//            return (x >= 100 && x <= 200) || (y >= 100 && y <= 200);
//        }
//    }
//}"
                    }
                }
            }); //C# check range - sleep 10 seconds

            solutions.Add(new Solution
            {
                OwnerId = users[1].Id,
                OwnerDisplayName = users[1].DisplayName,
                ChallengeId = challenges[2].Id,
                CreationDate = DateTime.UtcNow,
                ProgramingLanguage = challenges[2].ProgramingLanguage,
                TestCaseCount = challenges[2].TestCaseCount,
                Files = new List<SolutionFile>
                {
                    new SolutionFile
                    {
                        Name = "Main.cs",
                        Content = 
@"
using System;
using System.IO;
using System.Text;

class filexercise3
{
    public static void Main(string[] args)
    {
        string fileName = args[0]; 
         int count;
        try
        {
            using (StreamReader sr = File.OpenText(fileName))
            {
                string s = "" "";
                count=0;
                while ((s = sr.ReadLine()) != null)
                {
                    count++;
                }
            }
            Console.Write(count);
        }
        catch (Exception MyExcep)
        {
            Console.WriteLine(MyExcep.ToString());
        }
    }
}"
                    }
                }
            }); //C# read file

            solutions.Add(new Solution
            {
                OwnerId = users[1].Id,
                OwnerDisplayName = users[1].DisplayName,
                ChallengeId = challenges[3].Id,
                CreationDate = DateTime.UtcNow,
                ProgramingLanguage = challenges[3].ProgramingLanguage,
                TestCaseCount = challenges[3].TestCaseCount,
                Files = new List<SolutionFile>
                {
                    new SolutionFile
                    {
                        Name = "Main.java",
                        Content =
@"
import java.io.BufferedReader;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.FileReader;
 
public class Main {
 
    public static void main(String args[]){
        try {
             BufferedReader br = new BufferedReader(new FileReader(args[0]));
             String strLine = br.readLine();
			 while (strLine != null)
             {
                System.out.println(strLine);
                strLine = br.readLine();                
             }              
             br.close();
        } catch (FileNotFoundException e) {
            System.err.println(""File not found"");
        } catch (IOException e)
            {
                System.err.println(""Unable to read the file."");
            }
        }
    }"
                    }
                }
            }); //Java read file

            solutions.Add(new Solution
            {
                OwnerId = users[1].Id,
                OwnerDisplayName = users[1].DisplayName,
                ChallengeId = challenges[4].Id,
                CreationDate = DateTime.UtcNow,
                ProgramingLanguage = challenges[4].ProgramingLanguage,
                TestCaseCount = challenges[4].TestCaseCount,
                Files = new List<SolutionFile>
                {
                    new SolutionFile
                    {
                        Name = "Main.py",
                        Content =
@"import sys

a = int(sys.argv[1])
n1 = int( ""%s"" % a )
n2 = int( ""%s%s"" % (a,a) )
n3 = int( ""%s%s%s"" % (a,a,a) )
print (n1+n2+n3, end = '')"
                    }
                }
            }); //Pyhton n+nn+nnn

            solutions.Add(new Solution
            {
                OwnerId = users[1].Id,
                OwnerDisplayName = users[1].DisplayName,
                ChallengeId = challenges[5].Id,
                CreationDate = DateTime.UtcNow,
                ProgramingLanguage = challenges[5].ProgramingLanguage,
                TestCaseCount = challenges[5].TestCaseCount,
                Files = new List<SolutionFile>
                {
                    new SolutionFile
                    {
                        Name = "Main.py",
                        Content =
@"import sys

a = int(sys.argv[1])

def near_thousand(n):
      return ((abs(1000 - n) <= 100) or (abs(2000 - n) <= 100))
print(near_thousand(a), end = '')"
                    }
                }
            }); //Pyhton 100 of 1000 or 2000

            db.Solutions.InsertMany(solutions);

        }
    }
}
