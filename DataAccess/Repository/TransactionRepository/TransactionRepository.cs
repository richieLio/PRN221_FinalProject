﻿using BusinessObject.Object;
using DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.TransactionRepository
{
    public class TransactionRepository : ITransactionRepository
    {
        public void InsertTransaction(TransactionHistory transactionHistory) => TransactionDAO.Instance.InsertTransaction(transactionHistory);
    }
}
