using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Domain.Models;

public class Company
{

	[Key]
	public int Id { get; set; }

	[Required]
	[DisplayName("Name")]
	public string Name { get; set; }

	[DisplayName("Street Address")]
	public string? StreetAddress { get; set; }


	[DisplayName("City")]
	public string City { get; set; }


	[DisplayName("Postal Code")]
	public string PostalCode { get; set; }


	[DisplayName("Phone Number")]
	public string PhoneNumber { get; set; }
}
