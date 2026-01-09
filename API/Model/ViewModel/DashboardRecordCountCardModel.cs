using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class DashboardRecordCountCardModel
    {
        public int Project_Finished { get; set; }
        public int Project_OnGoing { get; set;}
        public int Project_OnHold { get; set;}
        public int Project_Upcoming { get; set;}
        public int Project_Slowprogress { get; set;}
        public int Total_Project { get; set;}

        public decimal Project_Finished_Amount { get; set; }
        public decimal Project_OnGoing_Amount { get; set; }
        public decimal Project_OnHold_Amount { get; set; }
        public decimal Project_Upcoming_Amount { get; set; }
        public decimal Project_Slowprogress_Amount { get; set; }
        public decimal Total_Project_Amount { get; set; }

        public string Project_Finished_Amount_Text { get; set; } = string.Empty;
        public string Project_OnGoing_Amount_Text { get; set; } = string.Empty;
        public string Project_OnHold_Amount_Text { get; set; } = string.Empty;
        public string Project_Upcoming_Amount_Text { get; set; } = string.Empty;
        public string Project_Slowprogress_Amount_Text { get; set; }= string.Empty;
        public string Total_Project_Amount_Text { get; set; }=string.Empty;

        public int Mbook_Approved { get; set; }
        public int Mbook_InApproval { get; set; }
        public int Mbook_Upcoming { get; set; }
        public int Mbook_Rejected { get; set; }
        public int TotalMbooks { get; set; }




        public int Mbook_NotUploaded { get; set; }
        public int Mbook_Uploaded { get; set; }
        public int Mbook_No_Action_Taken { get; set; }
        public int Mbook_PaymentPending { get; set; }





        public decimal Mbook_Approved_Amount { get; set; }
        public decimal Mbook_InApproval_Amount { get; set; }
        public decimal Mbook_Upcoming_Amount { get; set; }
        public decimal Mbook_Rejected_Amount { get; set; }



        public decimal Mbook_NotUploaded_Amount { get; set; }
        public decimal Mbook_Uploaded_Amount { get; set; }
        public decimal Mbook_No_Action_Taken_Amount { get; set; }
        public decimal Mbook_PaymentPending_Amount { get; set; }
        public decimal Mbook_Total_Amount { get; set; }

        public string Mbook_Approved_Amount_Text { get; set; } = string.Empty;
        public string Mbook_InApproval_Amount_Text { get; set; } = string.Empty;
        public string Mbook_Upcoming_Amount_Text { get; set; } = string.Empty;
        public string Mbook_Rejected_Amount_Text { get; set; } = string.Empty;
        public string Mbook_Total_Amount_Text { get; set; } = string.Empty;



        public string Mbook_NotUploaded_Amount_Text { get; set; } = string.Empty;
        public string Mbook_Uploaded_Amount_Text { get; set; } = string.Empty;
        public string Mbook_No_Action_Taken_Amount_Text { get; set; } = string.Empty;
        public string Mbook_PaymentPending_Amount_Text { get; set; } = string.Empty;


    }
}
