﻿//using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;
//using AutoLotModel;
//using System.AutoLotEntitiesModel;
using System.Data;

namespace Pop_MihaelaMaria_Lab6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing
    }
    public partial class MainWindow : Window
    {
        //using AutoLotModel;
        ActionState action = ActionState.Nothing;
        AutoLotEntitiesModel ctx = new AutoLotEntitiesModel();
        
        
        CollectionViewSource customerViewSource;
        Binding txtFirstNameBinding = new Binding();
        Binding txtLastNameBinding = new Binding();

        CollectionViewSource inventoryViewSource;
        Binding txtColorBinding = new Binding();
        Binding txtMakeBinding = new Binding();


        CollectionViewSource customerOrdersViewSource;
        Binding txtCarIdBinding = new Binding();
        Binding txtCustIdBinding = new Binding();
        Binding lastNameValidationBinding = new Binding();





        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            txtFirstNameBinding.Path = new PropertyPath("FirstName");
            txtLastNameBinding.Path = new PropertyPath("LastName");
            //firstNameTextBox.SetBinding(TextBox.TextProperty, txtFirstNameBinding);
            //lastNameTextBox.SetBinding(TextBox.TextProperty, txtLastNameBinding);

            txtColorBinding.Path = new PropertyPath("Color");
            txtMakeBinding.Path = new PropertyPath("Make");
            //colorTextBox.SetBinding(TextBox.TextProperty, txtColorBinding);
            //makeTextBox.SetBinding(TextBox.TextProperty, txtMakeBinding);

            txtCarIdBinding.Path = new PropertyPath("CarId");
            txtCustIdBinding.Path = new PropertyPath("CustId");
            //carIdTextBox.SetBinding(TextBox.TextProperty, txtCarIdBinding);
            //custIdTextBox.SetBinding(TextBox.TextProperty, txtCustIdBinding);

            //lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameValidationBinding);





        }





        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //using System.Data.Entity;
            customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            customerViewSource.Source = ctx.Customers.Local;

            //System.Windows.Data.CollectionViewSource inventoryViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("inventoryViewSource")));
            inventoryViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("inventoryViewSource")));
            inventoryViewSource.Source = ctx.Inventories.Local;

            //pasu 31 din lab 6-7
            customerOrdersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerOrdersViewSource")));
            //customerOrdersViewSource.Source = ctx.Orders.Local;



            ctx.Customers.Load();
            ctx.Inventories.Load();
            ctx.Orders.Load();

            cmbCustomers.ItemsSource = ctx.Customers.Local;
            //cmbCustomers.DisplayMemberPath = "FirstName";
            cmbCustomers.SelectedValuePath = "CustId";

            cmbInventory.ItemsSource = ctx.Inventories.Local;
            //cmbInventory.DisplayMemberPath = "Make";
            cmbInventory.SelectedValuePath = "CarId";
            //pana aici


            BindDataGrid(); //pasu 38 din lab




           

        }


        private void BindDataGrid()
        {
            var queryOrder = from ord in ctx.Orders
                             join cust in ctx.Customers on ord.CustId equals
                             cust.CustId
                             join inv in ctx.Inventories on ord.CarId
                 equals inv.CarId
                             select new
                             {
                                 ord.OrderId,
                                 ord.CarId,
                                 ord.CustId,
                                 cust.FirstName,
                                 cust.LastName,
                                 inv.Make,
                                 inv.Color
                             };
            customerOrdersViewSource.Source = queryOrder.ToList();
        }


        private void SetValidationBinding()
        {
            Binding firstNameValidationBinding = new Binding();
            firstNameValidationBinding.Source = customerViewSource;
            firstNameValidationBinding.Path = new PropertyPath("FirstName");
            firstNameValidationBinding.NotifyOnValidationError = true;
            firstNameValidationBinding.Mode = BindingMode.TwoWay;
            firstNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string required
            firstNameValidationBinding.ValidationRules.Add(new StringNotEmpty());
            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameValidationBinding);
            Binding lastNameValidationBinding = new Binding();
            lastNameValidationBinding.Source = customerViewSource;
            lastNameValidationBinding.Path = new PropertyPath("LastName");
            lastNameValidationBinding.NotifyOnValidationError = true;
            lastNameValidationBinding.Mode = BindingMode.TwoWay;
            lastNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string min length validator
            lastNameValidationBinding.ValidationRules.Add(new StringMinLengthValidator());
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameValidationBinding); //setare binding nou
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantiem Customer entity
                    customer = new Customer()
                    {
                        FirstName = firstNameTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim()
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Customers.Add(customer);
                    customerViewSource.View.Refresh();
                    //salvam modificaril
                    ctx.SaveChanges();
                }
                //using System.Data;
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }



                //am adaugat eu din labu 5
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;

                customerDataGrid.IsEnabled = true;

                btnPrevious.IsEnabled = true;
                btnNext.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
                //pana aici

            }
            else
        if (action == ActionState.Edit)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    customer.FirstName = firstNameTextBox.Text.Trim();
                    customer.LastName = lastNameTextBox.Text.Trim();
                    //salvam modificarile
                    ctx.SaveChanges();

                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();
                // pozitionarea pe item-ul curent
                customerViewSource.View.MoveCurrentTo(customer);


