using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DropNet;
using DropNet.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Oauth2.v2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using InfusionSoft;
using InfusionSoft.Custom;
using log4net;
using MailChimpTest.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using Ninject;
using Ninject.Web.Common;
using Softensity.Hatley.Common;
using Softensity.Hatley.DAL;
using Softensity.Hatley.DAL.Interfaces;
using Softensity.Hatley.DAL.Models;
using Softensity.Hatley.Web.Controllers;
using Softensity.Hatley.Web.Core;
using Softensity.Hatley.Web.GoogleDrive;
using Softensity.Hatley.Web.Models.User;
using Stripe;
using File = Google.Apis.Drive.v2.Data.File;
using User = Softensity.Hatley.DAL.Models.User;

namespace Softensity.Hatley.Web.Core
{
    public class BackupEventArgs : EventArgs
    {
        private string _message;

        public BackupEventArgs(string message)
        {
            _message = message;
        }
        public string Messsage
        {
            get { return _message; }
        }
    }

    public class BackupManager
    {
        private IUnitOfWork _unitOfWork;
        private ILog logger = LogManager.GetLogger(typeof(BackupManager));
        public static DropboxConfigurationSection DropboxConfigurationSection =
            ConfigurationManager.GetSection("CustomConfigurationGroup/DropboxConfiguration") as
                DropboxConfigurationSection;
        private string serverFilePath = ConfigurationManager.AppSettings["fileCopyDirectory"];

        public BackupManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void BackupData(Guid userId)
        {
            var user = _unitOfWork.UserManager.FindById(userId);
            BackupData(user);
        }

        public bool BackupData(User user)
        {
            try
            {
                var startTime = DateTime.UtcNow;
                string loggerInfoFormat = "User {0} succesfully backuped his data";            
                if (user == null || ((user.GoogleDriveAccount == null || !user.GoogleDriveAccount.Enabled) &&
                                     (user.DropboxAccount == null || !user.DropboxAccount.Enabled)) ||
                    ((user.ActiveCampaignAccount == null || !user.ActiveCampaignAccount.Enabled) &&
                     (user.AweberAccount == null || !user.AweberAccount.Enabled) &&
                     (user.InfusionSoftAccount == null || !user.InfusionSoftAccount.Enabled)))
                {
                    throw new Exception();
                }
                OnProgress(new BackupEventArgs("Validation of payment ..."));

                StripeCustomerService customerService = new StripeCustomerService();
                IEnumerable<StripeCustomer> customerResponse = customerService.List();
                string customerId = customerResponse.FirstOrDefault(m => m.Email == user.Email).Id;
                StripeSubscriptionService subscriptionService = new StripeSubscriptionService();
                IEnumerable<StripeSubscription> subscriptionResponse = subscriptionService.List(customerId);
                StripeSubscription subscription = subscriptionResponse.First();
                string status = subscription.Status;
                if (user.Payment == null || status != "active")
                {
                    user.Payment.Status = status;
                    _unitOfWork.Commit();
                    throw new Exception("The payment subscription isn't being processed successfully.");
                }

                OnProgress(new BackupEventArgs("Validation is completed. Getting the data from the connected services ..."));
                string backupedFrom = "";
                string backupedTo = "";
                var backupDataHelper = new BackupDataHelper();

                var sourcesTaskList = new List<Task<bool>>();
                if (user.InfusionSoftAccount != null && user.InfusionSoftAccount.Enabled)
                {
                    sourcesTaskList.Add(Task.Factory.StartNew((x) => InfusionSoft(user, backupDataHelper), null));
                    backupedFrom += "InfusionSoft ";
                }
                if (user.ActiveCampaignAccount != null && user.ActiveCampaignAccount.Enabled)
                {
                    sourcesTaskList.Add(Task.Factory.StartNew((x) => ActiveCampaign(user, backupDataHelper), null));
                    backupedFrom += "ActiveCampaign ";
                }
                if (user.AweberAccount != null && user.AweberAccount.Enabled)
                {
                    sourcesTaskList.Add(Task.Factory.StartNew((x) => Aweber(user, backupDataHelper), null));
                    backupedFrom += "Aweber ";
                }
                if (user.MailChimpAccount != null && user.MailChimpAccount.Enabled)
                {
                    sourcesTaskList.Add(Task.Factory.StartNew((x) => MailChimp(user, backupDataHelper), null));
                    backupedFrom += "MailChimp ";
                }
                if (user.GetResponseAccount != null && user.GetResponseAccount.Enabled)
                {
                    sourcesTaskList.Add(Task.Factory.StartNew((x) => GetResponse(user, backupDataHelper), null));
                    backupedFrom += "GetResponse";
                }
                Task.WaitAll(sourcesTaskList.ToArray());
                int countSuccessAccounts = sourcesTaskList.Count(x => x.Result);
                if (countSuccessAccounts == 0)
                {
                    throw new Exception();
                }

                OnProgress(new BackupEventArgs("Getting the data is completed. Converting to CSV files ..."));
                byte[] data = backupDataHelper.Save();
                OnProgress(new BackupEventArgs("Converting to CSV file is completed. Saving data to the connected services .."));

                var deliverTaskList = new List<Task<bool>>();
                if (user.DropboxAccount != null && user.DropboxAccount.Enabled)
                {
                    deliverTaskList.Add(Task.Factory.StartNew((x) => Dropbox(user, data), null));
                    backupedTo += "Dropbox ";
                }
                if (user.GoogleDriveAccount != null && user.GoogleDriveAccount.Enabled)
                {
                    deliverTaskList.Add(Task.Factory.StartNew((x) => GoogleDrive(user, data), null));
                    backupedTo += "GoogleDrive";
                }

                //Save a copy to server----------------------------------

                System.IO.Directory.CreateDirectory(serverFilePath + user.Email.Replace('@', '_'));
                string path = Path.Combine(serverFilePath, user.Email.Replace('@', '_'));
                string fileName = "ListDefender " + DateTime.Now + ".zip";

                var invalidChars = Path.GetInvalidFileNameChars();
                fileName = string.Join("", fileName.Select(c => invalidChars.Contains(c) ? '_' : c));

                path = Path.Combine(path, fileName);


                System.IO.File.WriteAllBytes(path, data);

                //-------------------------------------------------------

                Task.WaitAll(deliverTaskList.ToArray());
                int countSuccessDeliverAccounts = deliverTaskList.Count(x => x.Result);
                if (countSuccessDeliverAccounts == 0)
                {
                    throw new Exception();
                }

                user.Backups.Add(new Backup
                {
                    TimeOfBackup = DateTime.UtcNow,
                    BackupedFrom = backupedFrom,
                    BackupedTo = backupedTo
                });
                _unitOfWork.Commit();
                logger.Info(String.Format(loggerInfoFormat, user.Email));
                var ts = DateTime.UtcNow - startTime;
                OnProgress(new BackupEventArgs("Backup is completed."));
                OnBackupComplete(new BackupEventArgs("Done " + ts.Minutes + ":" + ts.Seconds + ":" + ts.Milliseconds));
                return true;
            }
            catch (Exception e)
            {
                OnProgress(new BackupEventArgs("Backup fail. " + e.Message));
                OnBackupComplete(new BackupEventArgs(""));
                return false;
            }
        }

