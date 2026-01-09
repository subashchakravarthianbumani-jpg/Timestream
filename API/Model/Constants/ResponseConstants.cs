using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Constants
{
    public static class FTPFolderConstants
    {
        public const string ProfileImagesFolder = "USER_PROFILE_IMAGES";
    }
    public static class ResponseConstants
    {   
        public const string Success = "SUCCESS";
        public const string Failed = "FAILED";
        public const string Error = "ERROR";
    }
    public static class ResponceErrorCodes
    {
        public const string TCC_ConfigurationSaved = "TCCEC0000"; // Value Exist
        public const string TCC_ConfigurationValueExist = "TCCEC0001"; // Value Exist
        public const string TCC_ConfigurationCodeExist = "TCCEC0002"; // Code Exist
        public const string TCC_ConfigurationDependentRecordExist = "TCCEC0001"; // Dependent record exist

        public const string ROLE_ConfigurationSaved = "ROLEEC0000"; // Value Exist
        public const string ROLE_ConfigurationValueExist = "ROLEEC0001"; // Value Exist
        public const string ROLE_ConfigurationCodeExist = "ROLEEC0002"; // Code Exist
        public const string ROLE_ConfigurationDependentRecordExist = "ROLEEC0001"; // Dependent record exist

        public const string USER_ConfigurationSaved = "USEREC0000"; // Value Exist
        public const string USER_ConfigurationEmailExist = "USEREC0001"; // Value Exist
        public const string USER_ConfigurationMobileExist = "USEREC0002"; // Code Exist

        public const string QUICKLINK_ConfigurationSaved = "QUICKLINKEC0000"; // Value Exist
        public const string QUICKLINK_ConfigurationNameExist = "QUICKLINKEC0001"; // Value Exist
        public const string QUICKLINK_ConfigurationLinkExist = "QUICKLINKEC0002"; // Code Exist

        public const string TEMPLATE_ConfigurationSaved = "TEMPLATEEC0000"; // Value Exist
        public const string TEMPLATE_ConfigurationNameExist = "TEMPLATEEC0001"; // Value Exist
        public const string TEMPLATEMILESTONE_ConfigurationNameExist = "TEMPLATEMILESTONEEC0002"; // Value Exist
    }
    public static class RoleCodeConstants
    {
        public const string Contractor = "CONTRACTOR";
        public const string Admin = "ADM";
    }
    public static class ConfigurationCategory
    {
        public const string Division = "DIVISION";
        public const string Village = "VIL";
        public const string UserGroup = "USERGROUP";
        public const string Department = "DEPARTMENT";
        public const string WorkType = "WORKTYPE";
        public const string SubWorkType = "SUBWORKTYPE";
        public const string ServiceType = "SERVICETYPE";
        public const string CategoryType = "CATEGORYTYPE";
        public const string Taluk = "TLK";
        public const string District = "DIS";
        public const string Scheme = "SCHEME";
    }
}
