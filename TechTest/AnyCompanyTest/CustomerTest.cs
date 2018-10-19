using System;
using System.Data;
using AnyCompany;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnyCompanyTest
{
    [TestClass]
    public class CustomerTest
    {
        [TestMethod]
        public void ReadCustomerOrders_Test()
        {
            DataAccessor da = new DataAccessor();

            var dataSet = da.RetrieveOrdersFromDatabase();

            Assert.AreEqual("CustomerOrders", dataSet.DataSetName);
        }

        [TestMethod]
        public void ReadCustomerOrdersFromCustomerService_Test()
        {
            var cs = new CustomerService(new DataAccessor());

            var dataSet = cs.LoadCustomerOrders();

            Assert.AreEqual("CustomerOrders", dataSet.DataSetName);
        }
    }
}
