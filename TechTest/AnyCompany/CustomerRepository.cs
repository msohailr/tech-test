using System;
using System.Data;

namespace AnyCompany
{
    public static class CustomerRepository
    {
        private static DataAccessor dataReader = new DataAccessor();

        public static Customer Load(int customerId)
        {
            Customer customer = new Customer();

            DataRow customerRow = dataReader.RetrieveCustomer(customerId);

            if (customerRow != null)
            {
                customer.Name = customerRow["Name"].ToString();
                customer.DateOfBirth = DateTime.Parse(customerRow["DateOfBirth"].ToString());
                customer.Country = customerRow["Country"].ToString();
            }

            return customer;
        }
    }
}
