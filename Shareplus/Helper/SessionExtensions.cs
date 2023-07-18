using Entity.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json;
namespace CDSMODULE.Helper
{
    public static class SessionExtensions
    {

        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonSerializer.Deserialize<T>(value);
        }
        public static void UpdateSessionStorage(this ISession session, ATTSession sessionStorage)
        {
            string deSerializedSession = JsonSerializer.Serialize(sessionStorage);
            string encryptedSession = Crypto.Encrypt(deSerializedSession);
            session.SetString("PSMS", encryptedSession);
        }
        public static ATTSession GetSessionStorage(this ISession session)
        {
            try
            {
                string sessionStorage = session.GetString("PSMS");
                string decryptedSessionStorage = Crypto.Decrypt(sessionStorage);
                return JsonSerializer.Deserialize<ATTSession>(decryptedSessionStorage);
            }

            catch (Exception e)
            {
                return new ATTSession();
            }
        }
        public static ATTCompany GetConnectedCompany(this ISession session)
        {
            try
            {
                ATTCompany company = new ATTCompany();

                string sessionStorage = session.GetString("PSMS_CD");
                if (sessionStorage == null) return company;
                string decryptedSessionStorage = Crypto.Decrypt(sessionStorage);
                company = JsonSerializer.Deserialize<ATTCompany>(decryptedSessionStorage);

                return company;
            }
            catch (Exception e)
            {
                return new ATTCompany();
            }
        }
        public static void ConnectCompany(this ISession session, ATTCompany CompanyDetail)
        {
            string deSerializedSession = JsonSerializer.Serialize(CompanyDetail);
            string encryptedSession = Crypto.Encrypt(deSerializedSession);
            session.SetString("PSMS_CD", encryptedSession);


        }
        public static void AddSessionStorage(this ISession session, ATTCompany CompanyDetail)
        {
            string deSerializedSession = JsonSerializer.Serialize(CompanyDetail);
            string encryptedSession = Crypto.Encrypt(deSerializedSession);
            session.SetString("PSMS", encryptedSession);

        }
        public static void ClearSessionStorage(this ISession session)
        {
            session.Remove("PSMS_CD");
            session.Remove("PSMS");
        }
        public static void DisconnectCompany(this ISession session)
        {
            session.Remove("PSMS_CD");
        }

    }


}

