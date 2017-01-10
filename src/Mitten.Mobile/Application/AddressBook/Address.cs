namespace Mitten.Mobile.Application.AddressBook
{
    /// <summary>
    /// Represents a physical mailing address.
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Gets or sets the name and number of the street address.
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the province or state.
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// Gets or sets the postal or zip code.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        public string Country { get; set; }
    }
}