﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SystemZatzadzaniaZamowieniamiKlijenta_RESTAURACJA
{
    public partial class ChoosingMethodPayment : Form
    {
        private List<Klient> clientListOK;
        private List<Adresy> customerAddressListOK;
        public ChoosingMethodPayment(List<Klient> clientList, List<Adresy> customerAddressList)
        {
            clientListOK = clientList;
            customerAddressListOK = customerAddressList;
            InitializeComponent();
        }
        //public ChoosingMethodPayment()
        //{
        //    InitializeComponent();
        //}

        private void button2_Click(object sender, EventArgs e)
        {   //wywołanie STRONY GŁOWNEJ
            Home openForm = new Home();
            openForm.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {   //REZYGNACJA z zamówienia
            DialogResult result = MessageBox.Show("Czy na pewno chcesz zrezygnować z zamówienie?", "Confirmation", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                MessageBox.Show("Płatność anulowana.");
                //ZEROWANIE KOSZYKA?
                this.Hide();
                Home openForm = new Home();
                openForm.ShowDialog();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {//Blik
                blikPayment openForm = new blikPayment();
                openForm.ShowDialog();
            }
            else if (radioButton2.Checked)
            {//GOTÓWKA
                OrderStatusTrue openForm = new OrderStatusTrue();
                openForm.ShowDialog();
            }
            else if (radioButton3.Checked)
            {//KARTA PLATNICZA
                cashPayment openForm = new cashPayment();
                openForm.ShowDialog();
            }
            else
            {//brak zaznaczonej płatności
                DialogResult result = MessageBox.Show("Musisz wybrać petodę płatności!", "Confirmation", MessageBoxButtons.YesNo);
            }




            foreach (Klient client in clientListOK)
            {
                //DODANIE DO BAZY DANYCH
                string connectionString = ConfigurationManager.ConnectionStrings["Restaurant"].ConnectionString;
                SqlConnection cnn = new SqlConnection(connectionString);
                cnn.Open();
                //dodawanie do bazy
                SqlDataAdapter sqlKlient = new SqlDataAdapter("INSERT INTO Klient (idKlient, imie, nazwisko, email, nrtelefonu) VALUES(@id, @imie, @nazwisko, @email, @nrtelefonu)", cnn);
                string sqlKlient2 = "SELECT COUNT(*), MAX([id]) FROM Klient";
                SqlCommand cmd1 = new SqlCommand(sqlKlient2, cnn);
                SqlDataReader dataReader = cmd1.ExecuteReader();
                int output1 = 0;
                while (dataReader.Read())
                {
                    if ((int)dataReader.GetValue(0) != 0)
                    {
                        output1 = Convert.ToInt32(dataReader.GetValue(1)) + 1;
                    }
                }
                cmd1.Cancel();
                dataReader.Close();

                SqlCommand cmd = new SqlCommand(sqlKlient.ToString(), cnn);
                cmd.Parameters.Add("@idKlient", SqlDbType.Int);
                cmd.Parameters["@idKlient"].Value = output1;
                cmd.Parameters.Add("@imie", SqlDbType.NChar);
                cmd.Parameters["@status"].Value = client.Imie;
                cmd.Parameters.Add("@nazwisko", SqlDbType.NChar);
                cmd.Parameters["@nazwisko"].Value = client.Nazwisko;
                cmd.Parameters.Add("@email", SqlDbType.NChar);
                cmd.Parameters["@email"].Value = client.Email;
                cmd.Parameters.Add("@nrtelefonu", SqlDbType.NChar);
                cmd.Parameters["@nrtelefonu"].Value = client.Nrtelefonu;
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                foreach (Adresy adresy in customerAddressListOK)
                {
                    string connectionString2 = ConfigurationManager.ConnectionStrings["Restaurant"].ConnectionString;
                    SqlConnection cnn2 = new SqlConnection(connectionString2);
                    cnn2.Open();
                    //dodawanie do bazy
                    SqlDataAdapter sqlAdresy = new SqlDataAdapter("INSERT INTO Adresy (idAdresy, idKlient, ulica, numerDomu, numerMieszkania, kodPocztowy, miasto) VALUES (NULL, imie, nazwisko, email, nrtelefonu)", cnn2);
                    string sqlAdresy2 = "SELECT COUNT(*), MAX([id]) FROM Adresy";
                    SqlCommand cmd4 = new SqlCommand(sqlAdresy2, cnn2);
                    SqlDataReader dataReader2 = cmd4.ExecuteReader();
                    int output2 = 0;
                    while (dataReader2.Read())
                    {
                        if ((int)dataReader2.GetValue(0) != 0)
                        {
                            output2 = Convert.ToInt32(dataReader2.GetValue(1)) + 1;
                        }
                    }
                    cmd1.Cancel();
                    dataReader2.Close();

                    SqlCommand cmd3 = new SqlCommand(sqlAdresy.ToString(), cnn2);
                    cmd3.Parameters.Add("@idAdresy", SqlDbType.Int);
                    cmd3.Parameters["@idAdresy"].Value = output2;

                    cmd3.Parameters.Add("idKlient", SqlDbType.Int);
                    cmd3.Parameters["@idKlient"].Value = output1;

                    cmd3.Parameters.Add("@ulica", SqlDbType.VarChar);
                    cmd3.Parameters["@ulica"].Value = adresy.Ulica;

                    cmd3.Parameters.Add("@numerDomu", SqlDbType.VarChar);
                    cmd3.Parameters["@numerDomu"].Value = adresy.NumerDomu;

                    cmd3.Parameters.Add("@numerMieszkania", SqlDbType.VarChar);
                    cmd3.Parameters["@numerMieszkania"].Value = adresy.NumerMieszkania;

                    cmd3.Parameters.Add("@id_Seat", SqlDbType.VarChar);
                    cmd3.Parameters["@id_Seat"].Value = adresy.KodPocztowy;

                    cmd3.Parameters.Add("@kodPocztowy", SqlDbType.VarChar);
                    cmd3.Parameters["@kodPocztowy"].Value = adresy.Miasto;
                    cmd3.ExecuteNonQuery();
                    cmd3.Dispose();

                }
                cnn.Close();
            }


        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void ChoosingMethodPayment_Load(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {   //BLIK
            label2.Text = "Sposób płatności: Płatność BLIK";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {   //GOTOWKA
            label2.Text = "Sposób płatności: Płatność gotówką";
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {   //KARTA PLATNICZA
            label2.Text = "Sposób płatności: Kartą płatniczą";
        }
    }
}
