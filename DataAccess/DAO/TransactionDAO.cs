using BusinessObject.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    internal class TransactionDAO
    {
        private static TransactionDAO instance = null;
        private static readonly object instanceLock = new object();
        private TransactionDAO() { }
        public static TransactionDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new TransactionDAO();
                    }
                    return instance;
                }
            }
        }

        public void InsertTransaction(TransactionHistory transactionHistory)
        {
            using var context = new RmsContext();
            context.TransactionHistories.Add(transactionHistory);
            context.SaveChanges();
        }
    }
}
