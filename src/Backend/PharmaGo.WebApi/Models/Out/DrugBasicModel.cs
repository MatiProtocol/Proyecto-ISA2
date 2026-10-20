using PharmaGo.Domain.Entities;

namespace PharmaGo.WebApi.Models.Out
{
    public class DrugBasicModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int Stock { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Symptom { get; set; }
        public PharmacyBasicModel Pharmacy { get; set; }

        public DrugBasicModel(Drug drug)
        {
            Id = drug.Id;
            Code = drug.Code;
            Stock = drug.Stock;
            Name = drug.Name;
            Symptom = drug.Symptom;
            Price = drug.Price;
            Pharmacy = new PharmacyBasicModel(drug.Pharmacy);
        }
    }
}
