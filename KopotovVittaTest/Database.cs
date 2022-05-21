using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace KopotovVittaTest
{
    public static class Database
    {
        private static SqlConnection connection;

        public static void Initialize()
        {
            connection = new SqlConnection(@"Data Source=DESKTOP-P0TES7A\SQLEXPRESS;Initial Catalog=Vitta;Integrated Security=true");
            connection.Open();
        }

        public static void Close()
        {
            connection.Close();
        }


        public static bool TryGetUserId(string name, out int id)
        {
            var adapter = new SqlDataAdapter();
            var table = new DataTable();

            var test = $"Select * From Users Where Name = '{name}'";
            var command = new SqlCommand(test, connection);
            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count >= 1)
            {
                id = table.Rows[0].Field<int>("Id");
                return true;
            }
            else
            {
                id = 0;
                return false;
            }
        }

        public static int CreateUser(string name)
        {
            var adapter = new SqlDataAdapter();
            var table = new DataTable();

            var textId = "Select Top(1) Id From Users Order By Id Desc";
            var commadnId = new SqlCommand(textId, connection);
            adapter.SelectCommand = commadnId;
            adapter.Fill(table);

            int i;
            if (table.Rows.Count != 0)
                i = table.Rows[0].Field<int>("Id") + 1;
            else
                i = 0;

            var textInsert = $"Insert Into Users (Id, Name) Values({i},'{name}')";
            var commandInsert = new SqlCommand(textInsert, connection);
            commandInsert.ExecuteNonQuery();

            return i;
        }


        public static int CreateOrder(int userId, int value, DateTime date)
        {
            var d = date.Date.ToString("yyyy-MM-dd");

            var textInsert = $"IF EXISTS (Select * From Orders)" +
                $"Insert Into Orders(Id, Date, Summ, PaymentSumm, MasterId) Select Top(1) Id + 1, '{d}', {value}, 0, {userId} From Orders Order By Id Desc;" +
                $" else " +
                $"Insert Into Orders(Id, Date, Summ, PaymentSumm, MasterId) Values(0, '{d}', {value}, 0, {userId})";
            var commandInsert = new SqlCommand(textInsert, connection);
            return commandInsert.ExecuteNonQuery();
        }

        public static int CreateMoney(int userId, int value, DateTime date)
        {
            var d = date.Date.ToString("yyyy-MM-dd");

            var textInsert = $"IF EXISTS (Select * From Money)" +
                 $"Insert Into Money(Id, Date, Summ, SummLeft, MasterId) Select Top(1) Id + 1, '{d}', {value}, {value}, {userId} From Money Order By Id Desc;" +
                 $" else " +
                 $"Insert Into Money(Id, Date, Summ, SummLeft, MasterId) Values(0, '{d}', {value}, {value}, {userId})";
            var commandInsert = new SqlCommand(textInsert, connection);
            return commandInsert.ExecuteNonQuery();
        }

        public static int CreatePayment(int ordersId, int moneyId, int val)
        {
            var textInsert = $"Insert Into Payment(MoneyId, OrderId, Summ) Values({moneyId},{ordersId},{val})";
            var commandInsert = new SqlCommand(textInsert, connection);
            return commandInsert.ExecuteNonQuery();
        }


        public static Order GetOrderById(int id)
        {
            var adapter = new SqlDataAdapter();
            var table = new DataTable();

            var textId = $"Select * From Orders Where Id = {id}";
            var commadnId = new SqlCommand(textId, connection);
            adapter.SelectCommand = commadnId;
            adapter.Fill(table);
            var r = table.Rows[0];

            return new Order
            {
                id = r.Field<int>("Id"),
                summ = r.Field<int>("Summ"),
                paymentSumm = r.Field<int>("PaymentSumm"),
                date = r.Field<DateTime>("Date"),
            };
        }

        public static Money GetMoneyById(int id)
        {
            var adapter = new SqlDataAdapter();
            var table = new DataTable();

            var textId = $"Select * From Money Where Id = {id}";
            var commadnId = new SqlCommand(textId, connection);
            adapter.SelectCommand = commadnId;
            adapter.Fill(table);
            var r = table.Rows[0];

            return new Money
            {
                id = r.Field<int>("Id"),
                summ = r.Field<int>("Summ"),
                summLeft = r.Field<int>("SummLeft"),
                date = r.Field<DateTime>("Date"),
            };
        }

        public static Order[] GetOrders(int userId)
        {
            var adapter = new SqlDataAdapter();
            var table = new DataTable();

            var textId = $"Select * From Orders Where MasterId = {userId}";
            var commadnId = new SqlCommand(textId, connection);
            adapter.SelectCommand = commadnId;
            adapter.Fill(table);

            var arr = new Order[table.Rows.Count];
            int i = 0;

            foreach (DataRow r in table.Rows)
                arr[i++] = new Order
                {
                    id = r.Field<int>("Id"),
                    summ = r.Field<int>("Summ"),
                    paymentSumm = r.Field<int>("PaymentSumm"),
                    date = r.Field<DateTime>("Date"),
                };

            return arr;
        }

        public static Money[] GetMoney(int userId)
        {
            var adapter = new SqlDataAdapter();
            var table = new DataTable();

            var textId = $"Select * From Money Where MasterId = {userId}";
            var commadnId = new SqlCommand(textId, connection);
            adapter.SelectCommand = commadnId;
            adapter.Fill(table);

            var arr = new Money[table.Rows.Count];
            int i = 0;

            foreach (DataRow r in table.Rows)
                arr[i++] = new Money
                {
                    id = r.Field<int>("Id"),
                    summ = r.Field<int>("Summ"),
                    summLeft = r.Field<int>("SummLeft"),
                    date = r.Field<DateTime>("Date"),
                };

            return arr;
        }

        public static Payment[] GetPayments(int userId)
        {
            var adapter = new SqlDataAdapter();
            var table = new DataTable();

            var textId = $"Select OrderId, MoneyId, Payment.Summ From Payment Join Orders On OrderId = Id Where MasterId = {userId}";
            var commadnId = new SqlCommand(textId, connection);
            adapter.SelectCommand = commadnId;
            adapter.Fill(table);

            var arr = new Payment[table.Rows.Count];
            int i = 0;

            foreach (DataRow r in table.Rows)
                arr[i++] = new Payment
                {
                    orderId = r.Field<int>("OrderId"),
                    moneyId = r.Field<int>("MoneyId"),
                    value = r.Field<int>("Summ"),
                };

            return arr;
        }


        public struct Order
        {
            public int id;
            public int summ, paymentSumm;
            public DateTime date;

            public override string ToString()
            {
                return id + "     " + date.ToString("yyyy-MM-dd") + "     " + summ + "     " + paymentSumm + "     " + (summ == paymentSumm ? "+" : "-");
            }
        }

        public struct Money
        {
            public int id;
            public int summ, summLeft;
            public DateTime date;

            public override string ToString()
            {
                return id + "     " + date.ToString("yyyy-MM-dd") + "     " + summ + "     " + summLeft;
            }
        }

        public struct Payment
        {
            public int orderId, moneyId, value;

            public override string ToString()
            {
                return orderId + "     " + moneyId + "     " + value;
            }
        }
    }
}
