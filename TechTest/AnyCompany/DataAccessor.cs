using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;

namespace AnyCompany
{
    public class DataAccessor
    {
        /// <summary>
        /// Connection string should be set to a physical database, which can be published using the 
        /// sql project Customer under the solution.
        /// </summary>
        private string connectionString = @"Data Source=(local);Database=Customer;User Id=tech_test;Password=Passw0rd1;";

        #region Public methods

        /// <summary>
        /// The method retrieves all the customers and their relative orders
        /// </summary>
        /// <returns></returns>
        public DataSet RetrieveOrdersFromDatabase()
        {
            var dataSet = new DataSet("CustomerOrders");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var sbSql = new StringBuilder();
                sbSql.AppendLine($"SELECT orderDetails.CustomerId, orderDetails.OrderId, customer.Name, customer.Country, ord.Amount");
                sbSql.AppendLine("FROM Customer customer");
                sbSql.AppendLine("INNER JOIN CustomerOrder orderDetails on customer.CustomerId = orderDetails.CustomerId");
                sbSql.AppendLine("INNER JOIN [Order] ord on orderDetails.OrderId = ord.OrderId");
                sbSql.AppendLine("ORDER BY orderDetails.CustomerId"); // retrieve data according to the customer and their relative orders

                using (var adapter = new SqlDataAdapter(sbSql.ToString(), connection))
                {
                    adapter.Fill(dataSet);
                }
            }

            return dataSet;
        }

        /// <summary>
        /// Retrieve the customer specified by the customer Id
        /// </summary>
        /// <param name="customerId">The customer Id for the customer in question</param>
        /// <returns></returns>
        public DataRow RetrieveCustomer(int customerId)
        {
            var table = new DataTable("Customer");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand($"SELECT * FROM CUSTOMER WHERE CustomerId = {customerId}", connection))
                {
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(table);
                    }
                }
            }

            if (table.Rows.Count > 0)
            {
                return table.Rows[0];
            }

            return null;
        }

        /// <summary>
        /// Write a customer to database
        /// </summary>
        /// <param name="customer"></param>
        public int WriteCustomerToDatabase (Customer customer)
        {
            var customerId = 0;
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;

                        command.CommandText = "INSERT INTO [Customer] (CustomerId, DateOfBirth, Name, Country) VALUES (@CustomerId, @DateOfBirth, @Name, @Country)";

                        customerId = GetLastCustomerId() + 1;
                        command.Parameters.AddWithValue("@CustomerId", customerId);
                        command.Parameters.AddWithValue("@DateOfBirth", customer.DateOfBirth);
                        command.Parameters.AddWithValue("@Name", customer.Name);
                        command.Parameters.AddWithValue("@Country", customer.Country);

                        command.ExecuteNonQuery();
                    }
                }

                return customerId;
            }

            catch (SqlException exp)
            {
                customerId = 0;

                Debug.WriteLine($"Exception occured while saving order details: {Environment.NewLine}{exp.Message}");

                throw exp;
            }

            return customerId;
        }

        /// <summary>
        /// Write an order to the database under a SQL transaction
        /// </summary>
        /// <param name="order">The order to write</param>
        public void WriteOrderToDatabase(Order order)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;

                        // Multiple table operation - better to complete within transaction
                        command.Transaction = connection.BeginTransaction();

                        command.CommandText = "INSERT INTO [Order] (OrderId, VAT, Amount) VALUES (@OrderId, @VAT, @Amount)";

                        var newOrder = GetLastOrderId() + 1;
                        command.Parameters.AddWithValue("@OrderId", newOrder);
                        command.Parameters.AddWithValue("@VAT", order.VAT);
                        command.Parameters.AddWithValue("@Amount", order.Amount);

                        command.ExecuteNonQuery();

                        // Insert in CustomerOrder table
                        command.Parameters.Clear();

                        command.CommandText = "INSERT INTO CustomerOrder (CustomerId, OrderId) VALUES (@CustomerId, @OrderId)";

                        command.Parameters.AddWithValue("@CustomerId", order.CustomerId);
                        command.Parameters.AddWithValue("@OrderId", newOrder);

                        command.ExecuteNonQuery();

                        // Commit the database changes
                        command.Transaction.Commit();

                        order.OrderId = newOrder;
                    }
                }
            }

            catch (SqlException exp)
            {
                order.OrderId = 0;
                Debug.WriteLine($"Exception occured while saving order details: \n{exp.Message}");

                throw exp;
            }
        }

        /// <summary>
        /// Added to support the integration test to delete any data added by the test itself
        /// </summary>
        /// <param name="orderId">OrderId to delete</param>
        /// <param name="customerId">CustomerId to delete</param>
        public void DeleteOrder(int orderId, int customerId)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = $"DELETE FROM CustomerOrder WHERE CustomerId = {customerId} AND OrderId = {orderId}";

                        command.ExecuteNonQuery();

                        command.CommandText = $"DELETE FROM [Order] WHERE OrderId = {orderId}";

                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }

            catch (SqlException exp)
            {
                Debug.WriteLine($"Exception occured while deleting order informtion: {Environment.NewLine}{exp.Message}");

                throw exp;
            }
        }

        /// <summary>
        /// Added to support the integration test to delete any data added by the test itself
        /// </summary>
        /// <param name="orderId">OrderId to delete</param>
        /// <param name="customerId">CustomerId to delete</param>
        public void DeleteCustomer(int customerId)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = $"DELETE FROM Customer WHERE CustomerId = {customerId}";

                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }

            catch (SqlException exp)
            {
                Debug.WriteLine($"Exception occured while deleting order informtion: {Environment.NewLine}{exp.Message}");

                throw exp;
            }
        }

        #endregion

        #region Private

        /// <summary>
        /// Gets the last customer Id added to the customer table
        /// </summary>
        /// <returns></returns>
        private int GetLastCustomerId()
        {
            var customerId = 0;

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;

                        command.CommandText = "SELECT MAX(CustomerId) as CustomerId FROM Customer";

                        var reader = command.ExecuteReader();

                        if (reader.Read() && reader.HasRows)
                        {
                            if (!(reader[0] is DBNull))
                            {
                                customerId = reader.GetInt32(0);
                            }
                        }

                        reader.Close();
                    }

                    connection.Close();
                }
            }

            catch (SqlException exp)
            {
                Debug.WriteLine($"An error occured: {Environment.NewLine}{exp.Message}");
            }

            return customerId;
        }

        /// <summary>
        /// Gets the last customer Id added to the customer table
        /// </summary>
        /// <returns></returns>
        private int GetLastOrderId()
        {
            var orderId = 0;

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;

                        command.CommandText = "SELECT MAX(OrderId) as OrderId FROM [Order]";

                        var reader = command.ExecuteReader();

                        if (reader.Read() && reader.HasRows)
                        {
                            if (!(reader[0] is DBNull))
                            {
                                orderId = reader.GetInt32(0);
                            }
                        }

                        reader.Close();
                    }

                    connection.Close();
                }
            }

            catch (SqlException exp)
            {
                Debug.WriteLine($"An error occured: {Environment.NewLine}{exp.Message}");
            }

            return orderId;
        }

        #endregion

    }
}
