using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Windows.Forms;

namespace KopotovVittaTest
{
    public partial class MainForm : Form
    {
        private int userId;
        private int[] ordersIds, moneyIds;

        public MainForm(int userId)
        {
            InitializeComponent();

            this.userId = userId;

            UpdateOrdersList();
            UpdateMoneyList();
            UpdatePaymentList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Заказать
            if (!int.TryParse(textBox1.Text, out int value))
            {
                SystemSounds.Exclamation.Play();
                return;
            }

            Database.CreateOrder(userId, value, dateTimePicker1.Value);
            UpdateOrdersList();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Пополнить
            if (!int.TryParse(textBox2.Text, out int value))
            {
                SystemSounds.Exclamation.Play();
                return;
            }

            Database.CreateMoney(userId, value, dateTimePicker1.Value);
            UpdateMoneyList();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int orderId = ordersList.SelectedIndex;
            int moneyId = moneyList.SelectedIndex;

            if (orderId == -1 || moneyId == -1)
            {
                MessageBox.Show("Выберите значение из таблиц с заказами и счетами");
                return;
            }

            var form = new PaymentTransfer(ordersIds[orderId], moneyIds[moneyId]);
            form.ShowDialog();

            UpdateOrdersList();
            UpdateMoneyList();
            UpdatePaymentList();
        }

        private void UpdateOrdersList()
        {
            ordersList.Items.Clear();
            var orders = Database.GetOrders(userId);
            ordersIds = new int[orders.Length];
            int i = 0;

            foreach (var o in orders)
            {
                ordersIds[i++] = o.id;
                ordersList.Items.Add(o);
            }
        }

        private void UpdateMoneyList()
        {
            moneyList.Items.Clear();
            var money = Database.GetMoney(userId);
            moneyIds = new int[money.Length];
            int i = 0;

            foreach (var m in money)
            {
                moneyIds[i++] = m.id;
                moneyList.Items.Add(m);
            }
        }

        private void UpdatePaymentList()
        {
            paymentList.Items.Clear();
            var payment = Database.GetPayments(userId);

            foreach (var p in payment)
                paymentList.Items.Add(p);
        }
    }
}
