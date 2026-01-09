using Model.Constants;
using Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.MailTemplateHelper
{
    public static class WorkMailTemplate
    {
        public static EmailTemplateModel? GetEmailTemplate(string Code)
        {
            if (Code == EmailTemplateCode.UserCreate)
            {
                EmailTemplateModel model = new EmailTemplateModel();

                model.Subject = "Your account has been created/saved in TIME application";

                model.Body += "Dear {RECIPIENTFIRSTNAME} {RECIPIENTLASTNAME}, <br/>";
                model.Body += "Your account has been created/saved successfully, Please use the below for login. <br/>";
                model.Body += "User Name: {USERNAME}. <br/>";
                model.Body += "Password: {PASSWORD}. <br/>";

                return model;
            }
            else if (Code == EmailTemplateCode.MBookStatusMail)
            {
                EmailTemplateModel model = new EmailTemplateModel();

                model.Subject = "M-Book status changed : {MBOOKCODE}";

                model.Body += "Dear {RECIPIENTFIRSTNAME} {RECIPIENTLASTNAME}, <br/>";
                model.Body += "M-Book {MBOOKSTATUS} by {ACTIONBYROLE}. <br/>";
                model.Body += "M-Book Code: {MBOOKCODE}. <br/>";
                model.Body += "Milestone Code: {MILESTONECODE}. <br/>";
                model.Body += "User: {ACTIONBY}. <br/>";

                return model;
            }
            else if (Code == EmailTemplateCode.WorkCompletedMail)
            {
                EmailTemplateModel model = new EmailTemplateModel();

                model.Subject = "Work Completed : {WORKCODE}";

                model.Body += "Dear {RECIPIENTFIRSTNAME} {RECIPIENTLASTNAME}, <br/>";
                model.Body += "Work Completed. <br/>";
                model.Body += "Work Code: {WORKCODE}. <br/>";
                model.Body += "User: {ACTIONBY}. <br/>";
                model.Body += "User Role: {ACTIONBYROLE}. <br/>";

                return model;
            }
            else if (Code == EmailTemplateCode.MilestoneCompletedMail)
            {
                EmailTemplateModel model = new EmailTemplateModel();

                model.Subject = "Milestone Completed : {MILESTONECODE}";

                model.Body += "Dear {RECIPIENTFIRSTNAME} {RECIPIENTLASTNAME}, <br/>";
                model.Body += "Milestone Completed. <br/>";
                model.Body += "Milestone Code: {MILESTONECODE}. <br/>";
                model.Body += "User: {ACTIONBY}. <br/>";
                model.Body += "User Role: {ACTIONBYROLE}. <br/>";

                return model;
            }

            return null;
        }
    }
}