        private bool InfusionSoft(User user, BackupDataHelper backupDataHelper)
        {
            try
            {
                OnAccountStart(new BackupEventArgs(AccountEnum.InfusionSoft.ToString()));
                var executionTime = 5 * 60;
                var timespan = new TimeSpan(0, 0, 0, user.InfusionSoftAccount.ExpirationTime - executionTime);
                var timeOutToken = user.InfusionSoftAccount.BegginingTime.Add(timespan);
                if (DateTime.UtcNow >= timeOutToken)
                {
                    var helper = new InfusionSoftHelper();
                    var tokenData = helper.RefreshAccessToken(user.InfusionSoftAccount.RefreshToken);

                    tokenData.Enabled = user.InfusionSoftAccount.Enabled;
                    user.InfusionSoftAccount = tokenData;
                    _unitOfWork.Commit();
                }
                var cs = new DataServiceWrapperCustom(user.InfusionSoftAccount.AccessToken);
                var infusionsoftList =
                    DataServiceExtensionsCustom.Query<InfusionSoft.Tables.Contact>((IDataService)cs, user.InfusionSoftAccount.AccessToken);
                backupDataHelper.Add("InfusionSoft.csv", infusionsoftList);
                OnAccountComplete(new BackupEventArgs(AccountEnum.InfusionSoft.ToString()));
                return true;
            }
            catch (Exception ex)
            {
                OnShowError(new BackupEventArgs(AccountEnum.InfusionSoft.ToString()));
                logger.Error("Problems with InfusionSoft", ex);
                _unitOfWork.InfusionSoftRepository.Delete(user.Id);
                _unitOfWork.Commit();
                return false;
            }
        }

