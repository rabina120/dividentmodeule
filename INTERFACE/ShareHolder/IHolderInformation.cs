
using Entity.Common;
using Entity.ShareHolder;

namespace Interface.ShareHolder
{
    public interface IHolderInformation
    {

        public JsonResponse GetSHholder(string ShHolderNo, string CompCode, string SelectedAction, string UserName, string IP);
        //public JsonResponse GetSHholder(string ShHolderNo, string CompCode,string SelectedAction,string UserName);

        public JsonResponse GetNewShHolderNo(string compcode);
        //public JsonResponse SaveShHolder(ATTShHolder shholder, byte[] signature, string signFileLength, string filename, string updateRemarks, string selectedAction);

        //WithoutSignature
        public JsonResponse SaveShHolder(ATTShHolder shholder, string updateRemarks, string selectedAction, string UserName, string IP);

        public JsonResponse GetHolderByQuery(string CompCode, string FirstName, string LastName, string FatherName, string GrandFatherName, string UserName, string IP);

    }
}
