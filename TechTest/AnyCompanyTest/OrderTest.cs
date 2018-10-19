using System;
using AnyCompany;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnyCompanyTest
{
    [TestClass]
    public class OrderTest
    {
        private int customerId;
        private int orderId;

        [TestMethod]
        public void OrderPlacement_Test()
        {
            var da = new DataAccessor();

            // Add a customer
            customerId = da.WriteCustomerToDatabase(new AnyCompany.Customer { Country = "UK", DateOfBirth = DateTime.Now, Name = "Xyz" });

            // Create Order
            var order = new Order() { Amount = 10.05, CustomerId = customerId, VAT = 20 };
            da.WriteOrderToDatabase(order);

            orderId = order.OrderId;

            Assert.IsTrue(customerId > 0);
            Assert.IsTrue(orderId > 0);
        }

        [TestCleanup]
        public void Cleanup()
        {
            var da = new DataAccessor();

            da.DeleteOrder(orderId, customerId);
            da.DeleteCustomer(customerId);
        }
    }
}