        private bool ActiveCampaign(User user, BackupDataHelper backupDataHelper)
        {
            try
            {
                OnAccountStart(new BackupEventArgs(AccountEnum.ActiveCampaign.ToString()));
                var subs = ActiveCampaignHelper.GetContactsData(user.ActiveCampaignAccount.ApiUrl,
                    user.ActiveCampaignAccount.ApiKey);
                backupDataHelper.Add("ActiveCampaign.csv", subs.Subscribers);
                OnAccountComplete(new BackupEventArgs(AccountEnum.ActiveCampaign.ToString()));
                return true;
            }
            catch (Exception ex)
            {
                OnShowError(new BackupEventArgs(AccountEnum.ActiveCampaign.ToString()));
                logger.Error("Problems with Active Campaign", ex);
                _unitOfWork.ActiveCampaignRepository.Delete(user.Id);
                _unitOfWork.Commit();
                return false;
            }
        }

        private bool Aweber(User user, BackupDataHelper backupDataHelper)
        {
            try
            {
                OnAccountStart(new BackupEventArgs(AccountEnum.Aweber.ToString()));
                var aweberSubscribers = AweberHelper.GetContactsFromAweber(user.AweberAccount.AccessToken,
                    user.AweberAccount.TokenSecret);
                backupDataHelper.Add("Aweber.csv", aweberSubscribers);
                OnAccountComplete(new BackupEventArgs(AccountEnum.Aweber.ToString()));
                return true;
            }
            catch (Exception ex)
            {
                OnShowError(new BackupEventArgs(AccountEnum.Aweber.ToString()));
                logger.Error("Problems with Aweber", ex);
                _unitOfWork.AweberRepository.Delete(user.Id);
                _unitOfWork.Commit();
                return false;
            }
        }

        private bool MailChimp(User user, BackupDataHelper backupDataHelper)
        {
            try
            {
                OnAccountStart(new BackupEventArgs(AccountEnum.MailChimp.ToString()));
                var members = MailChimpHelper.GetAllMembers(user.MailChimpAccount.ApiKey);
                backupDataHelper.Add("MailChimp.csv", members);
                OnAccountComplete(new BackupEventArgs(AccountEnum.MailChimp.ToString()));
                return true;
            }
            catch (Exception ex)
            {
                OnShowError(new BackupEventArgs(AccountEnum.MailChimp.ToString()));
                logger.Error("Problems with MailChimp", ex);
                _unitOfWork.MailChimpRepository.Delete(user.Id);
                _unitOfWork.Commit();
                return false;
            }
        }

        private bool GetResponse(User user, BackupDataHelper backupDataHelper)
        {
            try
            {
                OnAccountStart(new BackupEventArgs(AccountEnum.GetResponse.ToString()));
                var accauntlist = GetResponseHelper.GetContactsData(user.GetResponseAccount.ApiKey);
                backupDataHelper.Add("GetResponse.csv", accauntlist);
                OnAccountComplete(new BackupEventArgs(AccountEnum.GetResponse.ToString()));
                return true;
            }
            catch (Exception ex)
            {
                OnShowError(new BackupEventArgs(AccountEnum.GetResponse.ToString()));
                logger.Error("Problems with GetResponse", ex);
                _unitOfWork.MailChimpRepository.Delete(user.Id);
                _unitOfWork.Commit();
                return false;
            }
        }

        private bool GoogleDrive(User user, byte[] data)
        {
            try
            {
                OnAccountStart(new BackupEventArgs(AccountEnum.GoogleDrive.ToString()));
                if (DateTime.UtcNow.Subtract(user.GoogleDriveAccount.ConnectingDate).TotalSeconds + 300 >
                    user.GoogleDriveAccount.ExpiresInSeconds)
                {
                    user.GoogleDriveAccount = GoogleDriveApi.RefreshUser(user.StringId, user.GoogleDriveAccount);
                    _unitOfWork.Commit();
                }

                DriveService service = CreateDriveService(user);
                string folderId = GetDriveFolderId(service);
                UploadDriveFile(service, data, folderId);

                OnAccountComplete(new BackupEventArgs(AccountEnum.GoogleDrive.ToString()));
                return true;
            }
            catch (Exception ex)
            {
                OnShowError(new BackupEventArgs(AccountEnum.GoogleDrive.ToString()));
                logger.Error("Problems with Google Drive", ex);
                _unitOfWork.GoogleDriveRepository.Delete(user.Id);
                _unitOfWork.Commit();
                return false;
            }
        }

