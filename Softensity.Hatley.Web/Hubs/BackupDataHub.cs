using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Softensity.Hatley.DAL.Interfaces;
using Softensity.Hatley.Web.Core;

namespace Softensity.Hatley.Web.Hubs
{
    public class BackupDataHub : Hub
    {
        private IUnitOfWork _unitOfWork;

        public BackupDataHub(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Start(Guid userId)
        {
            var manager = new BackupManager(_unitOfWork);
            manager.Progress += (sender, args) =>
            {
                Clients.Client(Context.ConnectionId).progress(args.Messsage);
            };
            manager.BackupComplete += (sender, args) =>
            {
                Clients.Client(Context.ConnectionId).backupComplete(args.Messsage);
            };
            manager.AccountStart += (sender, args) =>
            {
                Clients.Client(Context.ConnectionId).accountStart(args.Messsage);
            };
            manager.AccountComplete += (sender, args) =>
            {
                Clients.Client(Context.ConnectionId).accountComplete(args.Messsage);
            };
            manager.ShowError += (sender, args) =>
            {
                Clients.Client(Context.ConnectionId).showError(args.Messsage);
            };
            Task.Factory.StartNew((x) => manager.BackupData(userId), null);
        }
    }
}