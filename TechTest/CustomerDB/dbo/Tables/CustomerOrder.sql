CREATE TABLE [dbo].[CustomerOrder] (
    [CustomerId] INT NOT NULL,
    [OrderId]    INT NOT NULL,
    CONSTRAINT [FK_Customer_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Order] ([OrderId])
);