        private DriveService CreateDriveService(User user)
        {
            var token = new TokenResponse()
            {
                AccessToken = user.GoogleDriveAccount.AccessToken,
                Issued = user.GoogleDriveAccount.Issued,
                RefreshToken = user.GoogleDriveAccount.RefreshToken,
                Scope = user.GoogleDriveAccount.Scope,
                TokenType = user.GoogleDriveAccount.TokenType,
                ExpiresInSeconds = user.GoogleDriveAccount.ExpiresInSeconds
            };
            var userCredential = new UserCredential(GoogleDriveController.AppFlow.Flow, user.StringId, token);



            //string[] scopes = new string[] { DriveService.Scope.Drive };
            //var keyFilePath = @"E:\CodeCraft\Hatley\Google API Key\Hatley-f218d008253e.p12"; //TODO: move it to web.config
            //var serviceAccountEmail = "maryaninin97@gmail.com"; //TODO: move it to web.config
            //var certificate = new X509Certificate2(keyFilePath, "notasecret", X509KeyStorageFlags.Exportable);
            //var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(serviceAccountEmail)
            //{ Scopes = scopes }.FromCertificate(certificate));
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = userCredential,
                ApplicationName = "Hatley"
            });
            return service;
        }

        private string GetDriveFolderId(DriveService service)
        {
            var request = service.Files.List();
            request.Q = "title='List Defender'";
            var result = request.Execute();
            string folderId;
            if (result.Items.Count != 0)
            {
                folderId = result.Items[0].Id;
            }
            else
            {
                folderId = CreateFolder(service);
            }
            return folderId;
        }

        private string CreateFolder(DriveService service)
        {
            File folderMetadata = new File();
            folderMetadata.Title = "List Defender";
            folderMetadata.MimeType = "application/vnd.google-apps.folder";
            var request = service.Files.Insert(folderMetadata);
            request.Fields = "id";
            File folder = request.Execute();
            string result = folder.Id;
            return result;
        }

        private void UploadDriveFile(DriveService service, byte[] data, string folderId)
        {
            File zipFile = new File
            {
                Title = "ListDefender " + DateTime.Now + ".zip"
            };
            zipFile.Parents = new List<ParentReference> { new ParentReference() { Id = folderId } };
            FilesResource.InsertMediaUpload fileRequest;
            using (Stream stream = new MemoryStream(data))
            {
                fileRequest = service.Files.Insert(zipFile, stream, "application/zip");
                fileRequest.Fields = "id";
                fileRequest.Upload();
            }
        }

        private bool Dropbox(User user, byte[] data)
        {
            try
            {
                OnAccountStart(new BackupEventArgs(AccountEnum.Dropbox.ToString()));
                var dropClient = new DropNetClient(DropboxConfigurationSection.DropboxApiKey,
                    DropboxConfigurationSection.DropboxAppSecret);
                dropClient.UserLogin = new UserLogin
                {
                    Secret = user.DropboxAccount.UserSecret,
                    Token = user.DropboxAccount.UserToken
                };
                dropClient.AccountInfo();
                try
                {
                    dropClient.UploadFile("/List Defender", "ListDefender " + DateTime.Now + ".zip", data);
                }
                catch (Exception)
                {
                    dropClient.CreateFolder("List Defender");
                    dropClient.UploadFile("/List Defender", "ListDefender " + DateTime.Now + ".zip", data);
                }
                OnAccountComplete(new BackupEventArgs(AccountEnum.Dropbox.ToString()));
                return true;
            }
            catch (Exception ex)
            {
                OnShowError(new BackupEventArgs(AccountEnum.Dropbox.ToString()));
                logger.Error("Problems with Dropbox");
                _unitOfWork.DropboxRepository.Delete(user.Id);
                _unitOfWork.Commit();
                return false;
            }
        }

        public event EventHandler<BackupEventArgs> AccountStart;
        public event EventHandler<BackupEventArgs> AccountComplete;
        public event EventHandler<BackupEventArgs> BackupComplete;
        public event EventHandler<BackupEventArgs> Progress;
        public event EventHandler<BackupEventArgs> ShowError;
        protected virtual void OnAccountStart(BackupEventArgs e)
        {
            EventHandler<BackupEventArgs> handler = AccountStart;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        protected virtual void OnAccountComplete(BackupEventArgs e)
        {
            EventHandler<BackupEventArgs> handler = AccountComplete;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        protected virtual void OnBackupComplete(BackupEventArgs e)
        {
            EventHandler<BackupEventArgs> handler = BackupComplete;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        protected virtual void OnProgress(BackupEventArgs e)
        {
            EventHandler<BackupEventArgs> handler = Progress;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        protected virtual void OnShowError(BackupEventArgs e)
        {
            EventHandler<BackupEventArgs> handler = ShowError;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}

