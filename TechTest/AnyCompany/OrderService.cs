namespace AnyCompany
{
    public class OrderService
    {
        private readonly OrderRepository orderRepository = new OrderRepository();
        private DataAccessor dataAccessor = null;

        /// <summary>
        /// Performing constructor injection method to set the data accessor to save
        /// order information
        /// </summary>
        /// <param name="dataAccessor">The data accessor layer's object to save order information</param>
        public OrderService(DataAccessor dataAccessor)
        {
            this.dataAccessor = dataAccessor;
        }

        /// <summary>
        /// This method places an order for the passed in customer
        /// </summary>
        /// <param name="order">The order information</param>
        /// <param name="customerId">the customer for which the order is being placed</param>
        /// <returns></returns>
        public bool PlaceOrder(Order order, int customerId)
        {
            Customer customer = CustomerRepository.Load(customerId);
            order.CustomerId = customerId;

            if (order.Amount == 0)
                return false;

            if (customer.Country == "UK")
                order.VAT = 0.2d;
            else
                order.VAT = 0;

            dataAccessor.WriteOrderToDatabase(order);

            return true;
        }
    }
}
