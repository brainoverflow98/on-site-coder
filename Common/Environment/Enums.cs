using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Environment
{
    public enum Role
    {
        RegularUser,
        ApprovedUser,
        PremiumUser,
        Admin,
        SuperAdmin
    }

    public enum ProgramingLanguage
    {
        CSharp,
        Java,
        Python
    }

    public enum ChallengePrivacyLevel
    {
        Public,
        ShareWithLink,
        ShareWithDomain
    }

    public enum FileType
    {
        TemplateFile,
        InputFile,
        HiddenInputFile
    }
}
