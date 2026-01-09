using BAL.Interface;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Model.Constants;
using Model.MailTemplateHelper;
using Model.ViewModel;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.UtilModels;

namespace BAL.BackgroundWorkerService
{
    public class BackgroundServiceTenderGet : BackgroundService
    {
        public readonly IServiceScopeFactory _ServiceFactory;
        public readonly IConfiguration _configuration;
        private DateTime _lastExecutionTime;

        public BackgroundServiceTenderGet(IServiceScopeFactory ServiceFactory, IConfiguration configuration)
        {
            _ServiceFactory = ServiceFactory;
            _configuration = configuration;
            _lastExecutionTime = DateTime.MinValue;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                List<int> Time = (_configuration.GetSection("BackgroundProcessTime")?.Value?.ToString() ?? "01:01:01").Split(":").Select(x => Convert.ToInt32(x)).ToList();

                // // Calculate time until the next scheduled execution (e.g., 01:30 AM)
                //var now = DateTime.Now;
                //var nextRunTime = new DateTime(now.Year, now.Month, now.Day, Time[0], Time[1], Time[2]).AddDays(1);
                //var delay = nextRunTime - now;

                var now = DateTime.Now;
                var desiredTime = new TimeSpan(Time[0], Time[1], Time[2]); // For example, 8:00 AM
                var timeUntilNextRun = desiredTime > now.TimeOfDay ? desiredTime - now.TimeOfDay : TimeSpan.FromHours(24) - (now.TimeOfDay - desiredTime);


                // Wait until the next scheduled execution time
                await Task.Delay(timeUntilNextRun, stoppingToken);

                // Ensure the service runs only once per day
                var currentTime = DateTime.Now;
                if (currentTime.Date > _lastExecutionTime.Date)
                {
                    using (var scope = _ServiceFactory.CreateScope())
                    {
                        //await Task.Delay(TimeSpan.FromSeconds(5));

                        IGeneralBAL _generalBAL_back = scope.ServiceProvider.GetRequiredService<IGeneralBAL>();
                        IWorkBAL _workBAL_back = scope.ServiceProvider.GetRequiredService<IWorkBAL>();

                        try
                        {
                            #region Get GO/Tender

                            try
                            {
                                List<int> FromDate = (_configuration.GetSection("TenderGetStartDate")?.Value?.ToString() ?? "01/01/2000").Split("/").Select(x => Convert.ToInt32(x)).ToList();

                                _workBAL_back.FetchDepartmentRecords();
                                _workBAL_back.FetchDivisionRecords();
                                TenderDataIntegrationResponceModel responce = _workBAL_back.FetchAwardedTenders(new DateTime(FromDate[2], FromDate[1], FromDate[0]), DateTime.Now);
                                //if (responce.NewContractorList?.Count > 0)
                                //{
                                //    CurrentUserModel currentUser = new CurrentUserModel();
                                //    currentUser.UserName = "System";
                                //    currentUser.UserId = "";

                                //    List<EmailModel> mailList = new List<EmailModel>();
                                //    EmailTemplateModel template = WorkMailTemplate.GetEmailTemplate(EmailTemplateCode.UserCreate);
                                //    responce.NewContractorList.ForEach(contractor =>
                                //    {
                                //        EmailModel mail = new EmailModel();

                                //        mail.Body = template.Body;
                                //        mail.Subject = template.Subject;
                                //        mail.To = new List<string>() { contractor.Email };
                                //        mail.BodyPlaceHolders = new Dictionary<string, string>() {
                                //            { "{RECIPIENTFIRSTNAME}", contractor.FirstName },
                                //            { "{RECIPIENTLASTNAME}", contractor.LastName },
                                //            { "{USERNAME}", contractor.Email },
                                //            { "{PASSWORD}", contractor.Password }
                                //            };

                                //        mailList.Add(mail);
                                //    });

                                //    if (mailList.Count > 0)
                                //    {
                                //        _generalBAL_back.SendMessage(mailList, new List<SMSModel>(), currentUser);
                                //    }
                                //}
                            }
                            catch
                            {

                            }


                            #endregion Get GO/Tender

                            #region Alert Function
                            try
                            {
                                await _generalBAL_back.GetAlertsforBGProcess(Guid.Empty.ToString(), "System");
                            }
                            catch
                            {

                            }
                            #endregion Alert Function

                            #region Alert Mail
                            try
                            {

                            }
                            catch
                            {

                            }
                            #endregion Alert Mail
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, ex.Message);
                        }
                    }
                }

                _lastExecutionTime = currentTime;
            }
        }

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        using (var scope = _ServiceFactory.CreateScope())
        //        {
        //            //await Task.Delay(TimeSpan.FromSeconds(5));

        //            IGeneralBAL _generalBAL_back = scope.ServiceProvider.GetRequiredService<IGeneralBAL>();
        //            IWorkBAL _workBAL_back = scope.ServiceProvider.GetRequiredService<IWorkBAL>();

        //            try
        //            {
        //                _workBAL_back.InsertLog();
        //            }
        //            catch (Exception ex)
        //            {
        //                Log.Error(ex, ex.Message);
        //            }
        //        }

        //        await Task.Delay(TimeSpan.FromSeconds(5));
        //    }
        //}
    }
}