//am adaugat eu din labu 5
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;

                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;

                customerDataGrid.IsEnabled = true;

                btnPrevious.IsEnabled = true;
                btnNext.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;               
                firstNameTextBox.SetBinding(TextBox.TextProperty, txtFirstNameBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, txtLastNameBinding);
                
                
            

            }
     else
            if (action == ActionState.Delete)
                {
                 try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    ctx.Customers.Remove(customer);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                   MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();

 //am adaugat eu din labu 5
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;

                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                
                customerDataGrid.IsEnabled = true;
                
                btnPrevious.IsEnabled = true;
                btnNext.IsEnabled = true;
                
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
                firstNameTextBox.SetBinding(TextBox.TextProperty, txtFirstNameBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, txtLastNameBinding);
            


            }


        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToNext();
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToPrevious();
        }



        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;

            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            customerDataGrid.IsEnabled = false;
            btnPrevious.IsEnabled = false;
            btnNext.IsEnabled = false;

            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;

            
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);

            firstNameTextBox.Text = "";
            lastNameTextBox.Text = "";
            Keyboard.Focus(custIdTextBox);


        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            string fName = firstNameTextBox.Text.ToString();//modificat de mn
            string lName = lastNameTextBox.Text.ToString();//modificat de mn

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;

            customerDataGrid.IsEnabled = false; //modificat de mn

            btnPrevious.IsEnabled = false;
            btnNext.IsEnabled = false;

            firstNameTextBox.IsEnabled = true;//modificat de mn
            lastNameTextBox.IsEnabled = true;//modificat de mn

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);//modificat de mn
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);//modificat de mn

            SetValidationBinding();

            firstNameTextBox.Text = fName;//modificat de mn
            lastNameTextBox.Text = lName;//modificat de mn
            Keyboard.Focus(custIdTextBox);//modificat de mn
        }


        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            string fName = firstNameTextBox.Text.ToString();
            string lName = lastNameTextBox.Text.ToString();

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;

            customerDataGrid.IsEnabled = false;

            btnPrevious.IsEnabled = false;
            btnNext.IsEnabled = false;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);

            firstNameTextBox.Text = fName;
            lastNameTextBox.Text = lName;

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNew.IsEnabled = true;
            btnEdit.IsEnabled = true;
            btnEdit.IsEnabled = true;
            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;

            customerDataGrid.IsEnabled = true;

            btnPrevious.IsEnabled = true;
            btnNext.IsEnabled = true;

            firstNameTextBox.IsEnabled = false;
            lastNameTextBox.IsEnabled = false;

            firstNameTextBox.SetBinding(TextBox.TextProperty, txtFirstNameBinding);
            lastNameTextBox.SetBinding(TextBox.TextProperty, txtLastNameBinding);

        }







        /*ASTA I PT INVENTORY */


        private void btnSave1_Click(object sender, RoutedEventArgs e)
        {
            Inventory inventory = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantiem Inventory entity
                    inventory = new Inventory()
                    {
                        Color = colorTextBox.Text.Trim(),
                        Make = makeTextBox.Text.Trim()
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Inventories.Add(inventory);
                    inventoryViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();

                }
                //using System.Data;
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }


                //am adaugat eu din labu 5
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;

                inventoryDataGrid.IsEnabled = true;

                btnPrevious.IsEnabled = true;
                btnNext.IsEnabled = true;
                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;
                //pana aici



            }
            else
        if (action == ActionState.Edit)
            {
                try
                {
                    

                    inventory = (Inventory)inventoryDataGrid.SelectedItem;
                    inventory.Color = colorTextBox.Text.Trim();
                    inventory.Make = makeTextBox.Text.Trim();
                    //salvam modificarile
                    ctx.SaveChanges();


                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                inventoryViewSource.View.Refresh();
                // pozitionarea pe item-ul curent
                inventoryViewSource.View.MoveCurrentTo(inventory);


 //am adaugat eu din labu 5
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;

                inventoryDataGrid.IsEnabled = true;
                
                btnPrevious.IsEnabled = true;
                btnNext.IsEnabled = true;
                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;
                colorTextBox.SetBinding(TextBox.TextProperty, txtColorBinding);
                makeTextBox.SetBinding(TextBox.TextProperty, txtMakeBinding);


            }
            else
            if (action == ActionState.Delete)
            {
                try
                {
                    inventory = (Inventory)inventoryDataGrid.SelectedItem;
                    ctx.Inventories.Remove(inventory);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                inventoryViewSource.View.Refresh();




 //am adaugat eu din labu 5
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;

                inventoryDataGrid.IsEnabled = true;

                btnPrevious.IsEnabled = true;
                btnNext.IsEnabled = true;

                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;
                colorTextBox.SetBinding(TextBox.TextProperty, txtColorBinding);
                makeTextBox.SetBinding(TextBox.TextProperty, txtMakeBinding);


            }
        }



        

        private void btnPrevious1_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToNext();
        }

        private void btnNext1_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNew1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;

            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;

            inventoryDataGrid.IsEnabled = false;

            btnPrevious.IsEnabled = false;
            btnNext.IsEnabled = false;

            colorTextBox.IsEnabled = true;
            makeTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);

            

            colorTextBox.Text = "";
            makeTextBox.Text = "";
            Keyboard.Focus(carIdTextBox);


        }

        private void btnEdit1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            string tempColor = colorTextBox.Text.ToString();//modificat de mn
            string tempMake = makeTextBox.Text.ToString();//modificat de mn

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;

            inventoryDataGrid.IsEnabled = false; //modificat de mn

            btnPrevious.IsEnabled = false;
            btnNext.IsEnabled = false;

            colorTextBox.IsEnabled = true;
            makeTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);

            colorTextBox.Text = tempColor;//modificat de mn
            makeTextBox.Text = tempMake;//modificat de mn
            Keyboard.Focus(carIdTextBox);//modificat de mn
        }

        private void btnDelete1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            string tempColor = firstNameTextBox.Text.ToString();
            string tempMake = lastNameTextBox.Text.ToString();

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;

            inventoryDataGrid.IsEnabled = false;

            btnPrevious.IsEnabled = false;
            btnNext.IsEnabled = false;

            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);

            colorTextBox.Text = tempColor;
            makeTextBox.Text = tempMake;


        }

        private void btnCancel1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNew.IsEnabled = true;
            btnEdit.IsEnabled = true;
            btnEdit.IsEnabled = true;
            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;

            inventoryDataGrid.IsEnabled = true;

            btnPrevious.IsEnabled = true;
            btnNext.IsEnabled = true;

            colorTextBox.IsEnabled = false;
            makeTextBox.IsEnabled = false;

            colorTextBox.SetBinding(TextBox.TextProperty, txtColorBinding);
            makeTextBox.SetBinding(TextBox.TextProperty, txtMakeBinding);
        }






        /*ASTA I PT ORDER */


       


        private void btnSave2_Click(object sender, RoutedEventArgs e)
        {
            Order order = null;
            if (action == ActionState.New)
            {
                try
                {
                    Customer customer = (Customer)cmbCustomers.SelectedItem;
                    Inventory inventory = (Inventory)cmbInventory.SelectedItem;
                    //instantiem Order entity
                    order = new Order()
                    {

                        CustId = customer.CustId,
                        CarId = inventory.CarId
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Orders.Add(order);
                    customerOrdersViewSource.View.Refresh();
                    //salvam modificaril
                    ctx.SaveChanges();
                }
                //using System.Data;
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }



                //am adaugat eu din labu 5
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;

                ordersDataGrid.IsEnabled = true;

                btnPrevious.IsEnabled = true;
                btnNext.IsEnabled = true;

                carIdTextBox.IsEnabled = false;
                custIdTextBox.IsEnabled = false;
                //pana aici

            }
            else
        if (action == ActionState.Edit)
            {
                dynamic selectedOrder = ordersDataGrid.SelectedItem;
                try
                {
                    int curr_id = selectedOrder.OrderId;
                    var editedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (editedOrder != null)
                    {
                        editedOrder.CustId = Int32.Parse(cmbCustomers.SelectedValue.ToString());
                        editedOrder.CarId = Convert.ToInt32(cmbInventory.SelectedValue.ToString());
                        //salvam modificarile
                        ctx.SaveChanges();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                BindDataGrid();
                // pozitionarea pe item-ul curent
                customerViewSource.View.MoveCurrentTo(selectedOrder);
            }
            
            else if (action == ActionState.Delete)
            {
                try
                {
                    dynamic selectedOrder = ordersDataGrid.SelectedItem;
                    int curr_id = selectedOrder.OrderId;
                    var deletedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (deletedOrder != null)
                    {
                        ctx.Orders.Remove(deletedOrder);
                        ctx.SaveChanges();
                        MessageBox.Show("Order Deleted Successfully", "Message");
                        BindDataGrid();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }










      










    }
}
