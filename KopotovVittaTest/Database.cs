using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity;
using System.Linq;

namespace KopotovVittaTest
{
    public static class Database
    {
        public static bool TryGetUserId(string name, out int id)
        {
            using (Context db = new Context())
            {
                var users = db.Users.Where(u => u.Name == name);

                if (users.Count() <= 0)
                {
                    id = -1;
                    return false;
                }
                else
                {
                    id = users.First().Id;
                    return true;
                }
            }
        }

        public static int CreateUser(string name)
        {
            using (Context db = new Context())
            {
                var userId = 0;
                if (db.Users.Count() != 0)
                    userId = db.Users.Max(u => u.Id) + 1;

                db.Users.Add(new User() {
                    Id = userId,
                    Name = name
                });
                db.SaveChanges();

                return userId;
            }
        }


        public static void CreateOrder(int userId, int value, DateTime date)
        {
            using (Context db = new Context())
            {
                var orderId = 0;
                if (db.Orders.Count() != 0)
                    orderId = db.Orders.Max(u => u.Id) + 1;

                db.Orders.Add(new Order()
                {
                    Id = orderId,
                    Date = date,
                    MasterId = userId,
                    Summ = value,
                    PaymentSumm = 0
                });
                db.SaveChanges();
            }
        }

        public static void CreateMoney(int userId, int value, DateTime date)
        {
            using (Context db = new Context())
            {
                var moneyId = 0;
                if (db.Money.Count() != 0)
                    moneyId = db.Money.Max(u => u.Id) + 1;

                db.Money.Add(new Money()
                {
                    Id = moneyId,
                    Date = date,
                    SummLeft = value,
                    Summ = value,
                    MasterId = userId
                });
                db.SaveChanges();
            }
        }

        public static bool CreatePayment(int ordersId, int moneyId, int val)
        {
            using (Context db = new Context())
            {
                int was = db.Payments.Count();

                var Id = 0;
                if (was != 0)
                    Id = db.Payments.Max(u => u.Id) + 1;

                db.Payments.Add(new Payment()
                {
                    Id = Id,
                    OrderId = ordersId,
                    MoneyId = moneyId,
                    Summ = val
                });
                db.SaveChanges();

                int will = db.Payments.Count();

                return was != will;
            }
        }


        public static Order GetOrderById(int id)
        {
            using (Context db = new Context())
            {
                return db.Orders.Find(id);
            }
        }

        public static Money GetMoneyById(int id)
        {
            using (Context db = new Context())
            {
                return db.Money.Find(id);
            }
        }

        public static Order[] GetOrders(int userId)
        {
            using (Context db = new Context())
            {
                return db.Orders.Where(p => p.MasterId == userId).ToArray();
            }
        }

        public static Money[] GetMoney(int userId)
        {
            using (Context db = new Context())
            {
                return db.Money.Where(p => p.MasterId == userId).ToArray();
            }
        }

        public static Payment[] GetPayments(int userId)
        {
            using (Context db = new Context())
            {
                // Костыли, так как Join работает непонятно.

                var payments = db.Payments.Join(db.Orders,
                    u => u.OrderId,
                    c => c.Id,
                    (u, c) => new PaymentWithNameId()
                    {
                        payment = u,
                        userId = (int)c.MasterId
                    });

                var arr = payments.Where(p => p.userId == userId).ToArray();
                var outArr = new Payment[arr.Length];
                for (int i = 0; i < arr.Length; i++)
                    outArr[i] = arr[i].payment;
                return outArr;
            }
        }

        private class PaymentWithNameId
        {
            public Payment payment;
            public int userId;
        }
        
        private class Context : DbContext
        {
            public Context() : base("VittaEntities")
            { }

            public DbSet<User> Users { get; set; }
            public DbSet<Money> Money { get; set; }
            public DbSet<Order> Orders { get; set; }
            public DbSet<Payment> Payments { get; set; }
        }
    }

    // To String

    partial class Money
    {
        public override string ToString()
        {
            return Id + "     " + Date + "     " + Summ + "     " + SummLeft;
        }
    }
    
    partial class Order
    {
        public override string ToString()
        {
            return Id + "     " + Date + "     " + Summ + "     " + PaymentSumm + "     " + (Summ == PaymentSumm ? "+" : "-");
        }
    }

    partial class Payment
    {
        public override string ToString()
        {
            return OrderId + "     " + MoneyId + "     " + Summ;
        }
    }
}
