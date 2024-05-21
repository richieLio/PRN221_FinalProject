namespace DataAccess.Model.CustomerModel
{
    public class CustomerResModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string Dob { get; set; }
        public string FullName { get; set; }
        public string LicensePlates { get; set; }
        public string CreatedAt { get; set; }
        public string CitizenIdNumber { get; set; }
    }
}