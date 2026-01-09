using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Constants
{
    public static class AlertConfigConstants
    {
        // Type
        public const string Type_Schedule = "Schedule";
        public const string Type_Cost = "Cost";
        public const string Type_Approval = "Approval";

        public static List<string> Get_Type()
        {
            return new List<string>() { Type_Schedule, Type_Cost, Type_Approval };
        }

        // Object
        public const string Object_Work = "Work";
        public const string Object_Milestone = "Milestone";
        public const string Object_Mbook = "Mbook";

        public static List<string> Get_Object()
        {
            return new List<string>() { Object_Work, Object_Milestone, Object_Mbook };
        }

        // Severity
        public const string Severity_Red = "Red";
        public const string Severity_Yellow = "Yellow";

        public static List<string> Get_Severity()
        {
            return new List<string>() { Severity_Red, Severity_Yellow };
        }

        // Field
        public const string Field_WorkStartDate = "WorkStartDate"; // Work Start date
        public const string Field_WorkEndDate = "WorkEndDate"; // Work end date
        public const string Field_WorkTotalCost = "WorkTotalCost"; // Work Total Cost

        public const string Field_MilestoneStartDate = "MilestoneStartDate"; // Milestone Start Date
        public const string Field_MilestoneEndDate = "MilestoneEndDate"; // Milestone End Date
        public const string Field_MilestoneProgress = "MilestoneProgress"; // Milestone Progress

        public const string Field_ActiveMbookSubmittedDate = "ActiveMbookSubmittedDate"; // Mbook(Active) Submitted date

        public static List<string> Get_Field()
        {
            return new List<string>() { Field_WorkStartDate, Field_WorkEndDate, Field_WorkTotalCost, Field_MilestoneStartDate, 
                Field_MilestoneEndDate, Field_MilestoneProgress, Field_ActiveMbookSubmittedDate };
        }

        // Base Field
        public const string BaseField_CurrentDate = "CurrentDate"; // Current Date
        public const string BaseField_CurrentDateOrActualCompletionDate = "CurrentDateOrActualCompletionDate"; // Current Date or actual Completion Date
        public const string BaseField_CurrentDateAndMilestoneEndDate = "CurrentDateAndMilestoneEndDate"; // Current date and Milestone End Date
        public const string BaseField_CurrentDateAndMbookSubmittedDate = "CurrentDateAndMbookSubmittedDate"; // Current Date or Mbook Submitted Date
        public const string BaseField_WorkCost = "WorkCost"; // Work Cost

        public static List<string> Get_BaseField()
        {
            return new List<string>() { BaseField_CurrentDate, BaseField_CurrentDateOrActualCompletionDate, 
                BaseField_CurrentDateAndMilestoneEndDate, BaseField_CurrentDateAndMbookSubmittedDate, BaseField_WorkCost };
        }

        // Calculation Type
        public const string CalculationType_GreaterThan = "GreaterThan";
        public const string CalculationType_GreaterThanEqual = "GreaterThanEqual";
        public const string CalculationType_LessThan = "LessThan";
        public const string CalculationType_LessThanEqual = "LessThanEqual";

        public static List<string> Get_CalculationType()
        {
            return new List<string>() { CalculationType_GreaterThan, CalculationType_GreaterThanEqual,
                CalculationType_LessThan, CalculationType_LessThanEqual };
        }

        // Frequency
        public const string Frequency_Daily = "Daily";

        public static List<string> Get_Frequency()
        {
            return new List<string>() { Frequency_Daily };
        }


    }
}
