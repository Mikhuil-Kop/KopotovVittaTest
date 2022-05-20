using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KopotovVittaTest
{
    public partial class PaymentTransfer : Form
    {
        private int ordersId, moneyId, max = 0;

        public PaymentTransfer(int ordersId, int moneyId)
        {
            InitializeComponent();

            this.ordersId = ordersId;
            this.moneyId = moneyId;

            var order = Database.GetOrderById(ordersId);
            var money = Database.GetMoneyById(moneyId);

            orderLabel.Text = order.ToString();
            MoneyLabel.Text = money.ToString();

            max = Math.Min(order.summ - order.paymentSumm, money.summLeft);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox1.Text, out int val) && val > 0 && val <= max)
            {
                Database.CreatePayment(ordersId, moneyId, val);
                Hide();
            }
            else
                MessageBox.Show($"Введите число от 0 до {max}, которая будет списана.");
        }
    }
}
