
using Entity.Common;
using System;

namespace Interface.Security
{
    public interface IAudit
    {
        public void saveLogAsync(string xAction, string rFile, string username, string IPAddress);
        void auditSave(string username, string xAction, string rFile);
        JsonResponse auditSave(string username, string xAction, string rFile, string IPAddress);
        JsonResponse errorSave(string username, string rFile, string IPAddress, Exception exception);
    }
}
