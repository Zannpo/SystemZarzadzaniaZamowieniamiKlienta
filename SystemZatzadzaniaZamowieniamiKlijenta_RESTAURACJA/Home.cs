﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SystemZatzadzaniaZamowieniamiKlijenta_RESTAURACJA
{
    public partial class Home : Form
    {
        List<Danie> listOfTheDishes = new List<Danie>();
        List<PozycjaZamowienia> orderItemList = new List<PozycjaZamowienia>();
        decimal totalPrice = 0, priceOfTheDish = 0, delivery = 7, sumPrice=0;
        string chosenDish;
        static string connectionString = ConfigurationManager.ConnectionStrings["Restaurant"].ConnectionString;
        SqlConnection cnn = new SqlConnection(connectionString);
        public Home()
        {
            InitializeComponent();
            refresh();
            setDGVWidth();
            timerStatus();

            //Przypisanie wartości kwoty całkowitej
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                totalPrice = decimal.Parse(textBox1.Text);
            }
            textBox2.Text = "7";
                        
            deleteOldClients();
        }

        void deleteOldClients()
        {   //Usuwanie starszych zamówień niż 3 miesiące
            //Kaskadowo również klientów i pozycji zamówienia      
            cnn.Open();
            try
            {
                SqlCommand query = new SqlCommand("DELETE FROM Zamowienie WHERE statusZamowienia = 'dostarczone' AND DATEDIFF(DD, dataZamowienia, GETDATE()) > 90", cnn);
                query.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, " \n" + ex.StackTrace);               
            }
            cnn.Close();
        }

        public decimal setTotalPrice(decimal totalPrice)
        {
            decimal newTotalPrice = 0;

            newTotalPrice = totalPrice * 0.90m;
            textBox3.Text = newTotalPrice.ToString();
            MessageBox.Show("Gratulujemy! Załapałeś się na promocję! \n Twój koszt całkowity to: " + newTotalPrice.ToString());
            return newTotalPrice;
        }

        void refresh()
        {           
            //cnn.Open();
            dataGridView1.Rows.Clear();
            //Wyświetlenie całej listy menu po uruchomieniu formatki 
            SqlDataAdapter sql = new SqlDataAdapter("SELECT idDanie, nazwaDania, cenaDania, skladniki FROM Danie ", cnn);
            DataTable dishes = new DataTable();
            sql.Fill(dishes);
            dataGridView1.DataSource = dishes;
            if (dataGridView1.Rows.Count == 1 && dataGridView1.Rows != null)
            {
                MessageBox.Show("Aktualnie nie posiadamy żadnych dań w ofercie!");
            }
            dataGridView1.ClearSelection();
            //cnn.Close();
        }

        void refreshSale(decimal totalPrice)
        {//Pobranie aktualnego dnia
            var today = DateTime.Now.Date;
            
            cnn.Open();
            if (totalPrice > 200)
            {
                SqlDataAdapter sql = new SqlDataAdapter("SELECT nazwaPromocji FROM Promocja WHERE idPromocja = 1 ", cnn);
                DataTable sale = new DataTable();
                sql.Fill(sale);
                dataGridView3.DataSource = sale;
            }
            else if (today.DayOfWeek.ToString() == "Monday")
            {
                SqlDataAdapter sql = new SqlDataAdapter("SELECT nazwaPromocji FROM Promocja WHERE idPromocja = 2 ", cnn);
                DataTable sale = new DataTable();
                sql.Fill(sale);
                dataGridView3.DataSource = sale;
            }
            dataGridView3.ClearSelection();
            cnn.Close();
        }

        void setDGVWidth()
        {
            //Ustawienie szerokości
            if (dataGridView1.Rows.Count > 1)
            {
                dataGridView1.Columns[0].Width = 20;
                dataGridView1.Columns[1].Width = 100;
                dataGridView1.Columns[2].Width = 70;
                dataGridView1.Columns[3].Width = 270;
            }

            if (dataGridView3.Rows.Count > 1)
            {
                dataGridView3.Columns[0].Width = 140;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {}

        void timerStatus()
        {
            int hour = DateTime.Now.Hour;
            if (hour >= 12 && hour < 21)
            {   //Otwarte od 12:00am do 8:59pm
                LabelStatus.Text = "Restauracja OTWARTA";
            }
            else
            {   //Zamknięte 5:00pm through 9:59am
                LabelStatus.Text = "restauracja ZAMKNIĘTA";
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            decimal salePrice = 0;
            var today = DateTime.Now.Date;
           

            if (!string.IsNullOrEmpty(textBox3.Text))
            {
                salePrice = decimal.Parse(textBox3.Text);
                sumPrice = decimal.Parse(textBox3.Text);
            }

            //sprawdzenie godziny
            int hour = DateTime.Now.Hour;
            if (hour >= 9 && hour < 21)
            {
                if (totalPrice > 20)
                {
                    //Sprawdzenie czy poniedziałek i naliczenie zniżki
                    if (today.DayOfWeek.ToString() == "Monday")
                    {
                        setTotalPrice(salePrice);
                        totalPrice = decimal.Parse(textBox3.Text);
                    }

                    if (totalPrice > 200)
                    {                       
                        MessageBox.Show("Gratuluje twoja dostawa będzie darmowa!");
                    }
                   

                    try
                    {   //Przejście do koszyka i wrzucenie listy zamówień
                        OrderCart openForm = new OrderCart(listOfTheDishes, orderItemList, totalPrice, delivery, sumPrice);
                        openForm.ShowDialog();
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message, " \r\n" + ex.StackTrace);
                    }
                }
                else
                {
                    MessageBox.Show("Nie przyjmujemy zamówień poniżej 20 zł!");
                }
            }
            else
            {
                MessageBox.Show("Przykro nam, ale restauracja jest zamknięta! Zapraszamy codziennie w godzinach: 12-21");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {   //wywołanie KONTAKTÓW do restauracji
            RestaurantContactDetails openForm = new RestaurantContactDetails();
            openForm.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {   //wywołanie AKTUALNYCH PROMOCJI
            Sale openForm = new Sale();
            openForm.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cnn.Open();
            //wyświetlenie całej listy MENU
            SqlDataAdapter sql = new SqlDataAdapter("SELECT idDanie,nazwaDania, cenaDania, skladniki FROM Danie ", cnn);
            DataTable dishes = new DataTable();
            sql.Fill(dishes);
            dataGridView1.DataSource = dishes;

            if (dataGridView1.Rows.Count == 1 && dataGridView1.Rows != null)
            {
                MessageBox.Show("Aktualnie nie posiadamy żadnych dań w ofercie!");
            }
            dataGridView1.ClearSelection();
            cnn.Close();
        }

        private void button8_Click_1(object sender, EventArgs e)
        {   //DODAWANIE do zamówienia
            Danie dish = new Danie();
            PozycjaZamowienia orderItem = new PozycjaZamowienia();
            int id, amount = 0;
            decimal priceAfterAdding = 0, priceFromTextBox =0;

            if (dataGridView1.SelectedRows.Count > 0)
            {
                if (dataGridView1.SelectedCells[0].Value == null)
                {
                    MessageBox.Show("Nie można dodać nieistniejącej pozycji!");
                }
                else
                {   //Pobranie danych dania wybranego przez klienta
                    id = (int)dataGridView1.SelectedCells[0].Value;
                    chosenDish = (string)dataGridView1.SelectedCells[1].Value;
                    priceOfTheDish = (decimal)dataGridView1.SelectedCells[2].Value;
                    amount = (int)numericUpDown1.Value;

                    dish.nazwaDania = chosenDish.ToString();
                    dish.cenaDania = priceOfTheDish;
                    dish.idDanie = id;
                    orderItem.idDania = id;
                    orderItem.iloscKonkretnegoDania = amount;

                    listOfTheDishes.Add(dish);
                    orderItemList.Add(orderItem);

                    //Dodanie pozycji do koszyka
                    dataGridView2.Rows.Add(dish.nazwaDania, dish.cenaDania, amount);

                    //Pobranie ceny z textbox
                    if (!string.IsNullOrEmpty(textBox1.Text))
                    {
                        priceFromTextBox = decimal.Parse(textBox1.Text);
                    }

                    //Zliczanie jednostkowej ceny całkowitej
                    priceAfterAdding = dish.cenaDania * orderItem.iloscKonkretnegoDania;
                    totalPrice = priceFromTextBox + priceAfterAdding;
                    textBox1.Text = totalPrice.ToString();
                    refreshSale(totalPrice);
                  

                    decimal newTotalPrice = Convert.ToDecimal(textBox1.Text);
                    if(newTotalPrice > 200)
                    {
                        delivery = 0;
                    }
                    textBox2.Text = delivery.ToString();
                    decimal sumPrice = newTotalPrice + delivery;
                    textBox3.Text = sumPrice.ToString();


                }

                //Zliczanie jednostkowej ceny całkowitej
                priceAfterAdding = dish.cenaDania * orderItem.iloscKonkretnegoDania;
                totalPrice = priceFromTextBox + priceAfterAdding;
                textBox1.Text = totalPrice.ToString();
                refreshSale(totalPrice);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                totalPrice = decimal.Parse(textBox1.Text);
            }
            int amount = 0, amountToDelete = 0, index = 0;

            if (dataGridView2.SelectedRows.Count > 0)
            {   //Pobranie danych dania wybranego przez klienta
                if (dataGridView2.SelectedCells[0].Value == null)
                {
                    MessageBox.Show("Próbujesz usunąć pustą pozycję!");
                }
                else
                {   //Pobranie danych dania wybranego przez klienta
                    try
                    {
                        try
                        {
                            chosenDish = (string)dataGridView2.SelectedCells[0].Value;
                            priceOfTheDish = (decimal)dataGridView2.SelectedCells[1].Value;
                            amount = (int)dataGridView2.SelectedCells[2].Value;
                            amountToDelete = (int)numericUpDown1.Value;
                            amount = amount - amountToDelete;

                            dataGridView2.SelectedCells[2].Value = amount;
                            decimal priceAfterDeleting = 0, newTotalPrice = 0;
                            priceAfterDeleting = priceOfTheDish * amountToDelete;
                            newTotalPrice = totalPrice - priceAfterDeleting;
                            textBox1.Text = newTotalPrice.ToString();
                           
                            decimal newTotalPrice2 = Convert.ToDecimal(textBox1.Text);
                            if (newTotalPrice2 > 200)
                            {
                                delivery = 0;
                            }
                            textBox2.Text = delivery.ToString();
                            decimal sumPrice = newTotalPrice2 + delivery;
                            textBox3.Text = sumPrice.ToString();

                            //Usunięcie wybranego dania z listy
                            for (int i = 0; i < listOfTheDishes.Count; i++)
                            {
                                index = listOfTheDishes.FindIndex(danie => danie.nazwaDania == chosenDish);
                                newTotalPrice = listOfTheDishes[index].cenaDania;
                                amount = orderItemList[index].iloscKonkretnegoDania;
                                listOfTheDishes.RemoveAt(index);
                                orderItemList.RemoveAt(index);

                                //Usunięcie dania z koszyka
                                try
                                {
                                    int restAmount = (int)dataGridView2.SelectedCells[2].Value;
                                        if (restAmount == 0)
                                        {
                                            dataGridView2.Rows.RemoveAt(dataGridView2.SelectedRows[0].Index);
                                        }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Wybrano pustą pozycję!");
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Wybrano pustą pozycję!");
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {   //Wyświetlenie listy przystawek
            cnn.Open();
            SqlDataAdapter sql = new SqlDataAdapter("SELECT idDanie, nazwaDania, cenaDania, skladniki FROM Danie INNER JOIN TypDania ON Danie.idTypDania = TypDania.idTypDania WHERE TypDania.nazwa = 'Przystawka'", cnn);
            DataTable dishes = new DataTable();
            sql.Fill(dishes);
            dataGridView1.DataSource = dishes;
            dataGridView1.ClearSelection();
            if (dataGridView1.Rows.Count == 1 && dataGridView1.Rows != null)
            {
                MessageBox.Show("Aktualnie nie posiadamy żadnych przystawek w ofercie!");
            }
            cnn.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            sumPrice = Convert.ToDecimal(textBox3.Text);
            OrderCart openForm = new OrderCart(listOfTheDishes, orderItemList, totalPrice, delivery, sumPrice);
            openForm.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            cnn.Open();
            //Wyświetlenie listy ramenów            
            SqlDataAdapter sql = new SqlDataAdapter("SELECT idDanie, nazwaDania, cenaDania, skladniki FROM Danie INNER JOIN TypDania ON Danie.idTypDania = TypDania.idTypDania WHERE TypDania.nazwa = 'Ramen'", cnn);
            DataTable dishes = new DataTable();
            sql.Fill(dishes);
            dataGridView1.DataSource = dishes;

            if (dataGridView1.Rows.Count == 1 && dataGridView1.Rows != null)
            {
                MessageBox.Show("Aktualnie nie posiadamy żadnych ramenów w ofercie!");
            }
            dataGridView1.ClearSelection();
            cnn.Close();
        }
    }
}
