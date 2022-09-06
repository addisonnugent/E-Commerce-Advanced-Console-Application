using System;

namespace Library.ECommerceApp
{
    public class Product
    {
    
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int AssignedUser { get; set; }
        public double priceinp;

        public Product()
        {

        }

        public override string ToString()
        {
            return $"{Id}\t{Name}\t \t{Description}"; 
        }
    }
}   
