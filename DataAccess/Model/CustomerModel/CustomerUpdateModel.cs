using System;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Model.CustomerModel
{
    public class CustomerCreateReqModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string? Gender { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateTime Dob { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "License plates are required.")]
        public string? LicensePlates { get; set; }

       
        public string? Role { get; set; }

        
        public string? Status { get; set; }

        [Required(ErrorMessage = "Created at date is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "Invalid date format.")]
        public DateTime? CreatedAt { get; set; }

        [Required(ErrorMessage = "Citizen ID number is required.")]
        public string? CitizenIdNumber { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        [CustomEndDateValidation(ErrorMessage = "End date must be in the future.")]
        public DateTime EndDate { get; set; }

        public Guid RoomId { get; set; }
    }

    public class RoomUserCreateReqModel
    {
        [Required(ErrorMessage = "Room ID is required.")]
        public Guid RoomId { get; set; }

        [Required(ErrorMessage = "Customer ID is required.")]
        public Guid CustomerId { get; set; }
    }

    public class CustomerUpdateModel
    {
        [Required(ErrorMessage = "ID is required.")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string? Gender { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateTime? Dob { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "LicensePlates is required.")]

        public string? LicensePlates { get; set; }


        public string? Status { get; set; }

        [Required(ErrorMessage = "CitizenIdNumber is required.")]

        public string? CitizenIdNumber { get; set; }
    }

    public class CustomEndDateValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime endDate)
            {
                if (endDate < DateTime.Now)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            return ValidationResult.Success!;
        }
    }
}
