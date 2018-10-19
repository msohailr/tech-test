using System.Data;

namespace AnyCompany
{
    public class CustomerService
    {
        private DataAccessor dataAccessor = null;

        /// <summary>
        /// Constructor to set the data accessor object
        /// </summary>
        /// <param name="da"></param>
        public CustomerService(DataAccessor da)
        {
            this.dataAccessor = da;
        }

        /// <summary>
        /// Loads all customers and their relative orders
        /// </summary>
        public DataSet LoadCustomerOrders()
        {
            return dataAccessor.RetrieveOrdersFromDatabase();

            // This above dataset now holds all the customers and their orders
        }
    }
}
