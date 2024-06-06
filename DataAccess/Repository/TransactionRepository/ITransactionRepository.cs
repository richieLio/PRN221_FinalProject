using BusinessObject.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.TransactionRepository
{
    public interface ITransactionRepository
    {
        void InsertTransaction(TransactionHistory transactionHistory);
    }
}
