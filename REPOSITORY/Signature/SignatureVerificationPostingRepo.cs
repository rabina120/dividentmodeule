
using Entity.Common;
using Interface.Signature;
using Microsoft.Extensions.Options;

namespace Repository.Signature
{
    public class SignatureVerificationPostingRepo : ISignatureVerificationPosting
    {
        IOptions<ReadConfig> _connectionString;

        public SignatureVerificationPostingRepo(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }

    }
}
