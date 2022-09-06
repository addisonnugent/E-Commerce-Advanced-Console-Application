using System;
using Library.ECommerceApp;
using Library.ECommerceApp.Models;
using Library.ECommerceApp.Services;
using Library.ECommerceApp.Utility;
using System.Linq;
using Newtonsoft.Json;

namespace ECommerceApp 
{
    internal class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Ecommerce App: ");

            var Inventory = ProductServices.Current;    // set up Inventory list of products
            var ShoppingCart = CartService.Current;     // set up Shopping Cart list of products

            Console.WriteLine("1. Customer");
            Console.WriteLine("2. Employee");
            Console.Write("Enter choice: ");


            var usertype = int.Parse(Console.ReadLine() ?? "0");

            Inventory.Load("SaveData.json");    // Loads Existing Data if User Saved
            //ShoppingCart.Load("SaveCart.json");
            bool cont = true;   // if true Menu loop continues
            bool customer = true;
            if (usertype != 1) { customer = false; }

            // --------- EMPLOYEE - Inventory Menu (OPTION #2) ---------
            if (!customer)
            {
                while (cont)
                {
                    Console.WriteLine("\n");
                    var action = InventoryPrintMenu();  // print menu
                    Console.WriteLine("\n");

                    if (action == ActionType.Create)
                    {
                        // 1) CREATE - creates new product in inventory list
                        // ---- Product by weight and quality ----
                        
                        Console.WriteLine("How would you like to Price Products?");
                        Console.WriteLine("1. Price by Weight");
                        Console.WriteLine("2. Price by Quantity");
                        Console.Write("Enter choice: ");
                        var pricetype = int.Parse(Console.ReadLine() ?? "0");
                        Console.WriteLine("You chose to Add an Product.");
                        Product? newProduct = null;
                        
                        if (pricetype == 1)  // Weight 
                        {
                            newProduct = new ProductByWeight();
                        }
                        else    // Quantity
                        {
                            newProduct = new ProductByQuantity();
                        }
                        FillProduct(newProduct);
                        Inventory.Create(newProduct);
                    }
                    else if (action == ActionType.ListInventory) {
                        // 2) PRINT INVENTORY
                        PrintInventory(Inventory);
                    }
                    else if (action == ActionType.Update) {
                        // 3) Update - updates quantity/name/description of existing product in Inventory

                        Console.Write("Enter ID of Product you would like to update: ");
                        var choice = int.Parse(Console.ReadLine() ?? "0");

                        var prod = Inventory.Products.FirstOrDefault(p => p.Id == choice);
                        if (prod != null) {
                            var p = UpdateProduct(prod);
                            Inventory.Update(p);
                        }
                        else {
                            Console.Write("No Product with that ID.");
                        }
                    }
                    else if (action == ActionType.Delete)
                    {
                        // 4) Delete - Delete Product from inventory

                        Console.WriteLine("Enter Id of Product you would like to Delete: ");
                        var id = int.Parse(Console.ReadLine() ?? "0");
                        Inventory.Delete(id);
                    }
                    else if (action == ActionType.Search)
                    {
                        SearchInventory(Inventory);
                    }
                    else if (action == ActionType.Save)
                    {
                        // 6) Save - saves data using Json to "SaveData.json"
                        Inventory.Save("SaveData.json");
                    }
                    else if (action == ActionType.Load)
                    {
                        // 7) Load - Inventory by read text from text file
                        Inventory.Load("SaveData.json");
                    }
                    else if (action == ActionType.Exit)
                    {
                        // 8) - EXIT LOOP

                        Console.WriteLine("Exiting.");
                        cont = false;
                    }
                }// END WHILE LOOP - Inventory
            }
            else
            {
                // --------- CUSTOMER - Customer Menu (OPTION #1) ---------

                while (cont)
                {
                    Console.WriteLine("\n");
                    var action = CustomerPrintMenu();
                    Console.WriteLine("\n");

                    if (action == ActionType.Add)
                    {
                        // 1. Add Item to Cart

                        Console.WriteLine("\n\nAdd Item to Cart\n");
                        Console.Write("Enter Product Id: ");
                        var id = int.Parse(Console.ReadLine() ?? "0");

                        ShoppingCart.Add(id);   // takes away from inventory 
                    }
                    else if (action == ActionType.Remove)
                    {
                        // 2. Remove from cart

                        Console.WriteLine("\n\nDelete Item from Cart");
                        Console.Write("Enter Product Id: ");
                        var id = int.Parse(Console.ReadLine() ?? "0");

                        ShoppingCart.Remove(id);    // adds back to inventory

                    }
                    else if (action == ActionType.ListInventory)
                    {
                        PrintInventory(Inventory);
                    }
                    else if (action == ActionType.ShowCart)
                    {
                        // 4. List Current Shopping Cart
                        PrintCart(ShoppingCart);

                    }
                    else if (action == ActionType.Search)
                    {
                        // 5 - Search

                        Console.WriteLine("Where would you like to search:");
                        Console.WriteLine("1. Shopping Cart");
                        Console.WriteLine("2. Inventory");
                        Console.WriteLine("Type choice: ");
                        var choice = int.Parse(Console.ReadLine() ?? "0");

                        bool found = false;

                        if (choice == 1) // Search shopping Cart
                        {
                            Console.Write("Enter Name or Description of Item: ");
                            string nameDes = Console.ReadLine() ?? string.Empty;

                            Console.WriteLine("Search found in Cart: \n");

                            for (int i = 0; i < ShoppingCart.Cart.Count; i++)
                            {
                                var hold = ShoppingCart.Cart[i];
                                if (hold.Name != null && hold.Description != null)
                                {
                                    if (hold.Name.Contains(nameDes))    // checks Name
                                    {
                                        Console.WriteLine($"{hold}");
                                        found = true;
                                    }
                                    else if (hold.Description.Contains(nameDes))    // checks Description
                                    {
                                        Console.WriteLine($"{hold}");
                                        found = true;
                                    }
                                }
                            }
                            if (found == false)  // Nothing found matching
                            {
                                Console.WriteLine("No products dound matching that Name or Description. \n");
                            }
                        }
                        else // Search Inventory
                        {
                            SearchInventory(Inventory);
                        }

                    }
                    else if (action == ActionType.Checkout)
                    {
                        // 5) CHECKOUT - checks out customer then exits program
                        ShoppingCart.Checkout();
                        cont = false;
                    }
                    else if (action == ActionType.Exit)
                    {
                        // 6 - EXIT LOOP

                        //Saves Inventory & Cart
                        Inventory.Save("SaveData.json");

                        Console.Write("Would you like to Save Cart?\n1) Yes\n2) No\nEnter choice: ");

                        var answer = int.Parse(Console.ReadLine() ?? "0");
                        if (answer == 1)
                        {
                            ShoppingCart.Save("SaveCart.json");
                        }
                        else
                        {
                            File.WriteAllText("SaveCart.json", null);
                        }
                        Console.WriteLine("Exiting.");
                        cont = false;
                    }

                }// END WHILE LOOP

            }
            Console.WriteLine("Thank you for Shopping with us!");

        }


        // -------------------------------- SEARCH --------------------------------

        private static void SearchInventory(ProductServices inventory)
        {
            bool found = false;

            Console.Write("Enter Name or Description of Item: ");
            string nameDes = Console.ReadLine() ?? string.Empty;
            if (nameDes != null)
            {
                Console.WriteLine("Searching Inventory... \n\n ");
                Console.WriteLine($"-----------------------------------------------------------------------------");
                Console.WriteLine($"-----------------------------  Inventory Search -----------------------------");
                Console.WriteLine($"-----------------------------------------------------------------------------");
                Console.WriteLine($"\tID#\tName\tPrice"); 

                for (int i = 0; i < inventory.Products.Count; i++)
                {
                    var hold = inventory.Products[i];
                    if (hold.Name!= null && hold.Description != null) {
                        if (hold.Name.Contains(nameDes))    // checks Name
                        {
                            Console.WriteLine($"{hold}");
                            found = true;
                        }
                        else if (hold.Description.Contains(nameDes))    // checks Description
                        {
                            Console.WriteLine($"{hold}");
                            found = true;
                        }
                    }
                }

                if (found == false)  // Nothing found matching
                {
                    Console.WriteLine("\nNo products found matching that Name or Description. \n");
                }
            }
            else {
                Console.WriteLine("\nNo products found matching that Name or Description. \n");
            }
        }


        private static void FillProduct(Product? product)
        {
            // FillProduct - gets all compoents from user from new Product
            if (product == null)
            {
                return;
            }
            
            // -------------- Name -----------------
            Console.Write("\nEnter New Product Name: ");
            product.Name = Console.ReadLine() ?? string.Empty;

            // ------------ Description ------------
            Console.Write("\nEnter Description of Product: ");
            product.Description = Console.ReadLine() ?? string.Empty;

            // DERIVED CLASS (Weight & Quantity)
            // --------------- Price ---------------
            if(product is ProductByQuantity)
            {
                var productbyquantity = product as ProductByQuantity;
                if (productbyquantity != null) {
                    Console.Write("\nNumber of Units Avalible: ");
                    productbyquantity.Quantity = int.Parse(Console.ReadLine() ?? "0");
                    
                    Console.Write("\nPrice Per Unit: ");
                    productbyquantity.Price = double.Parse(Console.ReadLine() ?? "0");
                    product.priceinp = productbyquantity.Price;
                    
                    Console.Write("\nWould you like to make Product BOGO:\n1) Yes\n2)No\nEnter choice: ");
                    int bogodeal = int.Parse(Console.ReadLine() ?? "0");
                    
                    if (bogodeal == 1)   // Product is bogo
                        productbyquantity.Bogo = true;  // else already set to false
                }
            }
            else if(product is ProductByWeight){
                var productbyweight = product as ProductByWeight;
                
                if (productbyweight != null) {
                    Console.Write("\nEnter Weight of Product avalible in Pounds: ");
                    productbyweight.Weight = double.Parse(Console.ReadLine() ?? "0.0");
                    Console.Write("\nEnter Price Per Pound: ");
                    productbyweight.Price = double.Parse(Console.ReadLine() ?? "0.0");
                    product.priceinp = productbyweight.Price;
                }
            }
            product.AssignedUser = 1;
        }

        // -------------------------   MENUS  -------------------------

        private static ActionType InventoryPrintMenu()
        {
            // CRUD - Create, Read, Update, & Delete
            Console.WriteLine("------ Employee Menu ------\n");
            Console.WriteLine("1. Create New Product"); // Create
            Console.WriteLine("2. Print Inventory");    // ListInventory
            Console.WriteLine("3. Update Product");     // Update
            Console.WriteLine("4. Delete Product");     // Delete
            Console.WriteLine("5. Search Product");     // Search
            Console.WriteLine("6. Save Products");      // Save
            Console.WriteLine("7. Load Products");      // Load
            Console.WriteLine("8. EXIT\n");             // Exit


            Console.Write("\nEnter choice: ");             // Exit
            var input = int.Parse(Console.ReadLine() ?? "0");

            while(true)
            {
                switch (input)
                {
                    case 1:
                        return ActionType.Create;
                    case 2:
                        return ActionType.ListInventory;
                    case 3:
                        return ActionType.Update;
                    case 4:
                        return ActionType.Delete;
                    case 5:
                        return ActionType.Search;
                    case 6:
                        return ActionType.Save;
                    case 7:
                        return ActionType.Load;
                    case 8:
                        return ActionType.Exit;
                    default:
                        Console.WriteLine("Invalid Choice, Try Again.\n");
                        return InventoryPrintMenu(); 
                }
            }
        } 

        private static ActionType CustomerPrintMenu()
        {
            // CRUD - Create, Read, Update, & Delete
            Console.WriteLine("------ Customer Menu ------\n");
            Console.WriteLine("1. Add Item to Cart");         // Add
            Console.WriteLine("2. Remove Item from Cart");    // Remove
            Console.WriteLine("3. List Current Inventory");   // ListInventory
            Console.WriteLine("4. Show Shopping Cart");       // ShowCart
            Console.WriteLine("5. Checkout");       // ShowCart
            Console.WriteLine("6. EXIT\n");                     // Exit
            Console.Write("Enter choice: ");
            var input = int.Parse(Console.ReadLine() ?? "0");

            while (true)
            {
                switch (input)
                {
                    case 1:
                        return ActionType.Add;
                    case 2:
                        return ActionType.Remove;
                    case 3:
                        return ActionType.ListInventory;
                    case 4:
                        return ActionType.ShowCart;
                    case 5:
                        return ActionType.Checkout;
                    case 6:
                        return ActionType.Exit;
                    default:
                        Console.WriteLine("Invalid Choice, Try Again.\n");
                        return CustomerPrintMenu();
                }
            }

        } // END Customer MENU

        private static Product UpdateProduct(Product? prod)
        {
           bool updateDone = false;
           if (prod != null)
           {
                while (updateDone == false) {
                    Console.WriteLine($"\nWhat would you like to update:\n\t1)Name\n\t2)Description\n\t3)Price Per Unit & Units Avalible 4)Make Product BOGO\n");
                    Console.Write("Enter choice: ");
                    var spot = int.Parse(Console.ReadLine() ?? "0");

                    switch (spot)
                    {
                        case 1: // change Name
                            Console.Write("\nEnter New Name: ");
                            string n = Console.ReadLine() ?? string.Empty;
                            if (n == null) { break; }
                            prod.Name = n;
                            break;
                        case 2: // change Description
                            Console.Write("\nEnter New Description: ");
                            string d = Console.ReadLine() ?? string.Empty;
                            if (d == null) { break; }
                            prod.Description = d;
                            break;
                        case 3: // change Price (per unit/0.10 lbs)
                            {
                                //var productbyquantity = product as ProductByQuantity;
                                if (prod is ProductByWeight)
                                {
                                    var productbyweight = prod as ProductByWeight;
                                    if (productbyweight != null)
                                    {
                                        Console.Write("\nEnter Weight of Product in lbs Avalible: ");
                                        productbyweight.Weight = double.Parse(Console.ReadLine() ?? "0.0");
                                        Console.Write("\nPrice Per lbs: ");
                                        productbyweight.Price = double.Parse(Console.ReadLine() ?? "0.0");
                                        Console.WriteLine($"\nProduct Changed:\nName = {prod.Name}\t Description = {prod.Description}\t Weight = {productbyweight.Weight}\t Price = ${productbyweight.Price}");
                                    }
                                    break;
                                }
                                else if (prod is ProductByQuantity)
                                {
                                    var productbyquantity = prod as ProductByQuantity;
                                    if (productbyquantity != null)
                                    {
                                        Console.Write("\nEnter Number of Units Avalible: ");
                                        productbyquantity.Quantity = int.Parse(Console.ReadLine() ?? "0");
                                        Console.Write("\nPrice Per Unit: ");
                                        productbyquantity.Price = double.Parse(Console.ReadLine() ?? "0");
                                        Console.WriteLine($"\nProduct Changed:\nName = {prod.Name}\t Description = {prod.Description}\t Quantity = {productbyquantity.Quantity}\t Price = ${productbyquantity.Price}");
                                    }
                                    break;
                                }
                                break;
                            }
                        case 4:
                            if (prod is ProductByQuantity)
                            {
                                var productbyquantity = prod as ProductByQuantity;
                                if (productbyquantity != null)
                                {
                                    Console.Write("\nWould you like to make Product BOGO:\n1) Yes\n2)No \nEnter choice: ");
                                    int bogodeal = int.Parse(Console.ReadLine() ?? "0");
                                    if (bogodeal == 1)   // Product is bogo
                                        productbyquantity.Bogo = true;  // else already set to false
                                }
                            }
                            else
                            {
                                Console.Write("\nProduct is unable to allow Bogo Pricing.");
                            }
                            break;

                        default:
                            Console.WriteLine("\nInvalid Choice.");
                            break;
                    }
                    Console.WriteLine($"\n\nWould you like to make more changes:\n\t1) Yes\n\t2) No");
                    Console.Write("\nEnter choice: ");

                    var x = int.Parse(Console.ReadLine() ?? "0");
                    if (x == 2) { updateDone = true; }
                }
            }
            return prod; 
        }


        // -------------------------   PRINT LISTS  -------------------------


        private static void PrintInventory(ProductServices inventory)   // Prints Inventory 
        {
            inventory.Navigator();
        }

        private static void PrintCart(CartService shoppingcart)     // Prints shopping cart
        {
            shoppingcart.Navigator();
        }

    } // END Iternal Program


    public enum ActionType
    {
        Create, ListInventory, Update, Delete, Exit, Add, Remove, ShowCart,
        Search, Save, Load, Checkout
      
    }

    public enum ProductByType
    {
        ProductByWeight, ProductByQuantity
    }

} 
