using System;

namespace Library.ECommerceApp.Models
{
	public partial class ProductByQuantity : Product
	{
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double TotalPrice { get; set; }
        public Boolean Bogo { get; set; }

        public ProductByQuantity()
        {
            priceinp = Price;
            Bogo = false;
        }
       

        public override string ToString()
        {
            return $"{Id}\t{Name}\t\t$ {Price}\t\t\t{Quantity} units avalible\t{Description}\tBOGO = {Bogo}";
        }


        public double getTotalPrice()
        {
            if(Bogo == true && Quantity >= 2)       // if bogo calculate quantity/2
            {
                if(Quantity%2 == 0) // even
                    TotalPrice = (Quantity/2) * Price;
                else    // odd
                {
                    TotalPrice = ((Quantity - 1) / 2) * Price;  
                    TotalPrice += Price;
                }
            }
            else{
                TotalPrice = Quantity * Price;
            }

            return TotalPrice;
        }
    }
}
