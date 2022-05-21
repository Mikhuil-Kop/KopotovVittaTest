Устройство БД на скриншоте в этой папке.

USE [Vitta]
GO

/****** Object:  Table [dbo].[Users]    Script Date: 20.05.2022 0:04:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Users](
	[Id] [int] NOT NULL,
	[Name] [varchar](50) NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO






USE [Vitta]
GO

/****** Object:  Table [dbo].[Payment]    Script Date: 20.05.2022 0:04:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Payment](
	[OrderId] [int] NOT NULL,
	[MoneyId] [int] NOT NULL,
	[Summ] [int] NOT NULL
) ON [PRIMARY]
GO








USE [Vitta]
GO

/****** Object:  Table [dbo].[Orders]    Script Date: 20.05.2022 0:04:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Orders](
	[Id] [int] NOT NULL,
	[Date] [date] NOT NULL,
	[Summ] [int] NOT NULL,
	[PaymentSumm] [int] NOT NULL,
	[MasterId] [int] NULL,
 CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO









USE [Vitta]
GO

/****** Object:  Table [dbo].[Money]    Script Date: 20.05.2022 0:04:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Money](
	[Id] [int] NOT NULL,
	[Date] [date] NOT NULL,
	[Summ] [int] NOT NULL,
	[SummLeft] [int] NOT NULL,
	[MasterId] [int] NULL,
 CONSTRAINT [PK_Money] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO






Триггеры

------------------------- Хороший вариант. Работает для нескольких Insert -------------------------

Alter Trigger PaymentCreation On Payment Instead Of Insert As
Begin

Create Table #MyInsert(
	OrderId Int,
	MoneyId Int,
	Summ Int
)

Insert Into #MyInsert (OrderId, MoneyId, Summ)
Select P1.OrderId, P1.MoneyId, P1.Summ
		From inserted As P1
	Left Join (Select Sum(Summ) as OrderSumm, OrderId From inserted Group By OrderId) As P2
		On P1.OrderId = P2.OrderId
	Left Join (Select Sum(Summ) as MoneySumm, MoneyId From inserted Group By MoneyId) As P3
		On P1.MoneyId = P3.MoneyId
	Left Join Orders
		On P1.OrderId = Orders.Id
	Left Join Money
		On P1.MoneyId = Money.Id

	Where Orders.Summ - Orders.PaymentSumm >= OrderSumm And Money.SummLeft >= MoneySumm

Declare @message Int
Set @message = (Select Count(*) From #MyInsert)
Print(@message)

Insert Into Payment(OrderId, MoneyId, Summ)
	Select OrderId, MoneyId, Summ
	From #MyInsert

Update Orders
Set PaymentSumm = PaymentSumm + (Select SUM(Summ) From #MyInsert Where Id = OrderId)
Where Id In (Select OrderId From #MyInsert)

Update Money
Set SummLeft = SummLeft - (Select SUM(Summ) From #MyInsert Where Id = MoneyId)
Where Id In (Select MoneyId From #MyInsert)


Drop Table #MyInsert
End