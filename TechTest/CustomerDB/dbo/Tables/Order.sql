CREATE TABLE [dbo].[Order] (
    [OrderId] INT             NOT NULL,
    [Amount]  DECIMAL (37, 2) NULL,
    [VAT]     DECIMAL (18, 2) NULL,
    CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([OrderId] ASC)
);

