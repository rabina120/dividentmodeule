
using Entity.Common;
using Interface.Signature;
using Microsoft.Extensions.Options;

namespace Repository.Signature
{
    public class SignatureVerificationEntryRepo : ISignatureVerificationEntry
    {
        IOptions<ReadConfig> _connectionString;

        public SignatureVerificationEntryRepo(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }
    }
}
